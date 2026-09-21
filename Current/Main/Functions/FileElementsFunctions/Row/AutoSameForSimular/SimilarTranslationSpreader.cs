using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    /// <summary>
    /// One spread: carries the translation of one row over to every row of the project whose original
    /// is the same line but for its numbers and symbols.
    /// <para>
    /// One instance does one spread, so the row it reads and everything derived from it are fields of
    /// the instance rather than parameters threaded through the scan.
    /// </para>
    /// <para>
    /// It owns the walking of the project content and the decision taken per row. It does not own the
    /// rules (<see cref="SimilarTextRules"/>), nor the writing (the <see cref="IUiUpdater"/> it is
    /// given, which is what makes the write reach the grid on the right thread), nor whether it may
    /// run at all (<see cref="ProjectReadiness"/>). Nothing here knows WinForms, so a caller can hand
    /// it any writer.
    /// </para>
    /// </summary>
    internal sealed class SimilarTranslationSpreader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ProjectBase _project;
        private readonly IUiUpdater _writer;

        private readonly int _tableIndex;
        private readonly int _rowIndex;
        private readonly bool _force;

        /// <summary>
        /// Index of the translation column, read once: it is asked for on every row of every file.
        /// </summary>
        private int _translationColumnIndex;

        /// <summary>
        /// The row being spread from, and everything about it the scan compares against. Filled by
        /// <see cref="TryReadInputRow"/>; the spread does not start when it returns false.
        /// </summary>
        private DataTable _inputTable;
        private string _inputOriginal;
        private string _inputOriginalNormalized;
        private string _inputOriginalWithoutMatches;
        private string _inputTranslation;
        private string _inputTranslationNormalized;
        private MatchCollection _inputOriginalMatches;

        /// <summary>
        /// The project's index of rows holding exactly the input original, and the rows it gave back.
        /// Null or false when the project has no such index, which is the normal case.
        /// </summary>
        private DuplicateOriginalIndex _duplicates;
        private ConcurrentDictionary<string, ConcurrentSet<int>> _duplicateRows;
        private bool _hasDuplicateRows;

        internal SimilarTranslationSpreader(ProjectBase project, IUiUpdater writer, int tableIndex, int rowIndex, bool force)
        {
            _project = project;
            _writer = writer;
            _tableIndex = tableIndex;
            _rowIndex = rowIndex;
            _force = force;
        }

        /// <summary>
        /// Runs the spread.
        /// <para>
        /// Does nothing, quietly, when the project may not be worked on or the row has nothing to
        /// spread. A failure is logged rather than thrown: this runs on a background thread, where an
        /// exception would have no caller to reach and would only end up unobserved.
        /// </para>
        /// </summary>
        internal void Run()
        {
            try
            {
                RunCore();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to spread the translation of row {0} of table {1}", _rowIndex, _tableIndex);
            }
        }

        private void RunCore()
        {
            // Asked again here and not only where the spread was requested: a database load can start
            // between the two, and then the content is being replaced under this scan.
            if (!ProjectReadiness.IsReady) return;

            if (!TryReadInputRow()) return;

            if (_hasDuplicateRows)
                SpreadToRowsHoldingTheSameLine();

            SpreadToSimilarRows();
        }

        /// <summary>
        /// Reads the row the spread starts from and works out everything about it the scan needs.
        /// </summary>
        /// <returns>False when the row cannot be read or holds nothing to spread.</returns>
        private bool TryReadInputRow()
        {
            if (_project == null) return false;

            var filesContent = _project.FilesContent;
            if (filesContent == null) return false;

            var tables = filesContent.Tables;
            if (_tableIndex < 0 || _tableIndex >= tables.Count) return false;

            var table = tables[_tableIndex];
            if (_rowIndex < 0 || _rowIndex >= table.Rows.Count) return false;

            _translationColumnIndex = _project.TranslationColumnIndex;
            _inputTable = table;

            var row = table.Rows[_rowIndex];
            _inputOriginal = row.Field<string>(_project.OriginalColumnIndex);
            _inputTranslation = row.Field<string>(_translationColumnIndex);

            // Nothing to spread from: the line has no translation yet, or its translation is still the
            // original, which means it was never translated.
            if (string.IsNullOrEmpty(_inputTranslation) || string.Equals(_inputTranslation, _inputOriginal)) return false;

            // Full-width and circled digits are the same digits: a line typed with one kind has to
            // compare equal to the same line typed with the other. Only the input side is normalized,
            // so the digits a target line already has keep the form they were written in.
            _inputOriginalNormalized = FunctionsRomajiKana.ReplaceDigits(_inputOriginal);
            _inputTranslationNormalized = FunctionsRomajiKana.ReplaceDigits(_inputTranslation);

            // A translation that is only whitespace or a line break is a broken result of an online
            // service. It holds no match to carry over, and comparing it with the original is what
            // used to run past the end of an empty match list.
            if (string.IsNullOrWhiteSpace(_inputTranslationNormalized) || _inputTranslationNormalized == Environment.NewLine)
                return false;

            // A Japanese original does not keep its number of digit runs in translation: 万 is one
            // match in "\{\V[11] \}万円手に入れた！" and "10000" is one in the English line, but the two
            // are not the same line, and carrying the numbers over would replace 10000 by 万.
            if (AppSettings.IsJapaneseSourceLanguage
                && SimilarTextRules.DigitRunCount(_inputTranslationNormalized) != SimilarTextRules.DigitRunCount(_inputOriginalNormalized))
                return false;

            _inputOriginalMatches = SimilarTextRules.Matches(_inputOriginalNormalized);
            _inputOriginalWithoutMatches = SimilarTextRules.WithoutMatches(_inputOriginalNormalized);

            _duplicates = DuplicateOriginalIndex.Of(_project);
            _hasDuplicateRows = _duplicates != null && _duplicates.TryGetRows(_inputOriginal, out _duplicateRows);

            return true;
        }

        /// <summary>
        /// Writes the translation to every row the project recorded as holding exactly this original.
        /// <para>
        /// The project collected those coordinates while it parsed its files, so this reaches rows of
        /// every file without scanning them, and it is the path that handles repeated lines when the
        /// project keeps an index of them.
        /// </para>
        /// </summary>
        private void SpreadToRowsHoldingTheSameLine()
        {
            var inputTableName = _inputTable.TableName;
            var tables = _project.FilesContent.Tables;

            foreach (var rowsOfTable in _duplicateRows)
            {
                var tableName = rowsOfTable.Key;
                if (!tables.Contains(tableName)) continue;

                var table = tables[tableName];
                int rowsCount = table.Rows.Count;

                foreach (var targetRowIndex in rowsOfTable.Value)
                {
                    if (targetRowIndex < 0 || targetRowIndex >= rowsCount) continue;

                    // The row being spread from is not a target of its own spread.
                    if (tableName == inputTableName && targetRowIndex == _rowIndex) continue;

                    var targetRow = table.Rows[targetRowIndex];

                    // Without force only a row that has nothing yet is written, so a translation that
                    // was already made is never replaced.
                    if (!_force && !string.IsNullOrEmpty(targetRow.Field<string>(_translationColumnIndex))) continue;

                    Write(targetRow, _inputTranslation);
                }
            }
        }

        /// <summary>
        /// Scans every row of every file for a line that is the same as the one being spread from but
        /// for its numbers and symbols, and gives it the same translation with those numbers and
        /// symbols replaced.
        /// </summary>
        private void SpreadToSimilarRows()
        {
            // Without a number or symbol in the original there is nothing to carry over, so no line
            // can be recognised as similar by this path. The rows holding exactly the same line were
            // already written above.
            if (_inputOriginalMatches.Count == 0) return;

            var tables = _project.FilesContent.Tables;
            int tablesCount = tables.Count;

            for (int targetTableIndex = 0; targetTableIndex < tablesCount; targetTableIndex++)
            {
                // The application is closing: the grid is going away, and further writes would only
                // be work done for a window that is no longer there.
                if (AppSettings.IsTranslationHelperWasClosed) return;

                var targetTable = tables[targetTableIndex];
                int rowsCount = targetTable.Rows.Count;

                for (int targetRowIndex = 0; targetRowIndex < rowsCount; targetRowIndex++)
                {
                    if (AppSettings.IsTranslationHelperWasClosed) return;

                    // The row being spread from is not a target of its own spread.
                    if (targetTableIndex == _tableIndex && targetRowIndex == _rowIndex) continue;

                    // Rows holding exactly this line were already written from the project
                    // coordinates, so they are not looked at again here.
                    if (_hasDuplicateRows
                        && _duplicateRows.TryGetValue(targetTable.TableName, out var alreadyWritten)
                        && alreadyWritten.Contains(targetRowIndex))
                    {
                        continue;
                    }

                    var targetRow = targetTable.Rows[targetRowIndex];

                    SpreadToRow(
                        targetRow,
                        targetRow.Field<string>(_project.OriginalColumnIndex),
                        targetRow.Field<string>(_translationColumnIndex));
                }
            }
        }

        /// <summary>
        /// Decides what one candidate row gets, and writes it.
        /// <para>
        /// A row is a target when its original is the same line but for the numbers and symbols, and
        /// when its translation is either still missing or the same as the one being spread from apart
        /// from those numbers. That second condition is what keeps a translation that was made
        /// deliberately — "the second book" for "книга 2" — from being replaced by "book 2".
        /// </para>
        /// </summary>
        private void SpreadToRow(DataRow targetRow, string targetOriginal, string targetTranslation)
        {
            // Without force, a row whose translation is still its original is left alone: the user
            // asked for those to be skipped, and this is not the operation that fills them.
            // Compared null-safely: a cell without a value reads back as null, not as an empty string.
            if (!_force && string.Equals(targetTranslation, targetOriginal)) return;

            // A row with nothing to show is untranslated in all but name, and gets the translation
            // derived below without having to match anything first.
            bool targetIsUntranslated = string.IsNullOrEmpty(targetTranslation) || string.Equals(targetTranslation, targetOriginal);

            // The exact line: nothing to derive, the translation is the one being spread. Only usable
            // while the project coordinates were not consulted for this line, because those already
            // covered every row holding it.
            if (_inputOriginal != null
                && (targetIsUntranslated || _force)
                && !_hasDuplicateRows
                && string.Equals(targetOriginal, _inputOriginalNormalized))
            {
                Write(targetRow, _inputTranslation);
                return;
            }

            // A line that packs several lines of the project into one is handled by the
            // multi-extraction path, which replaces each packed line by its own translation. It needs
            // the row's translation to read, so it only applies to a row that has one.
            if (!string.IsNullOrEmpty(targetTranslation)
                && ParsedWithExtractMulti(targetOriginal, targetTranslation, targetRow))
            {
                return;
            }

            // Nothing to substitute: the line being spread from has no number or symbol, or this one
            // has none of its own.
            if (_inputOriginalMatches.Count == 0) return;

            var targetOriginalMatches = SimilarTextRules.Matches(targetOriginal);
            if (targetOriginalMatches.Count == 0) return;

            // A translated row is only rewritten when its translation is the same as the one being
            // spread from once the numbers are removed. A row that has nothing yet has no translation
            // to protect, so the test does not apply to it.
            if (!targetIsUntranslated
                && SimilarTextRules.WithoutMatches(targetTranslation) != SimilarTextRules.WithoutMatches(_inputTranslationNormalized))
            {
                return;
            }

            // The originals have to be the same line, with the same numbers in the same places.
            if (!SimilarTextRules.AreSimilar(_inputOriginalWithoutMatches, _inputOriginalMatches, targetOriginal, targetOriginalMatches))
                return;

            // The translation being spread from has to have as many numbers and symbols as its
            // original, or there is no one-to-one relation to substitute along.
            var inputTranslationMatches = SimilarTextRules.Matches(_inputTranslationNormalized);
            if (inputTranslationMatches.Count != _inputOriginalMatches.Count) return;

            var derivedTranslation = SimilarTextRules.TranslateLike(
                _inputTranslationNormalized, inputTranslationMatches, targetOriginalMatches);

            // Only write when the cell would actually change. For a row with nothing to show that is
            // always the case, because a value derived from a non-empty translation is not empty
            // either; for a row that already holds it, writing it again would only raise a change
            // event for a value that did not change, and every one of those starts the operation over.
            if (string.Equals(derivedTranslation, targetTranslation)) return;

            Write(targetRow, derivedTranslation);
        }

        /// <summary>
        /// Try to translate using the multi-extraction method: a line that holds several of the
        /// project's lines packed into one is taken apart, and the part that is the original being
        /// spread from is replaced by its translation.
        /// </summary>
        /// <param name="targetOriginal">Original of the row being considered.</param>
        /// <param name="targetTranslation">Translation of that row.</param>
        /// <param name="targetRow">The row, written when a part of it was replaced.</param>
        /// <returns>True when the row was changed, and the caller has nothing more to do for it.</returns>
        private bool ParsedWithExtractMulti(string targetOriginal, string targetTranslation, DataRow targetRow)
        {
            // A cell without a value reads back as null, and the extraction returns null for a null
            // line rather than an empty result, so this is asked before its result is measured.
            if (string.IsNullOrEmpty(targetOriginal)) return false;

            var targetOriginalIndexes = new List<int>();
            var extractedTargetOriginal = targetOriginal.ExtractMulty(outIndexes: targetOriginalIndexes);

            int extractedTargetOriginalLength = extractedTargetOriginal.Length;
            int targetOriginalIndexesCount = targetOriginalIndexes.Count;

            // Not packed: the extraction returned the line itself, or returned as many parts as it did
            // indexes, which means nothing was taken apart.
            if (extractedTargetOriginalLength == 0
                || extractedTargetOriginal[0] == targetOriginal
                || targetOriginalIndexesCount != extractedTargetOriginalLength)
            {
                return false;
            }

            var targetTranslationIndexes = new List<int>();
            string[] extractedTargetTranslation = targetTranslation.ExtractMulty(outIndexes: targetTranslationIndexes);
            if (extractedTargetTranslation == null) return false;

            int extractedTargetTranslationLength = extractedTargetTranslation.Length;

            // The two sides have to come apart into the same number of parts, or there is no part of
            // the original to find on the translation side.
            if (extractedTargetTranslationLength == 0
                || extractedTargetTranslation[0] == targetTranslation
                || extractedTargetTranslationLength != targetTranslationIndexes.Count
                || extractedTargetTranslationLength != extractedTargetOriginalLength)
            {
                return false;
            }

            bool replaced = false;

            // Last part first, so replacing one does not move the positions of the parts still to come.
            for (int partIndex = targetOriginalIndexesCount - 1; partIndex >= 0; partIndex--)
            {
                if (!string.Equals(_inputOriginal, extractedTargetOriginal[partIndex])) continue;

                replaced = true;

                targetTranslation = targetTranslation
                    .Remove(targetTranslationIndexes[partIndex], extractedTargetTranslation[partIndex].Length)
                    .Insert(targetTranslationIndexes[partIndex], _inputTranslation);
            }

            if (!replaced) return false;

            Write(targetRow, targetTranslation);

            return true;
        }

        /// <summary>
        /// Writes a translation through the updater rather than into the row directly.
        /// <para>
        /// The row belongs to a table the grid may be bound to, and while the "[ALL]" entry is the
        /// displayed one the write also has to reach the view that entry presents. Both only happen on
        /// the thread that owns the grid, and this runs on a background thread: writing the row
        /// directly is what makes the "[ALL]" entry keep showing the value it had, and what makes a
        /// bound grid fail on a cross-thread change.
        /// </para>
        /// </summary>
        private void Write(DataRow targetRow, string translation)
        {
            _writer.SetTranslation(targetRow, _translationColumnIndex, translation);
        }
    }
}

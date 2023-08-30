using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Data.Interfaces;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;
using static TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslateNew;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    partial class OnlineTranslateNew : RowBase
    {
        protected override bool IsParallelTables => false;
        protected override bool IsParallelRows => false;

        // full row string
        public Dictionary<string, RowTranslationInfo> TranslationDataList = new Dictionary<string, RowTranslationInfo>();

        /// <summary>
        /// buffer for lines
        /// </summary>
        internal class Buffer : IOriginalTranslationUser
        {
            public string Original { get; }
            public string Translation { get; set; }
            internal bool IsExtracted { get; }

            internal Buffer(string original, string translation, bool isExtracted)
            {
                Original = original;
                Translation = translation;
                IsExtracted = isExtracted;
            }
        }

        internal class TranslationData
        {
            internal int TableIndex;
            internal List<RowTranslationData> Rows = new List<RowTranslationData>();
        }

        internal class RowTranslationData
        {
            internal bool IsAllLinesAdded = false;
            internal int RowIndex;
            internal List<LineTranslationData> Lines = new List<LineTranslationData>();
        }

        internal class LineTranslationData : IOriginalTranslationUser
        {
            internal readonly int LineIndex;
            public string Original { get; }
            public string Translation { get; set; }
            internal ExtractRegexInfo RegexExtractionData;

            public LineTranslationData(int lineIndex, string originalText)
            {
                LineIndex = lineIndex;
                Original = originalText;
                RegexExtractionData = new ExtractRegexInfo(Original);
            }
        }

        List<TranslationData> _buffer;

        int Size { get; set; }
        static int MaxSize { get => 1000; }

        bool IsMax()
        {
            return Size >= MaxSize;
        }

        public OnlineTranslateNew()
        {
            if (_buffer == null) _buffer = new List<TranslationData>();
            if (_translator == null) _translator = new GoogleAPIOLD();
        }

        protected override bool IsValidRow(Row.RowBaseRowData rowData)
        {
            return !AppSettings.InterruptTtanslation && base.IsValidRow(rowData)
                && (string.IsNullOrEmpty(rowData.Translation)
                || rowData.Original.HasAnyTranslationLineValidAndEqualSameOrigLine(rowData.Translation));
        }

        /// <summary>
        /// true if all DB was loaded
        /// </summary>
        bool _allDbLoaded4All;
        protected override void ActionsInit()
        {
            FunctionsOnlineCache.Init();

            if (_allDbLoaded4All || !IsAll || !AppSettings.UseAllDBFilesForOnlineTranslationForAll) return;

            if (!AppSettings.EnableTranslationCache)
            {
                //cache disabled but all db loading enabled. ask for load then. maybe not need
                var result = MessageBox.Show(T._("Translation cache disabled but load all DB enabled. While all DB loading cache can be enabled in settings. Load all DB?"), T._("Translation cache disabled"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            AppData.Main.ProgressInfo(true, "Get all DB");

            var mergingAllDb = new Task(() => FunctionsDBFile.MergeAllDBtoOne());
            mergingAllDb.ConfigureAwait(true);
            mergingAllDb.Start();
            mergingAllDb.Wait();
            _allDbLoaded4All = true;

            AppData.Main.ProgressInfo(false);
        }
        protected override void ActionsFinalize()
        {
            TranslateStrings();
            Size = 0;
            _buffer = null;
            FunctionsOnlineCache.Unload();

            AppData.Main.ProgressInfo(false);

            if (AppSettings.InterruptTtanslation) AppSettings.InterruptTtanslation = false;
        }

        protected override bool Apply(Row.RowBaseRowData rowData)
        {
            try
            {
                AppData.Main.ProgressInfo(true, "Translate" + " " + rowData.SelectedTable.TableName + "/" + rowData.SelectedRowIndex);

                SetRowLinesToBuffer(rowData);

                AppData.Main.ProgressInfo(false);
                return true;
            }
            catch
            {
            }

            return false;
        }

        private void SetRowLinesToBuffer(RowBaseRowData rowData)
        {
            var originalText = rowData.Original;
            var lineNumber = 0;

            // Find the table data for the selected table index, or create a new one if it doesn't exist
            var selectedTableData = _buffer.FirstOrDefault(tableData => tableData.TableIndex == rowData.SelectedTableIndex);
            if (selectedTableData == null)
            {
                selectedTableData = new TranslationData
                {
                    TableIndex = rowData.SelectedTableIndex
                };
                _buffer.Add(selectedTableData);
            }

            // Find the row data for the selected row index under the selected table, or create a new one if it doesn't exist
            var selectedRowData = selectedTableData.Rows.FirstOrDefault(rowTranslationData => rowTranslationData.RowIndex == rowData.SelectedRowIndex);
            if (selectedRowData == null)
            {
                selectedRowData = new RowTranslationData();
                selectedRowData.RowIndex = rowData.SelectedRowIndex;
                selectedTableData.Rows.Add(selectedRowData);
            }

            var originalTextSize = Size;

            // Parse each line of the original text
            foreach (var line in originalText.SplitToLines())
            {
                // Find the line data for the current line under the selected row, or create a new one if it doesn't exist
                var lineCoordinates = rowData.SelectedTableIndex + "," + rowData.SelectedRowIndex;
                var lineData = selectedRowData.Lines.FirstOrDefault(data => data.LineIndex == lineNumber);

                // If the line data already exists and is not translatable, skip it and move to the next line
                if (lineData != null && string.IsNullOrEmpty(lineData.Translation) && lineData.Translation != lineData.Original)
                {
                    lineNumber++;
                    continue;
                }

                // If the line data does not exist, create a new one
                if (lineData == null)
                {
                    lineData = new LineTranslationData(lineNumber, line);
                    selectedRowData.Lines.Add(lineData);
                }

                // If the line is not valid for translation, add the original as the translation and move to the next line
                if (!line.IsValidForTranslation())
                {
                    lineData.Translation = line;
                    lineNumber++;
                    continue;
                }

                // Extract information from the line using regex
                var extractData = new ExtractRegexInfo(line);
                lineData.RegexExtractionData = extractData;
                var extractedValuesCount = extractData.ExtractedValuesList.Count;
                bool isExtracted = extractedValuesCount > 0;

                int skippedValuesCount = 0;

                // Parse each extracted value from the line
                foreach (var extractedValueData in extractData.ExtractedValuesList)
                {
                    // Get the translation from the translation cache, if available
                    var translationCache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(extractedValueData.Original);

                    if (!string.IsNullOrEmpty(translationCache))
                    {
                        skippedValuesCount++;
                        extractedValueData.Translation = translationCache; // add translation from cache if found
                    }
                    else if (extractedValueData.Original.IsSoundsText() || !extractedValueData.Original.IsValidForTranslation())
                    {
                        skippedValuesCount++;
                        extractedValueData.Translation = extractedValueData.Original; // original=translation when value is soundtext
                    }
                    else
                    {
                        Size += extractedValueData.Original.Length; // increase size of string for translation for check later
                    }
                }

                // If all extracted values were skipped, don't translate this line and move to the next one
                if (extractedValuesCount > 0 && skippedValuesCount == extractedValuesCount)
                {
                    lineNumber++;
                    continue;
                }

                if (AppData.OnlineTranslationCache == null)
                {
                    FunctionsOnlineCache.Init();
                }

                // If the line is not extracted and its translation is available in the cache, use the cached translation
                if (!isExtracted)
                {
                    var lineCache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(line);
                    if (!string.IsNullOrEmpty(lineCache))
                    {
                        lineData.Translation = lineCache;
                        lineNumber++;
                        continue;
                    }

                    Size += lineData.Original.Length; // increase size of string for translation for check later
                }

                // If we've reached the maximum size for translation, translate the strings and reset the size counter
                if (IsMax())
                {
                    TranslateStrings();

                    Size = 0;

                    // Write the translation cache periodically
                    AppData.OnlineTranslationCache.Write();

                    // Stop translating after the last pack of text if translation is interrupted
                    if (AppSettings.InterruptTtanslation) return;
                }

                lineNumber++;
            }

            // Mark the row data as having all lines added
            selectedRowData.IsAllLinesAdded = true;

            // If no text has been added for translation, clean up the data and return
            if (originalTextSize == Size)
            {
                if (WriteRowData(selectedRowData, selectedTableData.TableIndex))
                {
                    selectedRowData = null;
                    selectedTableData.Rows.Remove(selectedRowData);
                    if (selectedTableData.Rows.Count > 0) selectedTableData = null;
                    return;
                }
            }

            // If this is not the last row and fewer than 300 rows have been added to the buffer, return
            if (!rowData.IsLastRow && _buffer.Count < 300) return;

            // Translate the strings and reset the buffer
            TranslateStrings();
            Size = 0;
            _buffer = new List<TranslationData>();

            // Write the translation cache periodically
            AppData.OnlineTranslationCache.Write();
        }

        readonly GoogleAPIOLD _translator;
        /// <summary>
        /// get originals and translate them
        /// </summary>
        private void TranslateStrings()
        {
            var originals = GetOriginals();

            if (originals.Length == 0 && _buffer.Count == 0) return;

            var translated = TranslateOriginals(originals);

            SetTranslationsToBuffer(originals, translated);

            SetBufferToRows();
        }

        /// <summary>
        /// get valid unique originals from buffer
        /// </summary>
        /// <returns></returns>
        private string[] GetOriginals()
        {
            return EnumerateLineData(_buffer)
                .AsParallel()
                .Aggregate(
                    new ConcurrentBag<string>(),
                    (originalLines, lineData) =>
                    {
                        foreach (var originalTranslationData in EnumerateOriginalTranslation(lineData))
                        {
                            if (!originalLines.Contains(originalTranslationData.Original) && string.IsNullOrEmpty(originalTranslationData.Translation) && originalTranslationData.Original.IsValidForTranslation())
                            {
                                originalLines.Add(originalTranslationData.Original);
                            }
                        }

                        return originalLines;
                    }
                )
                .ToArray();
        }

        private static IEnumerable<IOriginalTranslationUser> EnumerateOriginalTranslation(LineTranslationData lineData)
        {
            if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0)
            {
                foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
                {
                    yield return value;
                }
            }
            else
            {
                yield return lineData;
            }
        }

        /// <summary>
        /// apply project specific pre actions like hide variables to original lines before they will be translated
        /// </summary>
        /// <param name="originalLines"></param>
        /// <returns></returns>
        private static string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            if (AppData.CurrentProject.HideVARSMatchCollectionsList?.Count > 0)
            {
                // Clear the collection of found matches
                AppData.CurrentProject.HideVARSMatchCollectionsList.Clear();
            }

            int numOriginalLines = originalLines.Length;

            var preTranslatedLines = new string[numOriginalLines];
            Array.Copy(originalLines, preTranslatedLines, numOriginalLines);

            for (int i = 0; i < numOriginalLines; i++)
            {
                var preTranslatedLine = AppData.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], null);
                if (!string.IsNullOrEmpty(preTranslatedLine))
                {
                    preTranslatedLines[i] = preTranslatedLine;
                }
            }

            return preTranslatedLines;
        }

        /// <summary>
        /// apply project specific post actions like unhide variables to original lines after they was translated
        /// </summary>
        /// <param name="originalLines"></param>
        /// <param name="translatedLines"></param>
        /// <returns></returns>
        private static string[] ApplyProjectPostTranslationAction(string[] originalLines, string[] translatedLines)
        {
            int numTranslatedLines = translatedLines.Length;
            for (int i = 0; i < numTranslatedLines; i++)
            {
                string translatedLine = translatedLines[i];

                var postTranslatedLine = AppData.CurrentProject.OnlineTranslationProjectSpecificPostTranslationAction(originalLines[i], translatedLine);
                if (!string.IsNullOrEmpty(postTranslatedLine) && postTranslatedLine != translatedLine)
                {
                    translatedLines[i] = postTranslatedLine;
                }
            }

            var hideVarsMatchCollections = AppData.CurrentProject.HideVARSMatchCollectionsList;
            if (hideVarsMatchCollections?.Count > 0)
            {
                // Clear the collection of found matches
                hideVarsMatchCollections.Clear();
            }

            return translatedLines;
        }

        //readonly GoogleTranslator translator = new GoogleTranslator();

        /// <summary>
        /// translate originals in selected translator
        /// </summary>
        /// <param name="originals"></param>
        /// <returns></returns>
        private string[] TranslateOriginals(string[] originals)
        {
            if (originals == null || originals.Length == 0) return Array.Empty<string>();

            string[] translated = Array.Empty<string>();
            try
            {
                var originalLinesArePreApplied = ApplyProjectPretranslationAction(originals);
                if (originalLinesArePreApplied.Length == 0) return translated;

                translated = _translator.Translate(originalLinesArePreApplied);
                if (translated == null || originals.Length != translated.Length) return Array.Empty<string>();

                translated = ApplyProjectPostTranslationAction(originals, translated);
            }
            catch (Exception ex)
            {
                _log.LogToFile("Error while translation:"
                    + Environment.NewLine
                    + ex
                    + Environment.NewLine
                    + "OriginalLines="
                    + string.Join(Environment.NewLine + "</br>" + Environment.NewLine, originals));
            }

            return translated;
        }

        /// <summary>
        /// set translations to buffer data
        /// </summary>
        /// <param name="originals"></param>
        /// <param name="translated"></param>
        private void SetTranslationsToBuffer(string[] originals, string[] translated)
        {
            if (originals == null || translated == null || originals.Length != translated.Length)
                return;

            var translations = new Dictionary<string, string>();
            for (int i = 0; i < originals.Length; i++)
            {
                var original = originals[i];
                var translation = translated[i];

                translations.Add(original, translation);

                FunctionsOnlineCache.TryAdd(original, translation);
            }

            Parallel.ForEach(EnumerateLineData(_buffer), lineData =>
            {
                foreach (var originalTranslation in EnumerateOriginalTranslation(lineData))
                {
                    if (!TryGetTranslation(translations, originalTranslation.Original, originalTranslation.Translation, out var v)) return;

                    originalTranslation.Translation = v;
                }
            });
        }

        private static bool TryGetTranslation(Dictionary<string, string> translations, string original, string translation, out string outTranslation)
        {
            outTranslation = default;

            if (translation != null && translation != original) return false;
            if (!original.IsValidForTranslation()) return false;
            if (!translations.TryGetValue(original, out var v)) return false;

            outTranslation = v;

            return true;
        }

        private IEnumerable<LineTranslationData> EnumerateLineData(List<TranslationData> buffer)
        {
            foreach (var data in _buffer) // get data
            {
                foreach (var rowData in data.Rows) // get rows data
                {
                    foreach (var lineData in rowData.Lines) // get line data
                    {
                        yield return lineData;
                    }
                }
            }
        }

        /// <summary>
        /// set buffer content to rows by coordinates
        /// </summary>
        private void SetBufferToRows()
        {
            for (int i = _buffer.Count - 1; i >= 0; i--)
            {
                var data = _buffer[i];
                var dataRows = data.Rows;
                for (int n = dataRows.Count - 1; n >= 0; n--)
                {
                    var rowData = dataRows[n];
                    if (WriteRowData(rowData, data.TableIndex))
                    {
                        dataRows.Remove(rowData);
                    }
                }

                if (dataRows.Count == 0) _buffer.Remove(data);
            }
        }

        private readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
        private readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
        private readonly RowBase _hardFixes = new AllHardFixes();
        private readonly RowBase _fixCells = new FixCells();
        private bool WriteRowData(RowTranslationData rowData, int tableIndex)
        {
            if (!rowData.IsAllLinesAdded) return false; // skip if row is not fully translated

            var row = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rowData.RowIndex];
            var ignoreOrigEqualTransLines = AppSettings.IgnoreOrigEqualTransLines;

            // skip equal
            var originalText = row.Field<string>(_originalColumnIndex);
            var translationText = row.Field<string>(_translationColumnIndex);
            var cellTranslationEqualOriginal = Equals(translationText, originalText);
            if (ignoreOrigEqualTransLines && cellTranslationEqualOriginal) return false;

            // skip when translation not equal to original and and have No any original line equal translation
            var cellTranslationIsNotEmptyAndNotEqualOriginal = !string.IsNullOrEmpty(translationText) && !cellTranslationEqualOriginal;
            if (cellTranslationIsNotEmptyAndNotEqualOriginal && !originalText.HasAnyTranslationLineValidAndEqualSameOrigLine(translationText, false)) return false;

            // set new row value
            row.SetValue(_translationColumnIndex, string.Join(Environment.NewLine, EnumerateNewLines(cellTranslationIsNotEmptyAndNotEqualOriginal ? translationText : originalText, rowData.Lines)));

            // apply fixes for cell
            // apply only for finished rows
            _hardFixes.Selected(row, tableIndex, rowData.RowIndex);
            _fixCells.Selected(row, tableIndex, rowData.RowIndex);

            return true;
        }

        private static IEnumerable<string> EnumerateNewLines(string rowValue, List<LineTranslationData> rowDataLines)
        {
            int lineNum = 0;
            foreach (var line in rowValue.SplitToLines())
            {
                var lineData = rowDataLines[lineNum++];

                if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0) // when line has extracted values
                {
                    yield return MergeExtracted(lineData);
                }
                else if (string.IsNullOrWhiteSpace(lineData.Translation) || lineData.Translation.Equals(line) || line.IsSoundsText())
                {
                    // when null,=original or issoundstext. jusst insert original line
                    yield return line;
                }
                else yield return lineData.Translation;
            }
        }

        static readonly Regex _replacerListTypeRegex = new Regex(@"^\$[0-9]+(,\$[0-9]+)+$", RegexOptions.Compiled);
        static readonly Regex _oneMatchNeedInsertTextRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Compiled);
        static readonly Regex _groupReplacerMarkerRegex = new Regex(@"\$[0-9]+", RegexOptions.Compiled);
        private static string MergeExtracted(LineTranslationData lineData)
        {
            // replace all groups with translation of selected value
            bool isMarked = false;
            var replacerMatches = _groupReplacerMarkerRegex.Matches(lineData.RegexExtractionData.Replacer);
            var matchesCount = replacerMatches.Count;
            var replacerType = TranslationRegexExtractType.ReplaceOne;
            bool isOneMatchNeedInsertText = matchesCount == 1 && _oneMatchNeedInsertTextRegex.IsMatch(lineData.RegexExtractionData.Replacer.Trim());
            if (!isOneMatchNeedInsertText)
            {
                replacerType = _replacerListTypeRegex.IsMatch(lineData.RegexExtractionData.Replacer.Trim())
                    ? TranslationRegexExtractType.ReplaceList
                    : TranslationRegexExtractType.Replacer;
            }

            var newLineText = new StringBuilder(lineData.Original);

            var list = lineData.RegexExtractionData.ExtractedValuesList;
            var maxValueDataIndex = list.Count - 1;
            for (int i = maxValueDataIndex; i >= 0; i--)
            {
                var valueData = list[i];
                // search all $1-$99 in replacer

                // just replace values for each group by translation
                if (replacerType != TranslationRegexExtractType.Replacer)
                {
                    var maxGroupIndex = valueData.MatchGroups.Count - 1;
                    for (int i1 = maxGroupIndex; i1 >= 0; i1--)
                    {
                        var group = valueData.MatchGroups[i1];

                        if (string.IsNullOrWhiteSpace(group.Value)) continue;
                        if (string.IsNullOrEmpty(valueData.Translation)) continue;

                        try
                        {
                            // replace original value text with translation
                            newLineText.Replace(group.Value
                                , valueData.Translation ?? group.Value
                                , group.Index, group.Length);
                        }
                        catch (IndexOutOfRangeException) 
                        { 
                        }
                        catch (ArgumentException) 
                        { 
                        }
                    }

                    if (replacerType == TranslationRegexExtractType.ReplaceOne) break; // exit from values loop, to not execute lines below
                    else continue; // continue for list
                }

                ////
                // Standart replacer parse
                //
                if (!isMarked)
                {
                    isMarked = true; // mark only one time

                    newLineText = new StringBuilder(lineData.RegexExtractionData.Replacer);

                    // mark all matches for precise replacement, $1 to %$1%
                    for (int i2 = matchesCount - 1; i2 >= 0; i2--)
                    {
                        var match = replacerMatches[i2];

                        newLineText.Replace(match.Value, $"%{match.Value}%", match.Index, match.Length);
                    }
                }
                //
                // replace group mark with translation
                foreach (var matchGroup in valueData.MatchGroups)
                {
                    newLineText.Replace($"%${matchGroup.Name}%", valueData.Translation ?? matchGroup.Value);
                }
                ////
            }

            return newLineText.ToString();
        }
    }
}

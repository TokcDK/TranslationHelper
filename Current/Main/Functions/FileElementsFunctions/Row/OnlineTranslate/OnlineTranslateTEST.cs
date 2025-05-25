using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Data.Interfaces;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    /// <summary>
    /// Handles online translation of rows, inheriting from RowBase.
    /// Manages buffering, translation, and writing back to rows.
    /// </summary>
    partial class OnlineTranslateTEST : RowBase
    {
        #region Fields

        /// <summary>
        /// List of translation info for rows (original -> translation info).
        /// </summary>
        public Dictionary<string, RowTranslationInfo> TranslationDataList = new Dictionary<string, RowTranslationInfo>();

        /// <summary>
        /// Currently used translator.
        /// </summary>
        private readonly ITranslator _translator;

        /// <summary>
        /// Translation cache for faster repeated requests.
        /// </summary>
        private readonly ITranslationCache _cache;

        /// <summary>
        /// Buffer for storing data of rows to be translated.
        /// </summary>
        private readonly ConcurrentDictionary<int, TranslationData> _buffer;

        /// <summary>
        /// Total length of text waiting for translation.
        /// </summary>
        private int TranslationTextLength { get; set; }

        /// <summary>
        /// Maximum text length for a single translation request.
        /// </summary>
        private static int MaxTranslationTextLength => 1000;

        /// <summary>
        /// Maximum number of rows in the buffer.
        /// </summary>
        private const int BufferMaxRows = 300;

        /// <summary>
        /// Flag indicating that all DBs were loaded for all translations.
        /// </summary>
        private bool _allDbLoaded4All;

        /// <summary>
        /// Name of the last processed table.
        /// </summary>
        private string _lastTableName = string.Empty;

        /// <summary>
        /// Class for applying "hard" fixes to strings.
        /// </summary>
        private readonly AllHardFixesChanger _hardFixes = new AllHardFixesChanger();

        /// <summary>
        /// Class for applying fixes to cells.
        /// </summary>
        private readonly FixCellsChanger _fixCells = new FixCellsChanger();

        /// <summary>
        /// Regex for determining replacer list type.
        /// </summary>
        private static readonly Regex _replacerListTypeRegex = new Regex(@"^\$[0-9]+(,\$[0-9]+)+$", RegexOptions.Compiled);

        /// <summary>
        /// Regex for determining if text insertion is needed for a single match.
        /// </summary>
        private static readonly Regex _oneMatchNeedInsertTextRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Compiled);

        /// <summary>
        /// Regex for finding group replacer markers.
        /// </summary>
        private static readonly Regex _groupReplacerMarkerRegex = new Regex(@"\$[0-9]+", RegexOptions.Compiled);

        #endregion

        #region Properties

        /// <summary>
        /// Name of the current translator.
        /// </summary>
        public override string Name => T._("TranslatorTEST");

        /// <summary>
        /// Flag: whether to use parallel processing for tables.
        /// </summary>
        protected override bool IsParallelTables => false;

        /// <summary>
        /// Flag: whether to use parallel processing for rows.
        /// </summary>
        protected override bool IsParallelRows => false;

        /// <summary>
        /// Flag: whether to translate all rows.
        /// </summary>
        protected virtual bool IsTranslateAll => true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of OnlineTranslateTEST with dependencies.
        /// </summary>
        /// <param name="translator">Translator instance (optional).</param>
        /// <param name="cache">Translation cache instance (optional).</param>
        public OnlineTranslateTEST(ITranslator translator = null, ITranslationCache cache = null)
        {
            Logger.Debug("Initializing OnlineTranslateTEST");
            _translator = translator ?? new GoogleTranslator(THSettings.SourceLanguageCode, THSettings.TargetLanguageCode);
            _cache = cache ?? new TranslationCache();
            _buffer = new ConcurrentDictionary<int, TranslationData>();
        }

        #endregion

        #region RowBase Overrides

        /// <summary>
        /// Checks if the row is valid for translation.
        /// </summary>
        /// <param name="rowData">Row data.</param>
        /// <returns>True if the row is valid for translation.</returns>
        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            Logger.Debug($"Checking if row is valid for translation: RowIndex={rowData?.SelectedRowIndex}");
            return !AppSettings.InterruptTtanslation && base.IsValidRow(rowData)
                && (string.IsNullOrEmpty(rowData.Translation)
                || rowData.Original.HasAnyTranslationLineValidAndEqualSameOrigLine(rowData.Translation));
        }

        /// <summary>
        /// Initializes resources for translation, e.g., loads DBs if needed.
        /// </summary>
        protected async override Task ActionsInit()
        {
            Logger.Info("Initializing actions for online translation.");
            await base.ActionsInit();

            if (_allDbLoaded4All || !IsAll || !AppSettings.UseAllDBFilesForOnlineTranslationForAll) return;

            if (!AppSettings.EnableTranslationCache)
            {
                Logger.Warn("Translation cache is disabled, but loading all DBs is enabled.");
                var result = MessageBox.Show(T._("Translation cache disabled but load all DB enabled. While all DB loading cache can be enabled in settings. Load all DB?"),
                    T._("Translation cache disabled"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            if (!IsTranslateAll && AppSettings.EnableTranslationCache)
            {
                Logger.Info("Request to load all existing DB files.");
                var result = MessageBox.Show(T._("Load all exist database files?"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            Logger.Info(T._("Get all DB"));
            await FunctionsDBFile.MergeAllDBtoOne().ConfigureAwait(false);
            _allDbLoaded4All = true;
        }

        /// <summary>
        /// Finalizes translation: processes remaining buffer and cleans up resources.
        /// </summary>
        protected override async Task ActionsFinalize()
        {
            Logger.Info("Finalizing translation.");
            await base.ActionsFinalize();

            if (!_buffer.IsEmpty)
            {
                Logger.Debug("There are unprocessed rows in the buffer, performing translation.");
                TranslateStrings();
            }
            _cache.Dispose();
            if (AppSettings.InterruptTtanslation)
            {
                Logger.Warn("Translation was interrupted by the user.");
                AppSettings.InterruptTtanslation = false;
            }
            Logger.Info(T._("Translation complete"));
        }

        /// <summary>
        /// Adds rows to the buffer for batch translation.
        /// </summary>
        /// <param name="rowData">Row data.</param>
        /// <returns>True if the row was successfully added to the buffer.</returns>
        protected override bool Apply(RowBaseRowData rowData)
        {
            if (_lastTableName != rowData.SelectedTable.TableName)
            {
                _lastTableName = rowData.SelectedTable.TableName;
                Logger.Info(T._("Translate {0}"), _lastTableName);
            }

            SetRowLinesToBuffer(rowData);
            return true;
        }

        #endregion

        #region Buffering Methods

        /// <summary>
        /// Checks if the buffer or text length limit is reached.
        /// </summary>
        /// <returns>True if the limit is reached.</returns>
        private bool IsMax()
        {
            bool isMax = TranslationTextLength >= MaxTranslationTextLength || _buffer.Count >= BufferMaxRows;
            if (isMax)
                Logger.Debug($"Buffer limit reached: TranslationTextLength={TranslationTextLength}, BufferCount={_buffer.Count}");
            return isMax;
        }

        /// <summary>
        /// Buffers rows for batch translation.
        /// </summary>
        /// <param name="rowData">Row data.</param>
        private void SetRowLinesToBuffer(RowBaseRowData rowData)
        {
            if (rowData == null)
            {
                Logger.Error("rowData is null when adding row to buffer.");
                throw new ArgumentNullException(nameof(rowData));
            }
            if (rowData.SelectedTable == null)
            {
                Logger.Error("SelectedTable is null when adding row to buffer.");
                throw new InvalidOperationException("SelectedTable is null");
            }
            if (rowData.Original == null)
            {
                Logger.Error("Original text is null when adding row to buffer.");
                throw new InvalidOperationException("Original text is null");
            }

            var tableData = _buffer.GetOrAdd(rowData.SelectedTableIndex, _ => new TranslationData(rowData));
            var rowDataEntry = tableData.Rows.GetOrAdd(rowData.SelectedRowIndex, _ => new RowTranslationData(rowData));

            int originalTextLength = TranslationTextLength;
            int lineIndex = -1;

            foreach (var line in rowData.Original.SplitToLines())
            {
                lineIndex++;
                var lineData = rowDataEntry.Lines.FirstOrDefault(l => l.LineIndex == lineIndex) ?? new LineTranslationData(lineIndex, line);
                if (!rowDataEntry.Lines.Contains(lineData)) rowDataEntry.Lines.Add(lineData);

                if (lineData.IsTranslated) continue;

                lineData.RegexExtractionData = new ExtractRegexInfo(line);
                bool isExtracted = lineData.RegexExtractionData.ExtractedValuesList.Count > 0;

                if (!isExtracted && !line.IsValidForTranslation())
                {
                    Logger.Debug($"Line excluded from translation: {line}");
                    lineData.IsExcluded = true;
                    lineData.Translation = line;
                    continue;
                }

                GetFromExtracted(lineData.RegexExtractionData, out int skippedCount);
                if (isExtracted && skippedCount == lineData.RegexExtractionData.ExtractedValuesList.Count) continue;

                if (!isExtracted && CheckInCache(line, lineData)) continue;

                if (IsMax())
                {
                    Logger.Info("Buffer is full, performing translation.");
                    TranslateStrings();
                }
                if (AppSettings.InterruptTtanslation) return;
            }

            rowDataEntry.IsAllLinesAdded = true;

            if (originalTextLength == TranslationTextLength && WriteRowData(rowDataEntry, tableData.TableIndex))
            {
                tableData.Rows.TryRemove(rowDataEntry.RowIndex, out _);
                if (tableData.Rows.IsEmpty) _buffer.TryRemove(tableData.TableIndex, out _);
            }
        }

        /// <summary>
        /// Checks the cache for a string and updates the line data if found.
        /// </summary>
        /// <param name="line">Original string.</param>
        /// <param name="lineData">Line data.</param>
        /// <returns>True if translation was found in cache.</returns>
        private bool CheckInCache(string line, LineTranslationData lineData)
        {
            var cached = _cache.TryGetValue(line);
            if (!string.IsNullOrEmpty(cached))
            {
                Logger.Debug($"Translation found in cache: {line} -> {cached}");
                lineData.Translation = cached;
                return true;
            }
            TranslationTextLength += line.Length;
            return false;
        }

        /// <summary>
        /// Processes extracted values, updating length or using cache.
        /// </summary>
        /// <param name="extractData">Extraction data.</param>
        /// <param name="skippedValuesCount">Number of skipped values.</param>
        private void GetFromExtracted(ExtractRegexInfo extractData, out int skippedValuesCount)
        {
            skippedValuesCount = 0;
            foreach (var value in extractData.ExtractedValuesList)
            {
                var cached = _cache.TryGetValue(value.Original);
                if (!string.IsNullOrEmpty(cached))
                {
                    Logger.Debug($"Extracted value found in cache: {value.Original} -> {cached}");
                    value.Translation = cached;
                    skippedValuesCount++;
                }
                else if (value.Original.IsSoundsText() || !value.Original.IsValidForTranslation())
                {
                    Logger.Debug($"Extracted value excluded from translation: {value.Original}");
                    value.Translation = value.Original;
                    skippedValuesCount++;
                }
                else
                {
                    TranslationTextLength += value.Original.Length;
                }
            }
        }

        #endregion

        #region Translation Methods

        /// <summary>
        /// Translates strings from the buffer and updates rows.
        /// </summary>
        private void TranslateStrings()
        {
            Logger.Info("Starting batch translation of strings from buffer.");
            var originals = GetOriginals();
            if (originals.Length == 0 && _buffer.IsEmpty)
            {
                Logger.Debug("No strings to translate.");
                return;
            }

            var translated = TranslateOriginals(originals);
            SetTranslationsToBuffer(originals, translated);
            SetBufferToRows();
            TranslationTextLength = 0;
            if (originals.Length > 1) _cache.Write();
        }

        /// <summary>
        /// Gets unique originals from the buffer for translation.
        /// </summary>
        /// <returns>Array of original strings.</returns>
        private string[] GetOriginals()
        {
            var originals = new ConcurrentDictionary<string, bool>();
            Parallel.ForEach(EnumerateBufferedLinesData(), line =>
            {
                foreach (var ot in EnumerateOriginalTranslation(line))
                    originals.TryAdd(ot.Original, true);
            });
            Logger.Debug($"Got {originals.Count} unique strings for translation.");
            return originals.Keys.ToArray();
        }

        /// <summary>
        /// Enumerates original texts that require translation.
        /// </summary>
        /// <param name="lineData">Line data.</param>
        /// <returns>Enumeration of objects for translation.</returns>
        private static IEnumerable<IOriginalTranslationUser> EnumerateOriginalTranslation(LineTranslationData lineData)
        {
            if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0)
            {
                foreach (var value in lineData.RegexExtractionData.ExtractedValuesList
                    .Where(v => !(v.IsExcluded = !v.Original.IsValidForTranslation()) && !v.IsTranslated))
                    yield return value;
            }
            else if (!lineData.IsTranslated && !(lineData.IsExcluded = !lineData.Original.IsValidForTranslation()))
            {
                yield return lineData;
            }
        }

        /// <summary>
        /// Enumerates all line data from the buffer.
        /// </summary>
        /// <returns>Enumeration of line data.</returns>
        private IEnumerable<LineTranslationData> EnumerateBufferedLinesData()
        {
            foreach (var table in _buffer.Values)
                foreach (var row in table.Rows.Values)
                    foreach (var line in row.Lines)
                        yield return line;
        }

        /// <summary>
        /// Translates originals with pre- and post-processing and fallback.
        /// </summary>
        /// <param name="originals">Original strings.</param>
        /// <returns>Translated strings.</returns>
        private string[] TranslateOriginals(string[] originals)
        {
            if (originals == null || originals.Length == 0) return Array.Empty<string>();

            var preApplied = ApplyProjectPretranslationAction(originals);
            var translated = TranslateWithFallback(preApplied);
            return ApplyProjectPostTranslationAction(originals, translated);
        }

        /// <summary>
        /// Translates texts with fallback in case of errors.
        /// </summary>
        /// <param name="texts">Texts to translate.</param>
        /// <returns>Translated strings.</returns>
        private string[] TranslateWithFallback(string[] texts)
        {
            try
            {
                Logger.Debug("Performing main translation of string array.");
                return _translator.Translate(texts);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Batch translation error: {ex}");
                var translated = new List<string>();
                const int batchSize = 4;
                for (int i = 0; i < texts.Length; i += batchSize)
                {
                    var batch = texts.Skip(i).Take(batchSize).ToArray();
                    try
                    {
                        translated.AddRange(_translator.Translate(batch));
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn($"Batch translation error: {ex2}");
                        foreach (var text in batch)
                        {
                            try
                            {
                                translated.Add(_translator.Translate(text));
                            }
                            catch (Exception ex3)
                            {
                                Logger.Error($"Error translating string: {text}", ex3);
                                translated.Add(string.Empty);
                            }
                        }
                    }
                }
                return translated.ToArray();
            }
        }

        /// <summary>
        /// Applies project-specific pre-processing to original strings.
        /// </summary>
        /// <param name="originalLines">Original strings.</param>
        /// <returns>Strings after pre-processing.</returns>
        private static string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            var preTranslated = new string[originalLines.Length];
            Array.Copy(originalLines, preTranslated, originalLines.Length);
            for (int i = 0; i < originalLines.Length; i++)
            {
                var result = AppData.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], null);
                if (!string.IsNullOrEmpty(result)) preTranslated[i] = result;
            }
            return preTranslated;
        }

        /// <summary>
        /// Applies project-specific post-processing to translated strings.
        /// </summary>
        /// <param name="originalLines">Original strings.</param>
        /// <param name="translatedLines">Translated strings.</param>
        /// <returns>Strings after post-processing.</returns>
        private static string[] ApplyProjectPostTranslationAction(string[] originalLines, string[] translatedLines)
        {
            for (int i = 0; i < translatedLines.Length; i++)
            {
                var result = AppData.CurrentProject.OnlineTranslationProjectSpecificPostTranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(result) && result != translatedLines[i]) translatedLines[i] = result;
            }
            return translatedLines;
        }

        /// <summary>
        /// Assigns translations back into the buffer.
        /// </summary>
        /// <param name="originals">Original strings.</param>
        /// <param name="translated">Translated strings.</param>
        private void SetTranslationsToBuffer(string[] originals, string[] translated)
        {
            if (originals == null || translated == null || originals.Length != translated.Length) return;

            var translations = originals.Zip(translated, (o, t) => new { Original = o, Translation = t })
                .ToDictionary(x => x.Original, x => x.Translation);

            Parallel.ForEach(EnumerateBufferedLinesData(), line =>
            {
                foreach (var ot in EnumerateOriginalTranslation(line))
                    if (translations.TryGetValue(ot.Original, out var trans))
                    {
                        ot.Translation = trans;
                        _cache.TryAdd(ot.Original, trans);
                    }
            });
        }

        /// <summary>
        /// Writes translations from the buffer back to rows.
        /// </summary>
        private void SetBufferToRows()
        {
            var tablesToRemove = new List<int>();
            foreach (var table in _buffer)
            {
                var rowsToRemove = new List<int>();
                foreach (var row in table.Value.Rows)
                {
                    if (WriteRowData(row.Value, table.Key))
                        rowsToRemove.Add(row.Key);
                }
                foreach (var rowIndex in rowsToRemove)
                    table.Value.Rows.TryRemove(rowIndex, out _);
                if (table.Value.Rows.IsEmpty)
                    tablesToRemove.Add(table.Key);
            }
            foreach (var tableIndex in tablesToRemove)
                _buffer.TryRemove(tableIndex, out _);
        }

        #endregion

        #region Row Writing and Merging

        /// <summary>
        /// Writes translated data back to the row if translation is complete.
        /// </summary>
        /// <param name="rowData">Row data.</param>
        /// <param name="tableIndex">Table index.</param>
        /// <returns>True if the write was performed.</returns>
        private bool WriteRowData(RowTranslationData rowData, int tableIndex)
        {
            if (!rowData.IsAllLinesAdded) return false;

            var row = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rowData.RowIndex];
            var original = rowData.Row.Original;
            var translation = rowData.Row.Translation;
            var ignoreEqual = AppSettings.IgnoreOrigEqualTransLines;

            if (ignoreEqual && string.Equals(translation, original) && !rowData.Lines.Any(l => l.Original != l.Translation)) return false;

            var transNotEmptyAndNotEqual = !string.IsNullOrEmpty(translation) && !string.Equals(translation, original);
            if (transNotEmptyAndNotEqual && !original.HasAnyTranslationLineValidAndEqualSameOrigLine(translation, false)) return false;

            rowData.Row.Translation = string.Join(Environment.NewLine, EnumerateNewLines(rowData.Lines));
            Logger.Debug($"Row written: RowIndex={rowData.RowIndex}, TableIndex={tableIndex}");
            return true;
        }

        /// <summary>
        /// Generates translated lines for writing back.
        /// </summary>
        /// <param name="lines">List of line data.</param>
        /// <returns>Enumeration of translated lines.</returns>
        private IEnumerable<string> EnumerateNewLines(List<LineTranslationData> lines)
        {
            foreach (var line in lines)
            {
                if (line.RegexExtractionData.ExtractedValuesList.Count > 0)
                    yield return MergeExtracted(line);
                else if (line.IsTranslated)
                    yield return string.IsNullOrEmpty(line.Translation) ? line.Original : ApplyFixes(line);
            }
        }

        /// <summary>
        /// Applies fixes to the translation of a line.
        /// </summary>
        /// <param name="line">Line data.</param>
        /// <returns>Fixed translation.</returns>
        private string ApplyFixes(LineTranslationData line) => ApplyFixes(line.Original, line.Translation);

        /// <summary>
        /// Applies fixes to the translation.
        /// </summary>
        /// <param name="original">Original.</param>
        /// <param name="translation">Translation.</param>
        /// <returns>Fixed translation.</returns>
        private string ApplyFixes(string original, string translation)
        {
            var text = _hardFixes.Change(translation, original);
            return _fixCells.Change(text, original);
        }

        /// <summary>
        /// Merges extracted values back into the string.
        /// </summary>
        /// <param name="lineData">Line data.</param>
        /// <returns>Merged string.</returns>
        private string MergeExtracted(LineTranslationData lineData)
        {
            var extractData = lineData.RegexExtractionData;
            if (extractData.ExtractedValuesList.Count == 0) return lineData.Translation;

            var replacerType = DetermineReplacerType(extractData.Replacer);
            switch (replacerType)
            {
                case TranslationRegexExtractType.ReplaceOne:
                    return MergeReplaceOne(lineData);
                case TranslationRegexExtractType.ReplaceList:
                    return MergeReplaceList(lineData);
                case TranslationRegexExtractType.Replacer:
                    return MergeReplacer(lineData);
                default:
                    Logger.Error("Unknown replacer type when merging string.");
                    throw new InvalidOperationException("Unknown replacer type");
            }
        }

        /// <summary>
        /// Determines the replacer type for merging.
        /// </summary>
        /// <param name="replacer">Replacer string.</param>
        /// <returns>Replacer type.</returns>
        private static TranslationRegexExtractType DetermineReplacerType(string replacer)
        {
            var trimmed = replacer.Trim();
            if (_oneMatchNeedInsertTextRegex.IsMatch(trimmed)) return TranslationRegexExtractType.ReplaceOne;
            if (_replacerListTypeRegex.IsMatch(trimmed)) return TranslationRegexExtractType.ReplaceList;
            return TranslationRegexExtractType.Replacer;
        }

        /// <summary>
        /// Merges using the ReplaceOne strategy: replaces the entire match with the translation of the first group.
        /// </summary>
        /// <param name="lineData">Line data.</param>
        /// <returns>Merged string.</returns>
        private string MergeReplaceOne(LineTranslationData lineData)
        {
            var sb = new StringBuilder(lineData.Original);
            var value = lineData.RegexExtractionData.ExtractedValuesList[0];
            foreach (var group in value.MatchGroups)
            {
                if (string.IsNullOrWhiteSpace(group.Value) || string.IsNullOrEmpty(value.Translation)) continue;
                var text = ApplyFixes(group.Value, value.Translation);
                sb.Replace(group.Value, text, group.Index, group.Length);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Merges using the ReplaceList strategy: replaces each group with its translation.
        /// </summary>
        /// <param name="lineData">Line data.</param>
        /// <returns>Merged string.</returns>
        private string MergeReplaceList(LineTranslationData lineData)
        {
            var sb = new StringBuilder(lineData.Original);
            foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
            {
                foreach (var group in value.MatchGroups)
                {
                    if (string.IsNullOrWhiteSpace(group.Value) || string.IsNullOrEmpty(value.Translation)) continue;
                    var text = ApplyFixes(group.Value, value.Translation);
                    sb.Replace(group.Value, text, group.Index, group.Length);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Merges using the Replacer strategy: uses a custom replacer with placeholders.
        /// </summary>
        /// <param name="lineData">Line data.</param>
        /// <returns>Merged string.</returns>
        private string MergeReplacer(LineTranslationData lineData)
        {
            var sb = new StringBuilder(lineData.RegexExtractionData.Replacer);
            var matches = _groupReplacerMarkerRegex.Matches(sb.ToString());
            foreach (Match match in matches.Cast<Match>().Reverse())
                sb.Replace(match.Value, $"%{match.Value}%", match.Index, match.Length);

            var reversedExtractedValuesList = new List<ExtractRegexValueInfo>(lineData.RegexExtractionData.ExtractedValuesList);
            reversedExtractedValuesList.Reverse();

            foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
            {
                foreach (var group in value.MatchGroups)
                {
                    var text = value.Translation ?? group.Value;
                    if (text != group.Value) text = ApplyFixes(group.Value, text);
                    sb.Replace($"%${group.Name}%", text);
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// Buffer for rows waiting for translation.
        /// </summary>
        internal class Buffer : IOriginalTranslationUser
        {
            /// <summary>
            /// Original text.
            /// </summary>
            public string Original { get; }

            /// <summary>
            /// Translated text.
            /// </summary>
            public string Translation { get; set; }

            /// <summary>
            /// Flag: whether the value was extracted.
            /// </summary>
            internal bool IsExtracted { get; }

            internal Buffer(string original, string translation, bool isExtracted)
            {
                Original = original;
                Translation = translation;
                IsExtracted = isExtracted;
            }
        }

        /// <summary>
        /// Stores translation data for a table.
        /// </summary>
        internal class TranslationData
        {
            /// <summary>
            /// Table index.
            /// </summary>
            internal int TableIndex => Row.SelectedTableIndex;

            /// <summary>
            /// Dictionary of rows by index.
            /// </summary>
            internal ConcurrentDictionary<int, RowTranslationData> Rows = new ConcurrentDictionary<int, RowTranslationData>();

            /// <summary>
            /// Row data.
            /// </summary>
            internal RowBaseRowData Row { get; }

            public TranslationData(RowBaseRowData rowData) => Row = rowData;
        }

        /// <summary>
        /// Stores translation data for a row.
        /// </summary>
        internal class RowTranslationData
        {
            /// <summary>
            /// Flag: whether all lines are added.
            /// </summary>
            internal bool IsAllLinesAdded = false;

            /// <summary>
            /// Row index.
            /// </summary>
            internal int RowIndex => Row.SelectedRowIndex;

            /// <summary>
            /// List of line data.
            /// </summary>
            internal List<LineTranslationData> Lines = new List<LineTranslationData>();

            /// <summary>
            /// Row data.
            /// </summary>
            internal RowBaseRowData Row { get; }

            public RowTranslationData(RowBaseRowData rowData) => Row = rowData;
        }

        /// <summary>
        /// Stores translation data for a single line.
        /// </summary>
        internal class LineTranslationData : IOriginalTranslationUser
        {
            /// <summary>
            /// Line index.
            /// </summary>
            internal readonly int LineIndex;

            /// <summary>
            /// Original text.
            /// </summary>
            public string Original { get; }

            /// <summary>
            /// Translated text.
            /// </summary>
            public string Translation { get; set; }

            /// <summary>
            /// Regex extraction data.
            /// </summary>
            internal ExtractRegexInfo RegexExtractionData;

            /// <summary>
            /// Flag: whether the line is excluded from translation.
            /// </summary>
            internal bool IsExcluded = false;

            public LineTranslationData(int lineIndex, string originalText)
            {
                LineIndex = lineIndex;
                Original = originalText;
                RegexExtractionData = new ExtractRegexInfo(Original);
            }

            /// <summary>
            /// Flag: line is translated or excluded.
            /// </summary>
            internal bool IsTranslated
            {
                get
                {
                    if (IsExcluded) return true;
                    if (RegexExtractionData.ExtractedValuesList.Count > 0)
                        return RegexExtractionData.ExtractedValuesList.All(v => v.IsTranslated);
                    return !string.IsNullOrEmpty(Translation) && !string.Equals(Original, Translation);
                }
            }
        }

        #endregion
    }

    #region Interfaces and External Classes

    /// <summary>
    /// Defines the contract for translation services.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Translates an array of strings.
        /// </summary>
        /// <param name="texts">Array of strings to translate.</param>
        /// <returns>Array of translated strings.</returns>
        string[] Translate(string[] texts);

        /// <summary>
        /// Translates a single string.
        /// </summary>
        /// <param name="text">String to translate.</param>
        /// <returns>Translated string.</returns>
        string Translate(string text);
    }

    /// <summary>
    /// Implementation of a translator using Google Translate (web).
    /// </summary>
    public class GoogleTranslator : ITranslator, IDisposable
    {
        #region Fields
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// HttpClient for requests to Google Translate.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Source language code.
        /// </summary>
        private readonly string _sourceLanguage;

        /// <summary>
        /// Target language code.
        /// </summary>
        private readonly string _targetLanguage;

        /// <summary>
        /// List of User-Agent strings to bypass restrictions.
        /// </summary>
        private readonly List<string> _userAgents;

        /// <summary>
        /// Random number generator for selecting User-Agent.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Semaphore for limiting concurrent requests.
        /// </summary>
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Delay between requests.
        /// </summary>
        private readonly TimeSpan _delayBetweenRequests;

        /// <summary>
        /// Flag: object disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Maximum number of retries for HTTP 429 errors.
        /// </summary>
        private const int Max429Retries = 5;

        /// <summary>
        /// Delay increase for HTTP 429 errors (ms).
        /// </summary>
        private const int DelayIncreaseMs = 2000;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of GoogleTranslator.
        /// </summary>
        /// <param name="sourceLanguage">Source language code.</param>
        /// <param name="targetLanguage">Target language code.</param>
        /// <param name="maxConcurrentRequests">Maximum concurrent requests.</param>
        /// <param name="delayMs">Delay between requests (ms).</param>
        public GoogleTranslator(string sourceLanguage, string targetLanguage, int maxConcurrentRequests = 5, int delayMs = 1000)
        {
            Logger.Debug("Initializing GoogleTranslator");
            _sourceLanguage = sourceLanguage ?? throw new ArgumentNullException(nameof(sourceLanguage));
            _targetLanguage = targetLanguage ?? throw new ArgumentNullException(nameof(targetLanguage));
            _userAgents = new List<string>
                    {
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Safari/605.1.15",
                        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36"
                    };
            _random = new Random();
            _semaphore = new SemaphoreSlim(maxConcurrentRequests);
            _delayBetweenRequests = TimeSpan.FromMilliseconds(delayMs);

            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };
            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        #endregion

        #region ITranslator Implementation

        /// <summary>
        /// Translates a single string (synchronously).
        /// </summary>
        /// <param name="text">String to translate.</param>
        /// <returns>Translated string.</returns>
        public string Translate(string text)
        {
            Logger.Debug($"Translating string: {text}");
            return TranslateAsync(text).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Translates an array of strings (synchronously).
        /// </summary>
        /// <param name="texts">Array of strings to translate.</param>
        /// <returns>Array of translated strings.</returns>
        public string[] Translate(string[] texts)
        {
            Logger.Debug($"Batch translation of string array. Count: {texts?.Length ?? 0}");
            return Task.WhenAll(texts.Select(TranslateAsync)).GetAwaiter().GetResult();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Asynchronously translates a single string.
        /// </summary>
        /// <param name="text">String to translate.</param>
        /// <returns>Translated string.</returns>
        private async Task<string> TranslateAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Logger.Error("Text to translate cannot be null or empty.");
                throw new ArgumentException("Text to translate cannot be null or empty.", nameof(text));
            }

            await _semaphore.WaitAsync();
            try
            {
                int retryCount = 0;
                int currentDelayMs = (int)_delayBetweenRequests.TotalMilliseconds;
                while (true)
                {
                    await Task.Delay(currentDelayMs);
                    string userAgent = _userAgents[_random.Next(_userAgents.Count)];

                    string url = $"https://translate.google.com/m?hl=en&sl={_sourceLanguage}&tl={_targetLanguage}&ie=UTF-8&prev=_m&q={Uri.EscapeDataString(text)}";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.UserAgent.ParseAdd(userAgent);

                    HttpResponseMessage response = null;
                    try
                    {
                        response = await _httpClient.SendAsync(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Error sending request to Google Translate: {ex.Message}");
                        if (retryCount > Max429Retries)
                        {
                            Logger.Error("Exceeded retry count for request error.");
                            throw;
                        }
                        retryCount++;
                        currentDelayMs += DelayIncreaseMs;
                        continue;
                    }

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        Logger.Warn("Received HTTP 429 (rate limit).");
                        if (retryCount > Max429Retries)
                        {
                            Logger.Error("Exceeded retry count for HTTP 429 error.");
                            throw new HttpRequestException("Rate limit exceeded (HTTP 429) after several retries. Consider increasing delay or using proxies.");
                        }
                        retryCount++;
                        currentDelayMs += DelayIncreaseMs;
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    string html = await response.Content.ReadAsStringAsync();
                    string translation = ExtractTranslation(html);
                    Logger.Debug($"Translation received: {translation}");
                    return translation;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Extracts translation from Google Translate HTML response.
        /// </summary>
        /// <param name="html">HTML response.</param>
        /// <returns>Translated string.</returns>
        private static string ExtractTranslation(string html)
        {
            var match = Regex.Match(html, @"<div class=""result-container"">(.*?)</div>");
            if (!match.Success)
            {
                Logger.Debug("Failed to extract translation from Google Translate response.");
                throw new InvalidOperationException("Failed to extract translation from response.");
            }

            // Decode HTML entities to normalize special characters
            return HttpUtility.HtmlDecode(match.Groups[1].Value);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases resources used by the translator.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Logger.Debug("Disposing GoogleTranslator resources.");
                _httpClient.Dispose();
                _semaphore.Dispose();

                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    #endregion
}
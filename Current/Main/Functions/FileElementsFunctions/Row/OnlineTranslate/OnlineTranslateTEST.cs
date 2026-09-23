using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Data.Interfaces;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate.OnlineTranslators;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;
using TranslationHelper.Main.Functions;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

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
        /// Whether this instance created <see cref="_translator"/> and therefore owns its lifetime.
        /// An injected translator belongs to the caller.
        /// </summary>
        private readonly bool _ownsTranslator;

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
            _ownsTranslator = translator == null;
            _translator = translator ?? new GoogleTranslator(sourceLanguage: THSettings.SourceLanguageCode, targetLanguage: THSettings.TargetLanguageCode);
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
            DisposeTranslator();
            if (AppSettings.InterruptTtanslation)
            {
                Logger.Warn("Translation was interrupted by the user.");
                AppSettings.InterruptTtanslation = false;
            }
            Logger.Info(T._("Translation complete"));
        }

        /// <summary>
        /// Releases the translator when this instance created it. Without this the HTTP client of every
        /// run stays alive until the process ends, because a fresh function instance is built per menu call.
        /// </summary>
        private void DisposeTranslator()
        {
            if (!_ownsTranslator) return;

            var disposable = _translator as IDisposable;
            if (disposable == null) return;

            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not dispose the translator ({ex.Message}).");
            }
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
        private string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            var preTranslated = new string[originalLines.Length];
            Array.Copy(originalLines, preTranslated, originalLines.Length);
            for (int i = 0; i < originalLines.Length; i++)
            {
                var result = Project.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], null);
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
        private string[] ApplyProjectPostTranslationAction(string[] originalLines, string[] translatedLines)
        {
            for (int i = 0; i < translatedLines.Length; i++)
            {
                var result = Project.OnlineTranslationProjectSpecificPostTranslationAction(originalLines[i], translatedLines[i]);
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

            var row = Project.FilesContent.Tables[tableIndex].Rows[rowData.RowIndex];
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
    /// Raised when a translation request was refused or answered with something that could not be read.
    /// Derives from <see cref="HttpRequestException"/> so callers that already handled a failed request
    /// keep working.
    /// </summary>
    public class TranslationRequestException : HttpRequestException
    {
        /// <summary>
        /// Creates an instance with a message.
        /// </summary>
        /// <param name="message">Description of the failure.</param>
        public TranslationRequestException(string message) : base(message) { }

        /// <summary>
        /// Creates an instance with a message and the underlying cause.
        /// </summary>
        /// <param name="message">Description of the failure.</param>
        /// <param name="innerException">The cause.</param>
        public TranslationRequestException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Translates text through the JSON endpoint of the Google Translate web client.
    /// <para>
    /// Requests are signed with the <c>tk</c> token and carry the browser headers a real browser sends.
    /// This is the realization XUnity.AutoTranslator ships as <c>GoogleTranslateCompat</c>, and the token
    /// is what makes the service answer: the same request without it is refused with HTTP 403.
    /// </para>
    /// <para>
    /// The HTML endpoint this class used before (<c>https://translate.google.com/m</c>) is now answered
    /// with a redirect to <c>https://www.google.com/sorry/index</c>, and that interstitial is served with
    /// HTTP 429. Because a handler follows redirects by default, every translation arrived as a 429 no
    /// matter how short the text was or how long the caller waited, which is why even a single word
    /// failed. Waiting longer cannot clear a challenge page, so retrying with a bigger delay never helped.
    /// </para>
    /// <para>
    /// The endpoint is reachable under more than one host and the hosts are challenged independently, so
    /// a refusal moves to the next one rather than ending the run. That is what makes a single word work
    /// even while one of the hosts is refusing this client.
    /// </para>
    /// </summary>
    public class GoogleTranslator : ITranslator, IDisposable
    {
        #region Constants

        /// <summary>
        /// Endpoints of the Google Translate JSON API, tried in order.
        /// <para>
        /// They answer the same protocol with the same token, but they are separate services: measured
        /// from one machine, <c>translate.googleapis.com</c> answered the challenge page while
        /// <c>translate.google.com</c> answered the translation. A refusal therefore moves to the next
        /// entry instead of ending the run.
        /// </para>
        /// </summary>
        private static readonly string[] TranslateApiUrls =
        {
            "https://translate.googleapis.com/translate_a/single",
            "https://translate.google.com/translate_a/single",
        };

        /// <summary>
        /// Query template of the endpoint: base URL, client, source, target, what to return, token, text.
        /// </summary>
        private const string TranslateApiUrlTemplate = "{0}?client={1}&sl={2}&tl={3}&dt=t&tk={4}&q={5}";

        /// <summary>
        /// Site the referer points at, so a request looks like it came from the translate page.
        /// </summary>
        private const string TranslateSiteUrl = "https://translate.google.com";

        /// <summary>
        /// Client identifier the web front end uses. The signed "webapp" client is the one the endpoint accepts.
        /// </summary>
        private const string WebAppClient = "webapp";

        /// <summary>
        /// Texts joined into a single request. Mirrors the batch size of the reference realization, and
        /// is what keeps a large batch from turning into one request per string.
        /// </summary>
        private const int MaxTextsPerRequest = 10;

        /// <summary>
        /// Upper bound for the encoded query of one request. The texts travel in the query string, where a
        /// non-ASCII character costs up to nine characters once escaped, so the budget is measured on the
        /// escaped length rather than on the text length.
        /// </summary>
        private const int MaxEncodedQueryLength = 4000;

        /// <summary>
        /// Attempts per request before the failure is reported.
        /// </summary>
        private const int MaxAttempts = 5;

        /// <summary>
        /// First backoff step in milliseconds; doubles per attempt.
        /// </summary>
        private const int BaseRetryDelayMs = 500;

        /// <summary>
        /// Ceiling of the backoff, so a large Retry-After cannot freeze the caller for minutes.
        /// </summary>
        private const int MaxRetryDelayMs = 15000;

        /// <summary>
        /// Multiplier of the signing TKK.
        /// <para>
        /// The reference realization reads this from a TKK value on the translate page and falls back to
        /// this constant. The page has stopped publishing one — a fetched copy is 281 KB and carries no
        /// <c>tkk:</c> and no cookie at all — so the constant is what signs requests, and the page is not
        /// read for it. Re-add the read if Google starts publishing a TKK again.
        /// </para>
        /// </summary>
        private const long TkkMultiplier = 427761;

        /// <summary>
        /// Offset of the signing TKK, used together with <see cref="TkkMultiplier"/>.
        /// </summary>
        private const long TkkOffset = 1179739010;

        /// <summary>
        /// Length at which a response body is cut down for a log line.
        /// </summary>
        private const int LoggedBodyLength = 200;

        #endregion

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
        /// User-Agent of the simulated browser.
        /// </summary>
        private readonly string _userAgent;

        /// <summary>
        /// Accept-Language of the simulated browser.
        /// </summary>
        private readonly string _acceptLanguage;

        /// <summary>
        /// Accept header of the simulated browser.
        /// </summary>
        private readonly string _accept;

        /// <summary>
        /// Referer of a translation request.
        /// </summary>
        private readonly string _referer;

        /// <summary>
        /// Accept-Charset of the simulated browser.
        /// </summary>
        private readonly string _acceptCharset;

        /// <summary>
        /// Serialises requests. The endpoint is asked one request at a time, which is what the reference
        /// realization does and what keeps the caller from being throttled.
        /// </summary>
        private readonly SemaphoreSlim _requestGate;

        /// <summary>
        /// Minimum distance between two requests. <see cref="TimeSpan.Zero"/> disables the spacing.
        /// </summary>
        private readonly TimeSpan _minRequestInterval;

        /// <summary>
        /// Endpoint currently used. Only touched while <see cref="_requestGate"/> is held.
        /// </summary>
        private int _apiUrlIndex;

        private DateTime _lastRequestUtc = DateTime.MinValue;

        /// <summary>
        /// Flag: object disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of GoogleTranslator.
        /// </summary>
        /// <param name="sourceLanguage">Source language code.</param>
        /// <param name="targetLanguage">Target language code.</param>
        /// <param name="maxConcurrentRequests">Maximum concurrent requests.</param>
        /// <param name="delayMs">Minimum delay between two requests in milliseconds. The first request is not delayed.</param>
        public GoogleTranslator(string sourceLanguage = "auto", string targetLanguage = "en", int maxConcurrentRequests = 1, int delayMs = 1000)
        {
            Logger.Debug("Initializing GoogleTranslator");
            _sourceLanguage = sourceLanguage ?? throw new ArgumentNullException(nameof(sourceLanguage));
            _targetLanguage = targetLanguage ?? throw new ArgumentNullException(nameof(targetLanguage));

            // One coherent browser identity for the whole session: rotating it per request is what a bot
            // does, not what a browser does.
            _userAgent = UserAgents.Chrome_Win10;
            _acceptLanguage = "en-US,en;q=0.9";
            _accept = "application/json";
            _referer = TranslateSiteUrl + "/";
            _acceptCharset = Encoding.UTF8.WebName;

            _requestGate = new SemaphoreSlim(maxConcurrentRequests < 1 ? 1 : maxConcurrentRequests);
            _minRequestInterval = delayMs > 0 ? TimeSpan.FromMilliseconds(delayMs) : TimeSpan.Zero;

            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
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
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text to translate cannot be null or empty.", nameof(text));

            return Translate(new[] { text })[0];
        }

        /// <summary>
        /// Translates an array of strings (synchronously).
        /// </summary>
        /// <param name="texts">Array of strings to translate.</param>
        /// <returns>Array of translated strings, in the order of <paramref name="texts"/>.</returns>
        public string[] Translate(string[] texts)
        {
            if (texts == null) throw new ArgumentNullException(nameof(texts));

            Logger.Debug($"Batch translation of string array. Count: {texts.Length}");
            return TranslateAsync(texts).GetAwaiter().GetResult();
        }

        #endregion

        #region Translation Pipeline

        /// <summary>
        /// Translates every text, one chunk of requests at a time.
        /// </summary>
        /// <param name="texts">Texts to translate.</param>
        /// <returns>Translations in the order of <paramref name="texts"/>.</returns>
        private async Task<string[]> TranslateAsync(string[] texts)
        {
            EnsureNotDisposed();
            if (texts.Length == 0) return Array.Empty<string>();

            for (int i = 0; i < texts.Length; i++)
                if (string.IsNullOrEmpty(texts[i]))
                    throw new ArgumentException("Text to translate cannot be null or empty.", nameof(texts));

            var translations = new string[texts.Length];
            foreach (var chunk in BuildChunks(texts))
                await TranslateChunkAsync(texts, chunk, translations).ConfigureAwait(false);

            return translations;
        }

        /// <summary>
        /// Splits the texts into request-sized chunks, bounded by both the text count and the escaped length.
        /// </summary>
        /// <param name="texts">Texts to translate.</param>
        /// <returns>Indexes of the texts belonging to each request.</returns>
        private static IEnumerable<List<int>> BuildChunks(string[] texts)
        {
            var chunk = new List<int>(MaxTextsPerRequest);
            int encodedLength = 0;

            for (int i = 0; i < texts.Length; i++)
            {
                int textLength = Uri.EscapeDataString(texts[i]).Length;
                bool countReached = chunk.Count >= MaxTextsPerRequest;
                bool lengthReached = encodedLength + textLength + 1 > MaxEncodedQueryLength;
                if (chunk.Count > 0 && (countReached || lengthReached))
                {
                    yield return chunk;
                    chunk = new List<int>(MaxTextsPerRequest);
                    encodedLength = 0;
                }

                chunk.Add(i);
                encodedLength += textLength + 1;
            }

            if (chunk.Count > 0) yield return chunk;
        }

        /// <summary>
        /// Translates one chunk. A chunk travels as a single request, so when it fails it is retried text
        /// by text: one text the service dislikes must not cost the rest of the chunk.
        /// </summary>
        /// <param name="texts">All texts of the batch.</param>
        /// <param name="indexes">Indexes of the texts of this chunk.</param>
        /// <param name="translations">Translations collected so far.</param>
        private async Task TranslateChunkAsync(string[] texts, List<int> indexes, string[] translations)
        {
            if (indexes.Count == 0) return;

            try
            {
                var chunkTexts = new string[indexes.Count];
                for (int i = 0; i < indexes.Count; i++) chunkTexts[i] = texts[indexes[i]];

                var translated = await RequestChunkAsync(chunkTexts).ConfigureAwait(false);
                for (int i = 0; i < indexes.Count; i++) translations[indexes[i]] = translated[i];
                return;
            }
            catch (Exception ex)
            {
                if (indexes.Count == 1)
                    throw new TranslationRequestException($"Failed to translate \"{texts[indexes[0]]}\".", ex);

                Logger.Warn($"A chunk of {indexes.Count} texts failed ({ex.Message}); retrying its texts one by one.");
            }

            foreach (int index in indexes)
                await TranslateChunkAsync(texts, new List<int> { index }, translations).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends one chunk as a single request and maps the answer back onto the individual texts.
        /// </summary>
        /// <param name="texts">Texts of the chunk.</param>
        /// <returns>One translation per text.</returns>
        private async Task<string[]> RequestChunkAsync(string[] texts)
        {
            // The texts travel newline separated, which is how the service keeps them apart in the answer.
            var joined = string.Join("\n", texts);
            var translated = await SendAsync(joined).ConfigureAwait(false);

            if (texts.Length == 1) return new[] { translated };

            // The answer carries one line per text, so the split is what maps the translations back. When
            // it does not line up the mapping is unknown, and guessing it would put the wrong translation
            // into a row: the chunk is refused instead, and the caller retries its texts one by one.
            var lines = translated.Split('\n');
            if (lines.Length != texts.Length)
                throw new TranslationRequestException(
                    $"The service answered with {lines.Length} lines for {texts.Length} texts, so the translations cannot be matched back.");

            return lines;
        }

        #endregion

        #region Request Transport

        /// <summary>
        /// Sends one signed request and returns the translation, moving to the next endpoint and backing
        /// off while the service keeps refusing.
        /// </summary>
        /// <param name="text">Text of the request.</param>
        /// <returns>Translated text.</returns>
        private async Task<string> SendAsync(string text)
        {
            await _requestGate.WaitAsync().ConfigureAwait(false);
            try
            {
                for (int attempt = 1; ; attempt++)
                {
                    await WaitForRequestSlotAsync().ConfigureAwait(false);

                    HttpResponseMessage response = null;
                    string refusal = null;
                    TimeSpan? retryAfter = null;

                    try
                    {
                        response = await SendTranslationRequestAsync(text).ConfigureAwait(false);
                        int status = (int)response.StatusCode;

                        if (IsChallenge(response))
                        {
                            refusal = $"the challenge page at {GetFinalLocation(response)}";
                        }
                        else if (status == 429 || status == 403 || status >= 500)
                        {
                            refusal = $"HTTP {status}";
                            retryAfter = ReadRetryAfter(response);
                        }
                        else
                        {
                            response.EnsureSuccessStatusCode();
                            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            return ExtractTranslation(body);
                        }
                    }
                    catch (Exception ex)
                    {
                        refusal = ex.Message;
                    }
                    finally
                    {
                        if (response != null) response.Dispose();
                    }

                    if (attempt >= MaxAttempts)
                        throw new TranslationRequestException($"Google did not answer the translation request ({refusal}) after {attempt} attempts.");

                    Logger.Warn($"Translation request refused ({refusal}); retrying (attempt {attempt} of {MaxAttempts}).");

                    // The endpoints are challenged independently and one can also be unreachable, so every
                    // retry moves to the next one before waiting: a bad endpoint costs an attempt instead
                    // of the whole run. A failure that is not endpoint-specific simply alternates.
                    MoveToNextEndpoint();

                    await DelayBeforeRetryAsync(attempt, retryAfter).ConfigureAwait(false);
                }
            }
            finally
            {
                _requestGate.Release();
            }
        }

        /// <summary>
        /// Sends one signed translation request to the endpoint currently in use.
        /// </summary>
        /// <param name="text">Text of the request.</param>
        /// <returns>The raw response; the caller owns and disposes it.</returns>
        private async Task<HttpResponseMessage> SendTranslationRequestAsync(string text)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                TranslateApiUrlTemplate,
                TranslateApiUrls[_apiUrlIndex],
                WebAppClient,
                Uri.EscapeDataString(FixLanguage(_sourceLanguage)),
                Uri.EscapeDataString(FixLanguage(_targetLanguage)),
                Tk(text),
                Uri.EscapeDataString(text));

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddBrowserHeaders(request);
            try
            {
                // The content is read to the end before the call returns, which is what makes disposing
                // the request here safe.
                return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            }
            finally
            {
                request.Dispose();
            }
        }

        /// <summary>
        /// Moves to the next endpoint after a refusal, so a challenged host costs one attempt rather than
        /// the whole operation.
        /// </summary>
        private void MoveToNextEndpoint()
        {
            _apiUrlIndex = (_apiUrlIndex + 1) % TranslateApiUrls.Length;
            Logger.Warn($"Trying the next Google Translate endpoint: {TranslateApiUrls[_apiUrlIndex]}");
        }

        /// <summary>
        /// Tells whether the answer is the anti-abuse interstitial rather than a translation. The handler
        /// follows redirects, so the challenge is recognised by where the request ended up, which is what
        /// turns the old unexplained HTTP 429 into something that can be acted on.
        /// </summary>
        /// <param name="response">Answer to inspect.</param>
        /// <returns>True when the challenge page answered.</returns>
        private static bool IsChallenge(HttpResponseMessage response)
        {
            var location = GetFinalLocation(response);
            return location != null && location.IndexOf("/sorry/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Reads where a request ended up, for the challenge check and the log line.
        /// </summary>
        /// <param name="response">Answer to inspect.</param>
        /// <returns>The final address, or null when it is not known.</returns>
        private static string GetFinalLocation(HttpResponseMessage response)
        {
            var request = response.RequestMessage;
            var uri = request == null ? null : request.RequestUri;
            return uri == null ? null : uri.ToString();
        }

        /// <summary>
        /// Maps a language tag the settings can hold onto the code the service uses.
        /// </summary>
        /// <param name="language">Language tag.</param>
        /// <returns>Language code the service understands.</returns>
        private static string FixLanguage(string language)
        {
            switch (language)
            {
                case "zh-Hans":
                case "zh":
                    return "zh-CN";
                case "zh-Hant":
                    return "zh-TW";
                default:
                    return language;
            }
        }

        /// <summary>
        /// Adds the headers a browser would send. The endpoint is noticeably less willing to answer a bare
        /// request, and the referer is what tells it the caller came from the translate page.
        /// </summary>
        /// <param name="request">Request to decorate.</param>
        private void AddBrowserHeaders(HttpRequestMessage request)
        {
            AddHeader(request, "User-Agent", _userAgent);
            AddHeader(request, "Accept-Language", _acceptLanguage);
            AddHeader(request, "Accept", _accept);
            AddHeader(request, "Accept-Charset", _acceptCharset);
            AddHeader(request, "Referer", _referer);
        }

        /// <summary>
        /// Adds a header when it has a value, leaving the validation of unusual values to the caller.
        /// </summary>
        /// <param name="request">Request to decorate.</param>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        private static void AddHeader(HttpRequestMessage request, string name, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            request.Headers.TryAddWithoutValidation(name, value);
        }

        /// <summary>
        /// Keeps two requests apart by at least <see cref="_minRequestInterval"/>. The first request is
        /// never delayed, so translating a single word does not pay for a pause nobody needs.
        /// </summary>
        private async Task WaitForRequestSlotAsync()
        {
            if (_minRequestInterval <= TimeSpan.Zero) return;

            var elapsed = DateTime.UtcNow - _lastRequestUtc;
            if (elapsed < _minRequestInterval)
                await Task.Delay(_minRequestInterval - elapsed).ConfigureAwait(false);

            _lastRequestUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Waits before the next attempt, doubling the pause each time and never shorter than what the
        /// service asked for.
        /// </summary>
        /// <param name="attempt">Attempt that just failed, counted from one.</param>
        /// <param name="retryAfter">Delay the service asked for, if any.</param>
        private async Task DelayBeforeRetryAsync(int attempt, TimeSpan? retryAfter)
        {
            var backoff = TimeSpan.FromMilliseconds(Math.Min(BaseRetryDelayMs * Math.Pow(2, attempt - 1), MaxRetryDelayMs));
            var delay = retryAfter.HasValue && retryAfter.Value > backoff ? retryAfter.Value : backoff;

            if (delay > TimeSpan.FromMilliseconds(MaxRetryDelayMs))
            {
                Logger.Debug($"Google asked to wait {delay.TotalSeconds:F0} s; waiting {MaxRetryDelayMs / 1000} s instead.");
                delay = TimeSpan.FromMilliseconds(MaxRetryDelayMs);
            }

            Logger.Debug($"Waiting {delay.TotalMilliseconds:F0} ms before the next translation attempt.");
            await Task.Delay(delay).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the delay the service asked for, if it asked for one.
        /// </summary>
        /// <param name="response">Answer to inspect.</param>
        /// <returns>The requested delay, or null.</returns>
        private static TimeSpan? ReadRetryAfter(HttpResponseMessage response)
        {
            var retryAfter = response.Headers.RetryAfter;
            if (retryAfter == null) return null;
            if (retryAfter.Delta.HasValue) return retryAfter.Delta.Value;
            if (!retryAfter.Date.HasValue) return null;

            var delta = retryAfter.Date.Value - DateTimeOffset.UtcNow;
            return delta > TimeSpan.Zero ? delta : (TimeSpan?)null;
        }

        #endregion

        #region Request Signing

        /// <summary>
        /// Computes the <c>tk</c> token that signs a request. The arithmetic follows the algorithm the
        /// translation clients use, including the 32-bit mask, because the service validates the result.
        /// </summary>
        /// <param name="text">Text of the request.</param>
        /// <returns>The signing token.</returns>
        private static string Tk(string text)
        {
            var bytes = new List<long>();
            for (int i = 0; i < text.Length; i++)
            {
                long code = text[i];
                if (code < 128)
                {
                    bytes.Add(code);
                    continue;
                }

                if (code < 2048)
                {
                    bytes.Add(code >> 6 | 192);
                }
                else if (55296 == (64512 & code) && i + 1 < text.Length && 56320 == (64512 & text[i + 1]))
                {
                    code = 65536 + ((1023 & code) << 10) + (1023 & text[i + 1]);
                    i++;
                    bytes.Add(code >> 18 | 240);
                    bytes.Add(code >> 12 & 63 | 128);
                }
                else
                {
                    bytes.Add(code >> 12 | 224);
                    bytes.Add(code >> 6 & 63 | 128);
                }

                bytes.Add(63 & code | 128);
            }

            const string mixMultiply = "+-a^+6";
            const string mixFinish = "+-3^+b+-f";

            long value = TkkMultiplier;
            for (int i = 0; i < bytes.Count; i++)
            {
                value += bytes[i];
                value = Mix(value, mixMultiply);
            }

            value = Mix(value, mixFinish);
            value ^= TkkOffset;
            if (value < 0) value = (2147483647 & value) + 2147483648;
            value %= 1000000;

            return value.ToString(CultureInfo.InvariantCulture) + "." + (value ^ TkkMultiplier).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Bit-mixing step of the signing algorithm.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="operations">Operation string, read in triples.</param>
        /// <returns>Mixed value.</returns>
        private static long Mix(long value, string operations)
        {
            for (int i = 0; i < operations.Length; i += 3)
            {
                long amount = operations[i + 2];
                amount = amount >= 'a' ? amount - 87 : amount - '0';
                amount = '+' == operations[i + 1] ? value >> (int)amount : value << (int)amount;
                value = '+' == operations[i] ? value + amount & 4294967295 : value ^ amount;
            }

            return value;
        }

        #endregion

        #region Response Parsing

        /// <summary>
        /// Reads the translation out of the JSON answer. The answer is a nested array whose first element
        /// holds one segment per translated line, and the segments are concatenated because that is what
        /// rebuilds the text that was sent.
        /// </summary>
        /// <param name="json">Response body.</param>
        /// <returns>Translated text.</returns>
        private static string ExtractTranslation(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new TranslationRequestException("Google answered the translation request with an empty body.");

            JArray root;
            try
            {
                root = JArray.Parse(json);
            }
            catch (Exception ex)
            {
                // A challenge or error page arrives as HTML, and guessing what it says is not worth it.
                throw new TranslationRequestException($"Google answered the translation request with something that is not a JSON array: {Shorten(json)}", ex);
            }

            if (root.Count == 0 || root[0].Type != JTokenType.Array)
                throw new TranslationRequestException($"Google answered the translation request without a translation segment array: {Shorten(json)}");

            var builder = new StringBuilder();
            foreach (var token in (JArray)root[0])
            {
                var segment = token as JArray;
                if (segment == null || segment.Count == 0) continue;

                var value = segment[0] as JValue;
                if (value == null || value.Type != JTokenType.String) continue;

                builder.Append(value.Value<string>());
            }

            if (builder.Length == 0)
                throw new TranslationRequestException($"Google answered the translation request without any translated segment: {Shorten(json)}");

            return builder.ToString();
        }

        /// <summary>
        /// Shortens a response body so a log line stays readable.
        /// </summary>
        /// <param name="text">Body to shorten.</param>
        /// <returns>The body, truncated when long.</returns>
        private static string Shorten(string text)
        {
            if (string.IsNullOrEmpty(text)) return "<empty>";

            return text.Length <= LoggedBodyLength ? text : text.Substring(0, LoggedBodyLength) + "...";
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases resources used by the translator.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Logger.Debug("Disposing GoogleTranslator resources.");
            try { _httpClient.Dispose(); } catch (Exception ex) { Logger.Warn($"Could not dispose the HTTP client ({ex.Message})."); }
            try { _requestGate.Dispose(); } catch (Exception ex) { Logger.Warn($"Could not dispose the request gate ({ex.Message})."); }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Fails fast when the translator was already disposed, instead of reporting the failure as a
        /// request error.
        /// </summary>
        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GoogleTranslator));
        }

        #endregion
    }

    #endregion
}
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
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate.OnlineTranslators;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    partial class OnlineTranslateNew : RowBase
    {
        public override string Name => T._("Translator");

        protected override bool IsParallelTables => false;
        protected override bool IsParallelRows => false;
        protected virtual bool IsTranslateAll => true;

        ITranslationCache _cache;

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
            internal int TableIndex { get => Row.SelectedTableIndex; }
            internal List<RowTranslationData> Rows = new List<RowTranslationData>();
            internal RowBaseRowData Row { get; }

            public TranslationData(RowBaseRowData rowData)
            {
                this.Row = rowData;
            }
        }

        internal class RowTranslationData
        {
            internal bool IsAllLinesAdded = false;
            internal int RowIndex { get => Row.SelectedRowIndex; }
            internal List<LineTranslationData> Lines = new List<LineTranslationData>();
            internal RowBaseRowData Row { get; }

            public RowTranslationData(RowBaseRowData rowData)
            {
                this.Row = rowData;
            }
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

            /// <summary>
            /// Determines if dont need to translate the string for some reason
            /// </summary>
            internal bool IsExcluded = false;
            internal bool IsTranslated
            {
                get
                {
                    bool haveExtracted = RegexExtractionData.ExtractedValuesList.Count == 0;
                    bool isNotEqualOriginalTranslation = haveExtracted && !string.IsNullOrEmpty(Translation) && !string.Equals(Original, Translation);
                    bool haveAllExtractedTranslated = !haveExtracted && !RegexExtractionData.ExtractedValuesList.Any(l => !l.IsTranslated);

                    return IsExcluded || isNotEqualOriginalTranslation || haveAllExtractedTranslated;
                }
            }
        }

        List<TranslationData> _buffer;

        int TranslationTextLength { get; set; }
        static int MaxTranslationTextLength { get => 1000; }
        const int BufferMaxRows = 300;

        bool IsMax()
        {
            return TranslationTextLength >= MaxTranslationTextLength;
        }

        public OnlineTranslateNew(ITranslationCache cache = null)
        {
            if (_buffer == null) _buffer = new List<TranslationData>();
            if (_translator == null) _translator = new GoogleAPIOLD();
            _cache = cache ?? new TranslationCache();
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
        protected async override void ActionsInit()
        {
            if (_allDbLoaded4All || !IsAll || !AppSettings.UseAllDBFilesForOnlineTranslationForAll) return;

            if (!AppSettings.EnableTranslationCache)
            {
                //cache disabled but all db loading enabled. ask for load then. maybe not need
                var result = MessageBox.Show(T._("Translation cache disabled but load all DB enabled. While all DB loading cache can be enabled in settings. Load all DB?"), T._("Translation cache disabled"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            if (!IsTranslateAll && AppSettings.EnableTranslationCache)
            {
                var result = MessageBox.Show(T._("Load all exist database files?"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            Logger.Info(T._("Get all DB"));

            await Task.Run(() => FunctionsDBFile.MergeAllDBtoOne()).ConfigureAwait(true);

            _allDbLoaded4All = true;
        }
        protected override void ActionsFinalize()
        {
            if (_buffer.Count > 0) TranslateStrings();

            _cache.Dispose();

            if (AppSettings.InterruptTtanslation)
            {
                AppSettings.InterruptTtanslation = false;

                Logger.Info(T._("Translation interrupted"));
            }
            else
            {
                Logger.Info(T._("Translation complete"));
            }

        }

        private string _lasTableName = string.Empty;

        protected override bool Apply(RowBaseRowData rowData)
        {
            if (_lasTableName != rowData.SelectedTable.TableName)
            {
                _lasTableName = rowData.SelectedTable.TableName;

                Logger.Info(T._("Translate {0}"), _lasTableName);
            }

            SetRowLinesToBuffer(rowData);

            return true;
        }

        private readonly object _bufferLock = new object();

        private void SetRowLinesToBuffer(RowBaseRowData rowData)
        {
            if (rowData == null) throw new ArgumentNullException(nameof(rowData));
            if (rowData.SelectedTable == null) throw new InvalidOperationException("SelectedTable is null");
            if (rowData.Original == null) throw new InvalidOperationException("Original text is null");

            // Find the table data for the selected table index, or create a new one if it doesn't exist
            var selectedTableData = _buffer.FirstOrDefault(tableData => tableData.TableIndex == rowData.SelectedTableIndex);
            if (selectedTableData == null)
            {
                selectedTableData = new TranslationData(rowData);
                lock (_bufferLock)
                {
                    _buffer.Add(selectedTableData);
                }
            }

            // Find the row data for the selected row index under the selected table, or create a new one if it doesn't exist
            var selectedRowData = selectedTableData.Rows.FirstOrDefault(rowTranslationData => rowTranslationData.RowIndex == rowData.SelectedRowIndex);
            if (selectedRowData == null)
            {
                selectedRowData = new RowTranslationData(rowData);
                selectedTableData.Rows.Add(selectedRowData);
            }

            var originalTextLength = TranslationTextLength;

            int lineIndex = -1;
            string originalText = rowData.Original;
            // Parse each line of the original text
            foreach (var line in originalText.SplitToLines())
            {
                lineIndex++;

                // Find the line data for the current line under the selected row, or create a new one if it doesn't exist
                var lineCoordinates = rowData.SelectedTableIndex + "," + rowData.SelectedRowIndex;
                var lineData = selectedRowData.Lines.FirstOrDefault(data => data.LineIndex == lineIndex);

                // If the line data already exists and is not translatable, skip it and move to the next line
                if (lineData != null && lineData.IsTranslated)
                {
                    continue;
                }

                // If the line data does not exist, create a new one
                if (lineData == null)
                {
                    lineData = new LineTranslationData(lineIndex, line);
                    selectedRowData.Lines.Add(lineData);
                }

                // Extract information from the line using regex
                var extractData = new ExtractRegexInfo(line);
                lineData.RegexExtractionData = extractData;
                var extractedValuesCount = extractData.ExtractedValuesList.Count;
                bool isExtracted = extractedValuesCount > 0;

                // If the line is not valid for translation, add the original as the translation and move to the next line
                if (!isExtracted && !line.IsValidForTranslation())
                {
                    lineData.IsExcluded = true;
                    lineData.Translation = line;
                    continue;
                }

                // Parse each extracted value from the line
                GetFromExtracted(extractData, out int skippedValuesCount);

                // If all extracted values were skipped, don't translate this line and move to the next one
                if (extractedValuesCount > 0 && skippedValuesCount == extractedValuesCount)
                {
                    continue;
                }

                // If the line is not extracted and its translation is available in the cache, use the cached translation            
                if (!isExtracted && CheckInCache(line, lineData))
                {
                    continue;
                }

                // If we've reached the maximum size for translation, translate the strings and reset the size counter
                TranslateIfNeed();

                // Stop translating after the last pack of text if translation is interrupted
                if (AppSettings.InterruptTtanslation) return;
            }

            // Mark the row data as having all lines added
            selectedRowData.IsAllLinesAdded = true;

            // If no text has been added for translation, clean up the data and return
            if (originalTextLength == TranslationTextLength)
            {
                if (WriteRowData(selectedRowData, selectedTableData.TableIndex))
                {
                    selectedRowData = null;
                    selectedTableData.Rows.Remove(selectedRowData);
                    if (selectedTableData.Rows.Count == 0)
                    {
                        lock (_bufferLock)
                        {
                            _buffer.Remove(selectedTableData);
                        }
                    }

                    return;
                }
            }

            // If this is not the last row and fewer than 300 rows have been added to the buffer, return
            if (!rowData.IsLastRow && _buffer.Count < BufferMaxRows) return;

            // Translate the strings and reset the buffer
            TranslateStrings();
            ResetSizeAndBuffer();

            // Write the translation cache periodically
            _cache.Write();
        }

        private void ResetSizeAndBuffer()
        {
            TranslationTextLength = 0;
            //_buffer = new List<TranslationData>();
        }

        private void TranslateIfNeed()
        {
            if (IsMax())
            {
                TranslateStrings();

                // Write the translation cache periodically
                _cache.Write();
            }
        }

        private bool CheckInCache(string line, LineTranslationData lineData)
        {
            var lineCache = _cache.TryGetValue(line);
            if (!string.IsNullOrEmpty(lineCache))
            {
                lineData.Translation = lineCache;
                return true;
            }
            else
            {
                TranslationTextLength += lineData.Original.Length; // increase size of string for translation for check later
                return false;
            }
        }

        private void GetFromExtracted(ExtractRegexInfo extractData, out int skippedValuesCount)
        {
            skippedValuesCount = 0;
            foreach (var extractedValueData in extractData.ExtractedValuesList)
            {
                // Get the translation from the translation cache, if available
                var translationCache = _cache.TryGetValue(extractedValueData.Original);

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
                    TranslationTextLength += extractedValueData.Original.Length; // increase size of string for translation for check later
                }
            }
        }

        readonly GoogleAPIOLD _translator;
        /// <summary>
        /// get originals and translate them
        /// </summary>
        private void TranslateStrings()
        {
            var originals = GetOriginals();

            if (originals.Length == 0 && _buffer.Count == 0) return; // for case if no originals but there is still lines to translate inside of buffer

            var translated = TranslateOriginals(originals);

            SetTranslationsToBuffer(originals, translated);

            SetBufferToRows();

            ResetSizeAndBuffer();
        }

        /// <summary>
        /// get valid unique originals from buffer
        /// </summary>
        /// <returns></returns>
        private string[] GetOriginals()
        {
            return EnumerateBufferedLinesData()
                .AsParallel()
                .Aggregate(
                    new ConcurrentBag<string>(),
                    (originalLines, lineData) =>
                    {
                        foreach (var stringData in EnumerateOriginalTranslation(lineData))
                        {
                            if (originalLines.Contains(stringData.Original)) continue;

                            originalLines.Add(stringData.Original);
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
                foreach (var value in lineData.RegexExtractionData.ExtractedValuesList.Where(v => !(v.IsExcluded = !v.Original.IsValidForTranslation()) && !v.IsTranslated))
                {
                    yield return value;
                }
            }
            else
            {
                if (lineData.IsTranslated || (lineData.IsExcluded = !lineData.Original.IsValidForTranslation()))
                {
                    yield break;
                }

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
            //if (AppData.CurrentProject.HideVARSMatchCollectionsList?.Count > 0)
            //{
            //    // Clear the collection of found matches
            //    AppData.CurrentProject.HideVARSMatchCollectionsList.Clear();
            //}

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

            //var hideVarsMatchCollections = AppData.CurrentProject.HideVARSMatchCollectionsList;
            //if (hideVarsMatchCollections?.Count > 0)
            //{
            //    // Clear the collection of found matches
            //    hideVarsMatchCollections.Clear();
            //}

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
                if (translated == null || originals.Length != translated.Length)
                {
                    var translatedByParts = TryToTranslateByParts(originalLinesArePreApplied);
                    if (translatedByParts == null || translatedByParts.Count == 0) return Array.Empty<string>();

                    translated = translatedByParts.ToArray();
                }

                translated = ApplyProjectPostTranslationAction(originals, translated);
            }
            catch (Exception ex)
            {
                Logger.Warn("Error while translation:"
                    + Environment.NewLine
                    + ex
                    + Environment.NewLine
                    + "OriginalLines="
                    + string.Join(Environment.NewLine + "</br>" + Environment.NewLine, originals));
            }

            return translated;
        }

        private List<string> TryToTranslateByParts(string[] originalLinesArePreApplied)
        {
            // translate by x lines
            int i = 4;
            var resultTranslatedByParts = new List<string>();
            var listToTranslate = new List<string>();
            foreach (var line in originalLinesArePreApplied)
            {
                listToTranslate.Add(line);
                if (--i == 0)
                {
                    i = 2;

                    if (!TryToTranslateByParts1(listToTranslate, resultTranslatedByParts))
                        return null;

                    listToTranslate.Clear();
                }
            }

            if (!TryToTranslateByParts1(listToTranslate, resultTranslatedByParts))
                return null;

            return resultTranslatedByParts;
        }

        private bool TryToTranslateByParts1(List<string> listToTranslate, List<string> resultTranslatedByParts)
        {
            if (listToTranslate.Count == 0) return true;

            var translated = _translator.Translate(listToTranslate.ToArray());

            if (translated.Length != listToTranslate.Count)
            {
                // translate line by line
                foreach (var lineToTranslate in listToTranslate)
                {
                    string translatedLine = _translator.Translate(lineToTranslate);
                    if (string.IsNullOrWhiteSpace(translatedLine)) return false;

                    resultTranslatedByParts.Add(translatedLine);
                }
            }
            else
            {
                resultTranslatedByParts.AddRange(translated);
            }

            return true;
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

                _cache.TryAdd(original, translation);
            }

            Parallel.ForEach(EnumerateBufferedLinesData(), lineData =>
            {
                foreach (var originalTranslation in EnumerateOriginalTranslation(lineData))
                {
                    if (!TryGetTranslation(translations, originalTranslation, out var v)) continue;

                    originalTranslation.Translation = v;
                }
            });
        }

        private static bool TryGetTranslation(Dictionary<string, string> translations, IOriginalTranslationUser stringData, out string outTranslation)
        {
            outTranslation = default;

            if (!string.IsNullOrEmpty(stringData.Translation)
                && !string.Equals(stringData.Translation, stringData.Original)) return false;
            if (!stringData.Original.IsValidForTranslation()) return false;
            if (!translations.TryGetValue(stringData.Original, out var translation)) return false;

            outTranslation = translation;

            return true;
        }

        private IEnumerable<LineTranslationData> EnumerateBufferedLinesData()
        {
            foreach (var tableData in _buffer) // get data
            {
                foreach (var rowData in tableData.Rows) // get rows data
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

                if (dataRows.Count == 0)
                {
                    lock (_bufferLock)
                    {
                        _buffer.Remove(data);
                    }
                }
            }
        }

        private readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
        private readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
        private readonly StringChangerBase _hardFixes = new AllHardFixesChanger();
        private readonly StringChangerBase _fixCells = new FixCellsChanger();
        private bool WriteRowData(RowTranslationData rowData, int tableIndex)
        {
            if (!rowData.IsAllLinesAdded) return false; // skip if row is not fully translated

            var row = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rowData.RowIndex];
            var ignoreOrigEqualTransLines = AppSettings.IgnoreOrigEqualTransLines;

            // skip equal
            var originalText = rowData.Row.Original;
            var translationText = rowData.Row.Translation;
            var cellTranslationEqualOriginal = string.Equals(translationText, originalText);
            // ignore original=translation when it enable and when translations equal originals in translation lines data
            if (ignoreOrigEqualTransLines && cellTranslationEqualOriginal && !rowData.Lines.Any(l => l.Original != l.Translation)) return false;

            // skip when translation not equal to original and and have No any original line equal translation
            var cellTranslationIsNotEmptyAndNotEqualOriginal = !cellTranslationEqualOriginal && !string.IsNullOrEmpty(translationText);
            if (cellTranslationIsNotEmptyAndNotEqualOriginal && !originalText.HasAnyTranslationLineValidAndEqualSameOrigLine(translationText, false)) return false;

            // set new row value

            var newRowValue = string.Join(Environment.NewLine,
                EnumerateNewLines(rowData.Lines));
            //if (rowData.Row.Original.GetLinesCount() == 1 && string.Equals(newRowValue, rowData.Row.Original)) return false;

            //if (newRowValue.IsValidForTranslation()) return false; // was not fully translated o kind of

            rowData.Row.Translation = newRowValue;

            return true;
        }

        private bool HaveAnyLineExtractedValues(RowTranslationData rowData)
        {
            return rowData.Lines.Any(v => v.RegexExtractionData != null);
        }

        private IEnumerable<string> EnumerateNewLines(List<LineTranslationData> rowDataLines)
        {
            foreach (var lineData in rowDataLines)
            {
                if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0) // when line has extracted values
                {
                    yield return MergeExtracted(lineData);
                }
                else if (lineData.IsTranslated)
                {
                    // when was translated or excluded return translation or original line
                    yield return string.IsNullOrEmpty(lineData.Translation) ? lineData.Original : ApplyFixes(lineData);
                }
            }
        }

        private string ApplyFixes(LineTranslationData lineData) => ApplyFixes(lineData.Original, lineData.Translation);

        private string ApplyFixes(string original, string translation)
        {
            string text = _hardFixes.Change(translation, original);
            text = _fixCells.Change(text, original);

            return text;
        }

        static readonly Regex _replacerListTypeRegex = new Regex(@"^\$[0-9]+(,\$[0-9]+)+$", RegexOptions.Compiled);
        static readonly Regex _oneMatchNeedInsertTextRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Compiled);
        static readonly Regex _groupReplacerMarkerRegex = new Regex(@"\$[0-9]+", RegexOptions.Compiled);
        private string MergeExtracted(LineTranslationData lineData)
        {
            // replace all groups with translation of selected value
            bool isMarked = false;

            // get regex replacer type
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

            // merge extracted and translated values into translation of lineData
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
                            if (!string.IsNullOrEmpty(valueData.Translation))
                            {
                                // replace original value text with translation
                                var text = valueData.Translation;

                                if (text != group.Value)
                                {
                                    text = ApplyFixes(group.Value, text);
                                }

                                newLineText.Replace(group.Value
                                    , text
                                    , group.Index, group.Length);
                            }

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
                    var text = valueData.Translation;
                    if (text != null)
                    {
                        if (text != matchGroup.Value)
                        {
                            text = ApplyFixes(matchGroup.Value, text);
                        }
                    }
                    else
                    {
                        text = matchGroup.Value;
                    }

                    newLineText.Replace($"%${matchGroup.Name}%", text);
                }
                ////
            }

            return newLineText.ToString();
        }
    }
}

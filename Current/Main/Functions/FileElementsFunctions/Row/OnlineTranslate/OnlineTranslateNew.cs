using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using GoogleTranslateFreeApi;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;
using static TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslateNew;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    partial class OnlineTranslateNew : RowBase
    {
        // full row string
        public Dictionary<string, RowTranslationInfo> TranslationDataList = new Dictionary<string, RowTranslationInfo>();

        /// <summary>
        /// buffer for lines
        /// </summary>
        internal class Buffer
        {
            internal string GetOriginal { get; }
            internal string GetTranslation { get; set; }
            internal bool GetIsExtracted { get; }

            internal Buffer(string original, string translation, bool isExtracted)
            {
                GetOriginal = original;
                GetTranslation = translation;
                GetIsExtracted = isExtracted;
            }
        }

        internal class TranslationData
        {
            internal int TableIndex;
            internal List<RowsTranslationData> Rows = new List<RowsTranslationData>();
        }

        internal class RowsTranslationData
        {
            internal bool IsAllLinesAdded = false;
            internal int RowIndex;
            internal List<LineTranslationData> Lines = new List<LineTranslationData>();
        }

        internal class LineTranslationData
        {
            internal readonly int LineIndex;
            internal readonly string Original;
            internal string Translation;
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

        protected override bool IsValidRow()
        {
            return !AppSettings.InterruptTtanslation && base.IsValidRow()
                && string.IsNullOrEmpty(Translation)
                || Original.HasAnyTranslationLineValidAndEqualSameOrigLine(Translation);
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

        protected override bool Apply()
        {
            try
            {
                AppData.Main.ProgressInfo(true, "Translate" + " " + SelectedTable.TableName + "/" + SelectedRowIndex);

                SetRowLinesToBuffer();

                AppData.Main.ProgressInfo(false);
                return true;
            }
            catch
            {
            }

            return false;
        }

        private void SetRowLinesToBuffer()
        {
            var original = Original;
            var lineNum = 0;

            // add table data
            var data = _buffer.FirstOrDefault(d => d.TableIndex == SelectedTableIndex);
            if (data == null)
            {
                data = new TranslationData();
                data.TableIndex = SelectedTableIndex;
                _buffer.Add(data);
            }
            // add table's row data
            var rowData = data.Rows.FirstOrDefault(d => d.RowIndex == SelectedRowIndex);
            if (rowData == null)
            {
                rowData = new RowsTranslationData();
                rowData.RowIndex = SelectedRowIndex;
                data.Rows.Add(rowData);
            }

            // parse lines of original
            var oldSize = Size;
            foreach (var line in original.SplitToLines())
            {
                // set row's line data
                var lineCoordinates = SelectedTableIndex + "," + SelectedRowIndex;
                var lineData = rowData.Lines.FirstOrDefault(d => d.LineIndex == lineNum);

                // skip if line already exists?
                if (lineData != null && string.IsNullOrEmpty(lineData.Translation) && lineData.Translation != lineData.Original) { lineNum++; continue; }

                // init line data
                lineData = new LineTranslationData(lineNum, line) { Translation = line };
                rowData.Lines.Add(lineData);

                //check for line valid
                if (!line.IsValidForTranslation())
                {
                    //rowData.Lines.Add(lineData); // add original as translation because line is not valid and will be added as is
                    lineNum++;
                    continue;
                }

                // get extracted
                var extractData = new ExtractRegexInfo(line);
                lineData.RegexExtractionData = extractData;
                var extractedValuesCount = extractData.ExtractedValuesList.Count;
                bool isExtracted = extractedValuesCount > 0;

                int skippedValuesCount = 0;
                //parse all extracted values from original
                foreach (var valueData in extractData.ExtractedValuesList)
                {
                    // get translation from cache
                    var valcache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(valueData.Original);

                    if (!string.IsNullOrEmpty(valcache))
                    {
                        skippedValuesCount++;
                        valueData.Translation = valcache; // add translation from cache if found
                    }
                    else if (valueData.Original.IsSoundsText() || !valueData.Original.IsValidForTranslation())
                    {
                        skippedValuesCount++;
                        valueData.Translation = valueData.Original; // original=translation when value is soundtext
                    }
                    else
                    {
                        Size += valueData.Original.Length; // increase size of string for translation for check later
                    }
                }

                if (extractedValuesCount > 0 && skippedValuesCount == extractedValuesCount) { lineNum++; continue; } // all extracted was skipped, dont need to execute code below

                //check line value in cache
                if (!isExtracted)
                {
                    var linecache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(line);
                    if (!string.IsNullOrEmpty(linecache))
                    {
                        lineData.Translation = linecache;
                        lineNum++;
                        continue;
                    }

                    Size += lineData.Original.Length; // increase size of string for translation for check later
                }

                // when max limit of string size for translation
                if (IsMax())
                {
                    TranslateStrings();

                    Size = 0;
                    //_buffer.Clear();

                    //write cache periodically
                    AppData.OnlineTranslationCache.Write();

                    // stop translating after last pack of text when translation is interrupted
                    if (AppSettings.InterruptTtanslation) return;
                }

                lineNum++;
            }

            // mark row data, all lines was added
            rowData.IsAllLinesAdded = true;

            if (oldSize == Size) // not need to translate any
            {
                if (WriteRowData(rowData, data.TableIndex)) // write row data
                {
                    // clean data
                    rowData = null;
                    data.Rows.Remove(rowData);
                    if (data.Rows.Count > 0) data = null;
                    return;
                }
            }

            //translate if is last row or was added 300+ rows to buffer
            if (!IsLastRow && _buffer.Count < 300) return;

            TranslateStrings();

            Size = 0;
            _buffer = new List<TranslationData>();

            //write cache periodically
            AppData.OnlineTranslationCache.Write();
        }

        readonly GoogleAPIOLD _translator;
        /// <summary>
        /// get originals and translate them
        /// </summary>
        private void TranslateStrings()
        {
            var originals = GetOriginals();

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
            var orig = new List<string>();
            foreach (var data in _buffer) // get data
            {
                foreach (var rowData in data.Rows) // get rows data
                {
                    foreach (var lineData in rowData.Lines) // get line data
                    {
                        if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0)
                        {
                            foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
                            {
                                if (!orig.Contains(value.Original)
                                    && (string.IsNullOrEmpty(value.Translation))
                                    && value.Original.IsValidForTranslation())
                                {
                                    orig.Add(value.Original);
                                }
                            }

                            continue;
                        }

                        if (!orig.Contains(lineData.Original) && (lineData.Translation == null || lineData.Translation == lineData.Original) && lineData.Original.IsValidForTranslation()) orig.Add(lineData.Original);
                    }
                }
            }

            return orig.ToArray();
        }

        /// <summary>
        /// apply project specific pre actions like hide variables to original lines before they will be translated
        /// </summary>
        /// <param name="originalLines"></param>
        /// <returns></returns>
        private static string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            if (AppData.CurrentProject.HideVARSMatchCollectionsList != null
                && AppData.CurrentProject.HideVARSMatchCollectionsList.Count > 0) AppData.CurrentProject.HideVARSMatchCollectionsList.Clear();//clean of found maches collections

            var newOriginalLines = new string[originalLines.Length];
            Array.Copy(originalLines, newOriginalLines, originalLines.Length);
            for (int i = 0; i < newOriginalLines.Length; i++)
            {
                var s = AppData.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], string.Empty);
                if (!string.IsNullOrEmpty(s)) newOriginalLines[i] = s;
            }
            return newOriginalLines;
        }

        /// <summary>
        /// apply project specific post actions like unhide variables to original lines after they was translated
        /// </summary>
        /// <param name="originalLines"></param>
        /// <param name="translatedLines"></param>
        /// <returns></returns>
        private static string[] ApplyProjectPosttranslationAction(string[] originalLines, string[] translatedLines)
        {
            for (int i = 0; i < translatedLines.Length; i++)
            {
                var s = AppData.CurrentProject.OnlineTranslationProjectSpecificPosttranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(s) && s != translatedLines[i]) translatedLines[i] = s;
            }

            if (AppData.CurrentProject.HideVARSMatchCollectionsList != null && AppData.CurrentProject.HideVARSMatchCollectionsList.Count > 0)
            {
                AppData.CurrentProject.HideVARSMatchCollectionsList.Clear();//clean of found maches collections
            }

            return translatedLines;
        }

        GoogleTranslator translator = new GoogleTranslator();

        /// <summary>
        /// translate originals in selected translator
        /// </summary>
        /// <param name="originals"></param>
        /// <returns></returns>
        private string[] TranslateOriginals(string[] originals)
        {
            string[] translated = null;
            try
            {
                var originalLinesArePreApplied = ApplyProjectPretranslationAction(originals);
                if (originalLinesArePreApplied.Length > 0)
                {
                    translated = _translator.Translate(originalLinesArePreApplied);
                    if (translated == null || originals.Length != translated.Length) return new string[1] { "" };
                    translated = ApplyProjectPosttranslationAction(originals, translated);
                }
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
                translations.Add(originals[i], translated[i]);

                FunctionsOnlineCache.TryAdd(originals[i], translated[i]);
            }

            Parallel.ForEach(EnumerateLineData(_buffer), lineData =>
            {
                if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0)
                {
                    foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
                    {
                        if (TryGetTranslation(translations, value.Original, value.Translation, out var v)) return;

                        value.Translation = v;
                    }
                }
                else
                {
                    if (TryGetTranslation(translations, lineData.Original, lineData.Translation, out var v)) return;

                    lineData.Translation = v;
                }
            });
        }

        private bool TryGetTranslation(Dictionary<string, string> translations, string original, string translation, out string outTranslation)
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
        private bool WriteRowData(RowsTranslationData rowData, int tableIndex)
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
            row.SetValue(_translationColumnIndex, string.Join(Environment.NewLine, EnumerableNewLines(cellTranslationIsNotEmptyAndNotEqualOriginal ? translationText : originalText, rowData.Lines)));

            // apply fixes for cell
            // apply only for finished rows
            _hardFixes.Selected(row, tableIndex, rowData.RowIndex);
            _fixCells.Selected(row, tableIndex, rowData.RowIndex);

            return true;
        }

        private IEnumerable<string> EnumerableNewLines(string rowValue, List<LineTranslationData> rowDataLines)
        {
            int lineNum = -1;
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

                        try
                        {
                            // replace original value text with translation
                            newLineText.Replace(group.Value
                                , valueData.Translation ?? group.Value
                                , group.Index, group.Length);
                        }
                        catch (IndexOutOfRangeException) { }
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

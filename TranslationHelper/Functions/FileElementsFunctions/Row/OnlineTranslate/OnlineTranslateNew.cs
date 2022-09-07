using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleTranslateFreeApi;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;

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
            internal HashSet<RowsTranslationData> Rows = new HashSet<RowsTranslationData>();
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
            internal readonly string OriginalText;
            internal string TranslationText;
            internal ExtractRegexInfo RegexExtractionData = new ExtractRegexInfo();

            public LineTranslationData(int lineIndex, string originalText)
            {
                LineIndex = lineIndex;
                OriginalText = originalText;
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
                && (SelectedRow[1] == null || string.IsNullOrEmpty(SelectedRow[1] + "")
                || SelectedRow.HasAnyTranslationLineValidAndEqualSameOrigLine());
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
            var original = SelectedRow[0] as string;
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
            foreach (var line in original.SplitToLines())
            {
                // set row's line data
                var lineCoordinates = SelectedTableIndex + "," + SelectedRowIndex;
                var lineData = rowData.Lines.FirstOrDefault(d => d.LineIndex == lineNum);

                // skip if line already exists?
                if (lineData != null && string.IsNullOrEmpty(lineData.TranslationText) && lineData.TranslationText != lineData.OriginalText) { lineNum++; continue; }

                // init line data
                lineData = new LineTranslationData(lineNum, line) { TranslationText = line };
                rowData.Lines.Add(lineData);

                //check for line valid
                if (!line.IsValidForTranslation())
                {
                    //rowData.Lines.Add(lineData); // add original as translation because line is not valid and will be added as is
                    lineNum++;
                    continue;
                }

                // get extracted
                var extractData = line.ExtractMulty();
                lineData.RegexExtractionData = extractData;
                var extractedValuesCount = extractData.ValueDataList.Count;
                bool isExtracted = extractedValuesCount > 0;

                int skippedValuesCount = 0;
                //parse all extracted values from original
                foreach (var val in extractData.ValueDataList)
                {
                    // get translation from cache
                    var valcache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(val.Key);

                    if (!string.IsNullOrEmpty(valcache))
                    {
                        skippedValuesCount++;
                        val.Value.Translation = valcache; // add translation from cache if found
                    }
                    else if (val.Key.IsSoundsText())
                    {
                        skippedValuesCount++;
                        val.Value.Translation = val.Key; // original=translation when value is soundtext
                    }
                    else
                    {
                        Size += val.Key.Length; // increase size of string for translation for check later
                    }
                }

                if (extractedValuesCount > 0 && skippedValuesCount == extractedValuesCount) { lineNum++; continue; } // all extracted was skipped, dont need to execute code below

                //check line value in cache
                if (!isExtracted)
                {
                    var linecache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(line);
                    if (!string.IsNullOrEmpty(linecache))
                    {
                        lineData.TranslationText = linecache;
                        lineNum++;
                        continue;
                    }

                    Size += lineData.OriginalText.Length; // increase size of string for translation for check later
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
                        if (lineData.RegexExtractionData.ValueDataList.Count > 0)
                        {
                            foreach (var value in lineData.RegexExtractionData.ValueDataList) if (!orig.Contains(value.Key) && value.Value.Translation == null && value.Key.IsValidForTranslation()) orig.Add(value.Key);
                            continue;
                        }

                        if (!orig.Contains(lineData.OriginalText) && (lineData.TranslationText == null || lineData.TranslationText == lineData.OriginalText) && lineData.OriginalText.IsValidForTranslation()) orig.Add(lineData.OriginalText);
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
            var translations = new Dictionary<string, string>();
            if (originals != null && translated != null && originals.Length > 0 && originals.Length == translated.Length)
            {
                for (int i = 0; i < originals.Length; i++)
                {
                    translations.Add(originals[i], translated[i]);

                    FunctionsOnlineCache.TryAdd(originals[i], translated[i]);
                }
            }
            else return;

            foreach (var data in _buffer) // get data
            {
                foreach (var rowData in data.Rows) // get rows data
                {
                    foreach (var lineData in rowData.Lines) // get line data
                    {
                        if (lineData.RegexExtractionData.ValueDataList.Count > 0)
                        {
                            foreach (var value in lineData.RegexExtractionData.ValueDataList)
                            {
                                if (value.Value.Translation != null && value.Value.Translation != value.Key) continue;
                                if (!value.Key.IsValidForTranslation()) continue;
                                if (!translations.ContainsKey(value.Key)) continue;

                                value.Value.Translation = translations[value.Key];
                            }

                            continue;
                        }

                        if (lineData.TranslationText != null && lineData.TranslationText != lineData.OriginalText) continue;
                        if (!lineData.OriginalText.IsValidForTranslation()) continue;
                        if (!translations.ContainsKey(lineData.OriginalText)) continue;

                        lineData.TranslationText = translations[lineData.TranslationText];
                    }
                }
            }
        }

        /// <summary>
        /// set buffer content to rows by coordinates
        /// </summary>
        private void SetBufferToRows()
        {
            //get all coordinate keys
            foreach (var data in _buffer)
            {
                var translatedRowsData = new HashSet<RowsTranslationData>();
                foreach (var rowData in data.Rows)
                {
                    if (WriteRowData(rowData, data.TableIndex))
                        translatedRowsData.Add(rowData); // add rowData to translated
                }

                // clean rows
                foreach (var rowdata in translatedRowsData) data.Rows.Remove(rowdata);
                translatedRowsData = null;
            }

            // clean empty tables
            for (int t = _buffer.Count - 1; t >= 0; t--) if (_buffer[t].Rows.Count == 0) _buffer.RemoveAt(t);
        }

        private static bool WriteRowData(RowsTranslationData rowData, int tableIndex)
        {
            if (!rowData.IsAllLinesAdded) return false; // skip if row is not fully translated

            var row = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rowData.RowIndex];

            // skip equal
            var cellTranslationEqualOriginal = Equals(row[1], row[0]);
            if (AppSettings.IgnoreOrigEqualTransLines && cellTranslationEqualOriginal) return false;

            // skip when translation not equal to original and and have No any original line equal translation
            var cellTranslationIsNotEmptyAndNotEqualOriginal = (row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !cellTranslationEqualOriginal);
            if (cellTranslationIsNotEmptyAndNotEqualOriginal && !row.HasAnyTranslationLineValidAndEqualSameOrigLine(false)) return false;

            var newValue = new List<string>();
            var lineNum = 0;
            var rowValue = (cellTranslationIsNotEmptyAndNotEqualOriginal ? row[1] : row[0]) + "";
            foreach (var line in rowValue.SplitToLines())
            {
                //if (lineNum >= rowData.Lines.Count) break; // multyline row was not fully translated, skip to translate later on next loop
                if (lineNum >= rowData.Lines.Count)
                {
                    break; // temp check if this condition is useless because IsAllLinesAdded must skip it
                }

                var lineData = rowData.Lines[lineNum];

                if (lineData.RegexExtractionData.ValueDataList.Count > 0) // when line has extracted values
                {
                    // replace all groups with translation of selected value
                    var newLineText = lineData.RegexExtractionData.Replacer;
                    foreach (var valueData in lineData.RegexExtractionData.ValueDataList.Values)
                    {
                        // search all $1-$99 in replacer
                        var replacerMatches = Regex.Matches(newLineText, @"\$[0-9]+");
                        var matchesCount = replacerMatches.Count;

                        // when only one replacer mark like '$1'
                        if (matchesCount == 1 && valueData.Group.Count == 1 && Regex.IsMatch(newLineText.Trim(), @"^\$[0-9]+$"))
                        {
                            var group = valueData.Group[0];

                            // replace original value text with translation
                            newLineText = lineData.OriginalText
                                .Remove(group.Index, group.Length)
                                .Insert(group.Index, valueData.Translation);

                            break; // exit from values loop, to not execute lines below
                        }

                        // mark all matches for precise replacement, $1 to %$1%
                        for (int i = matchesCount - 1; i >= 0; i--)
                        {
                            var match = replacerMatches[i];

                            newLineText = newLineText.Remove(match.Index, match.Length)
                                .Insert(match.Index, $"%{match.Value}%");
                            ;
                        }

                        // replace group index by translation
                        foreach (var groupIndex in valueData.Group)
                        {
                            // replace group mark with translation
                            newLineText = newLineText.Replace($"%${groupIndex}%", valueData.Translation);
                        }
                    }

                    newValue.Add(newLineText);
                }
                else if (lineData.TranslationText == null || lineData.TranslationText == line || line.IsSoundsText())
                {
                    // when null,=original or issoundstext. jusst insert original line
                    newValue.Add(line);
                }
                else newValue.Add(lineData.TranslationText);

                lineNum++;
            }

            // set new row value
            row.SetValue(1, string.Join(Environment.NewLine, newValue));

            if (row.HasAnyTranslationLineValidAndEqualSameOrigLine(false)) return false; // continue if any original line equal to translation line with same index

            // apply fixes for cell
            // apply only for finished rows
            new AllHardFixes().Selected(row, tableIndex, rowData.RowIndex);
            new FixCells().Selected(row, tableIndex, rowData.RowIndex);

            return true;
        }

        ///// <summary>
        ///// merge extracted strings to result merged translation
        ///// </summary>
        ///// <param name="coordinates"></param>
        ///// <param name="lineNum"></param>
        ///// <param name="line"></param>
        ///// <returns></returns>
        //private string GetMergedValue(string coordinates, int lineNum, string line)
        //{
        //    using (var enumer = _bufferExtracted[coordinates][lineNum].GetEnumerator())
        //    {
        //        enumer.MoveNext();
        //        var extractionkeyvalue = enumer.Current;
        //        var replacement = extractionkeyvalue.Value;

        //        if (Regex.IsMatch(replacement.Trim(), @"^\$[1-9]$"))
        //        {
        //            var bufenum = _buffer[coordinates][lineNum].GetEnumerator();
        //            bufenum.MoveNext();

        //            int indexOfTheString = line.IndexOf(bufenum.Current.Key);
        //            if (indexOfTheString > -1)
        //            {
        //                if (bufenum.Current.Value != null)//null when o translation?
        //                {
        //                    return line
        //                    .Remove(indexOfTheString, bufenum.Current.Key.Length)
        //                    .Insert(indexOfTheString,
        //                    bufenum.Current.Value
        //                    );
        //                }
        //                else
        //                {
        //                    return line;
        //                }
        //            }
        //        }

        //        KeyValuePair<string, string> substitution;
        //        while (enumer.MoveNext())
        //        {
        //            substitution = enumer.Current;

        //            if (replacement.Contains(substitution.Key))
        //            {
        //                string val = substitution.Value;
        //                if (_buffer[coordinates][lineNum].ContainsKey(substitution.Value))
        //                    val = _buffer[coordinates][lineNum][substitution.Value];

        //                replacement = replacement.Replace(substitution.Key, val);
        //            }
        //        }

        //        return replacement;
        //    }
        //}
    }
}

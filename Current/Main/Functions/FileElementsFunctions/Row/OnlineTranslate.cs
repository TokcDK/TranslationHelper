using GoogleTranslateFreeApi;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class OnlineTranslate : RowBase
    {
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

        readonly Dictionary<string/*table index,row index*/, Dictionary<int/*line number*/, Dictionary<string/*text from original*/, string/*text of translation*/>>> _buffer;
        readonly Dictionary<string/*table index,row index*/, Dictionary<int/*line number*/, Dictionary<string/*text from original*/, string/*text of translation*/>>> _bufferExtracted;

        int Size { get; set; }
        static int MaxSize { get => 1000; }

        bool IsMax() { return Size >= MaxSize; }

        public OnlineTranslate()
        {
            if (_buffer == null) _buffer = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            if (_bufferExtracted == null) _bufferExtracted = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            if (_translator == null) _translator = new GoogleAPIOLD();
        }

        protected override bool IsValidRow()
        {
            return !AppSettings.InterruptTtanslation && base.IsValidRow()
                && (string.IsNullOrEmpty(Translation)
                || Original.HasAnyTranslationLineValidAndEqualSameOrigLine(Translation));
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
            _buffer.Clear();
            AppData.OnlineTranslationCache.Write();
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
            foreach (var line in original.SplitToLines())
            {
                var lineCoordinates = SelectedTableIndex + "," + SelectedRowIndex;

                //init data
                //add lineCoordinates and row data
                _buffer.TryAdd(lineCoordinates, new Dictionary<int, Dictionary<string, string>>());//add coordinates

                _buffer[lineCoordinates].TryAdd(lineNum, new Dictionary<string, string>());//add linenum

                //check for line valid
                if (!line.IsValidForTranslation())
                {
                    _buffer[lineCoordinates][lineNum].Add(line, line);//add original as translation because line is not valid and will be added as is

                    lineNum++;
                    continue;
                }

                //check line value in cache
                var linecache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(line);

                var values = line.ExtractMulty(lineCoordinates, lineNum, _bufferExtracted);

                //moved check cache here because when cache is enabled and value can be extracted then need to parse extracted instead of original vale
                if (!string.IsNullOrEmpty(linecache) && values.Length == 0)
                {
                    _buffer[lineCoordinates][lineNum].Add(line, linecache);
                    lineNum++;
                    continue;
                }

                //parse all extracted values from original
                foreach (var val in values)
                {
                    if (_buffer[lineCoordinates][lineNum].ContainsKey(val)) continue;

                    var valcache = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(val);

                    if (!string.IsNullOrEmpty(valcache))
                    {
                        _buffer[lineCoordinates][lineNum].Add(val, valcache);
                    }
                    else if (val.IsSoundsText())
                    {
                        _buffer[lineCoordinates][lineNum].Add(val, val);
                    }
                    else
                    {
                        _buffer[lineCoordinates][lineNum].Add(val, null);
                        Size += val.Length;
                    }
                }

                if (IsMax())
                {
                    TranslateStrings();

                    Size = 0;
                    _buffer.Clear();

                    //write cache periodically
                    AppData.OnlineTranslationCache.Write();

                    // stop translating after last pack of text when translation is interrupted
                    if (AppSettings.InterruptTtanslation) return;
                }

                lineNum++;
            }

            //translate if is last row or was added 300+ rows to buffer
            if (!IsLastRow && _buffer.Count < 300) return;

            TranslateStrings();

            Size = 0;
            _buffer.Clear();

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
            foreach (var coordinate in _buffer)//get all coordinate keys
            {
                foreach (var lineNumber in coordinate.Value) // get all sublines
                {
                    foreach (var linetext in lineNumber.Value) // get all sublines
                    {
                        if (!orig.Contains(linetext.Key) && linetext.Value == null && linetext.Key.IsValidForTranslation()) orig.Add(linetext.Key);
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
        private string[] ApplyProjectPretranslationAction(string[] originalLines)
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
        private string[] ApplyProjectPosttranslationAction(string[] originalLines, string[] translatedLines)
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
                for (int i = 0; i < originals.Length; i++)
                {
                    translations.Add(originals[i], translated[i]);

                    FunctionsOnlineCache.TryAdd(originals[i], translated[i]);
                }

            var coordinates = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>(_buffer);
            foreach (var coordinate in coordinates)//get all coordinate keys
            {
                var lineNumbers = new Dictionary<int, Dictionary<string, string>>(coordinate.Value);
                foreach (var linenumber in lineNumbers) // get all sublines
                {
                    var liTexts = new Dictionary<string, string>(linenumber.Value);
                    foreach (var linetext in liTexts) // get all sublines
                    {
                        if (linetext.Value == null && translations.ContainsKey(linetext.Key))
                        {
                            _buffer[coordinate.Key][linenumber.Key][linetext.Key] = translations[linetext.Key];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// set buffer content to rows by coordinates
        /// </summary>
        private void SetBufferToRows()
        {
            var coordinates = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>(_buffer);

            //get all coordinate keys
            foreach (var coordinate in coordinates)
            {
                var tr = coordinate.Key.Split(',');
                var tindex = int.Parse(tr[0]);
                var rindex = int.Parse(tr[1]);
                var row = AppData.CurrentProject.FilesContent.Tables[tindex].Rows[rindex];

                var o = row[0] + "";
                var t = row[1] + "";
                var cellTranslationEqualOriginal = Equals(t, o);

                //skip equal
                if (AppSettings.IgnoreOrigEqualTransLines && cellTranslationEqualOriginal) continue;

                var cellTranslationIsNotEmptyAndNotEqualOriginal = (!string.IsNullOrEmpty(o) && !cellTranslationEqualOriginal);

                if (cellTranslationIsNotEmptyAndNotEqualOriginal && !o.HasAnyTranslationLineValidAndEqualSameOrigLine(t,false)) continue;

                var newValue = new List<string>();
                var lineNum = 0;
                var rowValue = cellTranslationIsNotEmptyAndNotEqualOriginal ? t : o;
                foreach (var line in rowValue.SplitToLines())
                {
                    if ((!coordinate.Value.ContainsKey(lineNum) || coordinate.Value[lineNum].Count == 0) || line.IsSoundsText())
                    {
                        newValue.Add(line);
                    }
                    else if (_bufferExtracted != null
                        && _bufferExtracted.Count > 0
                        && _bufferExtracted.ContainsKey(coordinate.Key)
                        && _bufferExtracted[coordinate.Key].ContainsKey(lineNum)
                        && _bufferExtracted[coordinate.Key][lineNum].Count > 0
                        )
                    {
                        newValue.Add(GetMergedValue(coordinate.Key, lineNum, line));

                    }
                    else if (coordinate.Value[lineNum].ContainsKey(line))
                    {
                        newValue.Add(coordinate.Value[lineNum][line]);
                    }
                    else
                    {
                        newValue.Add(line);
                    }

                    lineNum++;
                }

                row.SetValue(1, string.Join(Environment.NewLine, newValue));

                if (!o.HasAnyTranslationLineValidAndEqualSameOrigLine(t,false))//apply only for finished rows
                {
                    //apply fixes for cell
                    new AllHardFixes().Selected(row, tindex, rindex);
                    new FixCells().Selected(row, tindex, rindex);
                }
            }
        }

        /// <summary>
        /// merge extracted strings to result merged translation
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="lineNum"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetMergedValue(string coordinates, int lineNum, string line)
        {
            using (var enumer = _bufferExtracted[coordinates][lineNum].GetEnumerator())
            {
                enumer.MoveNext();
                var extractionkeyvalue = enumer.Current;
                var replacement = extractionkeyvalue.Value;

                if (Regex.IsMatch(replacement.Trim(), @"^\$[1-9]$"))
                {
                    var bufenum = _buffer[coordinates][lineNum].GetEnumerator();
                    bufenum.MoveNext();

                    int indexOfTheString = line.IndexOf(bufenum.Current.Key);
                    if (indexOfTheString > -1)
                    {
                        if (bufenum.Current.Value != null)//null when o translation?
                        {
                            return line
                            .Remove(indexOfTheString, bufenum.Current.Key.Length)
                            .Insert(indexOfTheString,
                            bufenum.Current.Value
                            );
                        }
                        else
                        {
                            return line;
                        }
                    }
                }

                KeyValuePair<string, string> substitution;
                while (enumer.MoveNext())
                {
                    substitution = enumer.Current;

                    if (replacement.Contains(substitution.Key))
                    {
                        string val = substitution.Value;
                        if (_buffer[coordinates][lineNum].ContainsKey(substitution.Value))
                            val = _buffer[coordinates][lineNum][substitution.Value];

                        replacement = replacement.Replace(substitution.Key, val);
                    }
                }

                return replacement;
            }
        }
    }
}

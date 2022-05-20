﻿using System;
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

        bool IsMax()
        {
            return Size >= MaxSize;
        }

        public OnlineTranslate()
        {
            if (_buffer == null)
            {
                _buffer = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            }
            if (_bufferExtracted == null)
            {
                _bufferExtracted = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            }
            if (_translator == null)
            {
                _translator = new GoogleAPIOLD();
            }
        }

        protected override bool IsValidRow()
        {
            return !Properties.Settings.Default.InterruptTtanslation && base.IsValidRow()
                && (SelectedRow[1] == null || string.IsNullOrEmpty(SelectedRow[1] + "")
                || SelectedRow.HasAnyTranslationLineValidAndEqualSameOrigLine());

            //return (b = base.IsValidRow()) || (!b && AnyLineValidForTranslation());
        }

        //private bool AnyLineValidForTranslation()
        //{
        //    var t = SelectedRow[1] + "";
        //    foreach (var line in t.SplitToLines())
        //    {
        //        if (line.IsValidForTranslation())
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// true if all DB was loaded
        /// </summary>
        bool _allDbLoaded4All;
        protected override void ActionsInit()
        {
            FunctionsOnlineCache.Init();

            if (Properties.Settings.Default.UseAllDBFilesForOnlineTranslationForAll && IsAll && !_allDbLoaded4All)
            {
                if (!Properties.Settings.Default.EnableTranslationCache)
                {
                    //cache disabled but all db loading enabled. ask for load then. maybe not need
                    var result = MessageBox.Show(T._("Translation cache disabled but load all DB enabled. While all DB loading cache can be enabled in settings. Load all DB?"), T._("Translation cache disabled"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) return;
                }

                ProjectData.Main.ProgressInfo(true, "Get all DB");

                var mergingAllDb = new Task(() => FunctionsDBFile.MergeAllDBtoOne());
                mergingAllDb.ConfigureAwait(true);
                mergingAllDb.Start();
                mergingAllDb.Wait();
                _allDbLoaded4All = true;

                ProjectData.Main.ProgressInfo(false);
            }
        }
        protected override void ActionsFinalize()
        {
            TranslateStrings();
            Size = 0;
            _buffer.Clear();
            ProjectData.OnlineTranslationCache.Write();
            FunctionsOnlineCache.Unload();

            ProjectData.Main.ProgressInfo(false);

            if (Properties.Settings.Default.InterruptTtanslation) Properties.Settings.Default.InterruptTtanslation = false;
        }

        protected override bool Apply()
        {
            //if (SelectedRow[1] == null || (SelectedRow[1] + string.Empty).Length == 0 || SelectedRow.HasAnyTranslationLineValidAndEqualSameOrigLine())
            try
            {
                //if (SelectedRow[1] == null || string.IsNullOrEmpty(SelectedRow[1] + "") || SelectedRow.HasAnyTranslationLineValidAndEqualSameOrigLine(false))//translate only empty rows or rows where can be something translated
                {
                    ProjectData.Main.ProgressInfo(true, "Translate" + " " + SelectedTable.TableName + "/" + SelectedRowIndex);

                    SetRowLinesToBuffer();

                    ProjectData.Main.ProgressInfo(false);
                    return true;
                }
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
            foreach (var line in original.SplitToLines())
            {
                var lineCoordinates = SelectedTableIndex + "," + SelectedRowIndex;

                //add lineCoordinates and row data
                if (!_buffer.ContainsKey(lineCoordinates))
                {
                    //init data
                    _buffer.Add(lineCoordinates, new Dictionary<int, Dictionary<string, string>>());//add coordinates
                }
                if (!_buffer[lineCoordinates].ContainsKey(lineNum))
                {
                    _buffer[lineCoordinates].Add(lineNum, new Dictionary<string, string>());//add linenum
                }

                //check for line valid
                if (!line.IsValidForTranslation())
                {
                    _buffer[lineCoordinates][lineNum].Add(line, line);//add original as translation because line is not valid and will be added as is

                    lineNum++;
                    continue;
                }

                //check line value in cache
                var linecache = ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(line);
                if (!string.IsNullOrEmpty(linecache))
                {
                    _buffer[lineCoordinates][lineNum].Add(line, linecache);
                    lineNum++;
                    continue;
                }

                var values = line.ExtractMulty(lineCoordinates, lineNum, _bufferExtracted);

                //parse all extracted values from original
                foreach (var val in values)
                {
                    try
                    {
                        if (!_buffer[lineCoordinates][lineNum].ContainsKey(val))
                        {
                            var valcache = ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(val);

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
                    }
                    catch
                    {
                    }
                }

                if (IsMax())
                {
                    try
                    {
                        TranslateStrings();
                    }
                    catch
                    {
                    }
                    Size = 0;
                    _buffer.Clear();

                    //write cache periodically
                    ProjectData.OnlineTranslationCache.Write();

                    if (Properties.Settings.Default.InterruptTtanslation) // stop translating after last pack of text when translation is interrupted
                    {
                        return;
                    }
                }

                lineNum++;
            }

            //translate if is last row or was added 300+ rows to buffer
            if (IsLastRow || _buffer.Count >= 300)
            {
                try
                {
                    TranslateStrings();
                }
                catch
                {
                }
                Size = 0;
                _buffer.Clear();

                //write cache periodically
                ProjectData.OnlineTranslationCache.Write();
            }
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
                        if (!orig.Contains(linetext.Key) && linetext.Value == null && linetext.Key.IsValidForTranslation())
                        {
                            orig.Add(linetext.Key);
                        }
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
            if (ProjectData.CurrentProject.HideVARSMatchCollectionsList != null && ProjectData.CurrentProject.HideVARSMatchCollectionsList.Count > 0)
            {
                ProjectData.CurrentProject.HideVARSMatchCollectionsList.Clear();//clean of found maches collections
            }

            var newOriginalLines = new string[originalLines.Length];
            Array.Copy(originalLines, newOriginalLines, originalLines.Length);
            for (int i = 0; i < newOriginalLines.Length; i++)
            {
                var s = ProjectData.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], string.Empty);
                if (!string.IsNullOrEmpty(s))
                {
                    newOriginalLines[i] = s;
                }
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
                var s = ProjectData.CurrentProject.OnlineTranslationProjectSpecificPosttranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(s) && s != translatedLines[i])
                {
                    translatedLines[i] = s;
                }
            }

            if (ProjectData.CurrentProject.HideVARSMatchCollectionsList != null && ProjectData.CurrentProject.HideVARSMatchCollectionsList.Count > 0)
            {
                ProjectData.CurrentProject.HideVARSMatchCollectionsList.Clear();//clean of found maches collections
            }

            return translatedLines;
        }

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
                    if (translated == null || originals.Length != translated.Length)
                    {
                        return new string[1] { "" };
                    }
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

                    FunctionsOnlineCache.AddToTranslationCacheIfValid(originals[i], translated[i]);
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
            foreach (var coordinate in coordinates)//get all coordinate keys
            {
                var tr = coordinate.Key.Split(',');
                var tindex = int.Parse(tr[0]);
                var rindex = int.Parse(tr[1]);
                var row = ProjectData.FilesContent.Tables[tindex].Rows[rindex];
                //var linenumMax = (Row[0] + "").GetLinesCount();

                var cellTranslationEqualOriginal = Equals(row[1], row[0]);
                if (Properties.Settings.Default.IgnoreOrigEqualTransLines && cellTranslationEqualOriginal)//skip equal
                {
                    continue;
                }

                var cellTranslationIsNotEmptyAndNotEqualOriginal = (row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !cellTranslationEqualOriginal);

                if (cellTranslationIsNotEmptyAndNotEqualOriginal && !row.HasAnyTranslationLineValidAndEqualSameOrigLine(false))
                {
                    continue;
                }

                try
                {
                    var newValue = new List<string>();
                    var lineNum = 0;
                    var rowValue = (cellTranslationIsNotEmptyAndNotEqualOriginal ? row[1] : row[0]) + "";
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

                    if (!row.HasAnyTranslationLineValidAndEqualSameOrigLine(false))//apply only for finished rows
                    {
                        //apply fixes for cell
                        new AllHardFixes().Selected(row, tindex, rindex);
                        new FixCells().Selected(row, tindex, rindex);
                    }
                }
                catch
                {
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
                //var pattern = extractionkeyvalue.Key;
                var replacement = extractionkeyvalue.Value;
                //var regex = new Regex(pattern);
                //var match = regex.Match(line);
                //var replace = regex.Replace(line, replacement);

                //var replacementmatches = Regex.Matches(replacement, @"\$[1,9]");

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

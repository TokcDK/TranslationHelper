using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class OnlineTranslate : FileElementsRowFunctionsBase
    {
        /// <summary>
        /// buffer for lines
        /// </summary>
        internal class Buffer
        {
            internal string GetOriginal { get; }
            internal string GetTranslation { get; set; }
            internal bool GetIsExtracted { get; }

            internal Buffer(string Original, string Translation, bool IsExtracted)
            {
                GetOriginal = Original;
                GetTranslation = Translation;
                GetIsExtracted = IsExtracted;
            }
        }

        Dictionary<string/*table index,row index*/, Dictionary<int/*line number*/, Dictionary<string/*text from original*/, string/*text of translation*/>>> buffer;
        Dictionary<string/*table index,row index*/, Dictionary<int/*line number*/, Dictionary<string/*text from original*/, string/*text of translation*/>>> bufferExtracted;

        int Size { get; set; }
        static int MaxSize { get => 1000; }

        bool IsMax()
        {
            return Size >= MaxSize;
        }

        public OnlineTranslate(THDataWork thDataWork) : base(thDataWork)
        {
            if (buffer == null)
            {
                buffer = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            }
            if (bufferExtracted == null)
            {
                bufferExtracted = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            }
            if (Translator == null)
            {
                Translator = new GoogleAPIOLD(thDataWork);
            }
        }

        bool AllDBLoaded4All;
        protected override void ActionsPreRowsApply()
        {
            FunctionsOnlineCache.Init(thDataWork);

            if (Properties.Settings.Default.UseAllDBFilesForOnlineTranslationForAll && IsAll && !AllDBLoaded4All)
            {
                thDataWork.Main.ProgressInfo(true, "Get all DB");
                var mergingAllDB = new Task(() => FunctionsDBFile.MergeAllDBtoOne(thDataWork));
                mergingAllDB.ConfigureAwait(true);
                mergingAllDB.Start();
                mergingAllDB.Wait();
                AllDBLoaded4All = true;
            }
        }
        protected override void ActionsPostRowsApply()
        {
            TranslateStrings();
            Size = 0;
            buffer.Clear();
            thDataWork.OnlineTranslationCache.Write();
            FunctionsOnlineCache.Unload(thDataWork);
        }
        protected override bool Apply()
        {
            if (SelectedRow[1] == null || (SelectedRow[1] + string.Empty).Length == 0)
            {
                thDataWork.Main.ProgressInfo(true, "Translate" + " " + SelectedTable.TableName + "/" + SelectedRowIndex);

                SetRowLinesToBuffer();

                thDataWork.Main.ProgressInfo(false);

                return true;
            }

            return false;
        }

        private void SetRowLinesToBuffer()
        {
            var original = SelectedRow[0] as string;
            var lineNum = 0;
            foreach (var line in original.SplitToLines())
            {
                if (!IsValidForTranslation(line))
                {
                    lineNum++;
                    continue;
                }

                var lineCoordinates = SelectedTableIndex + "," + SelectedRowIndex;

                //add lineCoordinates and row data
                if (!buffer.ContainsKey(lineCoordinates))
                {
                    //init data
                    buffer.Add(lineCoordinates, new Dictionary<int, Dictionary<string, string>>());//add coordinates
                }
                if (!buffer[lineCoordinates].ContainsKey(lineNum))
                {
                    buffer[lineCoordinates].Add(lineNum, new Dictionary<string, string>());//add linenum
                }

                //check line value in cache
                var linecache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(line);
                if (!string.IsNullOrEmpty(linecache))
                {
                    buffer[lineCoordinates][lineNum].Add(line, linecache);
                    lineNum++;
                    continue;
                }

                var values = ExtractMulty(line, lineCoordinates, lineNum);

                foreach (var val in values)
                {
                    try
                    {
                        if (!buffer[lineCoordinates][lineNum].ContainsKey(val))
                        {
                            var valcache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(val);

                            if (!string.IsNullOrEmpty(valcache))
                            {
                                buffer[lineCoordinates][lineNum].Add(val, valcache);
                            }
                            else
                            {
                                buffer[lineCoordinates][lineNum].Add(val, null);
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
                    buffer.Clear();

                    //write cache periodically
                    thDataWork.OnlineTranslationCache.Write();
                }

                lineNum++;
            }
        }

        /// <summary>
        /// extract captured groups from string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineCoordinates"></param>
        /// <param name="lineNum"></param>
        /// <returns></returns>
        private string[] ExtractMulty(string line, string lineCoordinates, int lineNum)
        {
            if (!bufferExtracted.ContainsKey(lineCoordinates))
            {
                //init data
                bufferExtracted.Add(lineCoordinates, new Dictionary<int, Dictionary<string, string>>());//add coordinates
            }
            if (!bufferExtracted[lineCoordinates].ContainsKey(lineNum))
            {
                //init data
                bufferExtracted[lineCoordinates].Add(lineNum, new Dictionary<string, string>());//add linenum
            }

            var l = new List<string>();
            foreach (var PatternReplacementPair in thDataWork.TranslationRegexRules)
            {
                if (!Regex.IsMatch(line, PatternReplacementPair.Key))
                {
                    continue;
                }

                foreach (Group g in Regex.Match(line, PatternReplacementPair.Key).Groups)
                {
                    try
                    {

                        if (!bufferExtracted[lineCoordinates][lineNum].ContainsKey(PatternReplacementPair.Key))//add pattern-replacement data
                        {
                            bufferExtracted[lineCoordinates][lineNum].Add(PatternReplacementPair.Key, PatternReplacementPair.Value);
                        }

                        if (!bufferExtracted[lineCoordinates][lineNum].ContainsKey("$" + g.Name))
                            bufferExtracted[lineCoordinates][lineNum].Add("$" + g.Name, g.Value);

                        if (IsValidForTranslation(g.Value) && PatternReplacementPair.Value.Contains("$" + g.Name))
                            l.Add(g.Value);
                    }
                    catch
                    {

                    }
                }

                break;
            }

            if (l.Count > 0)
            {
                return l.ToArray();
            }
            else
            {
                return new string[1] { line };
            }
        }

        /// <summary>
        /// true if input string is valid
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static bool IsValidForTranslation(string inputString)
        {
            return !string.IsNullOrWhiteSpace(inputString) && !inputString.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther();
        }

        readonly GoogleAPIOLD Translator;
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
            foreach (var coordinate in buffer)//get all coordinate keys
            {
                foreach (var linenumber in coordinate.Value) // get all sublines
                {
                    foreach (var linetext in linenumber.Value) // get all sublines
                    {
                        if (!orig.Contains(linetext.Key) && linetext.Value == null && IsValidForTranslation(linetext.Key))
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
            if (thDataWork.CurrentProject.HideVARSMatchCollectionsList != null && thDataWork.CurrentProject.HideVARSMatchCollectionsList.Count > 0)
            {
                thDataWork.CurrentProject.HideVARSMatchCollectionsList.Clear();//clean of found maches collections
            }

            for (int i = 0; i < originalLines.Length; i++)
            {
                var s = thDataWork.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], string.Empty);
                if (!string.IsNullOrEmpty(s))
                {
                    originalLines[i] = s;
                }
            }
            return originalLines;
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
                var s = thDataWork.CurrentProject.OnlineTranslationProjectSpecificPosttranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(s) && s != translatedLines[i])
                {
                    translatedLines[i] = s;
                }
            }

            if (thDataWork.CurrentProject.HideVARSMatchCollectionsList != null && thDataWork.CurrentProject.HideVARSMatchCollectionsList.Count > 0)
            {
                thDataWork.CurrentProject.HideVARSMatchCollectionsList.Clear();//clean of found maches collections
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
                var OriginalLinesPreapplied = ApplyProjectPretranslationAction(originals);
                if (OriginalLinesPreapplied.Length > 0)
                {
                    translated = Translator.Translate(OriginalLinesPreapplied);
                    if (translated == null || originals.Length != translated.Length)
                    {
                        return new string[1] { "" };
                    }
                    translated = ApplyProjectPosttranslationAction(originals, translated);
                }
            }
            catch (Exception ex)
            {
                new FunctionsLogs().LogToFile("Error while translation:"
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

                    FunctionsOnlineCache.AddToTranslationCacheIfValid(thDataWork, originals[i], translated[i]);
                }

            var Coordinates = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>(buffer);
            foreach (var coordinate in Coordinates)//get all coordinate keys
            {
                var lineNumbers = new Dictionary<int, Dictionary<string, string>>(coordinate.Value);
                foreach (var linenumber in lineNumbers) // get all sublines
                {
                    var liTexts = new Dictionary<string, string>(linenumber.Value);
                    foreach (var linetext in liTexts) // get all sublines
                    {
                        if (linetext.Value == null && translations.ContainsKey(linetext.Key))
                        {
                            buffer[coordinate.Key][linenumber.Key][linetext.Key] = translations[linetext.Key];
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
            var Coordinates = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>(buffer);
            foreach (var coordinate in Coordinates)//get all coordinate keys
            {
                var TR = coordinate.Key.Split(',');
                var tindex = int.Parse(TR[0]);
                var rindex = int.Parse(TR[1]);
                var Row = thDataWork.THFilesElementsDataset.Tables[tindex].Rows[rindex];
                //var linenumMax = (Row[0] + "").GetLinesCount();

                try
                {
                    var newValue = new List<string>();
                    var lineNum = 0;
                    var rowValue = ((Row[1] != null && !string.IsNullOrEmpty(Row[1] as string) && !Equals(Row[1], Row[0])) ? Row[1] : Row[0]) + "";
                    foreach (var line in rowValue.SplitToLines())
                    {
                        if ((!coordinate.Value.ContainsKey(lineNum) || coordinate.Value[lineNum].Count == 0))
                        {
                            newValue.Add(line);
                        }
                        else if (coordinate.Value[lineNum].Count > 0
                            && bufferExtracted != null
                            && bufferExtracted.Count > 0
                            && bufferExtracted.ContainsKey(coordinate.Key)
                            && bufferExtracted[coordinate.Key].ContainsKey(lineNum)
                            && bufferExtracted[coordinate.Key][lineNum].Count > 0
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

                    Row[1] = string.Join(Environment.NewLine, newValue);

                    //apply fixes for cell
                    new AllHardFixes(thDataWork).Selected(Row);
                    new FixCells(thDataWork).Selected(Row);
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
            using (var enumer = bufferExtracted[coordinates][lineNum].GetEnumerator())
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
                    var bufenum = buffer[coordinates][lineNum].GetEnumerator();
                    bufenum.MoveNext();

                    int IndexOfTheString = line.IndexOf(bufenum.Current.Key);
                    if (IndexOfTheString > -1)
                    {
                        return line
                            .Remove(IndexOfTheString, bufenum.Current.Key.Length)
                            .Insert(IndexOfTheString,
                            bufenum.Current.Value
                            );
                    }
                }

                KeyValuePair<string, string> substitution;
                while (enumer.MoveNext())
                {
                    substitution = enumer.Current;

                    if (replacement.Contains(substitution.Key))
                    {
                        string val = substitution.Value;
                        if (buffer[coordinates][lineNum].ContainsKey(substitution.Value))
                            val = buffer[coordinates][lineNum][substitution.Value];

                        replacement = replacement.Replace(substitution.Key, val);
                    }
                }

                return replacement;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

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
        protected override void ActionsPostRowsApply()
        {
            TranslateAddedStrings();
            Size = 0;
            buffer.Clear();
        }

        protected override bool Apply()
        {
            if (SelectedRow[1] == null || (SelectedRow[1] + string.Empty).Length == 0)
            {
                SetRowLinesToBuffer();

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

                var values = MultyExtract(line, lineCoordinates, lineNum);

                foreach (var val in values)
                {
                    //if (!IsValidForTranslation(val))
                    //{
                    //    continue;
                    //}

                    try
                    {
                        if (!buffer[lineCoordinates][lineNum].ContainsKey(val))
                        {
                            buffer[lineCoordinates][lineNum].Add(val, null);
                            Size += val.Length;
                        }
                    }
                    catch
                    {
                    }

                    if (IsMax())
                    {
                        try
                        {
                            TranslateAddedStrings();
                        }
                        catch
                        {
                        }
                        Size = 0;
                        buffer.Clear();
                    }
                }

                lineNum++;
            }
        }

        private string[] MultyExtract(string line, string lineCoordinates, int lineNum)
        {
            if (!bufferExtracted.ContainsKey(lineCoordinates))
            {
                //init data
                bufferExtracted.Add(lineCoordinates, new Dictionary<int, Dictionary<string, string>>());//add coordinates
                bufferExtracted[lineCoordinates].Add(lineNum, new Dictionary<string, string>());//add linenum
            }

            var l = new List<string>();
            foreach (var PatternReplacementPair in thDataWork.TranslationRegexRules)
            {
                if (!Regex.IsMatch(line, PatternReplacementPair.Key))
                {
                    continue;
                }

                //int lastGroupInd = 0;
                //bool skippedgroup0 = false;
                //var groupInd = 0;
                foreach (Group g in Regex.Match(line, PatternReplacementPair.Key).Groups)
                {
                    //if (!skippedgroup0)
                    //{
                    //    skippedgroup0 = true;
                    //    continue;
                    //}

                    //if (g.Index < lastGroupInd)
                    //{
                    //    continue;
                    //}

                    //var gname=g.Name;

                    if (!bufferExtracted[lineCoordinates][lineNum].ContainsKey(PatternReplacementPair.Key))//add pattern-replacement data
                    {
                        bufferExtracted[lineCoordinates][lineNum].Add(PatternReplacementPair.Key, PatternReplacementPair.Value);
                    }

                    bufferExtracted[lineCoordinates][lineNum].Add("$" + g.Name, g.Value);

                    if (IsValidForTranslation(g.Value) && PatternReplacementPair.Value.Contains("$" + g.Name))
                        l.Add(g.Value);

                    //groupInd++;
                    //lastGroupInd = g.Index + g.Length;
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

        private static bool IsValidForTranslation(string line)
        {
            return !string.IsNullOrWhiteSpace(line) && !line.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther();
        }

        readonly GoogleAPIOLD Translator;
        private void TranslateAddedStrings()
        {
            var originals = GetOriginals();

            var translated = TranslateOriginals(originals);

            if (translated != null && translated.Length > 0)
            {
                SetTranslationsToBuffer(originals, translated);

                SetBufferToRows();
            }
        }

        private void SetBufferToRows()
        {
            var Coordinates = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>(buffer);
            foreach (var coordinate in Coordinates)//get all coordinate keys
            {
                var TR = coordinate.Key.Split(',');
                var Row = thDataWork.THFilesElementsDataset.Tables[int.Parse(TR[0])].Rows[int.Parse(TR[1])];
                var linenumMax = (Row[0] + "").GetLinesCount();

                try
                {
                    var newValue = new List<string>();
                    var lineNum = 0;
                    var rowValue = ((Row[1] != null && !string.IsNullOrEmpty(Row[1] as string) && !Equals(Row[1], Row[0])) ? Row[1] : Row[0]) + "";
                    foreach (var line in rowValue.SplitToLines())
                    {
                        if (!coordinate.Value.ContainsKey(lineNum) || coordinate.Value[lineNum].Count == 0)
                        {
                            newValue.Add(line);
                        }
                        else if (coordinate.Value[lineNum].Count > 0
                            && bufferExtracted != null
                            && bufferExtracted.Count > 0
                            && bufferExtracted.ContainsKey(coordinate.Key)
                            && bufferExtracted[coordinate.Key].ContainsKey(lineNum)
                            && bufferExtracted[coordinate.Key][lineNum].Count>0
                            )
                        {
                            newValue.Add(GetMergedValue(coordinate.Key, lineNum));

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
                }
                catch
                {
                }
            }
        }

        private string GetMergedValue(string coordinates, int lineNum)
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

                KeyValuePair<string, string> substitution;
                while (enumer.MoveNext())
                {
                    substitution = enumer.Current;

                    if(replacement.Contains(substitution.Key))
                    {
                        string val = substitution.Value;
                        if (buffer[coordinates][lineNum].ContainsKey(substitution.Value))
                            val = buffer[coordinates][lineNum][substitution.Value];

                        replacement = replacement.Replace(substitution.Key, val);
                    }
                }

                //int num = 0;
                //foreach (var k in buffer[coordinates][lineNum])
                //{
                //    while (bufferExtracted[coordinates][lineNum]["$" + num] != k.Key)
                //    {
                //        num++;
                //    }

                //    replacement = replacement.Replace("$" + num, k.Value ?? k.Key);
                //    num++;
                //}

                return replacement;
            }
        }

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

        private void SetTranslationsToBuffer(string[] originals, string[] translated)
        {
            var translations = new Dictionary<string, string>();
            for (int i = 0; i < originals.Length; i++)
            {
                translations.Add(originals[i], translated[i]);
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
    }
}

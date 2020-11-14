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

                //foreach (var val in values)
                //{
                //    buffer[lineCoordinates][lineNum].Add(val, null);
                //}

                buffer[lineCoordinates][lineNum].Add(line, null);
                Size += line.Length;

                if (IsMax())
                {
                    TranslateAddedStrings();
                    Size = 0;
                    buffer.Clear();
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
                    return new string[1] { line };
                }

                int lastGroupInd = 0;
                foreach (Group g in Regex.Match(line, PatternReplacementPair.Key).Groups)
                {
                    if (g.Index < lastGroupInd)
                    {
                        continue;
                    }

                    bufferExtracted[lineCoordinates][lineNum].Add(g.Value, PatternReplacementPair.Value);
                    l.Add(g.Value);
                    lastGroupInd = g.Index + g.Length;
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
            return !string.IsNullOrWhiteSpace(line);
        }

        readonly GoogleAPIOLD Translator;
        private void TranslateAddedStrings()
        {
            var originals = GetOriginals();

            string[] translated = TranslateOriginals(originals);

            SetTranslationsToBuffer(originals, translated);

            SetTranslationsToRows();
        }

        private void SetTranslationsToRows()
        {
            var Coordinates = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>(buffer);
            foreach (var coordinate in Coordinates)//get all coordinate keys
            {
                var TR = coordinate.Key.Split(',');
                var Row = thDataWork.THFilesElementsDataset.Tables[int.Parse(TR[0])].Rows[int.Parse(TR[1])];
                var linenumMax = (Row[0] + "").GetLinesCount();

                if (coordinate.Value.Count != linenumMax)
                {
                    continue;
                }

                try
                {
                    var newValue = new List<string>();
                    var lineNum = 0;
                    foreach (var line in (Row[0] + "").SplitToLines())
                    {
                        if (!coordinate.Value.ContainsKey(lineNum) || coordinate.Value[lineNum].Count == 0)
                        {
                            continue;
                        }

                        if (coordinate.Value[lineNum].Count > 1)
                        {
                            if (bufferExtracted != null && bufferExtracted.Count > 0 && bufferExtracted.ContainsKey(coordinate.Key) && bufferExtracted[coordinate.Key].ContainsKey(lineNum))
                            {
                            }
                        }
                        else
                        {
                            if (coordinate.Value[lineNum].ContainsKey(line))
                            {
                                newValue.Add(coordinate.Value[lineNum][line]);
                            }
                            else
                            {
                                newValue.Add(line);
                            }
                        }

                        lineNum++;
                    }

                    Row[1] = string.Join(Environment.NewLine, newValue);
                    buffer.Remove(coordinate.Key);
                }
                catch
                {
                }
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
                        if (linetext.Value == null)
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
                        if (!orig.Contains(linetext.Key) && linetext.Value == null)
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

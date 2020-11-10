using System;
using System.Collections.Generic;
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

        Dictionary<int[], string[]> buffer;

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
                buffer = new Dictionary<int[], string[]>();
            }
            if (Translator == null)
            {
                Translator = new GoogleAPIOLD(thDataWork);
            }
        }

        protected override bool Apply()
        {
            if (SelectedRow[1] == null || (SelectedRow[1] as string).Length == 0)
            {
                var original = SelectedRow[0] as string;
                var lineNum = 0;
                foreach (var line in original.SplitToLines())
                {
                    var lineCoordinates = new int[3] { SelectedTableIndex, SelectedRowIndex, lineNum };

                    //add lineCoordinates and row data
                    if (!buffer.ContainsKey(lineCoordinates))
                        buffer.Add(lineCoordinates, new string[2]);

                    buffer[lineCoordinates][0]=line;

                    Size += line.Length;

                    if (IsMax())
                    {
                        TranslateAddedStrings();
                    }

                    lineNum++;
                }
            }

            return false;
        }

        readonly GoogleAPIOLD Translator;
        private void TranslateAddedStrings()
        {
            var originals = GetOriginals();

            string[] translated = null;
            try
            {
                var OriginalLinesPreapplied = ApplyProjectPretranslationAction(originals);
                translated = Translator.Translate(OriginalLinesPreapplied);
                if (translated == null || originals.Length != translated.Length)
                {
                    return;
                }
                translated = ApplyProjectPosttranslationAction(originals, translated);
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

            SetTranslations(translated);
        }

        private void SetTranslations(string[] translated)
        {
            int lineNum = 0;
            foreach (var line in buffer)
            {
                var row = thDataWork.THFilesElementsDataset.Tables[line.Key[0]].Rows[line.Key[0]];
                var rowLineNum = 0;
                var LinesCount = (row[0] as string).GetLinesCount();
                var Translation = "";
                foreach (var subline in (row[0] as string).SplitToLines())
                {
                    rowLineNum++;
                }
                lineNum++;
            }
        }

        private string[] GetOriginals()
        {
            List<string> orig = new List<string>();
            foreach (var line in buffer)
            {
                if (line.Value[1] == null)
                {
                    orig.Add(line.Value[0]);
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.WolfRPG.WolfTrans
{
    class TXT : WolfRPGBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        internal override bool Save()
        {
            thDataWork.SaveFileMode = true;
            return ParseStringFile();
        }

        int lineNum;
        protected override int ParseStringFileLine()
        {
            lineNum++;

            //skip if not patch files
            if (lineNum == 1 && ParseData.line != "> WOLF TRANS PATCH FILE VERSION 1.0")
            {
                return -1;
            }

            //skip if begin string not found
            if (IsNotBeginString())
            {
                SaveModeAddLine("\n");
                return 0;
            }

            //------------------------------------
            //add original
            var originalLines = new List<string>();
            while (!ReadLine().StartsWith("> CONTEXT"))
            {
                originalLines.Add(ParseData.line);
            }

            //------------------------------------
            //add context
            var contextLines = new List<string>
            {
                //add first context line (was set last to line in check of previous block)
                ParseData.line
            };
            //add rest of context lines
            while (ReadLine().StartsWith("> CONTEXT"))
            {
                contextLines.Add(ParseData.line + (ParseData.line.EndsWith(" < UNTRANSLATED") ? string.Empty : " < UNTRANSLATED"));
            }
            //------------------------------------

            //add translation if exists
            var translationLines = new List<string>
            {
                //add last set line in check of previous context block
                ParseData.line
            };
            while (ReadLine() != "> END STRING")
            {
                translationLines.Add(ParseData.line);
            }
            //------------------------------------

            var original = string.Join(Properties.Settings.Default.NewLine, originalLines);
            var translation = string.Join(Properties.Settings.Default.NewLine, translationLines);

            //add begin end string block data
            if (thDataWork.OpenFileMode)
            {
                var context = string.Join(Properties.Settings.Default.NewLine, contextLines);
                if (string.IsNullOrEmpty(translation))
                {
                    AddRowData(original, context, true);
                }
                else
                {
                    AddRowData(new[] { original, translation }, context, true);
                }
            }
            else
            {
                var trans = original;
                var translated = thDataWork.SaveFileMode && IsValidString(original) && SetTranslation(ref trans) && !string.IsNullOrEmpty(trans) && (original != trans || trans != translation);

                if (translated)
                {
                    ParseData.Ret = true;
                    translation = trans;
                }

                SetContext(contextLines, translated);

                ParseData.line = 
                    "> BEGIN STRING"
                    + "\n" +
                    original
                    + "\n" +
                    string.Join("\n", contextLines)
                    + "\n" +
                    translation
                    + "\n" +
                    "> END STRING"
                    ;

                SaveModeAddLine("\n");//add endstring line
            }

            return 0;
        }

        private void SetContext(List<string> contextLines, bool translated)
        {
            for (int i = 0; i < contextLines.Count; i++)
            {
                var ends = contextLines[i].TrimEnd().EndsWith(" < UNTRANSLATED");

                if (translated)
                {
                    if (ends)
                    {
                        contextLines[i] = contextLines[i].Replace(" < UNTRANSLATED", string.Empty);
                    }
                }
                else
                {
                    if (!ends)
                    {
                        contextLines[i] = contextLines[i] + " < UNTRANSLATED";
                    }
                }
            }
        }

        private bool IsNotBeginString()
        {
            return ParseData.line != "> BEGIN STRING";
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && thDataWork.SaveFileMode && ParseData.ResultForWrite.Length > 0)
                {
                    File.WriteAllText(filePath.Length > 0 ? filePath : thDataWork.FilePath, ParseData.ResultForWrite.ToString().Replace(Properties.Settings.Default.NewLine, "\n"), FunctionsFileFolder.GetEncoding(thDataWork.FilePath));
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}

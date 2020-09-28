using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG.WolfTrans
{
    class TXT : WolfRPGBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseFile();
        }

        internal override bool Save()
        {
            thDataWork.SaveFileMode = true;
            return ParseFile();
        }

        int lineNum;
        protected override int ParseFileLine()
        {
            lineNum++;

            //skip if not patch files
            if (lineNum == 1 && ParseData.line != "> WOLF TRANS PATCH FILE VERSION 1.0")
            {
                return -1;
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

            //skip if begin string not found
            if (IsNotBeginString())
            {
                return 0;
            }
            //------------------------------------
            //add original
            var originalLines = new System.Collections.Generic.List<string>();
            while (!(ParseData.line = ParseData.reader.ReadLine()).StartsWith("> CONTEXT"))
            {
                originalLines.Add(ParseData.line);
                if (thDataWork.SaveFileMode)
                {
                    ParseData.ResultForWrite.AppendLine(ParseData.line);
                }
            }
            //------------------------------------
            //add context
            var contextLines = new System.Collections.Generic.List<string>
            {
                //add first context line (was set last to line in check of previous block)
                ParseData.line + (ParseData.line.EndsWith(" < UNTRANSLATED") ? string.Empty : " < UNTRANSLATED")
            };
            //add rest of context lines
            while ((ParseData.line = ParseData.reader.ReadLine()).StartsWith("> CONTEXT"))
            {
                contextLines.Add(ParseData.line + (ParseData.line.EndsWith(" < UNTRANSLATED") ? string.Empty : " < UNTRANSLATED"));
            }
            var context = string.Join(Properties.Settings.Default.NewLine, contextLines);
            //------------------------------------
            //add translation if exists
            var translationLines = new System.Collections.Generic.List<string>
            {
                //add last set line in check of previous context block
                ParseData.line
            };
            while ((ParseData.line = ParseData.reader.ReadLine()) != "> END STRING")
            {
                translationLines.Add(ParseData.line);
            }
            //------------------------------------
            var original = string.Join(Properties.Settings.Default.NewLine, originalLines);
            var translation = string.Join(Properties.Settings.Default.NewLine, translationLines);

            //add begin end string block data
            if (thDataWork.OpenFileMode)
            {
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
                var translated = thDataWork.SaveFileMode && thDataWork.TablesLinesDict.ContainsKey(original) && !string.IsNullOrEmpty(thDataWork.TablesLinesDict[original]) && (original != thDataWork.TablesLinesDict[original] || thDataWork.TablesLinesDict[original] != translation);
                ParseData.ResultForWrite.AppendLine(translated ? context.Replace(" < UNTRANSLATED", string.Empty) : context);
                if (translated && IsValidString(original))
                {
                    ParseData.Ret = true;
                    ParseData.ResultForWrite.AppendLine(thDataWork.TablesLinesDict[original]);
                }
                else
                {
                    ParseData.ResultForWrite.AppendLine(translation);
                }
                ParseData.ResultForWrite.AppendLine(ParseData.line);//add endstring line
            }

            return 0;
        }

        private bool IsNotBeginString()
        {
            return ParseData.line != "> BEGIN STRING";
        }
    }
}

using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    class KS : FormatBase
    {
        public KS(THDataWork thDataWork) : base(thDataWork)
        {
            scriptMark = new Script(thDataWork);
        }

        bool IsScript = false;
        Script scriptMark;
        protected override int ParseStringFileLine()
        {
            if (ParseData.IsComment)
            {
                if (ParseData.TrimmedLine.EndsWith("*/")) //comment section end
                {
                    ParseData.IsComment = false;
                }
            }
            else if (IsScript)
            {
                if (Regex.IsMatch(ParseData.line, scriptMark.EndsWith))
                {
                    IsScript = false;
                }
            }
            else if (Regex.IsMatch(ParseData.line, scriptMark.StartsWith))
            {
                IsScript = true;
            }
            else
            {
                if (ParseData.TrimmedLine.StartsWith("/*")) //comment section start
                {
                    if (!ParseData.TrimmedLine.EndsWith("*/"))
                    {
                        ParseData.IsComment = true;
                    }
                }
                else if (ParseData.TrimmedLine.StartsWith("//") || ParseData.TrimmedLine.StartsWith(";")) //comment
                {
                }
                else if (ParseData.line.StartsWith("[glink")
                    || ParseData.line.StartsWith("[ptext")
                    || ParseData.line.StartsWith("[mtext")
                    || (ParseData.line.Contains("text=") && Regex.IsMatch(ParseData.line,@"^\t*\[[a-zA-Z] "))
                    )
                {
                    var glinkStringData = Regex.Matches(ParseData.line, @"text\=\""([^\""\r\n\\]+(?:\\.[^\""\\]*)*)\""");//attributename="attributevalue"

                    if (glinkStringData.Count > 0)
                    {
                        for (int i = glinkStringData.Count - 1; i >= 0; i--)
                        {
                            var value = glinkStringData[i].Result("$1");
                            if (IsValidString(value))
                            {
                                if (thDataWork.OpenFileMode)
                                {
                                    AddRowData(value, "", true, false);
                                }
                                else
                                {
                                    if (thDataWork.TablesLinesDict.ContainsKey(value) && thDataWork.TablesLinesDict[value] != value)
                                    {
                                        ParseData.line = ParseData.line
                                            .Remove(glinkStringData[i].Index, glinkStringData[i].Length)
                                            .Insert(glinkStringData[i].Index, "text=\"" + thDataWork.TablesLinesDict[value] + "\"");
                                        ParseData.Ret = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(ParseData.line) /*&& ParseData.line.StartsWith("[link") || !ParseData.line.StartsWith("[")*/)
                {
                    var m = Regex.Match(ParseData.line, @"((\[[^\]]+\])*)([^\[\]]+(?:\\.[^\[\]]*)*)((\[[^\]]+\])*)");

                    if (m != null && m.Success && IsValidString(Cleaned(m.Result("$3"))))
                    {
                        var value = m.Result("$3");

                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(value, "", true, false);
                        }
                        else
                        {
                            string trans = null;
                            AddTranslation(ref trans, value);
                            if (trans != null)
                            {
                                ParseData.line = m.Result("$1") + trans + m.Result("$5");
                                ParseData.Ret = true;
                            }
                        }
                    }
                }
            }


            SaveModeAddLine("\n");//using \n as new line

            return 0;
        }

        private static string Cleaned(string line)
        {
            return Regex.Replace(line, @"\[[^\]]+\]", "");
        }
    }
}

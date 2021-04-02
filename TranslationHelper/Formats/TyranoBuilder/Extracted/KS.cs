using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    class KS : FormatBase
    {
        public KS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override int ParseStringFileLine()
        {
            if (ParseData.line.StartsWith(";"))
            {
                //comment
            }
            else if (ParseData.line.StartsWith("[glink") || ParseData.line.StartsWith("[ptext") || ParseData.line.StartsWith("[link"))
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
                                }
                            }
                        }
                    }
                }
            }
            else if (!ParseData.line.StartsWith("["))
            {
                if (IsValidString(Cleaned(ParseData.line)))
                {
                    if (thDataWork.OpenFileMode)
                    {
                        var m = Regex.Match(ParseData.line, @"((\[[^\]]+\])*)([^\[\]]+(?:\\.[^\[\]]*)*)((\[[^\]]+\])*)");
                        var value = m.Result("$3");
                        AddRowData(value, "", true, false);
                    }
                    else
                    {
                        AddTranslation();

                        if (!ParseData.line.EndsWith("[p]"))
                        {
                            ParseData.line += "[p]";
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

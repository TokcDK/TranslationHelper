using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    class Ks : FormatBase
    {
        public Ks()
        {
            _scriptMark = new Script();
        }

        internal override string Ext()
        {
            return ".ks";
        }

        bool _isScript = false;
        Script _scriptMark;
        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (ParseData.IsComment)
            {
                if (ParseData.TrimmedLine.EndsWith("*/")) //comment section end
                {
                    ParseData.IsComment = false;
                }
            }
            else if (_isScript)
            {
                if (Regex.IsMatch(ParseData.Line, _scriptMark.EndsWith))
                {
                    _isScript = false;
                }
            }
            else if (Regex.IsMatch(ParseData.Line, _scriptMark.StartsWith))
            {
                _isScript = true;
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
                else if (ParseData.Line.StartsWith("[glink")
                    || ParseData.Line.StartsWith("[ptext")
                    || ParseData.Line.StartsWith("[mtext")
                    || (ParseData.Line.Contains("text=") && Regex.IsMatch(ParseData.Line,@"^\t*\[[a-zA-Z] "))
                    )
                {
                    var glinkStringData = Regex.Matches(ParseData.Line, @"text\=\""([^\""\r\n\\]+(?:\\.[^\""\\]*)*)\""");//attributename="attributevalue"

                    if (glinkStringData.Count > 0)
                    {
                        for (int i = glinkStringData.Count - 1; i >= 0; i--)
                        {
                            var value = glinkStringData[i].Result("$1");
                            if (IsValidString(value))
                            {
                                if (ProjectData.OpenFileMode)
                                {
                                    AddRowData(value, ParseData.Line, true, false);
                                }
                                else
                                {
                                    if (ProjectData.TablesLinesDict.ContainsKey(value) && ProjectData.TablesLinesDict[value] != value)
                                    {
                                        ParseData.Line = ParseData.Line
                                            .Remove(glinkStringData[i].Index, glinkStringData[i].Length)
                                            .Insert(glinkStringData[i].Index, "text=\"" + ProjectData.TablesLinesDict[value] + "\"");
                                        ParseData.Ret = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(ParseData.Line) && !ParseData.Line.StartsWith("@") && !ParseData.Line.StartsWith("*") /*&& ParseData.line.StartsWith("[link") || !ParseData.line.StartsWith("[")*/)
                {
                    var mc = Regex.Matches(ParseData.Line, @"(\[*([^\[\]\r\n]+(?:\\.[^\[\]]*)*)\]*)");

                    if (mc.Count > 0)
                    {
                        if(ProjectData.SaveFileMode)
                        {
                            ParseData.Line = "";
                        }

                        for (int i = 0; i < mc.Count; i++)
                        {
                            var value = mc[i].Value;

                            if (ProjectData.OpenFileMode)
                            {
                                if (value.StartsWith("[") && value.EndsWith("]"))
                                {
                                    continue;
                                }

                                AddRowData(value, ParseData.Line, true, true);
                            }
                            else
                            {
                                if (!(value.StartsWith("[") && value.EndsWith("]")) && IsValidString(value))
                                {
                                    AddTranslation(ref value, value);
                                    ParseData.Ret = true;
                                }
                                ParseData.Line += value;
                            }
                        }
                    }

                    //var m = Regex.Match(ParseData.line, @"((\[[^\]]+\])*)([^\[\]]+(?:\\.[^\[\]]*)*)((\[[^\]]+\])*)");

                    //if (m != null && m.Success && IsValidString(Cleaned(m.Result("$3"))))
                    //{
                    //    var value = m.Result("$3");

                    //    if (ProjectData.OpenFileMode)
                    //    {
                    //        AddRowData(value, "", true, false);
                    //    }
                    //    else
                    //    {
                    //        string trans = null;
                    //        AddTranslation(ref trans, value);
                    //        if (trans != null)
                    //        {
                    //            ParseData.line = m.Result("$1") + trans + m.Result("$5");
                    //            ParseData.Ret = true;
                    //        }
                    //    }
                    //}
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

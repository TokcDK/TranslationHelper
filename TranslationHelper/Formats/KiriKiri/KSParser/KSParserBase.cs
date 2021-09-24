using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    abstract class KSParserBase : StringFileFormatBase
    {
        public KSParserBase()
        {
            scriptMark = new Script();
            Tag = new TAG1();
            textOnMessage = new List<string>();
        }

        internal override string Ext()
        {
            return ".ks";
        }

        bool IsScript = false;
        bool TextOn = false;
        readonly Script scriptMark;
        readonly TAG1 Tag;
        readonly List<string> textOnMessage;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.IsComment)
            {
                if (ParseData.TrimmedLine.Contains("*/")) //comment section end
                {
                    ParseData.IsComment = false;
                }
            }
            else if (IsScript)
            {
                if (Regex.IsMatch(ParseData.Line, scriptMark.EndsWith))
                {
                    IsScript = false;
                }
            }
            //else if (TextOn)
            //{
            //    if (ParseData.TrimmedLine == "[textoff]")
            //    {
            //        var joined = string.Join(Environment.NewLine, textOnMessage);
            //        if (IsValidString(joined))
            //        {
            //            if (ProjectData.OpenFileMode)
            //            {
            //                AddRowData(joined, "TextOn block message", CheckInput: false);
            //            }
            //            else
            //            {
            //                SetTranslation(ref joined);
            //            }
            //        }

            //        if (ProjectData.SaveFileMode)
            //        {
            //            ParseData.Line = joined + ParseData.Line;
            //        }

            //        TextOn = false;
            //    }
            //    else
            //    {
            //        textOnMessage.Add(ParseData.Line);
            //        return KeywordActionAfter.Continue;
            //    }
            //}
            else if (Regex.IsMatch(ParseData.Line, scriptMark.StartsWith))
            {
                IsScript = true;
            }
            //else if (ParseData.TrimmedLine == "[texton]")
            //{
            //    textOnMessage.Clear();
            //    TextOn = true;
            //}
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
                    || (ParseData.Line.Contains("text=") && Regex.IsMatch(ParseData.Line, @"^\t*\[[a-zA-Z] "))
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
                                    AddRowData(value, ParseData.Line, CheckInput: false);
                                }
                                else
                                {
                                    var trans = value;
                                    if (SetTranslation(ref trans))
                                    {
                                        ParseData.Line = ParseData.Line
                                            .Remove(glinkStringData[i].Index, glinkStringData[i].Length)
                                            .Insert(glinkStringData[i].Index, "text=\"" + trans + "\"");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Regex.IsMatch(ParseData.Line, Tag.StartsWith))
                {
                    if (Tag.Include() != null)
                    {
                        foreach (var attributePattern in Tag.Include())
                        {
                            if (attributePattern == null)
                            {
                                continue;
                            }

                            var attributeMatches = Regex.Matches(ParseData.Line, attributePattern.StartsWith);

                            for (int i = attributeMatches.Count - 1; i >= 0; i--)
                            {
                                var attributeMatch = attributeMatches[i];

                                var attributeValue = attributeMatch.Groups[1].Value;

                                if (!IsValidString(attributeValue))
                                {
                                    continue;
                                }

                                if (ProjectData.OpenFileMode)
                                {
                                    AddRowData(attributeValue, "", CheckInput: false);
                                }
                                else
                                {
                                    var trans = attributeValue;
                                    if (SetTranslation(ref trans))
                                    {
                                        int index = attributeMatch.Groups[1].Index;
                                        ParseData.Line = ParseData.Line
                                            .Remove(index, attributeMatch.Groups[1].Length)
                                            .Insert(index, trans);
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
                        if (ProjectData.SaveFileMode)
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

                                AddRowData(value, ParseData.Line, CheckInput: true);
                            }
                            else
                            {
                                if (!(value.StartsWith("[") && value.EndsWith("]")) && IsValidString(value))
                                {
                                    SetTranslation(ref value);
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


            SaveModeAddLine(newline: Environment.NewLine);//using \n as new line

            return KeywordActionAfter.Continue;
        }

        private static string Cleaned(string line)
        {
            return Regex.Replace(line, @"\[[^\]]+\]", "");
        }

        internal virtual Encoding FileEncoding()
        {
            return new UTF8Encoding(false);
        }
    }
}

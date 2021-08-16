using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JsQuotedStringsBase : JsBase
    {
        protected JsQuotedStringsBase()
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (!IsEmptyOrComment())
            {
                foreach (var regexQuote in new[] { "'", @"\""" })
                {
                    var mc = Regex.Matches(ParseData.Line, /*@"[\""']([^\""'\r\n]+)[\""']"*/
                          @"" + regexQuote + @"([^" + regexQuote + @"\r\n\\]+(?:\\.[^" + regexQuote + @"\\]*)*)" + regexQuote //all between " or ' include \" or \' : x: "abc" or "abc\"" or 'abc' or 'abc\''
                        );
                    for (int m = mc.Count - 1; m >= 0; m--)
                    {
                        var result = mc[m].Result("$1");

                        if (ProjectData.OpenFileMode)
                        {
                            AddRowData(result, ParseData.Line, true);
                        }
                        else if (IsValidString(result) && TablesLinesDict.ContainsKey(result))
                        {
                            var quote = regexQuote.Replace(@"\", "");
                            ParseData.Line = ParseData.Line.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, quote + TablesLinesDict[result] + quote);
                            ParseData.Ret = true;
                        }
                    }
                }
            }

            SaveModeAddLine("\n");

            return 0;
        }

        private bool IsEmptyOrComment()
        {
            if (!ParseData.IsComment && ParseData.Line.Contains("/*"))
            {
                ParseData.IsComment = true;
            }
            if (ParseData.IsComment && ParseData.Line.Contains("*/"))
            {
                ParseData.IsComment = false;
            }

            return ParseData.IsComment || ParseData.TrimmedLine.Length == 0 || ParseData.TrimmedLine[0] == ';' || ParseData.TrimmedLine.StartsWith("//");
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }
    }
}

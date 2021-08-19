using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSQuotedStringsBase : JSBase
    {
        protected JSQuotedStringsBase()
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
                    // remove comment // area and get matches
                    var mc = Regex.Matches(ParseData.line.Split(new[]{ "//" },System.StringSplitOptions.None)[0], /*@"[\""']([^\""'\r\n]+)[\""']"*/
                          @"" + regexQuote + @"([^" + regexQuote + @"\r\n\\]*(?:\\.[^" + regexQuote + @"\\]*)*)" + regexQuote //all between " or ' include \" or \' : x: "abc" or "abc\"" or 'abc' or 'abc\''
                        );
                    int internalCommentIndex = ParseData.line.IndexOf("//");
                    bool hasInternalComment = internalCommentIndex != 1;
                    for (int m = mc.Count - 1; m >= 0; m--)
                    {
                        var result = mc[m].Result("$1");

                        if (ProjectData.OpenFileMode)
                        {
                            AddRowData(result, ParseData.line, true);
                        }
                        else if (IsValidString(result) && TablesLinesDict.ContainsKey(result))
                        {
                            var quote = regexQuote.Replace(@"\", "");
                            ParseData.line = ParseData.line.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, quote + TablesLinesDict[result] + quote);
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
            if (!ParseData.IsComment && ParseData.line.Contains("/*"))
            {
                ParseData.IsComment = true;
            }
            if (ParseData.IsComment && ParseData.line.Contains("*/"))
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

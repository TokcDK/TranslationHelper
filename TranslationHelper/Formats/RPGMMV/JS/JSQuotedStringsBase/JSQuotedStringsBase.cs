using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSQuotedStringsBase : JSBase
    {
        protected JSQuotedStringsBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        protected override int ParseStringFileLine()
        {
            if (!IsEmptyOrComment())
            {
                foreach (var quote in new[] { "'", @"\""" })
                {
                    var mc = Regex.Matches(ParseData.line, /*@"[\""']([^\""'\r\n]+)[\""']"*/
                          @"" + quote + @"([^" + quote + @"\r\n\\]+(?:\\.[^" + quote + @"\\]*)*)" + quote //all between " or ' include \" or \' : x: "abc" or "abc\"" or 'abc' or 'abc\''
                        );
                    for (int m = mc.Count - 1; m >= 0; m--)
                    {
                        var result = mc[m].Result("$1");

                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(result, ParseData.line, true);
                        }
                        else if (IsValidString(result) && TablesLinesDict.ContainsKey(result))
                        {
                            ParseData.line = ParseData.line.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, quote + TablesLinesDict[result] + quote);
                            ParseData.Ret = true;
                        }
                    }
                }
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

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

            return ParseData.IsComment || ParseData.TrimmedLine.Length == 0 || (ParseData.TrimmedLine.Length > 0 && ParseData.TrimmedLine[0] == ';') || ParseData.TrimmedLine.StartsWith("//");
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }
    }
}

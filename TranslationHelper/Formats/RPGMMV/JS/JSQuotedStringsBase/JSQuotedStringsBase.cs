using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSQuotedStringsBase : JSBase
    {
        protected JSQuotedStringsBase()
        {
        }

        /// <summary>
        /// pattern which will be placed before quoted pattern
        /// </summary>
        protected virtual string PreQuoteRegexPattern => "";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!IsEmptyOrComment())
            {
                foreach (var regexQuote in new[] { "'", @"\""" })
                {
                    // remove comment // area and get matches
                    var lineNoComment = ParseData.Line.Split(new[] { "//" }, System.StringSplitOptions.None)[0];
                    var mc = Regex.Matches(lineNoComment, /*@"[\""']([^\""'\r\n]+)[\""']"*/
                          PreQuoteRegexPattern + @"" + regexQuote + @"([^" + regexQuote + @"\r\n\\]*(?:\\.[^" + regexQuote + @"\\]*)*)" + regexQuote //all between " or ' include \" or \' : x: "abc" or "abc\"" or 'abc' or 'abc\''
                        );
                    for (int m = mc.Count - 1; m >= 0; m--) // negative because lenght of string will be changing
                    {
                        var match = mc[m];

                        var result = match.Result("$1");

                        if (!IsValidString(result))
                        {
                            continue;
                        }

                        if (OpenFileMode)
                        {
                            AddRowData(result, ParseData.Line, CheckInput: false);
                        }
                        else
                        {
                            string translation = result;
                            if (!SetTranslation(ref translation))
                            {
                                continue;
                            }

                            //1111 abc "aaa"
                            int index = match.Value.IndexOf(result); // get internal index of result
                            ParseData.Line = ParseData.Line
                                .Remove(index = match.Index + index/*remove only original string*/, result.Length)
                                .Insert(index, translation); // paste translation on place of original
                            ParseData.Ret = true;
                        }
                    }
                }
            }

            SaveModeAddLine(newline: System.Environment.NewLine);

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
    }
}

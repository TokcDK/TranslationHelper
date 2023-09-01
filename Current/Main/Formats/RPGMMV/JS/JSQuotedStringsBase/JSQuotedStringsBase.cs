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

        readonly string[] _quotesList = new[] { "'", "`", @"\""" };
        readonly string[] _commentMark = new[] { "//" };
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!IsEmptyOrComment())
            {
                foreach (var regexQuote in _quotesList)
                {
                    // remove comment // area and get matches
                    var lineNoComment = ParseData.Line.Split(_commentMark, System.StringSplitOptions.None)[0];
                    var mc = Regex.Matches(lineNoComment, AppMethods.GetRegexQuotesCapturePattern(regexQuote));
                    for (int m = mc.Count - 1; m >= 0; m--) // negative because lenght of string will be changing
                    {
                        var match = mc[m];

                        var result = match.Groups[1].Value;

                        if (!IsValidString(result)) continue;

                        if (OpenFileMode)
                        {
                            AddRowData(result, ParseData.Line, isCheckInput: false);
                        }
                        else
                        {
                            string translation = result;
                            if (!SetTranslation(ref translation)) continue;

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

        readonly string _commentZoneStartMark = "/*";
        readonly string _commentZoneEndMark = "*/";
        private bool IsEmptyOrComment()
        {
            if (!ParseData.IsComment && ParseData.Line.Contains(_commentZoneStartMark))
            {
                ParseData.IsComment = true;
            }
            if (ParseData.IsComment && ParseData.Line.Contains(_commentZoneEndMark))
            {
                ParseData.IsComment = false;
            }

            return ParseData.IsComment || ParseData.TrimmedLine.Length == 0 || ParseData.TrimmedLine[0] == ';' || ParseData.TrimmedLine.StartsWith("//");
        }
    }
}

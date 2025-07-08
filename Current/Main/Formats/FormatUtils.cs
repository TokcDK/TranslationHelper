using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationHelper.Formats
{
    static class FormatUtils
    {
        public static IEnumerable<string> GetStringstByMatchStartingfromEnd(string input, string pattern)
        {
            var mc = Regex.Matches(input, pattern);
            if (mc.Count == 0) yield break;

            for (int i = mc.Count - 1; i >= 0; i--)
            {
                var match = mc[i];
                var matchGroup1Captures = match.Groups[1].Captures;
                for (int n = matchGroup1Captures.Count - 1; n >= 0; n--)
                {
                    yield return matchGroup1Captures[n].Value;
                }
            }
        }
    }
}

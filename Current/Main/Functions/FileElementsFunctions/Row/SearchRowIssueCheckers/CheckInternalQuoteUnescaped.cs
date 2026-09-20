using System;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class CheckInternalQuoteUnescaped : SearchIssueCheckerBase
    {
        public override string Description => "Check internal quote unescaped";

        /// <summary>
        /// Quote characters the check looks for. Shared because the previous per call array allocation
        /// happened once per row.
        /// </summary>
        private static readonly char[] Quotes = { '"', '\'' };

        public override bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            //A null translation (a DBNull cell) has no internal quote to report.
            return IsHaveInternalUnescapedQuote((data.Translation ?? string.Empty).Trim());
        }

        private bool IsHaveInternalUnescapedQuote(string t)
        {
            var tLen = t.Length;
            if (tLen < 3) return false; // at least 3 chars   

            var tLastIndex = tLen - 1;
            foreach (var quote in Quotes)
            {
                if (t[0] != quote || t[tLastIndex] != quote) continue;

                var i = t.IndexOf(quote, 1, tLastIndex - 1);
                if (i == -1) return false;
                if (tLen == 3) return true; // when len 3 central char will be same quote

                if (t[i - 1] != '\\') return true; // quote is not escaped
            }

            return false;
        }
    }
}

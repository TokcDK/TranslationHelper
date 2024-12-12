using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Data
{
    public static class AppMethods
    {
        /// <summary>
        /// regex pattern to capture quoted text like "text" or even "txt1\"text2\"text3"
        /// </summary>
        /// <param name="regexQuote"></param>
        /// <returns></returns>
        public static string GetRegexQuotesCapturePattern(string regexQuote = @"\""")
        {
            return $@"(?<!\\){regexQuote}((?:[^{regexQuote}\\]|\\.)*?){regexQuote}";
            //return $@"{regexQuote}([^{regexQuote}\r\n\\]*(?:\\.[^{regexQuote}\r\n\\]*)*){regexQuote}"; // old pattern
        }
    }
}

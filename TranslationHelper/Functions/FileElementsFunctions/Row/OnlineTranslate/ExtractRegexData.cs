using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    partial class OnlineTranslateNew
    {
        public class ExtractRegexData
        {
            public string ExtractedString { get; }
            public string Pattern { get; }
            public string Replacer { get; }
            public List<int> Groups = new List<int>();

            public ExtractRegexData(string extractedString, string pattern, string replacer)
            {
                ExtractedString = extractedString;
                Pattern = pattern;
                Replacer = replacer;
            }
        }
    }
}

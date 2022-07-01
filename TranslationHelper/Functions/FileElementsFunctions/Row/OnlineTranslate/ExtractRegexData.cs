using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class ExtractRegexGroupInfo
    {
        public string Pattern { get; }
        public string Replacer { get; }
        public List<int> Groups = new List<int>();

        public ExtractRegexGroupInfo(string pattern, string replacer)
        {
            Pattern = pattern;
            Replacer = replacer;
        }
    }
}

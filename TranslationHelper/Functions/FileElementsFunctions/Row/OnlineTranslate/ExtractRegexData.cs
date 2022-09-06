using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class ExtractRegexInfo
    {
        public string Pattern { get; set; }
        public string Replacer { get; set; }
        public List<int> Groups = new List<int>();

        public Dictionary<string, ExtractRegexValueInfo> ValueData = new Dictionary<string, ExtractRegexValueInfo>();
    }
    public class ExtractRegexValueInfo
    {
        public List<int> Groups = new List<int>();
        public string Translation;

        public IEnumerable<object> GroupIndexes { get; internal set; }
    }
}

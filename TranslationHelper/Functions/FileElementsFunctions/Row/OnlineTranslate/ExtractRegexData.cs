using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class ExtractRegexInfo
    {
        public string Pattern { get; set; }
        public string Replacer { get; set; }
        public Dictionary<string, ExtractRegexValueInfo> ValueDataList = new Dictionary<string, ExtractRegexValueInfo>();
    }
    public class ExtractRegexValueInfo
    {
        public List<int> GroupIndexes = new List<int>();
        public string Translation;
    }
}

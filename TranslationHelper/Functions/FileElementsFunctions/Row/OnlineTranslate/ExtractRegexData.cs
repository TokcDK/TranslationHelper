using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class ExtractRegexInfo
    {
        public string Pattern { get; set; }
        public string Replacer { get; set; }
        public Match Match { get; set; }
        public Dictionary<string, ExtractRegexValueInfo> ValueDataList = new Dictionary<string, ExtractRegexValueInfo>();
    }
    public class ExtractRegexValueInfo
    {
        public List<Group> Group = new List<Group>();
        public string Translation;
    }
}

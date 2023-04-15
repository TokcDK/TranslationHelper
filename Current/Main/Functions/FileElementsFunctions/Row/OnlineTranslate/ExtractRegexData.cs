using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public enum TranslationRegexExtractType
    {
        /// <summary>
        /// something like '$1'
        /// </summary>
        ReplaceOne,
        /// <summary>
        /// something like: '$1,$2,$3'
        /// </summary>
        ReplaceList,
        /// <summary>
        /// standart regex replacer when more of one $ group
        /// </summary>
        Replacer,

    }

    public class ExtractRegexInfo
    {
        public ExtractRegexInfo(string inputString)
        {
            InputString = inputString;
        }

        /// <summary>
        /// Input string from which values was extracted
        /// </summary>
        public string InputString { get; }
        /// <summary>
        /// Regex text values to capture with
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// Text to replace captured values by the Pattern
        /// </summary>
        public string Replacer { get; set; }
        /// <summary>
        /// Match value captured by the Pattern
        /// </summary>
        public Match Match { get; set; }
        /// <summary>
        /// Captured values data
        /// </summary>
        public List<ExtractRegexValueInfo> ValueDataList = new List<ExtractRegexValueInfo>();

    }
    public class ExtractRegexValueInfo
    {
        public ExtractRegexValueInfo(string original)
        {
            Original = original;
        }

        /// <summary>
        /// Captured group match value original
        /// </summary>
        public string Original { get; }

        /// <summary>
        /// Captured group match value translation
        /// </summary>
        public string Translation;

        /// <summary>
        /// Captured groups for the original text value
        /// </summary>
        public List<Group> MatchGroups = new List<Group>();
    }
}

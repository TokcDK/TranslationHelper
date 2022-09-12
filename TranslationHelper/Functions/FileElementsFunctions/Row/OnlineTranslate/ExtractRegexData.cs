using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class ExtractRegexInfo
    {
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
        public Dictionary<string, ExtractRegexValueInfo> ValueDataList = new Dictionary<string, ExtractRegexValueInfo>();
    }
    public class ExtractRegexValueInfo
    {
        /// <summary>
        /// Captured groups for the original text value
        /// </summary>
        public List<Group> MatchGroups = new List<Group>();
        
        private string translation;

        /// <summary>
        /// Captured group match values translation
        /// </summary>
        public string Translation
        {
            get
            {
                if (translation == null) return MatchGroups[0].Value; // return original captured text value

                return translation;
            }

            set => translation = value;
        }
    }
}

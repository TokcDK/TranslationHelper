using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

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
            ExtractMulty();
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
        /// <summary>
        /// extract captured groups from string
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        void ExtractMulty()
        {
            var log = new FunctionsLogs();
            try
            {
                foreach (var PatternReplacementPair in AppData.TranslationRegexRules)
                {
                    // check if any regex is match
                    Regex regex = null;
                    Match match = null;
                    try
                    {
                        regex = new Regex(PatternReplacementPair.Key);
                        match = regex.Match(InputString);

                        if (!match.Success) continue;
                    }
                    catch (System.ArgumentException ex)
                    {
                        log.LogToFile("ExtractMulty: Invalid regex:" + PatternReplacementPair.Key + "\r\nError:\r\n" + ex);
                        AppData.Main.ProgressInfo(true, "Invalid regex found. See " + THSettings.ApplicationLogName);
                        continue;
                    }

                    // add regex pattern and replacer
                    Match = match;
                    Pattern = PatternReplacementPair.Key;
                    Replacer = PatternReplacementPair.Value;

                    var added = new Dictionary<string, ExtractRegexValueInfo>();

                    // add matched groups values
                    foreach (Group g in match.Groups)
                    {
                        if (!Replacer.Contains("$" + g.Name)) continue; // skip if group is missing in replacer value

                        ExtractRegexValueInfo valueData = null;
                        if (added.ContainsKey(g.Value)) valueData = added[g.Value];
                        else added.Add(g.Value, valueData = new ExtractRegexValueInfo(g.Value));

                        if (valueData.MatchGroups.Contains(g)) continue;

                        valueData.MatchGroups.Add(g);
                    }

                    // set va;ues to extractregexinfo return
                    ValueDataList = new List<ExtractRegexValueInfo>(added.Values);

                    break; // regex found skip other
                }
            }
            catch (InvalidOperationException) // in case of collection was changed exception when rules was changed in time of iteration
            {
                if (++retryCount > 10) throw;

                // retry extraction
                ExtractMulty();
            }
        }
        int retryCount = 0;
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

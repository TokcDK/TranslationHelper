using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Projects
{
    /// <summary>
    /// A class that hides specified variables in strings using regex patterns and placeholders,
    /// and restores them to their original values. Implements IDisposable to release resources.
    /// </summary>
    public class ProjectHideRestoreVarsInstance : IDisposable
    {
        /// <summary>
        /// Dictionary of patterns where keys are variable identifiers and values are regex patterns to match them.
        /// </summary>
        private readonly Dictionary<string, string> _hidePatterns;

        /// <summary>
        /// Queue of match collections (stored as lists of matched values) from hide operations,
        /// used for restoring original values in sequence.
        /// </summary>
        private Queue<List<string>> _matchCollectionsQueue;

        /// <summary>
        /// Initializes a new instance of the class with the specified hide patterns.
        /// </summary>
        /// <param name="hidePatterns">Dictionary mapping variable identifiers to their regex patterns.</param>
        public ProjectHideRestoreVarsInstance(Dictionary<string, string> hidePatterns)
        {
            _hidePatterns = hidePatterns;
        }

        /// <summary>
        /// Hides variables in the input string by replacing matches of the hide patterns with placeholders
        /// (e.g., "{VAR000}", "{VAR001}"). Stores matched values for later restoration.
        /// </summary>
        /// <param name="str">The input string to hide variables in.</param>
        /// <returns>The string with variables replaced by placeholders.</returns>
        internal string HideVARSBase(string str)
        {
            // Return unchanged if patterns are invalid or empty
            if (_hidePatterns == null || _hidePatterns.Count == 0)
                return str;

            // Optional optimization: check if any pattern key is present in the string
            // This avoids regex processing if no relevant substrings are found
            bool keyFound = _hidePatterns.Keys.Any(key => str.Contains(key));
            if (!keyFound)
                return str;

            // Combine all regex patterns into a single pattern for matching
            var pattern = "(" + string.Join(")|(", _hidePatterns.Values) + ")";
            var matches = new List<string>();
            int counter = 0;

            // Replace each match with a unique placeholder and store the matched value
            str = Regex.Replace(str, pattern, match =>
            {
                matches.Add(match.Value);
                return "{VAR" + (counter++).ToString("000") + "}";
            });

            // Only queue matches if any were found
            if (matches.Count > 0)
            {
                if (_matchCollectionsQueue == null)
                {
                    _matchCollectionsQueue = new Queue<List<string>>();
                }
                _matchCollectionsQueue.Enqueue(matches);
            }

            return str;
        }

        /// <summary>
        /// Restores placeholders (e.g., "{VAR000}") in the input string to their original values
        /// using the most recent hide operation's matches.
        /// </summary>
        /// <param name="str">The input string with placeholders to restore.</param>
        /// <returns>The string with placeholders replaced by original values.</returns>
        internal string RestoreVARS(string str)
        {
            // Return unchanged if queue is empty or string contains no placeholders
            if (_matchCollectionsQueue == null || _matchCollectionsQueue.Count == 0 ||
                !Regex.IsMatch(str, @"\{ ?VAR ?([0-9]{3}) ?\}", RegexOptions.IgnoreCase))
            {
                return str;
            }

            // Dequeue the next set of matches
            var matches = _matchCollectionsQueue.Dequeue();

            // Fix any malformed placeholders (e.g., "{ VAR 001 }" -> "{VAR001}")
            str = Regex.Replace(str, @"\{ ?VAR ?([0-9]{3}) ?\}", "{VAR$1}", RegexOptions.IgnoreCase);

            // Replace each placeholder with its corresponding original value
            str = Regex.Replace(str, @"\{VAR(\d{3})\}", match =>
            {
                int index = int.Parse(match.Groups[1].Value);
                // Safety check to avoid index out of range, though should not occur in normal use
                return index < matches.Count ? matches[index] : match.Value;
            });

            return str;
        }

        /// <summary>
        /// Clears all stored match collections.
        /// </summary>
        internal void Clear()
        {
            _matchCollectionsQueue?.Clear();
        }

        /// <summary>
        /// Releases resources by clearing match collections.
        /// </summary>
        public void Dispose()
        {
            Clear();
            _matchCollectionsQueue = null;
            GC.SuppressFinalize(this);
        }
    }
}

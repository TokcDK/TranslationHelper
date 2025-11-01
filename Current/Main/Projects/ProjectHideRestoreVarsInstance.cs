using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        /// Cached compiled regex for hiding variables - improves performance for repeated operations
        /// </summary>
        private Regex _hideRegex;

        /// <summary>
        /// Cached compiled regex for normalizing placeholders - improves performance for repeated operations
        /// </summary>
        private static readonly Regex _normalizePlaceholderRegex = new Regex(@"\{ ?VAR ?([0-9]{3}) ?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Cached compiled regex for detecting placeholders - improves performance for repeated operations
        /// </summary>
        private static readonly Regex _detectPlaceholderRegex = new Regex(@"\{VAR\d{3}\}", RegexOptions.Compiled);

        /// <summary>
        /// Cached compiled regex for restoring placeholders - improves performance for repeated operations
        /// </summary>
        private static readonly Regex _restorePlaceholderRegex = new Regex(@"\{VAR(\d{3})\}", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the class with the specified hide patterns.
        /// </summary>
        /// <param name="hidePatterns">Dictionary mapping variable identifiers to their regex patterns.</param>
        public ProjectHideRestoreVarsInstance(Dictionary<string, string> hidePatterns)
        {
            _hidePatterns = hidePatterns;

            if (hidePatterns != null && hidePatterns.Count > 0)
            {
                var pattern = "(" + string.Join(")|(", hidePatterns.Values) + ")";
                _hideRegex = new Regex(pattern, RegexOptions.Compiled);
            }
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
            if (_hideRegex == null)
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

            // Replace each match with a unique placeholder and store the matched value using pre-compiled regex
            str = _hideRegex.Replace(str, match =>
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
                !_detectPlaceholderRegex.IsMatch(str))
            {
                return str;
            }

            // Dequeue the next set of matches
            var matches = _matchCollectionsQueue.Dequeue();

            // Fix any malformed placeholders (e.g., "{ VAR 001 }" -> "{VAR001}") using cached regex
            str = _normalizePlaceholderRegex.Replace(str, "{VAR$1}");

            // Replace each placeholder with its corresponding original value using cached regex
            str = _restorePlaceholderRegex.Replace(str, match =>
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

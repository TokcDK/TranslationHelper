using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranslationHelper.Forms.Search
{
    public interface ISearchCondition
    {
        string FindWhat { get; }
        string SearchColumn { get; }
        bool CaseSensitive { get; }
        bool UseRegex { get; }
        IReadOnlyList<IReplaceTask> ReplaceTasks { get; }
    }

    public interface IReplaceTask
    {
        string ReplaceWhat { get; }
        string ReplaceWith { get; }
    }

    public static class SearchHelpers
    {
        public static bool Matches(string input, string pattern, bool caseSensitive, bool useRegex)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
                return false;

            if (useRegex)
            {
                var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                return Regex.IsMatch(input, pattern, options);
            }
            else
            {
                var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return input.IndexOf(pattern, comparison) != -1;
            }
        }

        public static string ApplyReplaces(string input, IReadOnlyList<IReplaceTask> tasks, bool caseSensitive, bool useRegex)
        {
            if (tasks == null || !tasks.Any() || string.IsNullOrEmpty(input))
                return input;

            var result = input;
            foreach (var task in tasks.Where(t => !string.IsNullOrEmpty(t.ReplaceWhat)))
            {
                if (useRegex)
                {
                    var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    result = Regex.Replace(result, task.ReplaceWhat, task.ReplaceWith ?? string.Empty, options);
                }
                else
                {
                    if (caseSensitive)
                    {
                        result = result.Replace(task.ReplaceWhat, task.ReplaceWith ?? string.Empty);
                    }
                    else
                    {
                        result = ReplaceIgnoreCase(result, task.ReplaceWhat, task.ReplaceWith ?? string.Empty);
                    }
                }
            }
            return result;
        }

        private static string ReplaceIgnoreCase(string input, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue))
                return input;

            var result = input;
            int index;
            while ((index = result.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                result = result.Remove(index, oldValue.Length).Insert(index, newValue);
            }
            return result;
        }
    }

    internal class SearchHelper
    {
        internal List<SearchCondition> SearchConditions { get; set; } = new List<SearchCondition>();

        internal SearchHelper(string[] columns)
        {
            SearchConditions.Add(new SearchCondition(columns));
        }
    }

    internal class SearchCondition
    {
        public SearchCondition(string[] columns)
        {
            Options = new SearchOptions(columns);
            Replacers.Add(new Replacer());
        }

        internal string FindWhat { get; set; } = "";

        internal SearchOptions Options { get; set; }

        internal List<Replacer> Replacers { get; set; } = new List<Replacer>();
    }

    internal class SearchOptions
    {
        public SearchOptions(string[] columns)
        {
            SearchColumns = columns;
            if(SearchColumns.Length > 0)
            {
                SearchColumn = SearchColumns[0];
            }
        }

        internal string[] SearchColumns { get; }
        internal string SearchColumn { get; set; }

        internal bool IsCaseSensitive { get; set; } = false;

        internal bool IsRegex { get; set; } = false;
    }

    internal class Replacer
    {
        internal string ReplaceWhat { get; set; }
        internal string ReplaceWith { get; set; }
    }
}

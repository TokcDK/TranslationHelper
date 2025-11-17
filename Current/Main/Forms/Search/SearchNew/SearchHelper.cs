using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Forms.Search
{
    public interface ISearchConditionSearchResult
    {
        (List<string> searchQueries, List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchQueries(bool isReplace);
    }
    public interface ISearchCondition
    {
        string FindWhat { get; }
        string SearchColumn { get; }
        bool CaseSensitive { get; }
        bool UseRegex { get; }
        List<IReplaceTask> ReplaceTasks { get; }
    }

    public interface IReplaceTaskSearchResult
    {
        (List<string> searchReplacers, List<string> SearchReplacePatterns) GetSearchReplacers();
    }
    public interface IReplaceTask
    {
        string ReplaceWhat { get; }
        string ReplaceWith { get; }
    }
    public interface ISearchOption
    {
        int Priority { get; }
    }
    public interface ISearchOptionMatch
    {
        bool IsMatch(string inputString, string pattern);
    }
    public interface ISearchOptionReplace
    {
        string Replace(string inputString, string replaceWhat, string replaceWith);
    }
    public interface ISearchOptionUsingControl
    {
        Control Control { get; }
    }
    public class SearchOptionSearchColumn : ISearchOption, ISearchOptionUsingControl
    {
        readonly ComboBox _control = new ComboBox();

        public Control Control => _control;

        public int Priority => 999;
    }

    public class SearchOptionCaseSensitive : ISearchOption, ISearchOptionMatch, ISearchOptionReplace, ISearchOptionUsingControl
    {
        readonly CheckBox _control = new CheckBox() { Checked = false };

        public Control Control => _control;

        public int Priority => 10;

        protected bool CaseSensitive => _control.Checked;

        public bool IsMatch(string inputString, string pattern)
        {
            var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return inputString.IndexOf(pattern, comparison) != -1;
        }

        public string Replace(string inputString, string replaceWhat, string replaceWith)
        {
            var options = CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            return inputString.Replace(replaceWhat, replaceWith, options);
        }
    }
    public class SearchOptionRegex : SearchOptionCaseSensitive
    {
        public new bool IsMatch(string inputString, string pattern)
        {
            var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            return Regex.IsMatch(inputString, pattern, options);
        }

        public new string Replace(string inputString, string replaceWhat, string replaceWith)
        {
            var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            return Regex.Replace(inputString, replaceWhat, replaceWith, options);
        }
        public new int Priority => 100;
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

        public static string ApplyReplaces(string input, List<IReplaceTask> tasks, bool caseSensitive, bool useRegex)
        {
            var result = input;
            foreach (var task in tasks)
            {
                if(string.IsNullOrEmpty(task.ReplaceWhat)) continue;

                if (useRegex)
                {
                    var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    result = Regex.Replace(result, task.ReplaceWhat, task.ReplaceWith, options);
                }
                else
                {
                    var options = caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
                    result = result.Replace(task.ReplaceWhat, task.ReplaceWith, options);
                }
            }
            return result;
        }
    }
}

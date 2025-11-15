using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Forms.Search
{
    public interface ISearchCondition
    {
        string FindWhat { get; }
        string SearchColumn { get; }
        bool CaseSensitive { get; }
        bool UseRegex { get; }
        List<IReplaceTask> ReplaceTasks { get; }
    }

    public interface IReplaceTask
    {
        string ReplaceWhat { get; }
        string ReplaceWith { get; }
    }
    public interface ISearchOption
    {
        bool IsMatch(string inputString, string pattern);
        string Replace(string inputString, string replaceWhat, string replaceWith);
    }
    public interface ISearchOptionUsingControl
    {
        Control Control { get; }
    }

    public class SearchOptionCaseSensitive : ISearchOption, ISearchOptionUsingControl
    {
        readonly CheckBox _control = new CheckBox() { Checked = false };

        public Control Control => _control;

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

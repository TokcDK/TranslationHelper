using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Forms.Search.SearchNew.OptionsNew
{
    // search options will autoadd their controls into options field
    // main search options will search and replace strings in specific area
    // regex implemented from case sensitive and using it but overriding match and replace actions
    // maybe search option like selected column better
    // to include in main search options like case sensitive and using it regex search
    // maybe main search will include options of search area

    public enum SearchOptionType
    {
        SearchTarget = 0, // determine the search target like search column or info field
        SearchType = 0, // determine the main search type like case sensitive search or regex search
    }

    public interface ISearchOption
    {
        int Priority { get; }
        SearchOptionType SearchType { get; }
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
        public SearchOptionSearchColumn(string[] columns)
        {
            _control.Items.AddRange(columns);
        }

        readonly ComboBox _control = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };

        public Control Control => _control;

        public int Priority => 200;

        public SearchOptionType SearchType => SearchOptionType.SearchTarget;
    }
    public class SearchOptionInfoTarget : ISearchOption, ISearchOptionUsingControl
    {
        readonly CheckBox _control = new CheckBox();

        public Control Control => _control;

        public int Priority => 200;

        public SearchOptionType SearchType => SearchOptionType.SearchTarget;
    }

    public class SearchOptionCaseSensitive : ISearchOption, ISearchOptionMatch, ISearchOptionReplace, ISearchOptionUsingControl
    {
        readonly CheckBox _control = new CheckBox() { Checked = false };

        public Control Control => _control;

        public int Priority => 10;

        public SearchOptionType SearchType => SearchOptionType.SearchType;

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
}

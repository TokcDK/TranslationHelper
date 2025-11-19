using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
using TranslationHelper.Forms.Search.Data;
using TranslationHelper.Forms.Search.SearchNew.Data;
using TranslationHelper.Projects;
using static TranslationHelper.Forms.Search.SearchNew.SearchForm;

namespace TranslationHelper.Forms.Search.SearchNew.OptionsNew
{
    // search options will autoadd their controls into options field
    // main search options will search and replace strings in specific area
    // regex implemented from case sensitive and using it but overriding match and replace actions
    // maybe search option like selected column better
    // to include in main search options like case sensitive and using it regex search
    // maybe main search will include options of search area

    public interface ISearchOption
    {
        bool IsEnabled { get; }
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
    public interface ISearchTarget 
    {
        IEnumerable<(string stringToCheck, DataRow row)> EnumerateStrings(ProjectBase project);
    }
    public class SearchOptionSearchColumn : ISearchOption, ISearchOptionUsingControl, ISearchTarget
    {
        public SearchOptionSearchColumn(string[] columns)
        {
            _control.Items.AddRange(columns);
        }

        readonly ComboBox _control = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };

        public Control Control => _control;

        public bool IsEnabled => _control.Items.Count > 0;

        public IEnumerable<(string stringToCheck, DataRow row)> EnumerateStrings(ProjectBase project)
        {
            if(string.IsNullOrWhiteSpace(_control.Text)) yield break;

            foreach (DataTable table in project.FilesContent.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    // return value of cell for specific column
                    yield return (row.Field<string>(_control.Text), row);
                }
            }
        }
    }
    public class SearchOptionInfoTarget : ISearchOption, ISearchOptionUsingControl, ISearchTarget
    {
        readonly CheckBox _control = new CheckBox();

        public Control Control => _control;

        public bool IsEnabled => _control.Enabled;

        public IEnumerable<(string stringToCheck, DataRow row)> EnumerateStrings(ProjectBase project)
        {
            if (!_control.Checked) yield break; // Use Checked for enablement

            var tables = project.FilesContent.Tables;
            var tablesInfo = project.FilesContentInfo.Tables;
            if (tables.Count != tablesInfo.Count) yield break;

            int infoColIdx = 0;
            for (int t = 0; t < tables.Count; t++)
            {
                var rows = tables[t].Rows;
                var rowsInfo = tablesInfo[t].Rows;
                if (rows.Count != rowsInfo.Count) continue;

                for (int r = 0; r < rows.Count; r++)
                {
                    yield return (rowsInfo[r].Field<string>(infoColIdx), rows[r]);
                }
            }
        }
    }

    public class SearchOptionCaseSensitive : ISearchOption, ISearchOptionMatch, ISearchOptionReplace, ISearchOptionUsingControl
    {
        readonly CheckBox _control = new CheckBox() { Checked = false };

        public Control Control => _control;

        public bool IsEnabled => _control.Enabled;

        protected bool CaseSensitive => _control.Checked;

        public virtual bool IsMatch(string inputString, string pattern)
        {
            var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return inputString.IndexOf(pattern, comparison) != -1;
        }

        public virtual string Replace(string inputString, string replaceWhat, string replaceWith)
        {
            var options = CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            return inputString.Replace(replaceWhat, replaceWith, options);
        }
    }
    public class SearchOptionRegex : SearchOptionCaseSensitive
    {
        public override bool IsMatch(string inputString, string pattern)
        {
            var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            return Regex.IsMatch(inputString, pattern, options);
        }

        public override string Replace(string inputString, string replaceWhat, string replaceWith)
        {
            var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            return Regex.Replace(inputString, replaceWhat, replaceWith, options);
        }
    }

    public class SearchLoader
    {
        private readonly List<ISearchOption> searchOptions;
        private readonly List<ISearchTarget> searchTargets;
        private readonly List<ISearchOptionMatch> searchers;
        private readonly List<ISearchOptionReplace> replacers;
        private readonly List<ISearchOptionUsingControl> searchOptionsUsingControl;

        private readonly ProjectBase _project;

        public SearchLoader(ProjectBase project)
        {
            _project = project;

            // init search options
            var searchOptionInfoTarget = new SearchOptionInfoTarget();
            var searchOptionSearchColumn = new SearchOptionSearchColumn(_project.FilesContent.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray());
            var searchOptionRegex = new SearchOptionRegex();
            var searchOptionCaseSensitive = new SearchOptionCaseSensitive();

            searchOptions = new List<ISearchOption>()
            {
                searchOptionInfoTarget,
                searchOptionSearchColumn,
                searchOptionRegex,
                searchOptionCaseSensitive,
            };

            searchOptionsUsingControl = searchOptions.Where(o => o is ISearchOptionUsingControl).Select(o => o as ISearchOptionUsingControl).ToList();

            searchTargets = searchOptions.Where(o => o is ISearchTarget).Select(o => o as ISearchTarget).ToList();
            searchers = searchOptions.Where(o => o is ISearchOptionMatch).Select(o => o as ISearchOptionMatch).ToList();
            replacers = searchOptions.Where(o => o is ISearchOptionReplace).Select(o => o as ISearchOptionReplace).ToList();
        }

        public void CreateControls(Control parentControl)
        {
            foreach (var option in searchOptionsUsingControl)
            {
                if (option == null) continue;
                if (option.Control == null) continue;

                parentControl.Controls.Add(option.Control);
            }
        }

        public SearchResultsData PerformSearch(ISearchCondition[] conditions, bool isReplace = false)
        {
            var results = new SearchResultsData();

            if (conditions.Length == 0) return results;
            if (_project.FilesContent.Tables.Count == 0) return results;
            if (_project.FilesContent.Tables[0].Rows.Count == 0) return results;
            if (_project.FilesContent.Tables[0].Columns.Count == 0) return results;

            var searchArea = searchTargets.FirstOrDefault(t => t is ISearchOption o && o.IsEnabled);

            var searcher = searchers.FirstOrDefault(s => s is ISearchOption o && o.IsEnabled);
            var replacer = replacers.FirstOrDefault(s => s is ISearchOption o && o.IsEnabled);

            if (searchArea == default 
                || searcher == default 
                || (isReplace && replacer == default)) 
                return results;

            results.SearchConditions.AddRange(conditions);

            foreach (var (stringToCheck, row) in searchArea.EnumerateStrings(_project))
            {
                if (conditions.Any(c => !searcher.IsMatch(stringToCheck, c.FindWhat))) continue;

                if (!isReplace || SearchHelpers.TryReplaceAny(conditions, row, _project, replacer))
                {
                    results.FoundRows.Add(new FoundRowData(row));
                }
            }

            return results;
        }

        public SearchResultsData Search(ISearchCondition[] conditions)
        {
            return PerformSearch(conditions);
        }

        public SearchResultsData Replace(ISearchCondition[] conditions)
        {
            return PerformSearch(conditions, true);
        }
    }
}

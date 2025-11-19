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
using TranslationHelper.Forms.Search.Data;
using TranslationHelper.Forms.Search.SearchNew.Data;
using TranslationHelper.Forms.Search.SearchNew.OptionsNew;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;
using static TranslationHelper.Forms.Search.SearchNew.SearchForm;

namespace TranslationHelper.Forms.Search
{
    public interface ISearchConditionSearchResult
    {
        (List<string> searchQueries, List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchQueries(bool isReplace);

        void LoadSearchQueries(bool isReplace);
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
        (List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchReplacers();
        void LoadSearchReplacers();
    }
    public interface IReplaceTask
    {
        string ReplaceWhat { get; }
        string ReplaceWith { get; }
    }

    internal static class SearchHelpers
    {
        internal static bool Matches(string input, string pattern, bool caseSensitive, bool useRegex)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
                return false;

            if (useRegex)
            {
                var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                bool ret = Regex.IsMatch(input, pattern, options);
                return ret;
            }
            else
            {
                var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return input.IndexOf(pattern, comparison) != -1;
            }
        }

        internal static bool TryReplaceAny(IEnumerable<ISearchCondition> conditions, DataRow row, ProjectBase project)
        {
            string currentValue = row.Field<string>(project.TranslationColumnIndex);
            string newValue = currentValue;

            foreach (var cond in conditions)
            {
                var matchingValue = row.Field<string>(cond.SearchColumn);
                if (string.IsNullOrEmpty(matchingValue))
                {
                    continue;
                }

                // replace all values in the target string
                newValue = SearchHelpers.ApplyReplaces(newValue, cond.ReplaceTasks, cond.CaseSensitive, cond.UseRegex);
            }

            if (!string.Equals(currentValue, newValue))
            {
                // set result value to row when it was changed by any replacement
                row.SetField(project.TranslationColumnIndex, newValue);

                return true;
            }

            return false;
        }

        internal static bool TryReplaceAny(IEnumerable<ISearchCondition> conditions, DataRow row, ProjectBase project, ISearchOptionReplace replacer)
        {
            string currentValue = row.Field<string>(project.TranslationColumnIndex);
            string newValue = currentValue;

            foreach (var cond in conditions)
            {
                var matchingValue = row.Field<string>(cond.SearchColumn);
                if (string.IsNullOrEmpty(matchingValue))
                {
                    continue;
                }

                // replace all values in the target string
                foreach (var task in cond.ReplaceTasks)
                {
                    newValue = replacer.Replace(newValue, task.ReplaceWhat, task.ReplaceWith);
                }
            }

            if (!string.Equals(currentValue, newValue))
            {
                // set result value to row when it was changed by any replacement
                row.SetField(project.TranslationColumnIndex, newValue);

                return true;
            }

            return false;
        }

        internal static string ApplyReplaces(string input, List<IReplaceTask> tasks, bool caseSensitive, bool useRegex)
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

        internal static void ShowSelectedCellInMainTable(ProjectBase project, List<FoundRowData> _foundRowsList, int foundRowIndex, int selectedCellColumnIndex)
        {
            var _workFileDgv = AppData.Main.THFileElementsDataGridView;
            try
            {
                var foundRowData = _foundRowsList[foundRowIndex];
                var (_selectedTableIndex, _selectedRowIndex) = (foundRowData.TableIndex, foundRowData.RowIndex);

                AppData.Main.THFileElementsDataGridView.CleanFilter();

                var tableDefaultView = project.FilesContent.Tables[_selectedTableIndex].DefaultView;
                tableDefaultView.RowFilter = string.Empty;
                tableDefaultView.Sort = string.Empty;
                _workFileDgv.Refresh();

                FunctionsTable.ShowSelectedRow(AppData.Main.THFileElementsDataGridView, _selectedTableIndex, _selectedRowIndex, selectedCellColumnIndex);
                //if (_workFileDgv.CurrentCell != null)
                //{
                //    await Task.Run(() => SelectTextInTextBox(_workFileDgv.CurrentCell.Value.ToString())).ConfigureAwait(false);
                //}
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
        }

        internal static void SaveSearchResults(SearchResultsData searchResults, bool isReplace = false)
        {
            var (searchQueries, searchReplacers, searchReplacePatterns) = GetSearchQueries(searchResults, isReplace);

            foreach (var (list, sectionName) in new[]
            {
                (searchQueries, THSettings.SearchQueriesSectionName ),
                (searchReplacers, THSettings.SearchReplacersSectionName),
                (searchReplacePatterns, THSettings.SearchReplacePatternsSectionName)
            })
            {
                SearchSharedHelpers.SaveSearchQueries(list, sectionName);
            }

            UpdateSearchQueries(searchResults, isReplace);
        }

        internal static void UpdateSearchQueries(SearchResultsData searchResults, bool isReplace)
        {
            foreach (var item in searchResults.SearchConditions.Cast<ISearchConditionSearchResult>())
            {
                item.LoadSearchQueries(isReplace);
            }
        }

        internal static (List<string> searchQueries, List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchQueries(SearchResultsData searchResults, bool isReplace)
        {
            var searchQueries = new List<string>();
            var searchReplacers = new List<string>();
            var searchReplacePatterns = new List<string>();

            foreach (var item in searchResults.SearchConditions.Cast<ISearchConditionSearchResult>())
            {
                var results = item.GetSearchQueries(isReplace);

                searchQueries.AddRange(results.searchQueries
                    .Where(s => !searchQueries.Contains(s)));

                if (!isReplace) continue;

                searchReplacers.AddRange(results.searchReplacers
                    .Where(s => !searchReplacers.Contains(s)));
                searchReplacePatterns.AddRange(results.searchReplacePatterns
                    .Where(s => !searchReplacePatterns.Contains(s)));
            }

            return (searchQueries, searchReplacers, searchReplacePatterns);
        }
    }
}

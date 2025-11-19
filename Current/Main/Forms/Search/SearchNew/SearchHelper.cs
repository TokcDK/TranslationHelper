using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Forms.Search.Data;
using TranslationHelper.Forms.Search.SearchNew.Data;
using TranslationHelper.Forms.Search.SearchNew.OptionsNew;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;
using static Manina.Windows.Forms.TabControl;

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
        ISearchOptionTarget Target { get; }
        ISearchOptionMatch Searcher { get; }
        ISearchOptionReplace Replacer { get; }
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
        internal static string TextConditionTabIndexed { get; } = T._("Condition {0}");
        internal static string TextReplacerTabIndexed { get; } = T._("Replace {0}");
        internal static string TextSearchResultsReplacedPrefix { get; } = T._("Replaced");
        internal static string TextSearchResultsFoundPrefix { get; } = T._("Found");
        internal static string TextSearchResultsMatchingStringsMessage { get; } = T._("{0} {1} matching strings.");

        internal static Tab AddSearchConditionTab(Manina.Windows.Forms.TabControl tabControl, DataTableCollection tables)
        {
            var tabPage = new Tab
            {
                Text = string.Format(TextConditionTabIndexed, tabControl.Tabs.Count + 1),
            };
            var columns = tables.Count > 0 ? tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray() : Array.Empty<string>();
            var conditionUC = new SearchConditionUserControl(columns);
            tabPage.Controls.Add(conditionUC);
            conditionUC.Dock = DockStyle.Fill;
            tabControl.Tabs.Add(tabPage);

            tabControl.SelectedTab = tabPage;

            return tabPage;
        }

        public static void RemoveSearchConditionTab(Manina.Windows.Forms.TabControl tabControl, Tab tab, DataTableCollection tables)
        {
            tabControl.Tabs.Remove(tab);
            if (tabControl.Tabs.Count == 0)
            {
                SearchHelpers.AddSearchConditionTab(tabControl, tables); // always one default search condition tab by default
            }
            else
            {
                SearchHelpers.RenumerateTabNames(tabControl.Tabs, TextConditionTabIndexed);
            }
        }

        internal static void RenumerateTabNames(TabCollection tabs, string formattedText)
        {
            int i = 1;
            foreach (var t in tabs)
            {
                t.Text = string.Format(formattedText, i++);
            }
        }

        internal static IEnumerable<ISearchCondition> GetSearchConditions(TabCollection tabs)
        {
            foreach (var tab in tabs)
            {
                if (!(tab.Controls[0] is ISearchCondition c)) continue;

                yield return c;
            }
        }
        internal static ISearchCondition[] GetValidConditions(IEnumerable<ISearchCondition> conditions, bool isReplace)
        {
            return conditions.Where(c => c != null && !string.IsNullOrEmpty(c.FindWhat)).ToArray();
        }

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

        internal static bool TryReplaceAny(IEnumerable<ISearchCondition> conditions, DataRow row, ProjectBase project, bool useConditionReplacer = false)
        {
            string currentValue = row.Field<string>(project.TranslationColumnIndex);
            string newValue = currentValue;

            foreach (var cond in conditions)
            {
                if (useConditionReplacer)
                {
                    foreach (var task in cond.ReplaceTasks)
                    {
                        newValue = cond.Replacer.Replace(newValue, task.ReplaceWhat, task.ReplaceWith);
                    }
                }
                else
                {
                    newValue = SearchHelpers.ApplyReplaces(newValue, cond.ReplaceTasks, cond.CaseSensitive, cond.UseRegex);
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

        internal static void BindSearchResults(SearchResultsData searchResults, Control foundRowsControl, ProjectBase project)
        {
            var foundRowsDatagridView = new DataGridView
            {
                DataSource = searchResults.FoundRows,
                Dock = DockStyle.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders,
            };
            foundRowsDatagridView.CellClick += (sender, e) =>
            {
                SearchHelpers.ShowSelectedCellInMainTable(project, searchResults.FoundRows, e.RowIndex, e.ColumnIndex);
            };
            foundRowsControl.Controls.Add(foundRowsDatagridView);
        }

        internal static string ApplyReplaces(string input, List<IReplaceTask> tasks, bool caseSensitive, bool useRegex)
        {
            var result = input;
            foreach (var task in tasks)
            {
                if (string.IsNullOrEmpty(task.ReplaceWhat)) continue;

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
                (searchReplacePatterns, THSettings.SearchReplacePatternsSectionName),
                (searchReplacers, THSettings.SearchReplacersSectionName),
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

                string lastQuery = results.searchQueries.Count > 0 ? results.searchQueries[0] : "";
                if (lastQuery != "")
                {
                    int lastQueryIndex = searchQueries.IndexOf(lastQuery);
                    if (lastQueryIndex > 0)
                    {
                        // insert last query of the current saving queries list as first query
                        searchQueries.RemoveAt(lastQueryIndex);
                        searchQueries.Insert(0, lastQuery);
                    }
                    else if (lastQueryIndex == -1)
                    {
                        searchQueries.Insert(0, lastQuery);
                    }
                }

                SearchHelpers.AddMissing(searchQueries, results.searchQueries);

                if (!isReplace) continue;

                SearchHelpers.AddMissing(searchReplacePatterns, results.searchReplacePatterns);
                SearchHelpers.AddMissing(searchReplacers, results.searchReplacers);
            }

            return (searchQueries, searchReplacers, searchReplacePatterns);
        }

        internal static void AddMissing<T>(List<T> listWhereAdd, IEnumerable<T> listFromAdd)
        {
            listWhereAdd.AddRange(listFromAdd
                .Where(s => !listWhereAdd.Contains(s)));
        }
    }
}

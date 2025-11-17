using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;
using Tab = Manina.Windows.Forms.Tab;

namespace TranslationHelper.Forms.Search.SearchNew
{
    public partial class SearchForm : Form
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ProjectBase _project;
        private readonly DataSet _dataSet;
        private Manina.Windows.Forms.TabControl _searchConditionsTabControl;
        private Tab _plusTab;
        private const string PLUS_TAB_TEXT = "+";

        public SearchForm(ProjectBase project)
        {
            _project = project;
            _dataSet = project.FilesContent ?? throw new ArgumentNullException(nameof(project));

            InitializeComponent();

            InitSearchConditionsTabControl();
        }
        public class FoundRowData
        {
            private static readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
            private static readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;

            public FoundRowData(DataRow row)
            {
                Row = row;
                TableIndex = AppData.CurrentProject.FilesContent.Tables.IndexOf(row.Table);
                RowIndex = row.Table.Rows.IndexOf(row);
            }

            [Browsable(false)]
            public DataRow Row { get; }
            public string Original => Row.Field<string>(_originalColumnIndex);
            public string Translation
            {
                get => Row.Field<string>(_translationColumnIndex);
                set => Row.SetField(_translationColumnIndex, value);
            }

            [Browsable(false)]
            public int TableIndex { get; }
            [Browsable(false)]
            public int RowIndex { get; }
        }

        public class SearchResultsData
        {
            public List<FoundRowData> FoundRows { get; set; } = new List<FoundRowData>();

            public List<ISearchCondition> searchConditions { get; set; } = new List<ISearchCondition>();
        }
        private void SearchAllButton_Click(object sender, EventArgs e)
        {
            GetSearchResults();
        }

        private void GetSearchResults(bool isReplace = false)
        {
            var searchResults = PerformSearch();

            var actionName = isReplace ?
                "Replaced" :
                "Found";
            SearchResultInfoLabel.Text = $"{actionName} {searchResults.FoundRows.Count} matching strings.";

            if (searchResults.FoundRows.Count == 0)
            {
                return;
            }

            SaveSearchResults(searchResults, isReplace);

            FoundRowsPanel.Controls.Clear();
            var foundRowsDatagridView = new DataGridView
            {
                DataSource = searchResults,
                Dock = DockStyle.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders,
            };
            foundRowsDatagridView.CellClick += (sender, e) =>
            {
                ShowSelectedCellInMainTable(searchResults.FoundRows, e.RowIndex, e.ColumnIndex);
            };
            foundRowsDatagridView.DataSource = searchResults.FoundRows;
            FoundRowsPanel.Controls.Add(foundRowsDatagridView);
        }

        private void SaveSearchResults(SearchResultsData searchResults, bool isReplace = false)
        {
            var (searchQueries, searchReplacers, searchReplacePatterns) = GetSearchQueries(searchResults, isReplace);

            foreach (var (list, sectionName) in new [] 
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

        private static void UpdateSearchQueries(SearchResultsData searchResults, bool isReplace)
        {
            foreach (var item in searchResults.searchConditions.Cast<ISearchConditionSearchResult>())
            {
                item.LoadSearchQueries(isReplace);
            }
        }

        private (List<string> searchQueries, List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchQueries(SearchResultsData searchResults, bool isReplace)
        {
            var searchQueries = new List<string>();
            var searchReplacers = new List<string>();
            var searchReplacePatterns = new List<string>();

            foreach (var item in searchResults.searchConditions.Cast<ISearchConditionSearchResult>())
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

        private void ShowSelectedCellInMainTable(List<FoundRowData> _foundRowsList, int foundRowIndex, int columnIndex)
        {
            var _workFileDgv = AppData.Main.THFileElementsDataGridView;
            try
            {
                var foundRowData = _foundRowsList[foundRowIndex];
                var (_selectedTableIndex, _selectedRowIndex) = (foundRowData.TableIndex, foundRowData.RowIndex);

                AppData.Main.THFileElementsDataGridView.CleanFilter();

                var tableDefaultView = _project.FilesContent.Tables[_selectedTableIndex].DefaultView;
                tableDefaultView.RowFilter = string.Empty;
                tableDefaultView.Sort = string.Empty;
                _workFileDgv.Refresh();

                FunctionsTable.ShowSelectedRow(_selectedTableIndex, columnIndex, _selectedRowIndex, AppData.Main.THFileElementsDataGridView);
                //if (_workFileDgv.CurrentCell != null)
                //{
                //    await Task.Run(() => SelectTextInTextBox(_workFileDgv.CurrentCell.Value.ToString())).ConfigureAwait(false);
                //}
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
        }

        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            GetSearchResults(true);
        }
        public void InitSearchConditionsTabControl()
        {
            _searchConditionsTabControl = new Manina.Windows.Forms.TabControl
            {
                Dock = DockStyle.Fill,
                ShowCloseTabButtons = true,
                AllowDrop = true
            };
            _searchConditionsTabControl.CloseTabButtonClick += (o, e) =>
            {
                RemoveSearchConditionTab(e.Tab);
            };
            SearchConditionTabsPanel.Controls.Add(_searchConditionsTabControl);

            AddNewSearchConditionTabButton.Click += (o, e) => AddSearchConditionTab();

            AddSearchConditionTab(); // always one default search condition tab by default

        }

        public Tab AddSearchConditionTab()
        {
            var tabPage = new Tab
            {
                Text = $"Condition {(_searchConditionsTabControl.Tabs.Count + 1)}"
            };
            var columns = _dataSet.Tables.Count > 0 ? _dataSet.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray() : Array.Empty<string>();
            var conditionUC = new SearchConditionUserControl(columns);
            tabPage.Controls.Add(conditionUC);
            conditionUC.Dock = DockStyle.Fill;
            _searchConditionsTabControl.Tabs.Add(tabPage);

            _searchConditionsTabControl.SelectedTab = tabPage;

            return tabPage;
        }

        public void RemoveSearchConditionTab(Tab tab)
        {
            _searchConditionsTabControl.Tabs.Remove(tab);
            if (_searchConditionsTabControl.Tabs.Count == 0)
            {
                AddSearchConditionTab(); // always one default search condition tab by default
            }
        }

        private SearchResultsData PerformSearch(bool isReplace = false)
        {
            var searchResults = new SearchResultsData();
            var conditions = GetSearchConditions();
            var nonEmptyConditions = conditions.Where(c => c != null && !string.IsNullOrEmpty(c.FindWhat)
            && (!isReplace || isReplace && c.ReplaceTasks.Any(t => !string.IsNullOrEmpty(t.ReplaceWhat))))
                .ToArray();
            if (nonEmptyConditions.Length == 0) return searchResults;

            searchResults.searchConditions.AddRange(nonEmptyConditions);

            foreach (DataTable table in _dataSet.Tables)
            {
                // Skip tables missing required columns
                if (!nonEmptyConditions.All(c => !string.IsNullOrEmpty(c.SearchColumn) && table.Columns.Contains(c.SearchColumn)))
                    continue;

                var matchingRows = table.AsEnumerable()
                    .Where(row => nonEmptyConditions.All(cond =>
                        SearchHelpers.Matches(row.Field<string>(cond.SearchColumn), cond.FindWhat, cond.CaseSensitive, cond.UseRegex)));

                foreach (var row in matchingRows)
                {
                    foreach (var cond in nonEmptyConditions)
                    {
                        var matchingValue = row.Field<string>(cond.SearchColumn);
                        if (string.IsNullOrEmpty(matchingValue))
                        {
                            continue;
                        }

                        if (isReplace)
                        {
                            // replace all values in the target string
                            var columnName = cond.SearchColumn;
                            var currentValue = row.Field<string>(columnName);
                            var newValue = SearchHelpers.ApplyReplaces(currentValue, cond.ReplaceTasks, cond.CaseSensitive, cond.UseRegex);

                            if (!string.Equals(currentValue, newValue))
                            {
                                // set result value to row when it was changed by any replacement
                                row.SetField(columnName, newValue);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        searchResults.FoundRows.Add(new FoundRowData(row));
                    }
                }
            }

            return searchResults;
        }

        private IEnumerable<ISearchCondition> GetSearchConditions()
        {
            foreach (var tab in _searchConditionsTabControl.Tabs)
            {
                yield return tab.Controls[0] as ISearchCondition;
            }
        }

        private void SearchRootTableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

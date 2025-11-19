using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Forms.Search.Data;
using TranslationHelper.Forms.Search.SearchNew.Data;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;
using static Manina.Windows.Forms.TabControl;
using Tab = Manina.Windows.Forms.Tab;

namespace TranslationHelper.Forms.Search.SearchNew
{
    public partial class SearchForm : Form
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ProjectBase _project;
        private readonly DataSet _dataSet;
        private Manina.Windows.Forms.TabControl _searchConditionsTabControl;

        public SearchForm(ProjectBase project)
        {
            _project = project;
            _dataSet = project.FilesContent ?? throw new ArgumentNullException(nameof(project));

            InitializeComponent();

            InitSearchConditionsTabControl();
        }
        private void SearchAllButton_Click(object sender, EventArgs e)
        {
            GetSearchResults();
        }

        private void GetSearchResults(bool isReplace = false)
        {
            var searchResults = PerformSearch(isReplace);

            FoundRowsPanel.Controls.Clear();

            var actionName = isReplace ?
                T._("Replaced") :
                T._("Found");
            SearchResultInfoLabel.Text = string.Format(T._("{0} {1} matching strings."), actionName, searchResults.FoundRows.Count);
            
            if (searchResults.FoundRows.Count == 0)
            {
                return;
            }

            SearchHelpers.SaveSearchResults(searchResults, isReplace);

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
                SearchHelpers.ShowSelectedCellInMainTable(_project, searchResults.FoundRows, e.RowIndex, e.ColumnIndex);
            };
            foundRowsDatagridView.DataSource = searchResults.FoundRows;
            FoundRowsPanel.Controls.Add(foundRowsDatagridView);
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
                e.Cancel = true; // cancel included removal function to remove manually with renumerate
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
                Text = string.Format(T._("Condition {0}", _searchConditionsTabControl.Tabs.Count + 1)),
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
            else
            {
                RenumerateTabNames(_searchConditionsTabControl.Tabs);
            }
        }

        private static void RenumerateTabNames(TabCollection tabs)
        {
            int i = 1;
            foreach (var t in tabs)
            {
                t.Text = string.Format(T._("Condition {0}", i++));
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

            searchResults.SearchConditions.AddRange(nonEmptyConditions);

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
                    bool haveAnyReplaced = false;
                    if (isReplace)
                    {
                        string currentValue = row.Field<string>(_project.TranslationColumnIndex);
                        string newValue = currentValue;

                        foreach (var cond in nonEmptyConditions)
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
                            haveAnyReplaced = true;

                            // set result value to row when it was changed by any replacement
                            row.SetField(_project.TranslationColumnIndex, newValue);
                        }
                    }

                    if(!isReplace || (isReplace && haveAnyReplaced))
                    {
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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Forms.Search
{
    public partial class SearchForm : Form
    {
        private readonly ProjectBase _project;
        private readonly DataSet _dataSet;
        private TabControl _searchConditionsTabControl;

        public SearchForm(ProjectBase project)
        {
            _project = project;
            _dataSet = project.FilesContent ?? throw new ArgumentNullException(nameof(project)); 

            InitializeComponent();
            
            AddSearchConditionTab();
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

        private void SearchAllButton_Click(object sender, EventArgs e)
        {
            GetSearchResults();
        }

        private void GetSearchResults(bool isReplace = false)
        {
            var foundRows = PerformSearch();

            var actionName = isReplace ?
                "Replaced" :
                "Found";
            MessageBox.Show($"{actionName} {foundRows.Count} matching strings.", "Results");

            if(foundRows.Count == 0) 
            { 
                return; 
            }

            var foundRowsDatagridView = new DataGridView
            {
                DataSource = foundRows,
                Dock = DockStyle.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders,
            };
            foundRowsDatagridView.CellClick += (sender, e) =>
            {
                ShowSelectedCellInMainTable(foundRows, e.RowIndex, e.ColumnIndex);
            };
            FoundRowsPanel.Controls.Add(foundRowsDatagridView);
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

        public void AddSearchConditionTab()
        {
            _searchConditionsTabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            SearchConditionsPanel.Controls.Add(_searchConditionsTabControl);

            var tabPage = new TabPage($"Condition {(_searchConditionsTabControl.TabCount + 1)}");
            var columns = _dataSet.Tables.Count > 0 ? _dataSet.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray() : Array.Empty<string>();
            var conditionUC = new SearchConditionUserControl(columns);
            tabPage.Controls.Add(conditionUC);
            conditionUC.Dock = DockStyle.Fill;
            _searchConditionsTabControl.TabPages.Add(tabPage);
        }

        public void RemoveSearchConditionTab(TabPage tabPage)
        {
            if (_searchConditionsTabControl.TabCount > 1)
            {
                _searchConditionsTabControl.TabPages.Remove(tabPage);
            }
        }

        private List<FoundRowData> PerformSearch(bool isReplace = false)
        {
            var conditions = GetSearchConditions();
            var nonEmptyConditions = conditions.Where(c => !string.IsNullOrEmpty(c.FindWhat) 
            && (!isReplace || isReplace && c.ReplaceTasks.Any(t => !string.IsNullOrEmpty(t.ReplaceWhat))))
                .ToArray();
            if (nonEmptyConditions.Length == 0) return new List<FoundRowData>();

            var foundRows = new List<FoundRowData>();

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
                        if (!string.IsNullOrEmpty(matchingValue))
                        {
                            if (isReplace)
                            {
                                var columnName = cond.SearchColumn;
                                var currentValue = row.Field<string>(columnName);
                                var newValue = SearchHelpers.ApplyReplaces(currentValue, cond.ReplaceTasks, cond.CaseSensitive, cond.UseRegex);

                                if(!string.Equals(currentValue, newValue))
                                {
                                    row.SetField(columnName, newValue);
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            foundRows.Add(new FoundRowData(row));
                        }
                    }
                }
            }

            return foundRows;
        }

        private IEnumerable<ISearchCondition> GetSearchConditions()
        {
            foreach (TabPage tabPage in _searchConditionsTabControl.TabPages)
            {
                yield return tabPage.Controls[0] as ISearchCondition;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Projects;

namespace TranslationHelper.Forms.Search
{
    public partial class SearchForm : Form
    {
        private readonly DataSet _dataSet;

        public SearchForm(ProjectBase project)
        {
            _dataSet = project.FilesContent ?? throw new ArgumentNullException(nameof(project)); 

            InitializeComponent();
            
            AddSearchConditionTab();
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
                Dock = DockStyle.Fill
            };
            FoundRowsPanel.Controls.Add(foundRowsDatagridView);
        }

        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            GetSearchResults(true);
        }

        public void AddSearchConditionTab()
        {
            var tabPage = new TabPage($"Condition {(SearchConditionsTabControl.TabCount + 1)}");
            var columns = _dataSet.Tables.Count > 0 ? _dataSet.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray() : Array.Empty<string>();
            var conditionUC = new SearchConditionUserControl(columns);
            tabPage.Controls.Add(conditionUC);
            conditionUC.Dock = DockStyle.Fill;
            SearchConditionsTabControl.TabPages.Add(tabPage);
        }

        public void RemoveSearchConditionTab(TabPage tabPage)
        {
            if (SearchConditionsTabControl.TabCount > 1)
            {
                SearchConditionsTabControl.TabPages.Remove(tabPage);
            }
        }

        private List<DataRow> PerformSearch(bool isReplace = false)
        {
            var conditions = GetSearchConditions();
            var nonEmptyConditions = conditions.Where(c => !string.IsNullOrEmpty(c.FindWhat) 
            && (!isReplace || isReplace && c.ReplaceTasks.Any(t => !string.IsNullOrEmpty(t.ReplaceWhat))))
                .ToArray();
            if (nonEmptyConditions.Length == 0) return new List<DataRow>();

            var foundRows = new List<DataRow>();

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

                            foundRows.Add(row);
                        }
                    }
                }
            }

            return foundRows;
        }

        private IEnumerable<ISearchCondition> GetSearchConditions()
        {
            foreach (TabPage tabPage in SearchConditionsTabControl.TabPages)
            {
                yield return tabPage.Controls[0] as ISearchCondition;
            }
        }
    }
}

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
            var foundStrings = PerformSearch();

            MessageBox.Show($"Found {foundStrings.Count} matching strings.", "Search Results");
        }

        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            var foundStrings = PerformSearch(true);

            MessageBox.Show("Replace operation completed.", "Replace Results");

            MessageBox.Show($"Found {foundStrings.Count} matching strings.", "Search Results");
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

        private List<string> PerformSearch(bool isReplace = false)
        {
            var conditions = GetSearchConditions();
            var nonEmptyConditions = conditions.Where(c => !string.IsNullOrEmpty(c.FindWhat) 
            && (!isReplace || isReplace && c.ReplaceTasks.Any(t => !string.IsNullOrEmpty(t.ReplaceWhat))))
                .ToArray();
            if (nonEmptyConditions.Length == 0) return new List<string>();

            var foundStrings = new HashSet<string>();

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
                        var matchingValue = row[cond.SearchColumn]?.ToString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(matchingValue))
                        {
                            foundStrings.Add(matchingValue);

                            if (!isReplace) continue;

                            var columnName = cond.SearchColumn;
                            var currentValue = row.Field<string>(columnName);
                            row[columnName] = SearchHelpers.ApplyReplaces(currentValue, cond.ReplaceTasks, cond.CaseSensitive, cond.UseRegex);
                        }
                    }
                }
            }

            return foundStrings.OrderBy(s => s).ToList();
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

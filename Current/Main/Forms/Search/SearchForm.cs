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
            _dataSet = project.FilesContent ?? throw new ArgumentNullException("input project");
            
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
            PerformReplace();

            MessageBox.Show("Replace operation completed.", "Replace Results");
        }

        public void AddSearchConditionTab()
        {
            var tabPage = new TabPage($"Condition {(SearchConditionsTabControl.TabCount + 1)}");
            var conditionUC = new SearchConditionUserControl();
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

        private List<string> PerformSearch()
        {
            var conditions = GetSearchConditions();
            var nonEmptyConditions = conditions.Where(c => !string.IsNullOrEmpty(c.FindWhat)).ToList();
            if (nonEmptyConditions.Count == 0)
                return new List<string>();

            var foundStrings = new HashSet<string>(); // Use HashSet for uniqueness

            foreach (DataTable table in _dataSet.Tables)
            {
                // Skip tables missing required columns
                if (!nonEmptyConditions.All(c => !string.IsNullOrEmpty(c.SearchColumn) && table.Columns.Contains(c.SearchColumn)))
                    continue;

                var matchingRows = table.AsEnumerable()
                    .Where(row => nonEmptyConditions.All(cond =>
                        SearchHelpers.Matches(row[cond.SearchColumn]?.ToString() ?? string.Empty, cond.FindWhat, cond.CaseSensitive, cond.UseRegex)))
                    .ToList();

                foreach (var row in matchingRows)
                {
                    foreach (var cond in nonEmptyConditions)
                    {
                        var matchingValue = row[cond.SearchColumn]?.ToString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(matchingValue))
                        {
                            foundStrings.Add(matchingValue);
                        }
                    }
                }
            }

            return foundStrings.OrderBy(s => s).ToList();
        }

        private void PerformReplace()
        {
            var conditions = GetSearchConditions();
            var nonEmptyConditions = conditions.Where(c => !string.IsNullOrEmpty(c.FindWhat)).ToList();
            if (nonEmptyConditions.Count == 0)
                return;

            foreach (DataTable table in _dataSet.Tables)
            {
                // Skip tables missing required columns
                if (!nonEmptyConditions.All(c => !string.IsNullOrEmpty(c.SearchColumn) && table.Columns.Contains(c.SearchColumn)))
                    continue;

                var matchingRows = table.AsEnumerable()
                    .Where(row => nonEmptyConditions.All(cond =>
                        SearchHelpers.Matches(row[cond.SearchColumn]?.ToString() ?? string.Empty, cond.FindWhat, cond.CaseSensitive, cond.UseRegex)))
                    .ToList();

                foreach (var row in matchingRows)
                {
                    foreach (var cond in nonEmptyConditions)
                    {
                        var columnName = cond.SearchColumn;
                        var currentValue = row[columnName]?.ToString() ?? string.Empty;
                        var newValue = SearchHelpers.ApplyReplaces(currentValue, cond.ReplaceTasks, cond.CaseSensitive, cond.UseRegex);
                        row[columnName] = newValue;
                    }
                }
            }
        }

        private List<ISearchCondition> GetSearchConditions()
        {
            var conditions = new List<ISearchCondition>();
            foreach (TabPage tabPage in SearchConditionsTabControl.TabPages)
            {
                if (tabPage.Controls.Count > 0)
                {
                    if (tabPage.Controls[0] is ISearchCondition conditionUC)
                    {
                        conditions.Add(conditionUC);
                    }
                }
            }
            return conditions;
        }
    }
}

using NLog;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Forms.Search.Data;
using TranslationHelper.Forms.Search.SearchNew.Data;
using TranslationHelper.Projects;

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
                SearchHelpers.TextSearchResultsReplacedPrefix :
                SearchHelpers.TextSearchResultsFoundPrefix;
            SearchResultInfoLabel.Text = string.Format(SearchHelpers.TextSearchResultsMatchingStringsMessage, actionName, searchResults.FoundRows.Count);

            if (searchResults.FoundRows.Count == 0)
            {
                return;
            }

            SearchHelpers.SaveSearchResults(searchResults, isReplace);

            SearchHelpers.BindSearchResults(searchResults, FoundRowsPanel, _project);
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
                SearchHelpers.RemoveSearchConditionTab(_searchConditionsTabControl, e.Tab, _dataSet.Tables);
            };
            SearchConditionTabsPanel.Controls.Add(_searchConditionsTabControl);

            AddNewSearchConditionTabButton.Click += (o, e) => SearchHelpers.AddSearchConditionTab(_searchConditionsTabControl, _dataSet.Tables);

            SearchHelpers.AddSearchConditionTab(_searchConditionsTabControl, _dataSet.Tables); // always one default search condition tab by default

        }

        private SearchResultsData PerformSearch(bool isReplace = false)
        {
            var searchResults = new SearchResultsData();

            var validConditions = SearchHelpers.GetValidConditions(SearchHelpers.GetSearchConditions(_searchConditionsTabControl.Tabs), isReplace);
            if (validConditions.Length == 0) return searchResults;

            searchResults.SearchConditions.AddRange(validConditions);

            foreach (DataTable table in _dataSet.Tables)
            {
                // Skip tables missing required columns
                if (!validConditions.All(c => !string.IsNullOrEmpty(c.SearchColumn) && table.Columns.Contains(c.SearchColumn)))
                    continue;

                var matchingRows = table.AsEnumerable()
                    .Where(row => validConditions.All(cond =>
                        SearchHelpers.Matches(row.Field<string>(cond.SearchColumn), cond.FindWhat, cond.CaseSensitive, cond.UseRegex)));

                foreach (var row in matchingRows)
                {
                    if (!isReplace || SearchHelpers.TryReplaceAny(validConditions, row, _project))
                    {
                        searchResults.FoundRows.Add(new FoundRowData(row));
                    }
                }
            }

            return searchResults;
        }
    }
}

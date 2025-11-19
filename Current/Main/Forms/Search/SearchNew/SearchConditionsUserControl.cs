using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Forms.Search.SearchNew.OptionsNew;
using Vip.ComboBox;

namespace TranslationHelper.Forms.Search
{
    public partial class SearchConditionUserControl : UserControl, ISearchCondition, ISearchConditionSearchResult
    {
        private TabControl _replaceWhatWithTabControl;

        public SearchConditionUserControl(string[] columns)
        {
            InitializeComponent();

            SearchOptionSelectedColumnComboBox.Items.AddRange(columns);
            if (columns.Length > 0)
            {
                SearchOptionSelectedColumnComboBox.SelectedIndex = AppData.CurrentProject.TranslationColumnIndex;
            }

            _replaceWhatWithTabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            ReplaceWhatWithPanel.Controls.Add(_replaceWhatWithTabControl);

            AddReplaceTab();
        }

        public void AddReplaceTab()
        {
            var tabPage = new TabPage($"Replace {(_replaceWhatWithTabControl.TabCount + 1)}");
            var replaceUC = new ReplaceWhatWithUserControl();
            tabPage.Controls.Add(replaceUC);
            replaceUC.Dock = DockStyle.Fill;
            _replaceWhatWithTabControl.TabPages.Add(tabPage);
            _replaceWhatWithTabControl.Update();
        }

        public void RemoveReplaceTab(TabPage tabPage)
        {
            if (_replaceWhatWithTabControl.TabCount > 1)
            {
                _replaceWhatWithTabControl.TabPages.Remove(tabPage);
            }
        }
        public string FindWhat => FindWhatComboBox.Text ?? string.Empty;

        public string SearchColumn => SearchOptionSelectedColumnComboBox.SelectedItem?.ToString() ?? string.Empty;

        public bool CaseSensitive => SearchOptionCaseSensitiveCheckBox.Checked;

        public bool UseRegex => SearchOptionRegexCheckBox.Checked;

        public List<IReplaceTask> ReplaceTasks
        {
            get
            {
                var tasks = new List<IReplaceTask>();
                foreach (TabPage tabPage in _replaceWhatWithTabControl.TabPages)
                {
                    tasks.Add(tabPage.Controls[0] as IReplaceTask);
                }

                return tasks;
            }
        }

        public ISearchOptionTarget Target { get; }

        public ISearchOptionMatch Searcher { get; }

        public ISearchOptionReplace Replacer { get; }

        private void SearchConditionUserControl_Load(object sender, EventArgs e)
        {
            LoadSearchQueries(false);
        }

        public (List<string> searchQueries, List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchQueries(bool isReplace)
        {
            var searchQueries = new List<string>();
            var searchReplacers = new List<string>();
            var searchReplacePatterns = new List<string>();

            if(!string.IsNullOrWhiteSpace(FindWhatComboBox.Text))
            {
                searchQueries.Add(FindWhatComboBox.Text);
            }
            SearchHelpers.AddMissing(searchQueries, FindWhatComboBox.Items.Cast<string>());

            if (!isReplace)
            {
                return (searchQueries, searchReplacers, searchReplacePatterns);
            }

            foreach (var replaceTask in ReplaceTasks.Cast<IReplaceTaskSearchResult>())
            {
                var results = replaceTask.GetSearchReplacers();

                SearchHelpers.AddMissing(searchReplacers, results.searchReplacers);
                SearchHelpers.AddMissing(searchReplacePatterns, results.searchReplacePatterns);
            }

            return (searchQueries, searchReplacers, searchReplacePatterns);
        }

        public void LoadSearchQueries(bool isReplace)
        {
            FindWhatComboBox.Items.Clear();
            var items = SearchSharedHelpers.LoadSearchQueries(THSettings.SearchQueriesSectionName).ToArray();
            FindWhatComboBox.Items.AddRange(items);

            if (!isReplace) return;

            foreach (var replaceTask in ReplaceTasks.Cast<IReplaceTaskSearchResult>())
            {
                replaceTask.LoadSearchReplacers();
            }
        }

        private void SearchOptionSelectedColumnComboBox_TextUpdate(object sender, EventArgs e)
        {

        }
    }
}

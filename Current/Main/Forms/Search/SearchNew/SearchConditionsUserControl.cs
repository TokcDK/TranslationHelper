using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Forms.Search.SearchNew.OptionsNew;

namespace TranslationHelper.Forms.Search
{
    public partial class SearchConditionUserControl : System.Windows.Forms.UserControl, ISearchCondition, ISearchConditionSearchResult
    {
        private readonly Manina.Windows.Forms.TabControl _replaceWhatWithTabControl;

        public SearchConditionUserControl(string[] columns)
        {
            InitializeComponent();

            SearchOptionSelectedColumnComboBox.Items.AddRange(columns);
            if (columns.Length > 0)
            {
                SearchOptionSelectedColumnComboBox.SelectedIndex = AppData.CurrentProject.TranslationColumnIndex;
            }

            _replaceWhatWithTabControl = new Manina.Windows.Forms.TabControl
            {
                Dock = DockStyle.Fill,
                ShowCloseTabButtons = true,
            };
            _replaceWhatWithTabControl.CloseTabButtonClick += (o, e) =>
            {
                e.Cancel = true; // cancel included removal function to remove manually with renumerate
                RemoveReplaceTab(e.Tab);
            };
            ReplaceWhatWithPanel.Controls.Add(_replaceWhatWithTabControl);

            AddReplaceTab();
        }

        public void AddReplaceTab()
        {
            var tab = new Tab() { Text = string.Format(T._("Replace {0}"), _replaceWhatWithTabControl.Tabs.Count + 1) };
            var replaceUC = new ReplaceWhatWithUserControl();
            tab.Controls.Add(replaceUC);
            replaceUC.Dock = DockStyle.Fill;
            _replaceWhatWithTabControl.Tabs.Add(tab);
            _replaceWhatWithTabControl.Update();
        }

        public void RemoveReplaceTab(Tab tab)
        {
            _replaceWhatWithTabControl.Tabs.Remove(tab);
            if (_replaceWhatWithTabControl.Tabs.Count == 0)
            {
                AddReplaceTab(); // always one default search replacer tab by default
            }
            else
            {
                SearchHelpers.RenumerateTabNames(_replaceWhatWithTabControl.Tabs, SearchHelpers.TextReplacerTabIndexed);
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
                foreach (var tab in _replaceWhatWithTabControl.Tabs)
                {
                    tasks.Add(tab.Controls[0] as IReplaceTask);
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

            if (!string.IsNullOrWhiteSpace(FindWhatComboBox.Text))
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

                SearchHelpers.AddMissing(searchReplacePatterns, results.searchReplacePatterns);
                SearchHelpers.AddMissing(searchReplacers, results.searchReplacers);
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

        private void ReplaceWhatWithAddTabButton_Click(object sender, EventArgs e)
        {
            AddReplaceTab();
        }
    }
}

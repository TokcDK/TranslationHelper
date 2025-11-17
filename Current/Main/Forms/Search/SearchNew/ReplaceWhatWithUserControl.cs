using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Forms.Search
{
    public partial class ReplaceWhatWithUserControl : UserControl, IReplaceTask, IReplaceTaskSearchResult
    {
        public ReplaceWhatWithUserControl()
        {
            InitializeComponent();
        }

        public string ReplaceWhat => ReplaceWhatComboBox.Text ?? string.Empty;
        public string ReplaceWith => ReplaceWithComboBox.Text ?? string.Empty;

        public (List<string> searchReplacers, List<string> SearchReplacePatterns) GetSearchReplacers()
        {
            return (ReplaceWhatComboBox.Items.Cast<string>().ToList()
                , ReplaceWithComboBox.Items.Cast<string>().ToList());
        }

        public void LoadSearchReplacers()
        {
            ReplaceWithComboBox.Items.Clear();
            var items = SearchSharedHelpers.LoadSearchQueries(THSettings.SearchReplacersSectionName).ToArray();
            ReplaceWithComboBox.Items.AddRange(items);

            ReplaceWithComboBox.Items.Clear();
            items = SearchSharedHelpers.LoadSearchQueries(THSettings.SearchReplacePatternsSectionName).ToArray();
            ReplaceWithComboBox.Items.AddRange(items);
        }

        private void ReplaceWhatWithUserControl_Load(object sender, EventArgs e)
        {
            LoadSearchReplacers();
        }
    }
}

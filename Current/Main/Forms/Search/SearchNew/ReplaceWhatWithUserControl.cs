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

        public (List<string> searchReplacers, List<string> searchReplacePatterns) GetSearchReplacers()
        {
            var searchReplacers = new List<string>();
            var searchReplacePatterns = new List<string>();

            foreach(var (list, comboBox) in new[]
            {
                (searchReplacers, ReplaceWhatComboBox),
                (searchReplacePatterns, ReplaceWithComboBox),
            })
            {
                if (!string.IsNullOrWhiteSpace(comboBox.Text))
                {
                    list.Add(comboBox.Text);
                }
                var items = comboBox.Items.Cast<string>().ToList();
                if (items.Count > 0)
                {
                    list.AddRange(items);
                }
            }
            
            return (searchReplacers, searchReplacePatterns);
        }

        public void LoadSearchReplacers()
        {
            ReplaceWhatComboBox.Items.Clear();
            var items = SearchSharedHelpers.LoadSearchQueries(THSettings.SearchReplacersSectionName).ToArray();
            ReplaceWhatComboBox.Items.AddRange(items);

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

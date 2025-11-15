using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Forms.Search
{
    public partial class SearchConditionUserControl : UserControl, ISearchCondition
    {
        public SearchConditionUserControl(string[] columns)
        {
            InitializeComponent();

            SearchOptionSelectedColumnComboBox.Items.AddRange(columns);
            if (columns.Length > 0)
            {
                SearchOptionSelectedColumnComboBox.SelectedIndex = 0;
            }

            AddReplaceTab();
        }

        public void AddReplaceTab()
        {
            var tabPage = new TabPage($"Replace {(ReplaceWhatWithTabControl.TabCount + 1)}");
            var replaceUC = new ReplaceWhatWithUserControl();
            tabPage.Controls.Add(replaceUC);
            replaceUC.Dock = DockStyle.Fill;
            ReplaceWhatWithTabControl.TabPages.Add(tabPage);
            ReplaceWhatWithTabControl.Update();
        }

        public void RemoveReplaceTab(TabPage tabPage)
        {
            if (ReplaceWhatWithTabControl.TabCount > 1)
            {
                ReplaceWhatWithTabControl.TabPages.Remove(tabPage);
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
                foreach (TabPage tabPage in ReplaceWhatWithTabControl.TabPages)
                {
                    tasks.Add(tabPage.Controls[0] as IReplaceTask);
                }

                return tasks;
            }
        }
    }
}

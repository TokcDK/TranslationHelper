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
        public SearchConditionUserControl()
        {
            AddReplaceTab();
            InitializeComponent();
        }

        public void AddReplaceTab()
        {
            var tabPage = new TabPage($"Replace {(ReplaceWhatWithTabControl.TabCount + 1)}");
            var replaceUC = new ReplaceWhatWithUserControl();
            tabPage.Controls.Add(replaceUC);
            replaceUC.Dock = DockStyle.Fill;
            ReplaceWhatWithTabControl.TabPages.Add(tabPage);
        }

        public void RemoveReplaceTab(TabPage tabPage)
        {
            if (ReplaceWhatWithTabControl.TabCount > 1)
            {
                ReplaceWhatWithTabControl.TabPages.Remove(tabPage);
            }
        }
        public string FindWhat => !string.IsNullOrEmpty(FindWhatComboBox.SelectedValue?.ToString())
            ? FindWhatComboBox.SelectedValue.ToString()
            : string.Empty;

        public string SearchColumn => SearchOptionSelectedColumnComboBox.SelectedValue?.ToString() ?? string.Empty;

        public bool CaseSensitive => SearchOptionCaseSensitiveCheckBox.Checked;

        public bool UseRegex => SearchOptionRegexCheckBox.Checked;

        public IReadOnlyList<IReplaceTask> ReplaceTasks
        {
            get
            {
                var tasks = new List<IReplaceTask>();
                foreach (TabPage tabPage in ReplaceWhatWithTabControl.TabPages)
                {
                    if (tabPage.Controls.Count > 0)
                    {
                        if (tabPage.Controls[0] is IReplaceTask replaceUC)
                        {
                            tasks.Add(replaceUC);
                        }
                    }
                }
                return tasks.AsReadOnly();
            }
        }
    }
}

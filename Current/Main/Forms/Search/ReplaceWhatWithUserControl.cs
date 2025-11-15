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
    public partial class ReplaceWhatWithUserControl : UserControl, IReplaceTask
    {
        public ReplaceWhatWithUserControl()
        {
            ReplaceWhatComboBox = new ComboBox();
            ReplaceWithComboBox = new ComboBox();

            InitializeComponent();
        }

        public string ReplaceWhat => !string.IsNullOrEmpty(ReplaceWhatComboBox.SelectedValue?.ToString())
            ? ReplaceWhatComboBox.SelectedValue.ToString()
            : string.Empty;

        public string ReplaceWith => ReplaceWithComboBox.SelectedValue?.ToString() ?? string.Empty;
    }
}

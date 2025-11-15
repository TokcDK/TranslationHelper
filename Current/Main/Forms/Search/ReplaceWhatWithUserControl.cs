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
            InitializeComponent();
        }

        public string ReplaceWhat => ReplaceWhatComboBox.Text ?? string.Empty;
        public string ReplaceWith => ReplaceWithComboBox.Text ?? string.Empty;
    }
}

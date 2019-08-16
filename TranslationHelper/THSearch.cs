using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THSearch : Form
    {
        public THSearch()
        {
            InitializeComponent();
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = false;
            radioButton1.Checked = true;
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = true;
        }

        private void RadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Checked = false;
            radioButton4.Checked = true;
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton4.Checked = false;
            radioButton3.Checked = true;
        }
    }
}

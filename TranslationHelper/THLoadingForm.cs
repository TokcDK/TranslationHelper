using System;
using System.Drawing;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THLoadingForm : Form
    {
        public THLoadingForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        private void THLoadingForm_Load(object sender, EventArgs e)
        {
            //this.SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = System.Drawing.Color.Transparent;
        }
    }
}

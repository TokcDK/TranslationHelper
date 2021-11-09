using System;
using System.Windows.Forms;

namespace TranslationHelper.Projects.WolfRPG.Menus
{
    public partial class AddToStandaloneContextListForm : Form
    {
        public AddToStandaloneContextListForm()
        {
            InitializeComponent();
        }

        public string ContextLine = "";
        private void button1_Click(object sender, EventArgs e)
        {
            ContextLine = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

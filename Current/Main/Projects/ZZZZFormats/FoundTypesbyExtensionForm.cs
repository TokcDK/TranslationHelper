using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Projects.ZZZZFormats
{
    public partial class FoundTypesbyExtensionForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FoundTypesbyExtensionForm()
        {
            InitializeComponent();

            Logger.Info("Select project to try open with.");
        }

        public int SelectedTypeIndex = -1;

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedTypeIndex = listBox1.SelectedIndex;
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

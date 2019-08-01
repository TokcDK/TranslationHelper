using System;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THSettingsForm : Form
    {
        //Defaults
        public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");

        public THSettingsForm()
        {
            InitializeComponent();
        }

        public void GetSettings()
        {
            try
            {
                //MessageBox.Show("Optimizations exist="+ THConfigINI.KeyExists("Optimizations", "THOptionDontLoadStringIfRomajiPercent"));
                if (THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercent", "Optimizations"))
                {
                    THOptionDontLoadStringIfRomajiPercentTextBox.Text = THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent");
                    //MessageBox.Show("GetSettings() Get "+ THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDontLoadStringIfRomajiPercentTextBox.Text = "80";
                }
                if (THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", "Optimizations"))
                {
                    THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked"));
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = true;
                }

                if (THConfigINI.KeyExists("THOptionDBCompression", "Optimizations"))
                {
                    THOptionDBCompressionComboBox.SelectedItem = THConfigINI.ReadINI("Optimizations", "THOptionDBCompression");
                    //MessageBox.Show("GetSettings() Get "+ THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDBCompressionComboBox.SelectedItem = "XML (None)";
                }
                if (THConfigINI.KeyExists("THOptionDBCompressionCheckBox.Checked", "Optimizations"))
                {
                    THOptionDBCompressionCheckBox.Checked = bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDBCompressionCheckBox.Checked = true;
                }
            }
            catch
            {

            }
        }

        private void THOptionDontLoadStringIfRomajiPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidTHOptionDontLoadStringIfRomajiPercent();
        }

        private void ValidTHOptionDontLoadStringIfRomajiPercent()
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(THOptionDontLoadStringIfRomajiPercentTextBox.Text, "^[0-9]{1,3}$") && int.Parse(THOptionDontLoadStringIfRomajiPercentTextBox.Text) <= 100)
            {
                //MessageBox.Show("80");
                //THOptionDontLoadStringIfRomajiPercentTextBox.Text = "80";
                //MessageBox.Show("Set");
                THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent", THOptionDontLoadStringIfRomajiPercentTextBox.Text);
            }
            else
            {
                //MessageBox.Show("ValidTHOptionDontLoadStringIfRomajiPercent() 80");
                THOptionDontLoadStringIfRomajiPercentTextBox.Text = "80";
            }
        }

        private void THProgramSettingsForm_Load(object sender, EventArgs e)
        {
            GetSettings();
        }

        private void THOptionDontLoadStringIfRomajiPercentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("THOptionDontLoadStringIfRomajiPercentCheckBox.Checked.ToString()="+THOptionDontLoadStringIfRomajiPercentCheckBox.Checked.ToString());
            THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", THOptionDontLoadStringIfRomajiPercentCheckBox.Checked.ToString());
        }

        private void THOptionDBCompressionComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Optimizations", "THOptionDBCompression", THOptionDBCompressionComboBox.SelectedItem.ToString());
        }

        private void THOptionDBCompressionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Optimizations", "THOptionDBCompressionCheckBox.Checked", THOptionDBCompressionCheckBox.Checked.ToString());
        }
    }
}

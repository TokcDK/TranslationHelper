using System;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THSettingsForm : Form
    {
        //Defaults
        public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");
        public int THOptionDontLoadStringIfRomajiPercentValue = 90;
        public bool THOptionDontLoadStringIfRomajiPercentCheckBoxChecked = true;
        public bool THOptionDBCompressionCheckBoxChecked = true;
        public string THOptionDBCompressionComboBoxSelectedItemValue = "XML (None)";

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
                    THOptionDontLoadStringIfRomajiPercentValue = int.Parse(THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                    //MessageBox.Show("GetSettings() Get "+ THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDontLoadStringIfRomajiPercentTextBox.Text = THOptionDontLoadStringIfRomajiPercentValue.ToString();
                }
                if (THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", "Optimizations"))
                {
                    THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked"));
                    THOptionDontLoadStringIfRomajiPercentCheckBoxChecked = THOptionDontLoadStringIfRomajiPercentCheckBox.Checked;
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = true;
                }

                if (THConfigINI.KeyExists("THOptionDBCompression", "Optimizations"))
                {
                    THOptionDBCompressionComboBox.SelectedItem = THConfigINI.ReadINI("Optimizations", "THOptionDBCompression");
                    THOptionDBCompressionComboBoxSelectedItemValue = THOptionDBCompressionComboBox.SelectedItem.ToString();
                    //MessageBox.Show("GetSettings() Get "+ THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDBCompressionComboBox.SelectedItem = THOptionDBCompressionComboBoxSelectedItemValue;
                }
                if (THConfigINI.KeyExists("THOptionDBCompressionCheckBox.Checked", "Optimizations"))
                {
                    THOptionDBCompressionCheckBox.Checked = bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
                    THOptionDBCompressionCheckBoxChecked = THOptionDBCompressionCheckBox.Checked;
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THOptionDBCompressionCheckBox.Checked = THOptionDBCompressionCheckBoxChecked;
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
                //MessageBox.Show("Set"+ THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent", THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                THOptionDontLoadStringIfRomajiPercentValue = int.Parse(THOptionDontLoadStringIfRomajiPercentTextBox.Text);
            }
            else
            {
                //MessageBox.Show("ValidTHOptionDontLoadStringIfRomajiPercent() 80");
                THOptionDontLoadStringIfRomajiPercentTextBox.Text = THOptionDontLoadStringIfRomajiPercentValue.ToString();
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
            THOptionDontLoadStringIfRomajiPercentCheckBoxChecked = THOptionDontLoadStringIfRomajiPercentCheckBox.Checked;
        }

        private void THOptionDBCompressionComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Optimizations", "THOptionDBCompression", THOptionDBCompressionComboBox.SelectedItem.ToString());
            THOptionDBCompressionComboBoxSelectedItemValue = THOptionDBCompressionComboBox.SelectedItem.ToString();
        }

        private void THOptionDBCompressionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Optimizations", "THOptionDBCompressionCheckBox.Checked", THOptionDBCompressionCheckBox.Checked.ToString());
            THOptionDBCompressionCheckBoxChecked = THOptionDBCompressionCheckBox.Checked;
        }
    }
}

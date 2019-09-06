using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THSettings : Form
    {
        //Defaults
        public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");
        public int THOptionDontLoadStringIfRomajiPercentValue = 90;
        public bool THOptionDontLoadStringIfRomajiPercentCheckBoxChecked = true;
        public bool THOptionDBCompressionCheckBoxChecked = true;
        public string THOptionDBCompressionComboBoxSelectedItemValue = "XML (None)";

        public THSettings()
        {
            InitializeComponent();
            SetTooltips();
        }

        private void SetTooltips()
        {
            //http://qaru.site/questions/47162/c-how-do-i-add-a-tooltip-to-a-control
            //THMainResetTableButton
            ToolTip THToolTip = new ToolTip
            {

                // Set up the delays for the ToolTip.
                AutoPopDelay = 10000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                UseAnimation = true,
                UseFading = true,
                // Force the ToolTip text to be displayed whether or not the form is active.
                ShowAlways = true
            };

            //Settings
            //Optimization
            THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBox, "String will not be loaded for translation if this string contains romaji characters in text more of specified percent.");
            THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen, "Is true while opening. Always true for RPGMaker MV files or jsons for strings filtering purposes.");
            THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation, "Is true while online translating. Will be used both with other chars like .!/? and other same");
            THToolTip.SetToolTip(THOptionDBCompressionCheckBox, "Format for DB files: standard not compressed xml and compressed xml for both other");
            //Tools
            THToolTip.SetToolTip(THOptionAutotranslationForIdenticalCheckBox, "Automatically will be translated all almost identical cells with same original.");
            THToolTip.SetToolTip(THOptionEnableTranslationCacheCheckBox, "Will save online translation result in cache db to use it in next time for same values instead of attemp to connect to service.");
            THToolTip.SetToolTip(THSettingsWebTransLinkTextBox, "Web site which wil be opened by pressing F12 key with added selected table cells values. Can be any here Google, Yandex, DeepL or other.");
            //support links
            THToolTip.SetToolTip(linkLabel1, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            THToolTip.SetToolTip(linkLabel2, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            THToolTip.SetToolTip(linkLabel4, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            ////////////////////////////
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
                if (THConfigINI.KeyExists("THOptionWebPageLinkForManualTranslation", "Tools"))
                {
                    THSettingsWebTransLinkTextBox.Text = THConfigINI.ReadINI("Tools", "THOptionWebPageLinkForManualTranslation");
                }
                else
                {
                    //MessageBox.Show("GetSettings() 81");
                    THSettingsWebTransLinkTextBox.Text = "https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}";
                }
                if (THConfigINI.KeyExists("THOptionEnableTranslationCacheCheckBox.Checked", "Tools"))
                {
                    THOptionEnableTranslationCacheCheckBox.Checked = bool.Parse(THConfigINI.ReadINI("Tools", "THOptionEnableTranslationCacheCheckBox.Checked"));
                }
                else
                {
                    THOptionEnableTranslationCacheCheckBox.Checked = true;
                }
                if (THConfigINI.KeyExists("THOptionAutotranslationForIdenticalCheckBox.Checked", "Tools"))
                {
                    THOptionAutotranslationForIdenticalCheckBox.Checked = bool.Parse(THConfigINI.ReadINI("Tools", "THOptionAutotranslationForIdenticalCheckBox.Checked"));
                }
                else
                {
                    THOptionAutotranslationForIdenticalCheckBox.Checked = true;
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

        private void THSettingsWebTransLinkTextBox_Validated(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Tools", "THOptionWebPageLinkForManualTranslation", THSettingsWebTransLinkTextBox.Text);
        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }

        private void THOptionEnableTranslationCacheCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Tools", "THOptionEnableTranslationCacheCheckBox.Checked", THOptionEnableTranslationCacheCheckBox.Checked.ToString());
        }

        private void THOptionAutotranslationForIdenticalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            THConfigINI.WriteINI("Tools", "THOptionAutotranslationForIdenticalCheckBox.Checked", THOptionAutotranslationForIdenticalCheckBox.Checked.ToString());
        }
    }
}

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THSettings : Form
    {
        //Defaults
        internal IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");

        public THSettings()
        {
            InitializeComponent();

            SetTooltips();

            SetUIStrings();
        }

        private void SetUIStrings()
        {
            //translation
            THOptionFullComprasionDBload.Text = T._("Full recursive scan while translation DB loading (slower)");
            THOptionLineCharLimitLabel.Text = "- "+T._("char limit of line length (for line split functions)");
            this.THSettingsMainTabPage.Text = T._("General");
            //this.label3.Text = T._("Translation  Helper support:");
            this.THSettingsOptimizationsTabPage.Text = T._("Optimizations");
            //this.label2.Text = T._("Translation  Helper support:");
            this.THOptionDBCompressionCheckBox.Text = T._("Compression for DB files:");
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Text = T._("Do not load string if it has more of next romaji percent - ");
            this.THSettingsToolsTabPage.Text = T._("Tools");
            this.THOptionAutotranslationForIdenticalCheckBox.Text = T._("Autotranslation for simular");
            this.THOptionEnableTranslationCacheCheckBox.Text = T._("Enable online translation cache.");
            //this.label4.Text = T._("Translation  Helper support:");
            this.label1.Text = T._("Web service link for manual translation (F12):");
            this.Text = T._("Settings");
        }

        ToolTip THToolTip;
        private void SetTooltips()
        {
            //http://qaru.site/questions/47162/c-how-do-i-add-a-tooltip-to-a-control
            //THMainResetTableButton
            THToolTip = new ToolTip
            {

                // Set up the delays for the ToolTip.
                AutoPopDelay = 32000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                UseAnimation = true,
                UseFading = true,
                // Force the ToolTip text to be displayed whether or not the form is active.
                ShowAlways = true
            };

            //Settings
            //General
            THToolTip.SetToolTip(THOptionFullComprasionDBload, T._("In time of DB loading will be checked all DB lines for each line of table for translation."));
            //Optimization
            THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBox, T._("String will not be loaded for translation if this string contains romaji characters in text more of specified percent."));
            
            //THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen, "Is true while opening. Always true for RPGMaker MV files or jsons for strings filtering purposes.");
            //THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation, T._("Is true while online translating. Will be used both with other chars like .!/? and other same"));
            THToolTip.SetToolTip(THOptionDBCompressionCheckBox, T._("Format for DB files: standard not compressed xml and compressed xml for both other"));
            //Tools
            THToolTip.SetToolTip(THOptionAutotranslationForIdenticalCheckBox, T._("Automatically will be translated all almost identical cells with same original."));
            THToolTip.SetToolTip(THOptionEnableTranslationCacheCheckBox, T._("Will save online translation result in cache db to use it in next time for same values instead of attemp to connect to service."));
            THToolTip.SetToolTip(THSettingsWebTransLinkTextBox, T._("Web site which wil be opened by pressing F12 key with added selected table cells values. Can be any here Google, Yandex, DeepL or other."));
            //support links
            //THToolTip.SetToolTip(linkLabel1, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            //THToolTip.SetToolTip(linkLabel2, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            //THToolTip.SetToolTip(linkLabel4, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            ////////////////////////////
        }

        public void GetSettings()
        {
            try
            {
                //General
                THOptionFullComprasionDBload.Checked = FullComprasionDBloadINI;

                //Optimizations
                THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = DontLoadStringIfRomajiPercentINI;
                THOptionDontLoadStringIfRomajiPercentTextBox.Text = DontLoadStringIfRomajiPercentNumINI.ToString();                
                THOptionDBCompressionCheckBox.Checked = DBCompressionINI;
                THOptionDBCompressionComboBox.SelectedItem = DBCompressionTypeINI;
                
                //Tools
                THSettingsWebTransLinkTextBox.Text = WebTransLinkINI;
                THOptionEnableTranslationCacheCheckBox.Checked = EnableTranslationCacheINI;
                THOptionAutotranslationForIdenticalCheckBox.Checked = AutotranslationForIdenticalINI;
            }
            catch
            {

            }
        }

        public int DontLoadStringIfRomajiPercentNumINI
        {
            get => THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercent", "Optimizations")
                    ? int.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent"))
                    : Properties.Settings.Default.DontLoadStringIfRomajiPercentNum;
            set => THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent", value.ToString());
        }

        public bool DontLoadStringIfRomajiPercentINI
        {
            get => THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", "Optimizations")
                    ? bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked"))
                    : Properties.Settings.Default.DontLoadStringIfRomajiPercent;
            set => THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", value.ToString());
        }

        public bool DBCompressionINI
        {
            get => THConfigINI.KeyExists("THOptionDBCompressionCheckBox.Checked", "Optimizations")
                    ? bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"))
                    : true;
            set => THConfigINI.WriteINI("Optimizations", "THOptionDBCompressionCheckBox.Checked", value.ToString());
        }

        public string DBCompressionTypeINI
        {
            get => THConfigINI.KeyExists("THOptionDBCompression", "Optimizations")
                    ? THConfigINI.ReadINI("Optimizations", "THOptionDBCompression")
                    : "XML (none)";
            set => THConfigINI.WriteINI("Optimizations", "THOptionDBCompression", value);
        }

        public string WebTransLinkINI
        {
            get => THConfigINI.KeyExists("THOptionWebPageLinkForManualTranslation", "Tools")
                    ? THConfigINI.ReadINI("Tools", "THOptionWebPageLinkForManualTranslation")
                    : "https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}";
            set => THConfigINI.WriteINI("Tools", "THOptionWebPageLinkForManualTranslation", value);
        }

        public bool EnableTranslationCacheINI
        {
            get => THConfigINI.KeyExists("THOptionEnableTranslationCacheCheckBox.Checked", "Tools")
                    ? bool.Parse(THConfigINI.ReadINI("Tools", "THOptionEnableTranslationCacheCheckBox.Checked"))
                    : true;
            set => THConfigINI.WriteINI("Tools", "THOptionEnableTranslationCacheCheckBox.Checked", value.ToString());
        }

        public bool AutotranslationForIdenticalINI
        {
            get => THConfigINI.KeyExists("THOptionAutotranslationForIdenticalCheckBox.Checked", "Tools")
                    ? bool.Parse(THConfigINI.ReadINI("Tools", "THOptionAutotranslationForIdenticalCheckBox.Checked"))
                    : Properties.Settings.Default.AutotranslationForSimular;
            set => THConfigINI.WriteINI("Tools", "THOptionAutotranslationForIdenticalCheckBox.Checked", value.ToString());
        }

        public bool FullComprasionDBloadINI
        {
            get => THConfigINI.KeyExists("THOptionFullComprasionDBload.Checked", "General")
                    ? bool.Parse(THConfigINI.ReadINI("General", "THOptionFullComprasionDBload.Checked"))
                    : true;
            set => THConfigINI.WriteINI("General", "THOptionFullComprasionDBload.Checked", value.ToString());
        }

        public int LineCharLimitINI
        {
            get => THConfigINI.KeyExists("THOptionLineCharLimit", "General")
                    ? int.Parse(THConfigINI.ReadINI("General", "LineCharLimit"))
                    : 60;
            set => THConfigINI.WriteINI("General", "THOptionLineCharLimit", value.ToString());
        }

        private void THOptionDontLoadStringIfRomajiPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidTHOptionDontLoadStringIfRomajiPercent();
        }

        private void ValidTHOptionDontLoadStringIfRomajiPercent()
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(THOptionDontLoadStringIfRomajiPercentTextBox.Text, "^[0-9]{1,3}$") && int.Parse(THOptionDontLoadStringIfRomajiPercentTextBox.Text) <= 100)
            {
                int newvalue = int.Parse(THOptionDontLoadStringIfRomajiPercentTextBox.Text);
                Properties.Settings.Default.DontLoadStringIfRomajiPercentNum = newvalue;
                DontLoadStringIfRomajiPercentNumINI = newvalue;                
            }
            else
            {
                THOptionDontLoadStringIfRomajiPercentTextBox.Text = "90";
            }
        }

        private void THProgramSettingsForm_Load(object sender, EventArgs e)
        {
            GetSettings();
        }

        private void THOptionDontLoadStringIfRomajiPercentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool newvalue = THOptionDontLoadStringIfRomajiPercentCheckBox.Checked;
            Properties.Settings.Default.DontLoadStringIfRomajiPercent = newvalue;
            DontLoadStringIfRomajiPercentINI = newvalue;            
        }

        private void THOptionDBCompressionComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DBCompressionTypeINI = THOptionDBCompressionComboBox.SelectedItem.ToString();
        }

        private void THOptionDBCompressionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DBCompressionINI = THOptionDBCompressionCheckBox.Checked;           
        }

        private void THSettingsWebTransLinkTextBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.WebTranslationLink = THSettingsWebTransLinkTextBox.Text;
            WebTransLinkINI = THSettingsWebTransLinkTextBox.Text;
        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Process.Start("https://patreon.com/TranslationHelper");
        }

        private void THOptionEnableTranslationCacheCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void THOptionAutotranslationForIdenticalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutotranslationForSimular = THOptionAutotranslationForIdenticalCheckBox.Checked;
            AutotranslationForIdenticalINI = THOptionAutotranslationForIdenticalCheckBox.Checked;
        }

        private void THOptionEnableTranslationCacheCheckBox_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsTranslationCacheEnabled = THOptionEnableTranslationCacheCheckBox.Checked;
            EnableTranslationCacheINI = THOptionEnableTranslationCacheCheckBox.Checked;            
        }

        private void THSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            THToolTip.Dispose();
        }

        private void FullComparasionDBload_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsFullComprasionDBloadEnabled = THOptionFullComprasionDBload.Checked;
            FullComprasionDBloadINI = THOptionFullComprasionDBload.Checked;
        }

        private void LineCharLimitTextBox_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LineCharLimitTextBox.Text, "^[0-9]{1,4}$") && int.Parse(LineCharLimitTextBox.Text) <= 9999)
            {
                int newvalue = int.Parse(LineCharLimitTextBox.Text);
                Properties.Settings.Default.THOptionLineCharLimit = newvalue;
                LineCharLimitINI = newvalue;
            }
            else
            {
                LineCharLimitTextBox.Text = "60";
            }

        }
    }
}

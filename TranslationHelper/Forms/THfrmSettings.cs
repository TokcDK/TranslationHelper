using AIHelper.Manage;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.INISettings;


//посмотреть также это: https://codereview.stackexchange.com/questions/84281/abstracted-interface-for-settings
//посмотреть также это: https://overcoder.net/q/1042/%D0%BB%D1%83%D1%87%D1%88%D0%B0%D1%8F-%D0%BF%D1%80%D0%B0%D0%BA%D1%82%D0%B8%D0%BA%D0%B0-%D1%81%D0%BE%D1%85%D1%80%D0%B0%D0%BD%D0%B5%D0%BD%D0%B8%D1%8F-%D0%BD%D0%B0%D1%81%D1%82%D1%80%D0%BE%D0%B5%D0%BA-%D0%BF%D1%80%D0%B8%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D1%8F-%D0%B2-%D0%BF%D1%80%D0%B8%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B8-windows-forms
//посмотреть также это: https://upread.ru/art.php?id=356
namespace TranslationHelper
{
    public partial class THfrmSettings : Form
    {
        //Defaults
        internal INIFile THConfigINI = new INIFile(Application.ProductName + ".ini");
        readonly THDataWork thDataWork;
        internal THfrmSettings(THDataWork thDataWork)
        {
            InitializeComponent();

            this.thDataWork = thDataWork;

            SetTooltips();

            SetUIStrings();

            //var r = new AppSettings().AutotranslationForSimular;

            //var s = SettingsList["AutotranslationForSimular"].

            //GetSettings();
        }

        private void SetUIStrings()
        {
            //translation
            THOptionFullComprasionDBloadCheckBox.Text = T._("Full recursive scan while translation DB loading (slower)");
            THOptionLineCharLimitLabel.Text = "- " + T._("char limit of line length (for line split functions)");
            this.THSettingsMainTabPage.Text = T._("General");
            //this.label3.Text = T._("Translation  Helper support:");
            this.THSettingsOptimizationsTabPage.Text = T._("Optimizations");
            //this.label2.Text = T._("Translation  Helper support:");
            this.THOptionDBCompressionCheckBox.Text = T._("Compression for DB files:");
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Text = T._("Do not load string if it has more of next romaji percent - ");
            this.THSettingsToolsTabPage.Text = T._("Tools");
            this.THOptionAutotranslationForSimularCheckBox.Text = T._("Autotranslation for simular");
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
            THToolTip.SetToolTip(THOptionFullComprasionDBloadCheckBox, T._("In time of DB loading will be checked all DB lines for each line of table for translation."));
            //Optimization
            THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBox, T._("String will not be loaded for translation if this string contains romaji characters in text more of specified percent."));

            //THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen, "Is true while opening. Always true for RPGMaker MV files or jsons for strings filtering purposes.");
            //THToolTip.SetToolTip(THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation, T._("Is true while online translating. Will be used both with other chars like .!/? and other same"));
            THToolTip.SetToolTip(THOptionDBCompressionCheckBox, T._("Format for DB files: standard not compressed xml and compressed xml for both other"));
            //Tools
            THToolTip.SetToolTip(THOptionAutotranslationForSimularCheckBox, T._("Automatically will be translated all almost identical cells with same original."));
            THToolTip.SetToolTip(THOptionEnableTranslationCacheCheckBox, T._("Will save online translation result in cache db to use it in next time for same values instead of attemp to connect to service."));
            THToolTip.SetToolTip(THSettingsWebTranslationLinkTextBox, T._("Web site which wil be opened by pressing F12 key with added selected table cells values. Can be any here Google, Yandex, DeepL or other."));
            //support links
            //THToolTip.SetToolTip(linkLabel1, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            //THToolTip.SetToolTip(linkLabel2, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            //THToolTip.SetToolTip(linkLabel4, "Report bugs,\n offer features\\ideas\n or just support development if you like it.\n Any support is essential.");
            ////////////////////////////
        }

        public void GetSettings()
        {
            if (thDataWork.SettingsIsLoading)
            {
                return;
            }

            thDataWork.SettingsIsLoading = true;

            //try
            //{
            //General
            //THOptionFullComprasionDBload.Checked = FullComprasionDBloadINI;
            //LineCharLimitTextBox.Text = LineCharLimitINI.ToString(CultureInfo.InvariantCulture);

            //Optimizations
            //THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = DontLoadStringIfRomajiPercentINI;
            //DontLoadStringIfRomajiPercentNumberTextBox.Text = DontLoadStringIfRomajiPercentNumINI.ToString(CultureInfo.InvariantCulture);
            //THOptionDBCompressionCheckBox.Checked = DBCompressionINI;
            //THOptionDBCompressionExtComboBox.SelectedItem = DBCompressionTypeINI;

            //Tools
            //THSettingsWebTranslationLinkTextBox.Text = WebTransLinkINI;
            //THOptionEnableTranslationCacheCheckBox.Checked = EnableTranslationCacheINI;
            //THOptionAutotranslationForSimularCheckBox.Checked = AutotranslationForIdenticalINI;

            SettingsList = SettingsBaseTools.GetSettingsList(thDataWork);

            foreach (var setting in SettingsList.Keys)
            {
                if (THConfigINI.KeyExists(SettingsList[setting].Key, SettingsList[setting].Section))
                {
                    thDataWork.BufferValueString = THConfigINI.ReadINI(SettingsList[setting].Section, SettingsList[setting].Key);
                }
                else
                {
                    thDataWork.BufferValueString = SettingsList[setting].Default;
                }
                SettingsList[setting].Set(true);
            }
            //}
            //catch { }

            AddQuickWebTranslators();

            thDataWork.SettingsIsLoading = false;
        }

        private void AddQuickWebTranslators()
        {
            if(flpQuickTranslatorSelection.Controls.Count== cbxWebTranslatorsSelector.Items.Count)
            {
                return;
            }

            foreach(string line in cbxWebTranslatorsSelector.Items)
            {
                LinkLabel L = new LinkLabel
                {
                    Text = cleanline(line),
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    AutoSize=true

                };

                L.LinkClicked += (s, e) =>
                {
                    THSettingsWebTranslationLinkTextBox.Text = line;
                };

                flpQuickTranslatorSelection.Controls.Add(L);
            }
        }

        static string cleanline(string line)
        {
            if (line.ToUpperInvariant().StartsWith("HTTPS://"))
            {
                line = line.Remove(0, 8);
            }
            else if (line.ToUpperInvariant().StartsWith("HTTP://"))
            {
                line = line.Remove(0, 7);
            }

            if (line.ToUpperInvariant().StartsWith("WWW."))
                line = line.Remove(0, 4);

            if (line.ToUpperInvariant().StartsWith("TRANSLATE."))
                line = line.Remove(0, 10);

            return line[0].ToString().ToUpperInvariant();
        }

        //public int DontLoadStringIfRomajiPercentNumINI
        //{
        //    get => THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercent", "Optimizations")
        //            ? int.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent"), CultureInfo.InvariantCulture)
        //            : Properties.Settings.Default.DontLoadStringIfRomajiPercentNumber;
        //    set => THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercent", value.ToString(CultureInfo.InvariantCulture));
        //}

        //public bool DontLoadStringIfRomajiPercentINI
        //{
        //    get => THConfigINI.KeyExists("THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", "Optimizations")
        //            ? bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked"))
        //            : Properties.Settings.Default.DontLoadStringIfRomajiPercent;
        //    set => THConfigINI.WriteINI("Optimizations", "THOptionDontLoadStringIfRomajiPercentCheckBox.Checked", value.ToString(CultureInfo.InvariantCulture));
        //}

        //public bool DBCompressionINI
        //{
        //    get => THConfigINI.KeyExists("THOptionDBCompressionCheckBox.Checked", "Optimizations")
        //            ? bool.Parse(THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"))
        //            : true;
        //    set => THConfigINI.WriteINI("Optimizations", "THOptionDBCompressionCheckBox.Checked", value.ToString(CultureInfo.InvariantCulture));
        //}

        //public string DBCompressionTypeINI
        //{
        //    get => THConfigINI.KeyExists("THOptionDBCompression", "Optimizations")
        //            ? THConfigINI.ReadINI("Optimizations", "THOptionDBCompression")
        //            : "XML (none)";
        //    set => THConfigINI.WriteINI("Optimizations", "THOptionDBCompression", value);
        //}

        //public string WebTransLinkINI
        //{
        //    get => THConfigINI.KeyExists("THOptionWebPageLinkForManualTranslation", "Tools")
        //            ? THConfigINI.ReadINI("Tools", "THOptionWebPageLinkForManualTranslation")
        //            : "https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}";
        //    set => THConfigINI.WriteINI("Tools", "THOptionWebPageLinkForManualTranslation", value);
        //}

        //public bool EnableTranslationCacheINI
        //{
        //    get => THConfigINI.KeyExists("THOptionEnableTranslationCacheCheckBox.Checked", "Tools")
        //            ? bool.Parse(THConfigINI.ReadINI("Tools", "THOptionEnableTranslationCacheCheckBox.Checked"))
        //            : true;
        //    set => THConfigINI.WriteINI("Tools", "THOptionEnableTranslationCacheCheckBox.Checked", value.ToString(CultureInfo.InvariantCulture));
        //}

        //public bool AutotranslationForIdenticalINI
        //{
        //    get => THConfigINI.KeyExists("THOptionAutotranslationForIdenticalCheckBox.Checked", "Tools")
        //            ? bool.Parse(THConfigINI.ReadINI("Tools", "THOptionAutotranslationForIdenticalCheckBox.Checked"))
        //            : Properties.Settings.Default.AutotranslationForSimular;
        //    set => THConfigINI.WriteINI("Tools", "THOptionAutotranslationForIdenticalCheckBox.Checked", value.ToString(CultureInfo.InvariantCulture));
        //}

        //public bool FullComprasionDBloadINI
        //{
        //    get => THConfigINI.KeyExists("THOptionFullComprasionDBload.Checked", "General")
        //            ? bool.Parse(THConfigINI.ReadINI("General", "THOptionFullComprasionDBload.Checked"))
        //            : true;
        //    set => THConfigINI.WriteINI("General", "THOptionFullComprasionDBload.Checked", value.ToString(CultureInfo.InvariantCulture));
        //}

        //public int LineCharLimitINI
        //{
        //    get => THConfigINI.KeyExists("THOptionLineCharLimit", "General")
        //            ? int.Parse(THConfigINI.ReadINI("General", "THOptionLineCharLimit"), CultureInfo.InvariantCulture)
        //            : 60;
        //    set => THConfigINI.WriteINI("General", "THOptionLineCharLimit", value.ToString(CultureInfo.InvariantCulture));
        //}

        private void THOptionDontLoadStringIfRomajiPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            SetValue((sender as TextBox).Name);
            //ValidTHOptionDontLoadStringIfRomajiPercent();
        }

        private void THProgramSettingsForm_Load(object sender, EventArgs e)
        {
            GetSettings();
        }

        private void THOptionDontLoadStringIfRomajiPercentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
            //bool newvalue = THOptionDontLoadStringIfRomajiPercentCheckBox.Checked;
            //Properties.Settings.Default.DontLoadStringIfRomajiPercent = newvalue;
            //DontLoadStringIfRomajiPercentINI = newvalue;
        }

        private void THOptionDBCompressionComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetValue((sender as ComboBox).Name);
            //DBCompressionTypeINI = THOptionDBCompressionExtComboBox.SelectedItem.ToString();
        }

        private void THOptionDBCompressionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
            //DBCompressionINI = THOptionDBCompressionCheckBox.Checked;
        }

        private void THSettingsWebTransLinkTextBox_Validated(object sender, EventArgs e)
        {
            SetValue((sender as TextBox).Name);
            //Properties.Settings.Default.WebTranslationLink = THSettingsWebTranslationLinkTextBox.Text;
            //WebTransLinkINI = THSettingsWebTranslationLinkTextBox.Text;
        }

        private void THOptionEnableTranslationCacheCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        internal Dictionary<string, SettingsBase> SettingsList;
        private void THOptionAutotranslationForIdenticalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
            //Properties.Settings.Default.AutotranslationForSimular = THOptionAutotranslationForSimularCheckBox.Checked;
            //AutotranslationForIdenticalINI = THOptionAutotranslationForSimularCheckBox.Checked;
        }

        private void SetValue(string ID)
        {
            if (SettingsList.ContainsKey(ID))
            {
                SettingsList[ID].Set();
            }
        }

        private void THOptionEnableTranslationCacheCheckBox_Click(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
            //Properties.Settings.Default.IsTranslationCacheEnabled = THOptionEnableTranslationCacheCheckBox.Checked;
            //EnableTranslationCacheINI = THOptionEnableTranslationCacheCheckBox.Checked;
        }

        private void THSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            THToolTip.Dispose();
        }

        private void FullComparasionDBload_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
            //Properties.Settings.Default.IsFullComprasionDBloadEnabled = THOptionFullComprasionDBload.Checked;
            //FullComprasionDBloadINI = THOptionFullComprasionDBload.Checked;
        }

        private void LineCharLimitTextBox_TextChanged(object sender, EventArgs e)
        {
            SetValue((sender as TextBox).Name);
        }

        private void THOptionDontLoadStringIfRomajiPercentForOpenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
        }

        private void THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
        }

        private void THOptionEnableTranslationCacheCheckBox_CheckedChanged_1(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
        }

        private void THSettingsWebTranslationLinkTextBox_TextChanged(object sender, EventArgs e)
        {
            SetValue((sender as TextBox).Name);
        }

        private void THSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsToINI();
        }

        private void SaveSettingsToINI()
        {
            foreach (var setting in SettingsList.Keys)
            {
                bool keyExists;
                if (((keyExists = THConfigINI.KeyExists(SettingsList[setting].Key, SettingsList[setting].Section))
                    && THConfigINI.ReadINI(SettingsList[setting].Section, SettingsList[setting].Key) != SettingsList[setting].Get()) || !keyExists)
                {
                    THConfigINI.WriteINI(SettingsList[setting].Section, SettingsList[setting].Key, SettingsList[setting].Get(), false);
                }
            }
            THConfigINI.SaveINI();
        }

        private void SettingsAutosaveTimeoutValueTextBox_TextChanged(object sender, EventArgs e)
        {
            SetValue((sender as TextBox).Name);
        }

        private void SettingsAutosaveEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            THSettingsWebTranslationLinkTextBox.Text = (sender as ComboBox).SelectedItem.ToString();
        }

        private void llblQuickWebTranslatorSetGoogle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            THSettingsWebTranslationLinkTextBox.Text = (sender as ComboBox).SelectedItem.ToString();
        }

        private void THOptionIgnoreOrigEqualTransLinesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetValue((sender as CheckBox).Name);
        }
    }
}

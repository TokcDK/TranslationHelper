using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.Translators;

namespace TranslationHelper.INISettings
{
    class SSourceLanguage : General
    {
        public SSourceLanguage()
        {
            if (AppData.Settings.SourceLanguageComboBox.Items.Count == 0)
            {
                AppData.Settings.SourceLanguageComboBox.DataSource = TranslatorsBase.Languages;
            }
            AppData.Settings.SourceLanguageComboBox.SelectedIndex = TranslatorsBase.Languages.IndexOf(Properties.Settings.Default.OnlineTranslationSourceLanguage);
        }

        internal override string Key => "SourceLanguage";

        internal override string Default => "Japanese ja";

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                Properties.Settings.Default.OnlineTranslationSourceLanguage = AppData.Settings.SourceLanguageComboBox.SelectedItem.ToString();
                
            }
            else
            {
                Properties.Settings.Default.OnlineTranslationSourceLanguage = AppData.BufferValueString;
                AppData.Settings.SourceLanguageComboBox.SelectedItem = Properties.Settings.Default.OnlineTranslationSourceLanguage;
            }
        }

        internal override string Get()
        {
            return Properties.Settings.Default.OnlineTranslationSourceLanguage;
        }

        internal override string ID()
        {
            return AppData.Settings.SourceLanguageComboBox.Name;
        }
    }
}

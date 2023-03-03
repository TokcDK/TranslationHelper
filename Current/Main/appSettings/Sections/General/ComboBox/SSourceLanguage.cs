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
                AppData.Settings.SourceLanguageComboBox.DataSource = TranslatorsBase.SourceLanguages;
            }
            AppData.Settings.SourceLanguageComboBox.SelectedIndex = TranslatorsBase.SourceLanguages.IndexOf(AppSettings.OnlineTranslationSourceLanguage);
        }

        internal override string Key => "SourceLanguage";

        internal override string Default => "Japanese ja";

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                AppSettings.OnlineTranslationSourceLanguage = AppData.Settings.SourceLanguageComboBox.SelectedItem.ToString();                
            }
            else
            {
                AppSettings.OnlineTranslationSourceLanguage = AppData.BufferValueString;
                AppData.Settings.SourceLanguageComboBox.SelectedItem = AppSettings.OnlineTranslationSourceLanguage;
            }
        }

        internal override string Get()
        {
            return AppSettings.OnlineTranslationSourceLanguage;
        }

        internal override string ID()
        {
            return AppData.Settings.SourceLanguageComboBox.Name;
        }
    }
}

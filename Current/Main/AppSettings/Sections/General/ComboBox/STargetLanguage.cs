using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.Translators;

namespace TranslationHelper.INISettings
{
    class STargetLanguage : General
    {
        public STargetLanguage()
        {
            if (AppData.Settings.TargetLanguageComboBox.Items.Count == 0)
            {
                AppData.Settings.TargetLanguageComboBox.DataSource = TranslatorsBase.TargetLanguages;
            }
            AppData.Settings.TargetLanguageComboBox.SelectedIndex = TranslatorsBase.TargetLanguages.IndexOf(AppSettings.OnlineTranslationSourceLanguage);
        }

        internal override string Key => "TargetLanguage";

        internal override string Default => "English en";

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                AppSettings.OnlineTranslationTargetLanguage = AppData.Settings.TargetLanguageComboBox.SelectedItem.ToString();                
            }
            else
            {
                AppSettings.OnlineTranslationTargetLanguage = AppData.BufferValueString;
                AppData.Settings.TargetLanguageComboBox.SelectedItem = AppSettings.OnlineTranslationTargetLanguage;
            }
        }

        internal override string Get()
        {
            return AppSettings.OnlineTranslationTargetLanguage;
        }

        internal override string ID()
        {
            return AppData.Settings.TargetLanguageComboBox.Name;
        }
    }
}

using System.IO;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;
using TranslationHelper.Forms.Search;
using TranslationHelper.Settings;

namespace TranslationHelper.Data
{
    /// <summary>
    /// Application-wide values.
    /// <para>
    /// Everything under <em>Settings</em> below is a thin forwarder to the one setting object that
    /// owns the value. Settings are declared by the feature they belong to — see
    /// <see cref="TranslationSettings"/>, <see cref="SearchSettings"/>, <see cref="DataSettings"/>
    /// and <see cref="TextSettings"/> — and this class only keeps the long-standing names working.
    /// New code should read the owning setting directly, which makes it obvious which feature the
    /// value belongs to.
    /// </para>
    /// <para>
    /// The rest of the class is runtime state, not settings: it is never written to the INI file.
    /// </para>
    /// </summary>
    public static class AppSettings
    {
        public static bool THdebug { get; set; } = true;
        public static bool DebugMode { get; set; } = true;
        public static bool IsTranslationHelperWasClosed { get; set; } = false;
        public static string ApplicationStartupPath { get; set; }
        public static bool InterruptTtanslation { get; set; } = false;
        public static int THSavedSearchQueriesReplacersCount { get; set; } = 20;
        public static string THSelectedGameDir { get; set; } = "";
        public static string THSelectedDir { get; set; } = "";
        public static string RPGMTransPatchVersion { get; set; } = "3";
        public static string THSelectedSourceType { get; set; } = "";
        public static int THFilesListSelectedIndex { get; set; } = 0;

        private static string _translationCachePath;

        /// <summary>
        /// Computed on first use rather than in a field initialiser: the path depends on the source
        /// language setting, and reading a setting from inside this class's static initialiser would
        /// load every setting before the type was fully built.
        /// </summary>
        public static string THTranslationCachePath
        {
            get => _translationCachePath ?? (_translationCachePath = Path.Combine(THSettings.DBDirPathByLanguage, "THTranslationCache"));
            set => _translationCachePath = value;
        }

        public static string ProjectNewLineSymbol { get; set; } = "\r\n";
        public static string NewLine { get; set; } = "\r\n";
        public static string THProjectWorkDir { get; set; } = "";
        public static bool THAutoSetSameTranslationForSimularIsBusy { get; set; } = false;
        public static int DGVSelectedRowIndex { get; set; } = -1;
        public static int DGVSelectedRowRealIndex { get; set; } = -1;
        public static int DGVSelectedColumnIndex { get; set; } = -1;
        public static bool ProjectIsOpened { get => AppData.CurrentProject != null; }
        public static bool DGVCellInEditMode { get => IsRowInEditMode; }
        public static bool DBTryToCheckLinesOfEachMultilineValue { get; set; } = true;
        public static bool ApplyFixesOnTranslation { get; set; } = true;
        public static bool UseAllDBFilesForOnlineTranslationForAll { get; set; } = true;
        public static string ApplicationProductName { get; set; } = "";
        public static bool IsFileOpened { get => AppData.Main.THFileElementsDataGridView.DataSource != null; }
        public static bool IsFileContentFocused { get => AppData.Main.THFileElementsDataGridView.Focused; }
        public static bool IsEditTextBoxFocused { get => AppData.Main.THTargetRichTextBox.Focused; }
        public static bool IsRowInEditMode { get => (IsFileContentFocused && AppData.Main.THFileElementsDataGridView.IsCurrentCellInEditMode) || IsEditTextBoxFocused; }
        public static bool IsFilesListFocused { get => AppData.FilesListControl.Focused; }
        public static bool IsParseRow { get => IsFileOpened && IsFileContentFocused; }
        public static bool IsParseFile { get => !IsFileContentFocused && IsFilesListFocused && AppData.THFilesList.SelectedItems.Count > 0; }
        public static bool IsParseAllFiles { get => !IsFileContentFocused && IsFilesListFocused && AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count; }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Settings. Every one of these is stored in, and owned by, the setting it forwards to.
        ////////////////////////////////////////////////////////////////////////////////////////////

        public static string OnlineTranslationSourceLanguage
        {
            get => SettingsRegistry.Get<TranslationSettings.SourceLanguage>().Value;
            set => SettingsRegistry.Get<TranslationSettings.SourceLanguage>().Value = value;
        }

        public static string OnlineTranslationTargetLanguage
        {
            get => SettingsRegistry.Get<TranslationSettings.TargetLanguage>().Value;
            set => SettingsRegistry.Get<TranslationSettings.TargetLanguage>().Value = value;
        }

        public static string WebTranslationLink
        {
            get => SettingsRegistry.Get<TranslationSettings.WebTranslationLink>().Value;
            set => SettingsRegistry.Get<TranslationSettings.WebTranslationLink>().Value = value;
        }

        public static bool EnableTranslationCache
        {
            get => SettingsRegistry.Get<TranslationSettings.EnableTranslationCache>().Value;
            set => SettingsRegistry.Get<TranslationSettings.EnableTranslationCache>().Value = value;
        }

        public static bool AutotranslationForSimular
        {
            get => SettingsRegistry.Get<TranslationSettings.AutotranslationForSimular>().Value;
            set => SettingsRegistry.Get<TranslationSettings.AutotranslationForSimular>().Value = value;
        }

        public static bool IgnoreOrigEqualTransLines
        {
            get => SettingsRegistry.Get<TranslationSettings.IgnoreOrigEqualTransLines>().Value;
            set => SettingsRegistry.Get<TranslationSettings.IgnoreOrigEqualTransLines>().Value = value;
        }

        public static bool DBCompression
        {
            get => SettingsRegistry.Get<DataSettings.DBCompression>().Value;
            set => SettingsRegistry.Get<DataSettings.DBCompression>().Value = value;
        }

        public static string DBCompressionExt
        {
            get => SettingsRegistry.Get<DataSettings.DBCompressionExt>().Value;
            set => SettingsRegistry.Get<DataSettings.DBCompressionExt>().Value = value;
        }

        public static bool DontLoadDuplicates
        {
            get => SettingsRegistry.Get<DataSettings.DontLoadDuplicates>().Value;
            set => SettingsRegistry.Get<DataSettings.DontLoadDuplicates>().Value = value;
        }

        public static bool IsFullComprasionDBloadEnabled
        {
            get => SettingsRegistry.Get<DataSettings.FullComprasionDBload>().Value;
            set => SettingsRegistry.Get<DataSettings.FullComprasionDBload>().Value = value;
        }

        public static bool EnableDBAutosave
        {
            get => SettingsRegistry.Get<DataSettings.EnableDBAutosave>().Value;
            set => SettingsRegistry.Get<DataSettings.EnableDBAutosave>().Value = value;
        }

        public static int DBAutoSaveTimeout
        {
            get => SettingsRegistry.Get<DataSettings.DBAutosaveTimeout>().Value;
            set => SettingsRegistry.Get<DataSettings.DBAutosaveTimeout>().Value = value;
        }

        public static bool SearchRowIssueOptionsCheckNonRomaji
        {
            get => SettingsRegistry.Get<SearchSettings.CheckNonRomaji>().Value;
            set => SettingsRegistry.Get<SearchSettings.CheckNonRomaji>().Value = value;
        }

        public static bool SearchRowIssueOptionsCheckActors
        {
            get => SettingsRegistry.Get<SearchSettings.CheckActors>().Value;
            set => SettingsRegistry.Get<SearchSettings.CheckActors>().Value = value;
        }

        public static bool SearchRowIssueOptionsCheckAnyLineTranslatable
        {
            get => SettingsRegistry.Get<SearchSettings.CheckAnyLineTranslatable>().Value;
            set => SettingsRegistry.Get<SearchSettings.CheckAnyLineTranslatable>().Value = value;
        }

        public static bool SearchRowIssueOptionsCheckProjectSpecific
        {
            get => SettingsRegistry.Get<SearchSettings.CheckProjectSpecific>().Value;
            set => SettingsRegistry.Get<SearchSettings.CheckProjectSpecific>().Value = value;
        }

        public static int THOptionLineCharLimit
        {
            get => SettingsRegistry.Get<TextSettings.LineCharLimit>().Value;
            set => SettingsRegistry.Get<TextSettings.LineCharLimit>().Value = value;
        }

        public static bool DontLoadStringIfRomajiPercent
        {
            get => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercent>().Value;
            set => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercent>().Value = value;
        }

        public static int DontLoadStringIfRomajiPercentNumber
        {
            get => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercentNumber>().Value;
            set => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercentNumber>().Value = value;
        }

        public static bool DontLoadStringIfRomajiPercentForOpen
        {
            get => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercentForOpen>().Value;
            set => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercentForOpen>().Value = value;
        }

        public static bool DontLoadStringIfRomajiPercentForTranslation
        {
            get => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercentForTranslation>().Value;
            set => SettingsRegistry.Get<TextSettings.DontLoadStringIfRomajiPercentForTranslation>().Value = value;
        }

        public static bool IsJapaneseSourceLanguage { get => OnlineTranslationSourceLanguage.Equals("Japanese ja"); }
    }
}

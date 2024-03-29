﻿using System.IO;

namespace TranslationHelper.Data
{
    public static class AppSettings
    {
        public static bool THdebug { get; set; } = true;
        public static bool DebugMode { get; set; } = true;
        public static bool IsTranslationHelperWasClosed { get; set; } = false;
        public static string ApplicationStartupPath { get; set; }
        public static bool AutotranslationForSimular { get; set; } = true;
        public static bool IsFullComprasionDBloadEnabled { get; set; } = false;
        public static bool DontLoadStringIfRomajiPercentForOpen { get; set; } = true;
        public static bool DontLoadStringIfRomajiPercentForTranslation { get; set; } = true;
        public static bool DontLoadStringIfRomajiPercent { get; set; } = false;
        public static int DontLoadStringIfRomajiPercentNumber { get; set; } = 90;
        public static string WebTranslationLink { get; set; } = "https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}";
        public static string OnlineTranslationSourceLanguage { get; set; } = "Japanese ja";
        public static string OnlineTranslationTargetLanguage { get; set; } = "English en";
        public static bool InterruptTtanslation { get; set; } = false;
        public static int THOptionLineCharLimit { get; set; } = 60;
        public static int THSavedSearchQueriesReplacersCount { get; set; } = 20;
        public static string THSelectedGameDir { get; set; } = "";
        public static string THSelectedDir { get; set; } = "";
        public static string RPGMTransPatchVersion { get; set; } = "3";
        public static string THSelectedSourceType { get; set; } = "";
        public static int THFilesListSelectedIndex { get; set; } = 0;
        public static string THTranslationCachePath { get; set; } = Path.Combine(THSettings.DBDirPathByLanguage, "THTranslationCache");
        public static bool EnableTranslationCache { get; set; } = true;
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
        public static bool DBCompression { get; set; } = true;
        public static string DBCompressionExt { get; set; } = "XML (none)";
        public static int DBAutoSaveTimeout { get; set; } = 60;
        public static bool EnableDBAutosave { get; set; } = true;
        public static string ApplicationProductName { get; set; } = "";
        public static bool IgnoreOrigEqualTransLines { get; set; } = true;
        public static bool DontLoadDuplicates { get; set; } = true;
        public static bool SearchRowIssueOptionsCheckNonRomaji { get; set; } = true;
        public static bool SearchRowIssueOptionsCheckActors { get; set; } = true;
        public static bool SearchRowIssueOptionsCheckAnyLineTranslatable { get; set; } = true;
        public static bool SearchRowIssueOptionsCheckProjectSpecific { get; set; } = true;
        public static bool IsFileOpened { get => AppData.Main.THFileElementsDataGridView.DataSource != null; }
        public static bool IsFileContentFocused { get => AppData.Main.THFileElementsDataGridView.Focused; }
        public static bool IsEditTextBoxFocused { get => AppData.Main.THTargetRichTextBox.Focused; }
        public static bool IsRowInEditMode { get => (IsFileContentFocused && AppData.Main.THFileElementsDataGridView.IsCurrentCellInEditMode) || IsEditTextBoxFocused; }
        public static bool IsFilesListFocused { get => AppData.FilesListControl.Focused; }
        public static bool IsParseRow { get => IsFileOpened && IsFileContentFocused; }
        public static bool IsParseFile { get => !IsFileContentFocused && IsFilesListFocused && AppData.THFilesList.SelectedItems.Count > 0; }
        public static bool IsParseAllFiles { get => !IsFileContentFocused && IsFilesListFocused && AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count; }
        public static bool IsJapaneseSourceLanguage { get => OnlineTranslationSourceLanguage.Equals("Japanese ja"); }
    }
}

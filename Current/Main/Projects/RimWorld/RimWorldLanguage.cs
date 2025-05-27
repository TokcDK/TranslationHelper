using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Formats.RimWorld;
using TranslationHelper.Formats.zzzOtherFormat;
using TranslationHelper.Menus.MainMenus;
using TranslationHelper.Projects.RPGMMV.Menus;

namespace TranslationHelper.Projects.RimWorld
{
    internal class RimWorldLanguage : ProjectBase
    {
        public RimWorldLanguage()
        {
            HideVarsBase = new Dictionary<string, string>
            {
                { "[", @"\[[a-zA-Z0-9-_]+\]" }
            };
        }

        public override string Name => "RimWorld mod language dir";

        protected override bool TryOpen() => OpenSave();

        protected override bool TrySave() => OpenSave();

        /// <summary>
        /// Opens or saves language files within the RimWorld mod's language directory structure.
        /// Processes all relevant files in "Strings", "DefInjected", and "Keyed" directories.
        /// </summary>
        /// <returns>True if any files were successfully processed; otherwise, false.</returns>
        private bool OpenSave()
        {
            string languageDirPath = GetLanguageDirPath();
            if (languageDirPath == null) return false;

            bool ret = false;

            using (new SettingsScope())
            {
                // Process "Strings" directory for .txt files
                string stringsPath = Path.Combine(languageDirPath, "Strings");
                if (Directory.Exists(stringsPath))
                {
                    if (ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(stringsPath),
                        typeof(TXTStringPerLine), "*.txt", searchOption: SearchOption.AllDirectories))
                    {
                        ret = true;
                    }
                }

                // Process "DefInjected" directory for .xml files
                string defInjectedPath = Path.Combine(languageDirPath, "DefInjected");
                if (Directory.Exists(defInjectedPath))
                {
                    if (ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(defInjectedPath),
                        typeof(RimWorldLanguageDataXML), "*.xml", searchOption: SearchOption.AllDirectories))
                    {
                        ret = true;
                    }
                }

                // Process "Keyed" directory for .xml files
                string keyedPath = Path.Combine(languageDirPath, "Keyed");
                if (Directory.Exists(keyedPath))
                {
                    if (ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(keyedPath),
                        typeof(RimWorldLanguageDataXML), "*.xml", searchOption: SearchOption.AllDirectories))
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        internal override bool IsValid() => GetLanguageDirPath() != null;

        /// <summary>
        /// Determines the language directory path if the selected file is within a valid RimWorld language structure.
        /// Expected structure: Languages/<language>/(DefInjected|Keyed|Strings)/...
        /// </summary>
        /// <returns>The language directory path (e.g., "Languages/English") or null if invalid.</returns>
        private string GetLanguageDirPath()
        {
            string filePath = Data.AppData.SelectedProjectFilePath;
            if (string.IsNullOrWhiteSpace(filePath)) return null;

            string currentPath = Path.GetFullPath(filePath);
            string languagesDir = null;

            // Locate the "Languages" directory by traversing up the path
            while (true)
            {
                string dirName = Path.GetFileName(currentPath);
                if (string.Equals(dirName, "Languages", StringComparison.InvariantCultureIgnoreCase))
                {
                    languagesDir = currentPath;
                    break;
                }
                string parent = Path.GetDirectoryName(currentPath);
                if (parent == null || parent == currentPath) break;
                currentPath = parent;
            }

            if (languagesDir == null) return null;

            // Find the language directory (e.g., "English") under "Languages"
            string langDir = Path.GetDirectoryName(filePath);
            while (langDir != null && !string.Equals(Path.GetDirectoryName(langDir), languagesDir, StringComparison.InvariantCultureIgnoreCase))
            {
                langDir = Path.GetDirectoryName(langDir);
            }

            if (langDir == null) return null;

            // Verify the file is in a valid subdirectory with the correct extension
            string subPath = filePath.Substring(langDir.Length + 1); // +1 for the directory separator
            string[] parts = subPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 1)
            {
                string subDir = parts[0];
                string fileName = parts[parts.Length - 1];
                string ext = Path.GetExtension(fileName)?.ToLowerInvariant();

                if ((subDir == "DefInjected" && parts.Length >= 3 && ext == ".xml") || // At least 3 for DefInjected/<type>/<file>.xml
                    (subDir == "Keyed" && parts.Length == 2 && ext == ".xml") ||      // Exactly 2 for Keyed/<file>.xml
                    (subDir == "Strings" && ext == ".txt"))                           // Strings can have nested dirs
                {
                    return langDir;
                }
            }

            return null;
        }

        /// <summary>
        /// Manages temporary changes to application settings during file processing.
        /// </summary>
        private class SettingsScope : IDisposable
        {
            private readonly bool originalDontLoadDups;
            private readonly bool originalDontLoadStringIfRomajiPercent;

            public SettingsScope()
            {
                originalDontLoadDups = Data.AppData.Settings.THOptionDontLoadDuplicates.Checked;
                originalDontLoadStringIfRomajiPercent = Data.AppData.Settings.THOptionDontLoadStringIfRomajiPercentCheckBox.Checked;
                Data.AppData.Settings.THOptionDontLoadDuplicates.Checked = true;
                Data.AppData.Settings.THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = false;
            }

            public void Dispose()
            {
                Data.AppData.Settings.THOptionDontLoadDuplicates.Checked = originalDontLoadDups;
                Data.AppData.Settings.THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = originalDontLoadStringIfRomajiPercent;
            }
        }

        internal override IMainMenuItem[] MainMenuItemMenusList => new IMainMenuItem[] { new MainMenuTryImportStringsFrom() };
    }
}

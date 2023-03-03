using System;
using System.IO;
using TranslationHelper.Formats.RimWorld;
using TranslationHelper.Formats.zzzOtherFormat;

namespace TranslationHelper.Projects.RimWorld
{
    internal class RimWorldLanguage : ProjectBase
    {
        public override string Name => "RimWorld mod language dir";

        public override bool Open()
        {
            return OpenSave();
        }

        public override bool Save()
        {
            return OpenSave();
        }

        bool OpenSave()
        {
            // change settings
            bool defaultDontLoadDups = Data.AppData.Settings.THOptionDontLoadDuplicates.Checked;
            Data.AppData.Settings.THOptionDontLoadDuplicates.Checked = true;
            bool defaultDontLoadStringIfRomajiPercent = Data.AppData.Settings.THOptionDontLoadStringIfRomajiPercentCheckBox.Checked;
            Data.AppData.Settings.THOptionDontLoadDuplicates.Checked = false;

            var variant = GetVariant();
            if (string.IsNullOrWhiteSpace(variant)) { return false; }

            string defsDirPath = Path.GetDirectoryName(variant == "Keyed" ? Data.AppData.SelectedFilePath : Path.GetDirectoryName(Data.AppData.SelectedFilePath));

            var languageDirPath = Path.GetDirectoryName(defsDirPath);

            bool ret = false;
            var path = Path.Combine(languageDirPath, "Strings");

            if (Directory.Exists(path) &&
                ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(path), typeof(TXTStringPerLine), "*.txt", searchOption: SearchOption.AllDirectories))
            {
                ret = true;
            }

            foreach (var dirName in new[] { "DefInjected", "Keyed" })
            {
                path = Path.Combine(languageDirPath, dirName);

                if (!Directory.Exists(path)) continue;

                if (ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(path), typeof(RimWorldLanguageDataXML), "*.xml", searchOption: SearchOption.AllDirectories))
                {
                    ret = true;
                }
            }

            // restore settings
            Data.AppData.Settings.THOptionDontLoadDuplicates.Checked = defaultDontLoadDups;
            Data.AppData.Settings.THOptionDontLoadDuplicates.Checked = defaultDontLoadStringIfRomajiPercent;

            return ret;
        }

        internal override bool IsValid()
        {
            if (GetVariant() == null) return false; return true;
        }

        private string GetVariant()
        {
            // make sure it is file in Languages dir
            if (string.IsNullOrWhiteSpace(Data.AppData.SelectedFilePath)) return null;
            var ext = Path.GetExtension(Data.AppData.SelectedFilePath);
            if (string.IsNullOrWhiteSpace(ext)) return null;

            string defsDirPath;
            try
            {
                defsDirPath = Path.GetDirectoryName(Path.GetDirectoryName(Data.AppData.SelectedFilePath));
            }
            catch { return null; }

            if (string.IsNullOrWhiteSpace(defsDirPath)) return null;

            string variantName = null;

            // check if Defs xml
            if (string.Equals(ext, ".xml"))
            {
                if (!string.Equals(Path.GetFileName(defsDirPath), "DefInjected", StringComparison.InvariantCultureIgnoreCase))
                {
                    defsDirPath = Path.GetDirectoryName(Data.AppData.SelectedFilePath);
                    if (!string.Equals(Path.GetFileName(defsDirPath),"Keyed", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return null;
                    }

                    variantName = "Keyed";
                }

                variantName = "DefInjected";
            }
            else if (string.Equals(ext, ".txt"))
            {
                // check if Strings
                if (!string.Equals(Path.GetFileName(defsDirPath),"Strings", StringComparison.InvariantCultureIgnoreCase)) 
                    return null;

                variantName = "Strings";
            }

            // check if Languages dir is exists
            if (!string.Equals(
                Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(defsDirPath))),
                "Languages", StringComparison.InvariantCultureIgnoreCase)) return null;

            return variantName;
        }
    }
}

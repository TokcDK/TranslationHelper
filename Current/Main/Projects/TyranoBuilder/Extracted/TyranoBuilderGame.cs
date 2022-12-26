using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.TyranoBuilder.Extracted;

namespace TranslationHelper.Projects.TyranoBuilder.Extracted
{
    class TyranoBuilderGame : ProjectBase
    {
        public TyranoBuilderGame()
        {
        }

        internal override bool IsValid()
        {
            return Path.GetFileName(AppData.SelectedFilePath) == "index.html" && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "data", "scenario", "config.ks"));
        }

        public override string Name => "TyranoBuilder";

        public override bool Open()
        {
            var export = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "export"));

            if (export.Exists && export.HasAnyFiles("*.csv"))
            {
                return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "export"), typeof(ExportedCSV), "*.csv");
                //var result = MessageBox.Show(T._("Project has exported csv by TyranoBuilder translator") + ". " + T._("Proceed exported?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //if (result == DialogResult.Yes)
                //{
                //}
            }
            else
            {
                return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "data", "scenario"), typeof(KS), "*.ks");
            }
        }

        public override bool Save()
        {
            var export = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "export"));

            if (export.Exists && export.HasAnyFiles("*.csv"))
            {
                return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "export"), typeof(ExportedCSV), "*.csv");
                //var result = MessageBox.Show(T._("Project has exported csv by TyranoBuilder translator") + ". " + T._("Proceed exported?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //if (result == DialogResult.Yes)
                //{
                //}
            }
            else
            {
                return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "data", "scenario"), typeof(KS), "*.ks");
            }
        }

        public override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(new[] { Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "data", "scenario") });
        }

        public override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(new[] { Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "data", "scenario") }, false);
        }
    }
}

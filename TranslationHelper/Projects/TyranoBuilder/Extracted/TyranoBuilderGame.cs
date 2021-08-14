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

        internal override bool Check()
        {
            return Path.GetFileName(ProjectData.SelectedFilePath) == "index.html" && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "data", "scenario", "config.ks"));
        }

        internal override string Name()
        {
            return "TyranoBuilder";
        }

        internal override bool Open()
        {
            var export = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "export"));

            if (export.Exists && export.HasAnyFiles("*.csv"))
            {
                return OpenSaveFilesBase(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "export"), new ExportedCSV(), "*.csv");
                //var result = MessageBox.Show(T._("Project has exported csv by TyranoBuilder translator") + ". " + T._("Proceed exported?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //if (result == DialogResult.Yes)
                //{
                //}
            }
            else
            {
                return OpenSaveFilesBase(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "data", "scenario"), new KS(), "*.ks");
            }
        }

        internal override bool Save()
        {
            var export = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "export"));

            if (export.Exists && export.HasAnyFiles("*.csv"))
            {
                return OpenSaveFilesBase(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "export"), new ExportedCSV(), "*.csv");
                //var result = MessageBox.Show(T._("Project has exported csv by TyranoBuilder translator") + ". " + T._("Proceed exported?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //if (result == DialogResult.Yes)
                //{
                //}
            }
            else
            {
                return OpenSaveFilesBase(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "data", "scenario"), new KS(), "*.ks");
            }
        }

        internal override bool BakCreate()
        {
            return BackupRestorePaths(new[] { Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "data", "scenario") });
        }

        internal override bool BakRestore()
        {
            return BackupRestorePaths(new[] { Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "data", "scenario") }, false);
        }
    }
}

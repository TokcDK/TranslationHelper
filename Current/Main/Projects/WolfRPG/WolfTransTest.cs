using System.IO;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG.WolfTransCSharp;

namespace TranslationHelper.Projects.WolfRPG
{
    internal class WolfTransTest : WolfRPGBase
    {
        public override string Name => "Wolftrans new";

        public override bool Open() => OpenSave();
        public override bool Save() => OpenSave();

        private bool OpenSave()
        {
            ExtractWolfFiles();
            bool[] b = new bool[] { 
                ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "MapData"), typeof(MPS), "*.mps")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "BasicData")), typeof(Database), "*.project", exclusions: new string[] { "SysDataBaseBasic.project" })
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "BasicData"), typeof(CommonEvents), "CommonEvent.dat")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext"), typeof(Formats.WolfRPG.EvTextTXT), "*.txt")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "TextE"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "TextH"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "TextP"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                };
            return b.Any(p => p == true);

        }


        internal override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "Evtext") });
        }

        internal override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "Evtext") });
        }

        //bool bakrestore()
        //{
        //    bool[] b = new bool[3] {
        //          BackupRestorePaths(new[] { Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "Data", "Evtext") })
        //        , BackupRestorePaths(new[] { Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "Data", "MapData") })
        //        , BackupRestorePaths(new[] { Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "Data", "BasicData") })
        //    };

        //    return b.All(r => r == true);
        //}
    }
}

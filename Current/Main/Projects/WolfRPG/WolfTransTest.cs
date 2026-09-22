using System.IO;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG.WolfTransCSharp;

namespace TranslationHelper.Projects.WolfRPG
{
    internal class WolfTransTest : WolfRPGBase
    {
        public override string Name => "Wolftrans new";

        protected override bool TryOpen() => OpenSave();
        protected override bool TrySave() => OpenSave();

        private bool OpenSave()
        {
            ExtractWolfFiles();
            bool[] b = new bool[] { 
                ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(SelectedGameDir, "Data", "MapData"), typeof(MPS), "*.mps")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(Path.Combine(SelectedGameDir, "Data", "BasicData")), typeof(Database), "*.project", exclusions: new string[] { "SysDataBaseBasic.project" })
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(SelectedGameDir, "Data", "BasicData"), typeof(CommonEvents), "CommonEvent.dat")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(SelectedGameDir, "data", "Evtext"), typeof(Formats.WolfRPG.EvTextTXT), "*.txt")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(SelectedGameDir, "data", "TextE"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(SelectedGameDir, "data", "TextH"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                ,ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(SelectedGameDir, "data", "TextP"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                };
            return b.Any(p => p == true);

        }


        public override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(this, new[] { Path.Combine(SelectedGameDir, "Data", "Evtext") });
        }

        public override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(this, new[] { Path.Combine(SelectedGameDir, "Data", "Evtext") });
        }

        //bool bakrestore()
        //{
        //    bool[] b = new bool[3] {
        //          BackupRestorePaths(this, new[] { Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "Data", "Evtext") })
        //        , BackupRestorePaths(this, new[] { Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "Data", "MapData") })
        //        , BackupRestorePaths(this, new[] { Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "Data", "BasicData") })
        //    };

        //    return b.All(r => r == true);
        //}
    }
}

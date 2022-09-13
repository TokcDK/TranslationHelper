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
                OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "MapData"), typeof(MPS), "*.mps")
                ,OpenSaveFilesBase(new DirectoryInfo(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "BasicData")), typeof(Database), "*.project", exclusions: new string[] { "SysDataBaseBasic.project" })
                ,OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "BasicData"), typeof(CommonEvents), "CommonEvent.dat")
                ,OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext"), typeof(Formats.WolfRPG.EvTextTXT), "*.txt")
                ,OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "TextE"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                ,OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "TextH"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                ,OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "TextP"), typeof(Formats.WolfRPG.TextEPH), "*.txt")
                };
            return b.Any(p => p == true);

        }


        internal override bool BakCreate()
        {
            return BackupRestorePaths(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "Evtext") });
        }

        internal override bool BakRestore()
        {
            return BackupRestorePaths(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "Evtext") });
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

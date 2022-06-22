using System.IO;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG.WolfTransCSharp;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.WolfRPG
{
    internal class WolfTransTest : WolfRPGBase
    {
        internal override bool Check()
        {
            string d;
            return Path.GetExtension(AppData.SelectedFilePath) == ".exe"
                && (FunctionsFileFolder.IsInDirExistsAnyFile(d = Path.GetDirectoryName(AppData.SelectedFilePath), "*.wolf", recursive: true)
                || (Directory.Exists(d = Path.Combine(d, "Data")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.wolf", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "MapData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.mps", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "BasicData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.dat", recursive: true))
                );
        }

        internal override string Name => "Wolftrans open test";

        public override bool Open() => OpenSave();
        public override bool Save() => OpenSave();

        private bool OpenSave()
        {
            bool[] b = new bool[] { OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "MapData"), typeof(MPS), "*.mps")
                ,
                OpenSaveFilesBase(new DirectoryInfo(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "BasicData")), typeof(Database), "*.project", exclusions: new string[] { "SysDataBaseBasic.project" })
                ,
                OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data", "BasicData"), typeof(CommonEvents), "CommonEvent.dat")
                ,
                OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext"), typeof(Formats.WolfRPG.EvTextTXT), "*.txt")
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

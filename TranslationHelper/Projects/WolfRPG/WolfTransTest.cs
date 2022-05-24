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
            return Path.GetExtension(ProjectData.SelectedFilePath) == ".exe"
                && (FunctionsFileFolder.IsInDirExistsAnyFile(d = Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.wolf", recursive: true)
                || (Directory.Exists(d = Path.Combine(d, "Data")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.wolf", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "MapData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.mps", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "BasicData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.dat", recursive: true))
                );
        }

        internal override string Name() => "Wolftrans open test";

        internal override bool Open() => OpenSave();
        internal override bool Save() => OpenSave();

        private bool OpenSave()
        {
            bool[] b = new bool[] { OpenSaveFilesBase(Path.Combine(ProjectData.SelectedGameDir, "Data", "MapData"), typeof(MPS), "*.mps")
                ,
                OpenSaveFilesBase(new DirectoryInfo(Path.Combine(ProjectData.SelectedGameDir, "Data", "BasicData")), typeof(Database), "*.project", exclusions: new string[] { "SysDataBaseBasic.project" })
                ,
                OpenSaveFilesBase(Path.Combine(ProjectData.SelectedGameDir, "Data", "BasicData"), typeof(CommonEvents), "CommonEvent.dat")
                };
            return b.Any(p => p == true);

        }
    }
}

using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.EAGLS
{
    class SCPACKpak : EAGLSBase
    {
        public SCPACKpak()
        {
        }

        bool ISpak;
        internal override bool Check()
        {
            return (ISpak = Path.GetFileName(AppData.SelectedFilePath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "SCPACK.idx")) || (Path.GetFileName(AppData.SelectedFilePath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "SCPACK.pak"));
        }

        internal override string Filters => "EAGLS SCPACK|SCPACK.pak;SCPACK.idx";

        public override bool Open()
        {
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
            ProjectName = "SCPACK" + (ISpak ? "pak" : "idx") + "_" + AppData.SelectedFilePath.GetCrc32();
            ScriptDir = AppData.CurrentProject.SelectedGameDir;
            return PackUnpackFiles() && OpenFiles();
        }

        public override string Name => ProjectTitlePrefix+ Path.GetFileName(Path.GetDirectoryName(AppData.SelectedFilePath));

        public override bool Save()
        {
            BakCreate();
            if (SaveFiles())
            {
                return PackFiles();
            }
            return false;
        }

        private bool PackFiles()
        {
            return PackUnpackFiles();
        }
    }
}

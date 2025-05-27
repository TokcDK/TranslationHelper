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
        internal override bool IsValid()
        {
            return (ISpak = Path.GetFileName(AppData.SelectedProjectFilePath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "SCPACK.idx")) || (Path.GetFileName(AppData.SelectedProjectFilePath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "SCPACK.pak"));
        }

        internal override string FileFilter => "EAGLS SCPACK|SCPACK.pak;SCPACK.idx";

        protected override bool TryOpen()
        {
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
            ProjectName = "SCPACK" + (ISpak ? "pak" : "idx") + "_" + AppData.SelectedProjectFilePath.GetCrc32();
            ScriptDir = AppData.CurrentProject.SelectedGameDir;
            return PackUnpackFiles() && OpenFiles();
        }

        public override string Name => ProjectTitlePrefix+ Path.GetFileName(Path.GetDirectoryName(AppData.SelectedProjectFilePath));

        protected override bool TrySaveProject()
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

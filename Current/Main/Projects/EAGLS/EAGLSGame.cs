using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.EAGLS
{
    class EAGLSGame : EAGLSBase
    {
        public EAGLSGame()
        {
        }

        internal override bool IsValid()
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "Script", "SCPACK.pak"))
                && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "Script", "SCPACK.idx"));
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        protected override bool TryOpen()
        {
            BakRestore();
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
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

using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;

namespace TranslationHelper.Projects.EAGLS
{
    class EAGLSGame : EAGLSBase
    {
        public EAGLSGame(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "Script", "SCPACK.pak"))
                && File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "Script", "SCPACK.idx"));
        }

        internal override string Filters()
        {
            return GameExeFilter;
        }

        internal override bool Open()
        {
            SCPACKpak = Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "Script", "SCPACK.pak");
            SCPACKidx = Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "Script", "SCPACK.idx");
            BakRestore();
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
            ProjectName = Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath));
            ScriptDir = Path.Combine(Properties.Settings.Default.THSelectedGameDir, "Script");
            return PackUnpackFiles(false) && OpenFiles();
        }

        internal override string Name()
        {
            return ProjectTitlePrefix() + Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath));
        }

        internal override bool Save()
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

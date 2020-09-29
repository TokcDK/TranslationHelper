using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.EAGLS
{
    class SCPACKpak : EAGLSBase
    {
        public SCPACKpak(THDataWork thDataWork) : base(thDataWork)
        {
        }

        bool ISpak;
        internal override bool Check()
        {
            return (ISpak = Path.GetFileName(thDataWork.SPath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "SCPACK.idx")) || (Path.GetFileName(thDataWork.SPath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "SCPACK.pak"));
        }

        internal override string Filters()
        {
            return "EAGLS SCPACK|SCPACK.pak;SCPACK.idx";
        }

        internal override bool Open()
        {
            SCPACKpak = Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "SCPACK.pak");
            SCPACKidx = Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "SCPACK.idx");
            BakRestore();
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
            ProjectName = "SCPACK" + (ISpak ? "pak" : "idx") + "_" + thDataWork.SPath.GetCrc32();
            ScriptDir = Properties.Settings.Default.THSelectedGameDir;
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

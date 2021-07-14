using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.EAGLS
{
    class SCPACKpak : EAGLSBase
    {
        public SCPACKpak() : base()
        {
        }

        bool ISpak;
        internal override bool Check()
        {
            return (ISpak = Path.GetFileName(ProjectData.SPath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "SCPACK.idx")) || (Path.GetFileName(ProjectData.SPath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "SCPACK.pak"));
        }

        internal override string Filters()
        {
            return "EAGLS SCPACK|SCPACK.pak;SCPACK.idx";
        }

        internal override bool Open()
        {
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
            ProjectName = "SCPACK" + (ISpak ? "pak" : "idx") + "_" + ProjectData.SPath.GetCrc32();
            ScriptDir = ProjectData.SelectedGameDir;
            return PackUnpackFiles() && OpenFiles();
        }

        internal override string Name()
        {
            return ProjectTitlePrefix() + Path.GetFileName(Path.GetDirectoryName(ProjectData.SPath));
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

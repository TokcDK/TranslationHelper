using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.EAGLS
{
    class SCPACKpak : EAGLSBase
    {
        public SCPACKpak(ProjectData projectData) : base(projectData)
        {
        }

        bool ISpak;
        internal override bool Check()
        {
            return (ISpak = Path.GetFileName(projectData.SPath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(projectData.SPath), "SCPACK.idx")) || (Path.GetFileName(projectData.SPath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(projectData.SPath), "SCPACK.pak"));
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
            ProjectName = "SCPACK" + (ISpak ? "pak" : "idx") + "_" + projectData.SPath.GetCrc32();
            ScriptDir = Properties.Settings.Default.THSelectedGameDir;
            return PackUnpackFiles() && OpenFiles();
        }

        internal override string Name()
        {
            return ProjectTitlePrefix() + Path.GetFileName(Path.GetDirectoryName(projectData.SPath));
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

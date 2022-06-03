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
            return (ISpak = Path.GetFileName(ProjectData.SelectedFilePath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "SCPACK.idx")) || (Path.GetFileName(ProjectData.SelectedFilePath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "SCPACK.pak"));
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
            ProjectName = "SCPACK" + (ISpak ? "pak" : "idx") + "_" + ProjectData.SelectedFilePath.GetCrc32();
            ScriptDir = ProjectData.CurrentProject.SelectedGameDir;
            return PackUnpackFiles() && OpenFiles();
        }

        internal override string Name()
        {
            return ProjectTitlePrefix() + Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath));
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

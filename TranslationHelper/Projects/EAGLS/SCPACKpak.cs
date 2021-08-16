using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.EAGLS
{
    class ScpacKpak : EaglsBase
    {
        public ScpacKpak()
        {
        }

        bool _spak;
        internal override bool Check()
        {
            return (_spak = Path.GetFileName(ProjectData.SelectedFilePath) == "SCPACK.pak") && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "SCPACK.idx")) || (Path.GetFileName(ProjectData.SelectedFilePath) == "SCPACK.idx") && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "SCPACK.pak"));
        }

        internal override string Filters()
        {
            return "EAGLS SCPACK|SCPACK.pak;SCPACK.idx";
        }

        internal override bool Open()
        {
            return UnpackScpack();
        }

        private bool UnpackScpack()
        {
            ProjectName = "SCPACK" + (_spak ? "pak" : "idx") + "_" + ProjectData.SelectedFilePath.GetCrc32();
            ScriptDir = ProjectData.SelectedGameDir;
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

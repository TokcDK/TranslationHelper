using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Projects
{
    class RpgmTransPatch : ProjectBase
    {
        public int RpgmTransPatchVersion { get; private set; }

        internal override bool Check()
        {
            if (Path.GetFileName(ProjectData.SelectedFilePath) == "RPGMKTRANSPATCH")
            {
                return true;
            }

            return false;
        }

        internal override string Name()
        {
            return "RPG Maker Trans Patch";
        }

        internal override string ProjectFolderName()
        {
            return "RPGMakerTrans";
        }

        internal override bool Open()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(ProjectData.SelectedFilePath), typeof(Txt), "*.txt");
        }

        internal override bool Save()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(ProjectData.SelectedFilePath), typeof(Txt), "*.txt");
        }
    }
}

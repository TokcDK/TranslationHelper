using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        public RPGMTransPatch(ProjectData thData) : base(thData)
        {
        }

        internal override bool Check()
        {
            if (Path.GetFileName(projectData.SPath) == "RPGMKTRANSPATCH")
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
            return OpenSaveFilesBase(Path.GetDirectoryName(projectData.SPath), new TXT(projectData), "*.txt");
        }

        internal override bool Save()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(projectData.SPath), new TXT(projectData), "*.txt");
        }
    }
}

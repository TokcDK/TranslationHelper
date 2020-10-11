using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        public RPGMTransPatch(THDataWork thData) : base(thData)
        {
        }

        internal override bool Check()
        {
            if (Path.GetFileName(thDataWork.SPath) == "RPGMKTRANSPATCH")
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
            return "RPGMakerTransPatch";
        }

        internal override bool Open()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(thDataWork.SPath), new TXT(thDataWork), "*.txt");
        }

        internal override bool Save()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(thDataWork.SPath), new TXT(thDataWork), "*.txt");
        }
    }
}

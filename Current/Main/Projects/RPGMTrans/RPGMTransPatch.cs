using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTransPatch;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        internal override bool IsValid()
        {
            if (Path.GetFileName(AppData.SelectedFilePath) == "RPGMKTRANSPATCH")
            {
                return true;
            }

            return false;
        }

        public override string Name => "RPG Maker Trans Patch";

        internal override string ProjectDBFolderName => "RPGMakerTrans";

        public override bool Open()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(AppData.SelectedFilePath), typeof(TXTv3), "*.txt");
        }

        public override bool Save()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(AppData.SelectedFilePath), typeof(TXTv3), "*.txt");
        }
    }
}

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
            if (Path.GetFileName(AppData.SelectedProjectFilePath) == "RPGMKTRANSPATCH")
            {
                return true;
            }

            return false;
        }

        public override string Name => "RPG Maker Trans Patch";

        internal override string ProjectDBFolderName => "RPGMakerTrans";

        protected override bool Open()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.GetDirectoryName(AppData.SelectedProjectFilePath), typeof(TXTv3), "*.txt");
        }

        protected override bool Save()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.GetDirectoryName(AppData.SelectedProjectFilePath), typeof(TXTv3), "*.txt");
        }
    }
}

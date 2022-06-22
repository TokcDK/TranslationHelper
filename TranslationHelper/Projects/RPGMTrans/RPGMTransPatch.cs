﻿using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTransPatch;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        internal override bool Check()
        {
            if (Path.GetFileName(AppData.SelectedFilePath) == "RPGMKTRANSPATCH")
            {
                return true;
            }

            return false;
        }

        internal override string Name => "RPG Maker Trans Patch";

        internal override string ProjectFolderName => "RPGMakerTrans";

        internal override bool TryOpen()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(AppData.SelectedFilePath), typeof(TXTv3), "*.txt");
        }

        internal override bool TrySave()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(AppData.SelectedFilePath), typeof(TXTv3), "*.txt");
        }
    }
}

﻿using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

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
            return OpenSaveFilesBase(Path.GetDirectoryName(ProjectData.SelectedFilePath), new TXT(), "*.txt");
        }

        internal override bool Save()
        {
            return OpenSaveFilesBase(Path.GetDirectoryName(ProjectData.SelectedFilePath), new TXT(), "*.txt");
        }
    }
}

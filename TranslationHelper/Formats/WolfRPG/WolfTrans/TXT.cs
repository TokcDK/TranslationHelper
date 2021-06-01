﻿using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.WolfRPG.WolfTrans
{
    class TXT : WolfRPGBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        internal override bool Save()
        {
            thDataWork.SaveFileMode = true;
            return ParseStringFile();
        }

        protected override int ParseStringFileLine()
        {
            //skip if not patch files
            if (!CheckSetPatchVersion())
            {
                return -1;
            }

            //skip if begin string not found
            if (IsBeginString())
            {
                ParseBeginEndBlock();
            }
            else
            {
                SaveModeAddLine();
            }

            return 0;
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && thDataWork.SaveFileMode && ParseData.ResultForWrite.Length > 0)
                {
                    File.WriteAllText(filePath.Length > 0 ? filePath : thDataWork.FilePath, ParseData.ResultForWrite.ToString().Replace(Properties.Settings.Default.NewLine, "\n"), FunctionsFileFolder.GetEncoding(thDataWork.FilePath));
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}

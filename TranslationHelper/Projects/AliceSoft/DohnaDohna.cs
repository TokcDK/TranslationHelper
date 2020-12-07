﻿using System;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.AliceSoft;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.AliceSoft
{
    class DohnaDohna : AliceSoftBase
    {
        public DohnaDohna(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            string dirPath = Path.GetDirectoryName(thDataWork.SPath);
            return Path.GetExtension(thDataWork.SPath) == ".exe"
                && new DirectoryInfo(dirPath).HasAnyFiles("*.ain")
                ;
        }

        internal override string Name()
        {
            return "Dohna Dohna - Let's do bad things together";
        }

        internal override bool Open()
        {
            return PackUnpack() && OpenSaveFilesBase(Properties.Settings.Default.THProjectWorkDir, new AINTXT(thDataWork), "*.ain.txt");
        }

        private bool PackUnpack()
        {
            if(thDataWork.OpenFileMode)
            {
                Properties.Settings.Default.THProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), Name());
            }

            foreach (var ain in Directory.GetFiles(Path.GetDirectoryName(thDataWork.SPath), "*.ain"))
            {
                var targetaintxtpath = Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(ain) + ".txt");

                if (thDataWork.OpenFileMode)
                {
                    Directory.CreateDirectory(Properties.Settings.Default.THProjectWorkDir);

                    var args = "ain dump -t -o \"" + targetaintxtpath + "\" \"" + ain + "\"";

                    FunctionsProcess.RunProcess(THSettingsData.AliceToolsExePath(), args);
                }
                else
                {
                    if (File.Exists(targetaintxtpath))
                    {
                        var args = "ain edit -t \""+targetaintxtpath+"\" -o \"" + Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(ain)) + "\" \"" + ain + "\"";

                        FunctionsProcess.RunProcess(THSettingsData.AliceToolsExePath(), args);
                    }
                }
            }

            if(thDataWork.OpenFileMode)
            {
                return new DirectoryInfo(Properties.Settings.Default.THProjectWorkDir).HasAnyFiles("*.txt");
            }
            else
            {
                return new DirectoryInfo(Properties.Settings.Default.THProjectWorkDir).HasAnyFiles("*.ain");
            }
        }

        internal override bool Save()
        {
            thDataWork.OpenFileMode = true;
            PackUnpack();//restore original txt before each writing because it will be writed with translated strings while 1st write and will be need to restore it
            thDataWork.SaveFileMode = true;
            return OpenSaveFilesBase(Properties.Settings.Default.THProjectWorkDir, new AINTXT(thDataWork), "*.ain.txt") && PackUnpack();
        }
    }
}

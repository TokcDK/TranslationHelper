using System;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.AliceSoft;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.AliceSoft
{
    class AliceSoftGame : AliceSoftBase
    {
        public AliceSoftGame(THDataWork thDataWork) : base(thDataWork)
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
            return "AliceSoft";
        }

        internal override bool Open()
        {
            return PackUnpack() && OpenSaveFilesBase(Properties.Settings.Default.THProjectWorkDir, new AINTXT(thDataWork), "*.ain.txt");
        }

        private bool PackUnpack()
        {
            if(thDataWork.OpenFileMode)
            {
                Properties.Settings.Default.THProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath)));
            }

            var ret = false;

            var first = false;
            foreach (var ain in Directory.GetFiles(Path.GetDirectoryName(thDataWork.SPath), "*.ain"))
            {
                //only 1st file, for any case
                if (first)
                    continue;
                first = true;

                var targetworkainpath = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "orig.ain");
                var targetworkaintxtpath = targetworkainpath + ".txt";                

                if (thDataWork.OpenFileMode)
                {
                    Directory.CreateDirectory(Properties.Settings.Default.THProjectWorkDir);

                    var args = "ain dump -t -o \"" + targetworkaintxtpath + "\" \"" + targetworkainpath + "\"";

                    if (File.Exists(targetworkainpath))
                        File.Delete(targetworkainpath);
                    File.Copy(ain, targetworkainpath);

                    FunctionsProcess.RunProcess(THSettingsData.AliceToolsExePath(), args);

                    if (File.Exists(targetworkaintxtpath))
                    {
                        ret = true;
                    }
                }
                else
                {
                    if (File.Exists(targetworkaintxtpath))
                    {
                        var outain = targetworkainpath + ".out";

                        var args = "ain edit -t \""+targetworkaintxtpath+"\" -o \"" + outain + "\" \"" + targetworkainpath + "\"";

                        File.WriteAllText(Path.Combine(Properties.Settings.Default.THProjectWorkDir,"write.bat"),
                            " \"" + THSettingsData.AliceToolsExePath()+"\" "+ args
                            + "\r\npause"
                            
                            );

                        FunctionsProcess.RunProcess(THSettingsData.AliceToolsExePath(), args);

                        if (File.Exists(ain + ".bak") && File.Exists(outain))
                        {
                            //remove file
                            var ainfinfo = new FileInfo(ain)
                            {
                                Attributes = FileAttributes.Normal
                            };
                            ainfinfo.Delete();

                            //copy new
                            File.Move(outain, ain);
                            ret = true;
                        }
                    }
                }
            }

            return ret;
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

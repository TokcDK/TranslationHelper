using System;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.AliceSoft;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.AliceSoft
{
    class AliceSoftGame : AliceSoftBase
    {
        public AliceSoftGame()
        {
        }

        internal override bool Check()
        {
            string dirPath = Path.GetDirectoryName(ProjectData.SelectedFilePath);
            return Path.GetExtension(ProjectData.SelectedFilePath) == ".exe"
                && new DirectoryInfo(dirPath).HasAnyFiles("*.ain")
                ;
        }

        internal override string Name()
        {
            return "AliceSoft";
        }

        internal override bool Open()
        {
            return PackUnpack() && OpenSaveFilesBase(ProjectData.ProjectWorkDir, typeof(AINTXT), "*.ain.txt");
        }

        private bool PackUnpack()
        {
            if (ProjectData.OpenFileMode)
            {
                ProjectData.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath(), ProjectFolderName(), Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath)));
            }

            var ret = false;

            var first = false;
            foreach (var ain in Directory.GetFiles(Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.ain"))
            {
                //only 1st file, for any case
                if (first)
                    continue;
                first = true;

                var targetworkainpath = Path.Combine(ProjectData.ProjectWorkDir, "orig.ain");
                var targetworkaintxtpath = targetworkainpath + ".txt";

                if (ProjectData.OpenFileMode)
                {
                    Directory.CreateDirectory(ProjectData.ProjectWorkDir);

                    var args = "ain dump -t -o \"" + targetworkaintxtpath + "\" \"" + targetworkainpath + "\"";

                    if (File.Exists(targetworkainpath))
                        File.Delete(targetworkainpath);
                    File.Copy(ain, targetworkainpath);

                    FunctionsProcess.RunProcess(THSettings.AliceToolsExePath(), args);

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

                        var args = "ain edit -t \"" + targetworkaintxtpath + "\" -o \"" + outain + "\" \"" + targetworkainpath + "\"";

                        File.WriteAllText(Path.Combine(ProjectData.ProjectWorkDir, "write.bat"),
                            " \"" + THSettings.AliceToolsExePath() + "\" " + args
                            + "\r\npause"

                            );

                        FunctionsProcess.RunProcess(THSettings.AliceToolsExePath(), args);

                        if (File.Exists(outain))
                        {
                            //make buckup
                            if (!File.Exists(ain + ".bak"))
                            {
                                File.Copy(ain, ain + ".bak");
                            }

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
                        else
                        {
                            MessageBox.Show(
                                T._("Out ain file creation was failed")
                                + Environment.NewLine
                                + T._("Try to run 'write.bat' manually and check console for errors")
                                );
                            FunctionsProcess.OpenProjectsDir();
                        }
                    }
                }
            }

            return ret;
        }

        internal override bool Save()
        {
            ProjectData.OpenFileMode = true;
            PackUnpack();//restore original txt before each writing because it will be writed with translated strings while 1st write and will be need to restore it
            ProjectData.SaveFileMode = true;
            return OpenSaveFilesBase(ProjectData.ProjectWorkDir, typeof(AINTXT), "*.ain.txt") && PackUnpack();
        }
    }
}

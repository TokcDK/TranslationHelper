using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Formats.RPGMTransPatch;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects
{
    class RPGMGame : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        internal override bool IsValid()
        {
            if (Path.GetExtension(AppData.SelectedFilePath) == ".exe")
            {
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(AppData.SelectedFilePath));

                if (File.Exists(Path.Combine(dir.FullName, "Data", "System.rvdata2"))
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rgss3a")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rgss2a")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rvdata")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rgssad")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rxdata")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.lmt")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.lmu")
                    )
                {
                    return true;
                }
            }

            return false;
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        public override string Name => "RPG Maker Game";

        internal override string ProjectDBFolderName => "RPGMakerTrans";

        internal override bool TablesLinesDictAddEqual => true;

        string extractedpatchpath;

        public override bool Open()
        {
            extractedpatchpath = string.Empty;

            if (Patching())
            {
                if (RPGMTransPatchPrepare())
                {
                    return true;
                }
            }

            return false;
        }

        string patchdir;
        private bool RPGMTransPatchPrepare()
        {
            patchdir = Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileName(AppData.CurrentProject.SelectedGameDir) + "_patch");

            return OpenSaveFilesBase(patchdir, typeof(TXTv3), "*.txt");
        }

        private bool Patching()
        {
            var GameDirPath = new DirectoryInfo(AppData.CurrentProject.SelectedGameDir);
            var workdir = new DirectoryInfo(Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, GameDirPath.Name));
            AppData.CurrentProject.ProjectWorkDir = workdir.FullName;
            var patchdirPath = Path.Combine(workdir.FullName, workdir.Name + "_patch");

            workdir.Create();

            FixOldPatchDirsLocation(workdir);

            if (OpenFileMode && IsPatchFilesExist(patchdirPath))
            {
                DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    //чистка и пересоздание папки
                    CleanDirs(workdir);
                    workdir.Create();
                }
            }

            var ret = CreateUpdatePatch(GameDirPath.FullName, workdir);
            AppData.Main.ProgressInfo(false);
            return ret;
        }

        private static void FixOldPatchDirsLocation(DirectoryInfo workdir)
        {
            if (Directory.Exists(workdir.FullName + "_patch") && !Directory.Exists(Path.Combine(workdir.FullName, workdir.Name + "_patch")))
            {
                Directory.Move(workdir.FullName + "_patch", Path.Combine(workdir.FullName, workdir.Name + "_patch"));
            }

            if (Directory.Exists(workdir.FullName + "_translated") && !Directory.Exists(Path.Combine(workdir.FullName, workdir.Name + "_translated")))
            {
                Directory.Move(workdir.FullName + "_translated", Path.Combine(workdir.FullName, workdir.Name + "_translated"));
            }
        }

        private bool CreateUpdatePatch(string gamedirPath, DirectoryInfo workdir)
        {
            bool ret;
            var rpgmakertranscli = THSettings.RPGMakerTransEXEPath;
            //string projectname = Path.GetFileName(outdir);

            //rpg maker trans cli options
            //maybe need to create settings for this
            //parser.add_argument("input", help = "Path of input game to patch")
            //parser.add_argument("-p", "--patch", help = "Path of patch (directory or zip)"
            //        "(Defaults to input_directory_patch")
            //parser.add_argument("-o", "--output", help = "Path to output directory "
            //        "(will create) (Defaults to input_directory_translated)")
            //parser.add_argument('-q', '--quiet', help = 'Suppress all output',
            //        action = 'store_true')
            //parser.add_argument('-b', '--use-bom', help = 'Use UTF-8 BOM in Patch'
            //        'files', action = 'store_true')
            //parser.add_argument('-r', '--rebuild', help = "Rebuild patch against game",
            //        action = "store_true")
            //parser.add_argument('-s', '--socket', type = int, default = 27899,
            //        help = 'Socket to use for XP/VX/VX Ace patching'
            //        '(default: 27899)')
            //parser.add_argument('-l', '--dump-labels', action = "store_true",
            //        help = "Dump labels to patch file")
            //parser.add_argument('--dump-scripts', type = str, default = None,
            //        help = "Dump scripts to given directory")
            var patchdir = Path.Combine(workdir.FullName, workdir.Name + "_patch");
            var translateddir = Path.Combine(workdir.FullName, workdir.Name + "_translated");
            var args = "\"" + gamedirPath + "\" -p \"" + patchdir + "\" -o \"" + translateddir + "\"";

            //FunctionsProcess.RunProgram(rpgmakertranscli, args);
            //ret = GetIsRPGMakerTransPatchCreatedAndValid(outdir);

            using (var program = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = rpgmakertranscli,
                    Arguments = args,
                    //UseShellExecute = false,
                    //CreateNoWindow = true,
                    //RedirectStandardError = true
                }
            })
            {
                AppData.Main.ProgressInfo(true, T._("Writing ") + "Patch.cmd");
                var patch = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Patch.cmd");
                File.WriteAllText(patch, "\r\n\"" + rpgmakertranscli + "\" " + args + "\r\npause");
                try
                {
                    AppData.Main.ProgressInfo(true, T._("Restore baks"));
                    BakRestore();//restore original files before patch creation
                    AppData.Main.ProgressInfo(true, T._("Patching"));
                    ret = program.Start();
                    program.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(T._("Error occured while patch execution. Error " + ex));
                    return false;
                }

                if (!ret || program.ExitCode > 0 || !IsPatchFilesExist(patchdir))
                {
                    new FunctionsLogs().LogToFile("RPGMaker Trans Patch failed: ret=" + ret + " Exitcode=" + program.ExitCode);
                    AppData.Main.ProgressInfo(true, T._("Patching failed"));
                    CleanDirs(workdir);

                    AppData.Main.ProgressInfo(true, " " + T._("Somethig wrong") + ".. " + T._("Trying 2nd variant"));
                    //2nd try because was error sometime after 1st patch creation execution
                    BakRestore();
                    program.StartInfo.Arguments = args + " -b";

                    AppData.Main.ProgressInfo(true, T._("Writing ") + "Patch.cmd");
                    File.WriteAllText(patch, "\r\n\"" + rpgmakertranscli + "\" " + args + " -b" + "\r\npause");

                    AppData.Main.ProgressInfo(true, T._("Patching") + "-b");
                    ret = program.Start();
                    program.WaitForExit();
                }

                //extract Game.rgss
                if (OpenFileMode && !ret || program.ExitCode > 0 || !IsPatchFilesExist(patchdir))
                {
                    new FunctionsLogs().LogToFile("RPGMaker Trans Patch failed: ret=" + ret + " Exitcode=" + program.ExitCode);
                    AppData.Main.ProgressInfo(true, T._("Last try"));
                    //Maybe rgss3 file was not extracted and need to extract it manually
                    string GameRgss3Path = RPGMFunctions.GetRPGMakerArc(gamedirPath);
                    if (GameRgss3Path.Length == 0)
                    {
                        return false;
                    }

                    var tempExtractDir = new DirectoryInfo(Path.Combine(gamedirPath, "tempTH"));

                    tempExtractDir.Create();

                    var rgssdecrypter = THSettings.RGSSDecrypterEXEPath;

                    var rgssdecrypterargs = "\"--output=" + tempExtractDir + "\" \"" + GameRgss3Path + "\"";

                    AppData.Main.ProgressInfo(true, T._("Extracting") + " " + "Game.rgss3");
                    FunctionsProcess.RunProgram(rgssdecrypter, rgssdecrypterargs);

                    if (!tempExtractDir.HasAnyDirs())
                    {
                        //чистка папок
                        if (tempExtractDir.Exists)
                        {
                            tempExtractDir.Attributes = FileAttributes.Normal;
                            tempExtractDir.Delete(true);
                        }
                        CleanDirs(workdir);
                        return false;
                    }

                    //move all dirs from extracted back to game and remove extracted
                    foreach (var dir in tempExtractDir.GetDirectories())
                    {
                        var targetDirPath = dir.FullName.Replace(tempExtractDir.FullName, gamedirPath);
                        if (Directory.Exists(targetDirPath))
                        {
                            foreach (var file in dir.EnumerateFiles("*.*", SearchOption.AllDirectories))
                            {
                                string targetFilePath = file.FullName.Replace(tempExtractDir.FullName, gamedirPath);
                                if (!File.Exists(targetFilePath))
                                {
                                    File.Move(file.FullName, targetFilePath);
                                }
                            }
                        }
                        else
                        {
                            Directory.Move(dir.FullName, targetDirPath);
                        }
                    }

                    if (new DirectoryInfo(gamedirPath).HasAnyDirs())
                    {
                        tempExtractDir.Attributes = FileAttributes.Normal;
                        tempExtractDir.Delete(true);

                        File.Move(GameRgss3Path, GameRgss3Path + ".orig");

                        args = "\"" + gamedirPath + "\" -p \"" + patchdir + "\"" + " -o \"" + translateddir + "\"";
                        //FunctionsProcess.RunProgram(rpgmakertranscli, args);

                        ret = IsPatchFilesExist(patchdir);

                        if (!ret)
                        {
                            CleanDirs(workdir);

                            AppData.Main.ProgressInfo(true, T._("Patching") + " 3");
                            //попытка с параметром -b - Use UTF-8 BOM in Patch files
                            program.StartInfo.FileName = rpgmakertranscli;
                            program.StartInfo.Arguments = args + " -b";
                            ret = program.Start();
                            program.WaitForExit();

                            //error checks
                            if (!ret)
                            {
                                new FunctionsLogs().LogToFile("RPGMaker Trans Patch failed: ret=" + ret + " Exitcode=" + program.ExitCode);
                                AppData.Main.ProgressInfo(true, T._("Patching failed"));
                                MessageBox.Show(T._("Error occured while patch execution."));
                                CleanDirs(workdir);
                                return false;
                            }

                            if (program.ExitCode > 0)
                            {
                                new FunctionsLogs().LogToFile("RPGMaker Trans Patch failed: ret=" + ret + " Exitcode=" + program.ExitCode);
                                AppData.Main.ProgressInfo(true, T._("Patching failed"));
                                MessageBox.Show(T._("Patch creation finished unsuccesfully.")
                                    + "Exit code="
                                    + program.ExitCode
                                    + Environment.NewLine
                                    + T._("Work folder will be opened.")
                                    + Environment.NewLine
                                    + T._("Try to run Patch.cmd manually and check it for errors.")
                                    );
                                Process.Start("explorer.exe", AppData.CurrentProject.ProjectWorkDir);
                                return false;
                            }

                            ret = IsPatchFilesExist(patchdir);
                        }
                    }
                }
            }

            return ret;
        }

        public static void CleanDirs(DirectoryInfo workdir)
        {
            try
            {
                workdir.Refresh();

                //чистка папок 
                if (Directory.Exists(Path.Combine(workdir.FullName, workdir.Name + "_patch")) && new DirectoryInfo(Path.Combine(workdir.FullName, workdir.Name + "_patch")).HasNoFiles("*.txt"))
                {
                    Directory.Delete(Path.Combine(workdir.FullName, workdir.Name + "_patch"), true);
                }
                if (!Directory.Exists(Path.Combine(workdir.FullName, workdir.Name + "_patch")) && Directory.Exists(Path.Combine(workdir.FullName, workdir.Name + "_translated")))
                {
                    Directory.Delete(Path.Combine(workdir.FullName, workdir.Name + "_translated"), true);
                }

                if (workdir.IsEmpty())
                {
                    workdir.Delete();
                }
            }
            catch
            {

            }
        }

        public static bool IsPatchFilesExist(string patchdir)
        {
            return Path.Combine(patchdir, "patch").ContainsFiles("*.txt");
        }

        public override bool Save()
        {
            OpenSaveFilesBase(patchdir, typeof(TXTv3), "*.txt");//not need to check return value here

            return Patching();
            //return OpenSaveFilesBase(patchdir, new TXT(), "*.txt") && Patching();
        }

        internal override void PreSaveDB()
        {
            OpenSaveFilesBase(patchdir, typeof(TXTv3), "*.txt");
        }

        internal override void AfterTranslationWriteActions()
        {
            Process.Start("explorer.exe", Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileName(AppData.CurrentProject.ProjectWorkDir) + "_translated"));
        }
    }
}

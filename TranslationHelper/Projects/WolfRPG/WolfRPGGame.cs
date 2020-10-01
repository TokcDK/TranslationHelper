using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.WolfRPG
{
    class WolfRPGGame : WolfRPGBase
    {
        public WolfRPGGame(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            string d;
            return Path.GetExtension(thDataWork.SPath) == ".exe"
                && (FunctionsFileFolder.IsInDirExistsAnyFile(d = Path.GetDirectoryName(thDataWork.SPath), "*.wolf", true)
                || (Directory.Exists(d = Path.Combine(d, "Data")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.wolf", true))
                || (Directory.Exists(d = Path.Combine(d, "MapData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.mps", true))
                );
        }

        internal override string Name()
        {
            return "Wolf RPG";
        }

        internal override bool Open()
        {
            return ExtractWolfFiles() && OpenSaveFiles();
        }

        private bool OpenSaveFiles(bool IsOpen = true)
        {
            thDataWork.OpenFileMode = IsOpen;
            var ret = false;
            var OrigFolder = Path.Combine(THSettingsData.WorkDirPath()
                , thDataWork.CurrentProject.ProjectFolderName()
                , Path.GetFileName(Properties.Settings.Default.THSelectedGameDir));
            var patchdir = Path.Combine(OrigFolder, "patch");
            foreach (var txt in Directory.EnumerateFiles(patchdir, "*.txt", SearchOption.AllDirectories))
            {
                thDataWork.FilePath = txt;
                thDataWork.Main.ProgressInfo(true, "Open" + " " + Path.GetFileName(txt));
                if (IsOpen ? new Formats.WolfRPG.WolfTrans.TXT(thDataWork).Open() : new Formats.WolfRPG.WolfTrans.TXT(thDataWork).Save())
                {
                    ret = true;
                }
            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        protected bool ExtractWolfFiles()
        {
            var ret = false;

            try
            {
                //Properties.Settings.Default.THSelectedGameDir

                var WorkFolder = Path.Combine(THSettingsData.WorkDirPath()
                    , thDataWork.CurrentProject.ProjectFolderName()
                    , Path.GetFileName(Properties.Settings.Default.THSelectedGameDir));

                //string DirName = Path.GetFileName(Properties.Settings.Default.THSelectedGameDir);;
                Properties.Settings.Default.THProjectWorkDir = WorkFolder;

                Directory.CreateDirectory(WorkFolder);

                var progressMessageTitle = "Wolf archive" + " " + T._("Extraction") + ".";

                var dataPath = Path.Combine(Properties.Settings.Default.THSelectedGameDir, "Data");

                var dxadecodew = THSettingsData.DXADecodeWExePath();

                foreach (var wolfFile in Directory.EnumerateFiles(Properties.Settings.Default.THSelectedGameDir, "*.wolf", SearchOption.AllDirectories))
                {
                    thDataWork.Main.ProgressInfo(true, progressMessageTitle + T._("Extract") + " " + Path.GetFileName(wolfFile));
                    if (FunctionsProcess.RunProcess(dxadecodew, "\"" + wolfFile + "\""))
                    {
                        ret = true;
                    }
                    File.Move(wolfFile, wolfFile + ".bak");
                }

                if (!Directory.Exists(dataPath))
                {
                    return false;
                }

                var patchdir = Path.Combine(WorkFolder, "patch");
                var translateddir = new DirectoryInfo(Path.Combine(WorkFolder, "translated"));
                if (!Directory.Exists(patchdir) || !FunctionsFileFolder.IsInDirExistsAnyFile(patchdir, "*", true, true))
                {
                    var ruby = THSettingsData.RubyPath();
                    var wolftrans = THSettingsData.WolfTransPath();
                    var args = "\"" + wolftrans + "\""
                        + " \"" + Properties.Settings.Default.THSelectedGameDir + "\""
                        + " \"" + patchdir + "\""
                        + " \"" + translateddir.FullName + "\"";

                    //File.WriteAllText(Path.Combine(OrigFolder, "extract.cmd"), "\r\n" + ruby + " " + args);

                    thDataWork.Main.ProgressInfo(true, progressMessageTitle + T._("Create patch"));
                    //ret = FunctionsProcess.RunProcess(ruby, args);

                    using (Process RubyWolfTrans = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = ruby,
                            Arguments = args,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            //RedirectStandardError = true
                        }
                    })
                    {
                        try
                        {
                            BakRestore();//restore original files before patch creation
                            ret = RubyWolfTrans.Start();
                            RubyWolfTrans.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(T._("Error occured while patch execution. Error " + ex));
                            return false;
                        }
                        if (!ret)
                        {
                            MessageBox.Show(T._("Error occured while patch execution."));
                            return false;
                        }
                        if (RubyWolfTrans.ExitCode > 0)
                        {
                            MessageBox.Show(T._("Patch creation finished unsuccesfully.")
                                + "Exit code="
                                + RubyWolfTrans.ExitCode
                                + Environment.NewLine
                                + T._("Work folder will be opened.")
                                + Environment.NewLine
                                + T._("Try to run Patch.cmd manually and check it for errors.")
                                );
                            Process.Start("explorer.exe", WorkFolder);
                            return false;
                        }
                    }
                }

                if (!(ret = Directory.Exists(patchdir) && translateddir.HasAnyFiles()))
                {
                    return false;
                }

            }
            catch
            {
            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        internal override bool Save()
        {
            return OpenSaveFiles(false) && WritePatch();
        }

        private bool WritePatch()
        {
            var ret = false;

            try
            {
                var WorkFolder = Path.Combine(THSettingsData.WorkDirPath()
                    , thDataWork.CurrentProject.ProjectFolderName()
                    , Path.GetFileName(Properties.Settings.Default.THSelectedGameDir));

                Properties.Settings.Default.THProjectWorkDir = WorkFolder;

                var progressMessageTitle = "Wolf archive" + " " + T._("Write patch") + ".";


                var patchdir = Path.Combine(WorkFolder, "patch");
                var translateddir = new DirectoryInfo(Path.Combine(WorkFolder, "translated"));

                if (translateddir.Exists)
                {
                    try
                    {
                        translateddir.Attributes = FileAttributes.Normal;
                        translateddir.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            T._("Failed to clean old translated dir. Error " + ex)
                            + Environment.NewLine
                            + T._("Work folder will be opened.")
                            + Environment.NewLine
                            + T._("Try to delete 'translated' dir manually and try again.")
                            );
                        Process.Start("explorer.exe", WorkFolder);
                        return false;
                    }
                }


                var ruby = THSettingsData.RubyPath();
                var wolftrans = THSettingsData.WolfTransPath();
                var log = Path.Combine(WorkFolder, "OutputLog.txt");
                //-Ku key for ruby to fix unicode errors
                var args = "-Ku \"" + wolftrans + "\""
                    + " \"" + Properties.Settings.Default.THSelectedGameDir + "\""
                    + " \"" + patchdir + "\""
                    + " \"" + translateddir.FullName + "\""
                    //+ " > \"" + log + "\""
                    ;

                thDataWork.Main.ProgressInfo(true, progressMessageTitle);
                var patch = Path.Combine(WorkFolder, "Patch.cmd");
                File.WriteAllText(patch, "\r\n\"" + ruby + "\" " + args + "\r\npause");

                using (Process RubyWolfTrans = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ruby,
                        Arguments = args,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        //RedirectStandardError = true
                    }
                })
                {
                    try
                    {
                        BakRestore();//restore original files before patch creation
                        ret = RubyWolfTrans.Start();
                        RubyWolfTrans.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(T._("Error occured while patch execution. Error " + ex));
                        return false;
                    }
                    if (!ret)
                    {
                        MessageBox.Show(T._("Error occured while patch execution."));
                        return false;
                    }
                    if (RubyWolfTrans.ExitCode > 0)
                    {
                        MessageBox.Show(T._("Patch creation finished unsuccesfully.")
                            + "Exit code="
                            + RubyWolfTrans.ExitCode
                            + Environment.NewLine
                            + T._("Work folder will be opened.")
                            + Environment.NewLine
                            + T._("Try to run Patch.cmd manually and check it for errors.")
                            );
                        Process.Start("explorer.exe", WorkFolder);
                        return false;
                    }
                }

                if (!(ret = Directory.Exists(patchdir) && translateddir.HasAnyFiles()))
                {
                    return false;
                }


                thDataWork.Main.ProgressInfo(true, T._("Create buckup of original files"));
                BakCreate();

                thDataWork.Main.ProgressInfo(true, T._("Replace translated files"));
                ReplaceFilesWithTranslated();
            }
            catch
            {

            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        private void ReplaceFilesWithTranslated()
        {
            var translatedDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), Path.GetFileName(Properties.Settings.Default.THSelectedGameDir), "translated");
            if (Directory.Exists(translatedDir))
            {
                foreach (var file in Directory.EnumerateFiles(translatedDir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var targetFile = file.Replace(translatedDir, Properties.Settings.Default.THSelectedGameDir);
                        if (File.Exists(targetFile))
                        {
                            if (!File.Exists(targetFile + ".bak"))
                            {
                                File.Move(targetFile, targetFile + ".bak");
                            }
                            else
                            {
                                File.Delete(targetFile);
                            }
                        }
                        File.Move(file, targetFile);
                    }
                    catch
                    {

                    }
                }
            }
        }

        internal override bool BakCreate()
        {
            var translatedDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), Path.GetFileName(Properties.Settings.Default.THSelectedGameDir), "translated");
            if (Directory.Exists(translatedDir))
                return BuckupRestorePaths(Directory.GetFiles(translatedDir, "*.*", SearchOption.AllDirectories).Select(l => l.Replace(translatedDir, Properties.Settings.Default.THSelectedGameDir)).ToArray());
            else
                return false;
        }

        internal override bool BakRestore()
        {
            var ret = false;
            foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(thDataWork.SPath), "*.bak", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".wolf.bak"))
                    continue;

                string origfile;
                if (File.Exists(origfile = file.Remove(file.Length - 4, 4)))
                {
                    if (BuckupFile(origfile))
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }
        internal override bool CheckForRowIssue(System.Data.DataRow row)
        {
            //escape sequences check
            string t;
            if((t = row[1] + string.Empty).Length == 0)
            {
            }
            else if (Regex.IsMatch(t, @"(?<!\\)\\[^sntr><#\\]"))
            {
                return true;
            }
            else if (Regex.IsMatch(row[0] as string, @"\\\\r\[[^\,]+\,[^\]]+\]") && !Regex.IsMatch(t, @"\\\\r\[[^\,]+\,[^\]]+\]"))
            {
                return true;
            }

            return false;
        }
    }
}
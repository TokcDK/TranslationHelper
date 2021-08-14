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
        public WolfRPGGame()
        {
        }

        internal override bool Check()
        {
            string d;
            return Path.GetExtension(ProjectData.SelectedFilePath) == ".exe"
                && (FunctionsFileFolder.IsInDirExistsAnyFile(d = Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.wolf", recursive: true)
                || (Directory.Exists(d = Path.Combine(d, "Data")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.wolf", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "MapData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.mps", recursive: true))
                );
        }

        internal override string Name()
        {
            return "Wolf RPG";
        }

        internal override bool Open()
        {
            return /*ExtractWolfFiles()*/ Patch() && OpenSaveFiles();
        }

        private bool OpenSaveFiles()
        {
            var OrigFolder = Path.Combine(THSettings.WorkDirPath()
                , ProjectData.CurrentProject.ProjectFolderName()
                , Path.GetFileName(ProjectData.SelectedGameDir));
            var patchdir = Path.Combine(OrigFolder, "patch");
            return OpenSaveFilesBase(new DirectoryInfo(patchdir), new Formats.WolfRPG.WolfTrans.TXT(), "*.txt");
        }

        protected bool ExtractWolfFiles()
        {
            var ret = false;

            try
            {
                //ProjectData.SelectedGameDir

                var WorkFolder = Path.Combine(THSettings.WorkDirPath()
                    , ProjectData.CurrentProject.ProjectFolderName()
                    , Path.GetFileName(ProjectData.SelectedGameDir));

                //string DirName = Path.GetFileName(ProjectData.SelectedGameDir);;
                ProjectData.ProjectWorkDir = WorkFolder;

                Directory.CreateDirectory(WorkFolder);

                var progressMessageTitle = "Wolf archive" + " " + T._("Extraction") + ".";

                var dataPath = Path.Combine(ProjectData.SelectedGameDir, "Data");

                var dxadecodew = THSettings.DXADecodeWExePath();

                foreach (var wolfFile in Directory.EnumerateFiles(ProjectData.SelectedGameDir, "*.wolf", SearchOption.AllDirectories))
                {
                    ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract") + " " + Path.GetFileName(wolfFile));
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

                var patchdir = new DirectoryInfo(Path.Combine(WorkFolder, "patch"));
                var translateddir = new DirectoryInfo(Path.Combine(WorkFolder, "translated"));
                if (!patchdir.Exists || !patchdir.HasAnyFiles("*.txt"))
                {
                    var ruby = THSettings.RubyPath();
                    var wolftrans = THSettings.WolfTransPath();
                    var args = "\"" + wolftrans + "\""
                        + " \"" + ProjectData.SelectedGameDir + "\""
                        + " \"" + patchdir.FullName + "\""
                        + " \"" + translateddir.FullName + "\"";

                    //File.WriteAllText(Path.Combine(OrigFolder, "extract.cmd"), "\r\n" + ruby + " " + args);

                    ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Create patch"));
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

                patchdir.Refresh();
                ret = patchdir.Exists && patchdir.HasAnyFiles("*.txt");
            }
            catch
            {
            }

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        internal override void PreSaveDB()
        {
            OpenSaveFiles();
        }

        internal override bool Save()
        {
            return OpenSaveFiles() && Patch();
        }

        private bool Patch()
        {
            var ret = false;

            try
            {
                var WorkFolder = Path.Combine(THSettings.WorkDirPath()
                    , ProjectData.CurrentProject.ProjectFolderName()
                    , Path.GetFileName(ProjectData.SelectedGameDir));

                ProjectData.ProjectWorkDir = WorkFolder;

                var progressMessageTitle = "Wolf archive" + " " + (ProjectData.OpenFileMode ? T._("Create patch") : T._("Write patch")) + ".";


                var dataPath = Path.Combine(ProjectData.SelectedGameDir, "Data");

                //decode wolf files
                if (ProjectData.OpenFileMode)
                {
                    var wolfextractor = THSettings.WolfRPGExtractorExePath();
                    foreach (var wolfFile in Directory.EnumerateFiles(ProjectData.SelectedGameDir, "*.wolf", SearchOption.AllDirectories))
                    {
                        var extractedDirPath = new DirectoryInfo(Path.Combine(dataPath, Path.GetFileNameWithoutExtension(wolfFile)));
                        if (extractedDirPath.Exists && !extractedDirPath.IsEmpty())
                        {
                            continue;
                        }

                        //.mps, .dat, .project
                        ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract") + " " + Path.GetFileName(wolfFile));
                        if (FunctionsProcess.RunProcess(wolfextractor, "\"" + wolfFile + "\""))
                        {
                            ret = true;
                            File.Move(wolfFile, wolfFile + ".bak");
                        }
                    }
                    if (!Directory.Exists(dataPath))
                    {
                        return false;
                    }
                }

                var patchdir = new DirectoryInfo(Path.Combine(WorkFolder, "patch"));
                var translateddir = new DirectoryInfo(Path.Combine(WorkFolder, "translated"));

                //if (translateddir.Exists)
                //{
                //    try
                //    {
                //        translateddir.Attributes = FileAttributes.Normal;
                //        translateddir.Delete(true);
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show(
                //            T._("Failed to clean old translated dir. Error " + ex)
                //            + Environment.NewLine
                //            + T._("Work folder will be opened.")
                //            + Environment.NewLine
                //            + T._("Try to delete 'translated' dir manually and try again.")
                //            );
                //        Process.Start("explorer.exe", WorkFolder);
                //        return false;
                //    }
                //}

                Directory.CreateDirectory(WorkFolder);

                var needpatch = ProjectData.SaveFileMode || (ProjectData.OpenFileMode && (!patchdir.Exists || !patchdir.HasAnyFiles("*.txt")));

                if (needpatch)
                {
                    var ruby = THSettings.RubyPath();
                    var wolftrans = THSettings.WolfTransPath();
                    var log = Path.Combine(WorkFolder, "OutputLog.txt");
                    //-Ku key for ruby to fix unicode errors
                    var args = "-Ku \"" + wolftrans + "\""
                        + " \"" + ProjectData.SelectedGameDir + "\""
                        + " \"" + patchdir.FullName + "\""
                        + " \"" + translateddir.FullName + "\""
                        //+ " > \"" + log + "\""
                        ;

                    ProjectData.Main.ProgressInfo(true, progressMessageTitle);
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

                        if (!ret || RubyWolfTrans.ExitCode > 0)
                        {
                            ProjectData.Main.ProgressInfo(true, progressMessageTitle + " " + T._("Somethig wrong") + ".. " + T._("Trying again"));
                            //2nd try because was error sometime after 1st patch creation execution
                            BakRestore();
                            ret = RubyWolfTrans.Start();
                            RubyWolfTrans.WaitForExit();
                        }

                        //error checks
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

                var checkdir = ProjectData.OpenFileMode ? translateddir : patchdir;
                checkdir.Refresh();
                ret = checkdir.Exists && checkdir.HasAnyFiles();

                if (ret && ProjectData.SaveFileMode)
                {
                    ProjectData.Main.ProgressInfo(true, T._("Create buckup of original files"));
                    BakCreate();

                    ProjectData.Main.ProgressInfo(true, T._("Replace translated files"));
                    ReplaceFilesWithTranslated();
                }
            }
            catch
            {

            }

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        private void ReplaceFilesWithTranslated()
        {
            var translatedDir = Path.Combine(THSettings.WorkDirPath(), ProjectFolderName(), Path.GetFileName(ProjectData.SelectedGameDir), "translated");
            if (Directory.Exists(translatedDir))
            {
                foreach (var file in Directory.EnumerateFiles(translatedDir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var targetFile = file.Replace(translatedDir, ProjectData.SelectedGameDir);
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
                        File.Copy(file, targetFile);
                    }
                    catch
                    {

                    }
                }
            }
        }

        internal override bool BakCreate()
        {
            var translatedDir = new DirectoryInfo(Path.Combine(THSettings.WorkDirPath(), ProjectFolderName(), Path.GetFileName(ProjectData.SelectedGameDir), "translated"));
            return translatedDir.Exists && BackupRestorePaths(translatedDir.GetFiles("*.*", SearchOption.AllDirectories).Select(filePath => filePath.FullName.Replace(translatedDir.FullName, ProjectData.SelectedGameDir)).ToArray());
        }

        internal override bool BakRestore()
        {
            foreach (var bak in Directory.EnumerateFiles(Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.wolf.bak", SearchOption.AllDirectories))
            {
                var ExtractedDirPath = bak.Remove(bak.Length - 9, 9);
                if (!Directory.Exists(ExtractedDirPath))
                {
                    Directory.Move(bak, bak.Remove(bak.Length - 4, 4));
                }
            }
            return BackupRestorePaths(Directory.GetFiles(Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.bak", SearchOption.AllDirectories).Where(filePath => !filePath.EndsWith(".wolf.bak")).ToArray(), false);
        }

        internal override bool CheckForRowIssue(System.Data.DataRow row)
        {
            string o;
            string t;
            string p;
            if ((t = row[1] + string.Empty).Length == 0)
            {
            }
            else if (Regex.IsMatch(t, @"(?<!\\)\\[^sntr><#\\]"))//escape sequences check
            {
                return true;
            }
            else if (Regex.IsMatch(o = row[0] as string, p = @"\\\\r\[[^\,]+\,[^\]]+\]") && !Regex.IsMatch(t, p))
            {
                return true;
            }
            else if (Regex.IsMatch(o, p = @"^\\(nc?)\s*\<[^\>]+\>") && !Regex.IsMatch(t, p))
            {
                return true;
            }
            else if (Regex.IsMatch(o, p = @"^\\(nc?)\s*\[[^\]]+\]") && !Regex.IsMatch(t, p))
            {
                return true;
            }
            return false;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.WolfRPG
{
    class WolfRPGGame : WolfRPGBase
    {
        public WolfRPGGame()
        {
        }

        public override string Name => "Wolf RPG";

        protected override bool Open()
        {
            return ExtractWolfFiles() && Patch() && OpenSaveFiles();
        }

        public override bool SubpathInTableName => true;

        private bool OpenSaveFiles()
        {
            var OrigFolder = Path.Combine(THSettings.WorkDirPath
                , AppData.CurrentProject.ProjectDBFolderName                , Path.GetFileName(AppData.CurrentProject.SelectedGameDir));
            var patchdir = Path.Combine(OrigFolder, "patch");
            bool[] b = new bool[2] { ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(patchdir), typeof(Formats.WolfRPG.WolfTrans.TXT), "*.txt"), ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext"), typeof(Formats.WolfRPG.EvTextTXT), "*.txt") };
            return b.Any(b1 => b1 == true);
        }

        internal override Task PreSaveDB()
        {
            //OpenSaveFiles();

            return Task.CompletedTask;
        }

        public override bool Save()
        {
            return OpenSaveFiles() && Patch();
        }

        private bool Patch()
        {
            var ret = false;

            try
            {
                var WorkFolder = Path.Combine(THSettings.WorkDirPath
                    , AppData.CurrentProject.ProjectDBFolderName                    , Path.GetFileName(AppData.CurrentProject.SelectedGameDir));

                AppData.CurrentProject.ProjectWorkDir = WorkFolder;

                var progressMessageTitle = "Wolf archive" + " " + (OpenFileMode ? T._("Create patch") : T._("Write patch")) + ".";

                var patchdir = new DirectoryInfo(Path.Combine(WorkFolder, "patch"));
                AppData.CurrentProject.OpenedFilesDir = patchdir.FullName;
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

                var needpatch = SaveFileMode || (OpenFileMode && (!patchdir.Exists || !patchdir.HasAnyFiles("*.txt")));

                if (needpatch)
                {
                    var ruby = THSettings.RubyPath;
                    var wolftrans = THSettings.WolfTransPath;
                    var log = Path.Combine(WorkFolder, "OutputLog.txt");
                    //-Ku key for ruby to fix unicode errors
                    var args = "-Ku \"" + wolftrans + "\""
                        + " \"" + AppData.CurrentProject.SelectedGameDir + "\""
                        + " \"" + patchdir.FullName + "\""
                        + " \"" + translateddir.FullName + "\""
                        //+ " > \"" + log + "\""
                        ;

                    Logger.Info(progressMessageTitle);
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
                            if (OpenFileMode) BakRestore();//restore original files before patch creation
                            Logger.Info("Patching..");
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
                            Logger.Info(progressMessageTitle + " " + T._("Somethig wrong") + ".. " + T._("Trying again"));
                            //2nd try because was error sometime after 1st patch creation execution
                            if (OpenFileMode) BakRestore();
                            ret = RubyWolfTrans.Start();
                            RubyWolfTrans.WaitForExit();
                        }

                        //error checks
                        if (!ret)
                        {
                            MessageBox.Show(T._("Error occured while patch execution."));
                            return false;
                        }
                        else if (RubyWolfTrans.ExitCode > 0)
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

                var checkdir = OpenFileMode ? translateddir : patchdir;
                checkdir.Refresh();
                ret = checkdir.Exists && checkdir.HasAnyFiles();

                if (ret && SaveFileMode)
                {
                    Logger.Info(T._("Create buckup of original files"));
                    BakCreate();

                    Logger.Info(T._("Replace translated files"));
                    ReplaceFilesWithTranslated();
                }
            }
            catch
            {

            }

            
            return ret;
        }

        private void ReplaceFilesWithTranslated()
        {
            //.mps,.dat,.project using in patcher
            var translatedDir = TranslatedDirPath;
            if (Directory.Exists(translatedDir))
            {
                foreach (var file in _projectTranslatableFilesExtensionMasks
                .SelectMany(f => Directory.GetFiles(translatedDir, f, searchOption: SearchOption.AllDirectories)))
                {
                    try
                    {
                        var targetFile = file.Replace(translatedDir, AppData.CurrentProject.SelectedGameDir);
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

        readonly string[] _projectTranslatableFilesExtensionMasks = new[] { "*.mps", "*.dat", "*.project" };
        string TranslatedDirPath => Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, Path.GetFileName(AppData.CurrentProject.SelectedGameDir), "translated");
        public override bool BakCreate()
        {
            //.mps,.dat,.project using in patcher
            var translatedDir = new DirectoryInfo(TranslatedDirPath);
            var filePaths = _projectTranslatableFilesExtensionMasks.SelectMany(f => translatedDir.GetFiles(f, searchOption: SearchOption.AllDirectories)).Select(filePath => filePath.FullName.Replace(translatedDir.FullName, AppData.CurrentProject.SelectedGameDir));
            return translatedDir.Exists && ProjectToolsBackup.BackupRestorePaths(filePaths.Concat(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext") }));
        }

        public override bool BakRestore()
        {
            foreach (var bak in Directory.EnumerateFiles(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "*.wolf.bak", SearchOption.AllDirectories))
            {
                var ExtractedDirPath = bak.Remove(bak.Length - 9, 9);
                if (!Directory.Exists(ExtractedDirPath) && new FileInfo(bak).Length > 1000)
                {
                    Directory.Move(bak, bak.Remove(bak.Length - 4, 4));
                }
            }
            return ProjectToolsBackup.BackupRestorePaths(Directory.GetFiles(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "*.bak", SearchOption.AllDirectories).Where(filePath => !filePath.EndsWith(".wolf.bak")).Concat(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext") }), false);
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
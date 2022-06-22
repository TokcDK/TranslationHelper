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
            return false; // disable
            //string d;
            //return Path.GetExtension(ProjectData.SelectedFilePath) == ".exe"
            //    && (FunctionsFileFolder.IsInDirExistsAnyFile(d = Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.wolf", recursive: true)
            //    || (Directory.Exists(d = Path.Combine(d, "Data")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.wolf", recursive: true))
            //    || (Directory.Exists(d = Path.Combine(d, "MapData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.mps", recursive: true))
            //    );
        }

        internal override string Name => "Wolf RPG";

        internal override bool TryOpen()
        {
            return /*ExtractWolfFiles()*/ Patch() && OpenSaveFiles();
        }

        public override bool SubpathInTableName => true;

        private bool OpenSaveFiles()
        {
            var OrigFolder = Path.Combine(THSettings.WorkDirPath()
                , AppData.CurrentProject.ProjectFolderName                , Path.GetFileName(AppData.CurrentProject.SelectedGameDir));
            var patchdir = Path.Combine(OrigFolder, "patch");
            bool[] b = new bool[2] { OpenSaveFilesBase(new DirectoryInfo(patchdir), typeof(Formats.WolfRPG.WolfTrans.TXT), "*.txt"), OpenSaveFilesBase(Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext"), typeof(Formats.WolfRPG.EvTextTXT), "*.txt") };
            return b.Any(b1 => b1 == true);
        }

        protected bool ExtractWolfFiles()
        {
            var ret = false;

            try
            {
                //ProjectData.CurrentProject.SelectedGameDir

                var WorkFolder = Path.Combine(THSettings.WorkDirPath()
                    , AppData.CurrentProject.ProjectFolderName                    , Path.GetFileName(AppData.CurrentProject.SelectedGameDir));

                //string DirName = Path.GetFileName(ProjectData.CurrentProject.SelectedGameDir);;
                AppData.CurrentProject.ProjectWorkDir = WorkFolder;

                Directory.CreateDirectory(WorkFolder);

                var progressMessageTitle = "Wolf archive" + " " + T._("Extraction") + ".";

                var dataPath = Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data");

                //var extractor = THSettings.DXADecodeWExePath();
                var extractor = THSettings.WolfdecExePath();

                foreach (var wolfFile in Directory.EnumerateFiles(AppData.CurrentProject.SelectedGameDir, "*.wolf", SearchOption.AllDirectories))
                {
                    var nameNoExt = Path.GetFileNameWithoutExtension(wolfFile).ToLowerInvariant();
                    if (nameNoExt.Contains("cg")
                        || nameNoExt.Contains("anime")
                        || nameNoExt.Contains("bgm")
                        || nameNoExt.Contains("se")
                        || nameNoExt.Contains("voice")
                        || nameNoExt.Contains("picture")
                        ) continue;

                    AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract") + " " + Path.GetFileName(wolfFile));
                    if (FunctionsProcess.RunProcess(extractor, "\"" + wolfFile + "\"")) ret = true;

                    File.Move(wolfFile, wolfFile + ".bak");
                }

                if (!Directory.Exists(dataPath)) return false;

                var patchdir = new DirectoryInfo(Path.Combine(WorkFolder, "patch"));
                var translateddir = new DirectoryInfo(Path.Combine(WorkFolder, "translated"));
                if (!patchdir.Exists || !patchdir.HasAnyFiles("*.txt"))
                {
                    var ruby = THSettings.RubyPath();
                    var wolftrans = THSettings.WolfTransPath();
                    var args = "\"" + wolftrans + "\""
                        + " \"" + AppData.CurrentProject.SelectedGameDir + "\""
                        + " \"" + patchdir.FullName + "\""
                        + " \"" + translateddir.FullName + "\"";

                    //File.WriteAllText(Path.Combine(OrigFolder, "extract.cmd"), "\r\n" + ruby + " " + args);

                    AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Create patch"));
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

            AppData.Main.ProgressInfo(false);
            return ret;
        }

        internal override void PreSaveDB()
        {
            //OpenSaveFiles();
        }

        internal override bool TrySave()
        {
            return OpenSaveFiles() && Patch();
        }

        private bool Patch()
        {
            var ret = false;

            try
            {
                var WorkFolder = Path.Combine(THSettings.WorkDirPath()
                    , AppData.CurrentProject.ProjectFolderName                    , Path.GetFileName(AppData.CurrentProject.SelectedGameDir));

                AppData.CurrentProject.ProjectWorkDir = WorkFolder;

                var progressMessageTitle = "Wolf archive" + " " + (AppData.CurrentProject.OpenFileMode ? T._("Create patch") : T._("Write patch")) + ".";

                var dataPath = Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data");

                //decode wolf files
                if (AppData.CurrentProject.OpenFileMode)
                {
                    //var wolfextractor = THSettings.WolfRPGExtractorExePath();
                    var wolfextractor = THSettings.WolfdecExePath();
                    foreach (var wolfFile in Directory.EnumerateFiles(AppData.CurrentProject.SelectedGameDir, "*.wolf", SearchOption.AllDirectories))
                    {
                        var nameNoExt = Path.GetFileNameWithoutExtension(wolfFile).ToLowerInvariant();
                        if (nameNoExt.Contains("cg")
                            || nameNoExt.Contains("anime")
                            || nameNoExt.Contains("bgm")
                            || nameNoExt.Contains("se")
                            || nameNoExt.Contains("voice")
                            || nameNoExt.Contains("picture")
                            || nameNoExt.Contains("effect")
                            ) continue;

                        var extractedDirPath = new DirectoryInfo(Path.Combine(dataPath, Path.GetFileNameWithoutExtension(wolfFile)));
                        if (extractedDirPath.Exists && !extractedDirPath.IsEmpty()) continue;

                        //.mps, .dat, .project
                        AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract") + " " + Path.GetFileName(wolfFile));
                        if (FunctionsProcess.RunProcess(wolfextractor, "\"" + wolfFile + "\""))
                        {
                            ret = true;
                            File.Move(wolfFile, wolfFile + ".bak");
                        }
                    }

                    if (!Directory.Exists(dataPath)) return false;
                }

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

                var needpatch = AppData.CurrentProject.SaveFileMode || (AppData.CurrentProject.OpenFileMode && (!patchdir.Exists || !patchdir.HasAnyFiles("*.txt")));

                if (needpatch)
                {
                    var ruby = THSettings.RubyPath();
                    var wolftrans = THSettings.WolfTransPath();
                    var log = Path.Combine(WorkFolder, "OutputLog.txt");
                    //-Ku key for ruby to fix unicode errors
                    var args = "-Ku \"" + wolftrans + "\""
                        + " \"" + AppData.CurrentProject.SelectedGameDir + "\""
                        + " \"" + patchdir.FullName + "\""
                        + " \"" + translateddir.FullName + "\""
                        //+ " > \"" + log + "\""
                        ;

                    AppData.Main.ProgressInfo(true, progressMessageTitle);
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
                            if (AppData.CurrentProject.OpenFileMode) BakRestore();//restore original files before patch creation
                            AppData.Main.ProgressInfo(true, "Patching..");
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
                            AppData.Main.ProgressInfo(true, progressMessageTitle + " " + T._("Somethig wrong") + ".. " + T._("Trying again"));
                            //2nd try because was error sometime after 1st patch creation execution
                            if (AppData.CurrentProject.OpenFileMode) BakRestore();
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

                var checkdir = AppData.CurrentProject.OpenFileMode ? translateddir : patchdir;
                checkdir.Refresh();
                ret = checkdir.Exists && checkdir.HasAnyFiles();

                if (ret && AppData.CurrentProject.SaveFileMode)
                {
                    AppData.Main.ProgressInfo(true, T._("Create buckup of original files"));
                    BakCreate();

                    AppData.Main.ProgressInfo(true, T._("Replace translated files"));
                    ReplaceFilesWithTranslated();
                }
            }
            catch
            {

            }

            AppData.Main.ProgressInfo(false);
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
        string TranslatedDirPath => Path.Combine(THSettings.WorkDirPath(), ProjectFolderName, Path.GetFileName(AppData.CurrentProject.SelectedGameDir), "translated");
        internal override bool BakCreate()
        {
            //.mps,.dat,.project using in patcher
            var translatedDir = new DirectoryInfo(TranslatedDirPath);
            var filePaths = _projectTranslatableFilesExtensionMasks.SelectMany(f => translatedDir.GetFiles(f, searchOption: SearchOption.AllDirectories)).Select(filePath => filePath.FullName.Replace(translatedDir.FullName, AppData.CurrentProject.SelectedGameDir));
            return translatedDir.Exists && BackupRestorePaths(filePaths.Concat(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext") }));
        }

        internal override bool BakRestore()
        {
            foreach (var bak in Directory.EnumerateFiles(Path.GetDirectoryName(AppData.SelectedFilePath), "*.wolf.bak", SearchOption.AllDirectories))
            {
                var ExtractedDirPath = bak.Remove(bak.Length - 9, 9);
                if (!Directory.Exists(ExtractedDirPath) && new FileInfo(bak).Length > 1000)
                {
                    Directory.Move(bak, bak.Remove(bak.Length - 4, 4));
                }
            }
            return BackupRestorePaths(Directory.GetFiles(Path.GetDirectoryName(AppData.SelectedFilePath), "*.bak", SearchOption.AllDirectories).Where(filePath => !filePath.EndsWith(".wolf.bak")).Concat(new[] { Path.Combine(AppData.CurrentProject.SelectedGameDir, "data", "Evtext") }), false);
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
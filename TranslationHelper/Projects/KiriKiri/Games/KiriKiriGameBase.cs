using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    abstract class KiriKiriGameBase : ProjectBase
    {
        protected KiriKiriGameBase()
        {
            HideVarsBase = new Dictionary<string, string>()
            {
                {"[emb exp=\"", @"\[emb exp\=\""[^\""]+\""\]"}
            };
        }

        protected bool CheckKiriKiriBase()
        {
            return
            Path.GetExtension(ProjectData.SelectedFilePath).ToUpperInvariant() == ".EXE"
                &&
                FunctionsProcess.GetExeDescription(ProjectData.SelectedFilePath) != null
                &&
                FunctionsProcess.GetExeDescription(ProjectData.SelectedFilePath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")
                &&
                FunctionsFileFolder.IsInDirExistsAnyFile(Path.GetDirectoryName(ProjectData.SelectedFilePath), "*.xp3");
        }

        protected const string PatchDirName = "_patch";
        protected string exeCRC = string.Empty;

        protected static string GetXP3OrigDirPath()
        {
            return Path.Combine(ProjectData.ProjectWorkDir, "Orig");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formats">must be one as minimum and set for each mask</param>
        /// <param name="masks">{ "*.ks" } by default</param>
        /// <returns>true if atleast one file was open</returns>
        protected bool OpenSaveFiles(List<Type> formats = null, string[] masks = null)
        {
            var ret = false;

            if (ProjectData.OpenFileMode)
            {
                ret = ExtractXP3Data();

                if (!ret)
                {
                    return false;
                }
            }
            else
            {
                //PatchDir
                Directory.CreateDirectory(Path.Combine(ProjectData.ProjectWorkDir, PatchDirName));

                new FillEmptyTablesLinesDictSaveModeNoDups().All();
            }

            var dir = new DirectoryInfo(GetXP3OrigDirPath());
            if (!dir.Exists)
                return false;

            masks = masks ?? Mask();

            formats = formats ?? FormatType();

            if (OpenSaveFilesBase(dir, formats, masks, true))
            {
                ret = true;
            }

            ProjectData.CurrentProject.TablesLinesDict.Clear();

            if (ret && ProjectData.SaveFileMode)
            {
                ProjectData.Main.ProgressInfo(true, T._("Creating translation patch"));
                ret = PackTranslatedFilesInPatch();
            }

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        internal override string ProjectFolderName()
        {
            return "KiriKiri";
        }

        protected virtual List<Type> FormatType()
        {
            return new List<Type>
                {
                   typeof(Formats.KiriKiri.KSParser.KS)
                };
        }

        protected virtual string[] Mask()
        {
            return new[] { "*.ks" };
        }

        internal override bool Open()
        {
            return OpenSaveFiles();
        }

        internal override bool Save()
        {
            return OpenSaveFiles();
        }

        protected string KiriKiriWorkOrigFolder;

        protected Xp3PatchInfos ProjectXP3List;
        protected bool ExtractXP3Data()
        {
            var ret = false;

            try
            {
                //ProjectData.SelectedGameDir

                KiriKiriWorkOrigFolder = Path.Combine(THSettings.WorkDirPath(), ProjectData.CurrentProject.ProjectFolderName(), Path.GetFileName(ProjectData.SelectedGameDir), "Orig");

                //string DirName = Path.GetFileName(ProjectData.SelectedGameDir);;
                ProjectData.ProjectWorkDir = Path.GetDirectoryName(KiriKiriWorkOrigFolder);

                Directory.CreateDirectory(KiriKiriWorkOrigFolder);

                ProjectXP3List = new Xp3PatchInfos();

                bool usecrc = false;
                var progressMessageTitle = "XP3" + " " + T._("Extraction") + ".";
                var SkipAlreadyExtracted = false;
                var SkipAlreadyExtractedAsked = false;
                foreach (var xp3File in ProjectXP3List.Xp3PatchList)
                {
                    if (!xp3File.FileInfo.Exists)
                    {
                        continue;
                    }

                    if (xp3File.IsTranslation)
                    {
                        File.Delete(xp3File.FileInfo.FullName + KiriKiriGameUtils.KiriKiriTranslationSuffix);
                        xp3File.FileInfo.Delete();
                        continue;
                    }

                    ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Proceed") + ":" + xp3File.FileInfo.Name);
                    var xp3path = xp3File.FileInfo.FullName;

                    DirectoryInfo targetSubFolder = new DirectoryInfo(
                        KiriKiriWorkOrigFolder
                        + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(xp3File.FileInfo.FullName)
                        );

                    ProjectData.Main.ProgressInfo(true, progressMessageTitle + (usecrc ? T._("Calculate control crc") : string.Empty) + ":" + xp3File.FileInfo.Name);
                    var crc = usecrc ? xp3File.FileInfo.FullName.GetCrc32(true, ProjectData.Main.THActionProgressBar) : string.Empty;
                    var XP3crc32Path = usecrc ? targetSubFolder + ".xp3." + crc + ".crc32" : string.Empty;
                    var KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + targetSubFolder + "\"";

                    targetSubFolder.Create();

                    if (!FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder.FullName, "*.*"))
                    {
                        continue;
                    }

                    if (usecrc && File.Exists(XP3crc32Path))
                    {
                        KiriKiriGameUtils.ReCreateFolder(targetSubFolder);
                    }
                    else
                    {
                        if (SkipAlreadyExtracted)
                        {
                            ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Skipped:") + ":" + xp3File.FileInfo.Name);
                            ret = true;
                            continue;
                        }
                        else
                        {
                            if (SkipAlreadyExtractedAsked)
                            {
                                KiriKiriGameUtils.ReCreateFolder(targetSubFolder);
                            }
                            else
                            {
                                SkipAlreadyExtractedAsked = true;

                                var result = MessageBox.Show(T._("Found already extracted files") + " " + xp3File.FileInfo.Name + ". " + T._("Skip already extracted?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (result == DialogResult.Yes)
                                {
                                    SkipAlreadyExtracted = true;
                                    ret = true;
                                    continue;
                                }
                                else
                                {
                                    KiriKiriGameUtils.ReCreateFolder(targetSubFolder);
                                }
                            }
                        }
                    }

                    ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract files from") + " " + xp3File.FileInfo.Name);
                    FunctionsProcess.RunProcess(kirikiriunpacker, KiriKiriEXEargs);

                    if (!ret)
                    {
                        ret = FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder.FullName, "*.*");
                    }
                    if (ret)
                    {
                        if (usecrc)
                        {
                            CleanOldCRCFiles(targetSubFolder.Name, crc);
                            if (!File.Exists(XP3crc32Path))
                            {
                                File.WriteAllText(XP3crc32Path, "xp3 CRC32 control file");
                            }
                        }
                    }
                    else
                    {
                        targetSubFolder.Attributes = FileAttributes.Normal;
                        targetSubFolder.Delete(true);
                    }
                }

                // when files in data
                var dataDir = new DirectoryInfo(Path.Combine(ProjectData.SelectedGameDir,"Data"));
                if (dataDir.Exists)
                {
                    var targetSubFolder = new DirectoryInfo(
                        KiriKiriWorkOrigFolder
                        + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(dataDir.FullName)
                        );

                    bool parseData=false;
                    if (targetSubFolder.Exists)
                    {
                        if (!SkipAlreadyExtracted)
                        {
                            KiriKiriGameUtils.ReCreateFolder(targetSubFolder);
                            parseData = true;
                        }
                    }
                    else
                    {
                        parseData = true;
                    }

                    if (parseData)
                    {
                        ProjectData.Main.ProgressInfo(true, progressMessageTitle + (usecrc ? T._("Calculate control crc") : string.Empty) + ":" + dataDir.Name);

                        var paths = KiriKiriGameUtils.GetPaths(dataDir);

                        foreach(var path in paths)
                        {
                            var targetFileFir = new FileInfo(path.FullName
                                .Remove(0, dataDir.FullName.Length)
                                .Insert(0, Path.Combine(KiriKiriWorkOrigFolder,"Data"))
                                );

                            if (targetFileFir.Exists)
                            {
                                if(targetFileFir.LastWriteTime < path.LastWriteTime)
                                {
                                    // copy new file if target is older
                                    targetFileFir.Delete();
                                    path.CopyTo(targetFileFir.FullName);
                                }
                            }
                            else
                            {
                                path.CopyTo(targetFileFir.FullName);
                            }
                        }
                    }
                }

            }
            catch
            {
            }

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        readonly string kirikiriunpacker = THSettings.KiriKiriToolExePath();

        protected bool PackTranslatedFilesInPatch()
        {
            var ret = false;

            try
            {
                //PatchDir
                var PatchDir = Directory.CreateDirectory(Path.Combine(ProjectData.ProjectWorkDir, PatchDirName));

                if (!FunctionsFileFolder.IsInDirExistsAnyFile(PatchDir.FullName))
                {
                    return false;
                }

                if (ProjectXP3List == null)
                {
                    ProjectXP3List = new Xp3PatchInfos();
                }

                string PatchName = "patch" + ProjectXP3List.MaxIndexString;

                var patch = Path.Combine(ProjectData.ProjectWorkDir, PatchName + ".xp3");

                if (File.Exists(patch))
                {
                    File.Delete(patch);
                }

                string KiriKiriEXEargs = "-c -i \"" + PatchDir.FullName + "\" -o \"" + patch + "\"";

                var LE = THSettings.LocaleEmulatorEXE();
                var args = Path.GetFileName(kirikiriunpacker) + " " + "-c -i \"" + PatchDir.Name + "\" -o \"" + PatchName + ".xp3" + "\"";


                //C:\WINDOWS\AppPatch\AppLoc.exe "%1" "/L0411"
                //kirikiriunpacker = @"C:\WINDOWS\AppPatch\AppLoc.exe";
                //KiriKiriEXEargs = "\"" + kirikiriunpacker + "\"" + " " + KiriKiriEXEargs + " " + "/L0411";

                //var batPath = Path.Combine(ProjectData.ProjectWorkDir, "Extract.bat");
                //var batText = @"C:\WINDOWS\AppPatch\AppLoc.exe " + kirikiriunpacker + " " + KiriKiriEXEargs;
                //File.WriteAllText(batPath, batText);

                bool arc_conv = true;

                if (!arc_conv && !File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(kirikiriunpacker))))
                {
                    File.Copy(THSettings.KiriKiriToolExePath(), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(kirikiriunpacker)));
                    File.Copy(THSettings.KiriKiriToolDllPath(), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(THSettings.KiriKiriToolDllPath())));
                }
                else if (arc_conv && !File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(THSettings.ArcConvExePath()))))
                {
                    File.Copy(THSettings.ArcConvExePath(), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(THSettings.ArcConvExePath())));
                    File.Copy(Path.Combine(THSettings.ArcConvDirPath(), Path.GetFileNameWithoutExtension(THSettings.ArcConvExePath()) + ".dat"), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileNameWithoutExtension(THSettings.ArcConvExePath()) + ".dat"));
                }

                var targetPatchPath = Path.Combine(ProjectData.SelectedGameDir, PatchName + ".xp3");

                //kiririkiunpacker
                var setdir =
                    "cd \"" + ProjectData.ProjectWorkDir + "\""
                    ;
                var copyutil =
                    "if not exist \"" + Path.GetFileName(kirikiriunpacker) + "\" copy \"" + kirikiriunpacker + "\" \"" + ProjectData.ProjectWorkDir + "\\\""
                    + "\r\n"
                    + "if not exist madCHook.dll copy \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\" \"" + ProjectData.ProjectWorkDir + "\\\""
                    ;
                var delutil =
                    "if exist \"" + Path.GetFileName(kirikiriunpacker) + "\" del \"" + Path.GetFileName(kirikiriunpacker) + "\""
                    + "\r\n"
                    + "if exist \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\" del \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\""
                    ;
                var runpack =
                    "\"" + LE + "\" " + args
                    ;
                var copypatch =
                    "if not exist \"" + targetPatchPath + "\" copy \"" + patch + "\" \"" + targetPatchPath + "\""
                    + "\r\n"
                    + "if not exist \"" + targetPatchPath + KiriKiriGameUtils.KiriKiriTranslationSuffix + "\" echo translated files >\"" + targetPatchPath + KiriKiriGameUtils.KiriKiriTranslationSuffix + "\""
                    ;

                //arc_conv
                if (arc_conv)
                {
                    args = " --pack xp3 \"" + PatchDir.Name + "\" \"" + PatchName + ".xp3" + "\"";
                    copyutil =
                        "if not exist \"" + THSettings.ArcConvExeName() + "\" copy \"" + THSettings.ArcConvExePath() + "\" \"" + ProjectData.ProjectWorkDir + "\\\""
                        + "\r\n"
                        + "if not exist arc_conv.dat copy \"" + Path.GetDirectoryName(THSettings.ArcConvExePath()) + "\\arc_conv.dat\" \"" + ProjectData.ProjectWorkDir + "\\\""
                        ;
                    delutil =
                        "if exist \"" + THSettings.ArcConvExeName() + "\" del \"" + THSettings.ArcConvExeName() + "\""
                        + "\r\n"
                        + "if exist \"" + Path.GetDirectoryName(THSettings.ArcConvExePath()) + "\\arc_conv.dat\" del \"" + Path.GetDirectoryName(THSettings.ArcConvExePath()) + "\\arc_conv.dat\""
                        ;
                    runpack = THSettings.ArcConvExeName() + args;
                }

                var cmdContent =
                    setdir
                    + "\r\n"
                    + copyutil
                    + "\r\n"
                    + runpack
                    + "\r\n"
                    + "pause"
                    + "\r\n"
                    + copypatch
                    + "\r\n"
                    + delutil
                    + "\r\n"
                    + "pause"
                    ;
                File.WriteAllText(Path.Combine(ProjectData.ProjectWorkDir, "MakePatch.cmd"), cmdContent);

                FunctionsProcess.RunProcess(arc_conv ? THSettings.ArcConvExePath() : LE, args, ProjectData.ProjectWorkDir);

                if (!File.Exists(patch))
                {
                    Thread.Sleep(2000);
                }
                if (!File.Exists(patch))
                {
                    FunctionsProcess.RunProcess(arc_conv ? THSettings.ArcConvExePath() : LE, args, ProjectData.ProjectWorkDir);

                    if (!File.Exists(patch))
                    {
                        System.Windows.Forms.MessageBox.Show("Patch was not created. Try to create it with bat file in work dir");
                        System.Diagnostics.Process.Start("Explorer.exe", ProjectData.ProjectWorkDir);
                    }
                }

                //FunctionsProcess.RunProcess(kirikiriunpacker, KiriKiriEXEargs);
                //FunctionsProcess.RunProcess(batPath, "");

                if (File.Exists(patch))
                {
                    //var target = Path.Combine(ProjectData.SelectedGameDir, PatchName + ".xp3");
                    File.Copy(patch, targetPatchPath);
                    File.WriteAllText(targetPatchPath + KiriKiriGameUtils.KiriKiriTranslationSuffix, "translated files");//write check info txt file
                    ret = true;
                }

                if (!arc_conv && File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(kirikiriunpacker))))
                {
                    bool removed = false;
                    while (!removed)
                    {
                        try
                        {
                            File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(kirikiriunpacker)));
                            File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(THSettings.KiriKiriToolDllPath())));
                            removed = true;
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                else if (arc_conv && File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(THSettings.ArcConvExePath()))))
                {
                    File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(THSettings.ArcConvExePath())));
                    File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileNameWithoutExtension(THSettings.ArcConvExePath()) + ".dat"));
                }
            }
            catch
            {

            }

            return ret;
        }

        private void CleanOldCRCFiles(string name, string crc)
        {
            if (Directory.Exists(KiriKiriWorkOrigFolder))
            {
                foreach (var file in Directory.EnumerateFiles(KiriKiriWorkOrigFolder, name + ".xp3.*.crc32"))
                {
                    if (Path.GetFileName(file) != name + ".xp3." + crc + ".crc32")
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        //protected static string[] KiriKiriPatchPossiblePrefixes = new string[] { " ", "_", string.Empty }; // old code
        //protected string GetLastPatchNumberString()
        //{
        //    return ProjectXP3List.MaxIndex > 0 ? ProjectXP3List.MaxIndex + "" : "";

        //    // old code
        //    //string name;
        //    //string SelPrefix = null;
        //    //bool skip = false;
        //    //bool next = false;
        //    //for (int i = 1; i < 99; i++)
        //    //{
        //    //    if (SelPrefix == null || next)
        //    //    {
        //    //        next = false;
        //    //        if (SelPrefix == null)
        //    //        {
        //    //            foreach (var prefix in KiriKiriPatchPossiblePrefixes)
        //    //            {
        //    //                name = "patch" + prefix + i + ".xp3";
        //    //                if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name)))
        //    //                {
        //    //                    //check and clean translation patch
        //    //                    if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix)))
        //    //                    {
        //    //                        File.Delete(Path.Combine(ProjectData.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix));
        //    //                        File.Delete(Path.Combine(ProjectData.SelectedGameDir, name));
        //    //                        return prefix + i;
        //    //                    }

        //    //                    SelPrefix = prefix;//set found prefix

        //    //                    skip = true;//skip the file because it exists and not translation

        //    //                    break;//prefix found, exit from other prefix checks
        //    //                }
        //    //            }
        //    //        }

        //    //        if (skip)//skip this file
        //    //        {
        //    //            skip = false;
        //    //            continue;
        //    //        }

        //    //        if (SelPrefix == null)//when patch1 was not found
        //    //        {
        //    //            if (!File.Exists(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3")))
        //    //            {
        //    //                if (IsFoundPatchWithHigherIndex(ref i))
        //    //                {
        //    //                    next = true;
        //    //                    continue;
        //    //                }

        //    //                return string.Empty;
        //    //            }
        //    //            else
        //    //            {
        //    //                //check and clean translation patch
        //    //                if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3.translation")))
        //    //                {
        //    //                    File.Delete(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3.translation"));
        //    //                    File.Delete(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3"));
        //    //                    return string.Empty;
        //    //                }

        //    //                if (IsFoundPatchWithHigherIndex(ref i))
        //    //                {
        //    //                    next = true;
        //    //                    continue;
        //    //                }

        //    //                return string.Empty + i;
        //    //            }
        //    //        }
        //    //    }

        //    //    name = "patch" + SelPrefix + i + ".xp3";
        //    //    if (!File.Exists(Path.Combine(ProjectData.SelectedGameDir, name)))
        //    //    {

        //    //        if (IsFoundPatchWithHigherIndex(ref i, SelPrefix))
        //    //        {
        //    //            next = true;
        //    //            continue;
        //    //        }
        //    //        return SelPrefix + i;
        //    //    }
        //    //    else
        //    //    {
        //    //        //check and clean translation patch
        //    //        if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix)))
        //    //        {
        //    //            File.Delete(Path.Combine(ProjectData.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix));
        //    //            File.Delete(Path.Combine(ProjectData.SelectedGameDir, name));
        //    //            return SelPrefix + i;
        //    //        }
        //    //    }
        //    //}

        //    //return string.Empty;
        //}

        // old code
        //private static bool IsFoundPatchWithHigherIndex(ref int i, string SelPrefix = "")
        //{
        //    for (int ind = 1; ind < 5; ind++)
        //    {
        //        if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, "patch" + SelPrefix + (i + ind) + ".xp3")))
        //        {
        //            i += ind - 1;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //protected string[] GetXP3FilesOld()
        //{
        //    List<string> dataFiles = new List<string>();
        //    if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, "data.xp3")))
        //    {
        //        dataFiles.Add("data.xp3");
        //    }

        //    int number = -1;
        //    int notFound = 0;
        //    string name;
        //    while (number < 99 && notFound < 10)
        //    {
        //        number++;
        //        foreach (var prefix in new[] { " ", "_", string.Empty })
        //        {
        //            if (number > 0)
        //            {
        //                name = "patch" + prefix + number + ".xp3";
        //            }
        //            else
        //            {
        //                name = "patch.xp3";
        //            }

        //            if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name)))
        //            {
        //                if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix)))
        //                {
        //                    File.Delete(Path.Combine(ProjectData.ProjectWorkDir, name));
        //                    if (Directory.Exists(Path.Combine(ProjectData.ProjectWorkDir, name.Replace(".xp3", string.Empty))))
        //                    {
        //                        Directory.Delete(Path.Combine(ProjectData.ProjectWorkDir, name.Replace(".xp3", string.Empty)));
        //                    }
        //                    if (ProjectData.OpenFileMode)
        //                    {
        //                        return dataFiles.ToArray();
        //                    }
        //                    else
        //                    {
        //                        File.Delete(Path.Combine(ProjectData.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix));
        //                        File.Delete(Path.Combine(ProjectData.SelectedGameDir, name));
        //                    }
        //                }
        //                else
        //                {
        //                    if (!dataFiles.Contains(name))
        //                    {
        //                        dataFiles.Add(name);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                notFound++;
        //                if (number == 0)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    return dataFiles.ToArray();
        //}
    }
}

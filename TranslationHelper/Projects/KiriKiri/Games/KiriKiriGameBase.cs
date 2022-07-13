using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public override bool IsSaveToSourceFile => false; // save to patch dir

        protected bool CheckKiriKiriBase()
        {
            return
            Path.GetExtension(AppData.SelectedFilePath).ToUpperInvariant() == ".EXE"
                &&
                FunctionsProcess.GetExeDescription(AppData.SelectedFilePath) != null
                &&
                FunctionsProcess.GetExeDescription(AppData.SelectedFilePath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")
                &&
                FunctionsFileFolder.IsInDirExistsAnyFile(Path.GetDirectoryName(AppData.SelectedFilePath), "*.xp3");
        }

        protected const string PatchDirName = "_patch";
        protected string exeCRC = string.Empty;

        protected static string GetXP3OrigDirPath()
        {
            return Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Orig");
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

            if (OpenFileMode)
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
                Directory.CreateDirectory(Path.Combine(AppData.CurrentProject.ProjectWorkDir, PatchDirName));

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

            if (SaveFileMode && AppData.CurrentProject.DontLoadDuplicates)
            {
                AppData.CurrentProject.TablesLinesDict.Clear();
            }

            if (ret && SaveFileMode)
            {
                AppData.Main.ProgressInfo(true, T._("Creating translation patch"));
                ret = PackTranslatedFilesInPatch();
            }

            AppData.Main.ProgressInfo(false);
            return ret;
        }

        internal override string ProjectFolderName => "KiriKiri";

        protected virtual List<Type> FormatType()
        {
            return new List<Type>
                {
                   typeof(Formats.KiriKiri.Games.KS),
                   typeof(Formats.KiriKiri.Games.TJS),
                   typeof(Formats.KiriKiri.Games.CSV.CSV)
                };
        }

        protected virtual string[] Mask()
        {
            return new string[3] { "*.ks", "*.tjs", "*.csv" };
        }

        public override bool Open()
        {
            return OpenSaveFiles();
        }

        public override bool Save()
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
                //ProjectData.CurrentProject.SelectedGameDir

                KiriKiriWorkOrigFolder = Path.Combine(THSettings.WorkDirPath, AppData.CurrentProject.ProjectFolderName, Path.GetFileName(AppData.CurrentProject.SelectedGameDir), "Orig");

                //string DirName = Path.GetFileName(ProjectData.CurrentProject.SelectedGameDir);;
                AppData.CurrentProject.ProjectWorkDir = Path.GetDirectoryName(KiriKiriWorkOrigFolder);

                Directory.CreateDirectory(KiriKiriWorkOrigFolder);

                ProjectXP3List = new Xp3PatchInfos();

                bool usecrc = false;
                var progressMessageTitle = "XP3" + " " + T._("Extraction") + ".";
                var SkipAlreadyExtracted = false;
                var SkipAlreadyExtractedAsked = false;

                foreach (var xp3File in ProjectXP3List.Xp3PatchList)
                {
                    if (!xp3File.FileInfo.Exists) continue;
                    var name = Path.GetFileNameWithoutExtension(xp3File.FileInfo.FullName);
                    if (string.Equals(name, "video", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (string.Equals(name, "voice", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (string.Equals(name, "voices", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (string.Equals(name, "sound", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (string.Equals(name, "sounds", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (string.Equals(name, "bgm", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (string.Equals(name, "music", StringComparison.InvariantCultureIgnoreCase)) continue;

                    if (File.Exists(xp3File.FileInfo.FullName+".skip")) continue;
                    if (File.Exists(xp3File.FileInfo.FullName+".ignore")) continue;

                    if (xp3File.IsTranslation)
                    {
                        File.Delete(xp3File.FileInfo.FullName + KiriKiriGameUtils.KiriKiriTranslationSuffix);
                        xp3File.FileInfo.Delete();
                        continue;
                    }

                    AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Proceed") + ":" + xp3File.FileInfo.Name);
                    var xp3path = xp3File.FileInfo.FullName;

                    DirectoryInfo targetSubFolder = new DirectoryInfo(
                        KiriKiriWorkOrigFolder
                        + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(xp3File.FileInfo.FullName)
                        );

                    AppData.Main.ProgressInfo(true, progressMessageTitle + (usecrc ? T._("Calculate control crc") : string.Empty) + ":" + xp3File.FileInfo.Name);
                    var crc = usecrc ? xp3File.FileInfo.FullName.GetCrc32(true, AppData.Main.THActionProgressBar) : string.Empty;
                    var XP3crc32Path = usecrc ? targetSubFolder + ".xp3." + crc + ".crc32" : string.Empty;
                    var KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + targetSubFolder + "\"";

                    targetSubFolder.Create();

                    if (!FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder.FullName, "*.*")) continue;

                    if (usecrc && File.Exists(XP3crc32Path))
                    {
                        KiriKiriGameUtils.ReCreateFolder(targetSubFolder);
                    }
                    else
                    {
                        if (SkipAlreadyExtracted)
                        {
                            AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Skipped:") + ":" + xp3File.FileInfo.Name);
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

                    AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract files from") + " " + xp3File.FileInfo.Name);
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
                var dataDir = new DirectoryInfo(Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data"));
                if (dataDir.Exists)
                {
                    var targetSubFolder = new DirectoryInfo(
                        KiriKiriWorkOrigFolder
                        + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(dataDir.FullName)
                        );

                    bool parseData = false;
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
                        AppData.Main.ProgressInfo(true, progressMessageTitle + (usecrc ? T._("Calculate control crc") : string.Empty) + ":" + dataDir.Name);

                        var sourceFilePaths = KiriKiriGameUtils.GetKiriKiriScriptPaths(dataDir, Mask());

                        foreach (var sourceFilePath in sourceFilePaths)
                        {
                            var targetFilePath = new FileInfo(sourceFilePath.FullName
                                .Remove(0, dataDir.FullName.Length)
                                .Insert(0, Path.Combine(KiriKiriWorkOrigFolder, "Data"))
                                );

                            if (targetFilePath.Exists)
                            {
                                if (targetFilePath.LastWriteTime < sourceFilePath.LastWriteTime)
                                {
                                    // copy new file if target is older
                                    targetFilePath.Delete();
                                }
                            }

                            targetFilePath.Directory.Create();
                            sourceFilePath.CopyTo(targetFilePath.FullName);
                        }
                    }
                }
            }
            catch
            {
            }

            AppData.Main.ProgressInfo(false);
            return ret;
        }

        readonly string kirikiriunpacker = THSettings.KiriKiriToolExePath;

        protected bool PackTranslatedFilesInPatch()
        {
            var ret = false;

            try
            {
                //PatchDir
                var PatchDir = Directory.CreateDirectory(Path.Combine(AppData.CurrentProject.ProjectWorkDir, PatchDirName));

                if (!FunctionsFileFolder.IsInDirExistsAnyFile(PatchDir.FullName)) return false;

                if (ProjectXP3List == null) ProjectXP3List = new Xp3PatchInfos();

                string PatchName = "patch" + ProjectXP3List.MaxIndexString;

                var patch = Path.Combine(AppData.CurrentProject.ProjectWorkDir, PatchName + ".xp3");

                if (File.Exists(patch)) File.Delete(patch);

                string KiriKiriEXEargs = "-c -i \"" + PatchDir.FullName + "\" -o \"" + patch + "\"";

                var LE = THSettings.LocaleEmulatorEXE;
                var args = Path.GetFileName(kirikiriunpacker) + " " + "-c -i \"" + PatchDir.Name + "\" -o \"" + PatchName + ".xp3" + "\"";


                //C:\WINDOWS\AppPatch\AppLoc.exe "%1" "/L0411"
                //kirikiriunpacker = @"C:\WINDOWS\AppPatch\AppLoc.exe";
                //KiriKiriEXEargs = "\"" + kirikiriunpacker + "\"" + " " + KiriKiriEXEargs + " " + "/L0411";

                //var batPath = Path.Combine(ProjectData.CurrentProject.ProjectWorkDir, "Extract.bat");
                //var batText = @"C:\WINDOWS\AppPatch\AppLoc.exe " + kirikiriunpacker + " " + KiriKiriEXEargs;
                //File.WriteAllText(batPath, batText);

                bool arc_conv = true;

                var kirikiriUnpackerWorkDirPath = Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileName(kirikiriunpacker));
                var kirikiriUnpackerDllWorkDirPath = Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileName(THSettings.KiriKiriToolDllPath));
                var arcConverterWorkDirPath = Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileName(THSettings.ArcConvExePath));
                var arcConverterDatWorkDirPath = Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileNameWithoutExtension(THSettings.ArcConvExePath) + ".dat");
                if (!arc_conv && !File.Exists(kirikiriUnpackerWorkDirPath))
                {
                    THSettings.KiriKiriToolExePath.TryCopyTo(kirikiriUnpackerWorkDirPath);
                    THSettings.KiriKiriToolDllPath.TryCopyTo(kirikiriUnpackerDllWorkDirPath);
                }
                else if (arc_conv && !File.Exists(arcConverterWorkDirPath))
                {
                    THSettings.ArcConvExePath.TryCopyTo(arcConverterWorkDirPath);
                    Path.Combine(THSettings.ArcConvDirPath, Path.GetFileNameWithoutExtension(THSettings.ArcConvExePath) + ".dat").TryCopyTo(arcConverterDatWorkDirPath);
                }

                string foundTraslationPatchName = Directory.EnumerateFiles(AppData.CurrentProject.SelectedDir, "patch*.xp3.translation").FirstOrDefault();

                var targetPatchPath = foundTraslationPatchName != null ? foundTraslationPatchName.Replace(".translation", "") : Path.Combine(AppData.CurrentProject.SelectedDir, PatchName + ".xp3");

                //kiririkiunpacker
                var setdir =
                    "cd \"" + AppData.CurrentProject.ProjectWorkDir + "\""
                    ;
                var copyutil =
                    "if not exist \"" + Path.GetFileName(kirikiriunpacker) + "\" copy \"" + kirikiriunpacker + "\" \"" + AppData.CurrentProject.ProjectWorkDir + "\\\""
                    + "\r\n"
                    + "if not exist madCHook.dll copy \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\" \"" + AppData.CurrentProject.ProjectWorkDir + "\\\""
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
                    "if exist \"" + targetPatchPath + "\" del \"" + targetPatchPath + "\""
                    + "\r\nif not exist \"" + targetPatchPath + "\" copy \"" + patch + "\" \"" + targetPatchPath + "\""
                    + "\r\n"
                    + "if not exist \"" + targetPatchPath + KiriKiriGameUtils.KiriKiriTranslationSuffix + "\" echo translated files >\"" + targetPatchPath + KiriKiriGameUtils.KiriKiriTranslationSuffix + "\""
                    ;

                //arc_conv
                if (arc_conv)
                {
                    args = " --pack xp3 \"" + PatchDir.Name + "\" \"" + PatchName + ".xp3" + "\"";
                    copyutil =
                        "if not exist \"" + THSettings.ArcConvExeName+ "\" copy \"" + THSettings.ArcConvExePath + "\" \"" + AppData.CurrentProject.ProjectWorkDir + "\\\""
                        + "\r\n"
                        + "if not exist arc_conv.dat copy \"" + Path.GetDirectoryName(THSettings.ArcConvExePath) + "\\arc_conv.dat\" \"" + AppData.CurrentProject.ProjectWorkDir + "\\\""
                        ;
                    delutil =
                        "if exist \"" + THSettings.ArcConvExeName+ "\" del \"" + THSettings.ArcConvExeName+ "\""
                        + "\r\n"
                        + "if exist \"" + Path.GetDirectoryName(THSettings.ArcConvExePath) + "\\arc_conv.dat\" del \"" + Path.GetDirectoryName(THSettings.ArcConvExePath) + "\\arc_conv.dat\""
                        ;
                    runpack = THSettings.ArcConvExeName+ args;
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
                File.WriteAllText(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "MakePatch.cmd"), cmdContent);

                FunctionsProcess.RunProcess(arc_conv ? THSettings.ArcConvExePath : LE, args, AppData.CurrentProject.ProjectWorkDir);

                if (!File.Exists(patch)) Thread.Sleep(2000);
                if (!File.Exists(patch))
                {
                    FunctionsProcess.RunProcess(arc_conv ? THSettings.ArcConvExePath : LE, args, AppData.CurrentProject.ProjectWorkDir);

                    if (!File.Exists(patch))
                    {
                        System.Windows.Forms.MessageBox.Show("Patch was not created. Try to create it with bat file in work dir");
                        System.Diagnostics.Process.Start("Explorer.exe", AppData.CurrentProject.ProjectWorkDir);
                    }
                }

                //FunctionsProcess.RunProcess(kirikiriunpacker, KiriKiriEXEargs);
                //FunctionsProcess.RunProcess(batPath, "");

                if (File.Exists(patch))
                {
                    //var target = Path.Combine(ProjectData.CurrentProject.SelectedGameDir, PatchName + ".xp3");
                    if (File.Exists(targetPatchPath)) File.Delete(targetPatchPath);
                    File.Copy(patch, targetPatchPath);
                    File.WriteAllText(targetPatchPath + KiriKiriGameUtils.KiriKiriTranslationSuffix, "translated files");//write check info txt file
                    ret = true;
                }

                if (!arc_conv && File.Exists(kirikiriUnpackerWorkDirPath))
                {
                    bool removed = false;
                    while (!removed)
                    {
                        try
                        {
                            File.Delete(kirikiriUnpackerWorkDirPath);
                            File.Delete(kirikiriUnpackerDllWorkDirPath);
                            removed = true;
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                else if (arc_conv && File.Exists(arcConverterWorkDirPath))
                {
                    File.Delete(arcConverterWorkDirPath);
                    File.Delete(arcConverterDatWorkDirPath);
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
        //    //                if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name)))
        //    //                {
        //    //                    //check and clean translation patch
        //    //                    if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix)))
        //    //                    {
        //    //                        File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix));
        //    //                        File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name));
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
        //    //            if (!File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "patch.xp3")))
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
        //    //                if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "patch.xp3.translation")))
        //    //                {
        //    //                    File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "patch.xp3.translation"));
        //    //                    File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "patch.xp3"));
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
        //    //    if (!File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name)))
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
        //    //        if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix)))
        //    //        {
        //    //            File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix));
        //    //            File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name));
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
        //        if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "patch" + SelPrefix + (i + ind) + ".xp3")))
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
        //    if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "data.xp3")))
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

        //            if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name)))
        //            {
        //                if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix)))
        //                {
        //                    File.Delete(Path.Combine(ProjectData.CurrentProject.ProjectWorkDir, name));
        //                    if (Directory.Exists(Path.Combine(ProjectData.CurrentProject.ProjectWorkDir, name.Replace(".xp3", string.Empty))))
        //                    {
        //                        Directory.Delete(Path.Combine(ProjectData.CurrentProject.ProjectWorkDir, name.Replace(".xp3", string.Empty)));
        //                    }
        //                    if (ProjectData.OpenFileMode)
        //                    {
        //                        return dataFiles.ToArray();
        //                    }
        //                    else
        //                    {
        //                        File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name + KiriKiriGameUtils.KiriKiriTranslationSuffix));
        //                        File.Delete(Path.Combine(ProjectData.CurrentProject.SelectedGameDir, name));
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

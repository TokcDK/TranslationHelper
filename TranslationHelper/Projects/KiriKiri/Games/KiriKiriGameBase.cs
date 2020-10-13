﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    abstract class KiriKiriGameBase : ProjectBase
    {
        protected KiriKiriGameBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected bool CheckKiriKiriBase()
        {
            return
            Path.GetExtension(thDataWork.SPath).ToUpperInvariant() == ".EXE"
                &&
                FunctionsProcess.GetExeDescription(thDataWork.SPath) != null
                &&
                FunctionsProcess.GetExeDescription(thDataWork.SPath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")
                &&
                FunctionsFileFolder.IsInDirExistsAnyFile(Path.GetDirectoryName(thDataWork.SPath), "*.xp3");
        }

        protected const string PatchDirName = "_patch";
        protected string exeCRC = string.Empty;

        protected static string GetXP3OrigDirPath()
        {
            return Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Orig");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">must be one as minimum and set for each mask</param>
        /// <param name="masks">{ "*.ks" } by default</param>
        /// <returns>true if atleast one file was open</returns>
        protected bool OpenFiles(List<FormatBase> format, string[] masks = null)
        {
            var ret = false;
            var dir = new DirectoryInfo(GetXP3OrigDirPath());
            if (!dir.Exists)
                return false;

            masks = masks ?? new[] { "*.ks" };

            foreach (var mask in masks)
            {
                if (mask.Length == 0)
                    continue;

                thDataWork.Main.ProgressInfo(true, T._("get list of files") + " (" + mask + ")");
                var files = GetNewestFilesList(dir, mask);

                int i = 0;
                foreach (var file in files)
                {
                    if (file.Name == "extra_daidokoro_1e.ks")
                    {

                    }
                    thDataWork.FilePath = file.FullName;
                    thDataWork.Main.ProgressInfo(true, T._("open file: ") + file.Name);
                    try
                    {
                        if (format[i].Open())
                        {
                            ret = true;
                        }
                    }
                    catch
                    {
                        thDataWork.Main.ProgressInfo(true, T._("failed to open file file: ") + file.Name);
                    }
                }
            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        protected static List<FileInfo> GetNewestFilesList(DirectoryInfo dir, string mask = "*.*")
        {
            var newestfiles = new Dictionary<string, FileInfo>();

            foreach (var file in dir.EnumerateFiles(mask, SearchOption.AllDirectories))
            {
                var name = file.Name;
                bool contains = newestfiles.ContainsKey(name);
                if (contains)
                {
                    if (file.LastWriteTime <= newestfiles[name].LastWriteTime)
                    {
                        continue;
                    }
                    else
                    {
                        newestfiles[name] = file;
                    }
                }
                else
                {
                    newestfiles.Add(name, file);
                }
            }

            return newestfiles.Values.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">must be one as minimum and set for each mask</param>
        /// <param name="masks">{ "*.ks" } by default</param>
        /// <returns>true if atleast one file was saved</returns>
        protected bool SaveFiles(List<FormatBase> format, string[] masks = null)
        {
            var dir = new DirectoryInfo(GetXP3OrigDirPath());
            if (!dir.Exists)
                return false;

            masks = masks ?? new[] { "*.ks" };

            //PatchDir
            Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchDirName));

            thDataWork.CurrentProject.FillTablesLinesDict();

            var tableslist = thDataWork.THFilesElementsDataset.Tables;

            bool ret = false;
            foreach (var mask in masks)
            {
                if (mask.Length == 0)
                    continue;

                thDataWork.Main.ProgressInfo(true, T._("get list of files") + " (" + mask + ")");
                var files = GetNewestFilesList(dir, mask);

                int i = 0;
                foreach (var file in files)
                {
                    if (!tableslist.Contains(file.Name))
                    {
                        continue;
                    }

                    thDataWork.FilePath = file.FullName;
                    thDataWork.Main.ProgressInfo(true, T._("save file: ") + file.Name);
                    try
                    {
                        if (format[i].Save())
                        {
                            ret = true;
                        }
                    }
                    catch
                    {
                        thDataWork.Main.ProgressInfo(true, T._("failed to save file file: ") + file.Name);
                    }
                }
            }

            thDataWork.TablesLinesDict.Clear();

            if (ret)
            {
                thDataWork.Main.ProgressInfo(true, T._("create and copy patch to the game folder"));
                ret = PackTranslatedFilesInPatch();
            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        internal string[] ProjectXP3List;

        internal override string ProjectFolderName()
        {
            return "KiriKiri";
        }

        protected string KiriKiriWorkOrigFolder;
        protected bool ExtractXP3Data()
        {
            var ret = false;

            try
            {
                //Properties.Settings.Default.THSelectedGameDir

                KiriKiriWorkOrigFolder = Path.Combine(THSettingsData.WorkDirPath(), thDataWork.CurrentProject.ProjectFolderName(), Path.GetFileName(Properties.Settings.Default.THSelectedGameDir), "Orig");

                //string DirName = Path.GetFileName(Properties.Settings.Default.THSelectedGameDir);;
                Properties.Settings.Default.THProjectWorkDir = Path.GetDirectoryName(KiriKiriWorkOrigFolder);

                Directory.CreateDirectory(KiriKiriWorkOrigFolder);

                ProjectXP3List = GetXP3Files();

                bool usecrc = false;
                var progressMessageTitle = "XP3" + " " + T._("Extraction") + ".";
                var SkipAlreadyExtracted = false;
                var SkipAlreadyExtractedAsked = false;
                foreach (var name in ProjectXP3List)
                {
                    thDataWork.Main.ProgressInfo(true, progressMessageTitle + T._("Proceed") + ":" + name);
                    var xp3path = Path.Combine(Properties.Settings.Default.THSelectedGameDir, name);
                    if (File.Exists(xp3path))
                    {
                        DirectoryInfo targetSubFolder = new DirectoryInfo(
                            KiriKiriWorkOrigFolder
                            + Path.DirectorySeparatorChar
                            + Path.GetFileNameWithoutExtension(xp3path)
                            );

                        thDataWork.Main.ProgressInfo(true, progressMessageTitle + (usecrc ? T._("Calculate control crc") : string.Empty) + ":" + name);
                        var crc = usecrc ? xp3path.GetCrc32(true, thDataWork.Main.THActionProgressBar) : string.Empty;
                        var XP3crc32Path = usecrc ? targetSubFolder + ".xp3." + crc + ".crc32" : string.Empty;
                        var KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + targetSubFolder + "\"";
                        if (!targetSubFolder.Exists)
                        {
                            targetSubFolder.Create();
                        }
                        else
                        {
                            if (File.Exists(xp3path + ".translation"))
                            {
                                File.Delete(xp3path + ".translation");
                                File.Delete(xp3path);
                                continue;
                            }

                            if (FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder.FullName, "*.*"))
                            {
                                if ((usecrc && File.Exists(XP3crc32Path)) || !usecrc)
                                {
                                    if (SkipAlreadyExtracted)
                                    {
                                        thDataWork.Main.ProgressInfo(true, progressMessageTitle + T._("Skipped:") + ":" + name);
                                        ret = true;
                                        continue;
                                    }
                                    else
                                    {
                                        if (!SkipAlreadyExtractedAsked)
                                        {
                                            var result = MessageBox.Show(T._("Found already extracted files") + " " + name + ". " + T._("Skip already extracted?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                            if (result == DialogResult.Yes)
                                            {
                                                SkipAlreadyExtractedAsked = SkipAlreadyExtracted = true;
                                                ret = true;
                                                continue;
                                            }
                                            else
                                            {
                                                SkipAlreadyExtractedAsked = true;
                                                ReCreateFolder(targetSubFolder);
                                            }
                                        }
                                        else
                                        {
                                            ReCreateFolder(targetSubFolder);
                                        }
                                    }
                                }
                                else
                                {
                                    ReCreateFolder(targetSubFolder);
                                }
                            }
                        }

                        thDataWork.Main.ProgressInfo(true, progressMessageTitle + T._("Extract files from") + " " + name);
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
                }
            }
            catch
            {
            }

            thDataWork.Main.ProgressInfo(false);
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

        private static void ReCreateFolder(DirectoryInfo targetSubFolder)
        {
            if (!targetSubFolder.Exists)
                return;

            targetSubFolder.Attributes = FileAttributes.Normal;
            targetSubFolder.Delete(true);
            targetSubFolder.Create();
        }

        readonly string kirikiriunpacker = THSettingsData.KiriKiriToolExePath();

        protected bool PackTranslatedFilesInPatch()
        {
            var ret = false;

            try
            {
                //PatchDir
                var PatchDir = Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchDirName));

                if (!FunctionsFileFolder.IsInDirExistsAnyFile(PatchDir.FullName))
                {
                    return false;
                }

                if (ProjectXP3List == null)
                {
                    ProjectXP3List = GetXP3Files();
                }

                string PatchName = "patch" + GetLastIndexOfPatch();

                var patch = Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchName + ".xp3");

                if (File.Exists(patch))
                {
                    File.Delete(patch);
                }

                string KiriKiriEXEargs = "-c -i \"" + PatchDir.FullName + "\" -o \"" + patch + "\"";

                var LE = THSettingsData.LocaleEmulatorEXE();
                var args = Path.GetFileName(kirikiriunpacker) + " " + "-c -i \"" + PatchDir.Name + "\" -o \"" + PatchName + ".xp3" + "\"";


                //C:\WINDOWS\AppPatch\AppLoc.exe "%1" "/L0411"
                //kirikiriunpacker = @"C:\WINDOWS\AppPatch\AppLoc.exe";
                //KiriKiriEXEargs = "\"" + kirikiriunpacker + "\"" + " " + KiriKiriEXEargs + " " + "/L0411";

                //var batPath = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extract.bat");
                //var batText = @"C:\WINDOWS\AppPatch\AppLoc.exe " + kirikiriunpacker + " " + KiriKiriEXEargs;
                //File.WriteAllText(batPath, batText);

                if (!File.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(kirikiriunpacker))))
                {
                    File.Copy(THSettingsData.KiriKiriToolExePath(), Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(kirikiriunpacker)));
                    File.Copy(THSettingsData.KiriKiriToolDllPath(), Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(THSettingsData.KiriKiriToolDllPath())));
                }

                var targetPatchPath = Path.Combine(Properties.Settings.Default.THSelectedGameDir, PatchName + ".xp3");
                File.WriteAllText(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "MakePatch.cmd"),
                    "cd \"" + Properties.Settings.Default.THProjectWorkDir + "\""
                    + "\r\n"
                    + "if not exist \"" + Path.GetFileName(kirikiriunpacker) + "\" copy \"" + kirikiriunpacker + "\" \"" + Properties.Settings.Default.THProjectWorkDir + "\\\""
                    + "\r\n"
                    + "if not exist madCHook.dll copy \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\" \"" + Properties.Settings.Default.THProjectWorkDir + "\\\""
                    + "\r\n"
                    + "\"" + LE + "\" " + args
                    + "\r\n"
                    + "pause"
                    + "\r\n"
                    + "if not exist \"" + targetPatchPath + "\" copy \"" + patch + "\" \"" + targetPatchPath + "\""
                    + "\r\n"
                    + "if not exist \"" + targetPatchPath + ".translation\" echo translated files >\"" + targetPatchPath + ".translation\""
                    + "\r\n"
                    + "if exist \"" + Path.GetFileName(kirikiriunpacker) + "\" del \"" + Path.GetFileName(kirikiriunpacker) + "\""
                    + "\r\n"
                    + "if exist \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\" del \"" + Path.GetDirectoryName(kirikiriunpacker) + "\\madCHook.dll\""
                    + "pause"
                    );

                Task.Run(() => FunctionsProcess.RunProcess(LE, args, Properties.Settings.Default.THProjectWorkDir)).ConfigureAwait(true);
                if (!File.Exists(patch))
                {
                    Thread.Sleep(2000);
                }
                if (!File.Exists(patch))
                {
                    FunctionsProcess.RunProcess(LE, args, Properties.Settings.Default.THProjectWorkDir);

                    if (!File.Exists(patch))
                    {
                        System.Windows.Forms.MessageBox.Show("Patch was not created. Try to create it with bat file in work dir");
                        System.Diagnostics.Process.Start("Explorer.exe", Properties.Settings.Default.THProjectWorkDir);
                    }
                }

                if (File.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(kirikiriunpacker))))
                {
                    bool removed = false;
                    while (!removed)
                    {
                        try
                        {
                            File.Delete(Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(kirikiriunpacker)));
                            File.Delete(Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(THSettingsData.KiriKiriToolDllPath())));
                            removed = true;
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                //FunctionsProcess.RunProcess(kirikiriunpacker, KiriKiriEXEargs);
                //FunctionsProcess.RunProcess(batPath, "");

                if (File.Exists(patch))
                {
                    //var target = Path.Combine(Properties.Settings.Default.THSelectedGameDir, PatchName + ".xp3");
                    File.Copy(patch, targetPatchPath);
                    File.WriteAllText(targetPatchPath + ".translation", "translated files");//write check info txt file
                    ret = true;
                }
            }
            catch
            {

            }

            return ret;
        }

        protected static string[] KiriKiriPatchPossiblePrefixes = new string[] { " ", "_", string.Empty };
        protected static string GetLastIndexOfPatch()
        {
            string name;
            string SelPrefix = null;
            bool skip = false;
            bool next = false;
            for (int i = 1; i < 99; i++)
            {
                if (SelPrefix == null || next)
                {
                    next = false;
                    if (SelPrefix == null)
                    {
                        foreach (var prefix in KiriKiriPatchPossiblePrefixes)
                        {
                            name = "patch" + prefix + i + ".xp3";
                            if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name)))
                            {
                                //check and clean translation patch
                                if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation")))
                                {
                                    File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation"));
                                    File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name));
                                    return prefix + i;
                                }

                                SelPrefix = prefix;//set found prefix

                                skip = true;//skip the file because it exists and not translation

                                break;//prefix found, exit from other prefix checks
                            }
                        }
                    }

                    if (skip)//skip this file
                    {
                        skip = false;
                        continue;
                    }

                    if (SelPrefix == null)//when patch1 was not found
                    {
                        if (!File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "patch.xp3")))
                        {
                            if (IsFoundPatchWithHigherIndex(ref i))
                            {
                                next = true;
                                continue;
                            }

                            return string.Empty;
                        }
                        else
                        {
                            //check and clean translation patch
                            if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "patch.xp3.translation")))
                            {
                                File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "patch.xp3.translation"));
                                File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "patch.xp3"));
                                return string.Empty;
                            }

                            if (IsFoundPatchWithHigherIndex(ref i))
                            {
                                next = true;
                                continue;
                            }

                            return string.Empty + i;
                        }
                    }
                }

                name = "patch" + SelPrefix + i + ".xp3";
                if (!File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name)))
                {

                    if (IsFoundPatchWithHigherIndex(ref i, SelPrefix))
                    {
                        next = true;
                        continue;
                    }
                    return SelPrefix + i;
                }
                else
                {
                    //check and clean translation patch
                    if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation")))
                    {
                        File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation"));
                        File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name));
                        return SelPrefix + i;
                    }
                }
            }

            return string.Empty;
        }

        private static bool IsFoundPatchWithHigherIndex(ref int i, string SelPrefix = "")
        {
            for (int ind = 1; ind < 5; ind++)
            {
                if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "patch" + SelPrefix + (i + ind) + ".xp3")))
                {
                    i += ind - 1;
                    return true;
                }
            }
            return false;
        }

        protected static string[] GetXP3Files()
        {
            List<string> dataFiles = new List<string>();
            if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "data.xp3")))
            {
                dataFiles.Add("data.xp3");
            }

            int number = -1;
            int notFound = 0;
            string name;
            while (number < 99 && notFound < 10)
            {
                number++;
                foreach (var prefix in new[] { " ", "_", string.Empty })
                {
                    if (number > 0)
                    {
                        name = "patch" + prefix + number + ".xp3";
                    }
                    else
                    {
                        name = "patch.xp3";
                    }

                    if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name)))
                    {
                        if (File.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name)))
                        {
                            File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name));
                            File.Delete(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name));
                            if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation")))
                            {
                                File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation"));
                            }
                            if (Directory.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name.Replace(".xp3", string.Empty))))
                            {
                                Directory.Delete(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name.Replace(".xp3", string.Empty)));
                            }
                        }
                        else if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation")))
                        {
                            File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name + ".translation"));
                            File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name));
                        }
                        else
                        {
                            dataFiles.Add(name);
                        }
                    }
                    else
                    {
                        notFound++;
                        if (number == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return dataFiles.ToArray();
        }
    }
}
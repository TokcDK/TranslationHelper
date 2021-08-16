using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
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
        protected string ExeCrc = string.Empty;

        protected static string GetXp3OrigDirPath()
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
                ret = ExtractXp3Data();

                if (!ret)
                {
                    return false;
                }
            }
            else
            {
                //PatchDir
                Directory.CreateDirectory(Path.Combine(ProjectData.ProjectWorkDir, PatchDirName));

                ProjectData.CurrentProject.FillTablesLinesDict();
            }

            var dir = new DirectoryInfo(GetXp3OrigDirPath());
            if (!dir.Exists)
                return false;

            masks = masks ?? Mask();

            formats = formats ?? FormatType();

            if (OpenSaveFilesBase(dir, formats, masks, true))
            {
                ret = true;
            }

            ProjectData.TablesLinesDict.Clear();

            if (ret && ProjectData.SaveFileMode)
            {
                ProjectData.Main.ProgressInfo(true, T._("Creating translation patch"));
                ret = PackTranslatedFilesInPatch();
            }

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        internal string[] ProjectXp3List;

        internal override string ProjectFolderName()
        {
            return "KiriKiri";
        }

        protected virtual List<Type> FormatType()
        {
            return new List<Type>
                {
                   typeof(Formats.KiriKiri.Games.Ks)
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
        protected bool ExtractXp3Data()
        {
            var ret = false;

            try
            {
                //ProjectData.SelectedGameDir

                KiriKiriWorkOrigFolder = Path.Combine(ThSettings.WorkDirPath(), ProjectData.CurrentProject.ProjectFolderName(), Path.GetFileName(ProjectData.SelectedGameDir), "Orig");

                //string DirName = Path.GetFileName(ProjectData.SelectedGameDir);;
                ProjectData.ProjectWorkDir = Path.GetDirectoryName(KiriKiriWorkOrigFolder);

                Directory.CreateDirectory(KiriKiriWorkOrigFolder);

                ProjectXp3List = GetXp3Files();

                bool usecrc = false;
                var progressMessageTitle = "XP3" + " " + T._("Extraction") + ".";
                var skipAlreadyExtracted = false;
                var skipAlreadyExtractedAsked = false;
                foreach (var name in ProjectXp3List)
                {
                    ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Proceed") + ":" + name);
                    var xp3Path = Path.Combine(ProjectData.SelectedGameDir, name);
                    if (File.Exists(xp3Path))
                    {
                        DirectoryInfo targetSubFolder = new DirectoryInfo(
                            KiriKiriWorkOrigFolder
                            + Path.DirectorySeparatorChar
                            + Path.GetFileNameWithoutExtension(xp3Path)
                            );

                        ProjectData.Main.ProgressInfo(true, progressMessageTitle + (usecrc ? T._("Calculate control crc") : string.Empty) + ":" + name);
                        var crc = usecrc ? xp3Path.GetCrc32(true, ProjectData.Main.THActionProgressBar) : string.Empty;
                        var xp3Crc32Path = usecrc ? targetSubFolder + ".xp3." + crc + ".crc32" : string.Empty;
                        var kiriKiriExEargs = "-i \"" + xp3Path + "\" -o \"" + targetSubFolder + "\"";
                        if (!targetSubFolder.Exists)
                        {
                            targetSubFolder.Create();
                        }
                        else
                        {
                            if (File.Exists(xp3Path + ".translation"))
                            {
                                File.Delete(xp3Path + ".translation");
                                File.Delete(xp3Path);
                                continue;
                            }

                            if (FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder.FullName, "*.*"))
                            {
                                if ((usecrc && File.Exists(xp3Crc32Path)) || !usecrc)
                                {
                                    if (skipAlreadyExtracted)
                                    {
                                        ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Skipped:") + ":" + name);
                                        ret = true;
                                        continue;
                                    }
                                    else
                                    {
                                        if (!skipAlreadyExtractedAsked)
                                        {
                                            var result = MessageBox.Show(T._("Found already extracted files") + " " + name + ". " + T._("Skip already extracted?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                            if (result == DialogResult.Yes)
                                            {
                                                skipAlreadyExtractedAsked = skipAlreadyExtracted = true;
                                                ret = true;
                                                continue;
                                            }
                                            else
                                            {
                                                skipAlreadyExtractedAsked = true;
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

                        ProjectData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract files from") + " " + name);
                        FunctionsProcess.RunProcess(_kirikiriunpacker, kiriKiriExEargs);

                        if (!ret)
                        {
                            ret = FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder.FullName, "*.*");
                        }
                        if (ret)
                        {
                            if (usecrc)
                            {
                                CleanOldCrcFiles(targetSubFolder.Name, crc);
                                if (!File.Exists(xp3Crc32Path))
                                {
                                    File.WriteAllText(xp3Crc32Path, "xp3 CRC32 control file");
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

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        private void CleanOldCrcFiles(string name, string crc)
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

        readonly string _kirikiriunpacker = ThSettings.KiriKiriToolExePath();

        protected bool PackTranslatedFilesInPatch()
        {
            var ret = false;

            try
            {
                //PatchDir
                var patchDir = Directory.CreateDirectory(Path.Combine(ProjectData.ProjectWorkDir, PatchDirName));

                if (!FunctionsFileFolder.IsInDirExistsAnyFile(patchDir.FullName))
                {
                    return false;
                }

                if (ProjectXp3List == null)
                {
                    ProjectXp3List = GetXp3Files();
                }

                string patchName = "patch" + GetLastIndexOfPatch();

                var patch = Path.Combine(ProjectData.ProjectWorkDir, patchName + ".xp3");

                if (File.Exists(patch))
                {
                    File.Delete(patch);
                }

                string kiriKiriExEargs = "-c -i \"" + patchDir.FullName + "\" -o \"" + patch + "\"";

                var le = ThSettings.LocaleEmulatorExe();
                var args = Path.GetFileName(_kirikiriunpacker) + " " + "-c -i \"" + patchDir.Name + "\" -o \"" + patchName + ".xp3" + "\"";


                //C:\WINDOWS\AppPatch\AppLoc.exe "%1" "/L0411"
                //kirikiriunpacker = @"C:\WINDOWS\AppPatch\AppLoc.exe";
                //KiriKiriEXEargs = "\"" + kirikiriunpacker + "\"" + " " + KiriKiriEXEargs + " " + "/L0411";

                //var batPath = Path.Combine(ProjectData.ProjectWorkDir, "Extract.bat");
                //var batText = @"C:\WINDOWS\AppPatch\AppLoc.exe " + kirikiriunpacker + " " + KiriKiriEXEargs;
                //File.WriteAllText(batPath, batText);

                bool arcConv = true;

                if (!arcConv && !File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(_kirikiriunpacker))))
                {
                    File.Copy(ThSettings.KiriKiriToolExePath(), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(_kirikiriunpacker)));
                    File.Copy(ThSettings.KiriKiriToolDllPath(), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ThSettings.KiriKiriToolDllPath())));
                }
                else if (arcConv && !File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ThSettings.ArcConvExePath()))))
                {
                    File.Copy(ThSettings.ArcConvExePath(), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ThSettings.ArcConvExePath())));
                    File.Copy(Path.Combine(ThSettings.ArcConvDirPath(), Path.GetFileNameWithoutExtension(ThSettings.ArcConvExePath()) + ".dat"), Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileNameWithoutExtension(ThSettings.ArcConvExePath()) + ".dat"));
                }

                var targetPatchPath = Path.Combine(ProjectData.SelectedGameDir, patchName + ".xp3");

                //kiririkiunpacker
                var setdir =
                    "cd \"" + ProjectData.ProjectWorkDir + "\""
                    ;
                var copyutil =
                    "if not exist \"" + Path.GetFileName(_kirikiriunpacker) + "\" copy \"" + _kirikiriunpacker + "\" \"" + ProjectData.ProjectWorkDir + "\\\""
                    + "\r\n"
                    + "if not exist madCHook.dll copy \"" + Path.GetDirectoryName(_kirikiriunpacker) + "\\madCHook.dll\" \"" + ProjectData.ProjectWorkDir + "\\\""
                    ;
                var delutil =
                    "if exist \"" + Path.GetFileName(_kirikiriunpacker) + "\" del \"" + Path.GetFileName(_kirikiriunpacker) + "\""
                    + "\r\n"
                    + "if exist \"" + Path.GetDirectoryName(_kirikiriunpacker) + "\\madCHook.dll\" del \"" + Path.GetDirectoryName(_kirikiriunpacker) + "\\madCHook.dll\""
                    ;
                var runpack =
                    "\"" + le + "\" " + args
                    ;
                var copypatch =
                    "if not exist \"" + targetPatchPath + "\" copy \"" + patch + "\" \"" + targetPatchPath + "\""
                    + "\r\n"
                    + "if not exist \"" + targetPatchPath + ".translation\" echo translated files >\"" + targetPatchPath + ".translation\""
                    ;

                //arc_conv
                if (arcConv)
                {
                    args = " --pack xp3 \"" + patchDir.Name + "\" \"" + patchName + ".xp3" + "\"";
                    copyutil =
                        "if not exist \"" + ThSettings.ArcConvExeName() + "\" copy \"" + ThSettings.ArcConvExePath() + "\" \"" + ProjectData.ProjectWorkDir + "\\\""
                        + "\r\n"
                        + "if not exist arc_conv.dat copy \"" + Path.GetDirectoryName(ThSettings.ArcConvExePath()) + "\\arc_conv.dat\" \"" + ProjectData.ProjectWorkDir + "\\\""
                        ;
                    delutil =
                        "if exist \"" + ThSettings.ArcConvExeName() + "\" del \"" + ThSettings.ArcConvExeName() + "\""
                        + "\r\n"
                        + "if exist \"" + Path.GetDirectoryName(ThSettings.ArcConvExePath()) + "\\arc_conv.dat\" del \"" + Path.GetDirectoryName(ThSettings.ArcConvExePath()) + "\\arc_conv.dat\""
                        ;
                    runpack = ThSettings.ArcConvExeName() + args;
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

                FunctionsProcess.RunProcess(arcConv ? ThSettings.ArcConvExePath() : le, args, ProjectData.ProjectWorkDir);

                if (!File.Exists(patch))
                {
                    Thread.Sleep(2000);
                }
                if (!File.Exists(patch))
                {
                    FunctionsProcess.RunProcess(arcConv ? ThSettings.ArcConvExePath() : le, args, ProjectData.ProjectWorkDir);

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
                    File.WriteAllText(targetPatchPath + ".translation", "translated files");//write check info txt file
                    ret = true;
                }

                if (!arcConv && File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(_kirikiriunpacker))))
                {
                    bool removed = false;
                    while (!removed)
                    {
                        try
                        {
                            File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(_kirikiriunpacker)));
                            File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ThSettings.KiriKiriToolDllPath())));
                            removed = true;
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                else if (arcConv && File.Exists(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ThSettings.ArcConvExePath()))))
                {
                    File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ThSettings.ArcConvExePath())));
                    File.Delete(Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileNameWithoutExtension(ThSettings.ArcConvExePath()) + ".dat"));
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
            string selPrefix = null;
            bool skip = false;
            bool next = false;
            for (int i = 1; i < 99; i++)
            {
                if (selPrefix == null || next)
                {
                    next = false;
                    if (selPrefix == null)
                    {
                        foreach (var prefix in KiriKiriPatchPossiblePrefixes)
                        {
                            name = "patch" + prefix + i + ".xp3";
                            if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name)))
                            {
                                //check and clean translation patch
                                if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name + ".translation")))
                                {
                                    File.Delete(Path.Combine(ProjectData.SelectedGameDir, name + ".translation"));
                                    File.Delete(Path.Combine(ProjectData.SelectedGameDir, name));
                                    return prefix + i;
                                }

                                selPrefix = prefix;//set found prefix

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

                    if (selPrefix == null)//when patch1 was not found
                    {
                        if (!File.Exists(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3")))
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
                            if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3.translation")))
                            {
                                File.Delete(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3.translation"));
                                File.Delete(Path.Combine(ProjectData.SelectedGameDir, "patch.xp3"));
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

                name = "patch" + selPrefix + i + ".xp3";
                if (!File.Exists(Path.Combine(ProjectData.SelectedGameDir, name)))
                {

                    if (IsFoundPatchWithHigherIndex(ref i, selPrefix))
                    {
                        next = true;
                        continue;
                    }
                    return selPrefix + i;
                }
                else
                {
                    //check and clean translation patch
                    if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name + ".translation")))
                    {
                        File.Delete(Path.Combine(ProjectData.SelectedGameDir, name + ".translation"));
                        File.Delete(Path.Combine(ProjectData.SelectedGameDir, name));
                        return selPrefix + i;
                    }
                }
            }

            return string.Empty;
        }

        private static bool IsFoundPatchWithHigherIndex(ref int i, string selPrefix = "")
        {
            for (int ind = 1; ind < 5; ind++)
            {
                if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, "patch" + selPrefix + (i + ind) + ".xp3")))
                {
                    i += ind - 1;
                    return true;
                }
            }
            return false;
        }

        protected string[] GetXp3Files()
        {
            List<string> dataFiles = new List<string>();
            if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, "data.xp3")))
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

                    if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name)))
                    {
                        if (File.Exists(Path.Combine(ProjectData.SelectedGameDir, name + ".translation")))
                        {
                            File.Delete(Path.Combine(ProjectData.ProjectWorkDir, name));
                            if (Directory.Exists(Path.Combine(ProjectData.ProjectWorkDir, name.Replace(".xp3", string.Empty))))
                            {
                                Directory.Delete(Path.Combine(ProjectData.ProjectWorkDir, name.Replace(".xp3", string.Empty)));
                            }
                            if (ProjectData.OpenFileMode)
                            {
                                return dataFiles.ToArray();
                            }
                            else
                            {
                                File.Delete(Path.Combine(ProjectData.SelectedGameDir, name + ".translation"));
                                File.Delete(Path.Combine(ProjectData.SelectedGameDir, name));
                            }
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

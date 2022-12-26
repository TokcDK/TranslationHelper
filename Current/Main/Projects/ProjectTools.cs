using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects
{
    internal static class ProjectTools
    {
        /// <summary>
        /// exe files *.exe
        /// </summary>
        internal static string GameExeFilter { get => "Game execute|\"*.exe\""; }
        /// <summary>
        /// return if selected file of project is exe
        /// </summary>
        /// <returns></returns>
        internal static bool IsExe(string path)
        {
            return string.Equals(Path.GetExtension(path), ".exe", StringComparison.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Get all types of inherited classes of ProjectBase class
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetListOfProjectTypes()
        {
            return GetListOfSubClasses.Inherited.GetListOfInheritedTypes(typeof(ProjectBase));
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        public static List<ProjectBase> GetListOfProjects()
        {
            return GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<ProjectBase>();
        }

        /// <summary>
        /// Check all parent dirst is exists in list of paths
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listOfPaths"></param>
        /// <returns></returns>
        public static bool IsAnyParentPathInTheList(this string path, List<string> listOfPaths)
        {
            while ((path = Path.GetDirectoryName(path)) != null)
            {
                if (listOfPaths.Contains(path)) return true;
            }

            return false;
        }
        public static List<FileInfo> GetNewestFilesList(DirectoryInfo dir, string mask = "*.*")
        {
            var newestfiles = new Dictionary<string, FileInfo>();

            foreach (var file in dir.EnumerateFiles(mask, SearchOption.AllDirectories))
            {
                var name = file.Name;
                bool isAlreadyContains = newestfiles.ContainsKey(name);
                if (isAlreadyContains)
                {
                    if (file.LastWriteTime <= newestfiles[name].LastWriteTime) continue;

                    newestfiles[name] = file;
                }
                else
                {
                    newestfiles.Add(name, file);
                }
            }

            return newestfiles.Values.ToList();
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, string DirForSearch, List<Type> formatType, string[] masks = null, bool getNewestFiles = false)
        {
            return OpenSaveFilesBase(project, new DirectoryInfo(DirForSearch), formatType, masks, getNewestFiles);
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, string DirForSearch, Type formatType, string mask = "*")
        {
            return OpenSaveFilesBase(project, new DirectoryInfo(DirForSearch), formatType, mask);
        }

        /// <summary>
        /// open save project files for several file formats/masks
        /// </summary>
        /// <param name="DirForSearch"></param>
        /// <param name="format"></param>
        /// <param name="masks"></param>
        /// <param name="newest"></param>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, DirectoryInfo DirForSearch, List<Type> format, string[] masks = null, bool getNewestFiles = false)
        {
            if (masks == null || format == null || (masks.Length != 1 && format.Count != masks.Length)) return false;

            var ret = false;
            bool hasSingleMask = masks.Length == 1;
            for (int i = 0; i < masks.Length; i++)
            {
                var mask = hasSingleMask ? masks[0] : masks[i];
                if (project.OpenSaveFilesBase(DirForSearch, format[i], mask, getNewestFiles)) ret = true;
            }

            return ret;
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, DirectoryInfo DirForSearch, Type formatType, string mask = "*", bool getNewestFiles = false, string[] exclusions = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            //if (mask == "*")
            //{
            //    mask += format.Ext();
            //}

            if (!DirForSearch.Exists) return false;

            exclusions = exclusions ?? new[] { ".bak" };//set to skip bat if exclusions is null

            var ret = false;
            var existsTables = AppData.CurrentProject.FilesContent.Tables;
            var filesList = getNewestFiles ? ProjectTools.GetNewestFilesList(DirForSearch, mask) : DirForSearch.EnumerateFiles(mask, searchOption);
            Parallel.ForEach(filesList, file =>
            {
                if (project.OpenFileMode && File.Exists(file.FullName + ".bak")) RestoreFile(file.FullName);
                if (project.SaveFileMode
                && !File.Exists(file.FullName + ".bak")
                && !file.DirectoryName.IsAnyParentPathInTheList(project.BakPaths)) // maybe parent was backed up
                    BackupFile(file.FullName);

                //skip exclusions
                if (/*exclusions != null &&*/ file.FullName.ContainsAnyFromArray(exclusions)) return;

                //ProjectData.FilePath = file.FullName;

                var format = (FormatBase)Activator.CreateInstance(formatType); // create instance of format
                format.FilePath = file.FullName;

                // check extension for case im mask was "*.*" or kind of
                if (!string.IsNullOrWhiteSpace(format.Extension) && file.Extension != format.Extension) return;

                // check if exist table has any translated
                if (project.SaveFileMode && existsTables.Contains(format.FileName) && !format.FileName.HasAnyTranslated()) return;

                AppData.Main.ProgressInfo(true, (project.OpenFileMode ? T._("Opening") : T._("Saving")) + " " + file.Name);

                bool isOpenSuccess = false;
                try
                {
                    if (project.OpenFileMode ? (isOpenSuccess = format.Open()) : format.Save()) ret = true;
                }
                catch { }

                // add to bak paths for default backup
                if (project.OpenFileMode && isOpenSuccess
                && !project.BakPaths.Contains(file.FullName)
                && !file.FullName.IsAnyParentPathInTheList(project.BakPaths))
                    project.BakPaths.Add(file.FullName);
            });

            AppData.Main.ProgressInfo(false);

            return ret;
        }

        /// <summary>
        /// restore selected dir from it buckup .bak copy
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool RestoreDir(string dir)
        {
            try
            {

                if (!Directory.Exists(dir + ".bak")) return false;

                bool tmp = false;
                if (Directory.Exists(dir))
                {
                    tmp = true;

                    if (Directory.Exists(dir + ".tmp"))
                    {
                        new DirectoryInfo(dir + ".tmp").Attributes = FileAttributes.Normal;
                        Directory.Delete(dir + ".tmp", true);
                    }

                    Directory.Move(dir, dir + ".tmp");
                }
                Directory.Move(dir + ".bak", dir);
                if (tmp && Directory.Exists(dir + ".tmp") && Directory.Exists(dir))
                {
                    new DirectoryInfo(dir + ".tmp").Attributes = FileAttributes.Normal;
                    Directory.Delete(dir + ".tmp", true);
                    return true;
                }
                else if (!tmp && Directory.Exists(dir))
                {
                    return true;
                }
                return false;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// restore selected file from it buckup .bak copy
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool RestoreFile(string file)
        {
            AppData.Main.ProgressInfo(true, T._("restore") + ":" + Path.GetFileName(file));

            try
            {
                if (File.Exists(file + ".bak"))
                {
                    bool tmp = false;
                    if (File.Exists(file))
                    {
                        tmp = true;

                        if (File.Exists(file + ".tmp"))
                        {
                            new FileInfo(file + ".tmp").Attributes = FileAttributes.Normal;
                            File.Delete(file + ".tmp");
                        }

                        File.Move(file, file + ".tmp");
                    }
                    File.Move(file + ".bak", file);
                    if (tmp && File.Exists(file + ".tmp") && File.Exists(file))
                    {
                        new FileInfo(file + ".tmp").Attributes = FileAttributes.Normal;
                        File.Delete(file + ".tmp");
                        AppData.Main.ProgressInfo(false);
                        return true;
                    }
                    else if (!tmp && File.Exists(file))
                    {
                        AppData.Main.ProgressInfo(false);
                        return true;
                    }
                }
            }
            catch
            {
            }

            AppData.Main.ProgressInfo(false);
            return false;
        }

        /// <summary>
        /// make buckup .bak copy of selected paths
        /// </summary>
        /// <param name="paths">file paths</param>
        /// <param name="bak">true = backup, false = restore</param>
        /// <returns>true if was processed atleast one file\dir</returns>
        internal static bool BackupRestorePaths(IEnumerable<string> paths, bool bak = true)
        {
            if (paths == null) return false;

            var ret = false;

            var added = new HashSet<string>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                string fullPath = (path.StartsWith(@".\") || path.StartsWith(@"..\")) ? Path.GetFullPath(Path.Combine(AppData.CurrentProject.SelectedDir, path)) : path;

                if (string.IsNullOrWhiteSpace(fullPath) || added.Contains(fullPath)) continue;

                added.Add(fullPath);//add path to backuped

                var target = fullPath.EndsWith(".bak") ? fullPath.Remove(fullPath.Length - 4, 4) : fullPath;
                if (bak)
                {
                    if ((File.Exists(target) && BackupFile(target)) || (Directory.Exists(target) && BackupDir(target)))
                        ret = true;
                }
                else if ((File.Exists(target + ".bak") && RestoreFile(target)) || (Directory.Exists(target + ".bak") && RestoreDir(target)))
                {
                    ret = true;
                }
            }

            return ret;
        }
        /// <summary>
        /// make buckup .bak copy of selected dir
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool BackupDir(string dir)
        {
            try
            {
                if (Directory.Exists(dir + ".bak")) RestoreDir(dir);

                if (!Directory.Exists(dir + ".bak") && Directory.Exists(dir)) dir.CopyAll(dir + ".bak");
            }
            catch
            {
            }
            return Directory.Exists(dir + ".bak");
        }

        /// <summary>
        /// make buckup .bak copy of selected files
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool BackupFile(string[] filePaths)
        {
            var ret = false;
            foreach (var file in filePaths)
            {
                if (!BackupFile(file)) continue;

                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// make buckup .bak copy of selected file
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool BackupFile(string file)
        {
            AppData.Main.ProgressInfo(true, T._("backup") + ":" + Path.GetFileName(file));

            try
            {
                if (File.Exists(file + ".bak")) RestoreFile(file);

                if (File.Exists(file) && !File.Exists(file + ".bak")) File.Copy(file, file + ".bak");
            }
            catch
            {
            }
            AppData.Main.ProgressInfo(false);
            return File.Exists(file + ".bak");
        }
    }
}

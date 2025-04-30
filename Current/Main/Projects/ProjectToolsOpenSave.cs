using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranslationHelper;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects
{
    internal static class ProjectToolsOpenSave
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, string DirForSearch, List<Type> formatType, string[] masks = null, bool getNewestFiles = false)
        {
            return project.OpenSaveFilesBase(new DirectoryInfo(DirForSearch), formatType, masks, getNewestFiles);
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, string DirForSearch, Type formatType, string mask = "*")
        {
            return project.OpenSaveFilesBase(new DirectoryInfo(DirForSearch), formatType, mask);
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
            if (masks == null || format == null || masks.Length != 1 && format.Count != masks.Length) return false;

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
            var filesList = getNewestFiles ? GetNewestFilesList(DirForSearch, mask) : DirForSearch.EnumerateFiles(mask, searchOption);
            Parallel.ForEach(filesList, file =>
            {
                if (File.Exists(file.FullName + ".skipme") || File.Exists(file.FullName + ".skip")) return;

                if (project.OpenFileMode && File.Exists(file.FullName + ".bak")) ProjectToolsBackup.RestoreFile(file.FullName);
                if (project.SaveFileMode
                && !File.Exists(file.FullName + ".bak")
                && !file.DirectoryName.IsAnyParentPathInTheList(project.BakPaths)) // maybe parent was backed up
                    ProjectToolsBackup.BackupFile(file.FullName);

                //skip exclusions
                if (/*exclusions != null &&*/ file.FullName.ContainsAnyFromArray(exclusions)) return;

                //ProjectData.FilePath = file.FullName;

                var format = (FormatBase)Activator.CreateInstance(formatType); // create instance of format
                format.FilePath = file.FullName;

                // check extension for case im mask was "*.*" or kind of
                if (!string.IsNullOrWhiteSpace(format.Extension) && file.Extension != format.Extension) return;

                Logger.Info((project.OpenFileMode ? T._("Opening") : T._("Saving")) + " " + file.Name);

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

            

            return ret;
        }
    }
}
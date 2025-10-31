using Microsoft.Scripting.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            // Return values directly instead of creating an intermediate list
            return new List<FileInfo>(newestfiles.Values);
        }

        /// <summary>
        /// Check all parent directories are present in the list of paths
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
            if (!DirForSearch.Exists) return false;

            exclusions = exclusions ?? new[] { ".bak" };// set to skip .bak if exclusions is null

            // Get the list of files based on the parameters
            var filesList = getNewestFiles ? GetNewestFilesList(DirForSearch, mask) : DirForSearch.EnumerateFiles(mask, searchOption);

            // Filter out exclusions first to avoid unnecessary processing
            var filteredFiles = filesList
                .Where(file => !file.FullName.ContainsAnyFromArray(exclusions))
                .Select(file => (info: file, type: formatType))
                .ToArray();

            // Delegate to the second overload to process the files
            return filteredFiles.Length > 0 && project.OpenSaveFilesBase(filteredFiles);
        }
        public static bool OpenSaveFilesBase(this ProjectBase project, FileInfo info, Type type)
        {
            return project.OpenSaveFilesBase(new[] { (info, type) });
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        public static bool OpenSaveFilesBase(this ProjectBase project, IEnumerable<(FileInfo info, Type type)> filesList)
        {
            if (filesList == null)
                return false;

            var ret = false;
            bool isOpenMode = project.OpenFileMode;
            var bakPaths = project.BakPaths;

            Parallel.ForEach(filesList, file =>
            {
                var fileInfo = file.info;
                if (fileInfo == null)
                    return;

                // Check if the file is translated (for save mode only)
                if (!IsTheSaveModeFileTranslated(project, fileInfo.Name))
                    return;

                string fullName = fileInfo.FullName;
                string directoryName = fileInfo.DirectoryName;

                // Skip files marked with .skipme or .skip
                if (File.Exists(fullName + ".skipme") || File.Exists(fullName + ".skip"))
                    return;

                try
                {
                    // Handling backup files
                    if (isOpenMode)
                    {
                        // In open mode, restore file from .bak if it exists
                        if (File.Exists(fullName + ".bak"))
                            ProjectToolsBackup.RestoreFile(fullName);
                    }
                    else
                    {
                        // In save mode, create .bak if it does not exist and the directory is not among backup paths
                        bool backupExists = File.Exists(fullName + ".bak");
                        bool isInBackupPaths = directoryName != null && directoryName.IsAnyParentPathInTheList(bakPaths);

                        if (!backupExists && !isInBackupPaths)
                            ProjectToolsBackup.BackupFile(fullName);
                    }

                    // Create instance of format
                    if (!(Activator.CreateInstance(file.type, project) is FormatBase format))
                        return;

                    // Check file extension
                    if (!string.IsNullOrWhiteSpace(format.Extension) &&
                        !fileInfo.Extension.Equals(format.Extension, StringComparison.OrdinalIgnoreCase))
                        return;

                    // Logging operation
                    string operationName = isOpenMode ? T._("Opening") : T._("Saving");
                    Logger.Info($"{operationName} {fileInfo.Name}");

                    // Process file based on mode
                    bool success = false;
                    if (isOpenMode)
                    {
                        success = format.Open(fullName);
                        if (success)
                            ret = true;

                        // Add path to backup list if the file is opened successfully
                        if (success && !bakPaths.Contains(fullName) && !fullName.IsAnyParentPathInTheList(bakPaths))
                        {
                            lock (bakPaths)
                            {
                                if (!bakPaths.Contains(fullName))
                                    bakPaths.Add(fullName);
                            }
                        }
                    }
                    else if (format.Save(fullName))
                    {
                        ret = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, $"Error while {(isOpenMode ? "opening" : "saving")} file: {fullName}");
                }
            });

            return ret;
        }

        private static bool IsTheSaveModeFileTranslated(ProjectBase project, string tableName)
        {
            if (project.OpenFileMode)
            {
                return true;
            }

            if (!project.FilesContent.Tables.Contains(tableName))
            {
                // If the file is already loaded in the project, skip it
                return false;
            }
            else
            {
                var table = project.FilesContent.Tables[tableName];

                if (table == null || table.Rows.Count == 0 || !table.HasAnyTranslated())
                {
                    // If the file is already loaded in the project, skip it
                    return false;
                }
            }

            return true;
        }
    }
}
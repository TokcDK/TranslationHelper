using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects
{
    internal static class ProjectToolsBackup
    {
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
            FunctionsUI.ProgressInfo(true, T._("backup") + ":" + Path.GetFileName(file));

            try
            {
                if (File.Exists(file + ".bak")) RestoreFile(file);

                if (File.Exists(file) && !File.Exists(file + ".bak")) File.Copy(file, file + ".bak");
            }
            catch
            {
            }
            FunctionsUI.ProgressInfo(false);
            return File.Exists(file + ".bak");
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
            FunctionsUI.ProgressInfo(true, T._("restore") + ":" + Path.GetFileName(file));

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
                        FunctionsUI.ProgressInfo(false);
                        return true;
                    }
                    else if (!tmp && File.Exists(file))
                    {
                        FunctionsUI.ProgressInfo(false);
                        return true;
                    }
                }
            }
            catch
            {
            }

            FunctionsUI.ProgressInfo(false);
            return false;
        }
    }
}
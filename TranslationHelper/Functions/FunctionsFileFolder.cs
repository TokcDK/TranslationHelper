﻿using System.IO;
using System.Linq;

namespace TranslationHelper.Main.Functions
{
    internal class FunctionsFileFolder
    {
        /// <summary>
        /// True if the file is using by other program.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool FileInUse(string path)
        {
            //FileStream stream = null;

            try
            {
                using (FileStream stream = new FileInfo(path).Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                }
            }
            catch (IOException)
            {
                return true;
            }
            //finally
            //{
            //    if (stream != null)
            //    {
            //        stream.Close();
            //    }
            //}

            return false;
        }

        /// <summary>
        /// add index to end of path if file is exists
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        internal static string NewFilePathPlusIndex(string FilePath)
        {
            int index = 0;
            string newFilePath = FilePath;
            while (File.Exists(newFilePath))
            {
                index++;
                newFilePath = FilePath + index;
            }
            return newFilePath;
        }

        internal static bool IsInDirExistsAnyFile(string FilderPath, string mask = "*", bool SearchFiles = true, bool Recursive = false)
        {
            int cnt = 0;
            if (SearchFiles)
            {
                foreach (var file in Directory.EnumerateFiles(FilderPath, mask, Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    cnt++;
                    if (cnt > 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var folder in Directory.EnumerateDirectories(FilderPath, mask, Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    cnt++;
                    if (cnt > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool GetAnyFileWithTheNameExist(string[] array, string name)
        {
            //исключение имен с недопустимыми именами для файла или папки
            //http://www.cyberforum.ru/post5599483.html
            if (name.Length == 0 || FunctionsString.IsMultiline(name) || name.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                //MessageBox.Show("GetAnyFileWithTheNameExist return false because invalid! name=" + name);
                return false;
            }
            if (array.Contains(name))
            {
                return true;
            }

            //MessageBox.Show("Properties.Settings.Default.THSelectedDir=" + Properties.Settings.Default.THSelectedDir.Replace("\\www\\data", "\\www") + "\r\n, name=" + name + "\r\n, Count=" + Directory.EnumerateFiles(Properties.Settings.Default.THSelectedDir, name + ".*", SearchOption.AllDirectories).Count());
            //foreach (var f in Directory.EnumerateFiles(Properties.Settings.Default.THSelectedDir, name + ".*", SearchOption.AllDirectories))
            //{
            //    //MessageBox.Show("GetAnyFileWithTheNameExist Returns True! name="+ name);
            //    return true;
            //}
            //MessageBox.Show("GetAnyFileWithTheNameExist Returns False! name=" + name);
            return false;
        }
    }
}

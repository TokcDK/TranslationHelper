using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using TranslationHelper.Extensions;

namespace TranslationHelper.Main.Functions
{
    internal class FunctionsFileFolder
    {
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);

        //https://stackoverflow.com/a/757925
        //быстро проверить, пуста ли папка
        public static bool CheckDirectoryNullOrEmpty_Fast(string path, string Mask = "*"/*, string[] exclusions = null*/)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)
                //|| (DirectoryInfoExtensions.IsSymbolicLink(new DirectoryInfo(path)) && !DirectoryInfoExtensions.IsSymbolicLinkValid(new DirectoryInfo(path)))
                )
            {
                return true; //return true if path is empty
                //throw new ArgumentNullException(path);
            }

            //if (DirectoryInfoExtensions.IsSymbolicLink(new DirectoryInfo(path)) && !DirectoryInfoExtensions.IsSymbolicLinkValid(new DirectoryInfo(path)))
            //{
            //    return true;
            //}

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Mask;
            }
            else
            {
                path += Path.DirectorySeparatorChar + Mask;
            }

            var findHandle = FindFirstFile(path, out WIN32_FIND_DATA findData);

            if (findHandle != INVALID_HANDLE_VALUE)
            {
                try
                {
                    bool empty = true;
                    do
                    {
                        if (findData.cFileName != "." && findData.cFileName != ".."/* && !ManageStrings.IsStringContainsAnyExclusion(findData.cFileName, exclusions)*/)
                        {
                            empty = false;
                        }
                    } while (empty && FindNextFile(findHandle, out findData));

                    return empty;
                }
                finally
                {
                    FindClose(findHandle);
                }
            }

            throw new Exception("Failed to get directory first file",
                Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            //throw new DirectoryNotFoundException();
        }

        internal static string GetMD5(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder BuildedMD5Value = new StringBuilder();
                    int retValLength = retVal.Length;
                    for (int i = 0; i < retValLength; i++)
                    {
                        BuildedMD5Value.Append(retVal[i].ToString("x2", CultureInfo.InvariantCulture));
                    }
                    return BuildedMD5Value.ToString();
                }
            }
        }

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
            if (name.Length == 0 || name.IsMultiline() || name.Intersect(Path.GetInvalidFileNameChars()).Any())
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

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.SimpleHelpers;

namespace TranslationHelper.Main.Functions
{
    internal static class FunctionsFileFolder
    {
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WIN32_FIND_DATA
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

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                path += Mask;
            }
            else
            {
                path += Path.DirectorySeparatorChar + Mask;
            }

            var findHandle = NativeMethods.FindFirstFile(path, out WIN32_FIND_DATA findData);

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
                    } while (empty && NativeMethods.FindNextFile(findHandle, out findData));

                    return empty;
                }
                finally
                {
                    NativeMethods.FindClose(findHandle);
                }
            }
            return true;

            //throw new Exception("Failed to get directory first file",
            //    Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            //throw new DirectoryNotFoundException();
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

        internal static bool IsInDirExistsAnyFile(string FolderPath, string mask = "*", bool SearchFiles = true, bool Recursive = false)
        {
            return FolderPath.IsTheDirContainsFiles(mask, SearchFiles, Recursive);
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

        /// <summary>
        /// Determines a text file's encoding
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            //https://github.com/khalidsalomao/SimpleHelpers.Net/blob/master/docs/fileencoding.md
            var det = new FileEncoding();
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                if (stream.Length == 0)
                {
                    return Encoding.ASCII;
                }
                det.Detect(stream);
            }
            var encoding = det.Complete();
            var hasbom = det.HasByteOrderMark;
            if (encoding == Encoding.UTF8 && !hasbom)
            {
                return new UTF8Encoding(hasbom);//UTF8 no bom
            }
            return encoding;
        }
    }
}

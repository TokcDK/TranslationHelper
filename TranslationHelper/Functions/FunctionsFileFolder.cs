using System.IO;
using System.Linq;
using System.Text;
using TranslationHelper.Extensions;
using TranslationHelper.SimpleHelpers;

namespace TranslationHelper.Main.Functions
{
    internal static class FunctionsFileFolder
    {
        /// <summary>
        /// true when input text has invalid chars and cant be used in dir/file path
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static bool HasInvalidChars(string text)
        {
            return text.Intersect(Path.GetInvalidFileNameChars()).Any();//invalid file/folder name;
        }

        /// <summary>
        /// True if the file is using by other program.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool FileInUse(string path)
        {
            //FileStream stream = null;

            if (!File.Exists(path)) return false;

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

        internal static bool IsInDirExistsAnyFile(string FolderPath, string mask = "*", bool recursive = true)
        {
            return FolderPath.ContainsFiles(mask, recursive: recursive);
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

            //MessageBox.Show("ProjectData.CurrentProject.SelectedDir=" + ProjectData.CurrentProject.SelectedDir.Replace("\\www\\data", "\\www") + "\r\n, name=" + name + "\r\n, Count=" + Directory.EnumerateFiles(ProjectData.CurrentProject.SelectedDir, name + ".*", SearchOption.AllDirectories).Count());
            //foreach (var f in Directory.EnumerateFiles(ProjectData.CurrentProject.SelectedDir, name + ".*", SearchOption.AllDirectories))
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

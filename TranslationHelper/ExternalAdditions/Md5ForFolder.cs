using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TranslationHelper.ExternalAdditions
{
    /// <summary>
    /// https://stackoverflow.com/questions/3625658/creating-hash-for-folder
    /// </summary>
    class Md5ForFolder
    {
        /// <summary>
        /// hashes all file (relative) paths and contents, and correctly handles file ordering
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string CreateMd5ForFolder(string path)
        {
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string file = files[i];

                    // hash path
                    string relativePath = file.Substring(path.Length + 1);
                    byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower(CultureInfo.InvariantCulture));
                    md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                    // hash contents
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if (i == files.Count - 1)
                        md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                    else
                        md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }

                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower(CultureInfo.InvariantCulture);
            }
        }
    }
}

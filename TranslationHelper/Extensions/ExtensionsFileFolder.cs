﻿using CheckForEmptyDir;
using Soft160.Data.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsFileFolder
    {
        /// <summary>
        /// returns true if dir is empty
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool IsEmpty(this DirectoryInfo dir)
        {
            return dir.FullName.IsNullOrEmptyDirectory();
        }

        /// <summary>
        /// True if directory is exists and contains No files
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool HasNoFiles(this DirectoryInfo dir, string mask = "*", bool recursive = true)
        {
            return dir.IsNullOrEmptyDirectory(mask, searchForFiles: true, searchForDirs: false, recursive: recursive);
        }


        /// <summary>
        /// True if directory is exists and contains any files
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool HasAnyFiles(this DirectoryInfo dir, string mask = "*", bool recursive = true)
        {
            return !dir.IsNullOrEmptyDirectory(mask, searchForFiles: true, searchForDirs: false, recursive: recursive);
        }

        /// <summary>
        /// True if directory is exists and contains any files
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static bool HasAnyDirs(this DirectoryInfo dir, string mask = "*")
        {
            return !dir.IsNullOrEmptyDirectory(mask, searchForFiles: false, searchForDirs: true);
        }

        /// <summary>
        /// true if contains any file
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="mask"></param>
        /// <param name="SearchFiles"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        internal static bool ContainsFiles(this string folderPath, string mask = "*.*", bool recursive = true)
        {
            return !folderPath.IsNullOrEmptyDirectory(mask, searchForFiles: true, searchForDirs: false, recursive: recursive);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5351:Не используйте взломанные алгоритмы шифрования", Justification = "<Ожидание>")]
        internal static string GetMd5(this string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "-1";
            }

            try
            {
                using (var file = new FileStream(filePath, FileMode.Open))
                {
                    using (var md5 = new MD5CryptoServiceProvider())
                    {
                        byte[] retVal = md5.ComputeHash(file);
                        file.Close();

                        var buildedMd5Value = new StringBuilder();
                        var retValLength = retVal.Length;
                        for (int i = 0; i < retValLength; i++)
                        {
                            buildedMd5Value.Append(retVal[i].ToString("x2"));
                        }
                        return buildedMd5Value.ToString();
                    }
                }
            }
            catch
            {
                return "-1";
            }
        }

        internal static string GetCrc32(this string fileName, bool showprogress = false, System.Windows.Forms.ProgressBar pb = null)
        {
            //https://stackoverflow.com/a/57450238
            using (var crc32 = new CRCServiceProvider())
            {
                showprogress = showprogress && pb != null;
                string hash = string.Empty;
                using (var fs = File.Open(fileName, FileMode.Open))
                {
                    var array = crc32.ComputeHash(fs);
                    var arrayLength = array.Length;
                    System.Windows.Forms.ProgressBarStyle oldpbstyle = System.Windows.Forms.ProgressBarStyle.Marquee;
                    if (showprogress)
                    {
                        pb.Invoke((Action)(() => pb.Maximum = arrayLength));
                        oldpbstyle = pb.Style;
                        pb.Invoke((Action)(() => pb.Style = System.Windows.Forms.ProgressBarStyle.Blocks));
                        pb.Invoke((Action)(() => pb.Visible = true));
                    }
                    for (int i = 0; i < arrayLength; i++)
                    {
                        if (showprogress && pb.Value <= pb.Maximum)
                        {
                            pb.Invoke((Action)(() => pb.Value = i));
                        }
                        hash += array[i].ToString("x2")/*.ToLowerInvariant()*/;
                    }

                    if (showprogress && pb.Value > 0)
                    {
                        pb.Invoke((Action)(() => pb.Value = 0));
                        pb.Invoke((Action)(() => pb.Style = oldpbstyle));
                        pb.Invoke((Action)(() => pb.Visible = false));
                    }
                }

                return hash;
            }
        }

        //https://stackoverflow.com/questions/22152900/wrong-xor-decryption
        /// <summary>
        /// Added mainly for nscript project<br>Using selected key will xor not xored bytes and unxor xored bytes
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static byte[] XorUnxor(this byte[] text, byte[] key = null)
        {
            key = key ?? new byte[] { 0x84 };//nscript.dat key

            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte)(text[i] ^ key[i % key.Length]);
            }
            return xor;
        }
    }
}

using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsFileFolder
    {
        /// <summary>
        /// true if contains any file
        /// </summary>
        /// <param name="FilderPath"></param>
        /// <param name="mask"></param>
        /// <param name="SearchFiles"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        internal static bool IsTheDirContainsFiles(this string FilderPath, string mask = "*", bool SearchFiles = true, bool Recursive = false)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5351:Не используйте взломанные алгоритмы шифрования", Justification = "<Ожидание>")]
        internal static string GetMD5(this string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "-1";
            }

            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
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
            catch
            {
                return "-1";
            }
        }
    }
}

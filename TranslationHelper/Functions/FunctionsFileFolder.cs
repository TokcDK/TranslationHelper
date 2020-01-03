using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Main.Functions
{
    internal class FunctionsFileFolder
    {
        internal static bool IsInDirExistsAnyFile(string FilderPath, string mask = "*", bool SearchFiles=true, bool Recursive = false)
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

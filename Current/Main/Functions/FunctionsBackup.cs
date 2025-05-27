using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Functions
{
    internal class FunctionsBackup
    {
        internal static void ShiftToBackups(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var ext = Path.GetExtension(path);

                int maxBakIndex = 9;
                for (int i = maxBakIndex; i >= 1; i--)
                {
                    MoveFile(dir, name, ext, i, maxBakIndex);
                }

                System.IO.File.Move(path, Path.Combine(dir, name + _saveFileBakSuffix + 1 + ext));
            }
            catch (Exception ex)
            {
            }
        }

        readonly static string _saveFileBakSuffix = "_bak";
        private static void MoveFile(string dir, string name, string ext, int index, int maxBakIndex)
        {
            var path = Path.Combine(dir, name + _saveFileBakSuffix + index + ext);
            if (!System.IO.File.Exists(path)) return;

            if (index == maxBakIndex)
            {
                System.IO.File.Delete(path);
            }
            else
            {
                System.IO.File.Move(path, Path.Combine(dir, name + _saveFileBakSuffix + (index + 1) + ext));
            }
        }
    }
}

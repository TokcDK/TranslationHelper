using System;
using System.Runtime.InteropServices;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindFirstFile(string lpFileName, out FunctionsFileFolder.WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern bool FindNextFile(IntPtr hFindFile, out FunctionsFileFolder.WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        internal static extern bool FindClose(IntPtr hFindFile);
    }
}

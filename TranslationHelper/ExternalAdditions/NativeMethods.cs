using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.ExternalAdditions
{
    internal class NativeMethods
    {
        //THCreateSymlink
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, THCreateSymlink.SymbolicLink dwFlags);

        //ControlHelper
        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        //GDI
        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        //THSearch
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //IniFiles
        //[DllImport("kernel32", CharSet = CharSet.Unicode)] // Подключаем kernel32.dll и описываем его функцию WritePrivateProfilesString
        //internal static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        //[DllImport("kernel32", CharSet = CharSet.Unicode)] // Еще раз подключаем kernel32.dll, а теперь описываем функцию GetPrivateProfileString
        //internal static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
    }
}

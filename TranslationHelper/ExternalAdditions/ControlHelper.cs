using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//источник
//https://stackoverflow.com/questions/778095/windows-forms-using-backgroundimage-slows-down-drawing-of-the-forms-controls
namespace TranslationHelper
{
    public static class ControlHelper
    {
        #region Redraw Suspend/Resume
        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private const int WM_SETREDRAW = 0xB;

        public static void SuspendDrawing(this Control target)
        {
            if (target != null)
            {
                _ = SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
            }
        }

        public static void ResumeDrawing(this Control target) { ResumeDrawing(target, true); }
        public static void ResumeDrawing(this Control target, bool redraw)
        {
            if (target != null)
            {
                int errorcode = SendMessage(target.Handle, WM_SETREDRAW, 1, 0);

                if (redraw && errorcode == 0)
                {
                    target.Refresh();
                }
            }
        }
        #endregion
    }
}

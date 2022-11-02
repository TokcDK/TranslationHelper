using System.Windows.Forms;
using TranslationHelper.ExternalAdditions;

//источник
//https://stackoverflow.com/questions/778095/windows-forms-using-backgroundimage-slows-down-drawing-of-the-forms-controls
namespace TranslationHelper
{
    public static class ControlHelper
    {
        #region Redraw Suspend/Resume
        private const int WM_SETREDRAW = 0xB;

        public static void SuspendDrawing(this Control target)
        {
            if (target != null)
            {
                _ = NativeMethods.SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
            }
        }

        public static void ResumeDrawing(this Control target) { ResumeDrawing(target, true); }
        public static void ResumeDrawing(this Control target, bool redraw)
        {
            if (target != null)
            {
                _ = NativeMethods.SendMessage(target.Handle, WM_SETREDRAW, 1, 0);

                if (redraw)
                {
                    target.Refresh();
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Themes the scroll bars, which no colour reaches.
    /// <para>
    /// A scroll bar is not part of the area a control paints. A text box, a rich text box, a list box,
    /// a tree view and a panel that scrolls keep theirs in the non-client area of their window, and a
    /// data grid keeps its two as child windows of its own. Either way Windows draws the part from the
    /// theme the window was given, so <c>BackColor</c> and <c>ForeColor</c> never touch it, and it
    /// stays white on a dark window — the one light patch left once everything else has been painted.
    /// </para>
    /// <para>
    /// The theme of the window itself can be changed, and that is the whole of this class: the window
    /// is asked to use the theme Windows uses for the scroll bars of its own dark windows. Measured on
    /// Windows 10 build 19044 that takes every scroll bar in a window to #171717; it changes nothing
    /// else, with a combo box's arrow, a flat button and a label pixel for pixel identical before and
    /// after; it reaches the drop-down list of a combo box as well, although that is a pop-up window
    /// of its own; and asking for the default theme again puts every one of them back, so the light
    /// theme is left exactly as the system drew it.
    /// </para>
    /// <para>
    /// This is deliberately not an <see cref="IControlStyler"/>. <see cref="ControlStylerRegistry"/>
    /// lets the first styler that recognises a control paint it, so a styler here would take the place
    /// of the one that paints the control rather than work beside it. It is the same walk over the
    /// same tree with a different subject: the window, rather than what is painted inside it.
    /// </para>
    /// </summary>
    internal static class NativeScrollBarTheme
    {
        /// <summary>
        /// The theme Windows uses for the scroll bars of its own dark windows. It is not one of the
        /// documented theme names, but it is the one Explorer itself asks for.
        /// </summary>
        private const string DarkScrollBarTheme = "DarkMode_Explorer";

        /// <summary>
        /// The word to look for in the class name of a scroll bar the framework created for a control.
        /// The full name is <c>WindowsForms10.SCROLLBAR.app.0.…</c>; the parts around this one are
        /// generated per process, so this is the only part that can be relied on.
        /// </summary>
        private const string ScrollBarClass = "SCROLLBAR";

        private delegate bool EnumWindowsProc(IntPtr window, IntPtr parameter);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr window, string subAppName, string subIdList);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr parent, EnumWindowsProc callback, IntPtr parameter);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr window, StringBuilder className, int maxCount);

        /// <summary>
        /// Themes the scroll bars of <paramref name="root"/> and of everything inside it. Safe to call
        /// again after any change of theme, and safe on a control that has no scroll bar at all.
        /// </summary>
        internal static void Apply(Control root, ITheme theme)
        {
            if (root == null || root.IsDisposed)
            {
                return;
            }

            ApplyTo(root, theme);

            foreach (Control child in root.Controls)
            {
                Apply(child, theme);
            }
        }

        /// <summary>
        /// Themes the scroll bars of one control: the one in its non-client area, and the ones the
        /// framework made as windows of their own under it, which is how a data grid owns its two.
        /// <para>
        /// A control whose window does not exist yet is waited for rather than skipped. A control added
        /// to a window that is already on screen gets its window a moment after it is added, and
        /// leaving it out would leave exactly the white scroll bar this class exists to remove.
        /// </para>
        /// </summary>
        internal static void ApplyTo(Control control, ITheme theme)
        {
            if (control == null || control.IsDisposed || !CanHaveScrollBar(control))
            {
                return;
            }

            if (!control.IsHandleCreated)
            {
                // Both uses of the same method group make equal delegates, so this cannot pile up
                // handlers however many times the theme is applied.
                control.HandleCreated -= OnHandleCreated;
                control.HandleCreated += OnHandleCreated;
                return;
            }

            // Null asks for the default theme again, which is how the light theme is restored.
            string name = theme.IsDark ? DarkScrollBarTheme : null;

            SetWindowTheme(control.Handle, name, null);

            foreach (var scrollBar in ScrollBarWindows(control))
            {
                SetWindowTheme(scrollBar, name, null);
            }
        }

        /// <summary>
        /// The shapes that own a scroll bar. Everything else is left alone: the theme a window is given
        /// applies to every part of it, so a control that has no scroll bar has no reason to be asked
        /// for one, even though measuring says nothing else would change.
        /// </summary>
        private static bool CanHaveScrollBar(Control control)
        {
            if (control is TextBoxBase || control is ListControl)
            {
                return true;
            }

            if (control is TreeView || control is ListView || control is DataGridView || control is ScrollBar)
            {
                return true;
            }

            // A container scrolls only when it is told to: a panel, a tab page, a split container's
            // panel, a user control, a table layout panel, a window.
            return control is ScrollableControl scrollable && scrollable.AutoScroll;
        }

        private static void OnHandleCreated(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control == null)
            {
                return;
            }

            control.HandleCreated -= OnHandleCreated;

            ApplyTo(control, ThemeManager.Instance.CurrentTheme);
        }

        /// <summary>The scroll bars the framework made as windows of their own under this control.</summary>
        private static List<IntPtr> ScrollBarWindows(Control control)
        {
            var found = new List<IntPtr>();

            EnumChildWindows(control.Handle, (window, parameter) =>
            {
                if (IsScrollBar(window))
                {
                    found.Add(window);
                }

                return true;
            }, IntPtr.Zero);

            return found;
        }

        private static bool IsScrollBar(IntPtr window)
        {
            var name = new StringBuilder(256);

            if (GetClassName(window, name, name.Capacity) == 0)
            {
                return false;
            }

            return name.ToString().IndexOf(ScrollBarClass, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}

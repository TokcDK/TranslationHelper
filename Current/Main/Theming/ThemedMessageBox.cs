using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// The message box the application shows, which follows the theme.
    /// <para>
    /// The framework's own dialog cannot be darkened. Its body is painted by the theme it was given,
    /// and neither of the two calls that look as though they should change that — asking the window
    /// manager to draw a dark caption, and pointing the dialog at the dark explorer theme — has any
    /// effect on it; both were measured against the real dialog and left the body white. The
    /// undocumented route the shell itself uses needs a hook on window creation to reach the dialog
    /// before it is themed, which is not a thing to leave in an application. So the dark theme draws
    /// its own.
    /// </para>
    /// <para>
    /// The light theme does not. It hands the call straight to the framework's dialog, which means the
    /// application looks exactly as it always did whenever the light theme is chosen, down to the
    /// last detail of a dialog the framework draws and this class does not. Only the dark theme, where
    /// there is no framework dialog to match anyway, pays for a dialog of the application's own.
    /// </para>
    /// <para>
    /// Every overload the application calls is here, so a call site reads the same as it did and only
    /// the name of the type it resolves to has changed.
    /// </para>
    /// </summary>
    internal static class ThemedMessageBox
    {
        internal static DialogResult Show(string text)
        {
            return Show(text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        internal static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        internal static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        internal static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Shows the message, in a window of the application's own when the dark theme is on and in the
        /// framework's dialog otherwise.
        /// </summary>
        internal static DialogResult Show(
            string text,
            string caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            if (!ThemeManager.Instance.CurrentTheme.IsDark)
            {
                return System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon, defaultButton);
            }

            using (var dialog = new ThemedMessageBoxForm(text, caption, buttons, icon, defaultButton))
            {
                // Owned by whatever window is in front, so that the dialog is centred on it and holds
                // it disabled while it is up, exactly as the framework's dialog would.
                var owner = Form.ActiveForm;

                return owner == null || owner == dialog
                    ? dialog.ShowDialog()
                    : dialog.ShowDialog(owner);
            }
        }
    }
}

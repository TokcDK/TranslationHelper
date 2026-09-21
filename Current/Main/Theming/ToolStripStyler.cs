using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a menu strip or a tool strip.
    /// <para>
    /// Most of a menu is painted through <see cref="ThemedToolStripRenderer"/>, which the theme installs
    /// for the whole application. The colours set here cover what the renderer leaves to the control
    /// itself, such as its padding. Clearing them for the light theme is right, because a strip that
    /// was never given a colour takes the colour of the window it is on.
    /// </para>
    /// </summary>
    internal sealed class ToolStripStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is ToolStrip;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.MenuBack);
            control.ForeColor = state.Colour("ForeColor", theme, theme.MenuText);

            control.Invalidate();
        }
    }
}

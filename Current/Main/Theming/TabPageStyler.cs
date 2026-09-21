using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints the page of a tab, which is a container that has to be told not to use the visual style.
    /// <para>
    /// A page that asks for the visual style background ignores the colour set on it, so the request
    /// has to be dropped for the colour to take effect. The light theme puts the request back, because
    /// a page may have been designed with it — the application's own search window has one that was.
    /// </para>
    /// <para>
    /// A page is a <see cref="Panel"/>, so this styler is registered before the one for containers.
    /// </para>
    /// </summary>
    internal sealed class TabPageStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is TabPage;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);
            var page = (TabPage)control;

            // The colours first and the style last, and the order is not a matter of taste. A page
            // decides again whether it may use the visual style every time one of its colours changes,
            // and it decides to stop using it. Assigning the style first and a colour afterwards
            // therefore loses the style on the spot: a page designed with it would come back from the
            // dark theme no longer asking for it, and its background would read the system colour
            // where it used to read none.
            page.BackColor = state.Colour("BackColor", theme, theme.SurfaceBack);
            page.ForeColor = state.Colour("ForeColor", theme, theme.SurfaceText);
            page.UseVisualStyleBackColor = state.Value("UseVisualStyleBackColor", theme, false);
        }
    }
}

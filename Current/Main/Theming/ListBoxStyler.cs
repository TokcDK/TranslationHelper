using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a list box, which is a list the user picks from rather than a list the framework draws.
    /// <para>
    /// It takes the editor colours for the same reason a text box does: it is a surface the user reads
    /// a value off, so it is painted like one.
    /// </para>
    /// </summary>
    internal sealed class ListBoxStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is ListBox;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.EditorBack);
            control.ForeColor = state.Colour("ForeColor", theme, theme.EditorText);
        }
    }
}

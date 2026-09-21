using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a text box or a rich text box.
    /// <para>
    /// The text keeps the ordinary colour even when the control is disabled, because a disabled text
    /// box is drawn with the system's disabled colour whatever colour it is given, and a control that
    /// is enabled again keeps readable text this way.
    /// </para>
    /// <para>
    /// The log window is a rich text box, and this is the styler that makes it follow the theme. The
    /// text in it takes this colour because nothing writes a colour into it line by line any more, and
    /// assigning the colour to a rich text box recolours what is already in it — so the lines written
    /// before the theme was applied are put right by the same change.
    /// </para>
    /// </summary>
    internal sealed class TextBoxStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is TextBoxBase;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.EditorBack);
            control.ForeColor = state.Colour("ForeColor", theme, theme.EditorText);
        }
    }
}

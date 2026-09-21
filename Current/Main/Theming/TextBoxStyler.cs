using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a text box or a rich text box.
    /// <para>
    /// One shape cannot be painted at all while it is disabled. A disabled rich text box draws its own
    /// background in the system's button colour and ignores everything that can be thrown at it:
    /// <c>BackColor</c>, <c>SetWindowTheme</c>, <c>EM_SETBKGNDCOLOR</c>, a filled
    /// <c>WM_ERASEBKGND</c>, and the parent's colouring message — all five were measured, and the
    /// background stayed #F0F0F0 through every one. A plain text box does not have that problem, and
    /// neither does a read-only rich text box, so an editor that is not to be edited is held
    /// <c>ReadOnly</c> rather than <c>Enabled = false</c>. See
    /// <c>FunctionsUI.SetOnTHFileElementsDataGridViewWasLoaded</c>.
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

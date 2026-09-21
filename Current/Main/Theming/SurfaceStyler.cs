using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints anything no other styler recognised.
    /// <para>
    /// A control this application has never heard of still gets the surface colours, which is right for
    /// a picture box, a tree, a list view or a spin box, and which a control that draws itself ignores
    /// anyway. The alternative — leaving it alone — is what would leave a light rectangle on a dark
    /// window, so this styler accepts everything and is registered last.
    /// </para>
    /// <para>
    /// Supporting such a control properly means giving it a styler of its own and registering it before
    /// this one. Nothing else has to change.
    /// </para>
    /// </summary>
    internal sealed class SurfaceStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return true;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.SurfaceBack);
            control.ForeColor = state.Colour("ForeColor", theme, theme.SurfaceText);
        }
    }
}

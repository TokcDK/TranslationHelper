using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a window itself.
    /// <para>
    /// A window takes the colour of a window rather than the colour of a container, which is what lets
    /// a dark theme put lighter containers on a darker window. A window that is also a
    /// <see cref="ContainerControl"/> would otherwise be caught by the styler for containers, so this
    /// one is registered before it.
    /// </para>
    /// </summary>
    internal sealed class FormStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is Form;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.FormBack);
            control.ForeColor = state.Colour("ForeColor", theme, theme.SurfaceText);
        }
    }
}

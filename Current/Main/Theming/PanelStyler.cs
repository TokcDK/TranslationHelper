using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a container: a panel, a group box, a user control, or one of the two panels of a split
    /// container.
    /// <para>
    /// A container and the labels sitting on it take the same colour on purpose. That is what keeps a
    /// themed window free of light patches where a panel or a caption was, and it is also what lets a
    /// label that was never given a colour of its own simply inherit.
    /// </para>
    /// </summary>
    internal sealed class PanelStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is Panel || control is GroupBox || control is ContainerControl;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.SurfaceBack);
            control.ForeColor = state.Colour("ForeColor", theme, theme.SurfaceText);
        }
    }
}

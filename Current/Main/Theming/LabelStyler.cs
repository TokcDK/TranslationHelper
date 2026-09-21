using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a caption.
    /// <para>
    /// A disabled caption takes the disabled colour, because a label is drawn by the application and
    /// would otherwise keep its ordinary colour while the control it describes is greyed out.
    /// </para>
    /// </summary>
    internal sealed class LabelStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is Label;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.SurfaceBack);
            control.ForeColor = state.Colour(
                "ForeColor",
                theme,
                control.Enabled ? theme.SurfaceText : theme.DisabledText);
        }
    }
}

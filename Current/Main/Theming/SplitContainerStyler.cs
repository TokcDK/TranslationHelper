using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints the bar between the two halves of a split container.
    /// <para>
    /// Only the container itself, which is only the splitter bar: its two panels are children of it and
    /// are painted by the walk, like any other container. A panel that had no colour of its own
    /// therefore goes back to taking the colour of the bar it sits against.
    /// </para>
    /// </summary>
    internal sealed class SplitContainerStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is SplitContainer;
        }

        public void Apply(Control control, ITheme theme)
        {
            var state = ThemedControlState.For(control);

            control.BackColor = state.Colour("BackColor", theme, theme.Splitter);
            control.ForeColor = state.Colour("ForeColor", theme, theme.SurfaceText);
        }
    }
}

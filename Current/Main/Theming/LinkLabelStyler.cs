using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a link, which is a caption with three more colours of its own.
    /// <para>
    /// A link is a <see cref="Label"/>, so this styler is registered before the one for captions.
    /// </para>
    /// <para>
    /// All three states share the link colour. A visited link that kept the system's dark purple would
    /// be unreadable on a dark surface, and a link the user has already followed is not something the
    /// application needs to tell apart from one they have not.
    /// </para>
    /// </summary>
    internal sealed class LinkLabelStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is LinkLabel;
        }

        public void Apply(Control control, ITheme theme)
        {
            var linkLabel = (LinkLabel)control;
            var state = ThemedControlState.For(control);

            linkLabel.BackColor = state.Colour("BackColor", theme, theme.SurfaceBack);
            linkLabel.ForeColor = state.Colour(
                "ForeColor",
                theme,
                linkLabel.Enabled ? theme.SurfaceText : theme.DisabledText);

            // A link colour can be cleared, so the light theme gives back whatever was chosen for it,
            // and the framework's own colours where there was nothing.
            linkLabel.LinkColor = state.Colour("LinkColor", theme, theme.LinkText);
            linkLabel.ActiveLinkColor = state.Colour("ActiveLinkColor", theme, theme.LinkText);
            linkLabel.VisitedLinkColor = state.Colour("VisitedLinkColor", theme, theme.LinkText);
        }
    }
}

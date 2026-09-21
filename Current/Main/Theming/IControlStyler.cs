using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// How one kind of control is painted in a theme.
    /// <para>
    /// This is the behaviour half of the design, and <see cref="ITheme"/> is the data half. Keeping
    /// them apart is what lets a new control type be supported by adding a class and registering it,
    /// without touching the theme, the registry, or any styler that already works.
    /// </para>
    /// <para>
    /// A styler answers two questions: whether it is the one that should paint a given control, and how
    /// to paint it. The registry asks them in the order the stylers were registered and stops at the
    /// first that says yes, so a styler for a derived control is registered before the one for its base.
    /// </para>
    /// <para>
    /// A styler that writes a property the theme has to be able to take back must ask
    /// <see cref="ThemedControlState.For"/> for the control <em>before</em> its first write, because
    /// the write itself is what destroys the evidence of what was there before.
    /// </para>
    /// </summary>
    internal interface IControlStyler
    {
        /// <summary>True when this styler is the one that should paint <paramref name="control"/>.</summary>
        bool CanStyle(Control control);

        /// <summary>Paints <paramref name="control"/> in <paramref name="theme"/>.</summary>
        void Apply(Control control, ITheme theme);
    }
}

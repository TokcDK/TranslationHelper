using System.Drawing;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// A tab strip whose tabs each carry a close button.
    /// <para>
    /// The strip answers where the button of a tab is, and the theme draws it. The split is not
    /// arbitrary: the strip is painted by whoever owns its pixels, and while the theme is the one
    /// drawing the tabs, a second painter on the same tab would fight it for the same pixels — the
    /// button would come out under the tab's own fill or over it depending on which of two handlers
    /// happened to be subscribed first. Drawing the button as part of the tab is what makes its place
    /// on the tab a fact of the drawing instead of a hope about the order of two handlers.
    /// </para>
    /// <para>
    /// A strip that implements this is also a strip the theme has to draw in <em>every</em> theme,
    /// including the light one where the tabs would otherwise be handed back to the framework and
    /// there would be no button at all. That is why the strip also chooses owner drawing rather than
    /// leaving the choice to the theme.
    /// </para>
    /// </summary>
    internal interface IClosableTabStrip
    {
        /// <summary>
        /// The rectangle of the close button of the tab at <paramref name="index"/>, in the coordinates
        /// of the strip, or <see cref="Rectangle.Empty"/> when that tab has no button — an index outside
        /// the strip, or a tab too narrow to hold both its text and a button.
        /// </summary>
        Rectangle CloseButtonBounds(int index);
    }
}

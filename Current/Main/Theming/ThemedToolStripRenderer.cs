using System.Drawing;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Draws menus and their drop-downs in the current theme.
    /// <para>
    /// Colours come from <see cref="ThemedColorTable"/>. The text and the drop-down arrows have to be
    /// handled here instead, because the professional renderer takes those from the item itself: an
    /// item that was never given a colour reports the system one, which is dark, and dark text on a
    /// dark menu is unreadable. Deciding the colour at paint time also means an item that a feature
    /// adds to a menu later is themed without the feature knowing that themes exist.
    /// </para>
    /// <para>
    /// One instance is installed for the whole application by <see cref="ThemeManager"/>, and it
    /// reads the theme as it paints, so it never has to be replaced.
    /// </para>
    /// </summary>
    internal sealed class ThemedToolStripRenderer : ToolStripProfessionalRenderer
    {
        internal ThemedToolStripRenderer()
            : base(new ThemedColorTable())
        {
            // Rounded edges are drawn in the system's control colour and would leave a light rim
            // around a dark drop-down.
            RoundedEdges = false;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = TextColorFor(e.Item, e.TextColor);

            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = TextColorFor(e.Item, e.ArrowColor);

            base.OnRenderArrow(e);
        }

        /// <summary>
        /// The colour of a menu item's text: the theme's text, the theme's highlight text while the
        /// item is under the pointer, and the theme's disabled text when the item cannot be used.
        /// </summary>
        private static Color TextColorFor(ToolStripItem item, Color fallback)
        {
            if (item == null)
            {
                return fallback;
            }

            var theme = ThemeManager.Instance.CurrentTheme;

            if (!item.Enabled)
            {
                return theme.DisabledText;
            }

            return item.Selected ? theme.MenuHighlightText : theme.MenuText;
        }
    }
}

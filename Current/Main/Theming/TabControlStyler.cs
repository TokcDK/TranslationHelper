using System.Drawing;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Themes a tab strip, which the framework draws itself and will not recolour.
    /// <para>
    /// A tab control exposes a background colour for the strip and ignores it: the strip is painted by
    /// the native control, which takes its colour from the visual style, so a tab control on a dark
    /// window keeps a near white band above the pages. The tabs themselves are recoloured by owner
    /// drawing them; the band around them cannot be, and is filled by hand.
    /// </para>
    /// <para>
    /// Whether the strip is owner-drawn at all is the strip's decision, and it is read back from the
    /// control rather than assumed: the light theme leaves it to the framework, so a strip that wants
    /// to be drawn — one whose tabs carry close buttons, and whose buttons would have nowhere to be
    /// painted otherwise — says so by choosing owner drawing, and the theme honours that choice in
    /// every theme. See <see cref="ThemedControlState"/> for what "chose" means here.
    /// </para>
    /// <para>
    /// The fill happens inside the owner draw handler and not in a <see cref="Control.Paint"/> handler,
    /// which is the one thing about this class that looks wrong and is not. A tab control never raises
    /// <see cref="Control.Paint"/> at all — the native control answers its own paint message and the
    /// managed event is not reached — so a handler there would never run. The graphics object handed to
    /// the owner draw handler is the tab control's own, however, so filling through it lands on the
    /// same surface, in the same paint pass, with no flicker and nothing to subclass or hook.
    /// </para>
    /// </summary>
    internal sealed class TabControlStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is TabControl;
        }

        public void Apply(Control control, ITheme theme)
        {
            var tabs = (TabControl)control;
            var state = ThemedControlState.For(control);

            // Assigned only when it has to change: setting DrawMode recreates the control's handle, and
            // the theme is applied again after every change of the setting. The light theme hands the
            // tabs back to the framework, which is what a tab control starts with.
            var drawMode = state.Value("DrawMode", theme, TabDrawMode.OwnerDrawFixed);

            if (tabs.DrawMode != drawMode)
            {
                tabs.DrawMode = drawMode;
            }

            // Removed before it is added so that applying the theme twice cannot draw every tab
            // twice. While the light theme is on, the strip is handed back to the framework and the
            // handler returns immediately, which is what the designer's own handler, if there is one,
            // expects — unless the strip chose to be drawn itself, in which case it is drawn here in
            // both themes.
            tabs.DrawItem -= DrawTab;
            tabs.DrawItem += DrawTab;

            // The text of the control itself, and the colour the strip would be if the framework
            // honoured it. The band is filled by hand because the framework does not.
            tabs.BackColor = state.Colour("BackColor", theme, theme.TabBack);
            tabs.ForeColor = state.Colour("ForeColor", theme, theme.SurfaceText);
        }

        private static void DrawTab(object sender, DrawItemEventArgs e)
        {
            if (!(sender is TabControl tabs))
            {
                return;
            }

            if (e.Index < 0 || e.Index >= tabs.TabPages.Count)
            {
                return;
            }

            // Reaching here at all means the strip is owner-drawn, and this is the only thing that will
            // paint it: a strip the framework draws raises no DrawItem. The light theme used to hand the
            // tabs back to the framework and stop here, which it still does — through the DrawMode it
            // gives back, so a strip that left the choice to the theme is still the framework's to draw.
            // A strip that chose owner drawing instead — one that carries close buttons, whose button
            // has nowhere to be drawn otherwise — is drawn here in either theme.
            if (tabs.DrawMode != TabDrawMode.OwnerDrawFixed)
            {
                return;
            }

            var theme = ThemeManager.Instance.CurrentTheme;
            var bounds = tabs.GetTabRect(e.Index);
            var selected = e.Index == tabs.SelectedIndex;

            // The band first, so that the tab drawn over it is not painted over in turn. Filling the
            // complement of the tab rectangles rather than a rectangle of its own means the order the
            // tabs are drawn in does not matter and this can be done for every tab.
            FillStrip(e.Graphics, tabs, theme);

            using (var background = new SolidBrush(selected ? theme.TabSelectedBack : theme.TabBack))
            {
                e.Graphics.FillRectangle(background, bounds);
            }

            // An outline on three sides separates a tab from the strip. The selected tab is left open
            // along its bottom edge so that it joins the page.
            using (var outline = new Pen(theme.Border))
            {
                e.Graphics.DrawLine(outline, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top);
                e.Graphics.DrawLine(outline, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1);
                e.Graphics.DrawLine(outline, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
            }

            var close = CloseButtonBounds(tabs, e.Index);

            // The text is centred in what is left of the tab once the button has its corner, so that a
            // name long enough to fill the tab is shortened rather than written over the button.
            var textBounds = close.IsEmpty
                ? bounds
                : new Rectangle(bounds.Left, bounds.Top, close.Left - bounds.Left, bounds.Height);

            TextRenderer.DrawText(
                e.Graphics,
                tabs.TabPages[e.Index].Text,
                tabs.Font,
                textBounds,
                selected ? theme.TabSelectedText : theme.TabText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            DrawCloseButton(e.Graphics, close, selected ? theme.TabSelectedText : theme.TabText);
        }

        /// <summary>
        /// Where the close button of a tab is, asked of the strip itself, or
        /// <see cref="Rectangle.Empty"/> for a strip whose tabs cannot be closed.
        /// <para>
        /// It is asked rather than computed here because the geometry is the tab control's: it is the
        /// only thing that knows where its tabs are once they have been laid out, resized and wrapped.
        /// </para>
        /// </summary>
        private static Rectangle CloseButtonBounds(TabControl tabs, int index)
        {
            return tabs is IClosableTabStrip closable ? closable.CloseButtonBounds(index) : Rectangle.Empty;
        }

        /// <summary>
        /// Draws the cross that says a tab can be closed, in the colour the tab's own text is drawn in
        /// so that it reads as part of the tab rather than as a control that landed on it.
        /// </summary>
        private static void DrawCloseButton(Graphics graphics, Rectangle bounds, Color colour)
        {
            if (bounds.IsEmpty)
            {
                return;
            }

            const int Inset = 4;

            using (var pen = new Pen(colour))
            {
                graphics.DrawLine(pen, bounds.Left + Inset, bounds.Top + Inset, bounds.Right - Inset - 1, bounds.Bottom - Inset - 1);
                graphics.DrawLine(pen, bounds.Right - Inset - 1, bounds.Top + Inset, bounds.Left + Inset, bounds.Bottom - Inset - 1);
            }
        }

        /// <summary>
        /// Fills the band the tabs sit in, everywhere a tab is not. The band is the strip of the
        /// control between its edge and the pages, on whichever side the tabs were put.
        /// </summary>
        private static void FillStrip(Graphics graphics, TabControl tabs, ITheme theme)
        {
            var band = TabBand(tabs);

            if (band.Width <= 0 || band.Height <= 0)
            {
                return;
            }

            using (var outside = new Region(band))
            {
                for (var index = 0; index < tabs.TabPages.Count; index++)
                {
                    outside.Exclude(tabs.GetTabRect(index));
                }

                using (var brush = new SolidBrush(theme.TabBack))
                {
                    graphics.FillRegion(brush, outside);
                }
            }
        }

        /// <summary>
        /// The strip the tabs are drawn in. A tab control puts them at the top unless it was told
        /// otherwise, and the pages take everything that is left.
        /// </summary>
        private static Rectangle TabBand(TabControl tabs)
        {
            var pages = tabs.DisplayRectangle;

            switch (tabs.Alignment)
            {
                case TabAlignment.Bottom:
                    return new Rectangle(0, pages.Bottom, tabs.Width, tabs.Height - pages.Bottom);

                case TabAlignment.Left:
                    return new Rectangle(0, 0, pages.Left, tabs.Height);

                case TabAlignment.Right:
                    return new Rectangle(pages.Right, 0, tabs.Width - pages.Right, tabs.Height);

                default:
                    return new Rectangle(0, 0, tabs.Width, pages.Top);
            }
        }
    }
}

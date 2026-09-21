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
            // twice. While the light theme is on, the handler returns immediately and the framework
            // draws the tabs, which is what the designer's own handler, if there is one, expects.
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

            var theme = ThemeManager.Instance.CurrentTheme;

            if (!theme.IsDark)
            {
                return;
            }

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

            TextRenderer.DrawText(
                e.Graphics,
                tabs.TabPages[e.Index].Text,
                tabs.Font,
                bounds,
                selected ? theme.TabSelectedText : theme.TabText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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

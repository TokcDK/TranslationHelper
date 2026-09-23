using System;
using System.Drawing;
using System.Windows.Forms;
using TranslationHelper.Theming;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// A tab strip whose tabs each carry a close button.
    /// <para>
    /// It knows the one thing the framework's tab control does not: where the close button of a tab
    /// is. The button is drawn with the tab, by the theme — see <see cref="IClosableTabStrip"/> — and
    /// the geometry of it lives here, because the geometry is the tab control's own:
    /// <see cref="TabControl.GetTabRect"/> answers in its coordinates, and the tabs move when the strip
    /// is resized, when a tab is added and when one is removed.
    /// </para>
    /// <para>
    /// The strip never removes a page by itself. A tab presents something, and whether that something
    /// may be closed — and what closing it means — is decided above this control; the strip only says
    /// that the user asked. That is what keeps the pages in step with the list that owns them: the page
    /// is removed where the item is removed, and not here.
    /// </para>
    /// </summary>
    internal sealed class ClosableTabControl : TabControl, IClosableTabStrip
    {
        /// <summary>Side of the square the close button is drawn in.</summary>
        private const int CloseButtonSize = 14;

        /// <summary>
        /// Space kept between the button and the edges of the tab. It is also the distance the tab's
        /// text is held away from the button, so that the two cannot touch.
        /// </summary>
        private const int CloseButtonMargin = 3;

        /// <summary>
        /// The least room a tab has to leave around the button for it to be worth drawing. A tab of a
        /// stock tab control is around eighteen pixels high, which holds the button and a pixel of air
        /// above and below it and no more — hence the small number.
        /// </summary>
        private const int MinimumButtonClearance = 1;

        /// <summary>
        /// The narrowest tab that still has room for its text and a close button. Below it no button is
        /// drawn at all, rather than drawn over the name of what the tab presents.
        /// </summary>
        private const int MinimumTabWidth = 48;

        internal ClosableTabControl()
        {
            // Owner drawing is chosen here rather than left to the theme. A strip the framework paints
            // has nowhere to put a button, and the light theme hands the tabs back to the framework by
            // default, so without this the button would exist in the dark theme only. The theme keeps a
            // DrawMode that was chosen over its own — see ThemedControlState — so this survives a change
            // of theme, and the theme draws the tabs of a strip that chose to be drawn.
            DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        /// <summary>
        /// Raised when the user asks to close the tab at <paramref name="index"/>, which is when the
        /// close button of that tab is clicked.
        /// </summary>
        internal event Action<int> TabCloseRequested;

        Rectangle IClosableTabStrip.CloseButtonBounds(int index) => CloseButtonBounds(index);

        /// <summary>
        /// Where the close button of the tab at <paramref name="index"/> is, in the coordinates of the
        /// strip, or <see cref="Rectangle.Empty"/> when that tab has no button.
        /// </summary>
        internal Rectangle CloseButtonBounds(int index)
        {
            if (index < 0 || index >= TabPages.Count) return Rectangle.Empty;

            var tab = GetTabRect(index);

            if (tab.Width < MinimumTabWidth) return Rectangle.Empty;
            if (tab.Height < CloseButtonSize + 2 * MinimumButtonClearance) return Rectangle.Empty;

            return new Rectangle(
                tab.Right - CloseButtonSize - CloseButtonMargin,
                tab.Top + (tab.Height - CloseButtonSize) / 2,
                CloseButtonSize,
                CloseButtonSize);
        }

        /// <summary>
        /// Report a click on a close button, and let the tab be selected as it would have been.
        /// <para>
        /// The button is checked before the framework is told about the click, and the click is still
        /// passed on: the tab under the button becomes the selected one, so a close that is refused —
        /// the user cancelled the save, say — leaves them looking at the tab they aimed at.
        /// </para>
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Left)
            {
                // Every tab is asked rather than the one the point is in, because a button lies inside
                // its own tab: a point in a button is a point in exactly one tab, and asking the buttons
                // directly cannot be confused by a strip whose tabs wrapped onto a second row.
                for (var index = 0; index < TabPages.Count; index++)
                {
                    if (!CloseButtonBounds(index).Contains(e.Location)) continue;

                    // Reported once the click has been handled rather than in the middle of it. Closing
                    // a tab removes its page, and removing a page while the strip is still processing a
                    // mouse message about that page tears the control apart under the message that is
                    // being handled — measured: it ends in a stack overflow, because the native control
                    // is left to finish a click on a tab that no longer exists.
                    BeginInvoke((Action)(() => TabCloseRequested?.Invoke(index)));
                    break;
                }
            }

            base.OnMouseDown(e);
        }
    }
}

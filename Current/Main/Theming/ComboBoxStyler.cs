using System.Drawing;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Themes a combo box, whose list the framework draws in the system colours.
    /// <para>
    /// Only a plain <see cref="ComboBox"/> is drawn here. A derived combo box — the search windows
    /// use one that shows a suggestion list of its own — draws its own list, so it is given the
    /// colours it asks for and is left to paint the rest; taking its drawing over would replace
    /// whatever it was written to show.
    /// </para>
    /// </summary>
    internal sealed class ComboBoxStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is ComboBox;
        }

        public void Apply(Control control, ITheme theme)
        {
            var comboBox = (ComboBox)control;
            var state = ThemedControlState.For(control);

            // The colours first, then the rendering: the framework decides again whether a control may
            // use the visual style whenever one of its colours changes, so a style set before the
            // colours would not survive them.
            comboBox.BackColor = state.Colour("BackColor", theme, theme.EditorBack);
            comboBox.ForeColor = state.Colour("ForeColor", theme, theme.EditorText);

            // A flat style is what stops the framework drawing the system's own sunken border around the
            // box on a dark surface.
            comboBox.FlatStyle = state.Value("FlatStyle", theme, FlatStyle.Flat);

            // A derived combo box draws its own list and is left to it, so its draw mode is not the
            // theme's to set or to put back.
            if (comboBox.GetType() != typeof(ComboBox))
            {
                return;
            }

            // Assigned only when it has to change: setting DrawMode recreates the control's handle,
            // and the theme is applied again after every change of the setting. The light theme
            // hands the list back to the framework, which is what a combo box starts with.
            var drawMode = state.Value("DrawMode", theme, DrawMode.OwnerDrawFixed);

            if (comboBox.DrawMode != drawMode)
            {
                comboBox.DrawMode = drawMode;
            }

            // Removed before it is added so that applying the theme twice cannot draw every item
            // twice. While the light theme is on, the framework draws the list and this never runs.
            comboBox.DrawItem -= DrawItem;
            comboBox.DrawItem += DrawItem;
        }

        private static void DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is ComboBox comboBox))
            {
                return;
            }

            var theme = ThemeManager.Instance.CurrentTheme;

            if (!theme.IsDark)
            {
                return;
            }

            // The state is Selected both for the item under the pointer in an open list and for the
            // item shown in the closed box, so one brush pair serves both.
            var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (var background = new SolidBrush(selected ? theme.EditorSelectionBack : theme.EditorBack))
            {
                e.Graphics.FillRectangle(background, e.Bounds);
            }

            if (e.Index < 0 || e.Index >= comboBox.Items.Count)
            {
                return;
            }

            var text = comboBox.Enabled
                ? (selected ? theme.EditorSelectionText : theme.EditorText)
                : theme.DisabledText;

            var bounds = new Rectangle(e.Bounds.X + 2, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height);

            TextRenderer.DrawText(
                e.Graphics,
                comboBox.GetItemText(comboBox.Items[e.Index]),
                comboBox.Font,
                bounds,
                text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}

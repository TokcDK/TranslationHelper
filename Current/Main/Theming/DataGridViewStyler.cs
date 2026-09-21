using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Themes a data grid, which needs a property group of its own.
    /// <para>
    /// A grid does not have one background: cells, every second row, the row headers, the column
    /// headers and the empty area below the last row are all painted separately, and a header painted
    /// in the system's visual style ignores the colours set on it. Recolouring a grid therefore means
    /// switching the headers to being painted by the application and writing the colours into each of
    /// those places.
    /// </para>
    /// <para>
    /// The colours of the row and header styles are written in both themes, and no memory of them is
    /// needed, because the light theme holds the framework's own defaults for exactly those places.
    /// The three properties the designer can reach from the grid itself are another matter, and they
    /// go through <see cref="ThemedControlState"/> so that a grid given a colour on purpose gets it
    /// back.
    /// </para>
    /// </summary>
    internal sealed class DataGridViewStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is DataGridView;
        }

        public void Apply(Control control, ITheme theme)
        {
            var grid = (DataGridView)control;
            var state = ThemedControlState.For(control);

            // Without this the column headers keep the system's visual style and ignore every colour
            // set below. None of these three can be put back by clearing them, so the light theme
            // restores the value each one had: a grid that was given a colour on purpose keeps it, and
            // one that was not goes back to the AppWorkspace and ControlDark a grid starts with.
            grid.EnableHeadersVisualStyles = state.Value("EnableHeadersVisualStyles", theme, false);
            grid.BackgroundColor = state.Value("BackgroundColor", theme, theme.GridBack);
            grid.GridColor = state.Value("GridColor", theme, theme.GridLines);

            PaintStyle(grid.DefaultCellStyle, theme.GridBack, theme.GridText, theme.GridSelectionBack, theme.GridSelectionText);
            PaintStyle(grid.RowsDefaultCellStyle, theme.GridBack, theme.GridText, theme.GridSelectionBack, theme.GridSelectionText);
            PaintStyle(grid.AlternatingRowsDefaultCellStyle, theme.GridAlternateBack, theme.GridText, theme.GridSelectionBack, theme.GridSelectionText);
            PaintStyle(grid.ColumnHeadersDefaultCellStyle, theme.GridHeaderBack, theme.GridHeaderText, theme.GridHeaderBack, theme.GridHeaderText);
            PaintStyle(grid.RowHeadersDefaultCellStyle, theme.GridHeaderBack, theme.GridHeaderText, theme.GridHeaderBack, theme.GridHeaderText);

            grid.TopLeftHeaderCell.Style.BackColor = theme.GridHeaderBack;
            grid.TopLeftHeaderCell.Style.ForeColor = theme.GridHeaderText;

            ApplyToColumnHeaders(grid, theme);
        }

        private static void PaintStyle(
            DataGridViewCellStyle style,
            System.Drawing.Color background,
            System.Drawing.Color text,
            System.Drawing.Color selectionBack,
            System.Drawing.Color selectionText)
        {
            style.BackColor = background;
            style.ForeColor = text;
            style.SelectionBackColor = selectionBack;
            style.SelectionForeColor = selectionText;
        }

        /// <summary>
        /// Writes the header colours into each column as well.
        /// <para>
        /// A style set on a column beats the grid's default for that column, and a grid that has been
        /// used already carries such styles, so the header colours have to be applied per column or
        /// part of the header row would stay in the system colours.
        /// </para>
        /// </summary>
        private static void ApplyToColumnHeaders(DataGridView grid, ITheme theme)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                var header = column.HeaderCell.Style;

                header.BackColor = theme.GridHeaderBack;
                header.ForeColor = theme.GridHeaderText;
                header.SelectionBackColor = theme.GridHeaderBack;
                header.SelectionForeColor = theme.GridHeaderText;
            }
        }
    }
}

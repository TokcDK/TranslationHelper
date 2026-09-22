using System;
using System.Data;
using System.Windows.Forms;
using TranslationHelper.Functions;
using TranslationHelper.Workspace;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Writes into the work table through the grid of the file the project is showing, marshalling to
    /// the UI thread when the caller is not on it.
    /// <para>
    /// This is an adapter: it is the only place that knows a cell write has to go through the grid's
    /// thread, and the only place that knows a long run detaches the grid first so it is not repainted
    /// once per row. Row operations only see <see cref="IUiUpdater"/>.
    /// </para>
    /// </summary>
    internal class WinFormsUiUpdater : IUiUpdater
    {
        /// <summary>
        /// The project the run belongs to. The grid is read from it on every use rather than captured,
        /// because the file the project shows can change while a run is in progress.
        /// </summary>
        private readonly IProjectWorkspace _workspace;

        public WinFormsUiUpdater(IProjectWorkspace workspace)
        {
            _workspace = workspace;
        }

        /// <summary>
        /// The grid of the file the project is showing, or null while it shows none.
        /// </summary>
        private DataGridView Grid => _workspace?.ActiveFileWorkspace?.ElementsDataGridView;

        public void SetTranslation(DataRow row, int columnIndex, string value)
        {
            var grid = Grid;

            if (grid != null && grid.InvokeRequired)
            {
                grid.Invoke((MethodInvoker)delegate
                {
                    row.SetField(columnIndex, value);
                });
            }
            else
            {
                row.SetField(columnIndex, value);
            }
        }

        /// <summary>
        /// Shows the message in a message box. Kept as a plain, ownerless box: the operations that
        /// report a problem already ran on the UI thread, and changing that would change when the
        /// user is interrupted.
        /// </summary>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void BeginBulkChange(DataTable table)
        {
            var grid = Grid;

            // Only the file on screen is detached. A table the grid is not showing is not repainted per
            // row in the first place, so detaching it would achieve nothing and cost a rebind.
            if (grid == null || !ReferenceEquals(grid.DataSource, table)) return;

            OnGrid(grid, () =>
            {
                grid.DataSource = null;
                grid.Update();
                grid.Refresh();
            });
        }

        public void EndBulkChange(DataTable table)
        {
            var workspace = _workspace?.ActiveFileWorkspace;
            if (workspace == null) return;

            OnGrid(workspace.ElementsDataGridView, () =>
            {
                workspace.RefreshBinding();
                FunctionsUI.UpdateTextboxes(_workspace);
            });
        }

        /// <summary>
        /// Run <paramref name="action"/> on the thread that owns <paramref name="control"/>.
        /// </summary>
        private static void OnGrid(Control control, Action action)
        {
            if (control == null) return;

            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }

            action();
        }
    }
}

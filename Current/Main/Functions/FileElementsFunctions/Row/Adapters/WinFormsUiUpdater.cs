using System.Data;
using System.Windows.Forms;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Writes into the work table through its DataGridView, marshalling to the UI thread when the
    /// caller is not on it.
    /// <para>
    /// This is an adapter: it is the only place that knows a cell write has to go through the grid's
    /// thread. Row operations only see <see cref="IUiUpdater"/>.
    /// </para>
    /// </summary>
    internal class WinFormsUiUpdater : IUiUpdater
    {
        private readonly DataGridView _workTableDatagridView;

        public WinFormsUiUpdater(DataGridView workTableDatagridView)
        {
            _workTableDatagridView = workTableDatagridView;
        }

        public void SetTranslation(DataRow row, int columnIndex, string value)
        {
            if (_workTableDatagridView.InvokeRequired)
            {
                _workTableDatagridView.Invoke((MethodInvoker)delegate
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
    }
}

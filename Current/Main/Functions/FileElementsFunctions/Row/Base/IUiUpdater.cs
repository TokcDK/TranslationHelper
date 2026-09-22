using System.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// The write side of the work table: the only way a row operation changes a cell or reports
    /// something to the user.
    /// <para>
    /// It exists so that the operations stay independent of the UI toolkit. Writing a cell has to
    /// happen on the UI thread while rows are processed on several threads, and that marshalling is
    /// the implementation's business, not the operation's.
    /// </para>
    /// </summary>
    public interface IUiUpdater
    {
        /// <summary>
        /// Writes a value into a cell of a row of the work table.
        /// </summary>
        /// <param name="row">The row to change.</param>
        /// <param name="columnIndex">The column to change.</param>
        /// <param name="value">The value to write.</param>
        void SetTranslation(DataRow row, int columnIndex, string value);

        /// <summary>
        /// Reports a problem the operation cannot fix itself. How it is presented is up to the
        /// implementation; an operation only says that the user has to be told.
        /// </summary>
        void ShowMessage(string message);

        /// <summary>
        /// Announce that <paramref name="table"/> is about to be changed row by row, so a view showing
        /// it can stop following it for the duration.
        /// <para>
        /// This is what an operation that rewrites every row uses to avoid a repaint per row. It is
        /// deliberately not "clear the grid": which control shows the table, and whether any does, is
        /// the implementation's business.
        /// </para>
        /// </summary>
        /// <param name="table">The table the run is about to change.</param>
        void BeginBulkChange(DataTable table);

        /// <summary>
        /// Announce that the changes to <paramref name="table"/> are done, so a view showing it can
        /// follow it again. The counterpart of <see cref="BeginBulkChange"/>.
        /// </summary>
        /// <param name="table">The table the run has finished changing.</param>
        void EndBulkChange(DataTable table);
    }
}

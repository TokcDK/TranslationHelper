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
    }
}

using System.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Everything the row engine needs to know about what the user has selected, and the only way it
    /// reads the UI.
    /// <para>
    /// It is an interface because the engine must not know how the selection is presented: the
    /// WinForms implementation lives in the Adapters folder, and a test or a batch caller can supply
    /// its own. Every member is about *reading* the selection; nothing here applies an operation.
    /// </para>
    /// </summary>
    public interface ISelectionProvider
    {
        /// <summary>
        /// Table indexes of the selected entries. The "[ALL]" entry stands for every file, so it
        /// comes back as all of them.
        /// </summary>
        int[] GetSelectedTableIndexes();

        /// <summary>
        /// What a "selected rows" operation has to cover: the file tables the selected rows belong to
        /// and, for each of them, the rows to apply the operation to.
        /// </summary>
        TableRowPlan[] GetSelectedRowsPlan();

        /// <summary>
        /// Index of the entry selected in the files list, or -1 when nothing is selected.
        /// <para>
        /// This is an *entry* index, not a table index: the two differ while the "[ALL]" entry is
        /// present. Ask <see cref="GetSelectedTableIndexes"/> or
        /// <see cref="GetSelectedRowsPlan"/> when a table index is what is needed.
        /// </para>
        /// </summary>
        int GetSelectedListIndex();

        /// <summary>
        /// The one row selected in the work table grid, when exactly one row is selected.
        /// </summary>
        /// <param name="gridRowIndex">The selected row of the grid, or -1 when there is not exactly one.</param>
        /// <returns>True when exactly one row is selected.</returns>
        bool TryGetSingleSelectedGridRowIndex(out int gridRowIndex);

        /// <summary>
        /// Index in <paramref name="table"/> of the row the grid shows at <paramref name="gridRowIndex"/>.
        /// The grid can be sorted or filtered, so a grid row is not necessarily the table row with the
        /// same number.
        /// </summary>
        int GetRealTableRowIndex(DataTable table, int gridRowIndex);
    }
}

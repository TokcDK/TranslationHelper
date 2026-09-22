using System.Data;
using TranslationHelper.Functions.FilesListControl;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Works out which file table and which row of it a single-row operation has to work on.
    /// <para>
    /// It is pure addressing: it resolves a target from what the caller passed and what is selected,
    /// and it applies nothing. The hard part is that the entry selected in the files list may be the
    /// "[ALL]" entry, whose rows come from several files at once, so a row of the work table has to
    /// be mapped back to the file it was taken from.
    /// </para>
    /// </summary>
    internal sealed class RowTargetResolver
    {
        private readonly ISelectionProvider _selection;
        private readonly DataSet _filesContent;

        /// <summary>
        /// The project's relation between a files list entry and the content it presents. It is the
        /// project's own, so a run addresses the files of the project it was started for.
        /// </summary>
        private readonly FilesListContent _filesListContent;

        internal RowTargetResolver(ISelectionProvider selection, DataSet filesContent, FilesListContent filesListContent)
        {
            _selection = selection;
            _filesContent = filesContent;
            _filesListContent = filesListContent;
        }

        /// <summary>
        /// Resolves the table and row to work on.
        /// </summary>
        /// <param name="tableIndex">
        /// Index in the project content of the table to work on, or -1 to use the entry selected in
        /// the files list. That entry may be the "[ALL]" entry, in which case the row is resolved to
        /// the file it was taken from.
        /// </param>
        /// <param name="rowIndex">Row of the work table grid, or -1 to use the selected one.</param>
        /// <param name="tableData">The file table to work on, or null when it could not be resolved.</param>
        /// <param name="realRowIndex">Index of the row in that file table.</param>
        /// <returns>True when both a table and a row were resolved.</returns>
        internal bool TryResolve(ref int tableIndex, ref int rowIndex, out TableData tableData, out int realRowIndex)
        {
            tableData = null;
            realRowIndex = -1;

            if (rowIndex < 0 && !_selection.TryGetSingleSelectedGridRowIndex(out rowIndex)) return false;

            if (tableIndex < 0)
            {
                if (!TryResolveFromSelectedEntry(rowIndex, out tableIndex, out realRowIndex)) return false;
            }
            else
            {
                // a table was given, so the row index is already a row of it
                if (tableIndex >= _filesContent.Tables.Count) return false;

                realRowIndex = _selection.GetRealTableRowIndex(_filesContent.Tables[tableIndex], rowIndex);
            }

            if (tableIndex < 0 || tableIndex >= _filesContent.Tables.Count || realRowIndex < 0) return false;

            tableData = new TableData(_filesContent.Tables[tableIndex], tableIndex);
            return true;
        }

        /// <summary>
        /// Resolves the target from the entry selected in the files list, mapping a row of that entry
        /// back to the file table and file row it was taken from.
        /// </summary>
        private bool TryResolveFromSelectedEntry(int gridRowIndex, out int tableIndex, out int realRowIndex)
        {
            tableIndex = -1;
            realRowIndex = -1;

            var listIndex = _selection.GetSelectedListIndex();
            if (listIndex < 0) return false;

            var filesListContent = _filesListContent;
            var entryTable = filesListContent?.GetTable(listIndex);
            if (entryTable == null) return false;

            var entryRowIndex = _selection.GetRealTableRowIndex(entryTable, gridRowIndex);
            if (entryRowIndex < 0) return false;

            return filesListContent.TryResolveRow(listIndex, entryRowIndex, out tableIndex, out realRowIndex);
        }
    }
}

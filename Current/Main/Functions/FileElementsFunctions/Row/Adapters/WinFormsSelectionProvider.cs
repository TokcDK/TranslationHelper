using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Reads the user's selection out of the WinForms controls: the files list and the work table
    /// grid.
    /// <para>
    /// This is an adapter and the only place in this folder, together with
    /// <see cref="WinFormsUiUpdater"/>, that is allowed to know about WinForms and about
    /// <see cref="AppData"/>. The row engine and the row operations talk to
    /// <see cref="ISelectionProvider"/> instead.
    /// </para>
    /// <para>
    /// Everything here has to marshal to the UI thread, because row operations can run on several
    /// threads while the controls may only be read from the one that created them.
    /// </para>
    /// </summary>
    internal class WinFormsSelectionProvider : ISelectionProvider
    {
        private readonly ListBox _filesList;
        private readonly DataGridView _dataGridView;

        public WinFormsSelectionProvider(ListBox filesList, DataGridView dataGridView)
        {
            _filesList = filesList;
            _dataGridView = dataGridView;
        }

        public int[] GetSelectedTableIndexes()
        {
            var tableIndexes = new List<int>();

            foreach (var listIndex in GetSelectedListIndexes())
            {
                foreach (var tableIndex in AppData.FilesListContent.GetTableIndexes(listIndex))
                {
                    if (!tableIndexes.Contains(tableIndex)) tableIndexes.Add(tableIndex);
                }
            }

            return tableIndexes.ToArray();
        }

        public TableRowPlan[] GetSelectedRowsPlan()
        {
            var listIndexes = GetSelectedListIndexes();
            if (listIndexes.Length != 1) return Array.Empty<TableRowPlan>();

            int listIndex = listIndexes[0];

            // Grid row -> row of the entry's table -> the file table and row it was taken from. The
            // last step is what lets the operation work on the "[ALL]" entry, whose rows come from
            // several files at once.
            var rowsByTable = new SortedDictionary<int, List<int>>();
            foreach (var gridRowIndex in GetSelectedGridRowIndexes())
            {
                var entryRowIndex = FunctionsTable.GetRealRowIndex(listIndex, gridRowIndex);
                if (entryRowIndex < 0) continue;

                if (!AppData.FilesListContent.TryResolveRow(listIndex, entryRowIndex, out var tableIndex, out var sourceRowIndex)) continue;

                if (!rowsByTable.TryGetValue(tableIndex, out var rows))
                {
                    rows = new List<int>();
                    rowsByTable[tableIndex] = rows;
                }

                if (!rows.Contains(sourceRowIndex)) rows.Add(sourceRowIndex);
            }

            var allTables = AppData.CurrentProject.FilesContent.Tables;
            var plan = new List<TableRowPlan>(rowsByTable.Count);
            foreach (var rowsOfTable in rowsByTable)
            {
                if (rowsOfTable.Key < 0 || rowsOfTable.Key >= allTables.Count) continue;

                var rows = rowsOfTable.Value;
                rows.Sort();
                plan.Add(new TableRowPlan(new TableData(allTables[rowsOfTable.Key], rowsOfTable.Key), rows.ToArray()));
            }

            return plan.ToArray();
        }

        public int GetSelectedListIndex()
        {
            if (_filesList.InvokeRequired)
            {
                int i = -1;
                _filesList.Invoke((Action)(() => i = _filesList.SelectedIndex));
                return i;
            }
            return _filesList.SelectedIndex;
        }

        public bool TryGetSingleSelectedGridRowIndex(out int gridRowIndex)
        {
            gridRowIndex = -1;

            var selected = GetSelectedGridRowIndexes();
            if (selected.Length != 1) return false;

            gridRowIndex = selected[0];
            return true;
        }

        public int GetRealTableRowIndex(DataTable table, int gridRowIndex)
        {
            return table.GetRealRowIndex(gridRowIndex);
        }

        /// <summary>
        /// Indexes of the entries selected in the files list.
        /// </summary>
        private int[] GetSelectedListIndexes()
        {
            if (_filesList.InvokeRequired)
            {
                int[] indexes = Array.Empty<int>();
                _filesList.Invoke((Action)(() => indexes = _filesList.CopySelectedIndexes()));
                return indexes;
            }
            return _filesList.CopySelectedIndexes();
        }

        /// <summary>
        /// Indexes of the rows the selection in the work table grid touches.
        /// </summary>
        private int[] GetSelectedGridRowIndexes()
        {
            if (_dataGridView.InvokeRequired)
            {
                int[] indexes = null;
                _dataGridView.Invoke((Action)(() => indexes = ReadSelectedGridRowIndexes()));
                return indexes ?? Array.Empty<int>();
            }
            return ReadSelectedGridRowIndexes();
        }

        private int[] ReadSelectedGridRowIndexes()
        {
            return _dataGridView.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(c => c.RowIndex)
                .Distinct()
                .OrderBy(i => i)
                .ToArray();
        }
    }
}

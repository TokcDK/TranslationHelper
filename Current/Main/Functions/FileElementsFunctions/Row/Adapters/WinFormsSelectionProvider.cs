using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Main.Functions;
using TranslationHelper.Models;
using TranslationHelper.Workspace;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Reads the user's selection out of one project's WinForms controls: its files list and the grid
    /// of the file it is showing.
    /// <para>
    /// This is an adapter and, together with <see cref="WinFormsUiUpdater"/>, the only place in this
    /// folder that is allowed to know about WinForms. The row engine and the row operations talk to
    /// <see cref="ISelectionProvider"/> instead.
    /// </para>
    /// <para>
    /// Everything here has to marshal to the UI thread, because row operations can run on several
    /// threads while the controls may only be read from the one that created them.
    /// </para>
    /// </summary>
    internal class WinFormsSelectionProvider : ISelectionProvider
    {
        /// <summary>
        /// The project whose selection is read. Held as a workspace rather than as a pair of controls
        /// because the content an entry presents — which is what turns an entry into a table and a row
        /// — belongs to the project as well.
        /// </summary>
        private readonly IProjectWorkspace _workspace;

        public WinFormsSelectionProvider(IProjectWorkspace workspace)
        {
            _workspace = workspace;
        }

        /// <summary>
        /// The files list of the project. Null while the project is not on screen.
        /// </summary>
        private FilesListControlBase FilesList => _workspace?.FilesList;

        /// <summary>
        /// The grid of the file the project is showing. Null while no file is shown.
        /// </summary>
        private DataGridView DataGridView => _workspace?.ActiveFileWorkspace?.ElementsDataGridView;

        /// <summary>
        /// The relation between a files list entry and the content it presents, for this project.
        /// </summary>
        private FilesListContent FilesListContent => _workspace?.Project?.FilesListContent;

        public int[] GetSelectedTableIndexes()
        {
            var filesListContent = FilesListContent;
            if (filesListContent == null) return Array.Empty<int>();

            var tableIndexes = new List<int>();

            foreach (var listIndex in GetSelectedListIndexes())
            {
                foreach (var tableIndex in filesListContent.GetTableIndexes(listIndex))
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

            var filesListContent = FilesListContent;
            if (filesListContent == null) return Array.Empty<TableRowPlan>();

            int listIndex = listIndexes[0];

            // Grid row -> row of the entry's table -> the file table and row it was taken from. The
            // last step is what lets the operation work on the "[ALL]" entry, whose rows come from
            // several files at once.
            var rowsByTable = new SortedDictionary<int, List<int>>();
            foreach (var gridRowIndex in GetSelectedGridRowIndexes())
            {
                var entryRowIndex = FunctionsTable.GetRealRowIndex(_workspace, listIndex, gridRowIndex);
                if (entryRowIndex < 0) continue;

                if (!filesListContent.TryResolveRow(listIndex, entryRowIndex, out var tableIndex, out var sourceRowIndex)) continue;

                if (!rowsByTable.TryGetValue(tableIndex, out var rows))
                {
                    rows = new List<int>();
                    rowsByTable[tableIndex] = rows;
                }

                if (!rows.Contains(sourceRowIndex)) rows.Add(sourceRowIndex);
            }

            var allTables = _workspace.Project.FilesContent.Tables;
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
            var filesList = FilesList;
            if (filesList == null) return -1;

            return filesList.GetSelectedIndex();
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
            var grid = DataGridView;
            if (grid == null) return -1;

            return table.GetRealRowIndex(grid, gridRowIndex);
        }

        /// <summary>
        /// Indexes of the entries selected in the files list.
        /// </summary>
        private int[] GetSelectedListIndexes()
        {
            var filesList = FilesList;
            if (filesList == null) return Array.Empty<int>();

            var control = filesList.FilesListControl;

            if (control.InvokeRequired)
            {
                int[] indexes = Array.Empty<int>();
                control.Invoke((Action)(() => indexes = filesList.GetSelectedIndexes()));
                return indexes;
            }

            return filesList.GetSelectedIndexes();
        }

        /// <summary>
        /// Indexes of the rows the selection in the work table grid touches.
        /// </summary>
        private int[] GetSelectedGridRowIndexes()
        {
            var grid = DataGridView;
            if (grid == null) return Array.Empty<int>();

            if (grid.InvokeRequired)
            {
                int[] indexes = null;
                grid.Invoke((Action)(() => indexes = ReadSelectedGridRowIndexes(grid)));
                return indexes ?? Array.Empty<int>();
            }

            return ReadSelectedGridRowIndexes(grid);
        }

        private static int[] ReadSelectedGridRowIndexes(DataGridView grid)
        {
            return grid.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(c => c.RowIndex)
                .Distinct()
                .OrderBy(i => i)
                .ToArray();
        }
    }
}

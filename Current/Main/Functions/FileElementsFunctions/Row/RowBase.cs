using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Represents a table and its index within the DataSet.
    /// </summary>
    public class TableData
    {
        public TableData(DataTable selectedTable, int selectedTableIndex)
        {
            SelectedTable = selectedTable;
            SelectedTableIndex = selectedTableIndex;
        }

        public DataTable SelectedTable { get; }
        public int SelectedTableIndex { get; }
    }

    /// <summary>
    /// Encapsulates information about a single DataRow in context of a TableData.
    /// </summary>
    public class RowBaseRowData
    {
        public RowBaseRowData(DataRow row, int rowIndex, TableData table)
        {
            SelectedRow = row;
            SelectedRowIndex = rowIndex;
            TableData = table;

            _workTableDatagridView = AppData.Main.THFileElementsDataGridView;
        }

        readonly DataGridView _workTableDatagridView;

        public TableData TableData { get; }
        public DataRow SelectedRow { get; }
        public int SelectedRowIndex { get; }
        public bool IsLastRow { get; set; }

        public static int ColumnIndexOriginal => AppData.CurrentProject.OriginalColumnIndex;
        public static int ColumnIndexTranslation => AppData.CurrentProject.TranslationColumnIndex;

        public string Original => SelectedRow.Field<string>(ColumnIndexOriginal);
        public string Translation
        {
            get => SelectedRow.Field<string>(ColumnIndexTranslation);
            set
            {
                if (_workTableDatagridView.InvokeRequired)
                {
                    // must fix winforms cross-thread access issue, app crash because of scrolling datagridview in time of rows are changing
                    // but this only for the rowbase functions and same changes from other places can cause the error
                    _workTableDatagridView.Invoke((MethodInvoker)delegate {
                        SelectedRow.SetField(ColumnIndexTranslation, value);
                    });
                }
                else
                {
                    SelectedRow.SetField(ColumnIndexTranslation, value);
                }                
            }
        }

        public DataTable SelectedTable => TableData.SelectedTable;
        public int SelectedTableIndex => TableData.SelectedTableIndex;
    }

    /// <summary>
    /// Base class for row-based operations across one or more DataTables.
    /// Supports single-row, multi-row, single-table, multi-table, and all-tables modes.
    /// </summary>
    internal abstract class RowBase
    {
        public virtual string Name { get; } = string.Empty;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region State fields
        protected bool NeedInit = true;
        protected Dictionary<string, string> SessionData;
        protected bool Ret;
        protected int TablesCount;
        protected int SelectedRowsCount;
        protected int SelectedRowsCountRest;
        private int _parsedCount;
        #endregion

        #region Mode flags
        protected bool IsAll { get; private set; }
        protected bool IsTables { get; private set; }
        protected bool IsTable { get; private set; }
        protected bool IsSelectedRows => !IsAll && !IsTables && !IsTable && SelectedRowsCount > 1;
        protected virtual bool IsParallelRows => false;
        protected virtual bool IsParallelTables => false;
        #endregion

        #region Cached static indices
        protected static int ColumnIndexOriginal => AppData.CurrentProject.OriginalColumnIndex;
        protected static int ColumnIndexTranslation => AppData.CurrentProject.TranslationColumnIndex;
        #endregion

        #region UI references
        protected readonly ListBox FilesList = AppData.THFilesList;
        protected readonly DataSet AllFiles = AppData.CurrentProject.FilesContent;
        protected readonly DataGridView WorkTableDatagridView = AppData.Main.THFileElementsDataGridView;
        #endregion

        #region Extension points (hooks)
        // The following hooks have been changed to return Task to support asynchronous execution.
        /// <summary>
        /// Actions initialization hook.
        /// </summary>
        protected virtual async Task ActionsInit()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? "!" : Name;
            Logger.Info(T._("{0}: initializing actions..."), name);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on multiple tables.
        /// </summary>
        protected virtual async Task ActionsPreTablesApply()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on a single table.
        /// </summary>
        protected virtual async Task ActionsPreTableApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on rows within a table.
        /// </summary>
        protected virtual async Task ActionsPreRowsApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on a single row.
        /// </summary>
        protected virtual async Task ActionsPreRowApply(RowBaseRowData rowData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on a single row.
        /// </summary>
        protected virtual async Task ActionsPostRowApply()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on rows within a table.
        /// </summary>
        protected virtual async Task ActionsPostRowsApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on a single table.
        /// </summary>
        protected virtual async Task ActionsPostTableApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on multiple tables.
        /// </summary>
        protected virtual async Task ActionsPostTablesApply()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Finalization hook.
        /// </summary>
        protected virtual async Task ActionsFinalize()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? "!" : Name;
            Logger.Info(T._("{0}: parsed {1} values"), name, _parsedCount);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Plays completion sound.
        /// </summary>
        protected virtual async Task CompleteSound()
        {
            System.Media.SystemSounds.Asterisk.Play();
            await Task.CompletedTask;
        }

        protected virtual bool IsOkSelected(TableData tableData) => true;
        protected virtual bool IsOkTable(TableData tableData) => true;
        protected virtual bool IsOkAll() => true;
        protected virtual bool IsValidRow(RowBaseRowData rowData) =>
            !AppSettings.IgnoreOrigEqualTransLines || !Equals(rowData.Original, rowData.Translation);
        protected abstract bool Apply(RowBaseRowData rowData);
        #endregion

        #region Public/internal APIs
        /// <summary>
        /// Process a single DataRow, using current selection if indices not provided.
        /// </summary>
        /// <param name="row">Optional DataRow. If null, the row at <paramref name="rowIndex"/> is used.</param>
        /// <param name="tableIndex">Index of table in DataSet; -1 to use UI selection.</param>
        /// <param name="rowIndex">Row index; -1 to use UI selection.</param>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Selected(DataRow row, int tableIndex = -1, int rowIndex = -1)
        {
            ResolveSingleContext(row, ref tableIndex, ref rowIndex, out var tableData, out var realRowIdx);
            if (tableData == null || !IsOkSelected(tableData))
                return false;

            return await ExecutePerTableAsync(new[] { tableData }, new[] { realRowIdx }).ConfigureAwait(false);
        }

        /// <summary>
        /// Process multiple selected rows in the currently selected table.
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Rows()
        {
            var tableIndexes = GetSelectedTableIndexes();
            if (tableIndexes.Length != 1)
                return false;

            var tableData = new TableData(AllFiles.Tables[tableIndexes[0]], tableIndexes[0]);
            if (!IsOkSelected(tableData))
                return false;

            var rowIndexes = GetSelectedRowIndexes(tableData);
            if (rowIndexes.Length == 0)
                return false;

            // full table if all rows are selected
            if (rowIndexes.Length == tableData.SelectedTable.Rows.Count)
                rowIndexes = null;

            return await ExecutePerTableAsync(new[] { tableData }, rowIndexes).ConfigureAwait(false);
        }

        /// <summary>
        /// Process one or more user-selected tables fully.
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Table()
        {
            if (!IsOkAll())
                return false;

            var tables = GetSelectedTableIndexes()
                .Select(idx => new TableData(AllFiles.Tables[idx], idx))
                .ToArray();
            if (tables.Length == 0)
                return false;

            return await ExecutePerTableAsync(tables, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Async wrapper for Table().
        /// </summary>
        internal async Task<bool> TableT() => await ExecutePerTableAsync(
            GetSelectedTableIndexes()
                .Select(idx => new TableData(AllFiles.Tables[idx], idx))
                .ToArray(), null).ConfigureAwait(false);

        /// <summary>
        /// Process every table in the DataSet.
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> All()
        {
            if (!IsOkAll())
                return false;

            var all = Enumerable.Range(0, AllFiles.Tables.Count)
                .Select(idx => new TableData(AllFiles.Tables[idx], idx))
                .ToArray();
            return await ExecutePerTableAsync(all, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Async wrapper for All().
        /// </summary>
        internal async Task<bool> AllT() => await ExecutePerTableAsync(
            Enumerable.Range(0, AllFiles.Tables.Count)
                .Select(idx => new TableData(AllFiles.Tables[idx], idx))
                .ToArray(), null).ConfigureAwait(false);
        #endregion

        #region Core execution
        /// <summary>
        /// Drives processing per table and per row.
        /// </summary>
        private async Task<bool> ExecutePerTableAsync(TableData[] tables, int[] specificRowIndexes)
        {
            Ret = false;
            ResetCounters(tables, specificRowIndexes);

            IsAll = specificRowIndexes == null && tables.Length == AllFiles.Tables.Count;
            IsTables = specificRowIndexes == null && tables.Length > 1;
            IsTable = specificRowIndexes == null && tables.Length == 1;

            await ActionsInit().ConfigureAwait(false);
            if (IsTables)
                await ActionsPreTablesApply().ConfigureAwait(false);

            foreach (var tableData in tables)
            {
                if (specificRowIndexes == null && !IsOkTable(tableData))
                    continue;

                await ActionsPreTableApply(tableData).ConfigureAwait(false);
                await ExecuteRowsAsync(tableData, specificRowIndexes).ConfigureAwait(false);
                await ActionsPostTableApply(tableData).ConfigureAwait(false);
            }

            if (IsTables)
                await ActionsPostTablesApply().ConfigureAwait(false);

            await ActionsFinalize().ConfigureAwait(false);
            if (IsTables || IsAll)
                await CompleteSound().ConfigureAwait(false);

            return Ret;
        }

        /// <summary>
        /// Iterate over specified or all rows in a table.
        /// </summary>
        private async Task ExecuteRowsAsync(TableData tableData, int[] rowIndexes)
        {
            int count = rowIndexes?.Length ?? tableData.SelectedTable.Rows.Count;
            SelectedRowsCount = count;
            SelectedRowsCountRest = count;
            await ActionsPreRowsApply(tableData).ConfigureAwait(false);

            if (IsParallelRows)
            {
                var tasks = new List<Task>();
                for (int i = 0; i < count; i++)
                {
                    int index = rowIndexes == null ? i : rowIndexes[i];
                    tasks.Add(Task.Run(() => ProcessRowSafeAsync(tableData, index)));
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int index = rowIndexes == null ? i : rowIndexes[i];
                    await ProcessRowSafeAsync(tableData, index).ConfigureAwait(false);
                }
            }

            await ActionsPostRowsApply(tableData).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs hooks and Apply() for a single row safely.
        /// </summary>
        private async Task ProcessRowSafeAsync(TableData tableData, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= tableData.SelectedTable.Rows.Count)
                return;
            await ProcessRowAsync(tableData, rowIndex).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs hooks and Apply() for a single row.
        /// </summary>
        private async Task ProcessRowAsync(TableData tableData, int rowIndex)
        {
            SelectedTable = tableData.SelectedTable;
            SelectedTableIndex = tableData.SelectedTableIndex;
            SelectedRowIndex = rowIndex;
            SelectedRow = tableData.SelectedTable.Rows[rowIndex];

            var rowData = new RowBaseRowData(SelectedRow, rowIndex, tableData)
            {
                IsLastRow = (--SelectedRowsCountRest == 0)
            };

            if (!IsValidRow(rowData))
                return;

            await ActionsPreRowApply(rowData).ConfigureAwait(false);
            try
            {
                if (Apply(rowData))
                {
                    Ret = true;
                    _parsedCount++;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to parse row. Error: {0}", ex);
            }
            await ActionsPostRowApply().ConfigureAwait(false);
        }
        #endregion

        #region Protected state properties
        protected DataTable SelectedTable { get; private set; }
        protected int SelectedTableIndex { get; private set; }
        protected DataRow SelectedRow { get; private set; }
        protected int SelectedRowIndex { get; private set; }
        #endregion

        #region Helpers
        private void ResetCounters(TableData[] tables, int[] specificRows)
        {
            _parsedCount = 0;
            TablesCount = tables.Length;
            SelectedRowsCount = specificRows == null
                ? tables.Sum(t => t.SelectedTable.Rows.Count)
                : specificRows.Length * tables.Length;
            SelectedRowsCountRest = SelectedRowsCount;
        }

        private int[] GetSelectedTableIndexes()
        {
            int[] indexes = Array.Empty<int>();
            if (FilesList.InvokeRequired)
            {
                FilesList.Invoke((Action)(() => indexes = FilesList.CopySelectedIndexes()));
            }
            else
            {
                indexes = FilesList.CopySelectedIndexes();
            }
            return indexes;
        }

        private int[] GetSelectedRowIndexes(TableData tableData)
        {
            int[] selectedCells;
            if (WorkTableDatagridView.InvokeRequired)
            {
                selectedCells = null;
                WorkTableDatagridView.Invoke((Action)(() =>
                    selectedCells = WorkTableDatagridView.SelectedCells
                        .Cast<DataGridViewCell>()
                        .Select(c => c.RowIndex)
                        .Distinct()
                        .Select(r => FunctionsTable.GetRealRowIndex(tableData.SelectedTableIndex, r))
                        .OrderBy(i => i)
                        .ToArray()
                ));
            }
            else
            {
                selectedCells = WorkTableDatagridView.SelectedCells
                    .Cast<DataGridViewCell>()
                    .Select(c => c.RowIndex)
                    .Distinct()
                    .Select(r => FunctionsTable.GetRealRowIndex(tableData.SelectedTableIndex, r))
                    .OrderBy(i => i)
                    .ToArray();
            }
            return selectedCells ?? Array.Empty<int>();
        }

        private void ResolveSingleContext(DataRow row, ref int tableIndex, ref int rowIndex,
            out TableData tableData, out int realRowIdx)
        {
            tableData = null;
            realRowIdx = -1;
            if (tableIndex < 0)
            {
                int i = -1;
                if (FilesList.InvokeRequired)
                {
                    FilesList.Invoke((Action)(() => i = FilesList.SelectedIndex));
                }
                else
                {
                    i = FilesList.SelectedIndex;
                }
                if (i < 0) return;
                tableIndex = i;
            }
            if (tableIndex < 0 || tableIndex >= AllFiles.Tables.Count)
                return;
            var table = AllFiles.Tables[tableIndex];
            tableData = new TableData(table, tableIndex);

            if (rowIndex < 0)
            {
                int[] selected;
                if (WorkTableDatagridView.InvokeRequired)
                {
                    selected = null;
                    WorkTableDatagridView.Invoke((Action)(() => selected = WorkTableDatagridView.GetSelectedRowsIndexes().ToArray()));
                }
                else
                {
                    selected = WorkTableDatagridView.GetSelectedRowsIndexes().ToArray();
                }
                if (selected == null || selected.Length != 1) return;
                rowIndex = selected[0];
            }

            realRowIdx = FunctionsTable.GetRealRowIndex(tableIndex, rowIndex);
        }
        #endregion
    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
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

        int GetSelectedTableIndex(); // For single selection.
    }

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

        public int GetSelectedTableIndex()
        {
            if (_filesList.InvokeRequired)
            {
                int i = -1;
                _filesList.Invoke((Action)(() => i = _filesList.SelectedIndex));
                return i;
            }
            return _filesList.SelectedIndex;
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

    /// <summary>
    /// The tables an operation has to be applied to and, for each of them, the rows to apply it to.
    /// A null <see cref="RowIndexes"/> means every row of the table.
    /// <para>
    /// A list of these is how an operation expresses its scope, because the entry selected in the
    /// files list is not necessarily one table: the "[ALL]" entry is several files at once.
    /// </para>
    /// </summary>
    public sealed class TableRowPlan
    {
        public TableRowPlan(TableData table, int[] rowIndexes)
        {
            Table = table;
            RowIndexes = rowIndexes;
        }

        /// <summary>
        /// The table to apply the operation to.
        /// </summary>
        public TableData Table { get; }

        /// <summary>
        /// The rows of that table to apply it to, or null for every row of it.
        /// </summary>
        public int[] RowIndexes { get; }
    }

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

    public interface IUiUpdater
    {
        void SetTranslation(DataRow row, int columnIndex, string value);
    }

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
    }

    /// <summary>
    /// Encapsulates information about a single DataRow in context of a TableData.
    /// </summary>
    public class RowBaseRowData
    {
        ProjectBase Project { get; }

        IUiUpdater UiUpdater { get; }

        public RowBaseRowData(ProjectBase project, DataRow row, int rowIndex, TableData table, IUiUpdater uiUpdater = null)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            SelectedRow = row;
            SelectedRowIndex = rowIndex;
            TableData = table;

            _workTableDatagridView = AppData.Main.THFileElementsDataGridView;

            UiUpdater = uiUpdater ?? new WinFormsUiUpdater(_workTableDatagridView);
        }

        readonly DataGridView _workTableDatagridView;

        public TableData TableData { get; }
        public DataRow SelectedRow { get; }
        public int SelectedRowIndex { get; }
        public bool IsLastRow { get; set; }

        public int ColumnIndexOriginal => Project.OriginalColumnIndex;
        public int ColumnIndexTranslation => Project.TranslationColumnIndex;

        public string Original => SelectedRow.Field<string>(ColumnIndexOriginal);
        public string Translation
        {
            get => SelectedRow.Field<string>(ColumnIndexTranslation);
            set => UiUpdater.SetTranslation(SelectedRow, ColumnIndexTranslation, value);
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
        public class ProcessingState
        {
            public int ParsedCount;
            public int TablesCount { get; private set; }
            public int SelectedRowsCount { get; set; }
            public int SelectedRowsCountRest { get; set; }

            public void Reset(TableRowPlan[] plan)
            {
                ParsedCount = 0;
                TablesCount = plan.Length;
                SelectedRowsCount = plan.Sum(entry => entry.RowIndexes?.Length ?? entry.Table.SelectedTable.Rows.Count);
                SelectedRowsCountRest = SelectedRowsCount;
            }

            public void IncrementParsed() => Interlocked.Increment(ref ParsedCount); // Thread-safe for parallel.
        }

        public virtual string Name { get; } = string.Empty;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected ProjectBase Project { get; } = AppData.CurrentProject;
        protected readonly ISelectionProvider SelectionProvider;
        protected ProcessingState State { get; } = new ProcessingState();

        #region Constructor
        internal RowBase(ISelectionProvider selectionProvider = null)
        {
            SelectionProvider = selectionProvider ?? new WinFormsSelectionProvider(AppData.THFilesList, AppData.Main.THFileElementsDataGridView);
        }
        #endregion

        #region State fields
        protected bool NeedInit = true;
        protected Dictionary<string, string> SessionData;
        protected bool Ret;
        protected int TablesCount;
        protected int SelectedRowsCount => State.SelectedRowsCount;
        protected int SelectedRowsCountRest => State.SelectedRowsCountRest;
        //private int _parsedCount;
        #endregion

        #region Mode flags
        protected bool IsAll { get; private set; }
        protected bool IsTables { get; private set; }
        protected bool IsTable { get; private set; }
        protected bool IsSelectedRows => !IsAll && !IsTables && !IsTable && State.SelectedRowsCount > 1;
        protected virtual bool IsParallelRows => false;
        protected virtual bool IsParallelTables => false;
        #endregion

        #region Cached static indices
        protected int ColumnIndexOriginal => Project.OriginalColumnIndex;
        protected int ColumnIndexTranslation => Project.TranslationColumnIndex;
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
            var name = string.IsNullOrWhiteSpace(Name) ? GetType().Name : Name;
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
            var name = string.IsNullOrWhiteSpace(Name) ? GetType().Name : Name;
            Logger.Info(T._("{0}: parsed {1} values"), name, State.ParsedCount);
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

            return await ExecutePlanAsync(new[] { new TableRowPlan(tableData, new[] { realRowIdx }) }).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the rows selected in the work table grid.
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Rows()
        {
            var plan = SelectionProvider.GetSelectedRowsPlan();
            if (plan.Length == 0)
                return false;

            if (plan.Length == 1 && !IsOkSelected(plan[0].Table))
                return false;

            // a selection that covers every row of the one table it touches is the whole table
            if (plan.Length == 1 && plan[0].RowIndexes != null && plan[0].RowIndexes.Length == plan[0].Table.SelectedTable.Rows.Count)
            {
                plan = new[] { new TableRowPlan(plan[0].Table, null) };
            }

            return await ExecutePlanAsync(plan).ConfigureAwait(false);
        }

        /// <summary>
        /// Process one or more user-selected entries fully. The "[ALL]" entry stands for every file,
        /// so selecting it applies the operation to all of them.
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Table()
        {
            if (!IsOkAll())
                return false;

            var plan = GetSelectedEntriesPlan();
            if (plan.Length == 0)
                return false;

            return await ExecutePlanAsync(plan).ConfigureAwait(false);
        }

        /// <summary>
        /// Async wrapper for Table().
        /// </summary>
        internal async Task<bool> TableT() => await ExecutePlanAsync(GetSelectedEntriesPlan()).ConfigureAwait(false);

        /// <summary>
        /// Process every table in the DataSet.
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> All()
        {
            if (!IsOkAll())
                return false;

            return await ExecutePlanAsync(GetAllTablesPlan()).ConfigureAwait(false);
        }

        /// <summary>
        /// Async wrapper for All().
        /// </summary>
        internal async Task<bool> AllT() => await ExecutePlanAsync(GetAllTablesPlan()).ConfigureAwait(false);

        /// <summary>
        /// Whole-table plan for the entries selected in the files list.
        /// </summary>
        private TableRowPlan[] GetSelectedEntriesPlan()
        {
            return SelectionProvider.GetSelectedTableIndexes()
                .Where(idx => idx >= 0 && idx < AllFiles.Tables.Count)
                .Select(idx => new TableRowPlan(new TableData(AllFiles.Tables[idx], idx), null))
                .ToArray();
        }

        /// <summary>
        /// Whole-table plan for every table of the DataSet.
        /// </summary>
        private TableRowPlan[] GetAllTablesPlan()
        {
            return Enumerable.Range(0, AllFiles.Tables.Count)
                .Select(idx => new TableRowPlan(new TableData(AllFiles.Tables[idx], idx), null))
                .ToArray();
        }
        #endregion

        #region Core execution
        /// <summary>
        /// Drives processing per table and per row.
        /// </summary>
        private async Task<bool> ExecutePlanAsync(TableRowPlan[] plan)
        {
            Ret = false;
            ResetCounters(plan);

            // "every row of every table", "every row of several tables" and "every row of one table"
            // are still told apart, because the hooks and the completion sound depend on that.
            bool wholeTables = plan.All(entry => entry.RowIndexes == null);
            IsAll = wholeTables && plan.Length == AllFiles.Tables.Count;
            IsTables = wholeTables && plan.Length > 1;
            IsTable = wholeTables && plan.Length == 1;

            await ActionsInit().ConfigureAwait(false);
            if (IsTables)
                await ActionsPreTablesApply().ConfigureAwait(false);

            foreach (var entry in plan)
            {
                if (entry.RowIndexes == null && !IsOkTable(entry.Table))
                    continue;

                await ActionsPreTableApply(entry.Table).ConfigureAwait(false);
                await ExecuteRowsAsync(entry.Table, entry.RowIndexes).ConfigureAwait(false);
                await ActionsPostTableApply(entry.Table).ConfigureAwait(false);
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
            State.SelectedRowsCount = count;
            State.SelectedRowsCountRest = count;
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

            var rowData = new RowBaseRowData(Project, SelectedRow, rowIndex, tableData)
            {
                IsLastRow = (--State.SelectedRowsCountRest == 0)
            };

            try
            {
                if (!IsValidRow(rowData))
                    return;
            }
            catch
            {
                Logger.Warn("Error to check row valid");
                return;
            }

            await ActionsPreRowApply(rowData).ConfigureAwait(false);
            try
            {
                if (Apply(rowData))
                {
                    Ret = true;

                    State.IncrementParsed();
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
        private void ResetCounters(TableRowPlan[] plan)
        {
            State.Reset(plan);
        }

        /// <summary>
        /// Work out which file table and row a single-row operation has to work on.
        /// </summary>
        /// <param name="row">Unused; the row is identified by the indexes.</param>
        /// <param name="tableIndex">
        /// Index in the project content of the table to work on, or -1 to use the entry selected in
        /// the files list. That entry may be the "[ALL]" entry, in which case the row is resolved to
        /// the file it was taken from.
        /// </param>
        /// <param name="rowIndex">Row of the work table grid, or -1 to use the selected one.</param>
        /// <param name="tableData">The file table to work on, or null when it could not be resolved.</param>
        /// <param name="realRowIdx">Index of the row in that file table.</param>
        private void ResolveSingleContext(DataRow row, ref int tableIndex, ref int rowIndex,
            out TableData tableData, out int realRowIdx)
        {
            tableData = null;
            realRowIdx = -1;

            if (rowIndex < 0 && !TryGetSingleSelectedGridRowIndex(out rowIndex)) return;

            if (tableIndex < 0)
            {
                int listIndex = -1;
                if (FilesList.InvokeRequired)
                {
                    FilesList.Invoke((Action)(() => listIndex = FilesList.SelectedIndex));
                }
                else
                {
                    listIndex = FilesList.SelectedIndex;
                }
                if (listIndex < 0) return;

                var entryTable = AppData.FilesListContent?.GetTable(listIndex);
                if (entryTable == null) return;

                var entryRowIndex = entryTable.GetRealRowIndex(rowIndex);
                if (entryRowIndex < 0) return;

                if (!AppData.FilesListContent.TryResolveRow(listIndex, entryRowIndex, out tableIndex, out realRowIdx)) return;
            }
            else
            {
                // a table was given, so the row index is already a row of it
                if (tableIndex >= AllFiles.Tables.Count) return;

                realRowIdx = AllFiles.Tables[tableIndex].GetRealRowIndex(rowIndex);
            }

            if (tableIndex < 0 || tableIndex >= AllFiles.Tables.Count || realRowIdx < 0) return;

            tableData = new TableData(AllFiles.Tables[tableIndex], tableIndex);
        }

        /// <summary>
        /// The one row selected in the work table grid, when exactly one row is selected.
        /// </summary>
        private bool TryGetSingleSelectedGridRowIndex(out int gridRowIndex)
        {
            gridRowIndex = -1;

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

            if (selected == null || selected.Length != 1) return false;

            gridRowIndex = selected[0];
            return true;
        }
        #endregion
    }
}

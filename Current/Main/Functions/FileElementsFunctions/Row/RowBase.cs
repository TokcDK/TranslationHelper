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

    public class RowBaseRowData
    {
        public RowBaseRowData(DataRow row, int rowIndex, TableData table)
        {
            SelectedRow = row;
            SelectedRowIndex = rowIndex;
            TableData = table;
        }

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
            set => SelectedRow.SetValue(ColumnIndexTranslation, value);
        }

        public DataTable SelectedTable => TableData.SelectedTable;
        public int SelectedTableIndex => TableData.SelectedTableIndex;
    }

    internal abstract class RowBase
    {
        public virtual string Name { get; } = string.Empty;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Base fields
        protected bool NeedInit = true;
        protected Dictionary<string, string> SessionData;
        protected bool Ret;
        protected int TablesCount;
        protected int SelectedRowsCount;
        protected int SelectedRowsCountRest;
        private int _parsedCount;
        #endregion

        #region Flags
        protected bool IsAll { get; private set; }
        protected bool IsTables { get; private set; }
        protected bool IsTable { get; private set; }
        protected bool IsSelectedRows => !IsAll && !IsTables && !IsTable && SelectedRowsCount > 1;
        protected virtual bool IsParallelRows => false;
        protected virtual bool IsParallelTables => false;
        #endregion

        #region Cached static properties
        protected static int ColumnIndexOriginal => AppData.CurrentProject.OriginalColumnIndex;
        protected static int ColumnIndexTranslation => AppData.CurrentProject.TranslationColumnIndex;
        #endregion

        #region UI Components
        protected readonly ListBox FilesList = AppData.THFilesList;
        protected readonly DataSet AllFiles = AppData.CurrentProject.FilesContent;
        protected readonly DataGridView WorkTableDatagridView = AppData.Main.THFileElementsDataGridView;
        #endregion

        #region Abstract/Virtual hooks
        protected virtual bool IsOkSelected(TableData tableData) => true;
        protected virtual bool IsOkTable(TableData tableData) => true;
        protected virtual bool IsOkAll() => true;
        protected virtual void ActionsInit() { }
        protected virtual void ActionsPreTablesApply() { }
        protected virtual void ActionsPreTableApply(TableData tableData) { }
        protected virtual void ActionsPreRowsApply(TableData tableData) { }
        protected virtual void ActionsPreRowApply(RowBaseRowData rowData) { }
        protected virtual bool IsValidRow(RowBaseRowData rowData) =>
            !AppSettings.IgnoreOrigEqualTransLines || !Equals(rowData.Original, rowData.Translation);
        protected abstract bool Apply(RowBaseRowData rowData);
        protected virtual void ActionsPostRowApply() { }
        protected virtual void ActionsPostRowsApply(TableData tableData) { }
        protected virtual void ActionsPostTableApply(TableData tableData) { }
        protected virtual void ActionsPostTablesApply() { }
        protected virtual void ActionsFinalize()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? "!" : Name;
            Logger.Info(T._("{0}: parsed {1} values"), name, _parsedCount);
        }
        protected virtual void CompleteSound() => System.Media.SystemSounds.Asterisk.Play();
        #endregion

        #region Public APIs
        internal bool Selected(DataRow row, int tableIndex = -1, int rowIndex = -1)
        {
            ResolveSingleContext(row, ref tableIndex, ref rowIndex, out var tableData, out var realRowIdx);
            if (tableData == null) return false;
            if (!IsOkSelected(tableData)) return false;
            return ExecutePerTable(
                new[] { tableData },
                new[] { realRowIdx }
            );
        }

        internal bool Rows()
        {
            var tableIndexes = GetSelectedTableIndexes();
            if (tableIndexes.Length != 1) return false;
            var tableData = new TableData(AllFiles.Tables[tableIndexes[0]], tableIndexes[0]);
            if (!IsOkSelected(tableData)) return false;

            var rowIndexes = GetSelectedRowIndexes(tableData);
            if (rowIndexes.Length == 0) return false;

            // if all rows selected, treat as full table
            if (rowIndexes.Length == tableData.SelectedTable.Rows.Count)
                return ExecutePerTable(new[] { tableData }, null);

            return ExecutePerTable(
                new[] { tableData },
                rowIndexes
            );
        }

        internal bool Table()
        {
            if (!IsOkAll()) return false;
            var tables = GetSelectedTableIndexes()
                .Select(idx => new TableData(AllFiles.Tables[idx], idx))
                .ToArray();
            if (tables.Length == 0) return false;
            return ExecutePerTable(tables, null);
        }

        internal async Task<bool> TableT() => await Task.Run(Table).ConfigureAwait(false);

        internal bool All()
        {
            if (!IsOkAll()) return false;
            var tables = Enumerable.Range(0, AllFiles.Tables.Count)
                .Select(idx => new TableData(AllFiles.Tables[idx], idx))
                .ToArray();
            if (tables.Length == 0) return false;
            return ExecutePerTable(tables, null);
        }

        internal async Task<bool> AllT() => await Task.Run(All).ConfigureAwait(false);
        #endregion

        #region Core logic
        private bool ExecutePerTable(TableData[] tables, int[] specificRowIndexes)
        {
            Ret = false;
            ResetCounters(tables, specificRowIndexes);
            // set flags
            IsAll = specificRowIndexes == null && tables.Length == AllFiles.Tables.Count;
            IsTables = specificRowIndexes == null && tables.Length > 1;
            IsTable = specificRowIndexes == null && tables.Length == 1;

            ActionsInit();
            if (IsTables) ActionsPreTablesApply();

            foreach (var tableData in tables)
            {
                if (specificRowIndexes == null)
                {
                    if (!IsOkTable(tableData)) continue;
                    ActionsPreTableApply(tableData);
                    ExecuteRows(tableData, Enumerable.Range(0, tableData.SelectedTable.Rows.Count).ToArray());
                    ActionsPostTableApply(tableData);
                }
                else
                {
                    ActionsPreTableApply(tableData);
                    ExecuteRows(tableData, specificRowIndexes);
                    ActionsPostTableApply(tableData);
                }
            }

            if (IsTables) ActionsPostTablesApply();
            ActionsFinalize();
            if (IsTables || IsAll) CompleteSound();

            return Ret;
        }

        private void ExecuteRows(TableData tableData, int[] rowIndexes)
        {
            SelectedRowsCount = rowIndexes.Length;
            SelectedRowsCountRest = SelectedRowsCount;
            ActionsPreRowsApply(tableData);

            if (IsParallelRows)
            {
                Parallel.ForEach(rowIndexes, rowIndex => ProcessRow(tableData, rowIndex));
            }
            else
            {
                foreach (var rowIndex in rowIndexes)
                    ProcessRow(tableData, rowIndex);
            }

            ActionsPostRowsApply(tableData);
        }

        private void ProcessRow(TableData tableData, int rowIndex)
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

            ActionsPreRowApply(rowData);
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
            ActionsPostRowApply();
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
            if (specificRows != null)
                SelectedRowsCount = specificRows.Length * tables.Length;
            else
                SelectedRowsCount = tables.Sum(t => t.SelectedTable.Rows.Count);
            SelectedRowsCountRest = SelectedRowsCount;
        }

        private int[] GetSelectedTableIndexes()
        {
            int[] indexes = Array.Empty<int>();
            FilesList.Invoke((Action)(() => indexes = FilesList.CopySelectedIndexes()));
            return indexes;
        }

        private int[] GetSelectedRowIndexes(TableData tableData)
        {
            var distinctRows = WorkTableDatagridView.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(c => c.RowIndex)
                .Distinct()
                .Select(r => FunctionsTable.GetRealRowIndex(tableData.SelectedTableIndex, r))
                .OrderBy(i => i)
                .ToArray();
            return distinctRows;
        }

        private void ResolveSingleContext(DataRow row, ref int tableIndex, ref int rowIndex,
            out TableData tableData, out int realRowIdx)
        {
            tableData = null;
            realRowIdx = -1;
            if (tableIndex < 0)
            {
                int tInd = 0;
                FilesList.Invoke((Action)(() => tInd = FilesList.SelectedIndex));
                if (tableIndex < 0) return;

                tableIndex = tInd;
            }
            var table = AllFiles.Tables[tableIndex];
            tableData = new TableData(table, tableIndex);

            if (rowIndex < 0)
            {
                var selected = WorkTableDatagridView.GetSelectedRowsIndexes().ToArray();
                if (selected.Length != 1) return;
                rowIndex = selected.First();
            }

            realRowIdx = FunctionsTable.GetRealRowIndex(tableIndex, rowIndex);
            // row parameter ignored; we set SelectedRow via table lookup
        }
        #endregion
    }
}

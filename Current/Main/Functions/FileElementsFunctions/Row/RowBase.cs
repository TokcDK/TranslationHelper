﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Data.Interfaces;
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

        public DataTable SelectedTable { get; set; }
        public int SelectedTableIndex { get; set; }
    }

    public class RowData
    {
        public RowData(DataRow row, int rowIndex, TableData table) 
        { 
            SelectedRow = row;
            SelectedRowIndex = rowIndex;
            TableData = table;
        }
        public static int ColumnIndexOriginal { get => AppData.CurrentProject.OriginalColumnIndex; }
        public static int ColumnIndexTranslation { get => AppData.CurrentProject.TranslationColumnIndex; }
        public TableData TableData { get; }

        public DataRow SelectedRow { get; }
        public int SelectedRowIndex { get; }
        public bool IsLastRow { get; set; } = false;

        public string Original { get => SelectedRow.Field<string>(ColumnIndexOriginal); }

        public string Translation
        {
            get => SelectedRow.Field<string>(ColumnIndexTranslation);
            set => SelectedRow.SetValue(ColumnIndexTranslation, value);
        }
    }

    internal abstract class RowBase : IOriginalTranslationUser
    {
        /// <summary>
        /// execute one time
        /// </summary>
        protected bool NeedInit = true;

        /// <summary>
        /// init some vars
        /// </summary>
        protected virtual void Init()
        {
            if (!NeedInit) return;

            SelectedRowsCount = 0;
            SelectedRowsCountRest = 0;
            NeedInit = false;
        }

        protected readonly ListBox FilesList = AppData.THFilesList;
        protected readonly DataSet AllTables = AppData.CurrentProject.FilesContent;
        protected readonly DataGridView WorkTableDatagridView = AppData.Main.THFileElementsDataGridView;

        /// <summary>
        /// return value for Table/All functions. Depends on return of Apply
        /// </summary>
        protected bool Ret;

        /// <summary>
        /// an be used by some functions
        /// </summary>
        protected Dictionary<string, string> SessionData;

        /// <summary>
        /// true when SelectedRow(row) was executed from Selected()
        /// </summary>
        bool _isInternalSelectedRowExecution = false;

        /// <summary>
        /// proceed 1 selected row
        /// </summary>
        /// <returns></returns>
        internal bool Selected(DataRow row, int tableIndex, int rowIndex)
        {
            var table = AppData.CurrentProject.FilesContent.Tables[tableIndex];
        }

        bool Selected(RowData rowData)
        {
            try
            {
                if (!_isInternalSelectedRowExecution)
                {
                    Init();

                    GetTableData(rowData.TableData);

                    if (IsSelectedRows) SelectedRowsCount = 1;
                }

                //SelectedRow = rowData.SelectedRow;
                //SelectedRowIndex = GetRowIndex(rowData.ro);

                if (!IsAll && !IsTables && !IsTable && !IsSelectedRows) ActionsInit();

                ActionsPreRowApply(rowData);

                ApplyConditions(rowData);
            }
            catch (Exception ex)
            {
                _log.LogToFile($"An error occurred in the Selected method. Error: {ex}");
            }

            ActionsPostRowApply();

            if (!IsAll && !IsTables && !IsTable && !IsSelectedRows) ActionsFinalize();

            return Ret;
        }

        /// <summary>
        /// Check before processing Selected().
        /// </summary>
        /// <returns>State if Selected() can be continue</returns>
        protected virtual bool IsOkSelected() => true;

        /// <summary>
        /// Check before processing Table().
        /// </summary>
        /// <returns>State if Table() can be continue</returns>
        protected virtual bool IsOkTable() => true;

        /// <summary>
        /// Check before processing All().
        /// </summary>
        /// <returns>State if All() can be continue</returns>
        protected virtual bool IsOkAll() => true;

        ///// <summary>
        ///// get index of selected row
        ///// </summary>
        ///// <param name="rowIndex"></param>
        ///// <returns></returns>
        //private int GetRowIndex(int rowIndex = -1)
        //{
        //    if (rowIndex != -1) { SelectedRowIndex = rowIndex; }
        //    else if (SelectedRowIndex == -1) SelectedRowIndex = SelectedTable.Rows.IndexOf(SelectedRow);

        //    return SelectedRowIndex;
        //}

        /// <summary>
        /// Application log
        /// </summary>
        protected readonly FunctionsLogs _log = new FunctionsLogs();

        /// <summary>
        /// True when !IsAll && !IsTables && !IsTable
        /// </summary>
        protected bool IsSelectedRows { get => !IsAll && !IsTables && !IsTable && SelectedRowsCount > 1; }

        /// <summary>
        /// selected rows count
        /// </summary>
        protected int SelectedRowsCount = 0;
        /// <summary>
        /// proceed 1 or more of selected rows
        /// </summary>
        /// <returns></returns>
        internal bool Rows(TableData tableData)
        {
            if (!IsOkSelected()) return false;

            Init();

            SelectedRowsCount = WorkTableDatagridView.GetSelectedRowsCount();

            if (SelectedRowsCount <= 0) return Ret;

            try
            {
                GetTableData(tableData);

                // parse table instead of selected cells when all cells in the table are selected
                if (SelectedRowsCount == tableData.SelectedTable.Rows.Count) return Table();

                _isInternalSelectedRowExecution = true;

                if (IsSelectedRows)
                {
                    ActionsInit();

                    ActionsPreRowsApply(); // need here also when not table but selected rows more of one
                }

                var selectedRowIndexses = new int[SelectedRowsCount];
                var addedRows = new HashSet<int>(SelectedRowsCount);
                var dgvSelectedCells = WorkTableDatagridView.SelectedCells;
                var dgvSelectedCellsCount = dgvSelectedCells.Count;
                var ind = 0; // index for SelectedRowIndexses
                for (int i = 0; i < dgvSelectedCellsCount; i++)
                {
                    var dgvRowIndex = dgvSelectedCells[i].RowIndex;

                    if (addedRows.Contains(dgvRowIndex)) continue; // skip if parent row index already was added

                    addedRows.Add(dgvRowIndex); // add row index as added

                    //add row index
                    selectedRowIndexses[ind] = FunctionsTable.GetRealRowIndex(tableData.SelectedTableIndex, dgvRowIndex);

                    ind++; //raise added index
                }

                if (IsSelectedRows) Array.Sort(selectedRowIndexses);//sort indexes

                // here could be parallel foreach but there is some issues with it because IsLastRow, Original, Translation and other variables
                // need make var like IsLastRow avalaible only for parsing row
                Parallel.ForEach(selectedRowIndexses, rowIndex =>
                {
                    var rowData = new RowData(tableData.SelectedTable.Rows[rowIndex], rowIndex, tableData);
                    Selected(rowData);
                });
                //foreach (int rowIndex in selectedRowIndexses) Selected(tableData.SelectedTable.Rows[rowIndex]);

                if (IsSelectedRows)
                {
                    ActionsPostRowsApply(); // when selected more of one row

                    ActionsFinalize();
                }
            }
            catch (Exception ex) { _log.LogToFile("an error occured in base row function. error=\r\n" + ex); }

            return Ret;
        }

        /// <summary>
        /// apply the actions before selected row will be parsed
        /// will be executed in any case
        /// </summary>
        protected virtual void ActionsPreRowApply(RowData rowData) { }

        /// <summary>
        /// apply the actions before all rows for selected,table or all was applied
        /// will be executed if parse more of one row
        /// </summary>
        protected virtual void ActionsPreRowsApply() { }

        /// <summary>
        /// apply the actions before several tables will be parsed
        /// will be executed when parse more of one table
        /// </summary>
        protected virtual void ActionsPreTablesApply() { }

        /// <summary>
        /// apply the actions before selected table wil be processed
        /// will be executed when one or more tables parsed
        /// </summary>
        protected virtual void ActionsPreTableApply() { }

        /// <summary>
        /// apply the actions before selected type of object will be parsed
        /// will be executed always before all elements of selected type of object and before any other actions
        /// </summary>
        protected virtual void ActionsInit() { }

        /// <summary>
        /// apply the actions after row was parsed
        /// will be executed in any case
        /// </summary>
        protected virtual void ActionsPostRowApply() { }

        /// <summary>
        /// apply the actions after several rows was parsed
        /// will be executed after all rows of table or more of one selected rows was parsed
        /// </summary>
        protected virtual void ActionsPostRowsApply() { }

        /// <summary>
        /// apply the actions after selected table will be processed
        /// will be executed each time when table was parsed
        /// </summary>
        protected virtual void ActionsPostTableApply() { }

        /// <summary>
        /// apply the actions after all tables was parsed
        /// will be executed after all tables was parsed
        /// </summary>
        protected virtual void ActionsPostTablesApply() { }

        /// <summary>
        /// apply the actions after all selected type of object will be parsed
        /// will be executed always last, after all elements of selected type of object and after any other actions
        /// </summary>
        protected virtual void ActionsFinalize() { }

        /// <summary>
        /// init selected table's data
        /// </summary>
        /// <param name="tableIndex"></param>
        private void GetTableData(TableData tableData)
        {
            if (tableData.SelectedTableIndex == -1)
            {
#if DEBUG
                AppData.Main.Invoke((Action)(() => tableData.SelectedTableIndex = AppData.Main.THFilesList.GetSelectedIndex()));
#else
                tableData.SelectedTableIndex = AppData.Main.THFilesList.GetSelectedIndex();
#endif
            }

            if (tableData.SelectedTable == null) tableData.SelectedTable = AllTables.Tables[tableData.SelectedTableIndex];

            if (needToGetOrigTransColumnsNum)
            {
                needToGetOrigTransColumnsNum = false;
            }
        }

        /// <summary>
        /// only one time get Original and Translation columns numbers
        /// </summary>
        bool needToGetOrigTransColumnsNum = true;

        /// <summary>
        /// proceed selected table. Multithreaded.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> TableT() => await Task.Run(() => Table()).ConfigureAwait(false);

        /// <summary>
        /// true when processed one table
        /// </summary>
        protected bool IsTable;
        protected static int ColumnIndexOriginal { get => AppData.CurrentProject.OriginalColumnIndex; }
        protected static int ColumnIndexTranslation { get => AppData.CurrentProject.TranslationColumnIndex; }

        /// <summary>
        /// proceed selected table
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        internal bool Table(TableData tableData)
        {
            //SelectedTable = dataTable;

            Init();

            if (!IsAll && !IsTables)
            {
                IsTable = true;
                TablesCount = 1;
            }

            GetTableData(tableData);

            if (tableData.SelectedTableIndex == -1) return false; // return when table is not selected

            if (!IsValidTable(tableData)) return false;

            if (!IsAll && !IsTables) ActionsInit();

            ActionsPreTableApply();

            ActionsPreRowsApply();

            _isInternalSelectedRowExecution = true;

            var rowsCount = tableData.SelectedTable.Rows.Count;
            if (!IsAll && !IsTables && IsTable) SelectedRowsCount = rowsCount; //|| (IsAll && SelectedTableIndex == tablescount - 1)set rows count to selectedrowscount for last table but forgot for which purpose it is

            Parallel
            for (int i = 0; i < rowsCount; i++)
            {
                Selected(SelectedTable.Rows[i], SelectedTableIndex, i);
            }

            ActionsPostRowsApply(); // need here also as in All because must be executed even if only one table was selected

            ActionsPostTableApply();

            if (!IsAll && !IsTables) ActionsFinalize();

            return Ret;
        }

        /// <summary>
        /// proceed selected tables
        /// </summary>
        /// <returns></returns>
        internal bool Table()
        {
            if (!IsOkTable()) return false;

            Init();

            int[] selectedTableIndexes = null;
#if DEBUG
            FilesList.Invoke((Action)(() => selectedTableIndexes = FilesList.CopySelectedIndexes()));
#else
            tableindexes = _filesList.CopySelectedIndexes();
#endif
            DataTable[] selectedTables = null;
#if DEBUG
            AppData.Main.Invoke((Action)(() => selectedTables = AllTables.GetTablesByIndexes(selectedTableIndexes)));
#else
            tables = _allTables.GetTablesByIndexes(tableindexes);
#endif
            TablesCount = selectedTables.Length;
            IsTables = TablesCount > 1;

            if (!IsAll && IsTables)
            {
                SetSelectedRowsCountForTables(selectedTables);

                ActionsInit();

                ActionsPreTablesApply();
            }

            Parallel.ForEach(selectedTables, table =>
            {
                var tableData = new TableData(); 
                Table(table);
            });

            //foreach (var table in selectedTables) Table(table);

            if (!IsAll && IsTables)
            {
                ActionsPostTablesApply();

                ActionsFinalize();

                CompleteSound();
            }

            return Ret;
        }

        /// <summary>
        /// Check if table is valid. Can be used by any finction to not make same check in line checking for perfomance issues.
        /// </summary>
        /// <param name="table"></param>
        /// <returns>Default is always true</returns>
        protected virtual bool IsValidTable(TableData tableData) => true;

        /// <summary>
        /// True when selected more of one table
        /// </summary>
        protected bool IsTables;

        /// <summary>
        /// Proceed all tables. Multithreaded.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> AllT() => await Task.Run(() => All()).ConfigureAwait(false);

        //DataTable SelectedTable;
        //int SelectedTableIndex = -1;
        //DataRow SelectedRow;
        //int SelectedRowIndex;
        protected int TablesCount = 0;
        /// <summary>
        /// true when processed all tables
        /// </summary>
        protected bool IsAll;
        /// <summary>
        /// Proceed all tables
        /// </summary>
        /// <returns></returns>
        internal bool All()
        {
            if (!IsOkAll()) return false;

            IsAll = true;

            Init();

            var allTables = AllTables.Tables;
            TablesCount = allTables.Count;

            SetSelectedRowsCountForAll();

            ActionsInit();
            ActionsPreTablesApply();

            for (int selectedTableIndex = 0; selectedTableIndex < TablesCount; selectedTableIndex++)
            {
                var selectedTable = allTables[selectedTableIndex];
                var tableData = new TableData(selectedTable, selectedTableIndex);

                try
                {
                    Table(tableData);
                }
                catch (Exception ex)
                {
                    _log.LogToFile($"An error occurred while parsing all tables in method '{nameof(All)}'. Error: {ex}");
                }
            }


            ActionsPostTablesApply();
            ActionsFinalize();
            CompleteSound();

            return Ret;
        }

        /// <summary>
        /// get rows count from all tables
        /// </summary>
        private void SetSelectedRowsCountForAll()
        {
            if (!IsAll) return;

            SelectedRowsCount = 0;
            foreach (DataTable table in AllTables.Tables)
            {
                SelectedRowsCount += table.Rows.Count;
            }
        }

        /// <summary>
        /// get rows count from all tables
        /// </summary>
        private void SetSelectedRowsCountForTables(DataTable[] dataTables)
        {
            if (!IsTables) return;

            SelectedRowsCount = 0;
            foreach (DataTable table in dataTables) SelectedRowsCount += table.Rows.Count;
        }

        protected virtual void CompleteSound() => System.Media.SystemSounds.Asterisk.Play();

        /// <summary>
        /// count of rest rows
        /// </summary>
        protected int SelectedRowsCountRest;
        /// <summary>
        /// determine if SelectedRowsCountRest need to set
        /// </summary>
        bool _setRestRows = true;//
        ///// <summary>
        ///// true when last row processed
        ///// </summary>
        //protected bool IsLastRow;

        protected virtual void ApplyConditions(RowData rowData)
        {
            //set rest of rows with bool just because bool is faster
            if (_setRestRows)
            {
                _setRestRows = false;
                SelectedRowsCountRest = SelectedRowsCount;
            }

            //reduce rest of rows by 1
            //set IsLastRow to true because it is last processed row
            rowData.IsLastRow = --SelectedRowsCountRest == 0;

            if (!IsValidRow()) return;

            try 
            { 
                if (Apply()) Ret = true; 
            } 
            catch { }
        }

        /// <summary>
        /// check if row is valid for parse
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsValidRow() => AppSettings.IgnoreOrigEqualTransLines || !Equals(Original, Translation);

        protected abstract bool Apply();

        public string Original { get => SelectedRow.Field<string>(ColumnIndexOriginal); }

        public string Translation
        {
            get => SelectedRow.Field<string>(ColumnIndexTranslation);
            set => SelectedRow.SetValue(ColumnIndexTranslation, value);
        }
    }
}

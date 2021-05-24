using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    internal abstract class RowBase
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
            if (NeedInit)
            {
                SelectedRowsCount = 0;
                SelectedRowsCountRest = 0;
                NeedInit = false;
            }
        }

        /// <summary>
        /// Main app data
        /// </summary>
        protected THDataWork thDataWork;

        /// <summary>
        /// return value for Table/All functions. Depends on return of Apply
        /// </summary>
        protected bool ret;

        /// <summary>
        /// link to FileElements datagridview
        /// </summary>
        protected System.Windows.Forms.DataGridView DGV;

        /// <summary>
        /// an be used by some functions
        /// </summary>
        protected Dictionary<string, string> sessionData;

        protected RowBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }


        /// <summary>
        /// true when SelectedRow(row) was executed from Selected()
        /// </summary>
        bool IsInternalSelectedRowExecution = false;

        /// <summary>
        /// proceed 1 selected row
        /// </summary>
        /// <returns></returns>
        internal bool Selected(DataRow row, int TableIndex = -1, int RowIndex = -1)
        {
            try
            {
                if (!IsInternalSelectedRowExecution)
                {
                    Init();

                    GetTableData(TableIndex);

                    if (!IsAll && !IsTable)
                    {
                        SelectedRowsCount = 1;
                    }

                    if (DGV == null)
                    {
#if DEBUG
                            thDataWork.Main.THFileElementsDataGridView.Invoke((Action)(() => DGV = thDataWork.Main.THFileElementsDataGridView));
#else
                            DGV = thDataWork.Main.THFileElementsDataGridView;
#endif
                    }
                }

                SelectedRow = row;
                SelectedRowIndex = GetRowIndex(RowIndex);

                if (!IsInternalSelectedRowExecution && !IsAll && !IsTable)
                {
                    ActionsPreRowsApply();
                }

                ApplyConditions();
            }
            catch
            {
            }

            if (!IsInternalSelectedRowExecution && !IsAll && !IsTable)
            {
                ActionsPostRowsApply();
            }

            return ret;
        }

        /// <summary>
        /// get index of selected row
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <returns></returns>
        private int GetRowIndex(int RowIndex = -1)
        {
            if (RowIndex != -1)
            {
                SelectedRowIndex = RowIndex;
            }
            else
            {
                if (SelectedRowIndex == -1)
                {
                    SelectedRowIndex = SelectedTable.Rows.IndexOf(SelectedRow);
                }
            }

            return SelectedRowIndex;
        }

        readonly FunctionsLogs log = new FunctionsLogs();

        /// <summary>
        /// selected rows count
        /// </summary>
        protected int SelectedRowsCount = 0;
        /// <summary>
        /// proceed 1 or more of selected rows
        /// </summary>
        /// <returns></returns>
        internal bool Selected()
        {
            Init();

            if (DGV == null)
            {
#if DEBUG
                thDataWork.Main.THFileElementsDataGridView.Invoke((Action)(() => DGV = thDataWork.Main.THFileElementsDataGridView));
#else
                DGV = thDataWork.Main.THFileElementsDataGridView;
#endif
            }

            if (!IsAll && !IsTable)
            {
                SelectedRowsCount = DGV.GetCountOfRowsWithSelectedCellsCount();
            }

            if (SelectedRowsCount > 0)
            {
                try
                {
                    GetTableData();

                    if (!IsAll && !IsTable)
                    {
                        ActionsPreRowsApply();
                    }

                    IsInternalSelectedRowExecution = true;

                    var SelectedRowIndexses = new int[SelectedRowsCount];
                    for (int i = 0; i < SelectedRowsCount; i++)
                    {
                        log.DebugData.Add("SelectedTableIndex=" + SelectedTableIndex);
                        log.DebugData.Add("DGV.SelectedCells[i].RowIndex=" + DGV.SelectedCells[i].RowIndex);
                        log.DebugData.Add("i=" + i);

                        //координаты ячейки
                        SelectedRowIndexses[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, SelectedTableIndex, DGV.SelectedCells[i].RowIndex);
                    }

                    if (!IsAll && !IsTable)
                    {
                        Array.Sort(SelectedRowIndexses);//sort indexes
                    }

                    foreach (int RowIndex in SelectedRowIndexses)
                    {
                        Selected(SelectedTable.Rows[RowIndex], SelectedTableIndex, RowIndex);
                    }

                    if (!IsAll && !IsTable)
                    {
                        ActionsPostRowsApply();
                    }
                }
                catch (Exception ex)
                {
                    log.LogToFile("an error occured in base row function. error=\r\n" + ex);
                }
            }

            return ret;
        }

        /// <summary>
        /// apply the actions before all rows for selected,table or all was applied
        /// </summary>
        protected virtual void ActionsPreRowsApply()
        {
        }

        /// <summary>
        /// apply the actions before all tables wil be processed
        /// </summary>
        protected virtual void ActionsPreTablesApply()
        {
        }

        /// <summary>
        /// apply the actions before selected table wil be processed. 
        /// will be executed only for All or Table, not for Selected()
        /// </summary>
        protected virtual void ActionsPreTableApply()
        {
        }

        /// <summary>
        /// apply the actions after all rows for selected,table or all was applied
        /// </summary>
        protected virtual void ActionsPostRowsApply()
        {
        }

        /// <summary>
        /// apply the actions after all tables wil be processed
        /// </summary>
        protected virtual void ActionsPostTablesApply()
        {
        }

        /// <summary>
        /// apply the actions after selected table wil be processed
        /// </summary>
        protected virtual void ActionsPostTableApply()
        {
        }

        private void GetTableData(int TableIndex = -1)
        {
            if (SelectedTableIndex == -1)
            {
                if (TableIndex != -1)
                {
                    SelectedTableIndex = TableIndex;
                }
                else
                {
#if DEBUG
                    thDataWork.Main.Invoke((Action)(() => SelectedTableIndex = thDataWork.Main.THFilesList.SelectedIndex));
#else
                    SelectedTableIndex = thDataWork.Main.THFilesList.SelectedIndex;
#endif
                }
            }

            if (SelectedTable == null)
            {
                SelectedTable = thDataWork.THFilesElementsDataset.Tables[SelectedTableIndex];
            }

            ColumnIndexOriginal = SelectedTable.Columns[THSettingsData.OriginalColumnName()].Ordinal;// Колонка Original
            ColumnIndexTranslation = SelectedTable.Columns[THSettingsData.TranslationColumnName()].Ordinal;// Колонка Translation
        }

        /// <summary>
        /// proceed selected table. Multithreaded.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> TableT()
        {
            return await Task.Run(() => Table()).ConfigureAwait(false);
        }
        /// <summary>
        /// true when processed one table
        /// </summary>
        protected bool IsTable;
        protected int ColumnIndexOriginal;
        protected int ColumnIndexTranslation = 1;
        /// <summary>
        /// proceed selected table
        /// </summary>
        /// <returns></returns>
        internal bool Table()
        {
            Init();

            if (!IsAll)
            {
                IsTable = true;

                tablescount = 1;
            }

            GetTableData();

            if (SelectedTableIndex == -1)//return when table is not selected
            {
                return false;
            }

            if (IsValidTable(SelectedTable))
            {
                if (!IsAll)
                {
                    ActionsPreTablesApply();
                }

                ActionsPreTableApply();

                if (!IsAll)
                {
                    ActionsPreRowsApply();
                }

                IsInternalSelectedRowExecution = true;

                var RowsCount = SelectedTable.Rows.Count;
                if (!IsAll && IsTable /*|| (IsAll && SelectedTableIndex == tablescount - 1)set rows count to selectedrowscount for last table but forgot for which purpose it is*/)
                {
                    SelectedRowsCount = RowsCount;
                }

                for (int i = 0; i < RowsCount; i++)
                {
                    Selected(SelectedTable.Rows[i], SelectedTableIndex, i);
                }

                if (!IsAll && IsTable)
                {
                    ActionsPostRowsApply();
                }

                ActionsPostTableApply();

                if (!IsAll && IsTable)
                {
                    ActionsPostTablesApply();
                }
            }

            if (!IsAll)
            {
                CompleteSound();
            }

            return ret;
        }

        /// <summary>
        /// Check if table is valid. Can be used by any finction to not make same check in line checking for perfomance issues.
        /// </summary>
        /// <param name="table"></param>
        /// <returns>Default is always true</returns>
        protected virtual bool IsValidTable(DataTable table)
        {
            return true;
        }

        /// <summary>
        /// Proceed all tables. Multithreaded.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> AllT()
        {
            return await Task.Run(() => All()).ConfigureAwait(false);
        }

        protected DataTable SelectedTable;
        protected int SelectedTableIndex = -1;
        protected DataRow SelectedRow;
        protected int SelectedRowIndex;
        protected int tablescount = 0;
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
            IsAll = true;

            Init();

            int tindex = 0;
            var tables = thDataWork.THFilesElementsDataset.Tables;
            tablescount = tables.Count;
            SetSelectedRowsCountForAll();

            ActionsPreTablesApply();
            ActionsPreRowsApply();

            foreach (DataTable table in tables)
            {
                SelectedTable = table;
                SelectedTableIndex = tindex;
                try
                {
                    Table();
                }
                catch
                {
                }
                tindex++;
            }

            ActionsPostRowsApply();
            ActionsPostTablesApply();

            CompleteSound();

            return ret;
        }

        /// <summary>
        /// get rows count from all tables
        /// </summary>
        private void SetSelectedRowsCountForAll()
        {
            if (!IsAll)
            {
                return;
            }
            SelectedRowsCount = 0;
            foreach (DataTable t in thDataWork.THFilesElementsDataset.Tables)
            {
                SelectedRowsCount += t.Rows.Count;
            }
        }

        protected virtual void CompleteSound()
        {
            System.Media.SystemSounds.Asterisk.Play();
        }

        /// <summary>
        /// count of rest rows
        /// </summary>
        protected int SelectedRowsCountRest;
        /// <summary>
        /// determine if SelectedRowsCountRest need to set
        /// </summary>
        bool SetRestRows = true;//
        /// <summary>
        /// true when last row processed
        /// </summary>
        protected bool IsLastRow;

        protected virtual void ApplyConditions()
        {
            if (SetRestRows)//set rest of rows with bool just because bool is faster
            {
                SetRestRows = false;
                SelectedRowsCountRest = SelectedRowsCount;
            }

            SelectedRowsCountRest--;//reduce rest of rows by 1

            if (!IsValidRow())
            {
                return;
            }

            IsLastRow = SelectedRowsCountRest == 0;//set IsLastRow to true because it is last processed row

            try
            {
                if (Apply())
                {
                    ret = true;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// check if row is valid for parse
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsValidRow()
        {
            if (Properties.Settings.Default.IgnoreOrigEqualTransLines && Equals(SelectedRow[ColumnIndexOriginal], SelectedRow[ColumnIndexTranslation]))//apply only if translation not equal original
                return false;

            return true;
        }

        protected abstract bool Apply();
    }
}

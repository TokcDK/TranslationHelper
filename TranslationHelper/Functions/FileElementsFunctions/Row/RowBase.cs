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
        

        /// <summary>
        /// return value for Table/All functions. Depends on return of Apply
        /// </summary>
        protected bool Ret;

        /// <summary>
        /// link to FileElements datagridview
        /// </summary>
        protected System.Windows.Forms.DataGridView Dgv;

        /// <summary>
        /// an be used by some functions
        /// </summary>
        protected Dictionary<string, string> SessionData;

        protected RowBase()
        {
            
        }


        /// <summary>
        /// true when SelectedRow(row) was executed from Selected()
        /// </summary>
        bool _isInternalSelectedRowExecution = false;

        /// <summary>
        /// proceed 1 selected row
        /// </summary>
        /// <returns></returns>
        internal bool Selected(DataRow row, int tableIndex = -1, int rowIndex = -1)
        {
            try
            {
                if (!_isInternalSelectedRowExecution)
                {
                    Init();

                    GetTableData(tableIndex);

                    if (!IsAll && !IsTable)
                    {
                        SelectedRowsCount = 1;
                    }

                    if (Dgv == null)
                    {
#if DEBUG
                            ProjectData.Main.THFileElementsDataGridView.Invoke((Action)(() => Dgv = ProjectData.Main.THFileElementsDataGridView));
#else
                            DGV = ProjectData.Main.THFileElementsDataGridView;
#endif
                    }
                }

                SelectedRow = row;
                SelectedRowIndex = GetRowIndex(rowIndex);

                if (!_isInternalSelectedRowExecution && !IsAll && !IsTable)
                {
                    ActionsPreRowsApply();
                }

                ApplyConditions();
            }
            catch
            {
            }

            if (!_isInternalSelectedRowExecution && !IsAll && !IsTable)
            {
                ActionsPostRowsApply();
            }

            return Ret;
        }

        /// <summary>
        /// get index of selected row
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private int GetRowIndex(int rowIndex = -1)
        {
            if (rowIndex != -1)
            {
                SelectedRowIndex = rowIndex;
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

        protected readonly FunctionsLogs _log = new FunctionsLogs();

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

            if (Dgv == null)
            {
#if DEBUG
                ProjectData.Main.THFileElementsDataGridView.Invoke((Action)(() => Dgv = ProjectData.Main.THFileElementsDataGridView));
#else
                DGV = ProjectData.Main.THFileElementsDataGridView;
#endif
            }

            if (!IsAll && !IsTable)
            {
                SelectedRowsCount = Dgv.GetCountOfRowsWithSelectedCellsCount();
            }

            if (SelectedRowsCount > 0)
            {
                try
                {
                    GetTableData();

                    _isInternalSelectedRowExecution = true;

                    if (!IsAll && !IsTable)
                    {
                        ActionsPreRowsApply(); // need here also as in All because must be executed even if only selected rows was selected
                    }

                    var selectedRowIndexses = new int[SelectedRowsCount];
                    var addedRows = new HashSet<int>(SelectedRowsCount);
                    var dgvSelectedCells = Dgv.SelectedCells;
                    var dgvSelectedCellsCount = dgvSelectedCells.Count;
                    var ind = 0; // index for SelectedRowIndexses
                    for (int i = 0; i < dgvSelectedCellsCount; i++)
                    {
                        var dgvRowIndex = dgvSelectedCells[i].RowIndex;

                        if (addedRows.Contains(dgvRowIndex)) // skip if parent row index already was added
                        {
                            continue;
                        }

                        addedRows.Add(dgvRowIndex); // add row index as added

                        //log.DebugData.Add("SelectedTableIndex=" + SelectedTableIndex);
                        //log.DebugData.Add("DataGridView RowIndex=" + DGVRowIndex);
                        //log.DebugData.Add("i=" + i);

                        try
                        {
                            //add row index
                            selectedRowIndexses[ind] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(SelectedTableIndex, dgvRowIndex);
                        }
                        catch
                        {

                        }

                        ind++; //raise added index
                    }

                    if (!IsAll && !IsTable)
                    {
                        Array.Sort(selectedRowIndexses);//sort indexes
                    }

                    foreach (int rowIndex in selectedRowIndexses)
                    {
                        Selected(SelectedTable.Rows[rowIndex], SelectedTableIndex, rowIndex);
                    }

                    if (!IsAll && !IsTable)
                    {
                        ActionsPostRowsApply(); // need here also as in All because must be executed even if only selected rows was selected
                    }
                }
                catch (Exception ex)
                {
                    _log.LogToFile("an error occured in base row function. error=\r\n" + ex);
                }
            }

            return Ret;
        }

        /// <summary>
        /// apply the actions before all rows for selected,table or all was applied
        /// will be executed before Selected or before Table or before All
        /// </summary>
        protected virtual void ActionsPreRowsApply()
        {
        }

        /// <summary>
        /// apply the actions before all tables wil be processed
        /// will be executed before Table or before All
        /// </summary>
        protected virtual void ActionsPreTablesApply()
        {
        }

        /// <summary>
        /// apply the actions before selected table wil be processed. 
        /// will be executed each time before table parse
        /// </summary>
        protected virtual void ActionsPreTableApply()
        {
        }

        /// <summary>
        /// apply the actions after all rows for selected,table or all was applied
        /// will be executed after Selected or after Table or after All
        /// </summary>
        protected virtual void ActionsPostRowsApply()
        {
        }

        /// <summary>
        /// apply the actions after selected table wil be processed
        /// will be executed ech time after table parsed
        /// </summary>
        protected virtual void ActionsPostTableApply()
        {
        }

        /// <summary>
        /// apply the actions after all tables wil be processed
        /// will be executed after Table or after All
        /// </summary>
        protected virtual void ActionsPostTablesApply()
        {
        }

        private void GetTableData(int tableIndex = -1)
        {
            if (SelectedTableIndex == -1)
            {
                if (tableIndex != -1)
                {
                    SelectedTableIndex = tableIndex;
                }
                else
                {
#if DEBUG
                    ProjectData.Main.Invoke((Action)(() => SelectedTableIndex = ProjectData.Main.THFilesList.SelectedIndex));
#else
                    SelectedTableIndex = ProjectData.Main.THFilesList.SelectedIndex;
#endif
                }
            }

            if (SelectedTable == null)
            {
                SelectedTable = ProjectData.THFilesElementsDataset.Tables[SelectedTableIndex];
            }

            ColumnIndexOriginal = SelectedTable.Columns[THSettings.OriginalColumnName()].Ordinal;// Колонка Original
            ColumnIndexTranslation = SelectedTable.Columns[THSettings.TranslationColumnName()].Ordinal;// Колонка Translation
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

                Tablescount = 1;
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
                    ActionsPreTablesApply(); // need here also as in All because must be executed even if only one table was selected
                }

                ActionsPreTableApply();

                if (!IsAll)
                {
                    ActionsPreRowsApply(); // need here also as in All because must be executed even if only one table was selected
                }

                _isInternalSelectedRowExecution = true;

                var rowsCount = SelectedTable.Rows.Count;
                if (!IsAll && IsTable /*|| (IsAll && SelectedTableIndex == tablescount - 1)set rows count to selectedrowscount for last table but forgot for which purpose it is*/)
                {
                    SelectedRowsCount = rowsCount;
                }

                for (int i = 0; i < rowsCount; i++)
                {
                    Selected(SelectedTable.Rows[i], SelectedTableIndex, i);
                }

                if (!IsAll && IsTable)
                {
                    ActionsPostRowsApply(); // need here also as in All because must be executed even if only one table was selected
                }

                ActionsPostTableApply();

                if (!IsAll && IsTable) 
                {
                    ActionsPostTablesApply(); // need here also as in All because must be executed even if only one table was selected
                }
            }

            if (!IsAll)
            {
                CompleteSound();
            }

            return Ret;
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
        protected int Tablescount = 0;
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
            var tables = ProjectData.THFilesElementsDataset.Tables;
            Tablescount = tables.Count;
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

            return Ret;
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
            foreach (DataTable t in ProjectData.THFilesElementsDataset.Tables)
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
        bool _setRestRows = true;//
        /// <summary>
        /// true when last row processed
        /// </summary>
        protected bool IsLastRow;

        protected virtual void ApplyConditions()
        {
            if (_setRestRows)//set rest of rows with bool just because bool is faster
            {
                _setRestRows = false;
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
                    Ret = true;
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

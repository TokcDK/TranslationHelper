﻿using System;
using System.Collections.Generic;
using System.Data;
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
        /// true when function executed in another thread
        /// </summary>
        protected bool threaded;

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
            threaded = thDataWork.Main.InvokeRequired;
        }

        /// <summary>
        /// proceed 1 selected row
        /// </summary>
        /// <returns></returns>
        internal bool Selected(DataRow row)
        {
            Init();

            if (!IsAll && !IsTable)
            {
                ActionsPreRowsApply();
            }

            if (DGV == null)
            {
                if (threaded)
                {
                    thDataWork.Main.THFileElementsDataGridView.Invoke((Action)(() => DGV = thDataWork.Main.THFileElementsDataGridView));
                }
                else
                {
                    DGV = thDataWork.Main.THFileElementsDataGridView;
                }
            }

            try
            {
                SelectedRow = row;
                SelectedTableIndex = 0;
                SelectedRowIndex = 0;

                if (!IsAll && !IsTable)
                {
                    SelectedRowsCount = DGV.GetCountOfRowsWithSelectedCellsCount();
                }

                GetTableData();

                ApplyConditions();
            }
            catch
            {
            }

            if (!IsAll && !IsTable)
            {
                ActionsPostRowsApply();
            }

            return ret;
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
                if (threaded)
                {
                    thDataWork.Main.THFileElementsDataGridView.Invoke((Action)(() => DGV = thDataWork.Main.THFileElementsDataGridView));
                }
                else
                {
                    DGV = thDataWork.Main.THFileElementsDataGridView;
                }
            }


            //var selectedDGVCellsCount = DGV.GetCellCount(DataGridViewElementStates.Selected);

            if(!IsAll && !IsTable)
            {
                SelectedRowsCount = DGV.GetCountOfRowsWithSelectedCellsCount();
            }

            if (SelectedRowsCount > 0)
            {
                if (!IsAll && !IsTable)
                {
                    ActionsPreRowsApply();
                }
                try
                {
                    GetTableData();

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
                        SelectedRow = SelectedTable.Rows[RowIndex];
                        SelectedRowIndex = RowIndex;
                        ApplyConditions();
                    }
                }
                catch (Exception ex)
                {
                    log.LogToFile("an error occured in base row function. error=\r\n" + ex);
                }

                if (!IsAll && !IsTable)
                {
                    ActionsPostRowsApply();
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
        /// apply the actions after all rows for selected,table or all was applied
        /// </summary>
        protected virtual void ActionsPostRowsApply()
        {
        }

        private void GetTableData()
        {
            if (SelectedTableIndex == -1)
            {
                if (threaded)
                    thDataWork.Main.Invoke((Action)(() => SelectedTableIndex = thDataWork.Main.THFilesList.SelectedIndex));
                else
                    SelectedTableIndex = thDataWork.Main.THFilesList.SelectedIndex;
            }
            if (SelectedTable == null)
            {
                SelectedTable = thDataWork.THFilesElementsDataset.Tables[SelectedTableIndex];
            }
            ColumnIndexOriginal = SelectedTable.Columns[THSettingsData.OriginalColumnName()].Ordinal;// Колонка Original
            ColumnIndexTranslation = SelectedTable.Columns[THSettingsData.TranslationColumnName()].Ordinal;// Колонка Translation
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

            if (IsValidTable(SelectedTable))
            {
                if (!IsAll)
                {
                    ActionsPreRowsApply();
                }

                var RowsCount = SelectedTable.Rows.Count;
                for (int i = 0; i < RowsCount; i++)
                {
                    SelectedRow = SelectedTable.Rows[i];
                    SelectedRowIndex = i;
                    if (!IsAll && IsTable /*|| (IsAll && SelectedTableIndex == tablescount - 1)set rows count to selectedrowscount for last table but forgot for which purpose it is*/)
                    {
                        SelectedRowsCount = RowsCount;
                    }
                    ApplyConditions();
                }

                if (!IsAll && IsTable)
                {
                    ActionsPostRowsApply();
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

            ActionsPreRowsApply();

            int tindex = 0;
            var tables = thDataWork.THFilesElementsDataset.Tables;
            tablescount = tables.Count;
            SetSelectedRowsCountForAll();

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
            foreach(DataTable t in thDataWork.THFilesElementsDataset.Tables)
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

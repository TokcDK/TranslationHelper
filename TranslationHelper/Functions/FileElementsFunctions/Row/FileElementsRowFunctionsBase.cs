﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    internal abstract class FileElementsRowFunctionsBase
    {
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

        protected FileElementsRowFunctionsBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
            threaded = thDataWork.Main.InvokeRequired;
        }
        internal bool Selected()
        {
            if (DGV == null)
            {
                if (threaded)
                {
                    thDataWork.Main.Invoke((Action)(() => DGV = thDataWork.Main.THFileElementsDataGridView));
                }
                else
                {
                    DGV = thDataWork.Main.THFileElementsDataGridView;
                }
            }


            //var selectedDGVCellsCount = DGV.GetCellCount(DataGridViewElementStates.Selected);

            int DGVRowsCount = DGV.GetCountOfRowsWithSelectedCellsCount();
            if (DGVRowsCount > 0)
            {
                try
                {
                    GetTableData();

                    var SelectedRowIndexses = new int[DGVRowsCount];
                    for (int i = 0; i < DGVRowsCount; i++)
                    {
                        //координаты ячейки
                        SelectedRowIndexses[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, SelectedTableIndex, DGV.SelectedCells[i].RowIndex);

                    }
                    foreach (int RowIndex in SelectedRowIndexses)
                    {
                        SelectedRow = SelectedTable.Rows[RowIndex];
                        SelectedRowIndex = RowIndex;
                        ApplyConditions();
                    }
                }
                catch
                {
                }

                ActionsPostRowsApply();
            }

            return ret;
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
            ColumnIndexTranslation = SelectedTable.Columns[THSettingsData.TranslationColumnName()].Ordinal;// Колонка Original
        }

        protected int ColumnIndexOriginal;
        protected int ColumnIndexTranslation = 1;
        internal bool Table()
        {
            GetTableData();

            if (IsValidTable(SelectedTable))
            {
                if (!IsAll)
                {
                    IsTable = true;
                    PreLearn();
                }


                var RowsCount = SelectedTable.Rows.Count;
                for (int i = 0; i < RowsCount; i++)
                {
                    SelectedRow = SelectedTable.Rows[i];
                    SelectedRowIndex = i;
                    ApplyConditions();
                }

                if (IsTable)
                {
                    PostLearn();

                    ActionsPostRowsApply();
                }
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
        protected bool IsAll;
        protected bool IsTable;
        internal bool All()
        {
            IsAll = true;
            PreLearn();

            foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                SelectedTable = table;
                Table();
            }

            PostLearn();

            ActionsPostRowsApply();

            return ret;
        }

        protected virtual void PreLearn()
        {
        }
        protected virtual void PostLearn()
        {
        }

        protected virtual void ApplyConditions()
        {
            if (!Equals(SelectedRow[ColumnIndexOriginal], SelectedRow[ColumnIndexTranslation]))//apply only if translation not equal original
            {
                if (Apply())
                {
                    ret = true;
                }
            }
        }

        protected abstract bool Apply();
    }
}

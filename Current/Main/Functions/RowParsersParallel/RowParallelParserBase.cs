using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.RowParsersParallel
{
    public abstract class RowParallelParserBase
    {
        #region General


        protected readonly ListBox FilesList = AppData.THFilesList;
        protected readonly DataSet AllTables = AppData.CurrentProject.FilesContent;
        protected readonly DataGridView WorkTableDatagridView = AppData.Main.THFileElementsDataGridView;

        #endregion General

        #region Shared

        /// <summary>
        /// Parse all rows in all tables
        /// </summary>
        public void All()
        {
            ParseSelectedTables(AllTables.Tables.AsParallel().OfType<DataTable>());
        }
        /// <summary>
        /// Parse all rows in selected table[s]
        /// </summary>
        public void Tables()
        {
            var tables = AllTables.Tables;
            ParseSelectedTables(AppData.FilesListControl.GetSelectedIndexes().Select(i => tables[i]));
        }
        /// <summary>
        /// Parse selected rows
        /// </summary>
        public void Rows()
        {
            var selectedRowsIndexes = WorkTableDatagridView.GetSelectedRowsIndexes().OrderBy(i => i).ToArray();

            _rowsLeftToProcess = selectedRowsIndexes.Length;

            ParseSelectedRows(EnumerateRowsByRealRowIndexes(selectedRowsIndexes));
        }

        private IEnumerable<DataRow> EnumerateRowsByRealRowIndexes(int[] selectedRowsIndexes)
        {
            var table = WorkTableDatagridView.DataSource as DataTable;
            for (int i = 0; i < _rowsLeftToProcess; i++)
            {
                yield return table.GetRealRow(WorkTableDatagridView.Rows[selectedRowsIndexes[i]]);
            }
        }

        #endregion Shared

        #region Tables
        void ParseSelectedTables(IEnumerable<DataTable> tables)
        {
            _rowsLeftToProcess = tables.Select(t => t.Rows.Count).Sum();

            Parallel.ForEach(tables, table =>
            {
                if (!IsValidTable(table)) return;

                Parse(table);
            });
        }

        private bool IsValidTable(DataTable table)
        {
            return true;
        }

        void Parse(DataTable table)
        {
            ParseSelectedRows(table.Rows.AsParallel().OfType<DataRow>());
        }

        #endregion Tables

        #region Rows

        /// <summary>
        /// Determine if the row is last processing
        /// </summary>
        protected bool IsLastRow = false;
        int _rowsLeftToProcess = 0;

        void ParseSelectedRows(IEnumerable<DataRow> rows)
        {
            Parallel.ForEach(rows, row =>
            {
                IsLastRow = --_rowsLeftToProcess == 0;

                if (!IsValidRow(row)) return;

                Parse(row);
            });
        }

        protected virtual bool IsValidRow(DataRow row)
        {
            return true;
        }

        protected abstract bool Parse(DataRow row);

        #endregion Rows
    }
}

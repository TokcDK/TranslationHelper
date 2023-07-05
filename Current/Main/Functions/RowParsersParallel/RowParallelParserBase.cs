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
        public class RowData
        {
            public RowData(DataRow row)
            {
                Row = row;
            }

            int OriginalColumnIndex { get; } = AppData.CurrentProject.OriginalColumnIndex;
            int TranslationColumnIndex { get; } = AppData.CurrentProject.TranslationColumnIndex;

            public DataRow Row { get; }

            public string Original { get => Row.Field<string>(OriginalColumnIndex); }

            public string Translation
            {
                get => Row.Field<string>(TranslationColumnIndex);
                set => Row.SetValue(TranslationColumnIndex, value);
            }
        }

        #region General

        protected readonly ListBox FilesList = AppData.THFilesList;
        protected readonly DataSet AllTables = AppData.CurrentProject.FilesContent;
        protected readonly DataGridView WorkTableDatagridView = AppData.Main.THFileElementsDataGridView;

        #endregion General

        #region Shared

        /// <summary>
        /// Parse all rows in all tables
        /// </summary>
        public async Task All()
        {
            await Task.Run(() => ParseSelectedTables(AllTables.Tables.AsParallel().OfType<DataTable>())).ConfigureAwait(false);
        }
        /// <summary>
        /// Parse all rows in selected table[s]
        /// </summary>
        public async void Tables()
        {
            var tables = AllTables.Tables;
            await Task.Run(() => ParseSelectedTables(AppData.FilesListControl.GetSelectedIndexes().Select(i => tables[i]))).ConfigureAwait(false);
        }

        /// <summary>
        /// Parse selected rows
        /// </summary>
        public async Task Rows()
        {
            var selectedRowsIndexes = WorkTableDatagridView.GetSelectedRowsIndexes().OrderBy(i => i).ToArray();

            _rowsLeftToProcess = selectedRowsIndexes.Length;

            await Task.Run(() => ParseSelectedRows(WorkTableDatagridView.EnumerateSelectedRowsByRealRowIndexes(selectedRowsIndexes))).ConfigureAwait(false);
        }

        #endregion Shared

        #region Tables
        void ParseSelectedTables(IEnumerable<DataTable> tables)
        {
            _rowsLeftToProcess = tables.Select(t => t.Rows.Count).Sum();

            Parallel.ForEach(tables, table =>
            {
                Parse(table);
            });
        }

        protected virtual bool IsValidTable(DataTable table)
        {
            return true;
        }

        void Parse(DataTable table)
        {
            if (!IsValidTable(table)) return;

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
                var rowData = new RowData(row);
                Parse(rowData);
            });
        }

        bool Parse(RowData rowData)
        {
            IsLastRow = --_rowsLeftToProcess == 0;

            return IsValidRow(rowData) && Process(rowData);
        }

        #endregion Rows
    }
}


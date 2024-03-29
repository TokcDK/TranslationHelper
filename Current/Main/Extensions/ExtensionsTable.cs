﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Extensions
{
    static class ExtensionsTable
    {

        /// <summary>
        /// Check if any cell of translation column in the <paramref name="tableName"/> has value not equal to original
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool HasAnyTranslated(this string tableName)
        {
            return AppData.CurrentProject.FilesContent.Tables[tableName].HasAnyTranslated();
        }

        /// <summary>
        /// Check if any cell of translation column in the <paramref name="dataTable"/> has value not equal to original
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static bool HasAnyTranslated(this DataTable dataTable)
        {
            return !dataTable.IsTableColumnCellsAll(null, false);
        }

        /// <summary>
        /// Fills datatable with values from dictionary
        /// </summary>
        /// <param name="inputDictionary">Dictionary from which fill</param>
        /// <param name="inputDataTable">Datatable which need to fill</param>
        internal static bool FillWithDictionary(this DataTable inputDataTable, Dictionary<string, string> inputDictionary)
        {
            if (inputDataTable == null)
                return false;

            bool ret = false;
            foreach (var KeyValue in inputDictionary)
            {
                inputDataTable.Rows.Add(KeyValue.Key, KeyValue.Value);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Will return count of selected rows in datagridview
        /// </summary>
        /// <param name="DGV"></param>
        /// <returns></returns>
        internal static IEnumerable<int> GetSelectedRowsIndexes(this DataGridView DGV)
        {
            //https://stackoverflow.com/questions/47357051/c-datagridview-how-to-get-selected-count-with-cells-and-rows
            return DGV.SelectedCells.Cast<DataGridViewCell>()
                                       .Select(c => c.RowIndex).Distinct();
        }

        /// <summary>
        /// Will return count of selected rows in datagridview
        /// </summary>
        /// <param name="DGV"></param>
        /// <returns></returns>
        internal static int GetSelectedRowsCount(this DataGridView DGV)
        {
            int cnt;
            if (DGV.SelectedRows.Count == (cnt = DGV.Rows.Count))
            {
                return cnt;
            }

            return DGV.GetSelectedRowsIndexes().Count();
        }

        /// <summary>
        /// Enumerate selected DataRows by selected indexes in <paramref name="dataGridView"/>
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="selectedRowsIndexes"></param>
        /// <returns></returns>
        public static IEnumerable<DataRow> EnumerateSelectedRowsByRealRowIndexes(this DataGridView dataGridView, int[] selectedRowsIndexes = null)
        {
            if (!(dataGridView.DataSource is DataTable table)) yield break;

            if (selectedRowsIndexes == null)
            {
                selectedRowsIndexes = dataGridView.GetSelectedRowsIndexes().OrderBy(i => i).ToArray();
            }

            var indexesCount = selectedRowsIndexes.Length;

            for (int i = 0; i < indexesCount; i++)
            {
                yield return table.GetRealRow(dataGridView.Rows[selectedRowsIndexes[i]]);
            }
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev selected cell index. For case when row filter or sort is activated
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetRealRowIndex(this DataTable table, DataGridView dataGridView, int rowIndex)
        {
            return table.GetRealRowIndex(dataGridView.Rows[rowIndex]);
        }

        /// <summary>
        /// Return DataRow with real index in Datatable for Datagridviev selected cell index. For case when row filter or sort is activated
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static DataRow GetRealRow(this DataTable table, DataGridViewRow dataGridViewRow)
        {
            return table.Rows[table.GetRealRowIndex(dataGridViewRow)];
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev selected cell index. For case when row filter or sort is activated
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetRealRowIndex(this DataTable table, DataGridViewRow dataGridViewRow)
        {
            var tableDataView = table.DefaultView;
            if (string.IsNullOrEmpty(tableDataView.Sort) && string.IsNullOrEmpty(tableDataView.RowFilter))
            {
                return dataGridViewRow.Index;
            }

            var dataBoundItem = (DataRowView)dataGridViewRow.DataBoundItem;
            return table.Rows.IndexOf(dataBoundItem.Row);
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev selected cell index. For case when row filter or sort is activated
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetRealRowIndex(this DataTable table, int rowIndex)
        {
            return table.GetRealRowIndex(AppData.Main.THFileElementsDataGridView, rowIndex);
        }
    }
}

﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TranslationHelper.Extensions
{
    static class ExtensionsTable
    {
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
        internal static int GetCountOfRowsWithSelectedCellsCount(this DataGridView DGV)
        {
            int cnt;
            if (DGV.SelectedRows.Count == (cnt = DGV.Rows.Count))
            {
                return cnt;
            }

            //https://stackoverflow.com/questions/47357051/c-datagridview-how-to-get-selected-count-with-cells-and-rows
            return DGV.SelectedCells.Cast<DataGridViewCell>()
                                       .Select(c => c.RowIndex).Distinct().Count();
        }
    }
}
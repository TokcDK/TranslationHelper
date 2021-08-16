using System.Collections.Generic;
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
            foreach (var keyValue in inputDictionary)
            {
                inputDataTable.Rows.Add(keyValue.Key, keyValue.Value);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Will return count of selected rows in datagridview
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        internal static int GetCountOfRowsWithSelectedCellsCount(this DataGridView dgv)
        {
            int cnt;
            if (dgv.SelectedRows.Count == (cnt = dgv.Rows.Count))
            {
                return cnt;
            }

            //https://stackoverflow.com/questions/47357051/c-datagridview-how-to-get-selected-count-with-cells-and-rows
            return dgv.SelectedCells.Cast<DataGridViewCell>()
                                       .Select(c => c.RowIndex).Distinct().Count();
        }
    }
}

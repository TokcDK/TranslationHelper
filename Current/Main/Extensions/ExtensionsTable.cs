using System.Collections.Generic;
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
        internal static int GetSelectedRowsCount(this DataGridView DGV)
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

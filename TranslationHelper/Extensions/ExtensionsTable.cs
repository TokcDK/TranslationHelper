using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Extensions
{
    static class ExtensionsTable
    {
        /// <summary>
        /// Will return count of selected rows in datagridview
        /// </summary>
        /// <param name="DGV"></param>
        /// <returns></returns>
        internal static int GetCountOfRowsWithSelectedCellsCount(this DataGridView DGV)
        {
            //https://stackoverflow.com/questions/47357051/c-datagridview-how-to-get-selected-count-with-cells-and-rows
            return DGV.SelectedCells.Cast<DataGridViewCell>()
                                       .Select(c => c.RowIndex).Distinct().Count();
        }
    }
}

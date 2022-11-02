//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace TranslationHelper.ExternalAdditions
//{
//    public static class DataGridViewEx
//    {
//        public static void ClearColumn(this DataGridView dgv, int columnIndex)
//        {
//            if (dgv != null && dgv.Columns != null && dgv.Columns.Count - 1 >= columnIndex)
//            {
//                //https://stackoverflow.com/questions/15035219/how-do-you-clear-an-entire-datagridview-column
//                //string DGVCellName = dgv.Columns[columnIndex].Name;
//                //string DGVCellHeaderText = dgv.Columns[columnIndex].HeaderText;
//                ////DataGridViewColumn tmpCol = dgv.Columns[columnIndex];
//                //dgv.Invoke((Action)(() => dgv.Columns.Remove(dgv.Columns[columnIndex])));
//                ////dgv.Invoke((Action)(() => dgv.Columns.Insert(columnIndex, tmpCol)));
//                //dgv.Invoke((Action)(() => dgv.Columns.Add(DGVCellName, DGVCellHeaderText)));
//                //dgv.Invoke((Action)(() => dgv.Columns[DGVCellName]));
//            }
//        }
//    }
//}

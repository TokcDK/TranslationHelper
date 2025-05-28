//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TranslationHelper.Data;

//namespace TranslationHelper.Functions.FileElementsFunctions.Row.RowFunctionSelectedRowNewWIP
//{
//    public class SelectedTable
//    {
//        public readonly DataTable Table;

//        public SelectedTable(DataTable table)
//        {
//            Table = table;
//        }
//    }
//    public class SelectedRow
//    {
//        public SelectedRow(DataRow row)
//        {
//            Row = row;
//        }

//        public readonly DataRow Row;

//        string original;
//        bool IsOriginalNeedInit = true;
//        public string Original
//        {
//            get
//            {
//                if (IsOriginalNeedInit)
//                {
//                    IsOriginalNeedInit = false;
//                    return original = Row[Project.OriginalColumnIndex] as string;
//                }
//                else return original;
//            }
//        }

//        public string Translation
//        {
//            get
//            {
//                return Row.Field<string>(Project.TranslationColumnIndex);
//            }
//            set
//            {
//#if DEBUG
//                AppData.Main.Invoke((Action)(() => Row.SetField(Project.TranslationColumnIndex, value)));
//#else
//                Row.SetField(Project.TranslationColumnIndex, value);
//#endif
//            }
//        }
//    }

//    public abstract class RowFunctionNewWIPBase : RowFunctionSelectedFilesNewWIPBase
//    {

//    }

//    public abstract class RowFunctionSelectedFilesNewWIPBase : RowFunctionSelectedRowsNewWIPBase
//    {

//    }

//    public abstract class RowFunctionSelectedRowsNewWIPBase : RowFunctionSelectedRowNewWIPBase
//    {
//        public bool Rows()
//        {
//            DataTable[] tables = GetSelectedTables();

//            return false;
//        }

//        private DataTable[] GetSelectedTables()
//        {
//            int[] tableindexes = null;
//#if DEBUG
//            AppData.THFilesList.Invoke((Action)(() => tableindexes = AppData.THFilesList.CopySelectedIndexes()));
//#else
//            tableindexes = AppData.THFilesList.CopySelectedIndexes();
//#endif
//            DataTable[] tables = null;
//#if DEBUG
//            AppData.Main.Invoke((Action)(() => tables = Project.FilesContent.GetTablesByIndexes(tableindexes)));
//            return tables;
//#else
//            return Project.FilesContent.GetTablesByIndexes(tableindexes);
//#endif
//        }
//    }

//    public abstract class RowFunctionSelectedRowNewWIPBase
//    {
//        /// <summary>
//        /// check if row is valid for parse
//        /// </summary>
//        /// <returns></returns>
//        protected virtual bool IsValidRow(SelectedRow selectedRow) => AppSettings.IgnoreOrigEqualTransLines || !string.Equals(selectedRow.Original, selectedRow.Translation);
//        /// <summary>
//        /// change translation here using selected row data
//        /// </summary>
//        /// <param name="selectedRow"></param>
//        /// <returns></returns>
//        protected abstract bool Apply(SelectedRow selectedRow);
//    }
//}

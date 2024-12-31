using System;
using System.Data;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class ClearCells : RowBase
    {
        public ClearCells()
        {
        }

        protected override bool IsParallelTables => true;
        protected override bool IsParallelRows => true;

        bool _dataSourceClear = false;
        protected override void ActionsPreTableApply(TableData tableData)
        {
            if (IsAll || IsTables || IsTable)
            {
                if (AppData.Main.THFileElementsDataGridView.DataSource != tableData.SelectedTable) return;

                _dataSourceClear = true;
                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
            }
        }

        protected override void ActionsPostTableApply(TableData tableData)
        {
            if ((IsAll || IsTables || IsTable) && _dataSourceClear)
            {
                _dataSourceClear = false;
                AppData.Main.Invoke((Action)(() => FunctionsUI.ActionsOnTHFIlesListElementSelected()));
            }
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return true; //clear any rows
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            rowData.Translation = null;

            return true;
        }
    }
}

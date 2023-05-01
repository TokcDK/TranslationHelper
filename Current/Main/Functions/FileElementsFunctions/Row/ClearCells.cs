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

        bool _dataSourceClear = false;
        protected override void ActionsPreTableApply()
        {
            if (IsAll || IsTables || IsTable)
            {
                if (AppData.Main.THFileElementsDataGridView.DataSource != SelectedTable) return;

                _dataSourceClear = true;
                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
            }
        }

        protected override void ActionsPostTableApply()
        {
            if ((IsAll || IsTables || IsTable) && _dataSourceClear)
            {
                _dataSourceClear = false;
                AppData.Main.Invoke((Action)(() => AppData.Main.ActionsOnTHFIlesListElementSelected()));
            }
        }

        protected override bool IsValidRow()
        {
            return true; //clear any rows
        }

        protected override bool Apply()
        {
            Translation = null;

            return true;
        }
    }
}

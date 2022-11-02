using System;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class ClearCells : RowBase
    {
        public ClearCells()
        {
        }

        protected override void ActionsPreTableApply()
        {
            if (IsAll || IsTables || IsTable)
            {
                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
            }
        }

        protected override void ActionsPostTableApply()
        {
            if (IsAll || IsTables || IsTable)
            {
                AppData.Main.Invoke((Action)(() => AppData.Main.ActionsOnTHFIlesListElementSelected()));
            }
        }

        protected override bool IsValidRow()
        {
            return true; //clear any rows
        }

        protected override bool Apply()
        {
            SelectedRow[1] = null;

            return true;
        }
    }
}

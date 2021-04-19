using System;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class ClearCells : RowBase
    {
        public ClearCells(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override void ActionsPreRowsApply()
        {
            if (IsAll || IsTable)
            {
                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.DataSource = null));
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Update()));
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Refresh()));
            }
        }

        protected override void ActionsPostRowsApply()
        {
            if (IsAll || IsTable)
            {
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.ActionsOnTHFIlesListElementSelected()));
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

using System;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class ClearCells : RowBase
    {
        public ClearCells() : base()
        {
        }

        protected override void ActionsPreRowsApply()
        {
            if (IsAll || IsTable)
            {
                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
            }
        }

        protected override void ActionsPostRowsApply()
        {
            if (IsAll || IsTable)
            {
                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.ActionsOnTHFIlesListElementSelected()));
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

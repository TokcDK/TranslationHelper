using System;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class ClearCells : RowBase
    {
        public ClearCells(ProjectData projectData) : base(projectData)
        {
        }

        protected override void ActionsPreRowsApply()
        {
            if (IsAll || IsTable)
            {
                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                projectData.Main.Invoke((Action)(() => projectData.Main.THFileElementsDataGridView.DataSource = null));
                projectData.Main.Invoke((Action)(() => projectData.Main.THFileElementsDataGridView.Update()));
                projectData.Main.Invoke((Action)(() => projectData.Main.THFileElementsDataGridView.Refresh()));
            }
        }

        protected override void ActionsPostRowsApply()
        {
            if (IsAll || IsTable)
            {
                projectData.Main.Invoke((Action)(() => projectData.Main.ActionsOnTHFIlesListElementSelected()));
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

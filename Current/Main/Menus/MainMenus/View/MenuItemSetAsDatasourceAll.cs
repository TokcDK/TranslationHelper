using System;
using TranslationHelper.Data;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemSetAsDatasourceAll : MainMenuViewSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("SetAsDatasourceAll");

        public override string Description => Text;

        public override void OnClick(object sender, EventArgs e)
        {
            //The grid and the content of the project on screen, not of "the" project and "the" grid.
            var workspace = AppData.ActiveWorkspace;
            var grid = workspace?.ActiveFileWorkspace?.ElementsDataGridView;
            if (grid == null) return;

            grid.DataSource = workspace.Project.FilesContentAll;

            //смотрел тут но в данном случае пришел к тому что отображает все также только одну таблицу
            //https://social.msdn.microsoft.com/Forums/en-US/f63f612f-20be-4bad-a91c-474396941800/display-dataset-data-in-gridview-from-multiple-data-tables?forum=adodotnetdataset
            //if (THFilesElementsDataset.Relations.Contains("ALL"))
            //{

            //}
            //else
            //{
            //    DataRelation dr = new DataRelation("ALL",
            //         new DataColumn[] { THFilesElementsDataset.Tables[0].Columns[THSettings.OriginalColumnName], THFilesElementsDataset.Tables[0].Columns[THSettings.TranslationColumnName] },
            //         new DataColumn[] { THFilesElementsDataset.Tables[1].Columns[THSettings.OriginalColumnName], THFilesElementsDataset.Tables[1].Columns[THSettings.TranslationColumnName] },
            //         false
            //                                        );

            //    THFilesElementsDataset.Relations.Add(dr);
            //}

            //THFileElementsDataGridView.DataSource = THFilesElementsDataset.Relations["ALL"].ParentTable;
        }
    }
}

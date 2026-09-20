using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG;
using TranslationHelper.Menus;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Projects.WolfRPG.Menus
{
    /// <summary>
    /// Menu item that lets a translator attach extra context lines to the selected rows.
    /// The context list file itself is handled by <see cref="StandaloneContextList"/>, which the
    /// WolfRPG patch formats read as well.
    /// </summary>
    class AddToStandaloneContextList : FileRowMenuItemBase, IProjectSpecifiedMenuItem
    {
        public override string Text => "Add standalone context";

        public override string Description => "Adds entered context as standalone. Each string using the context path will be moved in separated text block in patch in time of open/save";

        public override void OnClick(object sender, EventArgs e)
        {
            var standaloneContextList = StandaloneContextList.LoadList(StandaloneContextList.StandaloneContextFilePath);

            using (var form = new AddToStandaloneContextListForm())
            {
                var result = form.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                if (AppData.THFilesList.SelectedIndex == -1)
                {
                    return;
                }

                var addedContextLine = form.ContextLine;

                var selectedCells = AppData.Main.THFileElementsDataGridView.SelectedCells;
                foreach (DataGridViewCell selectedCell in selectedCells)
                {
                    var cellValue = AppData.Main.THFileElementsDataGridView.Rows[selectedCell.RowIndex].Cells[0].Value + "";
                    if (string.IsNullOrWhiteSpace(cellValue))
                    {
                        continue;
                    }

                    StandaloneContextList.CleanContext(ref addedContextLine);

                    StandaloneContextList.Add(standaloneContextList, cellValue, addedContextLine);
                }

                if (standaloneContextList.Count > 0)
                {
                    StandaloneContextList.SaveList(StandaloneContextList.StandaloneContextFilePath, standaloneContextList);
                }
            }
        }
    }
}

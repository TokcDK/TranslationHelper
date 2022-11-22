using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemRowTextCopy : MainMenuEditSubItemBase, IFileRowMenuItem
    {
        public override string Text => T._("Copy");

        public override string Description => T._("Copy selected rows translation");

        public override void OnClick(object sender, EventArgs e)
        {
            if (!IsValidToCopy()) return;

            FunctionsCopyPaste.CopyToClipboard(AppData.Main.THFileElementsDataGridView);
        }

        protected bool IsValidToCopy()
        {
            if (AppSettings.DGVCellInEditMode) AppData.Main.ControlsSwitch(); // если ячейка в режиме редактирования вылючение действий для ячеек при выходе из режима редактирования

            return AppData.Main.THFileElementsDataGridView != null && AppData.Main.THFileElementsDataGridView.SelectedCells.Count > 0;
        }
        public override Keys ShortcutKeys => Keys.Control | Keys.C;
    }
}

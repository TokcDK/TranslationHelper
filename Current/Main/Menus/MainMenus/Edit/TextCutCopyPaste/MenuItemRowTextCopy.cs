using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace TranslationHelper.Menus.MainMenus.Edit.TextCutCopyPaste
{
    internal class MenuItemRowTextCopy : MenuItemRowTextCopyCutBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Copy");

        public override string Description => T._("Copy selected rows translation");

        protected override void ActionForSelectedRows()
        {
            if (!IsValidToCopy()) return;

            FunctionsCopyPaste.CopyToClipboard(AppData.Main.THFileElementsDataGridView);
        }

        protected override bool ActionForTextBoxObject(TextBoxBase tb)
        {
            if (string.IsNullOrEmpty(tb.SelectedText)) return false;

            Clipboard.SetDataObject(tb.SelectedText); // copy selected text to clipboard

            tb.DeselectAll(); // deselect text to show it was copied

            return true;
        }

        protected static bool IsValidToCopy()
        {
            if (AppSettings.DGVCellInEditMode) FunctionsUI.ControlsSwitch(); // если ячейка в режиме редактирования вылючение действий для ячеек при выходе из режима редактирования

            return AppData.Main.THFileElementsDataGridView != null && AppData.Main.THFileElementsDataGridView.SelectedCells.Count > 0;
        }
        public override Keys ShortcutKeys => Keys.Control | Keys.C;

        public override int Order => base.Order - 100;
    }
}

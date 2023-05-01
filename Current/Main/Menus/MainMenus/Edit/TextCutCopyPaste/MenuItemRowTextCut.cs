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

namespace TranslationHelper.Menus.MainMenus.Edit.TextCutCopyPaste
{
    internal class MenuItemRowTextCut : MenuItemRowTextCopyCutBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Cut");

        public override string Description => T._("Cut selected rows translation");

        protected override void ActionForSelectedRows()
        {
            if (AppSettings.DGVCellInEditMode) AppData.Main.ControlsSwitch(); // если ячейка в режиме редактирования вылючение действий для ячеек при выходе из режима редактирования

            if (AppData.Main.THFileElementsDataGridView == null) return;

            // Ensure that text is currently selected in the text box.    
            if (AppData.Main.THFileElementsDataGridView.SelectedCells.Count == 0) return;

            //Copy to clipboard
            FunctionsCopyPaste.CopyToClipboard(AppData.Main.THFileElementsDataGridView);

            //Clear selected cells                
            //проверка, выполнять очистку только если выбранные ячейки не помечены Только лдя чтения
            if (AppData.Main.THFileElementsDataGridView.CurrentCell.ReadOnly) return;

            foreach (DataGridViewCell dgvCell in AppData.Main.THFileElementsDataGridView.SelectedCells)
            {
                if (!dgvCell.ReadOnly) dgvCell.Value = null;
            }
        }

        protected override bool ActionForTextBoxObject(TextBoxBase tb)
        {
            if (string.IsNullOrEmpty(tb.SelectedText)) return false;
                       
            // cut only when textbox is not readonly, else just copy
            if (tb.ReadOnly)
            {
                Clipboard.SetDataObject(tb.SelectedText);
                tb.DeselectAll();
            }
            else
            {
                tb.Cut();
            }

            return true;
        }

        public override Keys ShortcutKeys => Keys.Control | Keys.X;
        public override int Order => base.Order + 22;
    }
}

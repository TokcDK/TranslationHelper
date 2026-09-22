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

            //IsValidToCopy has established that the project on screen has a grid with a selection.
            FunctionsCopyPaste.CopyToClipboard(AppData.ActiveWorkspace.ActiveFileWorkspace.ElementsDataGridView);
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

            //The grid of the project on screen, not a single application-wide one.
            var grid = AppData.ActiveWorkspace?.ActiveFileWorkspace?.ElementsDataGridView;
            return grid != null && grid.SelectedCells.Count > 0;
        }
        public override Keys ShortcutKeys => Keys.Control | Keys.C;

        public override int Order => base.Order - 100;
    }
}

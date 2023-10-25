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
    internal class MenuItemRowTextPaste : MenuItemRowTextCopyCutBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Paste");

        public override string Description => T._("Paste translation into selected rows");

        protected override void ActionForSelectedRows()
        {
            if (!Clipboard.ContainsText()) return;

            new PasteTranslation().Rows();
        }

        protected override bool ActionForTextBoxObject(TextBoxBase tb)
        {
            if (!Clipboard.ContainsText()) return false;            

            return CheckAndPasteSelectedTextToClipboardDeselect(tb);
        }

        private bool CheckAndPasteSelectedTextToClipboardDeselect(TextBoxBase tb)
        {
            if (tb.ReadOnly) return false;

            tb.SelectedText = Clipboard.GetText();

            return true;
        }

        public override Keys ShortcutKeys => Keys.Control | Keys.V;
        public override int Order => base.Order - 98;
    }
}

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
    internal class MenuItemRowTextCopyOriginal : MenuItemRowTextCopy, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Copy original");

        public override string Description => T._("Copy selected rows original values");

        public override void OnClick(object sender, EventArgs e)
        {
            if (!IsValidToCopy()) return;

            new CopyOriginals().Rows();
        }

        public override Keys ShortcutKeys => Keys.Control | Keys.Shift | Keys.C;

        public override int Order => base.Order - 99;
    }
}

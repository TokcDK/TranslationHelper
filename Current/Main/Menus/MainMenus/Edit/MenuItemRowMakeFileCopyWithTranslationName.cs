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
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemRowMakeFileCopyWithTranslationName : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Make translated filecopy for exist originals");

        public override string Description => T._("Search file in game dir with name ofrow original value and create copy of it with name as translated");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new MakeTranslatedCopyIfFileWithTheNameExists().AllT();
        }
    }
}

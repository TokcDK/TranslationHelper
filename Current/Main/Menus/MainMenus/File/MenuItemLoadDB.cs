using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemLoadDB : MainMenuFileSubItemBase, IProjectMenuItem
    {

        public override string Text => T._("Load DB");

        public override string Description => T._("Load translated strings from database file if exist");

        public override void OnClick(object sender, EventArgs e)
        {
            AppData.Main.UnLockDBLoad(false);
            AppData.Main.LoadDB();
            //AppData.Main.Invoke((Action)(() => AppData.Main.LoadTranslationToolStripMenuItem.Enabled = true));
        }
        public override int Order => base.Order + 10;
    }
}

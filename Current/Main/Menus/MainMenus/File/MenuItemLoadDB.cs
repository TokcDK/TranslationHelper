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
    internal class MenuItemLoadDB : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => T._("Load DB");

        public string Description => T._("Load translated strings from database file if exist");

        public string CategoryName => "";

        public void OnClick(object sender, EventArgs e)
        {
            AppData.Main.UnLockDBLoad(false);
            AppData.Main.LoadDB();
            //AppData.Main.Invoke((Action)(() => AppData.Main.LoadTranslationToolStripMenuItem.Enabled = true));
        }
    }
}

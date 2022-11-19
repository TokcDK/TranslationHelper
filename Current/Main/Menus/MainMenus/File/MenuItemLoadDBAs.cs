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
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemLoadDBAs : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => T._("Load DB as");

        public string Description => T._("Load translated strings from database file of selected locatin");

        public string CategoryName => "";

        public async void OnClick(object sender, EventArgs e)
        {
            AppData.Main.LoadDBAs();
        }
    }
}

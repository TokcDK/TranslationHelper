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
    internal class MenuItemSaveDBTo : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => T._("Save DB to");

        public string Description => T._("Save translated strings into database file into selected locatin");

        public string CategoryName => "";

        public async void OnClick(object sender, EventArgs e)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File.Export
{
    internal class MenuItemExportToRPGMakerTransPatch : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => T._("Export translation to RPG Maker Trans patch");

        public string Description => "";

        public string CategoryName => "Export";

        public void OnClick(object sender, EventArgs e)
        {
            _ = new RpgMakerLikeTxt().All();
        }
    }
}

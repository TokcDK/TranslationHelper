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
    internal class MenuItemExportToRPGMakerTransPatch : MainMenuFileSubItemExportBase, IProjectMenuItem
    {
        public override string Text => T._("Export translation to RPG Maker Trans patch");

        public override string Description => "";

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new RpgMakerLikeTxt().All();
        }
    }
}

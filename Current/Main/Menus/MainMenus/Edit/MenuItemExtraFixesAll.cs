using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemExtraFixes : MainMenuEditSubItemBase, IFileRowMenuItem
    {
        public override string Text => T._("Extra Fixes (All)");

        public override string Description => T._("Apply extra fixes to rows");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new AllHardFixes().AllT();
        }
    }
}

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
using TranslationHelper.Menus.MainMenus.File.Other;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemXORDecript : MainMenuFileSubItemOtherBase
    {
        public override string Text => "test xor decrypt";

        public override void OnClick(object sender, EventArgs e)
        {
            tests.Xorfornscript.DecryptXor();
        }
    }
}

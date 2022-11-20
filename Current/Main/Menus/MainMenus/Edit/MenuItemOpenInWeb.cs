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
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemOpenInWeb : MainMenuEditSubItemBase
    {
        public override string Text => T._("Open in Web");

        public override string Description => T._("Open selected rows in web service");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new OpenInWeb().Selected();
        }
    }
}

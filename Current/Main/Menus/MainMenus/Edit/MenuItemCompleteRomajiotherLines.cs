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
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemCompleteRomajiotherLines : MainMenuEditSubItemBase, IFileRowMenuItem
    {
        public override string Text => T._("Complete Romajio/Other lines");

        public override string Description => T._("For japanese language check if row have most of romajii or other chars and set it identical to original");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new CompleteRomajiotherLines().AllT();
            _ = new SetOrigToTransIfSoundsText().AllT();
        }
    }
}

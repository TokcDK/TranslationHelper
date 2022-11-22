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
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemRowFixNameInstance : MainMenuEditSubItemBase, IFileRowMenuItem
    {
        public override string Text => T._("File by translation");

        public override string Description => T._("Create copy of file in game dir with name like in translation if file with original name is exist");

        public override async void OnClick(object sender, EventArgs e)
        {
            await new FixInstancesOfName().AllT();
        }
    }
}

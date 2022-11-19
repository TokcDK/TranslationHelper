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
    internal class MenuItemXOREncript : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => "test xor encrypt";

        public string Description => "";

        public string CategoryName => "";

        public void OnClick(object sender, EventArgs e)
        {
            tests.Xorfornscript.EncryptXor();
        }
    }
}

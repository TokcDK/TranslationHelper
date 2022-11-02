using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Functions;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemOpen : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => T._("Open");

        public string Description => T._("Select game exe or file to try to open");

        public string CategoryName => "";

        public void OnClick(object sender, EventArgs e) { FunctionsOpen.OpenProject(); }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Functions;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemOpen : MainMenuFileSubItemBase
    {
        public override string Text => T._("Open");

        public override string Description => T._("Select game exe or file to try to open");

        public override void OnClick(object sender, EventArgs e) { FunctionsOpen.OpenProject(); }
        public override int Order => base.Order;
    }
}

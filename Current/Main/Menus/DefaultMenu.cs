using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Menus.MainMenus;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus
{
    internal class DefaultMenu : MenuItemBase, IDefaultMenuItem
    {
        public override string Text => "";

        public override void OnClick(object sender, EventArgs e)
        {
        }
    }
}

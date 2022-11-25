using System;
using System.Windows.Forms;
using TranslationHelper.Menus;

namespace TranslationHelper.Menus.MainMenus
{
    public abstract class MainMenuItemBase : MenuItemBase, IMainMenuItem
    {
        public override string ParentMenuName => "";

        public override int Order => base.Order + 10;
    }
}

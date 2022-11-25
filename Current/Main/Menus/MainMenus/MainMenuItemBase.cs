using System;
using System.Windows.Forms;
using TranslationHelper.Menus;

namespace TranslationHelper.Menus.MainMenus
{
    /// <summary>
    /// <see cref="MenuItemBase"/> based abstract class determines empty parent menu name
    /// </summary>
    public abstract class MainMenuItemBase : MenuItemBase, IMainMenuItem
    {
        public override string ParentMenuName => "";

        public override int Order => base.Order + 10;
    }
}

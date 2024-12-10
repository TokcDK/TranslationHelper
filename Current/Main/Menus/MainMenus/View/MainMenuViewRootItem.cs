using System;
using System.Windows;

namespace TranslationHelper.Menus.MainMenus.View
{
    public class MainMenuViewRootItem : MainMenuItemBase
    {
        public override string ParentMenuName => "";
        public override int Order => base.Order + 10000;

        public override string Text => T._("View");

        public override void OnClick(object sender, EventArgs e) { }
    }
}

using System;
using System.Windows;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public class MainMenuEditRootItem : MainMenuItemBase, IProjectMenuItem
    {
        public override string ParentMenuName => "";
        public override int Order => base.Order + 20000;

        public override string Text => T._("Edit");

        public override void OnClick(object sender, EventArgs e) { }
    }
}

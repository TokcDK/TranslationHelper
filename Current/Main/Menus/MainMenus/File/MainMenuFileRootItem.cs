using System;
using System.Windows;

namespace TranslationHelper.Menus.MainMenus.File
{
    public class MainMenuFileRootItem : MainMenuItemBase
    {
        public override string ParentMenuName => "";
        public override int Order => base.Order + 30000;

        public override string Text => T._("File");

        public override void OnClick(object sender, EventArgs e) { }
    }
}

using System;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public class MainMenuHelpRootItem : MainMenuItemBase
    {
        public override string CategoryName => "";
        public override string Text => T._("Help");
        public override int Order => base.Order - 10000;

        public override void OnClick(object sender, EventArgs e) {}
    }
}

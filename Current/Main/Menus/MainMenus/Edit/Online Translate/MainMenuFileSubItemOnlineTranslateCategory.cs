using System;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    public class MainMenuFileSubItemOnlineTranslateCategory : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Translate online");

        public override void OnClick(object sender, EventArgs e) { }

        public override int Order => base.Order - 500;
    }
}

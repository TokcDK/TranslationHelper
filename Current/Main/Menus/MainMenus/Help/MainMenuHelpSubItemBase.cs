namespace TranslationHelper.Menus.MainMenus.Edit
{
    public abstract class MainMenuHelpSubItemBase : MainMenuItemBase
    {
        public override string ParentMenuName => T._("Help");

        override public int Order => base.Order - 100000;
    }
}

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public abstract class MainMenuViewSubItemBase : MainMenuItemBase
    {
        public override string ParentMenuName => T._("View");

        override public int Order => base.Order + 6000;
    }
}

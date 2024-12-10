namespace TranslationHelper.Menus.MainMenus.Edit
{
    public abstract class MainMenuEditSubItemBase : MainMenuItemBase, IProjectMenuItem
    {
        public override string ParentMenuName => T._("Edit");

        override public int Order => base.Order + 1000;
    }
}

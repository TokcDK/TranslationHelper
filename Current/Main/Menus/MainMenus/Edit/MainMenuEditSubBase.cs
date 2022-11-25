namespace TranslationHelper.Menus.MainMenus.Edit
{
    public abstract class MainMenuEditSubItemBase : MainMenuItemBase, IProjectMenuItem
    {
        public override string ParentMenuName => T._("Edit");
    }
}

namespace TranslationHelper.Menus.MainMenus.File
{
    public abstract class MainMenuFileSubItemBase : MainMenuItemBase
    {
        public override string ParentMenuName => T._("File");
        public override int Priority => base.Priority + 10;
    }
}

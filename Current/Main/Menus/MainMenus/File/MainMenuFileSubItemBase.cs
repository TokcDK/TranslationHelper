namespace TranslationHelper.Menus.MainMenus.File
{
    public abstract class MainMenuFileSubItemBase : MainMenuItemBase
    {
        public override string ParentMenuName => T._("File");
        public override int Order => base.Order + 10000;
    }
}

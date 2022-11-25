namespace TranslationHelper.Menus.MainMenus.File.Other
{
    public abstract class MainMenuFileSubItemOtherBase : MainMenuFileSubItemBase
    {
        public override string CategoryName => T._("Other");
        public override int Order => base.Order + 500;
    }
}

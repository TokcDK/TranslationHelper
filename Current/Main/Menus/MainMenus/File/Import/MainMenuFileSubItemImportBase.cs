namespace TranslationHelper.Menus.MainMenus.File.Import
{
    public abstract class MainMenuFileSubItemImportBase : MainMenuFileSubItemBase
    {
        public override string CategoryName => T._("Import");
        public override int Order => base.Order + 60;
    }
}

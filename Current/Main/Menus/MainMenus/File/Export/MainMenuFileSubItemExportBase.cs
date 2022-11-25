namespace TranslationHelper.Menus.MainMenus.File.Export
{
    public abstract class MainMenuFileSubItemExportBase : MainMenuFileSubItemBase
    {
        public override string CategoryName => T._("Export");
        public override int Order => base.Order + 50;
    }
}

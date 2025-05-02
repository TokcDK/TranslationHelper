using System.Threading;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;
using TranslationHelper.Menus.FileRowMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    internal class MenuItemOnlineTranslateSelected : MenuItemOnlineTranslateAll, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Selected");

        public override string Description => T._("Translate selected rows");

        protected override ParameterizedThreadStart Param => (obj) => new OnlineTranslateTEST().Rows();
    }
}

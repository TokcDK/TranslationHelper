using System;
using System.Diagnostics;
using TranslationHelper.Data;
using TranslationHelper.Menus.MainMenus.View;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemOpenTranslationRulesFile : OpenPathByExplorerMainMenuViewSubItemBase
    {
        public override string Name => T._("Translation rules file");
        public override string DirPath => THSettings.TranslationRegexRulesFilePath;
    }
}

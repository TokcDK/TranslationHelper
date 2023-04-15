using System;
using System.Diagnostics;
using TranslationHelper.Data;
using TranslationHelper.Menus.MainMenus.View;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemOpenCellFixesFile : OpenPathByExplorerMainMenuViewSubItemBase
    {
        public override string Name => T._("CellFixes rules file");
        public override string DirPath => THSettings.CellFixesRegexRulesFilePath;
    }
}

using System;
using System.Diagnostics;
using TranslationHelper.Data;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemOpenCellFixesFile : MainMenuViewSubItemBase
    {
        public override string Text => T._("OpenTranslationRulesFile");

        public override string Description => Text;

        public override void OnClick(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(THSettings.CellFixesRegexRulesFilePath)) _ = Process.Start("explorer.exe", THSettings.CellFixesRegexRulesFilePath);
        }
    }
}

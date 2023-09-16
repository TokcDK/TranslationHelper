using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.FileRowMenus
{
    internal class ShowTranslationRegexMenu : FileRowMenuItemBase
    {
        public override string Text => T._("Show matching translation regex");

        public override void OnClick(object sender, EventArgs e)
        {
            new ShowTranslationRegex().Rows();
        }
    }
}

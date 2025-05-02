using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    internal class MenuItemOnlineTranslateTable : MenuItemOnlineTranslateAll, IFileRowMenuItem, IFileListMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Table");

        public override string Description => T._("Translate table rows");

        protected override ParameterizedThreadStart Param => (obj) => new OnlineTranslateTEST().Table();
    }
}

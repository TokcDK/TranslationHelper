using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    internal class MenuItemOnlineTranslateEmpty : MenuItemOnlineTranslateAll, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Empty");

        public override string Description => T._("Translate empty rows");

        protected override ParameterizedThreadStart Param => (obj) => new OnlineTranslateNewEmpty().All();
    }
}

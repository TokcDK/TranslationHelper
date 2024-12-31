using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus.FileRowMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    internal class MenuItemOnlineTranslateInterupt : MainMenuFileSubItemOnlineTranslateBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Interupt");

        public override string Description => T._("Interupt translation");

        public override void OnClick(object sender, EventArgs e)
        {
            AppSettings.InterruptTtanslation = true;
            FunctionsUI.InteruptTranslation = true;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus.FileRowMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    internal class MenuItemOnlineTranslateSelected : MenuItemOnlineTranslateAll, IFileRowMenuItem
    {
        public override string Text => T._("Selected");

        public override string Description => T._("Translate selected rows");

        protected override ParameterizedThreadStart Param => (obj) => new OnlineTranslateNew().Selected();
    }
}
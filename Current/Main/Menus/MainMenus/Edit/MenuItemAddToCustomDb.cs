﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemAddToCustomDb : MainMenuEditSubItemBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("AddToCustomDb");

        public override string Description => T._("AddToCustomDb");

        public override void OnClick(object sender, EventArgs e)
        {
            if (!AppSettings.IsFileContentFocused) return;

            _ = Task.Run(() => new LoadRowDataToCustomDb().Rows()).ConfigureAwait(false);
        }
    }
}

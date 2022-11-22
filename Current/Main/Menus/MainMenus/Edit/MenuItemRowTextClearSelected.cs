﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    internal class MenuItemRowTextClearSelected : MainMenuEditSubItemBase, IFileRowMenuItem
    {
        public override string Text => T._("Clear selected");

        public override string Description => T._("Clear translation in selected rows");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new ClearCells().Selected();
        }
    }
}
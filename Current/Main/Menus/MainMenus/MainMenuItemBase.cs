﻿using System;
using System.Windows.Forms;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus
{
    public abstract class MainMenuItemBase : MenuItemBase, IMainMenuItem
    {
        public override string ParentMenuName => "";

        public override int Priority => base.Priority + 10;
    }
}

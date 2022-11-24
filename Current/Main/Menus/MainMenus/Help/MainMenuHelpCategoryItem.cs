﻿using System;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public class MainMenuHelpCategoryItem : MainMenuItemBase
    {
        public override string CategoryName => "";
        public override string Text => T._("Help");
        public override int Priority => base.Priority + 999;

        public override void OnClick(object sender, EventArgs e) {}
    }
}

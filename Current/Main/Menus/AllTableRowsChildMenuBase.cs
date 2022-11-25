using System;
using TranslationHelper.Data;

namespace TranslationHelper.Menus
{
    public abstract class AllTableRowsChildMenuBase : MenuItemBase, IChildMenuItem
    {
        public override void OnClick(object sender, EventArgs e)
        {
            if (AppSettings.IsProcessAll) { OnAll(sender, e); }
            else if (AppSettings.IsProcessTable) { OnTable(sender, e); }
            else if (AppSettings.IsProcessSelected) OnRows(sender, e);
        }

        protected abstract void OnAll(object sender, EventArgs e);
        protected abstract void OnTable(object sender, EventArgs e);
        protected abstract void OnRows(object sender, EventArgs e);
    }
}

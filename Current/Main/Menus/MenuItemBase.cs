using System;
using System.Windows.Forms;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus
{
    public abstract class MenuItemBase : IMenuItem
    {
        public virtual string ParentMenuName => "";

        public abstract string Text { get; }

        public virtual string Description { get; } = "";

        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;

        public virtual int Priority { get; set; } = 10;
    }
}

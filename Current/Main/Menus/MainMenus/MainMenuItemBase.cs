using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Menus.MainMenus
{
    public abstract class MainMenuItemBase : IMainMenuItem
    {
        public virtual string ParentMenuName => "";

        public abstract string Text { get; }

        public virtual string Description { get; } = "";

        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);
    }
}

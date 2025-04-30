using NLog;
using System;
using System.Windows.Forms;

namespace TranslationHelper.Menus
{
    /// <summary>
    /// <see cref="IMenuItem"/> based abstract class with default values for some properties
    /// </summary>
    public abstract class MenuItemBase : IMenuItem
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public virtual string ParentMenuName => "";

        public abstract string Text { get; }

        public virtual string Description { get; } = "";

        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;

        public virtual int Order { get; set; } = 10;
    }
}

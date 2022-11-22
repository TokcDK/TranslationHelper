using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.FilesListMenus
{
    internal abstract class FileListMenuItemBase : IFileListMenuItem
    {
        public abstract string Text { get; }

        public abstract string Description { get; }

        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;
    }
}

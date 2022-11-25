using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Menus
{
    class MenusCreator
    {
        /// <summary>
        /// create project menus
        /// </summary>
        internal static void CreateMenus()
        {
            CreateContextMenuStrip();
        }

        private static void CreateContextMenuStrip()
        {
            //Load Menus
            string contextMenuName = T._("Project");

            foreach (var parent in new Dictionary<ContextMenuStrip, IMenuItem[]>()
            {
                { AppData.Main.FilesListMenus, AppData.CurrentProject.FilesListItemMenusList() },
                { AppData.Main.RowMenus, AppData.CurrentProject.FileRowItemMenusList() }
            })
            {
                var contextMenuParent = parent.Key;

                // remove old if exist
                if (contextMenuParent.Items.ContainsKey(contextMenuName))
                {
                    var menu = contextMenuParent.Items[contextMenuName];
                    if (menu != null || !menu.IsDisposed) menu.Dispose();

                    AppData.Main.Invoke((Action)(() => contextMenuParent.Items.RemoveByKey(contextMenuName)));
                }

                var contextMenuList = parent.Value;

                if (contextMenuList.Length == 0) continue;

                // create main category
                var contextMenuProjectCategory = new ToolStripMenuItem
                {
                    Text = contextMenuName
                };

                foreach (var contextMenu in contextMenuList)
                {
                    //Create new menu
                    var menu = new ToolStripMenuItem
                    {
                        Text = contextMenu.Text,
                        ToolTipText = contextMenu.Description,
                        ShortcutKeys = contextMenu.ShortcutKeys
                    };

                    //Register click event
                    menu.Click += contextMenu.OnClick;

                    //add result menu
                    contextMenuProjectCategory.DropDownItems.Add(menu);
                }

                AppData.Main.Invoke((Action)(() => contextMenuParent.Items.Add(contextMenuProjectCategory)));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Menus.ProjectMenus;

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

            foreach (var parent in new Dictionary<ContextMenuStrip, List<IProjectMenu>>()
            {
                { ProjectData.Main.CMSFilesList, ProjectData.CurrentProject.FilesListItemMenusList() },
                { ProjectData.Main.THFileElementsDataGridViewContextMenuStrip, ProjectData.CurrentProject.GridItemMenusList() }
            })
            {
                var contextMenuParent = parent.Key;

                // remove old if exist
                if (contextMenuParent.Items.ContainsKey(contextMenuName))
                {
                    var menu = contextMenuParent.Items[contextMenuName];
                    if (menu != null || !menu.IsDisposed)
                    {
                        menu.Dispose();
                    }
                    ProjectData.Main.Invoke((Action)(() => contextMenuParent.Items.RemoveByKey(contextMenuName)));
                }

                var contextMenuList = parent.Value;

                if (contextMenuList.Count == 0)
                {
                    continue;
                }

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
                        ToolTipText = contextMenu.Description
                    };

                    //Register click event
                    menu.Click += contextMenu.OnClick;

                    //add result menu
                    contextMenuProjectCategory.DropDownItems.Add(menu);
                }

                ProjectData.Main.Invoke((Action)(() => contextMenuParent.Items.Add(contextMenuProjectCategory)));
            }
        }
    }
}

using System;
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
            //Load Menus
            string filesListItemMenuName = T._("Project");

            // remove old if exist
            if (ProjectData.Main.CMSFilesList.Items.ContainsKey(filesListItemMenuName))
            {
                var menu = ProjectData.Main.CMSFilesList.Items[filesListItemMenuName];
                if (menu != null || !menu.IsDisposed)
                {
                    menu.Dispose();
                }
                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.CMSFilesList.Items.RemoveByKey(filesListItemMenuName)));
            }

            var filesListItemMenusList = ProjectData.CurrentProject.FilesListItemMenusList();

            if (filesListItemMenusList.Count > 0)
            {
                // create main category
                var filesListProjectCategory = new ToolStripMenuItem
                {
                    Text = filesListItemMenuName
                };

                foreach (var filesListItemMenu in filesListItemMenusList)
                {
                    //Create new menu
                    var menu = new ToolStripMenuItem
                    {
                        Text = filesListItemMenu.Text,
                        ToolTipText = filesListItemMenu.Description
                    };

                    //Register click event
                    menu.Click += filesListItemMenu.OnClick;

                    //add result menu
                    filesListProjectCategory.DropDownItems.Add(menu);
                }

                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.CMSFilesList.Items.Add(filesListProjectCategory)));
            }
        }
    }
}

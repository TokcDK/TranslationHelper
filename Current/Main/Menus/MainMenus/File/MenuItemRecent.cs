using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemRecent : MainMenuFileSubItemBase
    {
        public override string Text => RecentMenuName;

        public override string Description => T._("Recent oened paths");

        public override void OnClick(object sender, EventArgs e) { }
        public override int Order => base.Order + 999;

        /// <summary>
        /// Add last successfully opened project to recent files list
        /// </summary>
        internal static void UpdateRecentFiles()
        {
            if (AppData.ConfigIni == null) return;

            string[] items;
            bool changed = false;
            if (!AppData.ConfigIni.SectionExistsAndNotEmpty("RecentFiles"))
            {
                changed = true;
                items = new[] { AppData.SelectedProjectFilePath };
            }
            else
            {
                var values = AppData.ConfigIni.GetSectionValues("RecentFiles").Where(path => System.IO.File.Exists(path) || Directory.Exists(path)).ToList();

                // max 20 items
                while (values.Count >= 20)
                {
                    changed = true;
                    values.RemoveAt(values.Count - 1); // remove last when more of limit
                }

                // check if last value is on first place else update
                if (!values.Contains(AppData.SelectedProjectFilePath) && !string.IsNullOrWhiteSpace(AppData.SelectedProjectFilePath))
                {
                    changed = true;
                    values.Insert(0, AppData.SelectedProjectFilePath);
                }
                else if (values.IndexOf(AppData.SelectedProjectFilePath) > 0)
                {
                    changed = true;
                    values.Remove(AppData.SelectedProjectFilePath);
                    values.Insert(0, AppData.SelectedProjectFilePath);
                }

                items = values.ToArray();
            }


            //if (!changed) return; // write only when changed

            // save values in ini
            AppData.ConfigIni.SetArrayToSectionValues("RecentFiles", items);

            AddRecentMenuItems(items);
        }

        public static string RecentMenuName { get => T._("Recent"); }

        private static void AddRecentMenuItems(string[] items)
        {
            var recentMenuName = RecentMenuName;

            // search old menu
            ToolStripMenuItem category = null;
            bool foundOld = false;

            foreach (ToolStripMenuItem menuCategory in AppData.Main.MainMenuStrip.Items)
            {
                if (menuCategory.Text != T._("File")) continue;

                foreach (ToolStripMenuItem sub in menuCategory.DropDownItems)
                {
                    if (sub.Text != recentMenuName) continue;

                    foundOld = true;
                    category = sub;
                    break;
                }
            }

            //ProjectData.Main.fileToolStripMenuItem.DropDownItems
            if (!foundOld)
            {
                category = new ToolStripMenuItem() { Text = recentMenuName };
            }
            else
            {
                category.DropDownItems.Clear();
            }

            foreach (var item in items)
            {
                var ItemMenu = new ToolStripMenuItem() { Text = item };

                category.DropDownItems.Add(ItemMenu);
                ItemMenu.Click += RecentFilesOpen_Click;
            }

            if (!foundOld) AppData.Main.Invoke((Action)(() => AppData.Main.MainMenuStrip.Items.Add(category)));
        }

        private static void RecentFilesOpen_Click(object sender, EventArgs e) { FunctionsOpen.OpenProject((sender as ToolStripMenuItem).Text); }

        internal static void AfterOpenCleaning()
        {
            AppData.CurrentProject.TablesLinesDict?.Clear();
            AppData.ENQuotesToJPLearnDataFoundNext?.Clear();
            AppData.ENQuotesToJPLearnDataFoundPrev?.Clear();
        }
    }
}

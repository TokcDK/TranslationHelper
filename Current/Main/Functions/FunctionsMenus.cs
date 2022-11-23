using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Functions
{
    internal static class FunctionsMenus
    {
        internal static void CreateMainMenus(MenuStrip container)
        {
            var menusData = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IMainMenuItem>();

            foreach (var menuData in menusData)
            {
                if(menuData is IFileRowMenuItem) continue;
                if(menuData is IChildMenuItem) continue;

                //Create new menu
                var menu = new ToolStripMenuItem
                {
                    Text = menuData.Text,
                    ToolTipText = menuData.Description
                };

                //Register click event
                menu.Click += menuData.OnClick;

                //check parent menu
                ToolStripMenuItem parent = null;
                bool HasParent;
                if (HasParent = !string.IsNullOrWhiteSpace(menuData.ParentMenuName))
                    if (!container.Contains(menuData.ParentMenuName, out parent))
                    {
                        parent = new ToolStripMenuItem
                        {
                            Text = menuData.ParentMenuName
                        };
                    }

                //check parent category
                ToolStripMenuItem category = null;
                bool HasCategory = !string.IsNullOrWhiteSpace(menuData.CategoryName);
                if (!string.IsNullOrWhiteSpace(menuData.ParentMenuName) && HasCategory)
                    if (!parent.Contains(menuData.CategoryName, out category))
                    {
                        category = new ToolStripMenuItem
                        {
                            Text = menuData.CategoryName
                        };
                    }

                //add category and make it current if exist
                if (HasCategory)
                {
                    category.DropDownItems.Add(menu);
                    menu = category;
                }

                //add parent and make it current if exist
                if (HasParent)
                {
                    parent.DropDownItems.Add(menu);
                    menu = parent;
                }

                //add result menu
                if(!container.Items.Contains(menu)) container.Items.Add(menu);
            }
        }
        public static ToolStripMenuItem GetToolStripMenuItem(this MenuStrip toolStripMenuItem, string itemWithText)
        {
            if (!string.IsNullOrWhiteSpace(itemWithText))
            {
                foreach (ToolStripMenuItem item in toolStripMenuItem.Items)
                {
                    if (item.Text == itemWithText)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// check if toolStripMenuItem contains item with itemWithText
        /// </summary>
        /// <param name="toolStripMenuItem">Source menu where to search item with text</param>
        /// <param name="itemWithText">menu's text which to search</param>
        /// <param name="foundFoolStripMenuItem">set found ToolStripMenuItem to this else null</param>
        /// <returns>true if found and set out item, else false and null</returns>
        public static bool Contains(this MenuStrip toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem.Items)
            {
                if (item.Text == itemWithText)
                {
                    foundFoolStripMenuItem = item;
                    return true;
                }
            }

            foundFoolStripMenuItem = null;
            return false;
        }

        public static bool Contains(this ToolStripMenuItem toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem.DropDownItems)
            {
                if (item.Text == itemWithText)
                {
                    foundFoolStripMenuItem = item;
                    return true;
                }
            }

            foundFoolStripMenuItem = null;
            return false;
        }
    }
}

﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Functions
{
    public class MenuData
    {
        public string Text;
        public IMainMenuItem Menu { get; }

        public MenuData(IMainMenuItem menu, string text = "")
        {
            Text = menu != null && !string.IsNullOrWhiteSpace(menu.Text)
                ? menu.Text : text;
            Menu = menu;
        }

        public List<MenuData> Childs = new List<MenuData>();
    }

    internal static class FunctionsMenus
    {
        internal static void CreateMainMenus(MenuStrip container)
        {
            var menusData = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IMainMenuItem>();

            var menusList = new List<MenuData>();
            foreach (var menuData in menusData)
            {
                if (menuData is IFileRowMenuItem) continue;
                if (menuData is IChildMenuItem) continue;
                if (menuData is IDefaultMenuItem) continue;
                if (menuData is IProjectMenuItem) continue;

                var item = new MenuData(menuData);

                if (string.IsNullOrWhiteSpace(menuData.ParentMenuName))
                {
                    var mainMenu = menusList.FirstOrDefault(m => m.Text == menuData.Text);

                    if (mainMenu == default)
                    {
                        menusList.Add(item);
                    }
                    else if (mainMenu.Menu is DefaultMainMenu)
                    {
                        var mm = new MenuData(menuData)
                        {
                            Childs = mainMenu.Childs
                        };

                        mainMenu = mm; // relink menu for cases when it is main menu with same name
                    }

                    continue;
                }

                // create parent
                var parentMenuItem = menusList.FirstOrDefault(m => m.Text == menuData.ParentMenuName);
                if (parentMenuItem == default)
                {
                    var defMenu = new DefaultMainMenu();
                    defMenu.Priority += 100;

                    parentMenuItem = new MenuData(defMenu, menuData.ParentMenuName);
                }

                // check category
                if (!string.IsNullOrWhiteSpace(menuData.CategoryName))
                {
                    var catMenuItem = parentMenuItem.Childs.FirstOrDefault(m => m.Text == menuData.CategoryName);
                    if (catMenuItem == default)
                    {
                        var defMenu = new DefaultMainMenu();
                        defMenu.Priority += 100;

                        catMenuItem = new MenuData(defMenu, menuData.CategoryName);
                    }

                    catMenuItem.Childs.Add(item);

                    item = catMenuItem; // relink to category
                }

                if(!parentMenuItem.Childs.Contains(item)) parentMenuItem.Childs.Add(item);

                if (!menusList.Contains(parentMenuItem)) menusList.Add(parentMenuItem);
            }

            // sort by priority
            menusList = menusList.OrderBy(m => m.Menu.Priority).ToList();
            int max = menusList.Count;
            for (int i = 0; i < max; i++)
            {
                var menu = menusList[i];

                SortChilds(menu);
            }

            foreach (var menuData in menusList)
            {
                //Create new menu
                var menu = new ToolStripMenuItem
                {
                    Text = menuData.Text,
                    ToolTipText = menuData.Menu.Description,
                    ShortcutKeys = menuData.Menu.ShortcutKeys
                };

                //Register click event
                menu.Click += menuData.Menu.OnClick;

                foreach (var child in menuData.Childs)
                {
                    menu.DropDownItems.Add(SetChilds(child));
                }

                container.Items.Add(menu);
            }


            //foreach (var menuData in menusData)
            //{
            //    if (menuData is IFileRowMenuItem) continue;
            //    if (menuData is IChildMenuItem) continue;

            //    //Create new menu
            //    var menu = new ToolStripMenuItem
            //    {
            //        Text = menuData.Text,
            //        ToolTipText = menuData.Description
            //    };

            //    //Register click event
            //    menu.Click += menuData.OnClick;

            //    //check parent menu
            //    ToolStripMenuItem parent = null;
            //    bool HasParent;
            //    if (HasParent = !string.IsNullOrWhiteSpace(menuData.ParentMenuName))
            //        if (!container.Contains(menuData.ParentMenuName, out parent))
            //        {
            //            parent = new ToolStripMenuItem
            //            {
            //                Text = menuData.ParentMenuName
            //            };
            //        }

            //    //check parent category
            //    ToolStripMenuItem category = null;
            //    bool HasCategory = !string.IsNullOrWhiteSpace(menuData.CategoryName);
            //    if (!string.IsNullOrWhiteSpace(menuData.ParentMenuName) && HasCategory)
            //        if (!parent.Contains(menuData.CategoryName, out category))
            //        {
            //            category = new ToolStripMenuItem
            //            {
            //                Text = menuData.CategoryName
            //            };
            //        }

            //    //add category and make it current if exist
            //    if (HasCategory)
            //    {
            //        category.DropDownItems.Add(menu);
            //        menu = category;
            //    }

            //    //add parent and make it current if exist
            //    if (HasParent)
            //    {
            //        parent.DropDownItems.Add(menu);
            //        menu = parent;
            //    }

            //    //add result menu
            //    if (!container.Items.Contains(menu)) container.Items.Add(menu);
            //}
        }

        private static ToolStripMenuItem SetChilds(MenuData menuData)
        {
            //Create new menu
            var subMenu = new ToolStripMenuItem
            {
                Text = menuData.Text,
                ToolTipText = menuData.Menu.Description,
                ShortcutKeys = menuData.Menu.ShortcutKeys
            };

            //Register click event
            subMenu.Click += menuData.Menu.OnClick;

            foreach (var child in menuData.Childs)
            {
                subMenu.DropDownItems.Add(SetChilds(child));
            }

            return subMenu;
        }

        private static void SortChilds(MenuData menu)
        {
            if (menu.Childs == null) return;

            int max = menu.Childs.Count;
            if (max == 0) return;

            menu.Childs = menu.Childs.OrderBy(m => m.Menu.Priority).ToList();

            for (int i = 0; i < max; i++)
            {
                var child = menu.Childs[i];

                SortChilds(child);
            }
        }

        public static ToolStripMenuItem GetToolStripMenuItem(this MenuStrip toolStripMenuItem, string itemWithText)
        {
            if (string.IsNullOrWhiteSpace(itemWithText)) return null;

            foreach (ToolStripMenuItem item in toolStripMenuItem.Items) if (item.Text == itemWithText) return item;

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
                if (item.Text != itemWithText) continue;

                foundFoolStripMenuItem = item;
                return true;
            }

            foundFoolStripMenuItem = null;
            return false;
        }

        public static bool Contains(this ToolStripMenuItem toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem.DropDownItems)
            {
                if (item.Text != itemWithText) continue;

                foundFoolStripMenuItem = item;
                return true;
            }

            foundFoolStripMenuItem = null;
            return false;
        }
    }
}
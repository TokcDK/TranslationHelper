using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Menus;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus;
using TranslationHelper.Menus.MainMenus.File;

namespace TranslationHelper.Functions
{
    // ИЗМЕНИЛ списки меню в словари. в основных меню в Онлайн перевод присутствует только пункт Empty, остальные отсутствуют, ПОПРАВИТЬ!

    public class MenuData
    {
        public string Text;
        public IMenuItem Menu { get; }

        public MenuData(IMenuItem menu, string text = "")
        {
            Text = menu != null && !string.IsNullOrWhiteSpace(menu.Text)
                ? menu.Text : text;
            Menu = menu;

            if (menu.Childs == null) return;

            // add menu childs to data
            foreach (var child in menu.Childs)
            {
                var childData = new MenuData(child);
                if (Childs.ContainsKey(childData.Text)) continue;

                Childs.Add(childData.Text, childData);
            }
        }

        public Dictionary<string, MenuData> Childs = new Dictionary<string, MenuData>();
    }

    internal static class FunctionsMenus
    {
        internal static void CreateMainMenus()
        {
            var proj = AppData.CurrentProject;
            bool isProjOpen = proj != null;

            var mainMenus = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IMainMenuItem>()
                .Where(m => !(m is IProjectSpecifiedMenuItem));
            if (isProjOpen && proj.MainMenuItemMenusList != null && proj.MainMenuItemMenusList.Length > 0)
            {
                mainMenus = mainMenus.Concat(proj.MainMenuItemMenusList);
            }
            AddMainMenus(AppData.Main.MainMenus.Items, mainMenus);

            MenuItemRecent.UpdateRecentFiles();
        }
        internal static void CreateFilesListMenus()
        {
            var proj = AppData.CurrentProject;
            bool isProjOpen = proj != null;

            if (!isProjOpen) return;

            var fileListMenus = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFileListMenuItem>()
                .Where(m => !(m is IProjectSpecifiedMenuItem));
            if (proj.FilesListItemMenusList != null && proj.FilesListItemMenusList.Length > 0)
            {
                fileListMenus = fileListMenus.Concat(proj.FilesListItemMenusList);
            }
            AddMenus(AppData.Main.FilesListMenus.Items, fileListMenus);
        }

        /// <summary>
        /// Build the row menu of the project being worked on.
        /// <para>
        /// Called whenever the entry on screen changes as well as when the project does, so it has to
        /// be repeatable: what is built once is cached on the project, and every later call rebuilds
        /// the strip from that cache. The project's own row entries go into the cache rather than
        /// being appended to it, because a project may well hand out a fresh array each time it is
        /// asked and appending would then grow the cache on every call.
        /// </para>
        /// <para>
        /// The strip belongs to the window, so a call made before the window exists does nothing
        /// rather than failing: the caller does not have to know when the window is up.
        /// </para>
        /// </summary>
        internal static void CreateFileRowMenus()
        {
            var proj = AppData.CurrentProject;

            if (proj == null || AppData.Main == null) return;

            if (proj.RowMenusCache == null)
            {
                // The row entries are collected from the same implementations the other strips are
                // built from, so a project whose row menu is built for the first time is also a
                // project whose other menus have not been built for it yet.
                CreateMainMenus();
                CreateFilesListMenus();

                IEnumerable<IFileRowMenuItem> rowMenus = GetListOfSubClasses.Inherited
                    .GetInterfaceImplimentations<IFileRowMenuItem>()
                    .Where(m => !(m is IProjectSpecifiedMenuItem));

                if (proj.FileRowItemMenusList != null && proj.FileRowItemMenusList.Length > 0)
                {
                    rowMenus = rowMenus.Concat(proj.FileRowItemMenusList);
                }

                proj.RowMenusCache = rowMenus.ToArray();
            }

            AddMenus(AppData.Main.RowMenus.Items, proj.RowMenusCache);
        }
        internal static void CreateMenus()
        {
            CreateMainMenus();

            CreateFilesListMenus();

            CreateFileRowMenus();
        }

        private static void AddMenus(ToolStripItemCollection menuItems, IEnumerable<IMenuItem> menusData)
        {
            menuItems.Clear();

            var menusListDictionary = new Dictionary<string, MenuData>();
            foreach (var menuData in menusData)
            {
                if (!IsValidMenuItem(menuData)) continue;

                var item = new MenuData(menuData);

                // check category
                item = TryGetCategoryMenu(item, menuData, menusListDictionary);

                menusListDictionary.TryAdd(item.Text, item);
            }

            var sortedMenusList = SortByPriority(menusListDictionary.Values);
            CreateMenusByList(menuItems, sortedMenusList);
        }

        private static void AddMainMenus(ToolStripItemCollection menuItems, IEnumerable<IMainMenuItem> menusData)
        {
            menuItems.Clear();

            var menusListDictionary = new Dictionary<string, MenuData>();
            foreach (var menuData in menusData)
            {
                if (!IsValidMenuItem(menuData)) continue;

                var item = new MenuData(menuData);

                // when no parent, add as main menus and continue
                if (TryAddRootMainMenuItem(item, menuData, menusListDictionary))
                {
                    continue;
                }

                // create parent
                var parentMenuItem = TryGetFoundMenuItem(menusListDictionary, menuData.ParentMenuName);
                
                // check category
                item = TryGetCategoryMenu(item, menuData, parentMenuItem.Childs);

                parentMenuItem.Childs.TryAdd(item.Text, item);

                menusListDictionary.TryAdd(parentMenuItem.Text, parentMenuItem);
            }

            var sortedMenusList = SortByPriority(menusListDictionary.Values);
            CreateMenusByList(menuItems, sortedMenusList);
        }

        private static MenuData TryGetCategoryMenu(MenuData item, IMenuItem menuData, Dictionary<string, MenuData> menusListToWhereSearch)
        {
            if (string.IsNullOrWhiteSpace(menuData.CategoryName))
            {
                return item;
            }

            var categoryMenuItem = TryGetFoundMenuItem(menusListToWhereSearch, menuData.CategoryName);

            categoryMenuItem.Childs.TryAdd(item.Text, item);

            return categoryMenuItem; // relink to category
        }

        private static MenuData TryGetFoundMenuItem(Dictionary<string, MenuData> menusListToWhereSearch, string menuName, int order = 0)
        {
            if (!menusListToWhereSearch.TryGetValue(menuName, out MenuData foundMenuItem2))
            {
                var defaultMenu = new DefaultMainMenu();
                defaultMenu.Order += order;

                return new MenuData(defaultMenu, menuName);
            }
            else
            {
                return foundMenuItem2;
            }
        }

        private static bool TryAddRootMainMenuItem(MenuData item, IMainMenuItem menuData, Dictionary<string, MenuData> menusListDictionary)
        {
            if (!string.IsNullOrWhiteSpace(menuData.ParentMenuName))
            {
                return false;
            }

            if (!menusListDictionary.TryGetValue(menuData.ParentMenuName, out MenuData foundRootMainMenuItem))
            {
                menusListDictionary.Add(item.Text, item);
            }
            else
            {
                if (foundRootMainMenuItem.Menu is DefaultMainMenu)
                {
                    var mm = new MenuData(menuData)
                    {
                        Childs = foundRootMainMenuItem.Childs
                    };
                    mm.Menu.Order = foundRootMainMenuItem.Menu.Order + 500;

                    menusListDictionary[menuData.ParentMenuName] = mm; // relink menu for cases when it is main menu with same name
                }
            }

            return true;
        }

        private static bool IsValidMenuItem(IMenuItem menuData)
        {
            if (menuData is IDefaultMenuItem) return false;

            if (AppData.CurrentProject == null)
            {
                if (menuData is IFileRowMenuItem) return false;
                if (menuData is IChildMenuItem) return false;
                if (menuData is IProjectMenuItem) return false;
            }

            if (!AppSettings.IsFileOpened && menuData is IFileRowMenuItem) return false;

            return true;
        }

        private static void CreateMenusByList(ToolStripItemCollection menuItems, List<MenuData> menusList)
        {
            foreach (var menuData in menusList)
            {
                menuItems.Add(CreateMenuItem(menuData));
            }
        }

        /// <summary>
        /// Creates the menu item of <paramref name="menuData"/>, including its whole child tree.
        /// </summary>
        private static ToolStripMenuItem CreateMenuItem(MenuData menuData)
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
                menu.DropDownItems.Add(CreateMenuItem(child.Value));
            }

            return menu;
        }

        private static List<MenuData> SortByPriority(IEnumerable<MenuData> menus)
        {
            var menusList = menus.OrderByDescending(m => GetOrder(m)).ToList();
            foreach (var menu in menusList)
            {
                if (menu.Childs.Count == 0) continue;

                var sortedChilds = SortByPriority(menu.Childs.Values);

                menu.Childs = sortedChilds.ToDictionary(k => k.Text, v => v);
            }

            return menusList;
        }

        private static int GetOrder(MenuData m)
        {
            return m.Menu.Order + GetOrderByChildsCount(m);
        }

        private static int GetOrderByChildsCount(MenuData m)
        {
            return 0;

            //if(m.Menu is DefaultMainMenu)
            //{
            //    return 0;
            //}

            //return (m.Childs.Count * 1000);
        }

        //public static ToolStripMenuItem GetToolStripMenuItem(this MenuStrip toolStripMenuItem, string itemWithText)
        //{
        //    if (string.IsNullOrWhiteSpace(itemWithText)) return null;

        //    foreach (ToolStripMenuItem item in toolStripMenuItem.Items) if (item.Text == itemWithText) return item;

        //    return null;
        //}

        ///// <summary>
        ///// check if toolStripMenuItem contains item with itemWithText
        ///// </summary>
        ///// <param name="toolStripMenuItem">Source menu where to search item with text</param>
        ///// <param name="itemWithText">menu's text which to search</param>
        ///// <param name="foundFoolStripMenuItem">set found ToolStripMenuItem to this else null</param>
        ///// <returns>true if found and set out item, else false and null</returns>
        //public static bool Contains(this MenuStrip toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        //{
        //    foreach (ToolStripMenuItem item in toolStripMenuItem.Items)
        //    {
        //        if (item.Text != itemWithText) continue;

        //        foundFoolStripMenuItem = item;
        //        return true;
        //    }

        //    foundFoolStripMenuItem = null;
        //    return false;
        //}

        //public static bool Contains(this ToolStripMenuItem toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        //{
        //    foreach (ToolStripMenuItem item in toolStripMenuItem.DropDownItems)
        //    {
        //        if (item.Text != itemWithText) continue;

        //        foundFoolStripMenuItem = item;
        //        return true;
        //    }

        //    foundFoolStripMenuItem = null;
        //    return false;
        //}
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
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

        internal static void CreateFileRowMenus()
        {
            var proj = AppData.CurrentProject;
            bool isProjOpen = proj != null;

            if (!isProjOpen) return;

            bool isCached = proj.RowMenusCache != null;
            if (!isCached)
            {
                CreateMainMenus();
                CreateFilesListMenus();
            }

            var rowMenus = isCached ? proj.RowMenusCache : 
                (proj.RowMenusCache = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFileRowMenuItem>()
                .Where(m => !(m is IProjectSpecifiedMenuItem)).ToArray());
            if (proj.FileRowItemMenusList != null && proj.FileRowItemMenusList.Length > 0)
            {
                proj.RowMenusCache = proj.RowMenusCache.Concat(proj.FileRowItemMenusList).ToArray();
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

            var menusListDictionary = new Dictionary<string,MenuData>();
            foreach (var menuData in menusData)
            {
                if (!IsValidMenuItem(menuData)) continue;

                var item = new MenuData(menuData);

                // check category
                if (!string.IsNullOrWhiteSpace(menuData.CategoryName))
                {
                    MenuData catMenuItem;
                    if (!menusListDictionary.ContainsKey(menuData.CategoryName))
                    {
                        var defMenu = new DefaultMainMenu();
                        defMenu.Order += 100;

                        catMenuItem = new MenuData(defMenu, menuData.CategoryName);
                    }
                    else
                    {
                       catMenuItem = menusListDictionary[menuData.CategoryName];
                    }

                    catMenuItem.Childs.Add(item.Text, item);

                    item = catMenuItem; // relink to category
                }

                if (!menusListDictionary.ContainsKey(item.Text)) menusListDictionary.Add(item.Text, item);
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
                if (string.IsNullOrWhiteSpace(menuData.ParentMenuName))
                {
                    if (!menusListDictionary.ContainsKey(menuData.ParentMenuName))
                    {
                        menusListDictionary.Add(item.Text, item);
                    }
                    else
                    {
                        var mainMenu = menusListDictionary[menuData.ParentMenuName];
                        if (mainMenu.Menu is DefaultMainMenu)
                        {
                            var mm = new MenuData(menuData)
                            {
                                Childs = mainMenu.Childs
                            };

                            menusListDictionary[menuData.ParentMenuName] = mm; // relink menu for cases when it is main menu with same name
                        }
                    }

                    continue;
                }

                // create parent
                MenuData parentMenuItem;
                if (!menusListDictionary.ContainsKey(menuData.ParentMenuName))
                {
                    var defMenu = new DefaultMainMenu();
                    defMenu.Order += 100;

                    parentMenuItem = new MenuData(defMenu, menuData.ParentMenuName);
                }
                else
                {
                    parentMenuItem = menusListDictionary[menuData.ParentMenuName];
                }

                // check category
                if (!string.IsNullOrWhiteSpace(menuData.CategoryName))
                {
                    MenuData catMenuItem;
                    if (!menusListDictionary.ContainsKey(menuData.CategoryName))
                    {
                        var defMenu = new DefaultMainMenu();
                        defMenu.Order += 100;

                        catMenuItem = new MenuData(defMenu, menuData.CategoryName);
                    }
                    else                    
                    {
                        catMenuItem = menusListDictionary[menuData.CategoryName];
                    }

                    catMenuItem.Childs.Add(item.Text, item);

                    item = catMenuItem; // relink to category
                }

                if (!parentMenuItem.Childs.ContainsKey(item.Text))
                {
                    parentMenuItem.Childs.Add(item.Text, item);
                }

                if (!menusListDictionary.ContainsKey(parentMenuItem.Text))
                {
                    menusListDictionary.Add(parentMenuItem.Text, parentMenuItem);
                }
            }

            var sortedMenusList = SortByPriority(menusListDictionary.Values);
            CreateMenusByList(menuItems, sortedMenusList);
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
                    menu.DropDownItems.Add(SetChilds(child.Value));
                }

                menuItems.Add(menu);
            }
        }

        private static List<MenuData> SortByPriority(IEnumerable<MenuData> menus)
        {
            var menusList = menus.OrderBy(m => m.Menu.Order).ToList();
            int max = menusList.Count;
            for (int i = 0; i < max; i++)
            {
                var menu = menusList[i];

                SortChilds(menu);
            }

            return menusList;
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
                subMenu.DropDownItems.Add(SetChilds(child.Value));
            }

            return subMenu;
        }

        private static void SortChilds(MenuData menu)
        {
            if (menu.Childs == null) return;

            int max = menu.Childs.Count;
            if (max == 0) return;

            menu.Childs = menu.Childs.Values.OrderBy(m => m.Menu.Order).ToDictionary(k=>k.Text,v =>v);

            foreach (var child in menu.Childs)
            {
                SortChilds(child.Value);
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

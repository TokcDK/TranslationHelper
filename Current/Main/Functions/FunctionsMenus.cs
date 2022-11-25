using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Menus;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus;

namespace TranslationHelper.Functions
{
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
                if (Childs.Contains(childData)) continue;

                Childs.Add(childData);
            }
        }

        public List<MenuData> Childs = new List<MenuData>();
    }

    internal static class FunctionsMenus
    {
        internal static void CreateMenus()
        {
            var proj = AppData.CurrentProject;
            bool isProjOpen = proj != null;

            var mainMenus = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IMainMenuItem>().Where(m => !(m is IProjectSpecifiedMenuItem));
            if (isProjOpen && proj.MainMenuItemMenusList != null && proj.MainMenuItemMenusList.Length > 0)
            {
                mainMenus = mainMenus.Concat(proj.MainMenuItemMenusList);
            }
            AddMainMenus(AppData.Main.MainMenus.Items, mainMenus);

            if (!isProjOpen) return;

            var fileListMenus = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFileListMenuItem>().Where(m => !(m is IProjectSpecifiedMenuItem));
            if (proj.FilesListItemMenusList != null && proj.FilesListItemMenusList.Length > 0)
            {
                fileListMenus = fileListMenus.Concat(proj.FilesListItemMenusList);
            }
            AddMenus(AppData.Main.FilesListMenus.Items, fileListMenus);

            var rowMenus = GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFileRowMenuItem>().Where(m => !(m is IProjectSpecifiedMenuItem));
            if (proj.FileRowItemMenusList != null && proj.FileRowItemMenusList.Length > 0)
            {
                rowMenus = rowMenus.Concat(proj.FileRowItemMenusList);
            }
            AddMenus(AppData.Main.RowMenus.Items, rowMenus);
        }

        private static void AddMenus(ToolStripItemCollection menuItems, IEnumerable<IMenuItem> menusData)
        {
            menuItems.Clear();

            var menusList = new List<MenuData>();
            foreach (var menuData in menusData)
            {
                if (!IsValidMenuItem(menuData)) continue;

                var item = new MenuData(menuData);

                // check category
                if (!string.IsNullOrWhiteSpace(menuData.CategoryName))
                {
                    var catMenuItem = menusList.FirstOrDefault(m => m.Text == menuData.CategoryName);
                    if (catMenuItem == default)
                    {
                        var defMenu = new DefaultMainMenu();
                        defMenu.Order += 100;

                        catMenuItem = new MenuData(defMenu, menuData.CategoryName);
                    }

                    catMenuItem.Childs.Add(item);

                    item = catMenuItem; // relink to category
                }

                if (!menusList.Contains(item)) menusList.Add(item);
            }

            SortByPriority(ref menusList);
            CreateMenusByList(menuItems, menusList);
        }

        private static void AddMainMenus(ToolStripItemCollection menuItems, IEnumerable<IMainMenuItem> menusData)
        {
            menuItems.Clear();

            var menusList = new List<MenuData>();
            foreach (var menuData in menusData)
            {
                if (!IsValidMenuItem(menuData)) continue;

                var item = new MenuData(menuData);

                // when no parent, add as main menus and continue
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
                    defMenu.Order += 100;

                    parentMenuItem = new MenuData(defMenu, menuData.ParentMenuName);
                }

                // check category
                if (!string.IsNullOrWhiteSpace(menuData.CategoryName))
                {
                    var catMenuItem = parentMenuItem.Childs.FirstOrDefault(m => m.Text == menuData.CategoryName);
                    if (catMenuItem == default)
                    {
                        var defMenu = new DefaultMainMenu();
                        defMenu.Order += 100;

                        catMenuItem = new MenuData(defMenu, menuData.CategoryName);
                    }

                    catMenuItem.Childs.Add(item);

                    item = catMenuItem; // relink to category
                }

                if (!parentMenuItem.Childs.Contains(item)) parentMenuItem.Childs.Add(item);

                if (!menusList.Contains(parentMenuItem)) menusList.Add(parentMenuItem);
            }

            SortByPriority(ref menusList);
            CreateMenusByList(menuItems, menusList);


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
                    menu.DropDownItems.Add(SetChilds(child));
                }

                menuItems.Add(menu);
            }
        }

        private static void SortByPriority(ref List<MenuData> menusList)
        {
            menusList = menusList.OrderBy(m => m.Menu.Order).ToList();
            int max = menusList.Count;
            for (int i = 0; i < max; i++)
            {
                var menu = menusList[i];

                SortChilds(menu);
            }
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

            menu.Childs = menu.Childs.OrderBy(m => m.Menu.Order).ToList();

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

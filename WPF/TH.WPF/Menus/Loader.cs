﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetListOfSubClasses;
using TH.WPF.Core.Data.Project;
using TH.WPF.Menus.Main;
using TH.WPF.Models;
using static TH.WPF.ViewModels.MainVM;

namespace TH.WPF.Menus
{
    internal static class Loader
    {
        internal static void LoadMainMenus(ICollection<MenuItemData> menus)
        {
            // clear examples data
            menus.Clear();

            // Load Main menus
            var mainMenus = Inherited.GetInterfaceImplimentations<IMainMenuItem>();

            foreach (var menu in mainMenus)
            {
                MenuItemData? menu2add = null;
                bool isNeedParent = !string.IsNullOrWhiteSpace(menu.ParentMenuName);
                if (isNeedParent)
                {
                    // search main menu item
                    var searchMenu = menus.FirstOrDefault(m=>string.Equals(m.Name, menu.ParentMenuName, StringComparison.InvariantCultureIgnoreCase));

                    if (searchMenu != default) menu2add = searchMenu;
                }

                if(isNeedParent && menu2add == null)
                {
                    // create main menu if was not found but need to be created
                    menu2add = new MenuItemData(menu.ParentMenuName, menu.ParentMenuName);
                }

                if (menu2add != null)
                {
                    // create menu data from current menu
                    var menuItemData = new MenuItemData(menu.Name, menu.Description)
                    {
                        Command = new RelayCommand(obj => { menu.Command(); })
                    };

                    // category check and setup
                    if (!string.IsNullOrWhiteSpace(menu.CategoryName))
                    {
                        var categoryMenuItemData = menu2add.Childs.FirstOrDefault(m => string.Equals(m.Name, menu.CategoryName, StringComparison.InvariantCultureIgnoreCase));
                        if (categoryMenuItemData == default)
                        {
                            var newCategoryMenuItemData = new MenuItemData(menu.CategoryName, menu.CategoryName);
                            newCategoryMenuItemData.Childs.Add(menuItemData);
                            menuItemData = newCategoryMenuItemData; // relink to the new category menu
                        }
                        else
                        {
                            categoryMenuItemData.Childs.Add(menuItemData);
                            menuItemData = categoryMenuItemData; // relink to the exist category menu
                        }
                    }

                    // add if missing in case if it is category manu
                    if (!menu2add.Childs.Contains(menuItemData)) menu2add.Childs.Add(menuItemData);
                }
                else
                {
                    // create new main menu item from current menu
                    menu2add = new MenuItemData(menu.Name, menu.Description)
                    {
                        Command = new RelayCommand(obj => { menu.Command(); })
                    };
                }

                // add main menu if missing
                if (!menus.Contains(menu2add)) menus.Add(menu2add);
            }
        }
    }
}

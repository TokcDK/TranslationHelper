using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetListOfSubClasses;
using TH.WPF.Menus.Main;

namespace TH.WPF.Menus
{
    internal static class Loader
    {
        internal static void LoadMainMenus(ICollection<MenuItemData> menus)
        {
            menus.Clear();

            //Load Main menus
            var mainMenus = Inherited.GetInterfaceImplimentations<IMainMenuItem>();

            foreach (var menu in mainMenus)
            {
                MenuItemData? menu2add = null;
                bool isNeedParent = !string.IsNullOrWhiteSpace(menu.ParentMenuName);
                if (isNeedParent)
                {
                    var searchMenu = menus.FirstOrDefault(m=>string.Equals(m.Name, menu.ParentMenuName, StringComparison.InvariantCultureIgnoreCase));

                    if (searchMenu != default) menu2add = searchMenu;
                }

                if(isNeedParent && menu2add == null)
                    menu2add = new MenuItemData(menu.ParentMenuName, menu.ParentMenuName, null);

                if (menu2add != null)
                {
                    var m = new MenuItemData(menu.Name, menu.Description, menu.Command);
                    menu2add.Childs.Add(m);
                }
                else
                {
                    menu2add = new MenuItemData(menu.Name, menu.Description, menu.Command);
                    menus.Add(menu2add);
                }

                if (!menus.Contains(menu2add)) menus.Add(menu2add);
            }
        }
    }
}

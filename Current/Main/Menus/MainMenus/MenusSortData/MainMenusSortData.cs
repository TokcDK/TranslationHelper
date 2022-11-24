using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Menus.MainMenus.MenusSortData
{
    internal class MainMenusSortData : IMenusSortData
    {
        public string Name => "";

        public string NameT => "";

        public IMenusSortData[] ChildSortData => new IMenusSortData[] 
        { 
            new MainMenusFileSortData(),
        };
    }
    internal class MainMenusFileSortData : IMenusSortData
    {
        public string Name => "File";

        public string NameT => T._(Name);

        public IMenusSortData[] ChildSortData => new IMenusSortData[]
        {

        };
    }
    internal class MainMenusEditSortData : IMenusSortData
    {
        public string Name => "Edit";

        public string NameT => T._(Name);

        public IMenusSortData[] ChildSortData => new IMenusSortData[]
        {

        };
    }


}

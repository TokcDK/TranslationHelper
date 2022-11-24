using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Menus.MainMenus.MenusSortData
{
    public interface IMenusSortData
    {
        string Name { get; }
        string NameT { get; }

        IMenusSortData[] ChildSortData { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TH.WPF.Menus
{
    public interface IMainMenuItem: IMenu
    {
        /// <summary>
        /// Parent menu name, for example File, Edit or kind of
        /// </summary>
        string ParentMenuName { get; }
        /// <summary>
        /// Menu category if it is submenu
        /// </summary>
        string CategoryName { get; }
    }
}

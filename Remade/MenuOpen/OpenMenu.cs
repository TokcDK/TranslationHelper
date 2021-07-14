using Menus;
using System;
using THCore;

namespace MenuOpen
{
    public class OpenMenu : IMenu
    {
        public string Text => "Open";

        public string Description => "Open project";

        public string Parent => SharedData.Menus.FileMenuName;

        public string Category => null;

        public void OnClick(object sender, EventArgs e)
        {
        }
    }
}

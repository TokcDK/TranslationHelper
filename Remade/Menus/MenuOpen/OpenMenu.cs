using l10n;
using Menus;
using System;
using THCore;

namespace MenuOpen
{
    public class OpenMenu : IMenu
    {
        public string Text => T._("Open");

        public string Description => T._("Open project");

        public string Parent => SharedData.Menus.FileMenuName;

        public string Category => null;

        public void OnClick(object sender, EventArgs e)
        {
        }
    }
}

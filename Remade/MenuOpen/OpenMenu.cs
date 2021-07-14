using Menus;
using System;

namespace MenuOpen
{
    public class OpenMenu : IMenu
    {
        public string Text => "Open";

        public string Description => "Open project";

        public string Parent => "File";

        public string Category => null;

        public void OnClick(object sender, EventArgs e)
        {
        }
    }
}

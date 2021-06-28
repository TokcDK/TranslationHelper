using Menus;
using System;
using System.Windows.Forms;

namespace TestMenu
{
    public class TestMenu : IMenu
    {
        public string Text => "TestMenu";

        public string ToolTipText => "This Is Test Menu";

        public string Parent => "File";

        public string Category => "TestCategory";

        public void OnClick(object sender, EventArgs e)
        {
            MessageBox.Show("TEst!!!");
        }
    }
}

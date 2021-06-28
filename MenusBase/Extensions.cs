using System.Windows.Forms;

namespace Menus
{
    public static class Extensions
    {
        public static ToolStripMenuItem GetToolStripMenuItem(this MenuStrip toolStripMenuItem, string itemWithText)
        {
            if (!string.IsNullOrWhiteSpace(itemWithText))
            {
                foreach (ToolStripMenuItem item in toolStripMenuItem.Items)
                {
                    if (item.Text == itemWithText)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public static bool Contains(this MenuStrip toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem.Items)
            {
                if (item.Text == itemWithText)
                {
                    foundFoolStripMenuItem = item;
                    return true;
                }
            }

            foundFoolStripMenuItem = null;
            return false;
        }

        public static bool Contains(this ToolStripMenuItem toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem.DropDownItems)
            {
                if (item.Text == itemWithText)
                {
                    foundFoolStripMenuItem = item;
                    return true;
                }
            }

            foundFoolStripMenuItem = null;
            return false;
        }
    }
}

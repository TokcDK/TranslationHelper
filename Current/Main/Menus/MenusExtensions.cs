using System.Windows.Forms;

namespace TranslationHelper.Menus
{
    static class MenusExtensions
    {
        /// <summary>
        /// check if toolStripMenuItem contains item with itemWithText
        /// </summary>
        /// <param name="toolStripMenuItem">Source menu where to search item with text</param>
        /// <param name="itemWithText">menu's text which to search</param>
        /// <param name="foundFoolStripMenuItem">set found ToolStripMenuItem to this else null</param>
        /// <returns>true if found and set out item, else false and null</returns>
        public static bool Contains(this MenuStrip toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            return toolStripMenuItem.Items.Contains(itemWithText, out foundFoolStripMenuItem);
        }

        public static bool Contains(this ToolStripMenuItem toolStripMenuItem, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            return toolStripMenuItem.DropDownItems.Contains(itemWithText, out foundFoolStripMenuItem);
        }

        public static bool Contains(this ToolStripItemCollection toolStripMenuItems, string itemWithText, out ToolStripMenuItem foundFoolStripMenuItem)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItems)
            {
                if (item.Text != itemWithText) continue;

                foundFoolStripMenuItem = item;
                return true;
            }

            foundFoolStripMenuItem = null;
            return false;
        }
    }
}

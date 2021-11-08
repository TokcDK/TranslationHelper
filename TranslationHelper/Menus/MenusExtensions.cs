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

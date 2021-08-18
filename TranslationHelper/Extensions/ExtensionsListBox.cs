using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TranslationHelper.Extensions
{
    static class ExtensionsListBox
    {
        /// <summary>
        /// Get names from selected items in the <paramref name="listBox"/>
        /// </summary>
        /// <returns>String with names of selected items of <paramref name="listBox"/> splitted by new line</returns>
        internal static string CopySelectedNames(this ListBox listBox)
        {
            if (listBox == null || listBox.Items.Count == 0 || listBox.SelectedIndex == -1)
            {
                return "";
            }

            var names = new List<string>(listBox.SelectedItems.Count);
            foreach (var item in listBox.SelectedItems)
            {
                names.Add(item.ToString());
            }
            return string.Join(Environment.NewLine, names);
        }

        /// <summary>
        /// Get indexes of selected items in the <paramref name="listBox"/>
        /// </summary>
        /// <returns></returns>
        internal static int[] CopySelectedIndexes(this ListBox listBox)
        {
            if (listBox == null || listBox.Items.Count == 0 || listBox.SelectedIndex == -1)
            {
                return new int[1] { -1 };
            }

            var indexes = new int[listBox.SelectedItems.Count];
            int i = 0;
            foreach (var index in listBox.SelectedIndices)
            {
                indexes[i] = (int)index;
                i++;
            }

            return indexes;
        }

        /// <summary>
        /// Get name of selected item name from <paramref name="listControl"/> by <paramref name="itemIndex"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal static string GetItemName(this ListControl listControl, int itemIndex)
        {
            return (listControl as ListBox).Items[itemIndex] + "";
            //return ((listControl as ListBox).Items[itemIndex] as FilesListData).FIleName;
        }

        /// <summary>
        /// Get items count of <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <returns></returns>
        internal static int GetItemsCount(this ListControl listControl)
        {
            return (listControl as ListBox).Items.Count;
        }

        /// <summary>
        /// Add new <paramref name="item"/> to the <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="item"></param>
        internal static void AddItem(this ListControl listControl, object item)
        {
            (listControl as ListBox).Items.Add(item);
        }

        internal static int GetSelectedIndex(this ListControl listControl)
        {
            return (listControl as ListBox).SelectedIndex;
        }

        internal static void SetSelectedIndex(this ListControl listControl, int itemIndex)
        {
            (listControl as ListBox).SelectedIndex = itemIndex;
        }

        internal static void SetDrawMode(this ListControl listControl, DrawMode drawMode)
        {
            (listControl as ListBox).DrawMode = drawMode;
        }
    }
}

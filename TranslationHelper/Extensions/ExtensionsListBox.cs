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
    }
}

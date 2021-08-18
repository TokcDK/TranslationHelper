﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationHelper.Data;

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

            var names = new List<string>(listBox.GetSelectedItemsCount());
            foreach (var item in listBox.GetSelectedItems())
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

            var indexes = new int[listBox.GetSelectedItemsCount()];
            int i = 0;
            foreach (var index in ProjectData.FilesListControl.GetSelectedIndexes())
            {
                indexes[i] = (int)index;
                i++;
            }

            return indexes;
        }

        /// <summary>
        /// Get name of selected item name from <paramref name="listControl"/> by <paramref name="index"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static string GetItemName(this ListControl listControl, int index)
        {
            return ProjectData.FilesListControl.GetItemName(index);
            //return (listControl as ListBox).Items[index] + "";
            //return ((listControl as ListBox).Items[itemIndex] as FilesListData).FIleName;
        }

        /// <summary>
        /// Get name of selected item name from <paramref name="listControl"/> by <paramref name="index"/>.
        /// It includes index in list
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static string GetItemNameWithIndex(this ListControl listControl, int index)
        {
            return (index + 1) + " " + ProjectData.FilesListControl.GetItemName(index);
            //return (listControl as ListBox).Items[index] + "";
            //return ((listControl as ListBox).Items[itemIndex] as FilesListData).FIleName;
        }

        /// <summary>
        /// Get items count of <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <returns></returns>
        internal static int GetItemsCount(this ListControl listControl)
        {
            return ProjectData.FilesListControl.GetItemsCount();
            //return (listControl as ListBox).Items.Count;
        }

        /// <summary>
        /// Get selected items count of <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <returns></returns>
        internal static int GetSelectedItemsCount(this ListControl listControl)
        {
            return ProjectData.FilesListControl.GetSelectedItemsCount();
            //return (listControl as ListBox).SelectedItems.Count;
        }

        /// <summary>
        /// Get selected items count of <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <returns></returns>
        internal static object[] GetSelectedItems(this ListControl listControl)
        {
            return ProjectData.FilesListControl.GetSelectedItems();
            //return (listControl as ListBox).SelectedItems.Count;
        }

        /// <summary>
        /// Add new <paramref name="item"/> to the <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="item"></param>
        internal static void AddItem(this ListControl listControl, object item)
        {
            ProjectData.FilesListControl.AddItem(item);
            //(listControl as ListBox).Items.Add(item);
        }

        /// <summary>
        /// Get selected <paramref name="itemIndex"/> for the <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <returns></returns>
        internal static int GetSelectedIndex(this ListControl listControl)
        {
            return ProjectData.FilesListControl.GetSelectedIndex();
            //return (listControl as ListBox).SelectedIndex;
        }

        /// <summary>
        /// Set selected <paramref name="index"/> for the <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="index"></param>
        internal static void SetSelectedIndex(this ListControl listControl, int index, bool clearSelected = true)
        {
            ProjectData.FilesListControl.SetSelectedIndex(index, clearSelected);

            //(listControl as ListBox).ClearSelected();
            //(listControl as ListBox).SelectedIndex = index;
        }

        /// <summary>
        /// set <paramref name="drawMode"/> of the <paramref name="listControl"/>
        /// </summary>
        /// <param name="listControl"></param>
        /// <param name="drawMode"></param>
        internal static void SetDrawMode(this ListControl listControl, DrawMode drawMode)
        {
            ProjectData.FilesListControl.SetDrawMode(drawMode);
            //(listControl as ListBox).DrawMode = drawMode;
        }
    }
}

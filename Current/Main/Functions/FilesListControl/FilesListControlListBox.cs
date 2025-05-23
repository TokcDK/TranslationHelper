﻿using System;
using System.Drawing;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FilesListControl
{
    class FilesListControlListBox : FilesListControlBase, IDisposable
    {
        ListBox _listBox;

        public override Control FilesListControl { get => _listBox; protected set => _listBox = value as ListBox; }

        public override bool Focused => _listBox.Focused;

        public FilesListControlListBox()
        {
            //_listBox = new ListBox();
            //_listBox = ProjectData.FilesList as ListBox;
            _listBox = AppData.THFilesList;

            // register events
            _listBox.DrawItem += ListBox_DrawItem;
            _listBox.MouseUp += ListBox_MouseUp;
            _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
        }

        public override string GetItemName(int index)
        {
            var fullName = _listBox.Items[index] + "";
            var subPathEndIndex = fullName.LastIndexOf('\\');
            return subPathEndIndex == -1 ? fullName : fullName.Substring(subPathEndIndex + 1);
        }

        public override int GetItemsCount()
        {
            return _listBox.Items.Count;
        }

        public override int GetSelectedItemsCount()
        {
            return _listBox.SelectedItems.Count;
        }

        public override int GetSelectedIndex()
        {
            return _listBox.SelectedIndex;
        }

        public override void SetSelectedIndex(int index, bool clearSelected = true)
        {
            if (clearSelected)
            {
                _listBox.ClearSelected();
            }

            _listBox.SelectedIndex = index;
        }

        public override void AddItem(object item)
        {
            _listBox.Items.Add(item);
        }

        public override object[] GetSelectedItems()
        {
            var items = new object[GetSelectedItemsCount()];
            int i = 0;
            foreach (var item in _listBox.SelectedItems)
            {
                items[i++] = item;
            }

            return items;
        }

        public override int[] GetSelectedIndexes()
        {
            var indexes = new int[GetSelectedItemsCount()];
            int i = 0;
            foreach (var index in _listBox.SelectedIndices)
            {
                indexes[i++] = (int)index;
            }

            return indexes;
        }

        public override void SetDrawMode(DrawMode drawMode)
        {
            _listBox.DrawMode = drawMode;
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionsUI.ActionsOnTHFIlesListElementSelected();
            AppData.Main.TableCompleteInfoLabel.Visible = true;
        }

        private void ListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = _listBox.IndexFromPoint(e.Location);
                if (item >= 0)
                {
                    //_listBox.SetSelectedIndex(item);
                    AppData.Main.FilesListMenus.Show(_listBox, e.Location);
                }
            }
        }

        //global brushes with ordinary/selected colors
        private readonly SolidBrush ListBoxItemForegroundBrushSelected = new SolidBrush(Color.White);
        private readonly SolidBrush ListBoxItemForegroundBrush = new SolidBrush(Color.Black);
        private readonly SolidBrush ListBoxItemBackgroundBrushSelected = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
        private readonly SolidBrush ListBoxItemBackgroundBrush1 = new SolidBrush(Color.White);
        private readonly SolidBrush ListBoxItemBackgroundBrush1Complete = new SolidBrush(Color.FromArgb(235, 255, 235));
        private readonly SolidBrush ListBoxItemBackgroundBrush2 = new SolidBrush(Color.FromArgb(235, 240, 235));
        private readonly SolidBrush ListBoxItemBackgroundBrush2Complete = new SolidBrush(Color.FromArgb(225, 255, 225));
        //custom method to draw the items, don't forget to set DrawMode of the ListBox to OwnerDrawFixed
        public void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            //раскраска строк
            //https://stackoverflow.com/questions/2554609/c-sharp-changing-listbox-row-color
            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            e.DrawBackground();

            int index = e.Index;
            if (index >= 0 && index < AppData.THFilesList.GetItemsCount())
            {
                bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                string text = _listBox.GetItemNameWithIndex(index);
                Graphics g = e.Graphics;

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                {
                    backgroundBrush = ListBoxItemBackgroundBrushSelected;
                }
                else if ((index % 2) == 0)
                {
                    if (FunctionsTable.IsTableColumnCellsAll(AppData.CurrentProject.FilesContent.Tables[e.Index]))
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush1Complete;
                    }
                    else
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush1;
                    }
                }
                else
                {
                    if (FunctionsTable.IsTableColumnCellsAll(AppData.CurrentProject.FilesContent.Tables[e.Index]))
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush2Complete;
                    }
                    else
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush2;
                    }
                }

                g.FillRectangle(backgroundBrush, e.Bounds);

                //text:
                SolidBrush foregroundBrush = (selected) ? ListBoxItemForegroundBrushSelected : ListBoxItemForegroundBrush;
                g.DrawString(text, e.Font, foregroundBrush, AppData.THFilesList.GetItemRectangle(index).Location);
            }

            e.DrawFocusRectangle();
        }

        public void Dispose() { if (!_listBox.IsDisposed) _listBox.Dispose(); }
    }
}

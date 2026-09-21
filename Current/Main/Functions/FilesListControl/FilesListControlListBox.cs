using System;
using System.Drawing;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Theming;

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

            // The rows are painted here rather than by the framework, so the colours have to come from
            // the theme and be rebuilt whenever it changes. This is the one part of the files list
            // the theme applicator cannot reach: the list itself is a control, but the code that
            // decides what colour each row is drawn in is not.
            ThemeManager.Instance.ThemeChanged += ListBox_ThemeChanged;
            ApplyTheme(ThemeManager.Instance.CurrentTheme);
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

        // Row colours, rebuilt when the theme changes rather than created per item: every visible row
        // is drawn again on every scroll, and a list can hold thousands of entries.
        private SolidBrush _foregroundSelected;
        private SolidBrush _foreground;
        private SolidBrush _backgroundSelected;
        private SolidBrush _backgroundRow1;
        private SolidBrush _backgroundRow1Complete;
        private SolidBrush _backgroundRow2;
        private SolidBrush _backgroundRow2Complete;

        /// <summary>
        /// Rebuilds the row brushes from <paramref name="theme"/>.
        /// <para>
        /// The brushes of the theme being left behind are released here, so the theme can be changed
        /// any number of times without the previous one's brushes being kept alive.
        /// </para>
        /// </summary>
        private void ApplyTheme(ITheme theme)
        {
            DisposeBrushes();

            _foregroundSelected = new SolidBrush(theme.ListRowSelectedText);
            _foreground = new SolidBrush(theme.ListRowText);
            _backgroundSelected = new SolidBrush(theme.ListRowSelectedBack);
            _backgroundRow1 = new SolidBrush(theme.ListRowBack);
            _backgroundRow1Complete = new SolidBrush(theme.ListRowBackComplete);
            _backgroundRow2 = new SolidBrush(theme.ListRowAlternateBack);
            _backgroundRow2Complete = new SolidBrush(theme.ListRowAlternateBackComplete);

            // Repaint the rows already on screen in the colours that were just installed.
            _listBox?.Invalidate();
        }

        private void ListBox_ThemeChanged(object sender, ITheme theme)
        {
            ApplyTheme(theme);
        }

        private void DisposeBrushes()
        {
            _foregroundSelected?.Dispose();
            _foreground?.Dispose();
            _backgroundSelected?.Dispose();
            _backgroundRow1?.Dispose();
            _backgroundRow1Complete?.Dispose();
            _backgroundRow2?.Dispose();
            _backgroundRow2Complete?.Dispose();
        }

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

                // the entry presents a file, or every file at once for the "[ALL]" entry
                var table = AppData.FilesListContent?.GetTable(index);
                bool isComplete = table != null && FunctionsTable.IsTableColumnCellsAll(table);

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                {
                    backgroundBrush = _backgroundSelected;
                }
                else if ((index % 2) == 0)
                {
                    backgroundBrush = isComplete ? _backgroundRow1Complete : _backgroundRow1;
                }
                else
                {
                    backgroundBrush = isComplete ? _backgroundRow2Complete : _backgroundRow2;
                }

                g.FillRectangle(backgroundBrush, e.Bounds);

                //text:
                SolidBrush foregroundBrush = (selected) ? _foregroundSelected : _foreground;
                g.DrawString(text, e.Font, foregroundBrush, AppData.THFilesList.GetItemRectangle(index).Location);
            }

            e.DrawFocusRectangle();
        }

        public void Dispose()
        {
            //The list box belongs to the main form (AppData.THFilesList), so this adapter must not
            //dispose it; it only releases what it created itself: the event subscriptions and the
            //brushes. The previous version disposed the app wide ListBox and leaked the brushes.
            _listBox.DrawItem -= ListBox_DrawItem;
            _listBox.MouseUp -= ListBox_MouseUp;
            _listBox.SelectedIndexChanged -= ListBox_SelectedIndexChanged;
            ThemeManager.Instance.ThemeChanged -= ListBox_ThemeChanged;

            DisposeBrushes();
        }
    }
}

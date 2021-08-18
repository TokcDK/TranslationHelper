using System.Drawing;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FilesListControl
{
    class FilesListControlListBox : FilesListControlBase
    {
        ListBox _listBox = ProjectData.FilesList as ListBox;

        public override string ItemName(int index)
        {
            return (_listBox.Items[index] as FilesListData).FIleName;
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
        public void THFilesList_DrawItem(object sender, DrawItemEventArgs e)
        {
            //раскраска строк
            //https://stackoverflow.com/questions/2554609/c-sharp-changing-listbox-row-color
            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            e.DrawBackground();

            int index = e.Index;
            if (index >= 0 && index < ProjectData.THFilesList.Items.Count)
            {
                bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                string text = ProjectData.THFilesList.GetItemName(index);
                Graphics g = e.Graphics;

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                    backgroundBrush = ListBoxItemBackgroundBrushSelected;
                else if ((index % 2) == 0)
                {
                    if (FunctionsTable.IsTableRowsCompleted(ProjectData.THFilesElementsDataset.Tables[e.Index]))
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
                    if (FunctionsTable.IsTableRowsCompleted(ProjectData.THFilesElementsDataset.Tables[e.Index]))
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
                g.DrawString(text, e.Font, foregroundBrush, ProjectData.THFilesList.GetItemRectangle(index).Location);
            }

            e.DrawFocusRectangle();
        }
    }
}

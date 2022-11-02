namespace TranslationHelper.Functions.FilesListControl.FilesListControlItem
{
    class FilesListControlItemListItem : FilesListControlItemBase
    {
        readonly FilesListItem filesListItem;

        public FilesListControlItemListItem()
        {
            filesListItem = new FilesListItem();
        }

        public override object GetItem()
        {
            return filesListItem.FileName;
        }

        public override void SetItem(object item)
        {
            filesListItem.FileName = (string)item;
        }
    }

    public class FilesListItem
    {
        public int Order;
        public string FileName;
    }
}

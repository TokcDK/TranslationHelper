using System.Windows.Forms;

namespace TranslationHelper.Functions.FilesListControl
{
    abstract class FilesListControlBase
    {
        public abstract Control FilesListControl { get; protected set; }

        /// <summary>
        /// add new <paramref name="item"/> to the files list 
        /// </summary>
        /// <returns></returns>
        public abstract void AddItem(object item);

        /// <summary>
        /// item name by selected index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract string GetItemName(int index);

        /// <summary>
        /// Get items count
        /// </summary>
        /// <returns></returns>
        public abstract int GetItemsCount();

        /// <summary>
        /// Get selected items count
        /// </summary>
        /// <returns></returns>
        public abstract int GetSelectedItemsCount();

        /// <summary>
        /// get all selected items collection
        /// </summary>
        /// <returns></returns>
        public abstract object[] GetSelectedItems();

        /// <summary>
        /// Get last selected index
        /// </summary>
        /// <returns></returns>
        public abstract int GetSelectedIndex();

        /// <summary>
        /// Get last selected index
        /// </summary>
        /// <returns></returns>
        public abstract int[] GetSelectedIndexes();

        /// <summary>
        /// Set last selected index
        /// </summary>
        public abstract void SetSelectedIndex(int index, bool clearSelected = true);


        /// <summary>
        /// [Optional] Set <paramref name="drawMode"/>
        /// </summary>
        /// <param name="drawMode"></param>
        public virtual void SetDrawMode(DrawMode drawMode) { }

        /// <summary>
        /// Is focused the control
        /// </summary>
        public abstract bool Focused { get; }
    }
}

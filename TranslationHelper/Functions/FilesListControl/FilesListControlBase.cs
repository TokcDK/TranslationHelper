using System.Windows.Forms;

namespace TranslationHelper.Functions.FilesListControl
{
    abstract class FilesListControlBase
    {
        public abstract object FilesListControl { get; protected set; }

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
        /// Get last selected index
        /// </summary>
        /// <returns></returns>
        public abstract int GetSelectedIndex();

        /// <summary>
        /// Set last selected index
        /// </summary>
        public abstract void SetSelectedIndex(int index);


        /// <summary>
        /// [Optional] Set <paramref name="drawMode"/>
        /// </summary>
        /// <param name="drawMode"></param>
        public virtual void SetDrawMode(DrawMode drawMode) { }
    }
}

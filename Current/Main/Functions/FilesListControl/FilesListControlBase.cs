using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Names of the selected entries, one per line, or an empty string when nothing is selected.
        /// <para>
        /// Composed here from the abstract members rather than left to each caller, so every list —
        /// this project's and every future one — answers the question the same way. It is the list's
        /// own operation: the entries it holds are its, and a caller that had to reach for a list box
        /// to ask would be reading a list it may not own.
        /// </para>
        /// </summary>
        public string GetSelectedItemNames()
        {
            if (GetItemsCount() == 0 || GetSelectedIndex() == -1) return string.Empty;

            var indexes = GetSelectedIndexes();
            var names = new List<string>(indexes.Length);
            foreach (var index in indexes)
            {
                names.Add(GetItemName(index));
            }

            return string.Join(Environment.NewLine, names);
        }
    }
}

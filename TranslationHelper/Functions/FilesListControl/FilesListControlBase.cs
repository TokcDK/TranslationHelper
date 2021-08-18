using System.Windows.Forms;

namespace TranslationHelper.Functions.FilesListControl
{
    abstract class FilesListControlBase
    {
        /// <summary>
        /// item name by selecte index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract string ItemName(int index);
    }
}

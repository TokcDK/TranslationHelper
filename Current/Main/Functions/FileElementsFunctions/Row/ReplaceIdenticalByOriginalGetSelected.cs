using System.Collections.Generic;
using System.Windows.Documents;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// First get all non empty translations into the list 
    /// and then select all using <see cref="ReplaceIdenticalByOriginalALL"/>
    /// </summary>
    class ReplaceIdenticalByOriginalGetSelected : RowBase
    {
        // List to hold the list of items to replace 
        readonly HashSet<string> _listToReplace = new HashSet<string>();

        public ReplaceIdenticalByOriginalGetSelected()
        {
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            if(_listToReplace.Contains(rowData.Translation)) return false;

            try
            {
                _listToReplace.Add(rowData.Translation);
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected async override void ActionsFinalize()
        {
            base.ActionsFinalize();

            // run replacement by the list for all
            await new ReplaceIdenticalByOriginalALL(_listToReplace).AllT();
        }
    }
}

using System.Collections.Generic;
using System.Windows.Documents;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Replace all identical translations with original text using the list
    /// </summary>
    class ReplaceIdenticalByOriginalALL : RowBase
    {
        // List to hold the list of items to replace 
        readonly HashSet<string> _listToReplace = new HashSet<string>();
        readonly bool _isActive = false;

        public ReplaceIdenticalByOriginalALL(HashSet<string> listToReplace)
        {
            _listToReplace = listToReplace;
            _isActive = _listToReplace.Count > 0;
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            if(!_isActive) return false;

            if(!_listToReplace.Contains(rowData.Translation)) return false;

            try
            {
                rowData.Translation = rowData.Original;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

using System.Collections.Generic;
using static TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslateNew;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    public class RowTranslationInfo
    {
        /// <summary>
        /// translation for original
        /// </summary>
        public string Translation { get; set; }

        /// <summary>
        /// table number / row data
        /// </summary>
        public Dictionary<int, List<TranslatingRowData>> TableRowNumbers = new Dictionary<int, List<TranslatingRowData>>();
    }
}

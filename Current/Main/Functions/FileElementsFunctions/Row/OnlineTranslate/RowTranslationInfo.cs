using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    partial class OnlineTranslateNew
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
}

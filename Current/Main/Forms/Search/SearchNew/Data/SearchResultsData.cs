using System.Collections.Generic;
using TranslationHelper.Forms.Search.Data;

namespace TranslationHelper.Forms.Search.SearchNew.Data
{
    public class SearchResultsData
    {
        public List<FoundRowData> FoundRows { get; set; } = new List<FoundRowData>();

        public List<ISearchCondition> SearchConditions { get; set; } = new List<ISearchCondition>();
    }
}

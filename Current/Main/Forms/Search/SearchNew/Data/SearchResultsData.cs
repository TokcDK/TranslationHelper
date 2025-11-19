using System.Collections.Generic;
using TranslationHelper.Forms.Search.Data;

namespace TranslationHelper.Forms.Search.SearchNew.Data
{
    public class SearchResultsData
    {
        public List<FoundRowData> FoundRows { get; set; } = new List<FoundRowData>();

        public List<ISearchCondition> searchConditions { get; set; } = new List<ISearchCondition>();
    }
}

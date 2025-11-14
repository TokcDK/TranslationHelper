using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Forms.Search
{
    internal class SearchHelper
    {
        internal List<SearchCondition> SearchConditions { get; set; } = new List<SearchCondition>();

        internal SearchHelper(string[] columns)
        {
            SearchConditions.Add(new SearchCondition(columns));
        }
    }

    internal class SearchCondition
    {
        public SearchCondition(string[] columns)
        {
            Options = new SearchOptions(columns);
            Replacers.Add(new Replacer());
        }

        internal string FindWhat { get; set; } = "";

        internal SearchOptions Options { get; set; }

        internal List<Replacer> Replacers { get; set; } = new List<Replacer>();
    }

    internal class SearchOptions
    {
        public SearchOptions(string[] columns)
        {
            SearchColumns = columns;
            if(SearchColumns.Length > 0)
            {
                SearchColumn = SearchColumns[0];
            }
        }

        internal string[] SearchColumns { get; }
        internal string SearchColumn { get; set; }

        internal bool IsCaseSensitive { get; set; } = false;

        internal bool IsRegex { get; set; } = false;
    }

    internal class Replacer
    {
        internal string ReplaceWhat { get; set; }
        internal string ReplaceWith { get; set; }
    }
}

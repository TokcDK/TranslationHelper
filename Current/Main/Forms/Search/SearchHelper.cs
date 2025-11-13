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

        internal SearchHelper()
        {
        }
    }

    internal class SearchCondition
    {
        internal string FindWhat { get; set; } = "";

        internal SearchOptions Options { get; set; } = new SearchOptions();

        internal List<Replacer> Replacers { get; set; } = new List<Replacer>();
    }

    internal class SearchOptions
    {
        internal List<string> SearchColumn { get; set; }

        internal bool IsCaseSensitive { get; set; } = false;

        internal bool IsRegex { get; set; } = false;
    }

    internal class Replacer
    {
        internal string ReplaceWhat { get; set; }
        internal string ReplaceWith { get; set; }
    }
}

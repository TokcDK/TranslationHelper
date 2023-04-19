using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class ContainsNonRomaji : ISearchIssueChecker
    {
        public ContainsNonRomaji()
        {
        }

        public string Description => "Translation contains non romaji symbols";

        public bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            return AppSettings.SearchRowIssueOptionsCheckNonRomaji 
                && Regex.IsMatch(data.Translation, @"[^\x00-\x7F]+\ *(?:[^\x00-\x7F]|\ )*");
        }
    }
}

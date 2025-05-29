using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class ContainsNonRomaji : SearchIssueCheckerBase
    {
        public override string Description => "Translation contains non romaji symbols";

        public override bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            if (!AppSettings.SearchRowIssueOptionsCheckNonRomaji) return false;

            return Regex.IsMatch(data.Translation, @"[^\x00-\x7F]+\ *(?:[^\x00-\x7F]|\ )*");
        }
    }
}

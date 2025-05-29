using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class CheckAnyLineTranslatable : SearchIssueCheckerBase
    {
        public override string Description => "Check if multiline and one of line equal to original and valide for translation";

        public override bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            return AppSettings.SearchRowIssueOptionsCheckAnyLineTranslatable
                && data.Original.HasAnyTranslationLineValidAndEqualSameOrigLine(data.Translation);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class CheckAnyLineLngerOfMaxLength : SearchIssueCheckerBase
    {
        public override string Description => "Check any line longer Of max length";

        public override bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            //------------------------------
            //если длина любой из строк длиннее лимита
            return FunctionsString.GetLongestLineLength(data.Translation) > AppSettings.THOptionLineCharLimit;

            //------------------------------
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class ProjectSpecificIssues : SearchIssueCheckerBase
    {
        public override string Description => "Project specific issues";

        public override bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            return AppSettings.SearchRowIssueOptionsCheckProjectSpecific 
                && Project.CheckForRowIssue(data.Row);
        }
    }
}

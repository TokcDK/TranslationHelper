using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class ProjectSpecificIssues : ISearchIssueChecker
    {
        public string Description => "Project specific issues";

        public bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            return AppSettings.SearchRowIssueOptionsCheckProjectSpecific 
                && AppData.CurrentProject.CheckForRowIssue(data.Row);
        }
    }
}

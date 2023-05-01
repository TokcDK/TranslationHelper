using System.Data;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal interface ISearchIssueChecker
    {
        string Description { get; }
        bool IsHaveTheIssue(SearchIssueCheckerData data);
    }

    internal class SearchIssueCheckerData
    {
        readonly bool origWasSet = false;
        readonly string original;
        internal string Original { get => origWasSet ? original : Row.Field<string>(AppData.CurrentProject.OriginalColumnIndex); }

        readonly bool transWasSet = false;
        readonly string translation;
        internal string Translation { get => transWasSet ? translation : Row.Field<string>(AppData.CurrentProject.OriginalColumnIndex); }

        internal DataRow Row;

        public SearchIssueCheckerData(DataRow row, string original = null, string translation = null)
        {
            Row = row;
            if (original != null)
            {
                origWasSet = true;
                this.original = original;
            }
            if (translation != null)
            {
                transWasSet = true;
                this.translation = translation;
            }
        }
    }
}

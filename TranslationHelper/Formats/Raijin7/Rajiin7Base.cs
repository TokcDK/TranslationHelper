using System.Linq;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    abstract class Rajiin7Base : FormatBase
    {
        protected Rajiin7Base(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.GetEncoding(932);
        }

        protected bool IsValid(string v)
        {
            return IsValidString(v) && thDataWork.TablesLinesDict.ContainsKey(v) && v != thDataWork.TablesLinesDict[v];
        }

        protected void SetValue(params int[] nums)
        {
            var Values = ParseData.line.Split(',');

            var set = false;
            var numbers = nums[0] < 999 ? nums : Enumerable.Range(0, Values.Length);
            foreach (var num in numbers)
            {
                if (thDataWork.OpenFileMode)
                {
                    AddRowData(Values[num], "", true);
                }
                else if (IsValid(Values[num]))
                {
                    Values[num] = FixInvalidSymbols(thDataWork.TablesLinesDict[Values[num]]);
                    set = true;
                    ParseData.Ret = true;
                }
            }

            if (set)
            {
                ParseData.line = string.Join(",", Values);
            }
        }
    }
}

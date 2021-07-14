using System.Linq;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    abstract class Rajiin7Base : FormatBase
    {
        protected Rajiin7Base() : base()
        {
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.GetEncoding(932);
        }

        protected bool IsValid(string v, ref string trans)
        {
            trans = v;
            return IsValidString(v) && SetTranslation(ref trans) && v != trans;
        }

        protected void SetValue(params int[] nums)
        {
            var Values = ParseData.line.Split(',');

            var set = false;
            var numbers = nums[0] < 999 ? nums : Enumerable.Range(0, Values.Length);
            foreach (var num in numbers)
            {
                var trans="";
                if (ProjectData.OpenFileMode)
                {
                    AddRowData(Values[num], "", true);
                }
                else if (IsValid(Values[num], ref trans))
                {
                    Values[num] = FixInvalidSymbols(trans);
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

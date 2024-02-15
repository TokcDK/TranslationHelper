using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    abstract class Rajiin7Base : FormatStringBase
    {
        protected Rajiin7Base()
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
            var values = ParseData.Line.Split(',');

            var set = false;
            var numbers = nums[0] < 999 ? nums : Enumerable.Range(0, values.Length);
            foreach (var num in numbers)
            {
                string t = values[num];
                string emotionMarker = "";
                if (t.EndsWith("]"))
                {
                    int ind = t.Length - 3;
                    if (t[ind] == '[')
                    {
                        emotionMarker = t.Substring(ind);
                        t = t.Remove(ind);
                    }
                }

                if (AddRowData(ref t, "") && SaveFileMode)
                {
                    values[num] = FixInvalidSymbols(t) + emotionMarker;
                    set = true;
                }
            }

            if (set) ParseData.Line = string.Join(",", values);
        }
    }
}

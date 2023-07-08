using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixBrokenNameVar2 : HardFixesBase
    {
        public FixBrokenNameVar2()
        {
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            var translation = rowData.Translation;
            if (Regex.IsMatch(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]"))
            {
                var original = rowData.Original;
                if (Regex.IsMatch(original, @"\\\\N\[[0-9]{1,3}\]"))
                {
                    rowData.Translation = Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\\N[$2]");
                    return true;
                }
                else
                {
                    var mc = Regex.Matches(original, @"\\\\([A-Za-z])\[[0-9]{1,3}\]");
                    if (mc.Count == 1)
                    {
                        rowData.Translation = Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\" + mc[0].Value);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

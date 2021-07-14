using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixBrokenNameVar2 : HardFixesBase
    {
        public FixBrokenNameVar2(ProjectData projectData) : base(projectData)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";
            if (Regex.IsMatch(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]"))
            {
                var original = SelectedRow[ColumnIndexOriginal] as string;
                if (Regex.IsMatch(original, @"\\\\N\[[0-9]{1,3}\]"))
                {
                    SelectedRow[ColumnIndexTranslation] = Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\\N[$2]");
                    return true;
                }
                else
                {
                    var mc = Regex.Matches(original, @"\\\\([A-Za-z])\[[0-9]{1,3}\]");
                    if (mc.Count == 1)
                    {
                        SelectedRow[ColumnIndexTranslation] = Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\" + mc[0].Value);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

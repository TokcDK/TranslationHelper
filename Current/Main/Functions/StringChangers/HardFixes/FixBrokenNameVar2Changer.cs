using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixBrokenNameVar2Changer : StringChangerBase
    {
        public FixBrokenNameVar2Changer()
        {
        }
        internal override string Description => $"{nameof(FixBrokenNameVar2Changer)}";


        internal override string Change(string inputString, object extraData)
        {
            var translation = inputString;
            if (Regex.IsMatch(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]"))
            {
                var original = extraData as string;
                if (Regex.IsMatch(original, @"\\\\N\[[0-9]{1,3}\]"))
                {
                    return Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\\N[$2]");

                }
                else
                {
                    var mc = Regex.Matches(original, @"\\\\([A-Za-z])\[[0-9]{1,3}\]");
                    if (mc.Count == 1)
                    {
                        return Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\" + mc[0].Value);

                    }
                }
            }

            return inputString;
        }
    }
}

using System.Linq;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixEnjpQuoteOnStringStart1StLine : HardFixesBase
    {
        public FixEnjpQuoteOnStringStart1StLine()
        {
        }

        //fix
        /* 「一攫千金を狙ってスロットに挑戦する？
　\\C[16]１プレイ \\C[0]\\V[7] \\C[16]\\G\\C[0]よ。

"Challenge the slots for a quick getaway?
　\\C[16]1 play \\C[0]\\V[7]\\C[16]\\G\\C[0] */
        protected override bool Apply()
        {
            var transValue = Translation;

            string[] quotes; ;
            if (transValue.Length == 0 || !(quotes = new string[4] { "\"", "``", "`", "“" }).Contains(transValue.Substring(0, 1)))
                return false;

            bool oStartsJp;
            bool oEndsJp;
            bool tStartsEn;
            bool tStartsJp;
            bool tEndsEn;
            bool tEndsJp;

            var ret = false;
            for (int i = 0; i < quotes.Length; i++)
            {
                var origValue = Original;
                oStartsJp = origValue.StartsWith("「");
                oEndsJp = origValue.EndsWith("」");
                tStartsEn = transValue.StartsWith(quotes[i]);
                tStartsJp = transValue.StartsWith("「");
                tEndsEn = transValue.EndsWith(quotes[i]);
                tEndsJp = transValue.EndsWith("」");
                if (transValue.Length > (quotes[i].Length * 2) && oStartsJp && !tStartsEn && !tStartsJp && oEndsJp && tEndsEn && !tEndsJp)
                {
                    transValue = "「" + transValue.Substring(quotes[i].Length, transValue.Length - quotes[i].Length) + "」";
                    ret = true;
                }
                else if (transValue.Length > quotes[i].Length && oEndsJp && tEndsEn && !tEndsJp)
                {
                    transValue = transValue.Substring(0, transValue.Length - quotes[i].Length) + "」";
                    ret = true;
                }
                else if (transValue.Length > quotes[i].Length && oStartsJp && tStartsEn && !tStartsJp)
                {
                    transValue = "「" + transValue.Substring(quotes[i].Length);
                    ret = true;
                }
            }

            if (ret)
                Translation = transValue;

            return ret;
        }
    }
}

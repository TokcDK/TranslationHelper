using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixEnjpQuoteOnStringStart1StLineChanger : StringChangerBase
    {
        public FixEnjpQuoteOnStringStart1StLineChanger()
        {
        }
        internal override string Description => $"{nameof(FixEnjpQuoteOnStringStart1StLineChanger)}";


        //fix
        /* 「一攫千金を狙ってスロットに挑戦する？
　\\C[16]１プレイ \\C[0]\\V[7] \\C[16]\\G\\C[0]よ。

"Challenge the slots for a quick getaway?
　\\C[16]1 play \\C[0]\\V[7]\\C[16]\\G\\C[0] */
        internal override string Change(string inputString, object extraData)
        {
            var transValue = inputString;

            string[] quotes; ;
            if (transValue.Length == 0 || !(quotes = new string[4] { "\"", "``", "`", "“" }).Contains(transValue.Substring(0, 1)))
                return inputString;

            bool oStartsJp;
            bool oEndsJp;
            bool tStartsEn;
            bool tStartsJp;
            bool tEndsEn;
            bool tEndsJp;

            var ret = false;
            for (int i = 0; i < quotes.Length; i++)
            {
                var origValue = extraData as string;
                oStartsJp = origValue.StartsWith("「");
                oEndsJp = origValue.EndsWith("」");
                tStartsEn = transValue.StartsWith(quotes[i]);
                tStartsJp = transValue.StartsWith("「");
                tEndsEn = transValue.EndsWith(quotes[i]);
                tEndsJp = transValue.EndsWith("」");
                if (transValue.Length > quotes[i].Length * 2 && oStartsJp && !tStartsEn && !tStartsJp && oEndsJp && tEndsEn && !tEndsJp)
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

            if (ret) return transValue;

            return inputString;
        }
    }
}

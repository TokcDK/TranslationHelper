using System.Linq;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixENJPQuoteOnStringStart1stLine : HardFixesBase
    {
        public FixENJPQuoteOnStringStart1stLine(THDataWork thDataWork) : base(thDataWork)
        {
        }

        //fix
        /* 「一攫千金を狙ってスロットに挑戦する？
　\\C[16]１プレイ \\C[0]\\V[7] \\C[16]\\G\\C[0]よ。

"Challenge the slots for a quick getaway?
　\\C[16]1 play \\C[0]\\V[7]\\C[16]\\G\\C[0] */
        protected override bool Apply()
        {
            var transValue = SelectedRow[ColumnIndexTranslation] + "";

            string[] quotes; ;
            if (transValue.Length == 0 || !(quotes = new string[4] { "\"", "``", "`", "“" }).Contains(transValue.Substring(0, 1)))
                return false;

            bool oStartsJP;
            bool oEndsJP;
            bool tStartsEN;
            bool tStartsJP;
            bool tEndsEN;
            bool tEndsJP;

            var ret = false;
            for (int i = 0; i < quotes.Length; i++)
            {
                var origValue = SelectedRow[ColumnIndexOriginal] as string;
                oStartsJP = origValue.StartsWith("「");
                oEndsJP = origValue.EndsWith("」");
                tStartsEN = transValue.StartsWith(quotes[i]);
                tStartsJP = transValue.StartsWith("「");
                tEndsEN = transValue.EndsWith(quotes[i]);
                tEndsJP = transValue.EndsWith("」");
                if (transValue.Length > (quotes[i].Length * 2) && oStartsJP && !tStartsEN && !tStartsJP && oEndsJP && tEndsEN && !tEndsJP)
                {
                    transValue = "「" + transValue.Substring(quotes[i].Length, transValue.Length - quotes[i].Length) + "」";
                    ret = true;
                }
                else if (transValue.Length > quotes[i].Length && oEndsJP && tEndsEN && !tEndsJP)
                {
                    transValue = transValue.Substring(0, transValue.Length - quotes[i].Length) + "」";
                    ret = true;
                }
                else if (transValue.Length > quotes[i].Length && oStartsJP && tStartsEN && !tStartsJP)
                {
                    transValue = "「" + transValue.Substring(quotes[i].Length);
                    ret = true;
                }
            }

            if (ret)
                SelectedRow[ColumnIndexTranslation] = transValue;

            return ret;
        }
    }
}

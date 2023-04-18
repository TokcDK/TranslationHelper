using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForRpgmAkerQuotationInSomeStrings2 : HardFixesBase
    {
        public FixForRpgmAkerQuotationInSomeStrings2()
        {
        }

        /////////////////////////////////
        /* 
『先日、あなたが施した解呪の作用のようですね。
　古代種が相手なら意思疎通が可能になったようです』

"It sounds like the curse you did the other day.
　It seems that communication was possible if the ancient species was the opponent. '


『不安ならあなたもあの子を見守って下さい。
　私がいくら注意を払おうと、呪いの付与は稀に
　私の意識を超えて発現する』

"If you are uneasy, please watch over him.
　No matter how much attention I pay, curse grants are rare
　Expresses beyond my consciousness" 
*/
        protected override bool Apply()
        {
            var ret = false;
            var translation = Translation;

            string[][] quotes = new string[][]
            {
                 new string[] {"『","』"},
                 new string[] { "「", "」"}
            };

            var original = Original;
            foreach (var quote in quotes)
            {
                if (original.TrimStart().StartsWith(quote[0]) && original.TrimEnd().EndsWith(quote[1]))
                {
                    string translationTrimStart = translation.TrimStart();
                    if (!translationTrimStart.StartsWith(quote[0]))
                    {
                        string translationOnlyWhatWasTrimmedOnStart = translation.Replace(translationTrimStart, string.Empty);
                        if (translationTrimStart.StartsWith("''"))
                        {
                            translation = translationOnlyWhatWasTrimmedOnStart + quote[0] + translationTrimStart.Remove(0, 2);
                            ret = true;
                        }
                        else if (translationTrimStart.StartsWith("'") || translationTrimStart.StartsWith("“") || translationTrimStart.StartsWith("\""))
                        {
                            translation = translationOnlyWhatWasTrimmedOnStart + quote[0] + translationTrimStart.Remove(0, 1);
                            ret = true;
                        }
                        else
                        {
                            translation = translationOnlyWhatWasTrimmedOnStart + quote[0] + translationTrimStart;
                            ret = true;
                        }
                    }
                    string translationTrimEnd = translation.TrimEnd();
                    if (!translationTrimEnd.EndsWith(quote[1]))
                    {
                        string translationOnlyWhatWasTrimmedOnEnd = translation.Replace(translationTrimEnd, string.Empty);
                        if (translationTrimEnd.EndsWith("''"))
                        {
                            translation = translationTrimEnd.Remove(translationTrimEnd.Length - 2, 2) + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
                            ret = true;
                        }
                        else if (translationTrimEnd.EndsWith("'") || translationTrimEnd.EndsWith("\"") || translationTrimEnd.EndsWith("“"))
                        {
                            translation = translationTrimEnd.Remove(translationTrimEnd.Length - 1, 1) + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
                            ret = true;
                        }
                        else
                        {
                            translation = translationTrimEnd.Remove(translationTrimEnd.Length - 1, 1) + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
                            ret = true;
                        }
                    }

                    //extra corrections
                    //translation = Regex.Replace(translation, "^" + quote[0] + "''", quote[0]);
                    //translation = Regex.Replace(translation, "^" + quote[0] + "'", quote[0]);
                    //translation = Regex.Replace(translation, "^" + quote[0] + "“", quote[0]);
                    //translation = Regex.Replace(translation, "^" + quote[0] + "\"", quote[0]);
                    //translation = Regex.Replace(translation, "''" + quote[1] + "$", quote[1]);
                    //translation = Regex.Replace(translation, "'" + quote[1] + "$", quote[1]);
                    //translation = Regex.Replace(translation, "“" + quote[1] + "$", quote[1]);
                    //translation = Regex.Replace(translation, "\"" + quote[1] + "$", quote[1]);
                }
            }

            if (ret)
                Translation = translation;

            return ret;
        }
    }
}

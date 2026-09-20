using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixForRpgmAkerQuotationInSomeStrings2Changer : StringChangerBase
    {
        public FixForRpgmAkerQuotationInSomeStrings2Changer()
        {
        }
        internal override string Description => $"{nameof(FixForRpgmAkerQuotationInSomeStrings2Changer)}";


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
        internal override string Change(string inputString, object extraData)
        {
            var ret = false;
            var translation = inputString;

            string[][] quotes = new string[][]
            {
                 new string[] {"『","』"},
                 new string[] { "「", "」"}
            };

            var original = extraData as string;
            if (original == null) return inputString;

            foreach (var quote in quotes)
            {
                if (original.TrimStart().StartsWith(quote[0]) && original.TrimEnd().EndsWith(quote[1]))
                {
                    string translationTrimStart = translation.TrimStart();
                    if (!translationTrimStart.StartsWith(quote[0]))
                    {
                        //The trimmed off part is exactly the prefix which TrimStart removed. Substring is
                        //used instead of Replace because Replace throws ArgumentException for an empty
                        //oldValue, which is what an empty or whitespace only translation produced.
                        string translationOnlyWhatWasTrimmedOnStart = translation.Substring(0, translation.Length - translationTrimStart.Length);
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
                        //The trimmed off part is exactly the suffix which TrimEnd removed; see the note above.
                        string translationOnlyWhatWasTrimmedOnEnd = translation.Substring(translationTrimEnd.Length);
                        if (translationTrimEnd.EndsWith("''"))
                        {
                            translation = translationTrimEnd.Remove(translationTrimEnd.Length - 2, 2) + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
                            ret = true;
                        }
                        else
                        {
                            //ends with a single quote or with anything else: the last character is replaced
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

            if (ret) return translation;

            return inputString;
        }
    }
}

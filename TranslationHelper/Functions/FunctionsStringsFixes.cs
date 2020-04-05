using System;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsStringFixes
    {
        internal static string ApplyHardFixes(string original, string translation)
        {
            if (string.IsNullOrWhiteSpace(translation) || original == translation || string.IsNullOrWhiteSpace(original))
            {
                return translation;
            }

            //Fix 1
            /////////////////////////////////	
            //"
            //「……くっ……。
            //　いったい何をしてるんだ。わたしは……。"
            /////////////////////////////////	
            //" 
            //“…………….
            //　What are you doing? I……."
            /////////////////////////////////	
            translation = FixENJPQuoteOnStringStart2ndLine(original, translation);

            //fix
            /* 「一攫千金を狙ってスロットに挑戦する？
　\\C[16]１プレイ \\C[0]\\V[7] \\C[16]\\G\\C[0]よ。

"Challenge the slots for a quick getaway?
　\\C[16]1 play \\C[0]\\V[7]\\C[16]\\G\\C[0] */
            translation = FixENJPQuoteOnStringStart1stLine(original, translation);

            /////////////////////////////////
            //Fix 2 Quotation
            translation = FixForRPGMAkerQuotationInSomeStrings(original, translation);

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
            translation = FixForRPGMAkerQuotationInSomeStrings2(original, translation);

            // fix
            //orig = \\N[1]いったい何をしてるんだ
            //trans = \\NBlablabla[1]blabla
            translation = FixBrokenNameVar(translation);

            //\\N[3] in some strings was broken to \\3[3]
            translation = FixBrokenNameVar2(original, translation);

            //Lialua temp fix
            //translation = LuaLiaFix(original, translation);

            return translation;
        }

        internal static string LuaLiaFix(string original, string translation)
        {
            bool Lia;
            if (original.StartsWith("ルア") && ((Lia = translation.StartsWith("Lia")) || translation.StartsWith("Lila")))
            {
                translation = "Lua" + translation.Remove(0, Lia ? 3 : 4);
            }

            return translation;
        }

        internal static string FixForRPGMAkerQuotationInSomeStrings2(string original, string translation)
        {            /////////////////////////////////
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
            string[][] quotes = new string[][]
            {
                 new string[] {"『","』"},
                 new string[] { "「", "」"}
            };

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
                        }
                        else if (translationTrimStart.StartsWith("'") || translationTrimStart.StartsWith("“") || translationTrimStart.StartsWith("\""))
                        {
                            translation = translationOnlyWhatWasTrimmedOnStart + quote[0] + translationTrimStart.Remove(0, 1);
                        }
                        else
                        {
                            translation = translationOnlyWhatWasTrimmedOnStart + quote[0] + translationTrimStart;
                        }
                    }
                    string translationTrimEnd = translation.TrimEnd();
                    if (!translationTrimEnd.EndsWith(quote[1]))
                    {
                        string translationOnlyWhatWasTrimmedOnEnd = translation.Replace(translationTrimEnd, string.Empty);
                        if (translationTrimEnd.EndsWith("''"))
                        {
                            translation = translationTrimEnd.Remove(translationTrimEnd.Length - 2, 2) + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
                        }
                        else if (translationTrimEnd.EndsWith("'") || translationTrimEnd.EndsWith("\"") || translationTrimEnd.EndsWith("“"))
                        {
                            translation = translationTrimEnd.Remove(translationTrimEnd.Length - 1, 1) + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
                        }
                        else
                        {
                            translation = translationTrimEnd + quote[1] + translationOnlyWhatWasTrimmedOnEnd;
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

            return translation;
        }

        internal static string FixENJPQuoteOnStringStart2ndLine(string OriginalValue, string TranslationValue)
        {
            try
            {
                if (FunctionsString.IsMultiline(OriginalValue))
                {
                    string[] valueArray = OriginalValue.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    string origSecondLine = valueArray[1];
                    for (int i = 1; i < valueArray.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(valueArray[i]))
                        {
                            origSecondLine = valueArray[i];
                            break;
                        }
                    }

                    bool quote1 = false;
                    bool quote2 = false;
                    if (/*OriginalValue.StartsWith("\"") &&*/ (quote1 = origSecondLine.StartsWith("「")) || (quote2 = origSecondLine.StartsWith("『")))
                    {
                        bool endsWith = false;
                        //if (!TranslationValue.StartsWith("\""))
                        //{
                        //    return TranslationValue;
                        //}

                        if (FunctionsString.IsMultiline(TranslationValue))
                        {
                            string quoteString;
                            bool StartsWithJpQuote1 = false;
                            bool StartsWithJpQuote2 = false;
                            string secondline;
                            int secondlineIndex = 1;
                            try
                            {
                                string[] valueArrayTrans = TranslationValue.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                                secondline = valueArrayTrans[1];
                                for (int i = 1; i < valueArrayTrans.Length; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(valueArrayTrans[i]))
                                    {
                                        secondline = valueArrayTrans[i];
                                        secondlineIndex = i;
                                        break;
                                    }
                                };
                            }
                            catch
                            {
                                return TranslationValue;
                            }

                            string StartQuoteStringEN = string.Empty;
                            string EndQuoteStringEN = string.Empty;

                            if (secondline.StartsWith("''"))
                            {
                                StartQuoteStringEN = "''";
                            }
                            else if (secondline.StartsWith("'"))
                            {
                                StartQuoteStringEN = "'";
                            }
                            else if (secondline.StartsWith("“"))
                            {
                                StartQuoteStringEN = "“";
                            }
                            else if (secondline.StartsWith("\""))
                            {
                                StartQuoteStringEN = "\"";
                            }

                            if (StartQuoteStringEN.Length > 0 || !((quote1 && (StartsWithJpQuote1 = secondline.StartsWith("「"))) || (quote2 && (StartsWithJpQuote2 = secondline.StartsWith("『")))))
                            {
                                if (StartsWithJpQuote1 || StartsWithJpQuote2)
                                {
                                    return TranslationValue;
                                }
                                if (quote1)
                                {
                                    quoteString = "「";
                                }
                                else if (quote2)
                                {
                                    quoteString = "『";
                                }
                                else
                                {
                                    return TranslationValue;
                                }

                                if (TranslationValue.EndsWith("''"))
                                {
                                    EndQuoteStringEN = "''";
                                }
                                else if (TranslationValue.EndsWith("'"))
                                {
                                    EndQuoteStringEN = "'";
                                }
                                else if (TranslationValue.EndsWith("“"))
                                {
                                    EndQuoteStringEN = "“";
                                }
                                else if (TranslationValue.EndsWith("\""))
                                {
                                    EndQuoteStringEN = "\"";
                                }

                                int EndQuoteStringENLength = EndQuoteStringEN.Length;
                                endsWith = EndQuoteStringENLength > 0;

                                string resultString = string.Empty;
                                int ind = 0;
                                foreach (string line in TranslationValue.SplitToLines())
                                {

                                    //new line for multiline
                                    if (ind > 0)
                                    {
                                        resultString += Environment.NewLine;
                                    }

                                    if (ind != secondlineIndex)
                                    {
                                        resultString += line;
                                    }
                                    else
                                    {
                                        int lineLength = line.Length;
                                        int StartQuoteStringENLength = StartQuoteStringEN.Length;
                                        if (lineLength > 1 && StartQuoteStringENLength > 0 && line.StartsWith(StartQuoteStringEN))
                                        {
                                            resultString += quoteString + line.Remove(0, StartQuoteStringENLength);
                                        }
                                        else if (lineLength == 0 || (lineLength == 1 && StartQuoteStringENLength > 0 && line == StartQuoteStringEN))
                                        {
                                            resultString += quoteString;
                                        }
                                        else if (lineLength > 0)
                                        {
                                            resultString += quoteString + line;
                                        }
                                        else
                                        {
                                            resultString += line;
                                        }
                                    }
                                    ind++;
                                }

                                string EndQuoteString = (quote1 ? "」" : "』");
                                resultString = resultString.TrimEnd();
                                if (OriginalValue.EndsWith(EndQuoteString) && !resultString.EndsWith(EndQuoteString))
                                {
                                    resultString = (endsWith ? resultString.Remove(resultString.Length - EndQuoteStringENLength, EndQuoteStringENLength) : resultString) + EndQuoteString;
                                }

                                return resultString;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return TranslationValue;
        }

        internal static string FixENJPQuoteOnStringStart1stLine(string origValue, string transValue)
        {
            string[] quotes = new string[4] { "\"", "``", "`", "“" };

            if (transValue.Length > 0 && !quotes.Contains(transValue.Substring(0, 1)))
                return transValue;

            bool oStartsJP;
            bool oEndsJP;
            bool tStartsEN;
            bool tStartsJP;
            bool tEndsEN;
            bool tEndsJP;

            for (int i = 0; i < quotes.Length; i++)
            {
                oStartsJP = origValue.StartsWith("「");
                oEndsJP = origValue.EndsWith("」");
                tStartsEN = transValue.StartsWith(quotes[i]);
                tStartsJP = transValue.StartsWith("「");
                tEndsEN = transValue.EndsWith(quotes[i]);
                tEndsJP = transValue.EndsWith("」");
                if (transValue.Length > (quotes[i].Length * 2) && oStartsJP && !tStartsEN && !tStartsJP && oEndsJP && tEndsEN && !tEndsJP)
                {
                    return "「" + transValue.Substring(quotes[i].Length, transValue.Length - quotes[i].Length) + "」";
                }
                else if (transValue.Length > quotes[i].Length && oEndsJP && tEndsEN && !tEndsJP)
                {
                    return transValue.Substring(0, transValue.Length - quotes[i].Length) + "」";
                }
                else if (transValue.Length > quotes[i].Length && oStartsJP && tStartsEN && !tStartsJP)
                {
                    return "「" + transValue.Substring(quotes[i].Length);
                }
            }

            return transValue;
        }

        internal static string FixForRPGMAkerQuotationInSomeStrings(string origValue, string transValue)
        {
            string NewtransValue = transValue;

            //в оригинале " на начале и конце, а в переводе есть также " в середине, что может быть воспринято игрой как ошибка
            //также фикс, когда в оригинале кавычки в начале и конце, а в переводе нет в начале или конце
            bool cvalueStartsWith = NewtransValue.StartsWith("\"");
            bool cvalueEndsWith = NewtransValue.EndsWith("\"");
            if (
                 //если оригинал начинается и кончается на ", а в переводе " отсутствует на начале или конце
                 (origValue.StartsWith("\"") && origValue.EndsWith("\"") && (!cvalueStartsWith || !cvalueEndsWith))
                 ||
                 //если перевод начинается и кончается на " и также " есть в где-то середине и количество кавычек не равно
                 (cvalueStartsWith && cvalueEndsWith && NewtransValue.Length > 2
                 && FunctionsString.IsStringAContainsStringB(NewtransValue.Remove(NewtransValue.Length - 1, 1).Remove(0, 1), "\"")
                 //это, чтобы только когда количество кавычек не равно количеству в оригинале
                 && FunctionsString.GetCountOfTheSymbolInStringAandBIsEqual(origValue, NewtransValue, "\"", "\"")))
            {
                NewtransValue = "\"" +
                    NewtransValue
                    .Replace("\"", string.Empty)
                    + "\""
                    ;
            }
            else
            {
                //rpgmaker mv string will broke script if starts\ends with "'" and contains another "'" in middle
                //в оригинале  ' на начале и конце, а в переводе есть также ' в середине, что может быть воспринято игрой как ошибка, по крайней мере в MV
                cvalueStartsWith = NewtransValue.StartsWith("'");
                cvalueEndsWith = NewtransValue.EndsWith("'");
                if (
                //если оригинал начинается и кончается на ', а в переводе ' отсутствует на начале или конце
                (origValue.StartsWith("'") && origValue.EndsWith("'") && (!cvalueStartsWith || !cvalueEndsWith))
                ||
                //если перевод начинается и кончается на ' и также ' есть в где-то середине
                (cvalueStartsWith && cvalueEndsWith && NewtransValue.Length > 2
                && FunctionsString.IsStringAContainsStringB(NewtransValue.Remove(NewtransValue.Length - 1, 1).Remove(0, 1), "'")
                 //это, чтобы только когда количество ' не равно количеству в оригинале
                 && FunctionsString.GetCountOfTheSymbolInStringAandBIsEqual(origValue, NewtransValue, "'", "'")))
                {
                    NewtransValue = "'" +
                        NewtransValue
                        .Replace("do n't", "do not")
                        .Replace("don't", "do not")
                        .Replace("n’t", "not")
                        .Replace("'ve", " have")
                        .Replace("I'm", "I am")
                        .Replace("t's", "t is")
                        .Replace("'s", "s")
                        .Replace("'", string.Empty)
                        + "'"
                        ;
                }
            }

            return NewtransValue;
        }

        /// <summary>
        ///fix for kind of values when \\N was not with [#] in line
        ///\\N\\N[\\V[122]]
        ///"\\N[\\V[122]]'s blabla... and [1]' s bla...!
        ///　\\NIt \\Nseems to[2222] be[1]'s blabla...!
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        internal static string FixBrokenNameVar(string translation)
        {
            //вот такой пипец теоритически возможен
            //\\N\\N[\\V[122]]
            //"\\N[\\V[122]]'s blabla... and [1]' s bla...!
            //　\\NIt \\Nseems to[2222] be[1]'s blabla...!

            //выдирание совпадений из перевода
            //var mc1 = Regex.Matches(translation, @"\\\\N\[[0-9]+\]");
            var mc2 = Regex.Matches(translation, @"\\\\N(?=[^\[])"); //catch only \\N without [ after
            var mc3 = Regex.Matches(translation, @"(?<=[^\\][^\\][^A-Z])\[[0-9]+\]"); // match only \\A-Z[0-9+] but catch without \\A-Z before it

            //рабочие переменные
            int max = mc3.Count;//максимум итераций цикла
            int mc2Correction = mc3.Count > mc2.Count ? mc3.Count - mc2.Count : 0;//когда mc2 нашло меньше, чем mc3
            int PositionCorrectionMC3 = 0;//переменная для коррекции номера позиции в стоке, т.к. \\N выдирается и позиция меняется на 3
            int minimalIndex = 9999999; //минимальный индекс, для правильного контроля коррекции позиции
            string newValue = translation;//значение, которое будет редактироваться и возвращено
            for (int i = max - 1; i >= 0; i--)//цикл задается в обратную сторону, т.к. так проще контроллировать смещение позиции
            {
                int mc2i = i - mc2Correction;//задание индекса в коллекции для mc2, т.к. их может быть меньше
                if (mc2i == -1)//если mc2 закончится, выйти из цикла
                {
                    break;
                }

                //если индекс позиции больше последнего минимального, подкорректировать на 3, когда совпадение раньше, коррекция не требуется
                if (mc3[i].Index > minimalIndex)
                {
                    PositionCorrectionMC3 += 3;
                }
                else
                {
                    PositionCorrectionMC3 = 0;
                }

                int mc3PosIndex = mc3[i].Index - PositionCorrectionMC3;//новый индекс с учетом коррекции
                int mc2PosIndex = mc2[mc2i].Index;
                if (mc2PosIndex < 0)//если позиция для mc2 меньше нуля, установить её в ноль и проверить, если там нужное значение, иначе выйти из цикла
                {
                    mc2PosIndex = 0;
                    if (translation.Substring(0, 3) != @"\\N")
                    {
                        break;
                    }
                }

                if (minimalIndex > mc2PosIndex)//задание нового мин. индекса, если старый больше чем теекущая позиция mc2
                {
                    minimalIndex = mc2PosIndex;
                }

                //проверки для измежания ошибок, идти дальше когда позиция mc3 тремя символами ранее не совпадает с mc2, а также не содержит \\ в последних 3х символах перед mc3
                if (mc3PosIndex - 3 > -1 && mc2PosIndex > -1 && mc3PosIndex - 3 != mc2PosIndex && !translation.Substring(mc3PosIndex - 3, 3).Contains(@"\\"))
                {
                    newValue = newValue.Remove(mc2PosIndex, 3);//удаление \\N в позиции mc2

                    if (mc3PosIndex > mc2PosIndex)//если позиция mc2 была левее mc3, сместить на 3
                    {
                        mc3PosIndex -= 3;
                    }

                    //вставить \\n в откорректированную позицию перед mc3
                    newValue = newValue.Insert(mc3PosIndex, @"\\N");
                }
            }

            //экстра, вставить пробелы до и после, если их нет
            //newValue = Regex.Replace(newValue, @"([a-zA-Z])\\\\N\[([0-9]+)\]([a-zA-Z])", "$1 \\N[$2] $3");
            newValue = Regex.Replace(newValue, @"\\\\N\[([0-9]+)\]([a-zA-Z])", @"\\N[$1] $2");
            newValue = Regex.Replace(newValue, @"([a-zA-Z])\\\\N\[([0-9]+)\]", @"$1 \\N[$2]");

            return newValue;
        }

        internal static string FixBrokenNameVar2(string original, string translation)
        {
            if (Regex.IsMatch(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]"))
            {
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

            return translation;
        }
    }
}

using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixBrokenNameVarChanger : StringChangerBase
    {
        public FixBrokenNameVarChanger()
        {
        }
        internal override string Description => $"{nameof(FixBrokenNameVarChanger)}";


        // fix
        //orig = \\N[1]いったい何をしてるんだ
        //trans = \\NBlablabla[1]blabla
        /// <summary>
        ///fix for kind of values when \\N was not with [#] in line
        ///\\N\\N[\\V[122]]
        ///"\\N[\\V[122]]'s blabla... and [1]' s bla...!
        ///　\\NIt \\Nseems to[2222] be[1]'s blabla...!
        /// </summary>
        /// <param name=THSettings.TranslationColumnName></param>
        /// <returns></returns>
        internal override string Change(string inputString, object extraData)
        {
            //вот такой пипец теоритически возможен
            //\\N\\N[\\V[122]]
            //"\\N[\\V[122]]'s blabla... and [1]' s bla...!
            //　\\NIt \\Nseems to[2222] be[1]'s blabla...!

            var translation = inputString;
            var ret = false;

            //выдирание совпадений из перевода
            //var mc1 = Regex.Matches(translation, @"\\\\N\[[0-9]+\]");
            var mc2 = Regex.Matches(translation, @"\\\\N(?=[^\[])"); //catch only \\N without [ after
            var mc3 = Regex.Matches(translation, @"(?<=[^\\][^\\][^A-Z])\[[0-9]+\]"); // match only \\A-Z[0-9+] but catch without \\A-Z before it

            //рабочие переменные
            int max = mc3.Count;//максимум итераций цикла
            int mc2Correction = mc3.Count > mc2.Count ? mc3.Count - mc2.Count : 0;//когда mc2 нашло меньше, чем mc3
            int positionCorrectionMc3 = 0;//переменная для коррекции номера позиции в стоке, т.к. \\N выдирается и позиция меняется на 3
            int minimalIndex = 9999999; //минимальный индекс, для правильного контроля коррекции позиции
            string newValue = translation;//значение, которое будет редактироваться и возвращено
            for (int i = max - 1; i >= 0; i--)//цикл задается в обратную сторону, т.к. так проще контроллировать смещение позиции
            {
                int mc2I = i - mc2Correction;//задание индекса в коллекции для mc2, т.к. их может быть меньше
                if (mc2I == -1)//если mc2 закончится, выйти из цикла
                {
                    break;
                }

                //если индекс позиции больше последнего минимального, подкорректировать на 3, когда совпадение раньше, коррекция не требуется
                if (mc3[i].Index > minimalIndex)
                {
                    positionCorrectionMc3 += 3;
                }
                else
                {
                    positionCorrectionMc3 = 0;
                }

                int mc3PosIndex = mc3[i].Index - positionCorrectionMc3;//новый индекс с учетом коррекции
                int mc2PosIndex = mc2[mc2I].Index;
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
                if (mc3PosIndex - 3 > -1 && /*mc2PosIndex > -1 &&*/ mc3PosIndex - 3 != mc2PosIndex && !translation.Substring(mc3PosIndex - 3, 3).Contains(@"\\"))
                {
                    newValue = newValue.Remove(mc2PosIndex, 3);//удаление \\N в позиции mc2

                    if (mc3PosIndex > mc2PosIndex)//если позиция mc2 была левее mc3, сместить на 3
                    {
                        mc3PosIndex -= 3;
                    }

                    //вставить \\n в откорректированную позицию перед mc3
                    newValue = newValue.Insert(mc3PosIndex, @"\\N");
                    ret = true;
                }
            }

            if (ret)
            {
                //экстра, вставить пробелы до и после, если их нет
                //newValue = Regex.Replace(newValue, @"([a-zA-Z])\\\\N\[([0-9]+)\]([a-zA-Z])", "$1 \\N[$2] $3");
                newValue = Regex.Replace(newValue, @"\\\\N\[([0-9]+)\]([a-zA-Z])", @"\\N[$1] $2");
                newValue = Regex.Replace(newValue, @"([a-zA-Z])\\\\N\[([0-9]+)\]", @"$1 \\N[$2]");

                return newValue;
            }

            return inputString;
        }
    }
}

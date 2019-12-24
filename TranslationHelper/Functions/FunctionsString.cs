using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsString
    {
        /// <summary>
        /// added to string split function to make line split more safe with smaller chance to brake special symbols
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        public static bool IsStringContainsSpecialSymbols(string cellValue)
        {
            return
                cellValue.Contains("\\")
                ;

        }

        /// <summary>
        /// функция деления строки на равные части с остатком и запись их в строковый массив
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static string[] THSplit(string str, int chunkSize)
        {
            if (str == null)
                return null;

            string[] parts = new string[chunkSize];

            int ind = 0;
            int strLength = str.Length;
            //памятка о приведении типов
            //https://www.aubrett.com/article/information-technology/web-development/net-framework/csharp/csharp-division-floating-point
            //THMsg.Show("strLength=" + strLength + ",str=" + str + ",f=" + f);
            int substrLength = (int)Math.Ceiling((double)strLength / chunkSize);//округление числа символов в части в большую сторону
            //THMsg.Show("f="+f+", substrLength=" + substrLength);
            int partsLength = parts.Length;
            for (int i = 0; i < partsLength; i++)
            {
                if (i == partsLength - 1)
                {
                    parts[i] = str.Substring(ind, strLength - ind);
                }
                else
                {
                    parts[i] = str.Substring(ind, substrLength);
                    ind += substrLength;
                }
            }

            return parts;
        }

        public static string FixForRPGMAkerQuotationInSomeStrings(DataRow row)
        {
            string origValue = row[0] as string;
            string cvalue = row[1] as string;
            //rpgmaker mv string will broke script if starts\ends with "'" and contains another "'" in middle
            bool cvalueStartsWith = cvalue.StartsWith("\"");
            bool cvalueEndsWith = cvalue.EndsWith("\"");
            if (
                 //если оригинал начинается и кончается на апостроф, а в переводе апостроф отсутствует на начале или конце
                 (origValue.StartsWith("\"") && origValue.EndsWith("\"") && (!cvalueStartsWith || !cvalueEndsWith))
                 ||
                 //если перевод начинается и кончается на апостроф и также апостроф есть в где-то середине
                 (cvalueStartsWith && cvalueEndsWith && cvalue.Length > 2 && FunctionsString.IsStringAContainsStringB(cvalue.Remove(cvalue.Length - 1, 1).Remove(0, 1), "\"")))
            {
                cvalue = "\"" +
                    cvalue
                    .Replace("\"", string.Empty)
                    + "\""
                    ;
            }
            else
            {
                cvalueStartsWith = cvalue.StartsWith("'");
                cvalueEndsWith = cvalue.EndsWith("'");
                if (
                //если оригинал начинается и кончается на апостроф, а в переводе апостроф отсутствует на начале или конце
                (origValue.StartsWith("'") && origValue.EndsWith("'") && (!cvalueStartsWith || !cvalueEndsWith))
                ||
                //если перевод начинается и кончается на апостроф и также апостроф есть в где-то середине
                (cvalueStartsWith && cvalueEndsWith && cvalue.Length > 2 && FunctionsString.IsStringAContainsStringB(cvalue.Remove(cvalue.Length - 1, 1).Remove(0, 1), "'")))
                {
                    cvalue = "'" +
                        cvalue
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
            return cvalue;
        }

        /// <summary>
        /// Split string to lines
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitToLines(this string input)
        {
            //https://stackoverflow.com/a/23408020
            if (input == null)
            {
                yield break;
            }

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static bool IsMultiline(string input)
        {
            if (input != null)
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(input))
                {
                    int i = 0;
                    while (reader.ReadLine() != null)
                    {
                        i++;
                        if (i > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if string A contains string B (if length of A > length of A with replaced B by "")<br></br><br></br>
        /// Speed check tests: https://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
        /// </summary>
        /// <param name="StringAWhereSearch"></param>
        /// <param name="StringBToSearch"></param>
        /// <returns></returns>
        public static bool IsStringAContainsStringB(string StringAWhereSearch, string StringBToSearch)
        {
            int StringAInWhichSearchLength = StringAWhereSearch.Length;
            if (StringAInWhichSearchLength > 0 && StringBToSearch.Length > 0)//safe check for empty values
            {
                //if string A contains string B then string A with replaced stringB by empty will be
                return StringAInWhichSearchLength > StringAWhereSearch.Replace(StringBToSearch, string.Empty).Length;
            }
            return false;

        }

        public static bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            str = str.Replace(".", string.Empty);
            //https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits
            //наибыстрейший метод 
            int strLength = str.Length;//и моя оптимизация, ускоряющая с 2.19 до 1.62 при 100млн. итераций
            for (int i = 0; i < strLength; i++)
            {
                if ((str[i] ^ '0') > 9)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

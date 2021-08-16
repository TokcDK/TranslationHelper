using System;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions
{
    /// <summary>
    /// Functions for work with strings
    /// </summary>
    internal static class FunctionsString
    {
        /// <summary>
        /// replaces chars in selected string by replacement pairs from string[][2] array
        /// </summary>
        /// <param name="workString"></param>
        /// <param name="arrayPairs"></param>
        /// <returns></returns>
        internal static string CharsReplacementByPairsFromArray(string workString, string[][] arrayPairs)
        {
            int arrayPairsLength = arrayPairs.Length;

            if (arrayPairsLength == 0)
            {
                return workString;
            }

            for (int i = 0; i < arrayPairsLength; i++)
            {
                if (arrayPairs[i].Length != 2)
                {
                    continue;
                }

                workString = workString.Replace(arrayPairs[i][0], arrayPairs[i][1]);
            }

            return workString;
        }

        /// <summary>
        /// added to string split function to make line split more safe with smaller chance to brake special symbols
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        internal static bool IsStringContainsSpecialSymbols(string cellValue)
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
        internal static string[] SplitStringByEqualParts(string str, int chunkSize)
        {
            if (str == null || chunkSize < 1)
                return null;

            if (str.Length < chunkSize)//when length < of required chunkSize size
                return new string[1] { str };

            string[] parts = new string[chunkSize];
            parts[0] = string.Empty;
            if (chunkSize == 1)
            {
                foreach (var line in str.SplitToLines())
                {
                    parts[0] += line;
                }
            }
            else
            {

                int ind = 0;
                int strLength = str.Length;
                //памятка о приведении типов
                //https://www.aubrett.com/article/information-technology/web-development/net-framework/csharp/csharp-division-floating-point
                ///*THMsg*/MessageBox.Show("strLength=" + strLength + ",str=" + str + ",f=" + f);
                int substrLength = (int)Math.Ceiling((double)strLength / chunkSize);//округление числа символов в части в большую сторону
                ///*THMsg*/MessageBox.Show("f="+f+", substrLength=" + substrLength);
                int partsLength = parts.Length;
                for (int i = 0; i < partsLength; i++)
                {
                    try
                    {
                        if (i == partsLength - 1)
                        {
                            parts[i] = str.Substring(ind, strLength - ind);
                        }
                        else
                        {
                            parts[i] = str.Substring(ind, substrLength);//here is can be exception when ind parameter is out of range
                            ind += substrLength;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return parts;
        }

        /// <summary>
        /// true if count of symbol A in string A == count of symbol B in string B equals
        /// </summary>
        /// <param name="aValue"></param>
        /// <param name="bValue"></param>
        /// <param name="symbolA"></param>
        /// <param name="symbolB"></param>
        /// <returns></returns>
        internal static bool GetCountOfTheSymbolInStringAandBIsEqual(string aValue, string bValue, string symbolA, string symbolB)
        {
            return !(string.IsNullOrEmpty(aValue) || string.IsNullOrEmpty(bValue) || string.IsNullOrEmpty(symbolA) || string.IsNullOrEmpty(symbolB)) && aValue.Length - aValue.Replace(symbolA, string.Empty).Length == bValue.Length - bValue.Replace(symbolB, string.Empty).Length;
        }

        /// <summary>
        /// Check if string A contains string B (if length of A > length of A with replaced B by "")<br></br><br></br>
        /// Speed check tests: https://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
        /// </summary>
        /// <param name="stringAWhereSearch"></param>
        /// <param name="stringBToSearch"></param>
        /// <returns></returns>
        internal static bool IsStringAContainsStringB(string stringAWhereSearch, string stringBToSearch)
        {
            int stringAInWhichSearchLength = stringAWhereSearch.Length;
            if (stringAInWhichSearchLength > 0 && stringBToSearch.Length > 0)//safe check for empty values
            {
                //if string A contains string B then string A with replaced stringB by empty will be
                return stringAInWhichSearchLength > stringAWhereSearch.Replace(stringBToSearch, string.Empty).Length;
            }
            return false;

        }

        /// <summary>
        /// Returns length of longest line in string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static int GetLongestLineLength(string line)
        {
            int returnLength = 0;
            string[] sublines = line.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
            int sublinesLength = sublines.Length;
            if (sublinesLength > 1)
            {
                for (int n = 0; n < sublinesLength; n++)
                {
                    int sublinesNLength = sublines[n].LengthWithoutSpecSymbols();

                    if (sublinesNLength > 0 && sublinesNLength > returnLength)
                    {
                        returnLength = sublinesNLength;
                    }
                }
            }
            else
            {
                returnLength = line.TrimEnd().Length;
            }
            return returnLength;
        }

        /// <summary>
        /// Get all not empty lines to string array
        /// </summary>
        /// <param name="inputstring"></param>
        /// <returns></returns>
        internal static string[] GetAllNonEmptyLinesToArray(string inputstring)
        {
            return inputstring.GetAllLinesToArray(true);
            //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
            //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp

            //обычный способ быстрее
            //List<string> values = new List<string>();
            //foreach (string line in inputstring.SplitToLines())
            //{
            //    if (line.Length > 0)
            //    {
            //        values.Add(line);
            //    }
            //}

            //return values.ToArray();

            //способ с linq, который обычно медленнее
            //return inputstring.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
            //                                .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
            //                                .ToArray();//Все строки, кроме пустых
        }
    }
}

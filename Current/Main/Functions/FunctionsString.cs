using System;
using System.Collections.Generic;
using System.Linq;
//using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using System.Transactions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMMV;
using WolfTrans.Net.Parsers.Events.Map.Event;

namespace TranslationHelper.Functions
{
    internal class QuotedStringsExtractor
    {
        readonly string[] _quotesList = new[] { "'", "`", @"\""" };
        readonly string _commentMark = "//";

        internal string InputString;

        Match _match;
        Group _matchGroup;
        string _retValue;
        string _regexQuote;

        internal string Comment = "";
        internal string[] QuotesList { get => _quotesList; }

        public QuotedStringsExtractor(string inputString, bool removeComment = false, string[] quotesList = null)
        {
            InputString = inputString;

            if (removeComment)
            {
                var commentIndex = InputString.IndexOf(_commentMark);
                if(commentIndex != -1)
                {
                    Comment = InputString.Substring(commentIndex);
                    InputString = InputString.Remove(commentIndex);
                }
            }

            if(quotesList!=null && quotesList.Length > 0)
            {
                _quotesList = quotesList;
            }
        }

        /// <summary>
        /// Will return all quoted strings if any found
        /// </summary>
        /// <param name="removeComment"></param>
        /// <returns></returns>
        internal IEnumerable<string> Extract()
        {
            foreach (var regexQuote in _quotesList)
            {
                _regexQuote = regexQuote;

                var mc = Regex.Matches(InputString, AppMethods.GetRegexQuotesCapturePattern(regexQuote));
                for (int m = mc.Count - 1; m >= 0; m--) // negative because lenght of string will be changing
                {
                    _match = mc[m];
                    _retValue = (_matchGroup = _match.Groups[1]).Value;

                    if (_checkers.Any(_checker => !_checker.IsValid(InputString, _retValue, _matchGroup.Index))) continue;

                    yield return _retValue;
                }
            }
        }

        readonly QuotedStringChecker[] _checkers = new[] { new MetaKeyQuotedStringChecker() };

        /// <summary>
        /// will paste in input string <paramref name="valueToPaste"/> instead of last returned quoted string
        /// </summary>
        /// <param name="valueToPaste"></param>
        /// <returns></returns>
        internal string ReplaceLastExtractedString(string valueToPaste)
        {
            int index = _matchGroup.Index; // get internal index of result

            return InputString = InputString.Remove(index, _matchGroup.Length)
                            .Insert(index, valueToPaste.Replace(_regexQuote, "")); // paste translation on place of original
        }

        internal string ResultString { get => InputString + Comment; }

        internal abstract class QuotedStringChecker
        {
            internal abstract bool IsValid(string inputString, string quotedString, int quotedStringIndex);
        }

        internal class MetaKeyQuotedStringChecker : QuotedStringChecker
        {
            readonly string metaMarker = ".meta['";

            internal override bool IsValid(string parentString, string quotedString, int quotedStringIndex)
            {
                if (!parentString.Contains(metaMarker)) return true;

                var indexToCheck = quotedStringIndex - 7;
                if (indexToCheck <= 0) return true;

                string str2check = parentString.Substring(indexToCheck, 7);
                if (!string.Equals(str2check, metaMarker)) return true;

                return false;
            }
        }

    }

    /// <summary>
    /// Functions for work with strings
    /// </summary>
    internal static class FunctionsString
    {
        /// <summary>
        /// replaces chars in selected string by replacement pairs from string[][2] array
        /// </summary>
        /// <param name="workString"></param>
        /// <param name="ArrayPairs"></param>
        /// <returns></returns>
        internal static string CharsReplacementByPairsFromArray(string workString, string[][] ArrayPairs)
        {
            int ArrayPairsLength = ArrayPairs.Length;

            if (ArrayPairsLength == 0)
            {
                return workString;
            }

            for (int i = 0; i < ArrayPairsLength; i++)
            {
                if (ArrayPairs[i].Length != 2)
                {
                    continue;
                }

                workString = workString.Replace(ArrayPairs[i][0], ArrayPairs[i][1]);
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
                return Array.Empty<string>();

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
                        return Array.Empty<string>();
                    }
                }
            }

            return parts;
        }

        /// <summary>
        /// true if count of symbol A in string A == count of symbol B in string B equals
        /// </summary>
        /// <param name="AValue"></param>
        /// <param name="BValue"></param>
        /// <param name="SymbolA"></param>
        /// <param name="SymbolB"></param>
        /// <returns></returns>
        internal static bool GetCountOfTheSymbolInStringAandBIsEqual(string AValue, string BValue, string SymbolA, string SymbolB)
        {
            return !(string.IsNullOrEmpty(AValue) || string.IsNullOrEmpty(BValue) || string.IsNullOrEmpty(SymbolA) || string.IsNullOrEmpty(SymbolB)) && AValue.Length - AValue.Replace(SymbolA, string.Empty).Length == BValue.Length - BValue.Replace(SymbolB, string.Empty).Length;
        }

        /// <summary>
        /// Check if string A contains string B (if length of A > length of A with replaced B by "")<br></br><br></br>
        /// Speed check tests: https://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
        /// </summary>
        /// <param name="StringAWhereSearch"></param>
        /// <param name="StringBToSearch"></param>
        /// <returns></returns>
        internal static bool IsStringAContainsStringB(string StringAWhereSearch, string StringBToSearch)
        {
            int StringAInWhichSearchLength = StringAWhereSearch.Length;
            if (StringAInWhichSearchLength > 0 && StringBToSearch.Length > 0)//safe check for empty values
            {
                //if string A contains string B then string A with replaced stringB by empty will be
                return StringAInWhichSearchLength > StringAWhereSearch.Replace(StringBToSearch, string.Empty).Length;
            }
            return false;

        }

        /// <summary>
        /// Returns length of longest line in string
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        internal static int GetLongestLineLength(string Line)
        {
            int ReturnLength = 0;
            string[] sublines = Line.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
            int sublinesLength = sublines.Length;
            if (sublinesLength > 1)
            {
                for (int N = 0; N < sublinesLength; N++)
                {
                    int sublinesNLength = sublines[N].LengthWithoutSpecSymbols();

                    if (sublinesNLength > 0 && sublinesNLength > ReturnLength)
                    {
                        ReturnLength = sublinesNLength;
                    }
                }
            }
            else
            {
                ReturnLength = Line.TrimEnd().Length;
            }
            return ReturnLength;
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

using System;
using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Main.Functions
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
                return null;

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
        /// Changing string to uppercase(first char or all) or lowercase.
        /// variant 0 - lowercase / 
        /// variant 1 = Uppercase / 
        /// variant 2 = UPPERCASE / 
        /// </summary>
        /// <param name="THFileElementsDataGridView"></param>
        /// <param name="variant"></param>
        internal static void StringCaseMorph(THDataWork thDataWork, int TableIndex, int variant, bool All = false)
        {
            if (thDataWork.THFilesElementsDataset == null || variant > 2 || (!All && (TableIndex == -1 || thDataWork.Main.THFileElementsDataGridView == null)))
            {
                return;
            }

            int ctransind = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            int corigind = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
            if (!All)
            {
                bool IsAnimations = thDataWork.THFilesElementsDataset.Tables[TableIndex].TableName == "Animations";
                int THFileElementsDataGridViewSelectedCellsCount = thDataWork.Main.THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount();
                if (THFileElementsDataGridViewSelectedCellsCount > 0)
                {
                    try
                    {
                        int[] indexes = FunctionsTable.GetDGVRowIndexsesInDataSetTable(thDataWork);
                        foreach (var rindex in indexes)
                        {
                            var row = thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[rindex];
                            var DSOrigCell = row[corigind] + string.Empty;
                            var DSTransCell = row[ctransind] + string.Empty;
                            if (!string.IsNullOrWhiteSpace(DSTransCell) && DSTransCell != DSOrigCell)
                            {
                                if (IsAnimations && variant == 1 && DSTransCell.IndexOf('/') != -1)//change 'effect1/effect2' to 'Effect1/Effect2'
                                {
                                    string[] parts = DSTransCell.Split('/');
                                    for (int i = 0; i < parts.Length; i++)
                                    {
                                        parts[i] = ChangeRegistryCaseForTheCell(parts[i], variant);
                                    }
                                    row[ctransind] = string.Join("/", parts);
                                }
                                else
                                {
                                    row[ctransind] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                var THFilesElementsDatasetTablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
                for (int tindex = 0; tindex < THFilesElementsDatasetTablesCount; tindex++)
                {
                    var table = thDataWork.THFilesElementsDataset.Tables[tindex];
                    var THFilesElementsDatasetTableRowsCount = table.Rows.Count;
                    bool IsAnimations = table.TableName == "Animations";
                    for (int rindex = 0; rindex < THFilesElementsDatasetTableRowsCount; rindex++)
                    {
                        var row = table.Rows[rindex];
                        var DSOrigCell = row[corigind] + string.Empty;
                        var DSTransCell = row[ctransind] + string.Empty;
                        if (!string.IsNullOrWhiteSpace(DSTransCell)// not empty translation
                            && DSTransCell != DSOrigCell//not equal to original
                            && (variant != 1 || (variant == 1 && !DSTransCell.StartsWith("'s ")))//need for states table. not starts with "'s " to prevent change of this "John's boots" to "John'S boots"
                            )
                        {
                            if (IsAnimations && variant == 1 && DSTransCell.IndexOf('/') != -1)//change 'effect1/effect2' to 'Effect1/Effect2'
                            {
                                string[] parts = DSTransCell.Split('/');
                                for (int i = 0; i < parts.Length; i++)
                                {
                                    parts[i] = ChangeRegistryCaseForTheCell(parts[i], variant);
                                }
                                row[ctransind] = string.Join("/", parts);
                            }
                            else
                            {
                                row[ctransind] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
                            }
                        }
                    }
                }
            }
        }

        private static string ChangeRegistryCaseForTheCell(string DSTransCell, int variant)
        {
            switch (variant)
            {
                case 0:
                    //lowercase
#pragma warning disable CA1308 // Нормализуйте строки до прописных букв
                    return DSTransCell.ToLowerInvariant();
#pragma warning restore CA1308 // Нормализуйте строки до прописных букв
                case 1:
                    //Uppercase
                    //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                    return StringToUpper(DSTransCell);
                case 2:
                    //UPPERCASE
                    return DSTransCell.ToUpperInvariant();
                default:
                    return DSTransCell;
            }
        }

        internal static string StringToUpper(string inputString)
        {
            if (char.IsLetter(inputString[0]))
            {
                inputString = char.ToUpper(inputString[0], CultureInfo.InvariantCulture) + inputString.Substring(1);
            }
            else
            {
                int DSTransCellLength = inputString.Length;
                for (int c = 0; c < DSTransCellLength; c++)
                {
                    char @char = inputString[c];
                    if (IsCustomSymbol(@char) || char.IsWhiteSpace(@char) || char.IsPunctuation(@char))
                    {
                    }
                    else
                    {
                        if (c > 0 && ((@char == 's' && inputString[c - 1] == '\'') || inputString[c - 1] == '\\'))
                        {
                        }
                        else
                        {
                            inputString = inputString.Substring(0, c) + char.ToUpper(inputString[c], CultureInfo.InvariantCulture) + (c == DSTransCellLength - 1 ? string.Empty : inputString.Substring(c + 1));
                        }
                        break;
                    }
                }
            }

            if (inputString.StartsWith("[") && inputString.IsMultiline())
            {
                int lineCnt = 0;
                string resultLine = string.Empty;
                foreach (var line in inputString.SplitToLines())
                {
                    if (lineCnt == 0)
                    {
                        resultLine += line;
                    }
                    else
                    {
                        resultLine += Environment.NewLine;
                        if (lineCnt == 1)
                        {
                            resultLine += StringToUpper(line);
                        }
                        else
                        {
                            resultLine += line;
                        }
                    }
                    lineCnt++;
                }
                inputString = resultLine;
            }

            return inputString;
        }

        private static bool IsCustomSymbol(char @char)
        {
            return @char == '「' || @char == '『' || @char == '"';
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

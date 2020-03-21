using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

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


        internal static string FixENJPQuoteOnStringStart2ndLine(string OriginalValue, string TranslationValue)
        {
            try
            {
                if (FunctionsString.IsMultiline(OriginalValue))
                {
                    if (/*OriginalValue.StartsWith("\"") &&*/ OriginalValue.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1].StartsWith("「"))
                    {
                        //if (!TranslationValue.StartsWith("\""))
                        //{
                        //    return TranslationValue;
                        //}

                        if (FunctionsString.IsMultiline(TranslationValue))
                        {
                            bool StartsWithJpQuote = false;
                            string secondline = TranslationValue.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                            if (secondline.StartsWith("“") || secondline.StartsWith("\"") || !(StartsWithJpQuote = secondline.StartsWith("「")))
                            {
                                if (StartsWithJpQuote)
                                {
                                    return TranslationValue;
                                }

                                string resultString = string.Empty;
                                int ind = 0;
                                foreach (string line in TranslationValue.SplitToLines())
                                {

                                    //new line for multiline
                                    if (ind > 0)
                                    {
                                        resultString += Environment.NewLine;
                                    }

                                    if (ind != 1)
                                    {
                                        resultString += line;
                                    }
                                    else
                                    {
                                        int lineLength = line.Length;
                                        if (lineLength > 1 && (line.StartsWith("“") || line.StartsWith("\"")))
                                        {
                                            resultString += "「" + line.Substring(1);
                                        }
                                        else if (lineLength == 0 || (lineLength == 1 && (line == "“" || line == "\"")))
                                        {
                                            resultString += "「";
                                        }
                                        else if (lineLength > 0)
                                        {
                                            resultString += "「" + line;
                                        }
                                    }
                                    ind++;
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
                 && GetCountOfTheSymbolInStringAandBIsEqual(origValue, NewtransValue, "\"", "\"")))
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
                 && GetCountOfTheSymbolInStringAandBIsEqual(origValue, NewtransValue, "'", "'")))
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
                        return Regex.Replace(translation, @"\\\\([0-9]{1,3})\[([0-9]{1,3})\]", @"\"+mc[0].Value);
                    }
                }
            }

            return translation;
        }

        /// <summary>
        /// true if count of symbol A in string A == count of symbol B in string B equals
        /// </summary>
        /// <param name="AValue"></param>
        /// <param name="BValue"></param>
        /// <param name="SymbolA"></param>
        /// <param name="SymbolB"></param>
        /// <returns></returns>
        private static bool GetCountOfTheSymbolInStringAandBIsEqual(string AValue, string BValue, string SymbolA, string SymbolB)
        {
            return !(string.IsNullOrEmpty(AValue) || string.IsNullOrEmpty(BValue) || string.IsNullOrEmpty(SymbolA) || string.IsNullOrEmpty(SymbolB)) && AValue.Length - AValue.Replace(SymbolA, string.Empty).Length == BValue.Length - BValue.Replace(SymbolB, string.Empty).Length;
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

        /// <summary>
        /// Changing string to uppercase(first char or all) or lowercase.
        /// variant 0 - lowercase / 
        /// variant 1 = Uppercase / 
        /// variant 2 = UPPERCASE / 
        /// </summary>
        /// <param name="THFileElementsDataGridView"></param>
        /// <param name="variant"></param>
        public static void StringCaseMorph(THDataWork thDataWork, int TableIndex, int variant, bool All = false)
        {
            if (thDataWork.THFilesElementsDataset == null || variant > 2 || (!All && (TableIndex == -1 || thDataWork.Main.THFileElementsDataGridView == null)))
            {
                return;
            }

            int ctransind = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            int corigind = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
            if (!All)
            {
                int THFileElementsDataGridViewSelectedCellsCount = thDataWork.Main.THFileElementsDataGridView.GetRowsWithSelectedCellsCount();
                if (THFileElementsDataGridViewSelectedCellsCount > 0)
                {
                    try
                    {
                        int[] indexes = new int[THFileElementsDataGridViewSelectedCellsCount];
                        for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                        {
                            int rind = thDataWork.Main.THFileElementsDataGridView.SelectedCells[i].RowIndex;
                            indexes[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, TableIndex, rind);
                        }
                        foreach (var rindex in indexes)
                        {
                            var DSOrigCell = thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[rindex][corigind] + string.Empty;
                            var DSTransCell = thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[rindex][ctransind] + string.Empty;
                            if (!string.IsNullOrWhiteSpace(DSTransCell) && DSTransCell != DSOrigCell)
                            {
                                thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[rindex][ctransind] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
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
                int THFilesElementsDatasetTablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
                for (int tindex = 0; tindex < THFilesElementsDatasetTablesCount; tindex++)
                {
                    int THFilesElementsDatasetTableRowsCount = thDataWork.THFilesElementsDataset.Tables[tindex].Rows.Count;
                    for (int rindex = 0; rindex < THFilesElementsDatasetTableRowsCount; rindex++)
                    {
                        var DSOrigCell = thDataWork.THFilesElementsDataset.Tables[tindex].Rows[rindex][corigind] + string.Empty;
                        var DSTransCell = thDataWork.THFilesElementsDataset.Tables[tindex].Rows[rindex][ctransind] + string.Empty;
                        if (!string.IsNullOrWhiteSpace(DSTransCell) && DSTransCell != DSOrigCell)
                        {
                            thDataWork.THFilesElementsDataset.Tables[tindex].Rows[rindex][ctransind] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
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
                    return stringToUpper(DSTransCell);
                case 2:
                    //UPPERCASE
                    return DSTransCell.ToUpperInvariant();
                default:
                    return DSTransCell;
            }
        }

        private static string stringToUpper(string DSTransCell)
        {
            if (char.IsLetter(DSTransCell[0]))
            {
                DSTransCell = char.ToUpper(DSTransCell[0], CultureInfo.InvariantCulture) + DSTransCell.Substring(1);
            }
            else
            {
                int DSTransCellLength = DSTransCell.Length;
                for (int c = 0; c < DSTransCellLength; c++)
                {
                    char Char = DSTransCell[c];
                    if (char.IsWhiteSpace(Char) || char.IsPunctuation(Char))
                    {
                    }
                    else
                    {
                        DSTransCell = DSTransCell.Substring(0, c) + char.ToUpper(DSTransCell[c], CultureInfo.InvariantCulture) + (c == DSTransCellLength - 1 ? string.Empty : DSTransCell.Substring(c + 1));
                        break;
                    }
                }
            }

            if (DSTransCell.StartsWith("[") && FunctionsString.IsMultiline(DSTransCell))
            {
                int lineCnt = 0;
                string resultLine = string.Empty;
                foreach (var line in DSTransCell.SplitToLines())
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
                            resultLine += stringToUpper(line);
                        }
                        else
                        {
                            resultLine += line;
                        }
                    }
                    lineCnt++;
                }
                DSTransCell = resultLine;
            }

            return DSTransCell;
        }

        /// <summary>
        /// Returns length of longest line in string
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public static int GetLongestLineLength(string Line)
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
        /// gets length of string without several special symbols
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        private static int LengthWithoutSpecSymbols(this string inputLine)
        {
            string newline = inputLine;

            newline = Regex.Replace(newline, @"^([\s\S]+)(if|en)\([\s\S]+\)$", "$1");
            newline = Regex.Replace(newline, @"\\\#\{\$game_actors\[.+\]\.name\}", "ActorName1");
            newline = Regex.Replace(newline, @"\\\#\{\$game_variables\[.+\]\}", "variable10");
            newline = Regex.Replace(newline, @"\\\\[A-Za-z]\[.+\]", string.Empty);

            return newline.Length;
        }

        public static string SplitMultiLineIfBeyondOfLimit(string Line, int Limit)
        {
            //StringBuilder ReturnLine = new StringBuilder();
            string[] sublines = Line.Split(new string[2] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            int sublinesLength = sublines.Length;
            List<string> Lines = new List<string>();
            if (sublinesLength > 1)
            {
                bool lastLineWasSplitted = false;
                for (int N = 0; N < sublinesLength; N++)
                {
                    string subline = sublines[N];
                    if (lastLineWasSplitted && subline.Length > 0)
                    {
                        //lastLineWasSplitted значит, что соединять только когда предыдущая была разделена, если нет, не нужно трогать порядок переноса из оригинала
                        //когда не первая строка, соединяем эту строку с последним элементом в Lines и удаляем последний элемент, обрабатываем такую новую строку
                        int LasLineIndex = Lines.Count - 1;
                        if (Lines[LasLineIndex].Length > 0)
                        {
                            subline = Lines[LasLineIndex] + subline;
                            Lines.RemoveAt(LasLineIndex);
                        }
                    }
                    int i = 0;
                    lastLineWasSplitted = false;
                    foreach (var line in GetSplittedLine(subline, Limit).SplitToLines())
                    {
                        i++;
                        Lines.Add(line);
                    }

                    if (i > 1)
                    {
                        //если i>1 значит строка была разделена
                        lastLineWasSplitted = true;
                    }
                    //if (N == sublinesLength - 1)
                    //{
                    //    foreach(var line in GetSplittedLine(subline, Limit).SplitToLines())
                    //    {
                    //        Lines.Add(line);
                    //    }
                    //    ReturnLine.Append(string.Join(Environment.NewLine, Lines));
                    //}
                    //else
                    //{
                    //    ReturnLine.AppendLine(GetSplittedLine(subline, Limit));
                    //}
                }
            }
            else
            {
                return GetSplittedLine(Line, Limit);
            }

            return string.Join(Environment.NewLine, Lines); //ReturnLine.ToString();
        }

        private static string GetSplittedLine(string Line, int Limit)
        {
            string Trigger = string.Empty;
            string newLine = ((Trigger = Regex.Match(Line, @"(if|en)\([\s\S]+\)$").Value).Length > 0 ? Line.Replace(Trigger, string.Empty) : Line);
            if (newLine.Length == 0 || newLine.Length <= Limit)
            {
                return Line;
            }
            return string.Join(IsStringAContainsStringB(Properties.Settings.Default.THSelectedSourceType, "RPG Maker MV") ? "\\n " : Environment.NewLine, SplitLineIfBeyondOfLimit(Trigger.Length > 0 ? newLine : Line, Limit)) + Trigger;
        }

        public static string[] SplitLineIfBeyondOfLimit(string text, int max)
        {
            //https://ru.stackoverflow.com/questions/707937/c-%D0%BF%D0%B5%D1%80%D0%B5%D0%BD%D0%BE%D1%81-%D1%81%D0%BB%D0%BE%D0%B2-%D0%B2-%D1%81%D1%82%D1%80%D0%BE%D0%BA%D0%B5-%D1%81-%D1%80%D0%B0%D0%B7%D0%B1%D0%B8%D0%B2%D0%BA%D0%BE%D0%B9-%D0%BD%D0%B0-%D0%BE%D0%BF%D1%80%D0%B5%D0%B4%D0%B5%D0%BB%D0%B5%D0%BD%D0%BD%D1%83%D1%8E-%D0%B4%D0%BB%D0%B8%D0%BD%D1%83
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }
    }
}

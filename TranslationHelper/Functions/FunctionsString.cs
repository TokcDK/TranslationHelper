using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                int THFileElementsDataGridViewSelectedCellsCount = thDataWork.Main.THFileElementsDataGridView.SelectedRows.Count;
                if (THFileElementsDataGridViewSelectedCellsCount > 0)
                {
                    try
                    {
                        int[] indexes = new int[THFileElementsDataGridViewSelectedCellsCount];
                        for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                        {
                            int rind = thDataWork.Main.THFileElementsDataGridView.SelectedRows[i].Index;
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
                    int sublinesNLength = sublines[N].Length;
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
            if (Line.Length == 0 || Line.Length <= Limit)
            {
                return Line;
            }
            return string.Join(FunctionsString.IsStringAContainsStringB(Properties.Settings.Default.THSelectedSourceType, "RPG Maker MV") ? "\\n " : Environment.NewLine, SplitLineIfBeyondOfLimit(Line, Limit));
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

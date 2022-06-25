using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    static class AutoSameForSimularUtils
    {
        public static string GetStringSimularityRegexPattern()
        {
            //http://www.cyberforum.ru/csharp-beginners/thread244709.html
            string regexPatternQuotationMark = "\"";
            //string regexPatternLatinSymbols = @"\d|\!|\?|\.|[|]";
            string regexPatternLatinSymbolsStart = @"\d|\!|\?|\.|\[";
            string regexPatternLatinSymbolsEnd = @"\d|\!|\?|\.|\]";
            //string regexPatternJapanesSymbols = @"\！|\？|\。|\【|\】|\「|\」|\『|\』|\〝|\〟";
            string regexPatternJapanesSymbolsStart = @"！|？|。|【|「|『|〝";
            string regexPatternJapanesSymbolsEnd = @"！|？|。|】|」|』|〟";
            string regexPatternDigitsInAnyPlace = @"\d+";
            string regexPatternDigitsOrSymbolsInStartOfLine = "(^(" + regexPatternJapanesSymbolsStart + "|" + regexPatternQuotationMark + "|" + regexPatternLatinSymbolsStart + ")+)";
            string regexPatternDigitsOrSymbolsInEndOfLine = "((" + regexPatternLatinSymbolsEnd + "|" + regexPatternQuotationMark + "|" + regexPatternJapanesSymbolsEnd + ")+$)";

            return regexPatternDigitsOrSymbolsInStartOfLine + "|" + regexPatternDigitsInAnyPlace + "|" + regexPatternDigitsOrSymbolsInEndOfLine;
        }

        /// <summary>
        /// Set same translation values for rows with simular original
        /// </summary>
        public static void Set(this DataRow dataRow, bool inputForceSetValue = false)
        {
            var table = dataRow.Table;
            Set(inputTableIndex: AppData.CurrentProject.FilesContent.Tables.IndexOf(table), inputRowIndex: table.Rows.IndexOf(dataRow), inputForceSetValue: inputForceSetValue);
        }

        public static HashSet<string> AutoSame4SimilarStack = new HashSet<string>();

        /// <summary>
        /// Set same translation values for rows with simular original
        /// </summary>
        /// <param name="inputTableIndex"></param>
        /// <param name="inputRowIndex"></param>
        /// <param name="inputOriginalColumnIndex"></param>
        /// <param name="inputForceSetValue"></param>
        public static void Set(int inputTableIndex, int inputRowIndex, bool inputForceSetValue = false)
        {
            if (!Properties.Settings.Default.ProjectIsOpened) return;

            string inputTableRowOriginalCellValue = "";
            try
            {
                if (inputTableIndex == -1
                    || inputRowIndex == -1
                    //|| inputOriginalColumnIndex == -1
                    || AppData.CurrentProject.FilesContent == null
                    || inputTableIndex > AppData.CurrentProject.FilesContent.Tables.Count - 1
                    || inputRowIndex > AppData.CurrentProject.FilesContent.Tables[inputTableIndex].Rows.Count - 1
                    || (AppData.CurrentProject.FilesContent.Tables[inputTableIndex].Rows[inputRowIndex][AppData.CurrentProject.TranslationColumnIndex] + string.Empty).Length == 0
                    )
                {
                    return;
                }

                //присвоить значения для обработки
                var inputTable = AppData.CurrentProject.FilesContent.Tables[inputTableIndex];
                var inputTableRow = inputTable.Rows[inputRowIndex];
                var inputTableRowOriginalCell = inputTableRow[AppData.CurrentProject.OriginalColumnIndex];
                inputTableRowOriginalCellValue = inputTableRowOriginalCell + "";
                if (AutoSame4SimilarStack.Contains(inputTableRowOriginalCellValue)) return;
                AutoSame4SimilarStack.Add(inputTableRowOriginalCellValue);

                int translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
                var inputTableRowTranslationCell = inputTableRow[translationColumnIndex];

                var inputRow = AppData.CurrentProject.FilesContent.Tables[inputTableIndex].Rows[inputRowIndex];
                if (string.IsNullOrEmpty(inputTableRowTranslationCell + "") || inputTableRowTranslationCell == inputTableRowOriginalCell)
                {
                    AutoSame4SimilarStack.Remove(inputTableRowOriginalCellValue);
                    return;
                }

                string regexPattern = GetStringSimularityRegexPattern();

                Regex reg = new Regex(regexPattern); //reg равняется любым цифрам
                string inputOriginalValue = FunctionsRomajiKana.THFixDigits(inputTableRowOriginalCellValue);
                string inputTranslationValue = FunctionsRomajiKana.THFixDigits(inputTableRowTranslationCell as string);

                //Было исключение OutOfRangeException когда в оригинале присутствовали совпадения для regex, а входной перевод был пустой или равен \r\n, тогда попытка получить индекс совпадения из оригинала заканчивалась исключением, т.к. никаких совпадений не было. Похоже на неверный перевод от онлайн сервиса
                if (string.IsNullOrWhiteSpace(inputTranslationValue) || inputTranslationValue == Environment.NewLine)
                {
                    AutoSame4SimilarStack.Remove(inputTableRowOriginalCell + "");
                    return;
                }

                bool weUseDuplicates = false;
                try
                {
                    weUseDuplicates = !AppData.CurrentProject.DontLoadDuplicates && AppData.CurrentProject.OriginalsTableRowCoordinates != null /*&& ProjectData.CurrentProject.OriginalsTableRowCoordinates[inputOriginalValue].Values.Count > 1*/;
                }
                catch { }

                //set same value for duplicates from row coordinates list
                if (weUseDuplicates)
                {
                    var inputTableName = inputTable.TableName;
                    foreach (var storedTableName in AppData.CurrentProject.OriginalsTableRowCoordinates[inputOriginalValue])
                    {
                        var table = AppData.CurrentProject.FilesContent.Tables[storedTableName.Key];
                        if (table == null) continue;

                        foreach (var storedRowIndex in storedTableName.Value)
                        {
                            var row = table.Rows[storedRowIndex];

                            //skip if same table\row as input or row translation is not empty
                            if ((storedTableName.Key == inputTableName && storedRowIndex == inputRowIndex) || (!inputForceSetValue && (row[1] + "").Length > 0))
                            {
                                continue;
                            }

                            row[1] = inputTranslationValue;
                        }
                    }
                }

                //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
                if (Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese"
                    && Regex.Matches(inputTranslationValue, @"\d+").Count != Regex.Matches(inputOriginalValue, @"\d+").Count)
                {
                    AutoSame4SimilarStack.Remove(inputTableRowOriginalCellValue);
                    return;
                }

                MatchCollection inputOriginalMatches = reg.Matches(inputOriginalValue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
                int inputOriginalMatchesCount = inputOriginalMatches.Count;

                // Standart rows scan
                int tablesCount = AppData.CurrentProject.FilesContent.Tables.Count;
                for (int targetTableIndex = 0; targetTableIndex < tablesCount; targetTableIndex++) //количество файлов
                {
                    //если приложение закрылось
                    if (Properties.Settings.Default.IsTranslationHelperWasClosed) break;

                    var targetTable = AppData.CurrentProject.FilesContent.Tables[targetTableIndex];
                    var rowsCount = targetTable.Rows.Count;

                    var useSkipped = weUseDuplicates && AppData.CurrentProject.OriginalsTableRowCoordinates[inputOriginalValue].ContainsKey(targetTable.TableName);

                    //LogToFile("Table "+Tindx+" proceed");
                    for (int targetRowIndex = 0; targetRowIndex < rowsCount; targetRowIndex++) //количество строк в каждом файле
                    {
                        //если приложение закрылось
                        if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                        {
                            AutoSame4SimilarStack.Remove(inputTableRowOriginalCellValue);
                            return;
                        }

                        var targetRow = targetTable.Rows[targetRowIndex];
                        var targetOriginalCell = targetRow[AppData.CurrentProject.OriginalColumnIndex];
                        var targetTranslationCell = targetRow[translationColumnIndex];
                        string targetOriginallCellString = targetOriginalCell as string;
                        string targetTranslationCellString = targetTranslationCell + string.Empty;
                        if (
                            (targetTableIndex == inputTableIndex && targetRowIndex == inputRowIndex) // skip input row index
                            ||
                            (!inputForceSetValue && (
                            targetTranslationCellString == targetOriginallCellString /*только если оригинал и перевод целевой ячейки не равны-*/
                            || !string.IsNullOrEmpty(targetTranslationCellString) //Проверять только для пустых ячеек перевода
                            ))
                            )
                        {
                            continue;
                        }

                        if (ParsedWithExtractMulti(targetOriginallCellString, targetTranslationCellString, inputOriginalValue, inputTranslationValue, targetRow))
                        {
                            continue;
                        }

                        //LogToFile("THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString()=" + THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString());
                        //если количество совпадений в mc больше нуля, т.е. цифры были в поле untrans выбранной только что переведенной ячейки
                        //также проверить, если оригиналы с цифрами не равны, иначе присваивать по обычному
                        if (inputOriginalMatchesCount == 0 || Equals(inputTableRowOriginalCell, targetOriginalCell))
                        {
                            //иначе, если в поле оригинала не было цифр, сравнить как обычно, два поля между собой 
                            if (!weUseDuplicates // skip when using duplicates and search for fully duplicated values already was made earlier
                                && Equals(inputTableRowOriginalCell, targetOriginalCell)) //если поле Untrans елемента равно только что измененному
                            {
                                //ProjectData.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                                targetRow[translationColumnIndex] = inputTableRowTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                            }

                            continue;
                        }

                        //string TargetOriginallCellStringFixed = RomajiKana.THFixDigits(TargetOriginallCellString);
                        MatchCollection targetMatches = reg.Matches(targetOriginallCellString); //mc0 равно значениям цифр ячейки под номером y в файле i
                        int targetMatchesCount = targetMatches.Count;
                        if (targetMatchesCount == 0) continue; //если количество совпадений больше нуля(идти дальше), т.е. цифры были в поле untrans проверяемой на совпадение ячейки

                        string TargetTransCellValueWithRemovedPatternMatches = Regex.Replace(targetTranslationCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                        string InputTransCellValueWithRemovedPatternMatches = Regex.Replace(inputTranslationValue, regexPattern, string.Empty, RegexOptions.Compiled);

                        //Если значение ячеек перевода без паттернов равны, идти дальше
                        if (TargetTransCellValueWithRemovedPatternMatches == InputTransCellValueWithRemovedPatternMatches) continue;

                        string TargetOrigCellValueWithRemovedPatternMatches = Regex.Replace(targetOriginallCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                        string InputOrigCellValueWithRemovedPatternMatches = Regex.Replace(inputOriginalValue, regexPattern, string.Empty, RegexOptions.Compiled);

                        //LogToFile("checkingorigcellvalue=\r\n" + checkingorigcellvalue + "\r\ninputorigcellvalue=\r\n" + inputorigcellvalue);
                        //если поле перевода равно только что измененному во входной, без учета цифр
                        if (TargetOrigCellValueWithRemovedPatternMatches != InputOrigCellValueWithRemovedPatternMatches
                            || inputOriginalMatchesCount != targetMatchesCount
                            || !IsAllMatchesInIdenticalPlaces(inputOriginalMatches, targetMatches)
                            ) continue;

                        //{
                        ////инициализация основных целевого и входного массивов
                        //string[] inputOrigMatches = new string[mccount];
                        //string[] targetOrigMatches = new string[mccount];
                        ////присваивание цифр из совпадений в массивы, в основной входного и во временный целевого
                        //for (int r = 0; r < mccount; r++)
                        //{
                        //    inputOrigMatches[r] = RomajiKana.THFixDigits(mc[r].Value/*.Replace(mc[r].Value, mc[r].Value)*/);
                        //    targetOrigMatches[r] = RomajiKana.THFixDigits(mc0[r].Value/*.Replace(mc0[r].Value, mc0[r].Value)*/);
                        //}
                        //также инфо о другом способе:
                        //http://qaru.site/questions/41136/how-to-convert-matchcollection-to-string-array
                        //там же все тесты и for, как у здесь меня - наиболее быстрый вариант

                        //string inputresult = Regex.Replace(inputtranscellvalue, pattern, "{{$1}}");//оборачивание цифры в {{}}, чтобы избежать ошибочных замен например замены 5 на 6 в значении, где есть 5 50
                        //переименовано и закомментировано, т.к. было убрано оборачивание в цифры. string inputtranscellvalue = inputtranscellvalue;//оборачивание цифры в {{}}, чтобы избежать ошибочных замен например замены 5 на 6 в значении, где есть 5 50
                        //}

                        MatchCollection tm = reg.Matches(inputTranslationValue);

                        //количество совпадений должно быть равное для избежания исключений и прочих неверных замен
                        if (tm.Count != inputOriginalMatches.Count) continue;

                        int startindex;
                        int stringoverallength = 0;
                        int stringlength;
                        int stringoverallength0 = 0;
                        bool failed = false;
                        //LogToFile("arraysize=" + arraysize + ", wrapped inputresult" + inputresult);
                        for (int m = 0; m < inputOriginalMatchesCount; m++)
                        {
                            //проверка, ЧТОБЫ СОВПАДЕНИЯ ОТЛИЧАЛИСЬ, Т.Е. НЕ МЕНЯТЬ ! НА ! И ? НА ?, ТОЛЬКО ! НА ? И 1 НА 2
                            if (inputOriginalMatches[m].Value == targetMatches[m].Value) continue;

                            //замена символа путем удаления на позиции и вставки нового:https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
                            startindex = tm[m].Index - stringoverallength + stringoverallength0;//отнять предыдущее число и заменить новым числом, для корректировки индекса

                            stringlength = tm[m].Value.Length;
                            stringoverallength += stringlength;//запомнить общую длину заменяемых символов, для коррекции индекса позиции для замены

                            //InputTransCellValue = InputTransCellValue.Remove(startindex, stringlength).Insert(startindex, targetOrigMatches[m]);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."
                            inputTranslationValue = inputTranslationValue.Remove(startindex, stringlength).Insert(startindex, targetMatches[m].Value);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."

                            //stringoverallength0 += targetOrigMatches[m].Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены
                            stringoverallength0 += targetMatches[m].Value.Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены

                        }
                        //только если ячейка пустая
                        targetTranslationCell = AppData.CurrentProject.FilesContent.Tables[targetTableIndex].Rows[targetRowIndex][translationColumnIndex];
                        if (!failed && (inputForceSetValue || targetTranslationCell == null || string.IsNullOrEmpty(targetTranslationCell as string)))
                        {
                            //ProjectData.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTransCellValue;
                            targetRow[translationColumnIndex] = inputTranslationValue;
                        }
                    }
                }
            }
            catch { }

            if (string.IsNullOrEmpty(inputTableRowOriginalCellValue) || !AutoSame4SimilarStack.Contains(inputTableRowOriginalCellValue)) return;

            AutoSame4SimilarStack.Remove(inputTableRowOriginalCellValue);
        }

        /// <summary>
        /// Try to translate using multiextract method
        /// </summary>
        /// <param name="targetOriginallCellString"></param>
        /// <param name="targetTranslationCellString"></param>
        /// <param name="inputOriginalValue"></param>
        /// <param name="inputTranslationValue"></param>
        /// <param name="targetRow"></param>
        /// <returns></returns>
        private static bool ParsedWithExtractMulti(string targetOriginallCellString, string targetTranslationCellString, string inputOriginalValue, string inputTranslationValue, DataRow targetRow)
        {
            var extractedTargetOriginalIndexses = new List<int>();
            var extractedTargetOriginal = targetOriginallCellString.ExtractMulty(outIndexes: extractedTargetOriginalIndexses);
            int extractedTargetOriginalLength = extractedTargetOriginal.Length;
            var extractedTargetOriginalIndexsesCount = extractedTargetOriginalIndexses.Count;
            if (extractedTargetOriginalLength == 0
                || extractedTargetOriginal[0] == targetOriginallCellString
                || extractedTargetOriginalIndexsesCount != extractedTargetOriginalLength)
            {
                return false;
            }

            var extractedTargetTranslationIndexses = new List<int>();
            var extractedTargetTranslation = targetTranslationCellString.ExtractMulty(outIndexes: extractedTargetTranslationIndexses);
            var extractedTargetTranslationLength = extractedTargetTranslation.Length;
            if (extractedTargetTranslationLength == 0
                || extractedTargetTranslation[0] == targetTranslationCellString
                || extractedTargetTranslationLength != extractedTargetTranslationIndexses.Count
                || extractedTargetTranslationLength != extractedTargetOriginalLength
                )
            {
                return false;
            }

            bool parsedWithMultyExtract = false;
            for (int i = extractedTargetOriginalIndexsesCount - 1; i >= 0; i--)
            {
                if (inputOriginalValue == extractedTargetOriginal[i])
                {
                    parsedWithMultyExtract = true;

                    targetTranslationCellString = targetTranslationCellString
                        .Remove(extractedTargetTranslationIndexses[i], extractedTargetTranslation[i].Length)
                        .Insert(extractedTargetTranslationIndexses[i], inputTranslationValue);
                }
            }

            if (parsedWithMultyExtract)
            {
                targetRow[AppData.CurrentProject.TranslationColumnIndex] = targetTranslationCellString;
                return true; ;
            }

            return false;
        }

        public static bool IsAllMatchesInIdenticalPlaces(MatchCollection mc, MatchCollection mc0)
        {
            try
            {
                int overallength = 0;
                int overallength0 = 0;
                int mcCount = mc.Count;
                for (int m = 0; m < mcCount; m++)
                {
                    if (mc[m].Index - overallength == mc0[m].Index - overallength0)
                    {
                        overallength += mc[m].Length;
                        overallength0 += mc0[m].Length;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

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
        /// <param name="inputForceSetValue"></param>
        public static void Set(int inputTableIndex, int inputRowIndex, bool inputForceSetValue = false)
        {
            if (!AppSettings.ProjectIsOpened) return;
            if (inputTableIndex == -1) return;
            if (inputRowIndex == -1) return;
            if (AppData.CurrentProject.FilesContent == null) return;
            if (inputTableIndex > AppData.CurrentProject.FilesContent.Tables.Count - 1) return;
            if (inputRowIndex > AppData.CurrentProject.FilesContent.Tables[inputTableIndex].Rows.Count - 1) return;

            var inputRowDataForStack = inputTableIndex + "|" + inputRowIndex + "|" + inputForceSetValue;

            if (AutoSame4SimilarStack.Contains(inputRowDataForStack)) return;

            AutoSame4SimilarStack.Add(inputRowDataForStack);

            Set4Similar(inputTableIndex, inputRowIndex, inputForceSetValue, inputRowDataForStack);
        }

        private static void Set4Similar(int inputTableIndex, int inputRowIndex, bool inputForceSetValue, string inputRowDataForStack)
        {
            //присвоить значения для обработки
            var inputTable = AppData.CurrentProject.FilesContent.Tables[inputTableIndex];
            var inputTableRow = inputTable.Rows[inputRowIndex];
            var inputTableRowOriginalCellValue = inputTableRow.Field<string>(AppData.CurrentProject.OriginalColumnIndex);

            int translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
            var inputTableRowTranslationCellValue = inputTableRow.Field<string>(translationColumnIndex);
            if (string.IsNullOrEmpty(inputTableRowTranslationCellValue) || inputTableRowTranslationCellValue.Equals(inputTableRowOriginalCellValue))
            {
                AutoSame4SimilarStack.Remove(inputRowDataForStack);
                return;
            }

            string regexPattern = GetStringSimularityRegexPattern();

            Regex reg = new Regex(regexPattern); //reg равняется любым цифрам
            string inputOriginalValue = FunctionsRomajiKana.THFixDigits(inputTableRowOriginalCellValue);
            string inputTranslationValue = FunctionsRomajiKana.THFixDigits(inputTableRowTranslationCellValue);

            //Было исключение OutOfRangeException когда в оригинале присутствовали совпадения для regex, а входной перевод был пустой или равен \r\n, тогда попытка получить индекс совпадения из оригинала заканчивалась исключением, т.к. никаких совпадений не было. Похоже на неверный перевод от онлайн сервиса
            if (string.IsNullOrWhiteSpace(inputTranslationValue) || inputTranslationValue == Environment.NewLine)
            {
                AutoSame4SimilarStack.Remove(inputRowDataForStack);
                return;
            }

            bool weUseDuplicates = false;
            try
            {
                weUseDuplicates = !AppData.CurrentProject.DontLoadDuplicates && AppData.CurrentProject.OriginalsTableRowCoordinates != null /*&& ProjectData.CurrentProject.OriginalsTableRowCoordinates[inputOriginalValue].Values.Count > 1*/;
            }
            catch { }

            //set same value for duplicates from row coordinates list
            if (weUseDuplicates) SetSameIfUseDups(inputTable, inputOriginalValue, inputRowIndex, inputForceSetValue, inputTranslationValue);

            //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
            if (AppSettings.OnlineTranslationSourceLanguage == "Japanese"
                && Regex.Matches(inputTranslationValue, @"\d+").Count != Regex.Matches(inputOriginalValue, @"\d+").Count)
            {
                AutoSame4SimilarStack.Remove(inputRowDataForStack);
                return;
            }

            MatchCollection inputOriginalMatches = reg.Matches(inputOriginalValue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
            int inputOriginalMatchesCount = inputOriginalMatches.Count;

            // Standart rows scan
            int tablesCount = AppData.CurrentProject.FilesContent.Tables.Count;
            for (int targetTableIndex = 0; targetTableIndex < tablesCount; targetTableIndex++) //количество файлов
            {
                //если приложение закрылось
                if (AppSettings.IsTranslationHelperWasClosed) break;

                var targetTable = AppData.CurrentProject.FilesContent.Tables[targetTableIndex];
                var rowsCount = targetTable.Rows.Count;

                //var useSkipped = weUseDuplicates && AppData.CurrentProject.OriginalsTableRowCoordinates[inputOriginalValue].ContainsKey(targetTable.TableName);

                for (int targetRowIndex = 0; targetRowIndex < rowsCount; targetRowIndex++) //количество строк в каждом файле
                {
                    //если приложение закрылось
                    if (AppSettings.IsTranslationHelperWasClosed)
                    {
                        AutoSame4SimilarStack.Remove(inputRowDataForStack);
                        return;
                    }

                    var targetRow = targetTable.Rows[targetRowIndex];
                    var targetOriginalCellValue = targetRow.Field<string>(AppData.CurrentProject.OriginalColumnIndex);
                    var targetTranslationCellValue = targetRow.Field<string>(translationColumnIndex);
                    if (
                        (targetTableIndex == inputTableIndex && targetRowIndex == inputRowIndex) // skip input row index
                        ||
                        (!inputForceSetValue && (
                        targetTranslationCellValue == targetOriginalCellValue /*только если оригинал и перевод целевой ячейки не равны-*/
                        || !string.IsNullOrEmpty(targetTranslationCellValue) //Проверять только для пустых ячеек перевода
                        ))
                        )
                    {
                        continue;
                    }

                    if (ParsedWithExtractMulti(targetOriginalCellValue, targetTranslationCellValue, inputOriginalValue, inputTranslationValue, targetRow))
                    {
                        continue;
                    }

                    //если количество совпадений в mc больше нуля, т.е. цифры были в поле untrans выбранной только что переведенной ячейки
                    //также проверить, если оригиналы с цифрами не равны, иначе присваивать по обычному
                    if (inputOriginalMatchesCount == 0 || inputTableRowOriginalCellValue.Equals(targetOriginalCellValue))
                    {
                        //иначе, если в поле оригинала не было цифр, сравнить как обычно, два поля между собой 
                        if (!weUseDuplicates // skip when using duplicates and search for fully duplicated values already was made earlier
                            && inputTableRowOriginalCellValue.Equals(targetOriginalCellValue)) //если поле Untrans елемента равно только что измененному
                        {
                            targetRow[translationColumnIndex] = inputTableRowTranslationCellValue; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                        }

                        continue;
                    }

                    MatchCollection targetMatches = reg.Matches(targetOriginalCellValue); //mc0 равно значениям цифр ячейки под номером y в файле i
                    int targetMatchesCount = targetMatches.Count;
                    if (targetMatchesCount == 0) continue; //если количество совпадений больше нуля(идти дальше), т.е. цифры были в поле untrans проверяемой на совпадение ячейки

                    string TargetTransCellValueWithRemovedPatternMatches = Regex.Replace(targetTranslationCellValue, regexPattern, string.Empty, RegexOptions.Compiled);
                    string InputTransCellValueWithRemovedPatternMatches = Regex.Replace(inputTranslationValue, regexPattern, string.Empty, RegexOptions.Compiled);

                    //Если значение ячеек перевода без паттернов равны, идти дальше
                    if (TargetTransCellValueWithRemovedPatternMatches == InputTransCellValueWithRemovedPatternMatches) continue;

                    string TargetOrigCellValueWithRemovedPatternMatches = Regex.Replace(targetOriginalCellValue, regexPattern, string.Empty, RegexOptions.Compiled);
                    string InputOrigCellValueWithRemovedPatternMatches = Regex.Replace(inputOriginalValue, regexPattern, string.Empty, RegexOptions.Compiled);

                    //если поле перевода равно только что измененному во входной, без учета цифр
                    if (TargetOrigCellValueWithRemovedPatternMatches != InputOrigCellValueWithRemovedPatternMatches
                        || inputOriginalMatchesCount != targetMatchesCount
                        || !IsAllMatchesInIdenticalPlaces(inputOriginalMatches, targetMatches)
                        ) continue;

                    MatchCollection tm = reg.Matches(inputTranslationValue);

                    //количество совпадений должно быть равное для избежания исключений и прочих неверных замен
                    if (tm.Count != inputOriginalMatches.Count) continue;

                    int startindex;
                    int stringoverallength = 0;
                    int stringlength;
                    int stringoverallength0 = 0;
                    bool failed = false;

                    for (int m = 0; m < inputOriginalMatchesCount; m++)
                    {
                        //проверка, ЧТОБЫ СОВПАДЕНИЯ ОТЛИЧАЛИСЬ, Т.Е. НЕ МЕНЯТЬ ! НА ! И ? НА ?, ТОЛЬКО ! НА ? И 1 НА 2
                        if (inputOriginalMatches[m].Value == targetMatches[m].Value) continue;

                        //замена символа путем удаления на позиции и вставки нового:https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
                        startindex = tm[m].Index - stringoverallength + stringoverallength0;//отнять предыдущее число и заменить новым числом, для корректировки индекса

                        stringlength = tm[m].Value.Length;
                        stringoverallength += stringlength;//запомнить общую длину заменяемых символов, для коррекции индекса позиции для замены

                        inputTranslationValue = inputTranslationValue.Remove(startindex, stringlength).Insert(startindex, targetMatches[m].Value);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."

                        stringoverallength0 += targetMatches[m].Value.Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены

                    }
                    //только если ячейка пустая
                    targetTranslationCellValue = AppData.CurrentProject.FilesContent.Tables[targetTableIndex].Rows[targetRowIndex].Field<string>(translationColumnIndex);
                    if (!failed && (inputForceSetValue || string.IsNullOrEmpty(targetTranslationCellValue)))
                    {
                        targetRow[translationColumnIndex] = inputTranslationValue;
                    }
                }
            }

            AutoSame4SimilarStack.Remove(inputRowDataForStack);
        }

        public static void SetSameIfUseDups(DataTable inputTable, string inputOriginalValue, int inputRowIndex, bool inputForceSetValue, string inputTranslationValue)
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
                    if ((storedTableName.Key == inputTableName && storedRowIndex == inputRowIndex) || (!inputForceSetValue && (row[1] + "").Any()))
                    {
                        continue;
                    }

                    row[1] = inputTranslationValue;
                }
            }
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

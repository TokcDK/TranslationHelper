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
            Set(inputTableIndex: ProjectData.THFilesElementsDataset.Tables.IndexOf(table), inputRowIndex: table.Rows.IndexOf(dataRow), inputForceSetValue: inputForceSetValue);
        }

        static bool _autoSetSameTranslationForSimularIsBusy;
        static bool _forceSetValueFromStack;
        static int _inputTableIndexFromStack;
        static int _inputRowIndexFromStack;
        /// <summary>
        /// Table,Row
        /// </summary>
        static string _tableRowPair;
        /// <summary>
        /// List of coordinates
        /// </summary>
        static Dictionary<string, bool> _autoSetSameTranslationForSimularDataStack = new Dictionary<string, bool>();

        /// <summary>
        /// Set same translation values for rows with simular original
        /// </summary>
        /// <param name="inputTableIndex"></param>
        /// <param name="inputRowIndex"></param>
        /// <param name="inputOriginalColumnIndex"></param>
        /// <param name="inputForceSetValue"></param>
        public static void Set(int inputTableIndex, int inputRowIndex, bool inputForceSetValue = false)
        {
            if (!Properties.Settings.Default.ProjectIsOpened)
            {
                return;
            }

            if (inputTableIndex == -1
                || inputRowIndex == -1
                //|| inputOriginalColumnIndex == -1
                || ProjectData.THFilesElementsDataset == null
                || inputTableIndex > ProjectData.THFilesElementsDataset.Tables.Count - 1
                || inputRowIndex > ProjectData.THFilesElementsDataset.Tables[inputTableIndex].Rows.Count - 1
                || (ProjectData.THFilesElementsDataset.Tables[inputTableIndex].Rows[inputRowIndex][ProjectData.TranslationColumnIndex] + string.Empty).Length == 0)
            {
                return;
            }
            //if (Properties.Settings.Default.THAutoSetSameTranslationForSimularIsBusy)
            //{
            //    return;
            //}
            _autoSetSameTranslationForSimularDataStack.TryAdd(inputTableIndex + "|" + inputRowIndex, inputForceSetValue);

            while (!_autoSetSameTranslationForSimularIsBusy && _autoSetSameTranslationForSimularDataStack.Count > 0)
            {
                //если приложение закрылось
                if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                {
                    return;
                }

                _autoSetSameTranslationForSimularIsBusy = true;

                try
                {
                    {
                        //re-set input variables to prevent break of work while concurent execution
                        //forcevalue = ForceSetValue;
                        //iTableIndex = InputTableIndex;
                        //iRowIndex = InputRowIndex;
                        //iCellIndex = InputCellIndex;
                    }

                    //присвоить значения для обработки
                    _tableRowPair = _autoSetSameTranslationForSimularDataStack.ElementAt(0).Key;
                    _inputTableIndexFromStack = int.Parse(_tableRowPair.Split('|')[0], CultureInfo.InvariantCulture);
                    _inputRowIndexFromStack = int.Parse(_tableRowPair.Split('|')[1], CultureInfo.InvariantCulture);
                    _forceSetValueFromStack = _autoSetSameTranslationForSimularDataStack[_tableRowPair];

                    var inputTable = ProjectData.THFilesElementsDataset.Tables[_inputTableIndexFromStack];
                    var inputTableRow = inputTable.Rows[_inputRowIndexFromStack];
                    var inputTableRowOriginalCell = inputTableRow[ProjectData.OriginalColumnIndex];
                    int translationColumnIndex = ProjectData.TranslationColumnIndex;
                    var inputTableRowTranslationCell = inputTableRow[translationColumnIndex];

                    if (inputTableRowTranslationCell == null || string.IsNullOrEmpty(inputTableRowTranslationCell as string))
                    {
                        continue;
                    }

                    //Запускать сравнение только если ячейка имеет значение
                    //LogToFile("THFilesElementsDataset.Tables[tableind].Rows[rind][transcind]="+ THFilesElementsDataset.Tables[tableind].Rows[rind][transcind].ToString());

                    string regexPattern = GetStringSimularityRegexPattern();

                    Regex reg = new Regex(regexPattern); //reg равняется любым цифрам
                    string inputOriginalValue = FunctionsRomajiKana.THFixDigits(inputTableRowOriginalCell as string);
                    string inputTranslationValue = FunctionsRomajiKana.THFixDigits(inputTableRowTranslationCell as string);

                    //Было исключение OutOfRangeException когда в оригинале присутствовали совпадения для regex, а входной перевод был пустой или равен \r\n, тогда попытка получить индекс совпадения из оригинала заканчивалась исключением, т.к. никаких совпадений не было. Похоже на неверный перевод от онлайн сервиса
                    if (string.IsNullOrWhiteSpace(inputTranslationValue) || inputTranslationValue == Environment.NewLine)
                        return;

                    bool weUseDuplicates = false;
                    try
                    {
                        weUseDuplicates = !Properties.Settings.Default.DontLoadDuplicates && ProjectData.OriginalsTableRowCoordinats != null && ProjectData.OriginalsTableRowCoordinats[inputOriginalValue].Values.Count > 1;
                    }
                    catch
                    {

                    }
                    //set same value for duplicates from row coordinates list
                    if (weUseDuplicates)
                    {
                        var inputTableName = inputTable.TableName;
                        foreach (var storedTableName in ProjectData.OriginalsTableRowCoordinats[inputOriginalValue])
                        {
                            var table = ProjectData.THFilesElementsDataset.Tables[storedTableName.Key];
                            foreach (var storedRowIndex in storedTableName.Value)
                            {
                                var row = table.Rows[storedRowIndex];

                                //skip if same table\row as input or row translation is not empty
                                if ((storedTableName.Key == inputTableName && storedRowIndex == _inputRowIndexFromStack) || (!_forceSetValueFromStack && (row[1] + "").Length > 0))
                                {
                                    continue;
                                }

                                row[1] = inputTranslationValue;
                            }
                        }
                    }

                    //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
                    if (Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese" && Regex.Matches(inputTranslationValue, @"\d+").Count != Regex.Matches(inputOriginalValue, @"\d+").Count)
                    {
                        Properties.Settings.Default.THAutoSetSameTranslationForSimularIsBusy = false;
                        return;
                    }

                    MatchCollection inputOriginalMatches = reg.Matches(inputOriginalValue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
                    int inputOriginalMatchesCount = inputOriginalMatches.Count;

                    // Standart rows scan
                    int tablesCount = ProjectData.THFilesElementsDataset.Tables.Count;
                    for (int targetTableIndex = 0; targetTableIndex < tablesCount; targetTableIndex++) //количество файлов
                    {
                        var targetTable = ProjectData.THFilesElementsDataset.Tables[targetTableIndex];
                        var rowsCount = targetTable.Rows.Count;

                        var useSkipped = weUseDuplicates && ProjectData.OriginalsTableRowCoordinats[inputOriginalValue].ContainsKey(targetTable.TableName);

                        //LogToFile("Table "+Tindx+" proceed");
                        for (int targetRowIndex = 0; targetRowIndex < rowsCount; targetRowIndex++) //количество строк в каждом файле
                        {
                            //если приложение закрылось
                            if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                            {
                                return;
                            }

                            var targetRow = targetTable.Rows[targetRowIndex];
                            var targetOriginalCell = targetRow[ProjectData.OriginalColumnIndex];
                            var targetTranslationCell = targetRow[translationColumnIndex];
                            string targetOriginallCellString = targetOriginalCell as string;
                            string targetTranslationCellString = targetTranslationCell + string.Empty;
                            if (
                                (targetTableIndex == _inputTableIndexFromStack && targetRowIndex == _inputRowIndexFromStack) // skip input row index
                                ||
                                (!_forceSetValueFromStack && (
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
                            if (targetMatchesCount == 0) //если количество совпадений больше нуля(идти дальше), т.е. цифры были в поле untrans проверяемой на совпадение ячейки
                            {
                                continue;
                            }

                            string TargetTransCellValueWithRemovedPatternMatches = Regex.Replace(targetTranslationCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                            string InputTransCellValueWithRemovedPatternMatches = Regex.Replace(inputTranslationValue, regexPattern, string.Empty, RegexOptions.Compiled);

                            //Если значение ячеек перевода без паттернов равны, идти дальше
                            if (TargetTransCellValueWithRemovedPatternMatches == InputTransCellValueWithRemovedPatternMatches)
                            {
                                continue;
                            }

                            string TargetOrigCellValueWithRemovedPatternMatches = Regex.Replace(targetOriginallCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                            string InputOrigCellValueWithRemovedPatternMatches = Regex.Replace(inputOriginalValue, regexPattern, string.Empty, RegexOptions.Compiled);

                            //LogToFile("checkingorigcellvalue=\r\n" + checkingorigcellvalue + "\r\ninputorigcellvalue=\r\n" + inputorigcellvalue);
                            //если поле перевода равно только что измененному во входной, без учета цифр
                            if (TargetOrigCellValueWithRemovedPatternMatches != InputOrigCellValueWithRemovedPatternMatches
                                || inputOriginalMatchesCount != targetMatchesCount
                                || !IsAllMatchesInIdenticalPlaces(inputOriginalMatches, targetMatches)
                                )
                            {
                                continue;
                            }

                            {
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
                            }

                            MatchCollection tm = reg.Matches(inputTranslationValue);

                            //количество совпадений должно быть равное для избежания исключений и прочих неверных замен
                            if (tm.Count != inputOriginalMatches.Count)
                            {
                                continue;
                            }

                            int startindex;
                            int stringoverallength = 0;
                            int stringlength;
                            int stringoverallength0 = 0;
                            bool failed = false;
                            //LogToFile("arraysize=" + arraysize + ", wrapped inputresult" + inputresult);
                            for (int m = 0; m < inputOriginalMatchesCount; m++)
                            {
                                //проверка, ЧТОБЫ СОВПАДЕНИЯ ОТЛИЧАЛИСЬ, Т.Е. НЕ МЕНЯТЬ ! НА ! И ? НА ?, ТОЛЬКО ! НА ? И 1 НА 2
                                if (inputOriginalMatches[m].Value == targetMatches[m].Value)
                                {
                                    continue;
                                }

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
                            targetTranslationCell = ProjectData.THFilesElementsDataset.Tables[targetTableIndex].Rows[targetRowIndex][translationColumnIndex];
                            if (!failed && (_forceSetValueFromStack || targetTranslationCell == null || string.IsNullOrEmpty(targetTranslationCell as string)))
                            {
                                //ProjectData.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTransCellValue;
                                targetRow[translationColumnIndex] = inputTranslationValue;
                            }
                        }
                    }
                }
                catch
                {

                }

                _autoSetSameTranslationForSimularDataStack.TryRemove(_tableRowPair);
                _autoSetSameTranslationForSimularIsBusy = false;
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
            if (extractedTargetOriginal.Length > 0
                && extractedTargetOriginal[0] != targetOriginallCellString
                && extractedTargetOriginalIndexses.Count == extractedTargetOriginal.Length)
            {
                var extractedTargetTranslationIndexses = new List<int>();
                var extractedTargetTranslation = targetTranslationCellString.ExtractMulty(outIndexes: extractedTargetTranslationIndexses);
                if (extractedTargetTranslation.Length > 0
                    && extractedTargetTranslation[0] != targetTranslationCellString
                    && extractedTargetTranslation.Length == extractedTargetTranslationIndexses.Count
                    && extractedTargetTranslation.Length == extractedTargetOriginal.Length
                    )
                {
                    bool parsedWithMultyExtract = false;
                    for (int i = extractedTargetOriginalIndexses.Count - 1; i >= 0; i--)
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
                        targetRow[ProjectData.TranslationColumnIndex] = targetTranslationCellString;
                        return true;;
                    }
                }
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

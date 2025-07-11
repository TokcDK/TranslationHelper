﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    abstract class AutoSameForSimularBase : RowBase
    {
        public AutoSameForSimularBase()
        {
            _anyNumSymbolRegexPattern = GetStringSimularityRegexPattern();
            _anyNumSymbolRegex = new Regex(_anyNumSymbolRegexPattern, RegexOptions.Compiled); //reg равняется любым цифрам
            _simpleNumRegex = new Regex(@"\d+", RegexOptions.Compiled); //reg равняется любым цифрам, простое сравнение
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return !string.IsNullOrEmpty(rowData.Translation); // not empty original translation
        }

        protected virtual bool IsForce => false;
        protected override bool Apply(RowBaseRowData rowData)
        {
            if (Project == null) return false;
            if (Project.IsLoadingDB) return false;

            Set(rowData);

            return true;
        }

        private async void Set(RowBaseRowData rowData)
        {
            await Task.Run(() => Set(inputTableIndex: rowData.SelectedTableIndex, inputRowIndex: rowData.SelectedRowIndex, inputForceSetValue: IsForce)).ConfigureAwait(false);

        }

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
        public void Set(DataRow dataRow, bool inputForceSetValue = false)
        {
            var table = dataRow.Table;
            Set(inputTableIndex: Project.FilesContent.Tables.IndexOf(table), inputRowIndex: table.Rows.IndexOf(dataRow), inputForceSetValue: inputForceSetValue);
        }

        public HashSet<string> _autoSame4SimilarStack = new HashSet<string>();
        readonly string _anyNumSymbolRegexPattern;
        readonly Regex _anyNumSymbolRegex;
        readonly Regex _simpleNumRegex;

        /// <summary>
        /// Set same translation values for rows with simular original
        /// </summary>
        /// <param name="inputTableIndex"></param>
        /// <param name="inputRowIndex"></param>
        /// <param name="inputForceSetValue"></param>
        public void Set(int inputTableIndex, int inputRowIndex, bool inputForceSetValue = false)
        {
            if (!AppSettings.ProjectIsOpened) return;
            if (inputTableIndex == -1) return;
            if (inputRowIndex == -1) return;
            if (Project.FilesContent == null) return;
            if (inputTableIndex > Project.FilesContent.Tables.Count - 1) return;
            if (inputRowIndex > Project.FilesContent.Tables[inputTableIndex].Rows.Count - 1) return;

            var inputRowDataForStack = inputTableIndex + "|" + inputRowIndex + "|" + inputForceSetValue;

            if (_autoSame4SimilarStack.Contains(inputRowDataForStack)) return;

            _autoSame4SimilarStack.Add(inputRowDataForStack);

            Set4Similar(inputTableIndex, inputRowIndex, inputForceSetValue, inputRowDataForStack);
        }

        private void Set4Similar(int inputTableIndex, int inputRowIndex, bool inputForceSetValue, string inputRowDataForStack)
        {
            //присвоить значения для обработки
            var inputTable = Project.FilesContent.Tables[inputTableIndex];
            var inputTableRow = inputTable.Rows[inputRowIndex];
            var inputTableRowOriginalCellValue = inputTableRow.Field<string>(Project.OriginalColumnIndex);

            int translationColumnIndex = Project.TranslationColumnIndex;
            var inputTableRowTranslationCellValue = inputTableRow.Field<string>(translationColumnIndex);
            if (string.IsNullOrEmpty(inputTableRowTranslationCellValue) || inputTableRowTranslationCellValue.Equals(inputTableRowOriginalCellValue))
            {
                _autoSame4SimilarStack.Remove(inputRowDataForStack);
                return;
            }

            string inputOriginalValueFixedDigits = FunctionsRomajiKana.ReplaceDigits(inputTableRowOriginalCellValue);
            string inputTranslationValueFixedDigits = FunctionsRomajiKana.ReplaceDigits(inputTableRowTranslationCellValue);

            //Было исключение OutOfRangeException когда в оригинале присутствовали совпадения для regex, а входной перевод был пустой или равен \r\n, тогда попытка получить индекс совпадения из оригинала заканчивалась исключением, т.к. никаких совпадений не было. Похоже на неверный перевод от онлайн сервиса
            if (string.IsNullOrWhiteSpace(inputTranslationValueFixedDigits) || inputTranslationValueFixedDigits == Environment.NewLine)
            {
                _autoSame4SimilarStack.Remove(inputRowDataForStack);
                return;
            }

            bool weUseDuplicates = false;
            try
            {
                weUseDuplicates = Project != null && !Project.DontLoadDuplicates && Project.OriginalsTableRowCoordinates != null;
            }
            catch { }

            //set same value for duplicates from row coordinates list
            if (weUseDuplicates) SetSameIfUseDups(inputTable, inputTableRowOriginalCellValue, inputRowIndex, inputForceSetValue, inputTableRowTranslationCellValue);

            //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
            if (AppSettings.IsJapaneseSourceLanguage
                && _simpleNumRegex.Matches(inputTranslationValueFixedDigits).Count != _simpleNumRegex.Matches(inputOriginalValueFixedDigits).Count)
            {
                _autoSame4SimilarStack.Remove(inputRowDataForStack);
                return;
            }

            MatchCollection inputOriginalAnyNumRegexMatches = _anyNumSymbolRegex.Matches(inputOriginalValueFixedDigits); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
            int inputOriginalAnyNumRegexMatchesCount = inputOriginalAnyNumRegexMatches.Count;

            // Standart rows scan
            var coordinatesByInputOriginal = Project.OriginalsTableRowCoordinates[inputTableRowOriginalCellValue];
            var tables = Project.FilesContent.Tables;
            int tablesCount = tables.Count;

            // scan all files
            for (int targetTableIndex = 0; targetTableIndex < tablesCount; targetTableIndex++)
            {
                // app closing
                if (AppSettings.IsTranslationHelperWasClosed) break;

                var targetTable = tables[targetTableIndex];

                var rowsCount = targetTable.Rows.Count;

                // scan all rows of target file
                for (int targetRowIndex = 0; targetRowIndex < rowsCount; targetRowIndex++)
                {
                    // app closing
                    if (AppSettings.IsTranslationHelperWasClosed)
                    {
                        _autoSame4SimilarStack.Remove(inputRowDataForStack);
                        return;
                    }

                    // skip when input and target row is same
                    if (targetTableIndex == inputTableIndex && targetRowIndex == inputRowIndex) continue;

                    // skip row was already set by coordinates 
                    if (weUseDuplicates
                        && coordinatesByInputOriginal.TryGetValue(targetTable.TableName, out var rowCoordinates)
                        && rowCoordinates.Contains(targetRowIndex)
                        )
                    {
                        continue;
                    }

                    var targetRow = targetTable.Rows[targetRowIndex];
                    var targetOriginalCellValue = targetRow.Field<string>(Project.OriginalColumnIndex);
                    var targetTranslationCellValue = targetRow.Field<string>(translationColumnIndex);

                    if (!inputForceSetValue && targetOriginalCellValue.Equals(targetTranslationCellValue))
                    {
                        // when is not force set
                        // skip equal original and translation case
                        continue;
                    }

                    bool isEmptyTargetTranslation = string.IsNullOrEmpty(targetTranslationCellValue);

                    // set to translatino when original is same and translation is null or empty, and continue
                    if ((isEmptyTargetTranslation || inputForceSetValue) && !weUseDuplicates && targetOriginalCellValue.Equals(inputOriginalValueFixedDigits))
                    {
                        targetRow[translationColumnIndex] = inputTableRowTranslationCellValue;

                        continue;
                    }

                    // below it parse only not empty translation
                    if (isEmptyTargetTranslation) continue;

                    // skip when was parsed with multi extraction
                    if (ParsedWithExtractMulti(targetOriginalCellValue, targetTranslationCellValue, inputTableRowOriginalCellValue, inputTableRowTranslationCellValue, targetRow))
                    {
                        continue;
                    }

                    // no digits in input original, no sense for code below
                    if (inputOriginalAnyNumRegexMatchesCount == 0) continue;

                    MatchCollection targetTranslationMatches = _anyNumSymbolRegex.Matches(targetOriginalCellValue); //mc0 равно значениям цифр ячейки под номером y в файле i
                    int targetMatchesCount = targetTranslationMatches.Count;
                    if (targetMatchesCount == 0) continue; //если количество совпадений больше нуля(идти дальше), т.е. цифры были в поле untrans проверяемой на совпадение ячейки

                    string targetTranslationValueWithRemovedPatternMatches = _anyNumSymbolRegex.Replace(targetTranslationCellValue, string.Empty);
                    string inputTransCellValueWithRemovedPatternMatches = _anyNumSymbolRegex.Replace(inputTranslationValueFixedDigits, string.Empty);

                    //Если значение ячеек перевода без паттернов равны, идти дальше
                    if (targetTranslationValueWithRemovedPatternMatches == inputTransCellValueWithRemovedPatternMatches) continue;

                    string targetOrigCellValueWithRemovedPatternMatches = _anyNumSymbolRegex.Replace(targetOriginalCellValue, string.Empty);
                    string inputOrigCellValueWithRemovedPatternMatches = _anyNumSymbolRegex.Replace(inputOriginalValueFixedDigits, string.Empty);

                    //если поле перевода равно только что измененному во входной, без учета цифр
                    if (targetOrigCellValueWithRemovedPatternMatches != inputOrigCellValueWithRemovedPatternMatches
                        || inputOriginalAnyNumRegexMatchesCount != targetMatchesCount
                        || !IsAllMatchesInIdenticalPlaces(inputOriginalAnyNumRegexMatches, targetTranslationMatches)
                        ) continue;

                    MatchCollection inputTranslationMatches = _anyNumSymbolRegex.Matches(inputTranslationValueFixedDigits);

                    //количество совпадений должно быть равное для избежания исключений и прочих неверных замен
                    if (inputTranslationMatches.Count != inputOriginalAnyNumRegexMatches.Count) continue;

                    var newInputTranslationValue = inputTranslationValueFixedDigits;
                    for (int m = inputTranslationMatches.Count - 1; m >= 0; m--)
                    {
                        var inputTranslationMatch = inputTranslationMatches[m];
                        var targetTransationMatch = targetTranslationMatches[m];

                        //проверка, ЧТОБЫ СОВПАДЕНИЯ ОТЛИЧАЛИСЬ, Т.Е. НЕ МЕНЯТЬ ! НА ! И ? НА ?, ТОЛЬКО ! НА ? И 1 НА 2
                        if (inputTranslationMatch.Value == targetTransationMatch.Value) continue;

                        newInputTranslationValue = newInputTranslationValue
                            .Remove(inputTranslationMatch.Index, inputTranslationMatch.Length)
                            .Insert(inputTranslationMatch.Index, targetTransationMatch.Value);
                    }

                    //только если ячейка пустая
                    if (!inputTranslationValueFixedDigits.Equals(newInputTranslationValue))
                    {
                        targetRow[translationColumnIndex] = newInputTranslationValue;
                    }
                }
            }

            _autoSame4SimilarStack.Remove(inputRowDataForStack);
        }

        public void SetSameIfUseDups(DataTable inputTable, string inputOriginalValue, int inputRowIndex, bool inputForceSetValue, string inputTranslationValue)
        {
            if (!Project.OriginalsTableRowCoordinates.TryGetValue(inputOriginalValue, out var storedTableNames)) return;

            var inputTableName = inputTable.TableName;
            foreach (var storedTableName in storedTableNames)
            {
                if (!Project.FilesContent.Tables.Contains(storedTableName.Key)) continue;

                var table = Project.FilesContent.Tables[storedTableName.Key];

                var rowsCount = table.Rows.Count;
                foreach (var storedRowIndex in storedTableName.Value)
                {
                    if (rowsCount <= storedRowIndex) continue;

                    var row = table.Rows[storedRowIndex];

                    //skip if same table\row as input or row translation is not empty
                    if (storedTableName.Key == inputTableName && storedRowIndex == inputRowIndex) continue;
                    if (!inputForceSetValue 
                        && !string.IsNullOrEmpty(row.Field<string>(THSettings.TranslationColumnName)))
                    {
                        continue;
                    }

                    AppData.Main.Invoke((Action)(() => row.SetField(Project.TranslationColumnIndex, inputTranslationValue)));
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
        private bool ParsedWithExtractMulti(string targetOriginallCellString, string targetTranslationCellString, string inputOriginalValue, string inputTranslationValue, DataRow targetRow)
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
            string[] extractedTargetTranslation = targetTranslationCellString.ExtractMulty(outIndexes: extractedTargetTranslationIndexses);
            if (extractedTargetTranslation == null) return false;

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
                targetRow[Project.TranslationColumnIndex] = targetTranslationCellString;
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

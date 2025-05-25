using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Data.Interfaces;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    /// <summary>
    /// Handles online translation of rows, inheriting from RowBase.
    /// Manages buffering, translation, and writing back to rows.
    /// </summary>
    partial class OnlineTranslateTEST : RowBase
    {
        #region Fields

        /// <summary>
        /// Список информации о переводах строк (оригинал -> информация о переводе).
        /// </summary>
        public Dictionary<string, RowTranslationInfo> TranslationDataList = new Dictionary<string, RowTranslationInfo>();

        /// <summary>
        /// Текущий используемый переводчик.
        /// </summary>
        private readonly ITranslator _translator;

        /// <summary>
        /// Кэш переводов для ускорения повторных запросов.
        /// </summary>
        private readonly ITranslationCache _cache;

        /// <summary>
        /// Буфер для хранения данных переводимых строк.
        /// </summary>
        private readonly ConcurrentDictionary<int, TranslationData> _buffer;

        /// <summary>
        /// Общая длина текста, ожидающего перевода.
        /// </summary>
        private int TranslationTextLength { get; set; }

        /// <summary>
        /// Максимальная длина текста для одного запроса перевода.
        /// </summary>
        private static int MaxTranslationTextLength => 1000;

        /// <summary>
        /// Максимальное количество строк в буфере.
        /// </summary>
        private const int BufferMaxRows = 300;

        /// <summary>
        /// Объект блокировки для потокобезопасной работы с буфером.
        /// </summary>
        private readonly object _bufferLock = new object();

        /// <summary>
        /// Флаг, указывающий, что все БД были загружены для всех переводов.
        /// </summary>
        private bool _allDbLoaded4All;

        /// <summary>
        /// Имя последней обрабатываемой таблицы.
        /// </summary>
        private string _lastTableName = string.Empty;

        /// <summary>
        /// Индекс колонки с оригинальным текстом.
        /// </summary>
        private readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;

        /// <summary>
        /// Индекс колонки с переводом.
        /// </summary>
        private readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;

        /// <summary>
        /// Класс для применения "жёстких" фиксов к строкам.
        /// </summary>
        private readonly StringChangerBase _hardFixes = new AllHardFixesChanger();

        /// <summary>
        /// Класс для применения фиксов к ячейкам.
        /// </summary>
        private readonly StringChangerBase _fixCells = new FixCellsChanger();

        /// <summary>
        /// Регулярное выражение для определения типа списка замен.
        /// </summary>
        private static readonly Regex _replacerListTypeRegex = new Regex(@"^\$[0-9]+(,\$[0-9]+)+$", RegexOptions.Compiled);

        /// <summary>
        /// Регулярное выражение для определения необходимости вставки текста при одном совпадении.
        /// </summary>
        private static readonly Regex _oneMatchNeedInsertTextRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Compiled);

        /// <summary>
        /// Регулярное выражение для поиска маркеров групп замен.
        /// </summary>
        private static readonly Regex _groupReplacerMarkerRegex = new Regex(@"\$[0-9]+", RegexOptions.Compiled);

        #endregion

        #region Properties

        /// <summary>
        /// Имя текущего переводчика.
        /// </summary>
        public override string Name => T._("TranslatorTEST");

        /// <summary>
        /// Флаг: использовать ли параллельную обработку таблиц.
        /// </summary>
        protected override bool IsParallelTables => false;

        /// <summary>
        /// Флаг: использовать ли параллельную обработку строк.
        /// </summary>
        protected override bool IsParallelRows => false;

        /// <summary>
        /// Флаг: переводить ли все строки.
        /// </summary>
        protected virtual bool IsTranslateAll => true;

        #endregion

        #region Constructors

        /// <summary>
        /// Инициализирует новый экземпляр OnlineTranslateTEST с зависимостями.
        /// </summary>
        /// <param name="translator">Экземпляр переводчика (опционально).</param>
        /// <param name="cache">Экземпляр кэша переводов (опционально).</param>
        public OnlineTranslateTEST(ITranslator translator = null, ITranslationCache cache = null)
        {
            Logger.Debug("Инициализация OnlineTranslateTEST");
            _translator = translator ?? new GoogleTranslator(THSettings.SourceLanguageCode, THSettings.TargetLanguageCode);
            _cache = cache ?? new TranslationCache();
            _buffer = new ConcurrentDictionary<int, TranslationData>();
        }

        #endregion

        #region RowBase Overrides

        /// <summary>
        /// Проверяет, подходит ли строка для перевода.
        /// </summary>
        /// <param name="rowData">Данные строки.</param>
        /// <returns>True, если строка валидна для перевода.</returns>
        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            Logger.Debug($"Проверка валидности строки для перевода: RowIndex={rowData?.SelectedRowIndex}");
            return !AppSettings.InterruptTtanslation && base.IsValidRow(rowData)
                && (string.IsNullOrEmpty(rowData.Translation)
                || rowData.Original.HasAnyTranslationLineValidAndEqualSameOrigLine(rowData.Translation));
        }

        /// <summary>
        /// Инициализирует ресурсы для перевода, например, загружает БД при необходимости.
        /// </summary>
        protected async override Task ActionsInit()
        {
            Logger.Info("Инициализация действий для онлайн-перевода.");
            await base.ActionsInit();

            if (_allDbLoaded4All || !IsAll || !AppSettings.UseAllDBFilesForOnlineTranslationForAll) return;

            if (!AppSettings.EnableTranslationCache)
            {
                Logger.Warn("Кэш переводов отключён, но включена загрузка всех БД.");
                var result = MessageBox.Show(T._("Translation cache disabled but load all DB enabled. While all DB loading cache can be enabled in settings. Load all DB?"),
                    T._("Translation cache disabled"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            if (!IsTranslateAll && AppSettings.EnableTranslationCache)
            {
                Logger.Info("Запрос на загрузку всех существующих файлов БД.");
                var result = MessageBox.Show(T._("Load all exist database files?"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            Logger.Info(T._("Get all DB"));
            await FunctionsDBFile.MergeAllDBtoOne().ConfigureAwait(false);
            _allDbLoaded4All = true;
        }

        /// <summary>
        /// Финализирует перевод: обрабатывает оставшийся буфер и очищает ресурсы.
        /// </summary>
        protected override async Task ActionsFinalize()
        {
            Logger.Info("Финализация перевода.");
            await base.ActionsFinalize();

            if (!_buffer.IsEmpty)
            {
                Logger.Debug("В буфере остались необработанные строки, выполняется перевод.");
                TranslateStrings();
            }
            _cache.Dispose();
            if (AppSettings.InterruptTtanslation)
            {
                Logger.Warn("Перевод был прерван пользователем.");
                AppSettings.InterruptTtanslation = false;
            }
            Logger.Info(T._("Translation complete"));
        }

        /// <summary>
        /// Добавляет строки в буфер для пакетного перевода.
        /// </summary>
        /// <param name="rowData">Данные строки.</param>
        /// <returns>True, если строка успешно добавлена в буфер.</returns>
        protected override bool Apply(RowBaseRowData rowData)
        {
            if (_lastTableName != rowData.SelectedTable.TableName)
            {
                _lastTableName = rowData.SelectedTable.TableName;
                Logger.Info(T._("Translate {0}"), _lastTableName);
            }

            SetRowLinesToBuffer(rowData);
            return true;
        }

        #endregion

        #region Buffering Methods

        /// <summary>
        /// Проверяет, достигнут ли лимит буфера или длины текста.
        /// </summary>
        /// <returns>True, если достигнут лимит.</returns>
        private bool IsMax()
        {
            bool isMax = TranslationTextLength >= MaxTranslationTextLength || _buffer.Count >= BufferMaxRows;
            if (isMax)
                Logger.Debug($"Достигнут лимит буфера: TranslationTextLength={TranslationTextLength}, BufferCount={_buffer.Count}");
            return isMax;
        }

        /// <summary>
        /// Буферизует строки для пакетного перевода.
        /// </summary>
        /// <param name="rowData">Данные строки.</param>
        private void SetRowLinesToBuffer(RowBaseRowData rowData)
        {
            if (rowData == null)
            {
                Logger.Error("rowData is null при добавлении строки в буфер.");
                throw new ArgumentNullException(nameof(rowData));
            }
            if (rowData.SelectedTable == null)
            {
                Logger.Error("SelectedTable is null при добавлении строки в буфер.");
                throw new InvalidOperationException("SelectedTable is null");
            }
            if (rowData.Original == null)
            {
                Logger.Error("Original text is null при добавлении строки в буфер.");
                throw new InvalidOperationException("Original text is null");
            }

            var tableData = _buffer.GetOrAdd(rowData.SelectedTableIndex, _ => new TranslationData(rowData));
            var rowDataEntry = tableData.Rows.GetOrAdd(rowData.SelectedRowIndex, _ => new RowTranslationData(rowData));

            int originalTextLength = TranslationTextLength;
            int lineIndex = -1;

            foreach (var line in rowData.Original.SplitToLines())
            {
                lineIndex++;
                var lineData = rowDataEntry.Lines.FirstOrDefault(l => l.LineIndex == lineIndex) ?? new LineTranslationData(lineIndex, line);
                if (!rowDataEntry.Lines.Contains(lineData)) rowDataEntry.Lines.Add(lineData);

                if (lineData.IsTranslated) continue;

                lineData.RegexExtractionData = new ExtractRegexInfo(line);
                bool isExtracted = lineData.RegexExtractionData.ExtractedValuesList.Count > 0;

                if (!isExtracted && !line.IsValidForTranslation())
                {
                    Logger.Debug($"Строка исключена из перевода: {line}");
                    lineData.IsExcluded = true;
                    lineData.Translation = line;
                    continue;
                }

                GetFromExtracted(lineData.RegexExtractionData, out int skippedCount);
                if (isExtracted && skippedCount == lineData.RegexExtractionData.ExtractedValuesList.Count) continue;

                if (!isExtracted && CheckInCache(line, lineData)) continue;

                if (IsMax())
                {
                    Logger.Info("Буфер заполнен, выполняется перевод.");
                    TranslateStrings();
                }
                if (AppSettings.InterruptTtanslation) return;
            }

            rowDataEntry.IsAllLinesAdded = true;

            if (originalTextLength == TranslationTextLength && WriteRowData(rowDataEntry, tableData.TableIndex))
            {
                tableData.Rows.TryRemove(rowDataEntry.RowIndex, out _);
                if (tableData.Rows.IsEmpty) _buffer.TryRemove(tableData.TableIndex, out _);
            }
        }

        /// <summary>
        /// Проверяет кэш для строки и обновляет данные строки, если найдено.
        /// </summary>
        /// <param name="line">Оригинальная строка.</param>
        /// <param name="lineData">Данные строки.</param>
        /// <returns>True, если перевод найден в кэше.</returns>
        private bool CheckInCache(string line, LineTranslationData lineData)
        {
            var cached = _cache.TryGetValue(line);
            if (!string.IsNullOrEmpty(cached))
            {
                Logger.Debug($"Перевод найден в кэше: {line} -> {cached}");
                lineData.Translation = cached;
                return true;
            }
            TranslationTextLength += line.Length;
            return false;
        }

        /// <summary>
        /// Обрабатывает извлечённые значения, обновляя длину или используя кэш.
        /// </summary>
        /// <param name="extractData">Данные извлечения.</param>
        /// <param name="skippedValuesCount">Количество пропущенных значений.</param>
        private void GetFromExtracted(ExtractRegexInfo extractData, out int skippedValuesCount)
        {
            skippedValuesCount = 0;
            foreach (var value in extractData.ExtractedValuesList)
            {
                var cached = _cache.TryGetValue(value.Original);
                if (!string.IsNullOrEmpty(cached))
                {
                    Logger.Debug($"Извлечённое значение найдено в кэше: {value.Original} -> {cached}");
                    value.Translation = cached;
                    skippedValuesCount++;
                }
                else if (value.Original.IsSoundsText() || !value.Original.IsValidForTranslation())
                {
                    Logger.Debug($"Извлечённое значение исключено из перевода: {value.Original}");
                    value.Translation = value.Original;
                    skippedValuesCount++;
                }
                else
                {
                    TranslationTextLength += value.Original.Length;
                }
            }
        }

        #endregion

        #region Translation Methods

        /// <summary>
        /// Переводит строки из буфера и обновляет строки.
        /// </summary>
        private void TranslateStrings()
        {
            Logger.Info("Начало пакетного перевода строк из буфера.");
            var originals = GetOriginals();
            if (originals.Length == 0 && _buffer.IsEmpty)
            {
                Logger.Debug("Нет строк для перевода.");
                return;
            }

            var translated = TranslateOriginals(originals);
            SetTranslationsToBuffer(originals, translated);
            SetBufferToRows();
            TranslationTextLength = 0;
            if (originals.Length > 1) _cache.Write();
        }

        /// <summary>
        /// Получает уникальные оригиналы из буфера для перевода.
        /// </summary>
        /// <returns>Массив оригинальных строк.</returns>
        private string[] GetOriginals()
        {
            var originals = new ConcurrentDictionary<string, bool>();
            Parallel.ForEach(EnumerateBufferedLinesData(), line =>
            {
                foreach (var ot in EnumerateOriginalTranslation(line))
                    originals.TryAdd(ot.Original, true);
            });
            Logger.Debug($"Получено {originals.Count} уникальных строк для перевода.");
            return originals.Keys.ToArray();
        }

        /// <summary>
        /// Перечисляет оригинальные тексты, требующие перевода.
        /// </summary>
        /// <param name="lineData">Данные строки.</param>
        /// <returns>Перечисление объектов для перевода.</returns>
        private static IEnumerable<IOriginalTranslationUser> EnumerateOriginalTranslation(LineTranslationData lineData)
        {
            if (lineData.RegexExtractionData.ExtractedValuesList.Count > 0)
            {
                foreach (var value in lineData.RegexExtractionData.ExtractedValuesList
                    .Where(v => !(v.IsExcluded = !v.Original.IsValidForTranslation()) && !v.IsTranslated))
                    yield return value;
            }
            else if (!lineData.IsTranslated && !(lineData.IsExcluded = !lineData.Original.IsValidForTranslation()))
            {
                yield return lineData;
            }
        }

        /// <summary>
        /// Перечисляет все данные строк из буфера.
        /// </summary>
        /// <returns>Перечисление данных строк.</returns>
        private IEnumerable<LineTranslationData> EnumerateBufferedLinesData()
        {
            foreach (var table in _buffer.Values)
                foreach (var row in table.Rows.Values)
                    foreach (var line in row.Lines)
                        yield return line;
        }

        /// <summary>
        /// Переводит оригиналы с применением пред- и пост-обработки и fallback.
        /// </summary>
        /// <param name="originals">Оригинальные строки.</param>
        /// <returns>Переведённые строки.</returns>
        private string[] TranslateOriginals(string[] originals)
        {
            if (originals == null || originals.Length == 0) return Array.Empty<string>();

            var preApplied = ApplyProjectPretranslationAction(originals);
            var translated = TranslateWithFallback(preApplied);
            return ApplyProjectPostTranslationAction(originals, translated);
        }

        /// <summary>
        /// Переводит тексты с fallback на случай ошибок.
        /// </summary>
        /// <param name="texts">Тексты для перевода.</param>
        /// <returns>Переведённые строки.</returns>
        private string[] TranslateWithFallback(string[] texts)
        {
            try
            {
                Logger.Debug("Выполняется основной перевод массива строк.");
                return _translator.Translate(texts);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Ошибка пакетного перевода: {ex}");
                var translated = new List<string>();
                const int batchSize = 4;
                for (int i = 0; i < texts.Length; i += batchSize)
                {
                    var batch = texts.Skip(i).Take(batchSize).ToArray();
                    try
                    {
                        translated.AddRange(_translator.Translate(batch));
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn($"Ошибка перевода батча: {ex2}");
                        foreach (var text in batch)
                        {
                            try
                            {
                                translated.Add(_translator.Translate(text));
                            }
                            catch (Exception ex3)
                            {
                                Logger.Error($"Ошибка перевода строки: {text}", ex3);
                                translated.Add(string.Empty);
                            }
                        }
                    }
                }
                return translated.ToArray();
            }
        }

        /// <summary>
        /// Применяет проектные предобработки к оригинальным строкам.
        /// </summary>
        /// <param name="originalLines">Оригинальные строки.</param>
        /// <returns>Строки после предобработки.</returns>
        private static string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            var preTranslated = new string[originalLines.Length];
            Array.Copy(originalLines, preTranslated, originalLines.Length);
            for (int i = 0; i < originalLines.Length; i++)
            {
                var result = AppData.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], null);
                if (!string.IsNullOrEmpty(result)) preTranslated[i] = result;
            }
            return preTranslated;
        }

        /// <summary>
        /// Применяет проектные постобработки к переведённым строкам.
        /// </summary>
        /// <param name="originalLines">Оригинальные строки.</param>
        /// <param name="translatedLines">Переведённые строки.</param>
        /// <returns>Строки после постобработки.</returns>
        private static string[] ApplyProjectPostTranslationAction(string[] originalLines, string[] translatedLines)
        {
            for (int i = 0; i < translatedLines.Length; i++)
            {
                var result = AppData.CurrentProject.OnlineTranslationProjectSpecificPostTranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(result) && result != translatedLines[i]) translatedLines[i] = result;
            }
            return translatedLines;
        }

        /// <summary>
        /// Присваивает переводы обратно в буфер.
        /// </summary>
        /// <param name="originals">Оригинальные строки.</param>
        /// <param name="translated">Переведённые строки.</param>
        private void SetTranslationsToBuffer(string[] originals, string[] translated)
        {
            if (originals == null || translated == null || originals.Length != translated.Length) return;

            var translations = originals.Zip(translated, (o, t) => new { Original = o, Translation = t })
                .ToDictionary(x => x.Original, x => x.Translation);

            Parallel.ForEach(EnumerateBufferedLinesData(), line =>
            {
                foreach (var ot in EnumerateOriginalTranslation(line))
                    if (translations.TryGetValue(ot.Original, out var trans))
                    {
                        ot.Translation = trans;
                        _cache.TryAdd(ot.Original, trans);
                    }
            });
        }

        /// <summary>
        /// Записывает переводы из буфера обратно в строки.
        /// </summary>
        private void SetBufferToRows()
        {
            var tablesToRemove = new List<int>();
            foreach (var table in _buffer)
            {
                var rowsToRemove = new List<int>();
                foreach (var row in table.Value.Rows)
                {
                    if (WriteRowData(row.Value, table.Key))
                        rowsToRemove.Add(row.Key);
                }
                foreach (var rowIndex in rowsToRemove)
                    table.Value.Rows.TryRemove(rowIndex, out _);
                if (table.Value.Rows.IsEmpty)
                    tablesToRemove.Add(table.Key);
            }
            foreach (var tableIndex in tablesToRemove)
                _buffer.TryRemove(tableIndex, out _);
        }

        #endregion

        #region Row Writing and Merging

        /// <summary>
        /// Записывает переведённые данные обратно в строку, если перевод завершён.
        /// </summary>
        /// <param name="rowData">Данные строки.</param>
        /// <param name="tableIndex">Индекс таблицы.</param>
        /// <returns>True, если запись выполнена.</returns>
        private bool WriteRowData(RowTranslationData rowData, int tableIndex)
        {
            if (!rowData.IsAllLinesAdded) return false;

            var row = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rowData.RowIndex];
            var original = rowData.Row.Original;
            var translation = rowData.Row.Translation;
            var ignoreEqual = AppSettings.IgnoreOrigEqualTransLines;

            if (ignoreEqual && string.Equals(translation, original) && !rowData.Lines.Any(l => l.Original != l.Translation)) return false;

            var transNotEmptyAndNotEqual = !string.IsNullOrEmpty(translation) && !string.Equals(translation, original);
            if (transNotEmptyAndNotEqual && !original.HasAnyTranslationLineValidAndEqualSameOrigLine(translation, false)) return false;

            rowData.Row.Translation = string.Join(Environment.NewLine, EnumerateNewLines(rowData.Lines));
            Logger.Debug($"Строка записана: RowIndex={rowData.RowIndex}, TableIndex={tableIndex}");
            return true;
        }

        /// <summary>
        /// Генерирует переведённые строки для записи обратно.
        /// </summary>
        /// <param name="lines">Список данных строк.</param>
        /// <returns>Перечисление переведённых строк.</returns>
        private IEnumerable<string> EnumerateNewLines(List<LineTranslationData> lines)
        {
            foreach (var line in lines)
            {
                if (line.RegexExtractionData.ExtractedValuesList.Count > 0)
                    yield return MergeExtracted(line);
                else if (line.IsTranslated)
                    yield return string.IsNullOrEmpty(line.Translation) ? line.Original : ApplyFixes(line);
            }
        }

        /// <summary>
        /// Применяет фиксы к переводу строки.
        /// </summary>
        /// <param name="line">Данные строки.</param>
        /// <returns>Исправленный перевод.</returns>
        private string ApplyFixes(LineTranslationData line) => ApplyFixes(line.Original, line.Translation);

        /// <summary>
        /// Применяет фиксы к переводу.
        /// </summary>
        /// <param name="original">Оригинал.</param>
        /// <param name="translation">Перевод.</param>
        /// <returns>Исправленный перевод.</returns>
        private string ApplyFixes(string original, string translation)
        {
            var text = _hardFixes.Change(translation, original);
            return _fixCells.Change(text, original);
        }

        /// <summary>
        /// Объединяет извлечённые значения обратно в строку.
        /// </summary>
        /// <param name="lineData">Данные строки.</param>
        /// <returns>Объединённая строка.</returns>
        private string MergeExtracted(LineTranslationData lineData)
        {
            var extractData = lineData.RegexExtractionData;
            if (extractData.ExtractedValuesList.Count == 0) return lineData.Translation;

            var replacerType = DetermineReplacerType(extractData.Replacer);
            switch (replacerType)
            {
                case TranslationRegexExtractType.ReplaceOne:
                    return MergeReplaceOne(lineData);
                case TranslationRegexExtractType.ReplaceList:
                    return MergeReplaceList(lineData);
                case TranslationRegexExtractType.Replacer:
                    return MergeReplacer(lineData);
                default:
                    Logger.Error("Неизвестный тип replacer при объединении строки.");
                    throw new InvalidOperationException("Unknown replacer type");
            }
        }

        /// <summary>
        /// Определяет тип replacer для объединения.
        /// </summary>
        /// <param name="replacer">Строка replacer.</param>
        /// <returns>Тип replacer.</returns>
        private static TranslationRegexExtractType DetermineReplacerType(string replacer)
        {
            var trimmed = replacer.Trim();
            if (_oneMatchNeedInsertTextRegex.IsMatch(trimmed)) return TranslationRegexExtractType.ReplaceOne;
            if (_replacerListTypeRegex.IsMatch(trimmed)) return TranslationRegexExtractType.ReplaceList;
            return TranslationRegexExtractType.Replacer;
        }

        /// <summary>
        /// Объединяет с помощью стратегии ReplaceOne: заменяет всё совпадение переводом первой группы.
        /// </summary>
        /// <param name="lineData">Данные строки.</param>
        /// <returns>Объединённая строка.</returns>
        private string MergeReplaceOne(LineTranslationData lineData)
        {
            var sb = new StringBuilder(lineData.Original);
            var value = lineData.RegexExtractionData.ExtractedValuesList[0];
            foreach (var group in value.MatchGroups)
            {
                if (string.IsNullOrWhiteSpace(group.Value) || string.IsNullOrEmpty(value.Translation)) continue;
                var text = ApplyFixes(group.Value, value.Translation);
                sb.Replace(group.Value, text, group.Index, group.Length);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Объединяет с помощью стратегии ReplaceList: заменяет каждую группу её переводом.
        /// </summary>
        /// <param name="lineData">Данные строки.</param>
        /// <returns>Объединённая строка.</returns>
        private string MergeReplaceList(LineTranslationData lineData)
        {
            var sb = new StringBuilder(lineData.Original);
            foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
            {
                foreach (var group in value.MatchGroups)
                {
                    if (string.IsNullOrWhiteSpace(group.Value) || string.IsNullOrEmpty(value.Translation)) continue;
                    var text = ApplyFixes(group.Value, value.Translation);
                    sb.Replace(group.Value, text, group.Index, group.Length);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Объединяет с помощью стратегии Replacer: использует кастомный replacer с плейсхолдерами.
        /// </summary>
        /// <param name="lineData">Данные строки.</param>
        /// <returns>Объединённая строка.</returns>
        private string MergeReplacer(LineTranslationData lineData)
        {
            var sb = new StringBuilder(lineData.RegexExtractionData.Replacer);
            var matches = _groupReplacerMarkerRegex.Matches(sb.ToString());
            foreach (Match match in matches.Cast<Match>().Reverse())
                sb.Replace(match.Value, $"%{match.Value}%", match.Index, match.Length);

            var reversedExtractedValuesList = new List<ExtractRegexValueInfo>(lineData.RegexExtractionData.ExtractedValuesList);
            reversedExtractedValuesList.Reverse();

            foreach (var value in lineData.RegexExtractionData.ExtractedValuesList)
            {
                foreach (var group in value.MatchGroups)
                {
                    var text = value.Translation ?? group.Value;
                    if (text != group.Value) text = ApplyFixes(group.Value, text);
                    sb.Replace($"%${group.Name}%", text);
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// Буфер для строк, ожидающих перевода.
        /// </summary>
        internal class Buffer : IOriginalTranslationUser
        {
            /// <summary>
            /// Оригинальный текст.
            /// </summary>
            public string Original { get; }

            /// <summary>
            /// Переведённый текст.
            /// </summary>
            public string Translation { get; set; }

            /// <summary>
            /// Флаг: было ли значение извлечено.
            /// </summary>
            internal bool IsExtracted { get; }

            internal Buffer(string original, string translation, bool isExtracted)
            {
                Original = original;
                Translation = translation;
                IsExtracted = isExtracted;
            }
        }

        /// <summary>
        /// Хранит данные перевода для таблицы.
        /// </summary>
        internal class TranslationData
        {
            /// <summary>
            /// Индекс таблицы.
            /// </summary>
            internal int TableIndex => Row.SelectedTableIndex;

            /// <summary>
            /// Словарь строк по индексу.
            /// </summary>
            internal ConcurrentDictionary<int, RowTranslationData> Rows = new ConcurrentDictionary<int, RowTranslationData>();

            /// <summary>
            /// Данные строки.
            /// </summary>
            internal RowBaseRowData Row { get; }

            public TranslationData(RowBaseRowData rowData) => Row = rowData;
        }

        /// <summary>
        /// Хранит данные перевода для строки.
        /// </summary>
        internal class RowTranslationData
        {
            /// <summary>
            /// Флаг: все ли строки добавлены.
            /// </summary>
            internal bool IsAllLinesAdded = false;

            /// <summary>
            /// Индекс строки.
            /// </summary>
            internal int RowIndex => Row.SelectedRowIndex;

            /// <summary>
            /// Список данных по строкам.
            /// </summary>
            internal List<LineTranslationData> Lines = new List<LineTranslationData>();

            /// <summary>
            /// Данные строки.
            /// </summary>
            internal RowBaseRowData Row { get; }

            public RowTranslationData(RowBaseRowData rowData) => Row = rowData;
        }

        /// <summary>
        /// Хранит данные перевода для одной строки.
        /// </summary>
        internal class LineTranslationData : IOriginalTranslationUser
        {
            /// <summary>
            /// Индекс строки.
            /// </summary>
            internal readonly int LineIndex;

            /// <summary>
            /// Оригинальный текст.
            /// </summary>
            public string Original { get; }

            /// <summary>
            /// Переведённый текст.
            /// </summary>
            public string Translation { get; set; }

            /// <summary>
            /// Данные извлечения регулярным выражением.
            /// </summary>
            internal ExtractRegexInfo RegexExtractionData;

            /// <summary>
            /// Флаг: исключена ли строка из перевода.
            /// </summary>
            internal bool IsExcluded = false;

            public LineTranslationData(int lineIndex, string originalText)
            {
                LineIndex = lineIndex;
                Original = originalText;
                RegexExtractionData = new ExtractRegexInfo(Original);
            }

            /// <summary>
            /// Флаг: строка переведена или исключена.
            /// </summary>
            internal bool IsTranslated
            {
                get
                {
                    if (IsExcluded) return true;
                    if (RegexExtractionData.ExtractedValuesList.Count > 0)
                        return RegexExtractionData.ExtractedValuesList.All(v => v.IsTranslated);
                    return !string.IsNullOrEmpty(Translation) && !string.Equals(Original, Translation);
                }
            }
        }

        #endregion
    }

    #region Interfaces and External Classes

    /// <summary>
    /// Определяет контракт для сервисов перевода.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Переводит массив строк.
        /// </summary>
        /// <param name="texts">Массив строк для перевода.</param>
        /// <returns>Массив переведённых строк.</returns>
        string[] Translate(string[] texts);

        /// <summary>
        /// Переводит одну строку.
        /// </summary>
        /// <param name="text">Строка для перевода.</param>
        /// <returns>Переведённая строка.</returns>
        string Translate(string text);
    }

    /// <summary>
    /// Реализация переводчика через Google Translate (web).
    /// </summary>
    public class GoogleTranslator : ITranslator, IDisposable
    {
        #region Fields
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// HttpClient для запросов к Google Translate.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Код исходного языка.
        /// </summary>
        private readonly string _sourceLanguage;

        /// <summary>
        /// Код целевого языка.
        /// </summary>
        private readonly string _targetLanguage;

        /// <summary>
        /// Список User-Agent для обхода ограничений.
        /// </summary>
        private readonly List<string> _userAgents;

        /// <summary>
        /// Генератор случайных чисел для выбора User-Agent.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Семафор для ограничения параллельных запросов.
        /// </summary>
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Задержка между запросами.
        /// </summary>
        private readonly TimeSpan _delayBetweenRequests;

        /// <summary>
        /// Флаг: объект освобождён.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Максимальное количество повторных попыток при ошибке 429.
        /// </summary>
        private const int Max429Retries = 5;

        /// <summary>
        /// Увеличение задержки при ошибке 429 (мс).
        /// </summary>
        private const int DelayIncreaseMs = 2000;

        #endregion

        #region Constructors

        /// <summary>
        /// Создаёт экземпляр GoogleTranslator.
        /// </summary>
        /// <param name="sourceLanguage">Код исходного языка.</param>
        /// <param name="targetLanguage">Код целевого языка.</param>
        /// <param name="maxConcurrentRequests">Максимум параллельных запросов.</param>
        /// <param name="delayMs">Задержка между запросами (мс).</param>
        public GoogleTranslator(string sourceLanguage, string targetLanguage, int maxConcurrentRequests = 5, int delayMs = 1000)
        {
            Logger.Debug("Инициализация GoogleTranslator");
            _sourceLanguage = sourceLanguage ?? throw new ArgumentNullException(nameof(sourceLanguage));
            _targetLanguage = targetLanguage ?? throw new ArgumentNullException(nameof(targetLanguage));
            _userAgents = new List<string>
                    {
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Safari/605.1.15",
                        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36"
                    };
            _random = new Random();
            _semaphore = new SemaphoreSlim(maxConcurrentRequests);
            _delayBetweenRequests = TimeSpan.FromMilliseconds(delayMs);

            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };
            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        #endregion

        #region ITranslator Implementation

        /// <summary>
        /// Переводит одну строку (синхронно).
        /// </summary>
        /// <param name="text">Строка для перевода.</param>
        /// <returns>Переведённая строка.</returns>
        public string Translate(string text)
        {
            Logger.Debug($"Перевод строки: {text}");
            return TranslateAsync(text).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Переводит массив строк (синхронно).
        /// </summary>
        /// <param name="texts">Массив строк для перевода.</param>
        /// <returns>Массив переведённых строк.</returns>
        public string[] Translate(string[] texts)
        {
            Logger.Debug($"Пакетный перевод массива строк. Количество: {texts?.Length ?? 0}");
            return Task.WhenAll(texts.Select(TranslateAsync)).GetAwaiter().GetResult();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Асинхронно переводит одну строку.
        /// </summary>
        /// <param name="text">Строка для перевода.</param>
        /// <returns>Переведённая строка.</returns>
        private async Task<string> TranslateAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Logger.Error("Text to translate cannot be null or empty.");
                throw new ArgumentException("Text to translate cannot be null or empty.", nameof(text));
            }

            await _semaphore.WaitAsync();
            try
            {
                int retryCount = 0;
                int currentDelayMs = (int)_delayBetweenRequests.TotalMilliseconds;
                while (true)
                {
                    await Task.Delay(currentDelayMs);
                    string userAgent = _userAgents[_random.Next(_userAgents.Count)];

                    string url = $"https://translate.google.com/m?hl=en&sl={_sourceLanguage}&tl={_targetLanguage}&ie=UTF-8&prev=_m&q={Uri.EscapeDataString(text)}";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.UserAgent.ParseAdd(userAgent);

                    HttpResponseMessage response = null;
                    try
                    {
                        response = await _httpClient.SendAsync(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Ошибка при отправке запроса к Google Translate: {ex.Message}");
                        if (retryCount > Max429Retries)
                        {
                            Logger.Error("Превышено количество попыток при ошибке отправки запроса.");
                            throw;
                        }
                        retryCount++;
                        currentDelayMs += DelayIncreaseMs;
                        continue;
                    }

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        Logger.Warn("Получен HTTP 429 (rate limit).");
                        if (retryCount > Max429Retries)
                        {
                            Logger.Error("Превышено количество попыток при ошибке 429.");
                            throw new HttpRequestException("Rate limit exceeded (HTTP 429) after several retries. Consider increasing delay or using proxies.");
                        }
                        retryCount++;
                        currentDelayMs += DelayIncreaseMs;
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    string html = await response.Content.ReadAsStringAsync();
                    string translation = ExtractTranslation(html);
                    Logger.Debug($"Перевод получен: {translation}");
                    return translation;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Извлекает перевод из HTML-ответа Google Translate.
        /// </summary>
        /// <param name="html">HTML-ответ.</param>
        /// <returns>Переведённая строка.</returns>
        private static string ExtractTranslation(string html)
        {
            var match = Regex.Match(html, @"<div class=""result-container"">(.*?)</div>");
            if (!match.Success)
            {
                Logger.Debug("Не удалось извлечь перевод из ответа Google Translate.");
                throw new InvalidOperationException("Failed to extract translation from response.");
            }

            // Декодируем HTML-сущности для нормализации спецсимволов
            return HttpUtility.HtmlDecode(match.Groups[1].Value);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Освобождает ресурсы переводчика.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Logger.Debug("Освобождение ресурсов GoogleTranslator.");
                _httpClient.Dispose();
                _semaphore.Dispose();

                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    #endregion
}
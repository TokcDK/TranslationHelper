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
using TranslationHelper.OnlineTranslators;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Handles online translation of rows, inheriting from RowBase.
    /// Manages buffering, translation, and writing back to rows.
    /// </summary>
    partial class OnlineTranslateTEST : RowBase
    {
        public override string Name => T._("TranslatorTEST");

        protected override bool IsParallelTables => false;
        protected override bool IsParallelRows => false;
        protected virtual bool IsTranslateAll => true;

        // Full row string translation info
        public Dictionary<string, RowTranslationInfo> TranslationDataList = new Dictionary<string, RowTranslationInfo>();

        private readonly ITranslator _translator;
        private readonly ITranslationCache _cache;
        private readonly ConcurrentDictionary<int, TranslationData> _buffer;
        private int TranslationTextLength { get; set; }
        private static int MaxTranslationTextLength => 1000;
        private const int BufferMaxRows = 300;
        private readonly object _bufferLock = new object();

        /// <summary>
        /// Initializes a new instance with dependencies.
        /// </summary>
        public OnlineTranslateTEST(ITranslator translator = null, ITranslationCache cache = null)
        {
            _translator = translator ?? new GoogleTranslator("auto", "en");
            _cache = cache ?? new TranslationCache();
            _buffer = new ConcurrentDictionary<int, TranslationData>();
        }

        /// <summary>
        /// Checks if the buffer or text length has reached its limit.
        /// </summary>
        private bool IsMax() => TranslationTextLength >= MaxTranslationTextLength || _buffer.Count >= BufferMaxRows;

        /// <summary>
        /// Validates if the row is suitable for translation.
        /// </summary>
        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return !AppSettings.InterruptTtanslation && base.IsValidRow(rowData)
                && (string.IsNullOrEmpty(rowData.Translation)
                || rowData.Original.HasAnyTranslationLineValidAndEqualSameOrigLine(rowData.Translation));
        }

        private bool _allDbLoaded4All;

        /// <summary>
        /// Initializes translation resources, such as loading DB files if needed.
        /// </summary>
        protected async override void ActionsInit()
        {
            FunctionsOnlineCache.Init();

            if (_allDbLoaded4All || !IsAll || !AppSettings.UseAllDBFilesForOnlineTranslationForAll) return;

            if (!AppSettings.EnableTranslationCache)
            {
                var result = MessageBox.Show(T._("Translation cache disabled but load all DB enabled. While all DB loading cache can be enabled in settings. Load all DB?"),
                    T._("Translation cache disabled"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            if (!IsTranslateAll && AppSettings.EnableTranslationCache)
            {
                var result = MessageBox.Show(T._("Load all exist database files?"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            Logger.Info(T._("Get all DB"));
            await Task.Run(() => FunctionsDBFile.MergeAllDBtoOne()).ConfigureAwait(true);
            _allDbLoaded4All = true;
        }

        /// <summary>
        /// Finalizes translation by processing any remaining buffer and cleaning up.
        /// </summary>
        protected override void ActionsFinalize()
        {
            if (_buffer.Count > 0) TranslateStrings();
            FunctionsOnlineCache.Unload();
            if (AppSettings.InterruptTtanslation) AppSettings.InterruptTtanslation = false;
            Logger.Info(T._("Translation complete"));
        }

        private string _lastTableName = string.Empty;

        /// <summary>
        /// Applies translation to the row by buffering its lines.
        /// </summary>
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

        /// <summary>
        /// Buffers row lines for batch translation.
        /// </summary>
        private void SetRowLinesToBuffer(RowBaseRowData rowData)
        {
            if (rowData == null) throw new ArgumentNullException(nameof(rowData));
            if (rowData.SelectedTable == null) throw new InvalidOperationException("SelectedTable is null");
            if (rowData.Original == null) throw new InvalidOperationException("Original text is null");

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
                    lineData.IsExcluded = true;
                    lineData.Translation = line;
                    continue;
                }

                GetFromExtracted(lineData.RegexExtractionData, out int skippedCount);
                if (isExtracted && skippedCount == lineData.RegexExtractionData.ExtractedValuesList.Count) continue;

                if (!isExtracted && CheckInCache(line, lineData)) continue;

                if (IsMax()) TranslateStrings();
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
        /// Checks cache for a line and updates the line data if found.
        /// </summary>
        private bool CheckInCache(string line, LineTranslationData lineData)
        {
            var cached = _cache.GetValueFromCacheOrReturnEmpty(line);
            if (!string.IsNullOrEmpty(cached))
            {
                lineData.Translation = cached;
                return true;
            }
            TranslationTextLength += line.Length;
            return false;
        }

        /// <summary>
        /// Processes extracted values, updating length or using cache.
        /// </summary>
        private void GetFromExtracted(ExtractRegexInfo extractData, out int skippedValuesCount)
        {
            skippedValuesCount = 0;
            foreach (var value in extractData.ExtractedValuesList)
            {
                var cached = _cache.GetValueFromCacheOrReturnEmpty(value.Original);
                if (!string.IsNullOrEmpty(cached))
                {
                    value.Translation = cached;
                    skippedValuesCount++;
                }
                else if (value.Original.IsSoundsText() || !value.Original.IsValidForTranslation())
                {
                    value.Translation = value.Original;
                    skippedValuesCount++;
                }
                else
                {
                    TranslationTextLength += value.Original.Length;
                }
            }
        }

        /// <summary>
        /// Translates buffered strings and updates rows.
        /// </summary>
        private void TranslateStrings()
        {
            var originals = GetOriginals();
            if (originals.Length == 0 && _buffer.Count == 0) return;

            var translated = TranslateOriginals(originals);
            SetTranslationsToBuffer(originals, translated);
            SetBufferToRows();
            TranslationTextLength = 0;
            _cache.Write();
        }

        /// <summary>
        /// Retrieves unique originals from the buffer for translation.
        /// </summary>
        private string[] GetOriginals()
        {
            var originals = new ConcurrentDictionary<string, bool>();
            Parallel.ForEach(EnumerateBufferedLinesData(), line =>
            {
                foreach (var ot in EnumerateOriginalTranslation(line))
                    originals.TryAdd(ot.Original, true);
            });
            return originals.Keys.ToArray();
        }

        /// <summary>
        /// Enumerates original texts needing translation.
        /// </summary>
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
        /// Enumerates all buffered line data.
        /// </summary>
        private IEnumerable<LineTranslationData> EnumerateBufferedLinesData()
        {
            foreach (var table in _buffer.Values)
                foreach (var row in table.Rows.Values)
                    foreach (var line in row.Lines)
                        yield return line;
        }

        /// <summary>
        /// Translates originals with pre/post actions and fallback.
        /// </summary>
        private string[] TranslateOriginals(string[] originals)
        {
            if (originals == null || originals.Length == 0) return Array.Empty<string>();

            var preApplied = ApplyProjectPretranslationAction(originals);
            var translated = TranslateWithFallback(preApplied);
            return ApplyProjectPostTranslationAction(originals, translated);
        }

        /// <summary>
        /// Translates texts with fallback for failures.
        /// </summary>
        private string[] TranslateWithFallback(string[] texts)
        {
            try
            {
                return _translator.Translate(texts);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Translation failed: {ex}");
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
                        Logger.Warn($"Batch translation failed: {ex2}");
                        foreach (var text in batch)
                        {
                            try
                            {
                                translated.Add(_translator.Translate(text));
                            }
                            catch (Exception ex3)
                            {
                                Logger.Error($"Failed to translate: {text}", ex3);
                                translated.Add(string.Empty);
                            }
                        }
                    }
                }
                return translated.ToArray();
            }
        }

        /// <summary>
        /// Applies project-specific pre-translation actions.
        /// </summary>
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
        /// Applies project-specific post-translation actions.
        /// </summary>
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
        /// Assigns translations back to the buffer.
        /// </summary>
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
        /// Writes buffered translations back to rows.
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

        private readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
        private readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
        private readonly StringChangerBase _hardFixes = new AllHardFixesChanger();
        private readonly StringChangerBase _fixCells = new FixCellsChanger();

        /// <summary>
        /// Writes translated data back to the row if complete.
        /// </summary>
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
            return true;
        }

        /// <summary>
        /// Generates translated lines for writing back.
        /// </summary>
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

        private string ApplyFixes(LineTranslationData line) => ApplyFixes(line.Original, line.Translation);

        private string ApplyFixes(string original, string translation)
        {
            var text = _hardFixes.Change(translation, original);
            return _fixCells.Change(text, original);
        }

        private static readonly Regex _replacerListTypeRegex = new Regex(@"^\$[0-9]+(,\$[0-9]+)+$", RegexOptions.Compiled);
        private static readonly Regex _oneMatchNeedInsertTextRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex _groupReplacerMarkerRegex = new Regex(@"\$[0-9]+", RegexOptions.Compiled);

        /// <summary>
        /// Merges extracted values back into the line text.
        /// </summary>
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
                    throw new InvalidOperationException("Unknown replacer type");
            }
        }

        /// <summary>
        /// Determines the type of replacer used for merging.
        /// </summary>
        private TranslationRegexExtractType DetermineReplacerType(string replacer)
        {
            var trimmed = replacer.Trim();
            if (_oneMatchNeedInsertTextRegex.IsMatch(trimmed)) return TranslationRegexExtractType.ReplaceOne;
            if (_replacerListTypeRegex.IsMatch(trimmed)) return TranslationRegexExtractType.ReplaceList;
            return TranslationRegexExtractType.Replacer;
        }

        /// <summary>
        /// Merges using ReplaceOne strategy: replaces entire match with first group's translation.
        /// </summary>
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
        /// Merges using ReplaceList strategy: replaces each group with its translation.
        /// </summary>
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
        /// Merges using Replacer strategy: uses a custom replacer with placeholders.
        /// </summary>
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

        /// <summary>
        /// Buffer for lines to be translated.
        /// </summary>
        internal class Buffer : IOriginalTranslationUser
        {
            public string Original { get; }
            public string Translation { get; set; }
            internal bool IsExtracted { get; }

            internal Buffer(string original, string translation, bool isExtracted)
            {
                Original = original;
                Translation = translation;
                IsExtracted = isExtracted;
            }
        }

        /// <summary>
        /// Holds translation data for a table.
        /// </summary>
        internal class TranslationData
        {
            internal int TableIndex => Row.SelectedTableIndex;
            internal ConcurrentDictionary<int, RowTranslationData> Rows = new ConcurrentDictionary<int, RowTranslationData>();
            internal RowBaseRowData Row { get; }

            public TranslationData(RowBaseRowData rowData) => Row = rowData;
        }

        /// <summary>
        /// Holds translation data for a row.
        /// </summary>
        internal class RowTranslationData
        {
            internal bool IsAllLinesAdded = false;
            internal int RowIndex => Row.SelectedRowIndex;
            internal List<LineTranslationData> Lines = new List<LineTranslationData>();
            internal RowBaseRowData Row { get; }

            public RowTranslationData(RowBaseRowData rowData) => Row = rowData;
        }

        /// <summary>
        /// Holds translation data for a single line.
        /// </summary>
        internal class LineTranslationData : IOriginalTranslationUser
        {
            internal readonly int LineIndex;
            public string Original { get; }
            public string Translation { get; set; }
            internal ExtractRegexInfo RegexExtractionData;
            internal bool IsExcluded = false;

            public LineTranslationData(int lineIndex, string originalText)
            {
                LineIndex = lineIndex;
                Original = originalText;
                RegexExtractionData = new ExtractRegexInfo(Original);
            }

            /// <summary>
            /// Indicates if the line is translated or excluded.
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
    }

    /// <summary>
    /// Defines the contract for translation services.
    /// </summary>
    public interface ITranslator
    {
        string[] Translate(string[] texts);
        string Translate(string text);
    }

    /// <summary>
    /// Defines the contract for translation caching.
    /// </summary>
    public interface ITranslationCache
    {
        string GetValueFromCacheOrReturnEmpty(string key);
        void TryAdd(string key, string value);
        void Write();
    }

    public class GoogleTranslator : ITranslator, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _sourceLanguage;
        private readonly string _targetLanguage;
        private readonly List<string> _userAgents;
        private readonly Random _random;
        private readonly SemaphoreSlim _semaphore;
        private readonly TimeSpan _delayBetweenRequests;
        private bool _disposed;

        public GoogleTranslator(string sourceLanguage, string targetLanguage, int maxConcurrentRequests = 5, int delayMs = 1000)
        {
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

        public string Translate(string text)
        {
            return TranslateAsync(text).GetAwaiter().GetResult();
        }

        public string[] Translate(string[] texts)
        {
            return Task.WhenAll(texts.Select(TranslateAsync)).GetAwaiter().GetResult();
        }

        private async Task<string> TranslateAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text to translate cannot be null or empty.", nameof(text));

            await _semaphore.WaitAsync();
            try
            {
                await Task.Delay(_delayBetweenRequests);
                string userAgent = _userAgents[_random.Next(_userAgents.Count)];

                string url = $"https://translate.google.com/m?hl=en&sl={_sourceLanguage}&tl={_targetLanguage}&ie=UTF-8&prev=_m&q={Uri.EscapeDataString(text)}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.UserAgent.ParseAdd(userAgent);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.StatusCode == (HttpStatusCode)429)
                {
                    throw new HttpRequestException("Rate limit exceeded (HTTP 429). Consider increasing delay or using proxies.");
                }

                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();
                string translation = ExtractTranslation(html);
                return translation;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private string ExtractTranslation(string html)
        {
            var match = Regex.Match(html, @"<div class=""result-container"">(.*?)</div>");
            if (!match.Success)
            {
                throw new InvalidOperationException("Failed to extract translation from response.");
            }

            // Decode HTML entities to normalize special characters (e.g., &#39; to ')
            return HttpUtility.HtmlDecode(match.Groups[1].Value);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient.Dispose();
                _semaphore.Dispose();

                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }

    public class TranslationCache : ITranslationCache
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public string GetValueFromCacheOrReturnEmpty(string key)
        {
            return _cache.TryGetValue(key, out string value) ? value : string.Empty;
        }

        public void TryAdd(string key, string value)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache[key] = value;
            }
        }

        public void Write()
        {
            // Placeholder for persisting cache to storage (e.g., file or database)
            // Example: File.WriteAllLines("cache.txt", _cache.Select(kv => $"{kv.Key}:{kv.Value}"));
        }
    }
}
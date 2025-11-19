using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;
using static IronRuby.StandardLibrary.BigDecimal.BigDecimal;

namespace TranslationHelper.Forms.Search
{
    internal class SearchSharedHelpers
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        internal static void UnEscapeSearchValues(List<string> arr, bool unescape = true)
        {
            int arrCount = arr.Count;
            for (int i = 0; i < arrCount; i++)
            {
                try
                {
                    arr[i] = unescape ? Regex.Unescape(arr[i]) : Regex.Escape(arr[i]);
                }
                catch (ArgumentException ex) 
                {
                    /* Ignore but log invalid regex patterns */
                    _logger.Debug($"{nameof(UnEscapeSearchValues)} '{arr[i]}' value error:\n {ex}");
                }
            }
        }

        internal static void RemoveQuotesFromLoadedSearchValues(List<string> items)
        {
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                if (items[i].StartsWith("\"") && items[i].EndsWith("\""))
                    items[i] = items[i].Substring(1, items[i].Length - 2);
            }
        }

        internal static void AddQuotesToWritingSearchValues(List<string> items)
        {
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; i++)
                items[i] = $"\"{items[i]}\"";
        }

        internal static List<string> LoadSearchQueries(string sectionName = "", int maxEntriesCount = 20)
        {
            sectionName = string.IsNullOrWhiteSpace(sectionName) ? THSettings.SearchQueriesSectionName : sectionName;
            var list = new List<string>();
            try
            {
                var savedQueries = AppData.Settings.THConfigINI.GetSectionValues(sectionName)?.ToArray();
                if (savedQueries?.Length > 0)
                {
                    list.Clear();
                    list.AddRange(savedQueries.Take(maxEntriesCount));
                    SearchSharedHelpers.RemoveQuotesFromLoadedSearchValues(list);
                    SearchSharedHelpers.UnEscapeSearchValues(list);
                }
            }
            catch (IOException ex)
            {
                _logger.Warn($"Failed to load {sectionName}", ex);
            }

            return list;
        }
        internal static List<string> SaveSearchQueries(List<string> list, string sectionName = "", int maxEntriesCount = 20)
        {
            try
            {
                sectionName = string.IsNullOrWhiteSpace(sectionName) ? THSettings.SearchQueriesSectionName : sectionName;

                var lastLoadedList = LoadSearchQueries(sectionName, maxEntriesCount);

                if (list.Count > 0 && IsSearchQueriesReplacersListChanged(lastLoadedList, list))
                {
                    AddNewRecords(list, lastLoadedList);
                    list = list.Where(s => !string.IsNullOrEmpty(s)).Take(maxEntriesCount).ToList();
                    SearchSharedHelpers.AddQuotesToWritingSearchValues(list);
                    SearchSharedHelpers.UnEscapeSearchValues(list, false);
                    AppData.Settings.THConfigINI.SetArrayToSectionValues(sectionName, list.ToArray());
                }

                return list;
            }
            catch (IOException ex)
            {
                _logger.Warn($"Failed to save {sectionName}", ex);
            }

            return list;
        }
        private static bool IsSearchQueriesReplacersListChanged(List<string> oldList, List<string> newList)
            => oldList.Count != newList.Count || !oldList.SequenceEqual(newList);

        private static void AddNewRecords(List<string> list, IEnumerable<string> listToAdd)
        {
            foreach (var item in listToAdd)
            {
                if (!list.Contains(item)) 
                { 
                    list.Insert(0, item);
                }
            }
        }
    }
}

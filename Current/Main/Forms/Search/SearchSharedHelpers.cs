using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranslationHelper.Forms.Search
{
    internal class SearchSharedHelpers
    {
        internal static void UnEscapeSearchValues(List<string> arr, bool unescape = true)
        {
            int arrCount = arr.Count;
            for (int i = 0; i < arrCount; i++)
            {
                try
                {
                    arr[i] = unescape ? Regex.Unescape(arr[i]) : Regex.Escape(arr[i]);
                }
                catch (ArgumentException) { /* Ignore invalid regex patterns */ }
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
    }
}

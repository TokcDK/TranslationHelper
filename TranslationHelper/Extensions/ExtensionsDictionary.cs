using System.Collections.Generic;

namespace TranslationHelper.Extensions
{
    static class ExtensionsList
    {
        /// <summary>
        /// Add <paramref name="value"/> if not exists in <paramref name="hashset"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashset"></param>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        internal static void AddTry<T>(this HashSet<T> hashset,T value)
        {
            if (!hashset.Contains(value))
            {
                hashset.Add(value);
            }
        }

        /// <summary>
        /// Add <paramref name="key"/>y-<paramref name="value"/> if <paramref name="key"/> not exists in <paramref name="dictionary"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddTry<T>(this Dictionary<T, T> dictionary, T key, T value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }
    }
}

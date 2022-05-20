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
        internal static void TryAdd<T>(this HashSet<T> hashset,T value)
        {
            if (!hashset.Contains(value))
            {
                hashset.Add(value);
            }
        }

        /// <summary>
        /// Add <paramref name="key"/>-<paramref name="value"/> if <paramref name="key"/> not exists in <paramref name="dictionary"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key)) return;

            dictionary.Add(key, value);
        }

        /// <summary>
        /// Remove <paramref name="key"/> if <paramref name="key"/> exists in <paramref name="dictionary"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        internal static void TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
        }
    }
}

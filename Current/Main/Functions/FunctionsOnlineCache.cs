using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

//https://stackoverflow.com/questions/1799767/easy-way-to-convert-a-dictionarystring-string-to-xml-and-vice-versa
namespace TranslationHelper.Functions
{
    class FunctionsOnlineCache
    {
        internal Dictionary<string, string> Cache = new Dictionary<string, string>();

        internal string GetValueFromCacheOrReturnEmpty(string keyValue)
        {
            if (!AppSettings.EnableTranslationCache) return string.Empty;

            if (AppSettings.UseAllDBFilesForOnlineTranslationForAll
                && AppData.AllDBmerged != null
                && AppData.AllDBmerged.TryGetValue(keyValue, out string mergedDBValue)
                && !string.IsNullOrWhiteSpace(mergedDBValue))
            {
                return mergedDBValue;
            }
            else if (Cache != null
                && Cache.TryGetValue(keyValue, out string cachedValue)
                && !string.IsNullOrWhiteSpace(cachedValue))
            {
                return cachedValue;
            }
            else
            {
                var trimmed = keyValue.TrimAllExceptLettersOrDigits();
                if (AppSettings.UseAllDBFilesForOnlineTranslationForAll
                   && trimmed.Length > 0
                   && AppData.AllDBmerged != null
                   //&& AppData.AllDBmerged.Count > 0
                   && AppData.AllDBmerged.TryGetValue(trimmed, out string mergedDBValueByTrimmed)
                   && !string.IsNullOrWhiteSpace(mergedDBValueByTrimmed))
                {
                    var ind = keyValue.IndexOf(trimmed); //sometimes inex is -1 of 'え' in 'え゛？' for example
                    try
                    {
                        return keyValue.Remove(ind, trimmed.Length).Insert(ind, mergedDBValueByTrimmed);
                    }
                    catch { }
                }
                else if (
                    trimmed.Length > 0
                    && Cache != null
                    && Cache.TryGetValue(trimmed, out string cachedValueByTrimmed)
                    && !string.IsNullOrWhiteSpace(cachedValueByTrimmed))
                {
                    var ind = keyValue.IndexOf(trimmed);
                    try
                    {
                        return keyValue.Remove(ind, trimmed.Length).Insert(ind, cachedValueByTrimmed);
                    }
                    catch { }
                }

                return string.Empty;
            }
        }

        readonly static object cacheWriteLocker = new object();
        public void Write()
        {
            Write(Cache);
        }
        public static void Write(Dictionary<string, string> cache)
        {
            if (cache == null || cache.Count == 0) return;

            lock (cacheWriteLocker)
            {
                XElement el = new XElement("TranslationCache",
                    cache.Select(KeyValue =>
                    new XElement("Value",
                        new XElement(THSettings.OriginalColumnName, KeyValue.Key),
                        new XElement(THSettings.TranslationColumnName, KeyValue.Value)
                        )
                    ));
                FunctionsDBFile.WriteXElementToXMLFile(el, AppSettings.THTranslationCachePath);
            }
        }

        /// <summary>
        /// init cache when it was not init
        /// </summary>
        internal static void Init()
        {
            //if (!AppSettings.IsTranslationCacheEnabled)
            //    return;

            AppData.OnlineTranslationCache.UsersCount++;
        }

        /// <summary>
        /// Count of users of the instance of cache
        /// </summary>
        internal int UsersCount = 0;

        /// <summary>
        /// unload cache when need
        /// </summary>
        internal static void Unload()
        {
            AppData.OnlineTranslationCache.Write(); // write on unload

            AppData.OnlineTranslationCache.UsersCount--; // decrease users count

            if (AppData.OnlineTranslationCache != null && AppData.OnlineTranslationCache.UsersCount == 0)
            {
                AppData.OnlineTranslationCache = null;
            }
        }

        readonly object cacheReadLocker = new object();
        public void Read()
        {
            if (Cache != null && Cache.Count > 0) return;

            lock (cacheReadLocker)
            {
                string xml = FunctionsDBFile.ReadXMLToString(AppSettings.THTranslationCachePath);
                if (xml.Length == 0) return;

                XElement rootElement;// = null;
                try { rootElement = XElement.Parse(xml); }
                catch (Exception ex)
                {
                    //write exception, rename broken cache file and return
                    string targetFilePath = FunctionsFileFolder.NewFilePathPlusIndex(AppSettings.THTranslationCachePath + ".broken");
                    File.WriteAllText(Path.Combine(Path.GetDirectoryName(targetFilePath), Path.GetFileName(targetFilePath) + ".log"), ex.ToString());
                    File.Move(AppSettings.THTranslationCachePath, targetFilePath);
                    return;
                }

                foreach (var el in rootElement.Elements())
                {
                    string key = el.Element(THSettings.OriginalColumnName).Value;
                    if (!Cache.ContainsKey(key)) Cache.Add(key, el.Element(THSettings.TranslationColumnName).Value);
                }
            }

        }

        public static void TryAdd(string Original, string Translation)
        {
            if (!AppSettings.EnableTranslationCache || AppSettings.IsTranslationHelperWasClosed) return;
            if (string.IsNullOrWhiteSpace(Translation)) return;
            if (string.Equals(Original, Translation)) return;
            if (Original.GetLinesCount() != Translation.GetLinesCount()) return;
            if (AppData.OnlineTranslationCache.Cache.ContainsKey(Original)) return;

            AppData.OnlineTranslationCache.Cache.Add(Original, Translation);
        }
    }
}

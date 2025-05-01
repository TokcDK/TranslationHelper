using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

//https://stackoverflow.com/questions/1799767/easy-way-to-convert-a-dictionarystring-string-to-xml-and-vice-versa
namespace TranslationHelper.Functions
{
    class FunctionsOnlineCache
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal Dictionary<string, string> Cache = new Dictionary<string, string>();

        internal string GetValueFromCacheOrReturnEmpty(string key)
        {
            if (!AppSettings.EnableTranslationCache) return string.Empty;

            if ((AppSettings.UseAllDBFilesForOnlineTranslationForAll
                && TryGetNonEmptyValue(AppData.AllDBmerged, key, out var cachedValue)) 
                || TryGetNonEmptyValue(Cache, key, out cachedValue))
            {
                return cachedValue;
            }
            else
            {
                var trimmed = key.TrimAllExceptLettersAndDigits();
                int index;
                if ((AppSettings.UseAllDBFilesForOnlineTranslationForAll
                   && TryGetNonEmptyValue(AppData.AllDBmerged, trimmed, out var cachedValueByTrimmed))
                   || TryGetNonEmptyValue(Cache, trimmed, out cachedValueByTrimmed))
                {
                    index = key.IndexOf(trimmed); //sometimes index is -1 of 'え' in 'え゛？' for example
                    if(index != -1)
                    {
                        return key.Remove(index, trimmed.Length).Insert(index, cachedValueByTrimmed);
                    }
                }

                return string.Empty;
            }
        }

        internal static bool TryGetNonEmptyValue(Dictionary<string, string> dictionary, string keyString, out string value)
        {
            value = null;

            if(string.IsNullOrWhiteSpace(keyString)
               || dictionary == null
               || !dictionary.TryGetValue(keyString, out string s)
               || !string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            value = s;

            return true;
        }

        public void Write()
        {
            Write(Cache);
        }
        public static void Write(Dictionary<string, string> cache)
        {
            if (cache == null || cache.Count == 0) return;

            lock (_translationCacheLocker)
            {
                ReadCacheFile(cache); // read first to get new values if there was any added

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

        readonly static object _translationCacheLocker = new object();
        public void Read()
        {
            Read(Cache);
        }
        public static void Read(Dictionary<string, string> cache)
        {
            lock (_translationCacheLocker)
            {
                ReadCacheFile(cache);
            }
        }

        private static void ReadCacheFile(Dictionary<string, string> cache)
        {
            string xml = FunctionsDBFile.ReadXMLToString(AppSettings.THTranslationCachePath);
            if (xml.Length == 0) return;

            XElement rootElement;// = null;
            try { rootElement = XElement.Parse(xml); }
            catch (Exception ex)
            {
                //write exception, rename broken cache file and return
                string targetFilePath = FunctionsFileFolder.NewFilePathPlusIndex(AppSettings.THTranslationCachePath + ".broken");
                File.Move(AppSettings.THTranslationCachePath, targetFilePath);
                Logger.Warn(T._("Cache file '{0}' broken and renamed to {1}. Error: {2}"), AppSettings.THTranslationCachePath, targetFilePath, ex.ToString());

                return;
            }

            foreach (var el in rootElement.Elements())
            {
                string key = el.Element(THSettings.OriginalColumnName).Value;
                if (!cache.ContainsKey(key)) cache.Add(key, el.Element(THSettings.TranslationColumnName).Value);
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

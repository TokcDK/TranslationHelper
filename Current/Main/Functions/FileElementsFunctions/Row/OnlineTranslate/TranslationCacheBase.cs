using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

//https://stackoverflow.com/questions/1799767/easy-way-to-convert-a-dictionarystring-string-to-xml-and-vice-versa
namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    /// <summary>
    /// Half static cache to make it alive on time of app loaded
    /// </summary>
    public abstract class TranslationCacheBase : ITranslationCache
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        readonly static object _translationCacheLocker = new object();


        static Dictionary<string, string> _cache;
        static bool _cacheNeedReadByFirstUse = true;
        static Dictionary<string, string> Cache
        {
            get
            {
                if (_cacheNeedReadByFirstUse)
                {
                    InitCacheAndRead();
                }

                return _cache;
            }
        }

        private static void InitCacheAndRead()
        {
            _cacheNeedReadByFirstUse = false;

            if (_cache == null)
            {
                _cache = new Dictionary<string, string>();
            }

            Read();
        }

        static List<ITranslationCache> CacheUserInstancesList;

        /// <summary>
        /// init cache and cache use instance
        /// </summary>
        protected static void Init(TranslationCache cacheUserInstance)
        {
            lock (_translationCacheLocker)
            {
                if (cacheUserInstance == null)
                {
                    throw new ArgumentNullException("Translation cache instance was null");
                }

                if (CacheUserInstancesList == null)
                {
                    CacheUserInstancesList = new List<ITranslationCache>();
                }

                if (!CacheUserInstancesList.Contains(cacheUserInstance))
                {
                    CacheUserInstancesList.Add(cacheUserInstance);
                }
            }
        }

        /// <summary>
        /// unload cache and cache user instance
        /// </summary>
        protected static void Unload(TranslationCache translationCache)
        {
            lock (_translationCacheLocker)
            {
                if (CacheUserInstancesList.Contains(translationCache))
                {
                    CacheUserInstancesList.Remove(translationCache);
                }

                if (CacheUserInstancesList.Count == 0)
                {
                    //Cache = null; // if not commented, it will reread it each time
                }
            }
        }

        internal static bool TryGetNonEmptyValue(Dictionary<string, string> dictionary, string keyString, out string value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(keyString)
               || dictionary == null
               || !dictionary.TryGetValue(keyString, out string s)
               || !string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            value = s;

            return true;
        }

        static Dictionary<string, string> lastWroteCache;

        protected static void Write(Dictionary<string, string> cache)
        {
            if (cache == null || cache.Count == 0) return;

            if(ReferenceEquals(cache, lastWroteCache) && cache.Count == lastWroteCache.Count) // if the cache changed there must be more records
            {
                return;
            }

            lastWroteCache = cache;

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

        protected static void Read()
        {
            Read(Cache);
        }
        protected static void Read(Dictionary<string, string> cache)
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

        public virtual void TryAdd(string key, string value)
        {
            if (!AppSettings.EnableTranslationCache || AppSettings.IsTranslationHelperWasClosed) return;
            if (string.IsNullOrWhiteSpace(value)) return;
            if (string.Equals(key, value)) return;
            if (key.GetLinesCount() != value.GetLinesCount()) return;
            if (Cache.ContainsKey(key)) return;

            Cache.Add(key, value);
        }

        static string TryGetValueByKey(string key)
        {
            if (!AppSettings.EnableTranslationCache) return string.Empty;

            if (AppSettings.UseAllDBFilesForOnlineTranslationForAll
                && TryGetNonEmptyValue(AppData.AllDBmerged, key, out var cachedValue)
                || TryGetNonEmptyValue(Cache, key, out cachedValue))
            {
                return cachedValue;
            }
            else
            {
                var trimmed = key.TrimAllExceptLettersAndDigits();
                int index;
                if (AppSettings.UseAllDBFilesForOnlineTranslationForAll
                   && TryGetNonEmptyValue(AppData.AllDBmerged, trimmed, out var cachedValueByTrimmed)
                   || TryGetNonEmptyValue(Cache, trimmed, out cachedValueByTrimmed))
                {
                    index = key.IndexOf(trimmed); //sometimes index is -1 of 'え' in 'え゛？' for example
                    if (index != -1)
                    {
                        return key.Remove(index, trimmed.Length).Insert(index, cachedValueByTrimmed);
                    }
                }

                return string.Empty;
            }
        }

        public virtual string TryGetValue(string key)
        {
            return TryGetValueByKey(key);
        }
        public virtual void Write()
        {
            Write(Cache);
        }

        public abstract void Dispose();
    }
}

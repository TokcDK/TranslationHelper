﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

//https://stackoverflow.com/questions/1799767/easy-way-to-convert-a-dictionarystring-string-to-xml-and-vice-versa
namespace TranslationHelper.Functions
{
    class FunctionsOnlineCache
    {
        internal Dictionary<string, string> cache = new Dictionary<string, string>();

        private readonly THDataWork thDataWork;

        public FunctionsOnlineCache(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal string GetValueFromCacheOrReturnEmpty(string keyValue)
        {
            if (Properties.Settings.Default.IsTranslationCacheEnabled && Properties.Settings.Default.UseAllDBFilesForOnlineTranslationForAll && thDataWork.AllDBmerged !=null && thDataWork.AllDBmerged.Count > 0 && thDataWork.AllDBmerged.ContainsKey(keyValue) && !string.IsNullOrWhiteSpace(thDataWork.AllDBmerged[keyValue]))
            {
                return thDataWork.AllDBmerged[keyValue];
            }
            else if (Properties.Settings.Default.IsTranslationCacheEnabled && cache!=null && cache.Count > 0 && cache.ContainsKey(keyValue) && !string.IsNullOrWhiteSpace(cache[keyValue]))
            {
                return cache[keyValue];
            }
            else
            {
                return string.Empty;
            }
        }

        public void WriteCache()
        {
            if (cache == null || cache.Count < 1)
            {
                return;
            }

            XElement el = new XElement("TranslationCache",
                cache.Select(KeyValue =>
                new XElement("Value",
                    new XElement("Original", KeyValue.Key),
                    new XElement("Translation", KeyValue.Value)
                    )
                ));
            //el.Save("cache.xml");
            FunctionsDBFile.WriteXElementToXMLFile(el, Properties.Settings.Default.THTranslationCachePath);
        }

        public void ReadCache()
        {
            string xml = FunctionsDBFile.ReadXMLToString(Properties.Settings.Default.THTranslationCachePath);
            if (xml.Length == 0)
                return;

            XElement rootElement = null;
            try
            {
                rootElement = XElement.Parse(xml);
            }
            catch (Exception ex)
            {
                //write exception, rename broken cache file and return
                string targetFilePath = FunctionsFileFolder.NewFilePathPlusIndex(Properties.Settings.Default.THTranslationCachePath + ".broken");
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(targetFilePath), Path.GetFileName(targetFilePath) + ".log"), ex.ToString());
                File.Move(Properties.Settings.Default.THTranslationCachePath, targetFilePath);
                return;
            }
            foreach (var el in rootElement.Elements())
            {
                string key = el.Element("Original").Value;
                if (!cache.ContainsKey(key))
                {
                    cache.Add(key, el.Element("Translation").Value);
                }
            }
        }

        //private static string ReadXMLToString()
        //{
        //    using (FileStream fs = new FileStream(Properties.Settings.Default.THTranslationCachePath, FileMode.Open))
        //    {
        //        Stream s;
        //        string fileExtension = Path.GetExtension(Properties.Settings.Default.THTranslationCachePath);
        //        if (fileExtension == ".cmx")
        //        {
        //            s = new GZipStream(fs, CompressionMode.Decompress);
        //        }
        //        else if (fileExtension == ".cmz")
        //        {
        //            s = new DeflateStream(fs, CompressionMode.Decompress);
        //        }
        //        else
        //        {
        //            s = fs;
        //        }
        //        string stringForReturn;
        //        using (StreamReader sr = new StreamReader(s))
        //        {
        //            stringForReturn = sr.ReadToEnd();
        //        }
        //        return stringForReturn;
        //        //s.Close();
        //    }
        //}

        //private static void WriteXElementToXMLFile(XElement el)
        //{
        //    using (FileStream fs = new FileStream(Properties.Settings.Default.THTranslationCachePath, FileMode.Create))
        //    {
        //        Stream s;
        //        string fileExtension = Path.GetExtension(Properties.Settings.Default.THTranslationCachePath);
        //        if (fileExtension == ".cmx")
        //        {
        //            s = new GZipStream(fs, CompressionMode.Compress);
        //        }
        //        else if (fileExtension == ".cmz")
        //        {
        //            s = new DeflateStream(fs, CompressionMode.Compress);
        //        }
        //        else
        //        {
        //            s = fs;
        //        }
        //        el.Save(s);
        //        s.Close();
        //    }
        //}
    }
}

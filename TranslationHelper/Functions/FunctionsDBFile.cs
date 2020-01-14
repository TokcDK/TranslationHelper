using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;

namespace TranslationHelper.Main.Functions
{
    class FunctionsDBFile
    {
        public static void WriteTranslationCacheIfValid(DataSet THTranslationCache, string tHTranslationCachePath)
        {
            if (Properties.Settings.Default.IsTranslationCacheEnabled && !Properties.Settings.Default.IsTranslationHelperWasClosed && THTranslationCache.Tables[0].Rows.Count > 0)
            {
                FunctionsDBFile.WriteDBFile(THTranslationCache, tHTranslationCachePath);
                //THTranslationCache.Reset();
            }
        }

        //https://stackoverflow.com/questions/223738/net-stream-dataset-of-xml-data-to-zip-file
        //http://madprops.org/blog/saving-datasets-locally-with-compression/
        public static void ReadDBFile(DataSet DS, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Stream s;
                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Decompress);
                }
                else if (fileExtension == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Decompress);
                }
                else
                {
                    s = fs;
                }
                DS.ReadXml(s);
                s.Close();
            }
        }

        public static void WriteDBFile(DataSet DS, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                Stream s;
                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Compress);
                }
                else if (fileExtension == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Compress);
                }
                else
                {
                    s = fs;
                }
                DS.WriteXml(s);
                s.Close();
            }
        }

        internal static string GetDBCompressionExt(THDataWork thDataWork)
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked") == "True")
            {
                //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression"));
                if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "XML (none)")
                {
                    //MessageBox.Show(".xml");
                    return ".xml";
                }
                else if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Gzip (cmx)")
                {
                    //MessageBox.Show(".cmx");
                    return ".cmx";
                }
                else if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Deflate (cmz)")
                {
                    //MessageBox.Show(".cmz");
                    return ".cmz";
                }

            }
            //MessageBox.Show("Default .xml");
            return ".xml";
        }

        internal static string GetProjectDBFolder()
        {
            string ret = string.Empty;
            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker MV"))
            {
                ret = "RPGMakerMV";
            }
            else if (RPGMFunctions.THSelectedSourceType.Contains("RPGMaker") || RPGMFunctions.THSelectedSourceType.Contains("RPG Maker"))
            {
                ret = "RPGMakerTransPatch";
            }
            return Path.Combine(Application.StartupPath, "DB", ret);
        }

        internal static string GetDBFileName(THDataWork thDataWork, bool IsSaveAs = false)
        {
            string fName = Path.GetFileName(Properties.Settings.Default.THSelectedDir);
            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker MV"))
            {
                if (thDataWork.Main.THFilesList.Items.Count == 1 && thDataWork.Main.THFilesList.Items[0] != null && !string.IsNullOrWhiteSpace(thDataWork.Main.THFilesList.Items[0].ToString()))
                {
                    if (fName == "data")
                    {
                        fName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(Properties.Settings.Default.THSelectedDir))) + "_" + Path.GetFileNameWithoutExtension(thDataWork.Main.THFilesList.Items[0].ToString());
                    }
                    else
                    {
                        fName = Path.GetFileNameWithoutExtension(thDataWork.Main.THFilesList.Items[0].ToString());
                    }
                }
            }
            //else if (THSelectedSourceType.Contains("RPGMaker") || THSelectedSourceType.Contains("RPG Maker"))
            //{

            //}
            return fName + (IsSaveAs ? "_" + DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.GetCultureInfo("en-US")) : string.Empty);
        }

        public void WriteDictToXMLDB(Dictionary<string, string> db, string xmlPath)
        {
            XElement el = new XElement("TranslationCache",
                db.Select(kv =>
                new XElement("Value",
                    new XElement("Original", kv.Key),
                    new XElement("Translation", kv.Value)
                    )
                ));
            //el.Save("cache.xml");
            WriteXElementToXMLFile(el, xmlPath);
        }

        internal static Dictionary<string, string> ReadXMLDBToDictionary(string xmlPath)
        {
            //Dictionary<string, string> db = new Dictionary<string, string>();
            //XElement rootElement = XElement.Parse(ReadXMLToString(xmlPath));//ошибка xml на символ x1E в значении Original, там символом &#x1E; сохранен спецсимвол
            //foreach (var el in rootElement.Elements())
            //{
            //    string key = el.Element("Original").Value;
            //    if (!db.ContainsKey(key))
            //    {
            //        db.Add(key, el.Element("Translation").Value);
            //    }
            //}

            int OriginalLength = "Original".Length;
            Dictionary<string, string> db = new Dictionary<string, string>();
            //var settings = new XmlReaderSettings();
            string original=string.Empty;
            bool WaitingTranslation = false;

            //https://stackoverflow.com/questions/2441673/reading-xml-with-xmlreader-in-c-sharp
            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (WaitingTranslation)
                        {
                            if (reader.Name == "Translation")
                            {
                                if (XNode.ReadFrom(reader) is XElement el)
                                {
                                    db.Add(original, el.Value);
                                    WaitingTranslation = false;
                                }
                            }
                        }
                        else
                        {
                            if (reader.Name.Length != OriginalLength)
                                continue;

                            if (reader.Name == "Original")
                            {
                                try
                                {
                                    if (XNode.ReadFrom(reader) is XElement el)
                                    {
                                        if (!db.ContainsKey(el.Value))
                                        {
                                            original = el.Value;
                                            WaitingTranslation = true;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }


            return db;
        }

        internal static string ReadXMLToString(string xmlPath)
        {
            using (FileStream fs = new FileStream(xmlPath, FileMode.Open))
            {
                Stream s;
                string fileExtension = Path.GetExtension(xmlPath);
                if (fileExtension == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Decompress);
                }
                else if (fileExtension == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Decompress);
                }
                else
                {
                    s = fs;
                }
                //return s;
                string stringForReturn;
                using (StreamReader sr = new StreamReader(s))
                {
                    stringForReturn = sr.ReadToEnd();
                }
                return stringForReturn;
                //s.Close();
            }
        }

        internal static void WriteXElementToXMLFile(XElement el, string xmlPath)
        {
            using (FileStream fs = new FileStream(xmlPath, FileMode.Create))
            {
                Stream s;
                string fileExtension = Path.GetExtension(xmlPath);
                if (fileExtension == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Compress);
                }
                else if (fileExtension == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Compress);
                }
                else
                {
                    s = fs;
                }
                el.Save(s);
                s.Close();
            }
        }

        internal static Dictionary<string, string> DataSetToDictionary(DataSet dBDataSet)
        {
            Dictionary<string, string> db = new Dictionary<string, string>();

            int TablesCount = dBDataSet.Tables.Count;

            for (int t=0;t< TablesCount; t++)
            {
                int RowsCount = dBDataSet.Tables[t].Rows.Count;

                for (int r = 0; r < RowsCount; r++)
                {
                    if (!db.ContainsKey(dBDataSet.Tables[t].Rows[r][0] as string))
                    {
                        db.Add(dBDataSet.Tables[t].Rows[r][0] as string, dBDataSet.Tables[t].Rows[r][1]+string.Empty);
                    }
                }
            }

            return db;
        }
    }
}

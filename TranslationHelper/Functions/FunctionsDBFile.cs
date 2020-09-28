using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsDBFile
    {
        private const string baseNamePattern = @"^(.+)_[0-9]{2,4}\.[0-9]{2}\.[0-9]{2,4} [0-9]{2}-[0-9]{2}-[0-9]{2}$";

        public static void WriteTranslationCacheIfValid(DataSet THTranslationCache, string tHTranslationCachePath)
        {
            if (Properties.Settings.Default.EnableTranslationCache && !Properties.Settings.Default.IsTranslationHelperWasClosed && THTranslationCache.Tables[0].Rows.Count > 0)
            {
                FunctionsDBFile.WriteDBFile(THTranslationCache, tHTranslationCachePath);
                //THTranslationCache.Reset();
            }
        }


        //https://stackoverflow.com/questions/223738/net-stream-dataset-of-xml-data-to-zip-file
        //http://madprops.org/blog/saving-datasets-locally-with-compression/
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5366:Использовать XmlReader для чтения XML из набора данных", Justification = "<Ожидание>")]
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
                //XmlReaderSettings set = new XmlReaderSettings();
                //using (var xr = XmlReader.Create(s/*, set*/))
                //{
                //    DS.ReadXml(xr);//с этим не может читать hex в xml и не грузит многострочные
                //}
                DS.ReadXml(s);
                s.Close();
            }
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        //public static string SanitizeXmlString(string xml)
        //{
        //    if (xml == null)
        //    {
        //        throw new ArgumentNullException(nameof(xml));
        //    }

        //    StringBuilder buffer = new StringBuilder(xml.Length);

        //    foreach (char c in xml)
        //    {
        //        if (IsLegalXmlChar(c))
        //        {
        //            buffer.Append(c);
        //        }
        //    }

        //    return buffer.ToString();
        //}

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        //public static bool IsLegalXmlChar(int character)
        //{
        //    return
        //    (
        //         character == 0x9 /* == '\t' == 9   */          ||
        //         character == 0xA /* == '\n' == 10  */          ||
        //         character == 0xD /* == '\r' == 13  */          ||
        //        (character >= 0x20 && character <= 0xD7FF) ||
        //        (character >= 0xE000 && character <= 0xFFFD) ||
        //        (character >= 0x10000 && character <= 0x10FFFF)
        //    );
        //}

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

        internal static string GetProjectDBFolder(THDataWork thDataWork = null)
        {
            string ret = string.Empty;
            if (thDataWork != null && thDataWork.CurrentProject != null)
            {
                ret = thDataWork.CurrentProject.ProjectFolderName();
            }
            else if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker MV"))
            {
                ret = "RPGMakerMV";
            }
            else if (RPGMFunctions.THSelectedSourceType.Contains("RPGMaker") || RPGMFunctions.THSelectedSourceType.Contains("RPG Maker"))
            {
                ret = "RPGMakerTransPatch";
            }

            ret = Path.Combine(Application.StartupPath, "DB", ret.Length > 0 ? ret : "Other");
            if (!Directory.Exists(ret))
            {
                Directory.CreateDirectory(ret);
            }

            return ret;
        }

        internal static string GetDBFileName(THDataWork thDataWork, bool IsSaveAs = false)
        {
            string fName = Path.GetFileName(Properties.Settings.Default.THSelectedDir);
            if (thDataWork.CurrentProject != null && thDataWork.CurrentProject.GetProjectDBFileName().Length > 0)
            {
                fName = thDataWork.CurrentProject.GetProjectDBFileName();
            }
            else if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker MV"))
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
            return fName + (IsSaveAs ? "_" + DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.InvariantCulture) : string.Empty);
        }

        //public static void WriteDictToXMLDB(Dictionary<string, string> db, string xmlPath)
        //{
        //    XElement el = new XElement("TranslationCache",
        //        db.Select(kv =>
        //        new XElement("Value",
        //            new XElement("Original", kv.Key),
        //            new XElement("Translation", kv.Value)
        //            )
        //        ));

        //    //el = new XElement("TranslationCache");
        //    //foreach (var kv in db)
        //    //{
        //    //    el.Add(new XElement("Value",
        //    //        new XElement("Original", kv.Key),
        //    //        new XElement("Translation", kv.Value)
        //    //        ));
        //    //}

        //    //el.Save("cache.xml");

        //    WriteXElementToXMLFile(el, xmlPath);
        //}

        internal static Dictionary<string, string> ReadXMLDBToDictionary(string xmlPath)
        {
            int OriginalLength = "Original".Length;
            Dictionary<string, string> db = new Dictionary<string, string>();
            //var settings = new XmlReaderSettings();
            string original = string.Empty;
            bool WaitingTranslation = false;
            XmlReaderSettings settings = new XmlReaderSettings
            {
                XmlResolver = null
            };
            //https://stackoverflow.com/questions/2441673/reading-xml-with-xmlreader-in-c-sharp
            using (XmlReader reader = XmlReader.Create(xmlPath, settings))
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
            if (!File.Exists(xmlPath))
            {
                return string.Empty;
            }

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

                string stringForReturn;
                using (StreamReader sr = new StreamReader(s))
                {
                    stringForReturn = sr.ReadToEnd();
                }
                return stringForReturn;
            }
        }

        internal static void WriteXElementToXMLFile(XElement el, string xmlPath)
        {
            using (FileStream fs = new FileStream(xmlPath, FileMode.Create))
            {
                Stream s = null;
                try
                {
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
                catch
                {
                    if (el != null && s != null)
                    {
                        el.Save(s);
                        s.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dBDataSet"></param>
        /// <param name="inputDB"></param>
        /// <param name="DontAddEmptyTranslation"></param>
        /// <param name="DontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> DBDataSetToDBDictionary(this DataSet dBDataSet, Dictionary<string, string> inputDB = null, bool DontAddEmptyTranslation = true, bool DontAddEqualTranslation = false)
        {
            Dictionary<string, string> db;
            if (inputDB == null)
            {
                db = new Dictionary<string, string>();
            }
            else
            {
                db = inputDB;
            }

            int TablesCount = dBDataSet.Tables.Count;

            for (int t = 0; t < TablesCount; t++)
            {
                try
                {
                    var table = dBDataSet.Tables[t];
                    if (!table.Columns.Contains("Original") || !table.Columns.Contains("Translation"))
                    {
                        continue;
                    }

                    int RowsCount = table.Rows.Count;

                    for (int r = 0; r < RowsCount; r++)
                    {
                        var row = table.Rows[r];
                        if (!db.ContainsKey(row["Original"] as string))
                        {
                            if ((DontAddEmptyTranslation && (row["Translation"] == null || string.IsNullOrEmpty(row["Translation"] as string))) || (DontAddEqualTranslation && row["Translation"] as string == row["Original"] as string))
                            {
                                continue;
                            }

                            db.Add(row["Original"] as string, row["Translation"] + string.Empty);
                        }
                    }
                }
                catch
                {
                }
            }

            return db;
        }

        internal static Dictionary<string, string> GetTableRowsDataToDictionary(this DataSet dBDataSet)
        {
            Dictionary<string, string> db = new Dictionary<string, string>();

            int TablesCount = dBDataSet.Tables.Count;

            for (int t = 0; t < TablesCount; t++)
            {
                int RowsCount = dBDataSet.Tables[t].Rows.Count;

                for (int r = 0; r < RowsCount; r++)
                {
                    var row = dBDataSet.Tables[t].Rows[r];
                    if (db.ContainsKey(row[0] as string))
                    {
                        if (row[1] == null || string.IsNullOrEmpty(row[1] + string.Empty))
                        {
                            db[row[0] as string] = db[row[0] as string] + "|" + t + "!" + r;
                        }
                    }
                    else
                    {
                        db.Add(row[0] as string, t + "!" + r);
                    }
                }
            }

            return db;
        }

        internal static void MergeAllDBtoOne(THDataWork thDataWork)
        {
            if (thDataWork.AllDBmerged == null)
            {
                thDataWork.AllDBmerged = new Dictionary<string, string>();
            }

            var newestFilesList = GetNewestFIlesList(THSettingsData.DBDirPath());

            foreach (var DBfile in newestFilesList)
            {
                try
                {
                    using (var DBDataSet = new DataSet())
                    {
                        thDataWork.Main.ProgressInfo(true, T._("Loading") + " " + Path.GetFileName(DBfile.Value.Name));

                        ReadDBFile(DBDataSet, DBfile.Value.FullName);
                        DBDataSet.DBDataSetToDBDictionary(thDataWork.AllDBmerged, true, true);
                    }
                }
                catch
                {
                }
            }
        }

        private static List<KeyValuePair<string, FileInfo>> GetNewestFIlesList(string DBDir)
        {
            var info = new Dictionary<string, FileInfo>();
            foreach (var DBFile in Directory.EnumerateFiles(DBDir, "*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(DBFile);
                if ((ext != ".xml" && ext != ".cmx" && ext != ".cmz") || DBFile.Contains("THTranslationCache") || DBFile.Contains("_autosave") || Path.GetFileName(Path.GetDirectoryName(DBFile))==THSettingsData.DBAutoSavesDirName())
                {
                    continue;
                }

                var baseName = GetBaseDBFileName(DBFile);

                if (info.ContainsKey(baseName))
                {
                    var DBFInfo = new FileInfo(DBFile);
                    if (DBFInfo.LastWriteTime > info[baseName].LastWriteTime)
                    {
                        info[baseName] = DBFInfo;
                    }
                }
                else
                {
                    info.Add(baseName, new FileInfo(DBFile));
                }
            }

            //sort form newest to oldest

            var sortedList = info.ToList();

            sortedList.Sort((pair1, pair2) => pair1.Value.LastWriteTime.CompareTo(pair2.Value.LastWriteTime));
            sortedList.Reverse();

            return sortedList;
        }

        private static string GetBaseDBFileName(string DBfile)
        {
            string baseName = Path.GetFileNameWithoutExtension(DBfile);
            if (Regex.IsMatch(baseName, baseNamePattern))
            {
                baseName = Regex.Replace(baseName, baseNamePattern, "$1");
            }
            return baseName;
        }

        private static string FindNewestFile(string tDir, string DBfile, HashSet<string> paths = null)
        {
            var baseName = GetBaseDBFileName(DBfile);
            string newestfile = DBfile;
            foreach (var file in Directory.GetFiles(tDir, baseName + "*.*", SearchOption.AllDirectories))
            {
                if (paths != null && !paths.Contains(file))
                {
                    paths.Add(file);
                }

                if (file != newestfile && new FileInfo(file).LastWriteTime > new FileInfo(newestfile).LastWriteTime)
                {
                    newestfile = file;
                }
            }

            return newestfile;
        }
    }
}

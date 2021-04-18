using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.INISettings;
using TranslationHelper.Projects.RPGMMV;

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
        /// <summary>
        /// read xml file
        /// </summary>
        /// <param name="DS"></param>
        /// <param name="fileName"></param>
        public static void ReadDBFile(DataSet DS, string fileName)
        {
            ReadWriteDBFile(DS, fileName);
        }

        // <summary>
        // Remove illegal XML characters from a string.
        // </summary>
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
        // <summary>
        // Whether a given character is allowed by XML 1.0.
        // </summary>
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

        /// <summary>
        /// write xml file
        /// </summary>
        /// <param name="DS"></param>
        /// <param name="fileName"></param>
        public static void WriteDBFile(DataSet DS, string fileName)
        {
            ReadWriteDBFile(DS, fileName, false);
        }

        /// <summary>
        /// read or write db file
        /// </summary>
        /// <param name="DS"></param>
        /// <param name="fileName"></param>
        /// <param name="read"></param>
        internal static void ReadWriteDBFile(DataSet DS, string fileName, bool read = true)
        {
            var DBFormat = FunctionsInterfaces.GetCurrentDBFormat();
            fileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "." + DBFormat.Ext);
            using (var fs = new FileStream(fileName, read ? FileMode.Open : FileMode.Create))
            {
                Stream s;
                //string fileExtension = Path.GetExtension(fileName);
                s = DBFormat.FileStreamMod(fs, read);

                if (read)
                {
                    DS.ReadXml(s);
                }
                else
                {
                    DS.WriteXml(s);
                }

                s.Close();
            }
        }

        /// <summary>
        /// gets current selected format of database file
        /// </summary>
        /// <returns></returns>
        internal static IDBSave GetCurrentDBFormat()
        {
            IDBSave Format = new XML();
            foreach (var f in SettingsBaseTools.GetListOfInterfaceImplimentations<IDBSave>())
            {
                if (f.Description == Properties.Settings.Default.DBCompressionExt)
                {
                    return f;
                }
            }

            return Format;
        }

        internal static string GetDBCompressionExt(THDataWork thDataWork = null)
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (TranslationHelper.Properties.Settings.Default.DBCompression)
            {
                return "." + FunctionsInterfaces.GetCurrentDBFormat().Ext;
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
            else if (RPGMFunctions.THSelectedSourceType.Contains(new RPGMMVGame(thDataWork).Name()))
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
            else if (thDataWork.Main.THFilesList.Items.Count == 1 && thDataWork.Main.THFilesList.Items[0] != null && !string.IsNullOrWhiteSpace(thDataWork.Main.THFilesList.Items[0].ToString()))
            {
                //dbfilename as name of single file in files list
                fName = Path.GetFileNameWithoutExtension(thDataWork.Main.THFilesList.Items[0].ToString());
            }
            //else if (THSelectedSourceType.Contains("RPGMaker") || THSelectedSourceType.Contains("RPG Maker"))
            //{

            //}
            return fName + (IsSaveAs ? "_" + DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.InvariantCulture) : string.Empty);
        }

        public static void WriteDictToXMLDB(Dictionary<string, string> db, string xmlPath)
        {
            XElement el = new XElement("TranslationCache",
                db.Select(kv =>
                new XElement("Value",
                    new XElement("Original", kv.Key),
                    new XElement("Translation", kv.Value)
                    )
                ));

            //el = new XElement("TranslationCache");
            //foreach (var kv in db)
            //{
            //    el.Add(new XElement("Value",
            //        new XElement("Original", kv.Key),
            //        new XElement("Translation", kv.Value)
            //        ));
            //}

            //el.Save("cache.xml");

            WriteXElementToXMLFile(el, xmlPath);
        }

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

        private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        internal static void WriteXElementToXMLFile(XElement el, string xmlPath)
        {
            locker.EnterWriteLock();

            using (var fs = new FileStream(xmlPath, FileMode.Create))
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

            locker.ExitWriteLock();
        }

        /// <summary>
        /// Set strng,string dataset to dictionary
        /// </summary>
        /// <param name="dBDataSet"></param>
        /// <param name="inputDB"></param>
        /// <param name="DontAddEmptyTranslation"></param>
        /// <param name="DontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> ToDictionary(this DataSet dBDataSet, Dictionary<string, string> inputDB = null, bool DontAddEmptyTranslation = true, bool DontAddEqualTranslation = false)
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

        /// <summary>
        /// Set -string,string- dataset to dictionary of coordinates
        /// </summary>
        /// <param name="dBDataSet"></param>
        /// <param name="inputDB"></param>
        /// <param name="DontAddEmptyTranslation"></param>
        /// <param name="DontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> ToDictionary2(this DataSet dBDataSet, Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> inputDB = null)
        {
            Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db;
            if (inputDB == null)
            {
                db = new Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>>();
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
                        var O = row["Original"] as string;

                        if (!db.ContainsKey(O))
                        {
                            db.Add(O, new Dictionary<string, Dictionary<int, string>>());
                        }

                        if (db[O].Values.Count == 0 || !db[O].ContainsKey(table.TableName))
                        {
                            db[O].Add(table.TableName, new Dictionary<int, string>());
                        }

                        db[O][table.TableName].Add(r, row["Translation"] + string.Empty);
                    }
                }
                catch
                {
                }
            }

            return db;
        }

        /// <summary>
        /// set dictionary string,string to dataset
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        internal static DataSet ToDataSet(this Dictionary<string, string> dict)
        {
            using (var DS = new DataSet())
            {
                DS.Tables.Add("DB");
                DS.Tables["DB"].Columns.Add("Original");
                DS.Tables["DB"].Columns.Add("Translation");

                foreach (var pair in dict)
                {
                    DS.Tables["DB"].Rows.Add(pair.Key, pair.Value);
                }

                return DS;
            }
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
                        DBDataSet.ToDictionary(thDataWork.AllDBmerged, true, true);
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
                if ((ext != ".xml" && ext != ".cmx" && ext != ".cmz") || DBFile.Contains("THTranslationCache") || DBFile.Contains("_autosave") || Path.GetFileName(Path.GetDirectoryName(DBFile)) == THSettingsData.DBAutoSavesDirName())
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

        /// <summary>
        /// search if path exists for any extension from exist DB formats
        /// </summary>
        /// <param name="DBPath"></param>
        internal static void SearchByAllDBFormatExtensions(ref string DBPath)
        {
            var dir = Path.GetDirectoryName(DBPath);
            var name = Path.GetFileNameWithoutExtension(DBPath);
            foreach (var format in FunctionsInterfaces.GetDBSaveFormats())
            {
                var PathForFormat = Path.Combine(dir, name + "." + format.Ext);
                if (File.Exists(PathForFormat))
                {
                    DBPath = PathForFormat;
                    return;
                }
            }
        }
    }
}

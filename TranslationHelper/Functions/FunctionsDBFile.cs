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
    static class FunctionsDbFile
    {
        private const string BaseNamePattern = @"^(.+)_[0-9]{2,4}\.[0-9]{2}\.[0-9]{2,4} [0-9]{2}-[0-9]{2}-[0-9]{2}$";

        public static void WriteTranslationCacheIfValid(DataSet thTranslationCache, string tHTranslationCachePath)
        {
            if (Properties.Settings.Default.EnableTranslationCache && !Properties.Settings.Default.IsTranslationHelperWasClosed && thTranslationCache.Tables[0].Rows.Count > 0)
            {
                FunctionsDbFile.WriteDbFile(thTranslationCache, tHTranslationCachePath);
                //THTranslationCache.Reset();
            }
        }


        //https://stackoverflow.com/questions/223738/net-stream-dataset-of-xml-data-to-zip-file
        //http://madprops.org/blog/saving-datasets-locally-with-compression/
        /// <summary>
        /// read xml file
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="fileName"></param>
        public static void ReadDbFile(DataSet ds, string fileName)
        {
            ReadWriteDbFile(ds, fileName);
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
        /// <param name="ds"></param>
        /// <param name="fileName"></param>
        public static void WriteDbFile(DataSet ds, string fileName)
        {
            ReadWriteDbFile(ds, fileName, false);
        }

        /// <summary>
        /// read or write db file
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="fileName"></param>
        /// <param name="read"></param>
        internal static void ReadWriteDbFile(DataSet ds, string fileName, bool read = true)
        {
            var dbFormat = FunctionsInterfaces.GetCurrentDbFormat();
            fileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "." + dbFormat.Ext);
            using (var fs = new FileStream(fileName, read ? FileMode.Open : FileMode.Create))
            {
                Stream s;
                //string fileExtension = Path.GetExtension(fileName);
                s = dbFormat.FileStreamMod(fs, read);

                if (read)
                {
                    ds.ReadXml(s);
                }
                else
                {
                    ds.WriteXml(s);
                }

                s.Close();
            }
        }

        /// <summary>
        /// gets current selected format of database file
        /// </summary>
        /// <returns></returns>
        internal static IDbSave GetCurrentDbFormat()
        {
            IDbSave format = new Xml();
            foreach (var f in GetListOfSubClasses.Inherited.GetListOfInterfaceImplimentations<IDbSave>())
            {
                if (f.Description == Properties.Settings.Default.DBCompressionExt)
                {
                    return f;
                }
            }

            return format;
        }

        internal static string GetDbCompressionExt()
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (TranslationHelper.Properties.Settings.Default.DBCompression)
            {
                return "." + FunctionsInterfaces.GetCurrentDbFormat().Ext;
            }
            //MessageBox.Show("Default .xml");
            return ".xml";
        }

        internal static string GetProjectDbFolder()
        {
            string ret = string.Empty;
            if (ProjectData.CurrentProject != null)
            {
                ret = ProjectData.CurrentProject.ProjectFolderName();
            }
            else if (RpgmFunctions.ThSelectedSourceType.Contains("RPG Maker MV"))
            {
                ret = "RPGMakerMV";
            }
            else if (RpgmFunctions.ThSelectedSourceType.Contains("RPGMaker") || RpgmFunctions.ThSelectedSourceType.Contains("RPG Maker"))
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

        internal static string GetDbFileName(bool isSaveAs = false)
        {
            string fName = Path.GetFileName(ProjectData.SelectedDir);
            if (ProjectData.CurrentProject != null && ProjectData.CurrentProject.GetProjectDbFileName().Length > 0)
            {
                fName = ProjectData.CurrentProject.GetProjectDbFileName();
            }
            else if (RpgmFunctions.ThSelectedSourceType.Contains(new RpgmmvGame().Name()))
            {
                if (ProjectData.Main.THFilesList.Items.Count == 1 && ProjectData.Main.THFilesList.Items[0] != null && !string.IsNullOrWhiteSpace(ProjectData.Main.THFilesList.Items[0].ToString()))
                {
                    if (fName == "data")
                    {
                        fName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(ProjectData.SelectedDir))) + "_" + Path.GetFileNameWithoutExtension(ProjectData.Main.THFilesList.Items[0].ToString());
                    }
                    else
                    {
                        fName = Path.GetFileNameWithoutExtension(ProjectData.Main.THFilesList.Items[0].ToString());
                    }
                }
            }
            else if (ProjectData.Main.THFilesList.Items.Count == 1 && ProjectData.Main.THFilesList.Items[0] != null && !string.IsNullOrWhiteSpace(ProjectData.Main.THFilesList.Items[0].ToString()))
            {
                //dbfilename as name of single file in files list
                fName = Path.GetFileNameWithoutExtension(ProjectData.Main.THFilesList.Items[0].ToString());
            }
            //else if (THSelectedSourceType.Contains("RPGMaker") || THSelectedSourceType.Contains("RPG Maker"))
            //{

            //}
            return fName + (isSaveAs ? "_" + DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.InvariantCulture) : string.Empty);
        }

        public static void WriteDictToXmldb(Dictionary<string, string> db, string xmlPath)
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

            WriteXElementToXmlFile(el, xmlPath);
        }

        internal static Dictionary<string, string> ReadXmldbToDictionary(string xmlPath)
        {
            int originalLength = "Original".Length;
            Dictionary<string, string> db = new Dictionary<string, string>();
            //var settings = new XmlReaderSettings();
            string original = string.Empty;
            bool waitingTranslation = false;
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
                        if (waitingTranslation)
                        {
                            if (reader.Name == "Translation")
                            {
                                if (XNode.ReadFrom(reader) is XElement el)
                                {
                                    db.Add(original, el.Value);
                                    waitingTranslation = false;
                                }
                            }
                        }
                        else
                        {
                            if (reader.Name.Length != originalLength)
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
                                            waitingTranslation = true;
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

        internal static string ReadXmlToString(string xmlPath)
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

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        internal static void WriteXElementToXmlFile(XElement el, string xmlPath)
        {
            Locker.EnterWriteLock();

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

            Locker.ExitWriteLock();
        }

        /// <summary>
        /// Set strng,string dataset to dictionary
        /// </summary>
        /// <param name="dBDataSet"></param>
        /// <param name="inputDb"></param>
        /// <param name="dontAddEmptyTranslation"></param>
        /// <param name="dontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> ToDictionary(this DataSet dBDataSet, Dictionary<string, string> inputDb = null, bool dontAddEmptyTranslation = true, bool dontAddEqualTranslation = false)
        {
            Dictionary<string, string> db;
            if (inputDb == null)
            {
                db = new Dictionary<string, string>();
            }
            else
            {
                db = inputDb;
            }

            int tablesCount = dBDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                try
                {
                    var table = dBDataSet.Tables[t];
                    if (!table.Columns.Contains("Original") || !table.Columns.Contains("Translation"))
                    {
                        continue;
                    }

                    int rowsCount = table.Rows.Count;

                    for (int r = 0; r < rowsCount; r++)
                    {
                        var row = table.Rows[r];
                        if (!db.ContainsKey(row["Original"] as string))
                        {
                            if ((dontAddEmptyTranslation && (row["Translation"] == null || string.IsNullOrEmpty(row["Translation"] as string))) || (dontAddEqualTranslation && row["Translation"] as string == row["Original"] as string))
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
        /// <param name="inputDb"></param>
        /// <param name="DontAddEmptyTranslation"></param>
        /// <param name="DontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> ToDictionary2(this DataSet dBDataSet, Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> inputDb = null)
        {
            Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db;
            if (inputDb == null)
            {
                db = new Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>>();
            }
            else
            {
                db = inputDb;
            }

            int tablesCount = dBDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                try
                {
                    var table = dBDataSet.Tables[t];
                    if (!table.Columns.Contains("Original") || !table.Columns.Contains("Translation"))
                    {
                        continue;
                    }

                    int rowsCount = table.Rows.Count;

                    for (int r = 0; r < rowsCount; r++)
                    {
                        var row = table.Rows[r];
                        var o = row["Original"] as string;

                        if (!db.ContainsKey(o))
                        {
                            db.Add(o, new Dictionary<string, Dictionary<int, string>>());
                        }

                        if (db[o].Values.Count == 0 || !db[o].ContainsKey(table.TableName))
                        {
                            db[o].Add(table.TableName, new Dictionary<int, string>());
                        }

                        db[o][table.TableName].Add(r, row["Translation"] + string.Empty);
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
            //using (var DS = new DataSet())
            {
                var ds = new DataSet();
                ds.Tables.Add("DB");
                ds.Tables["DB"].Columns.Add("Original");
                ds.Tables["DB"].Columns.Add("Translation");

                foreach (var pair in dict)
                {
                    ds.Tables["DB"].Rows.Add(pair.Key, pair.Value);
                }

                return ds;
            }
        }

        internal static Dictionary<string, string> GetTableRowsDataToDictionary(this DataSet dBDataSet)
        {
            Dictionary<string, string> db = new Dictionary<string, string>();

            int tablesCount = dBDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                int rowsCount = dBDataSet.Tables[t].Rows.Count;

                for (int r = 0; r < rowsCount; r++)
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

        internal static void MergeAllDBtoOne()
        {
            if (ProjectData.AllDBmerged == null)
            {
                ProjectData.AllDBmerged = new Dictionary<string, string>();
            }

            var newestFilesList = GetNewestFIlesList(ThSettings.DbDirPath());

            foreach (var dBfile in newestFilesList)
            {
                try
                {
                    using (var dbDataSet = new DataSet())
                    {
                        ProjectData.Main.ProgressInfo(true, T._("Loading") + " " + Path.GetFileName(dBfile.Value.Name));

                        ReadDbFile(dbDataSet, dBfile.Value.FullName);
                        dbDataSet.ToDictionary(ProjectData.AllDBmerged, true, true);
                    }
                }
                catch
                {
                }
            }
        }

        private static List<KeyValuePair<string, FileInfo>> GetNewestFIlesList(string dbDir)
        {
            var info = new Dictionary<string, FileInfo>();
            foreach (var dbFile in Directory.EnumerateFiles(dbDir, "*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(dbFile);
                if ((ext != ".xml" && ext != ".cmx" && ext != ".cmz") || dbFile.Contains("THTranslationCache") || dbFile.Contains("_autosave") || Path.GetFileName(Path.GetDirectoryName(dbFile)) == ThSettings.DbAutoSavesDirName())
                {
                    continue;
                }

                var baseName = GetBaseDbFileName(dbFile);

                if (info.ContainsKey(baseName))
                {
                    var dbfInfo = new FileInfo(dbFile);
                    if (dbfInfo.LastWriteTime > info[baseName].LastWriteTime)
                    {
                        info[baseName] = dbfInfo;
                    }
                }
                else
                {
                    info.Add(baseName, new FileInfo(dbFile));
                }
            }

            //sort form newest to oldest

            var sortedList = info.ToList();

            sortedList.Sort((pair1, pair2) => pair1.Value.LastWriteTime.CompareTo(pair2.Value.LastWriteTime));
            sortedList.Reverse();

            return sortedList;
        }

        private static string GetBaseDbFileName(string dBfile)
        {
            string baseName = Path.GetFileNameWithoutExtension(dBfile);
            if (Regex.IsMatch(baseName, BaseNamePattern))
            {
                baseName = Regex.Replace(baseName, BaseNamePattern, "$1");
            }
            return baseName;
        }

        /// <summary>
        /// search if path exists for any extension from exist DB formats
        /// </summary>
        /// <param name="dbPath"></param>
        internal static void SearchByAllDbFormatExtensions(ref string dbPath)
        {
            var dir = Path.GetDirectoryName(dbPath);
            var name = Path.GetFileNameWithoutExtension(dbPath);
            foreach (var format in FunctionsInterfaces.GetDbSaveFormats())
            {
                var pathForFormat = Path.Combine(dir, name + "." + format.Ext);
                if (File.Exists(pathForFormat))
                {
                    dbPath = pathForFormat;
                    return;
                }
            }
        }
    }
}

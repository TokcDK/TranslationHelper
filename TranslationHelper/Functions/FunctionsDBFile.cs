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
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;
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
        /// <param name="dataSet"></param>
        /// <param name="dbFilePath"></param>
        public static void ReadDBFile(DataSet dataSet, string dbFilePath, bool useOriginaldbFilePath = false)
        {
            ReadWriteDBFile(dataSet: dataSet, dbFilePath: dbFilePath, useOriginaldbFilePath: useOriginaldbFilePath);
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
        /// <param name="dataSet"></param>
        /// <param name="dbFilePath"></param>
        public static void WriteDBFile(DataSet dataSet, string dbFilePath, bool useOriginaldbFilePath = false)
        {
            ReadWriteDBFile(dataSet: dataSet, dbFilePath: dbFilePath, read: false, useOriginaldbFilePath: useOriginaldbFilePath);
        }

        private static readonly ReaderWriterLockSlim WriteXmlLocker = new ReaderWriterLockSlim();
        /// <summary>
        /// read or write db file
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dbFilePath"></param>
        /// <param name="read"></param>
        internal static void ReadWriteDBFile(DataSet dataSet, string dbFilePath, bool read = true, bool useOriginaldbFilePath = false)
        {
            var DBFormat = FunctionsInterfaces.GetCurrentDBFormat();
            dbFilePath = useOriginaldbFilePath ? dbFilePath : Path.Combine(Path.GetDirectoryName(dbFilePath), Path.GetFileNameWithoutExtension(dbFilePath) + "." + DBFormat.Ext);
            using (var fs = new FileStream(dbFilePath, read ? FileMode.Open : FileMode.Create))
            {
                Stream s;
                //string fileExtension = Path.GetExtension(fileName);
                s = DBFormat.FileStreamMod(fs, read);

                if (read)
                {
                    dataSet.ReadXml(s);
                }
                else
                {
                    WriteXmlLocker.EnterWriteLock();
                    try
                    {
                        dataSet.WriteXml(s);
                    }
                    finally
                    {
                        WriteXmlLocker.ExitWriteLock();
                    }
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
            foreach (var f in GetListOfSubClasses.Inherited.GetListOfInterfaceImplimentations<IDBSave>())
            {
                if (f.Description == Properties.Settings.Default.DBCompressionExt)
                {
                    return f;
                }
            }

            return Format;
        }

        internal static string GetDBCompressionExt()
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (TranslationHelper.Properties.Settings.Default.DBCompression)
            {
                return "." + FunctionsInterfaces.GetCurrentDBFormat().Ext;
            }
            //MessageBox.Show("Default .xml");
            return ".xml";
        }

        internal static string GetProjectDBFolder()
        {
            string ret = string.Empty;
            if (ProjectData.CurrentProject != null)
            {
                ret = ProjectData.CurrentProject.ProjectFolderName();
            }
            //else if (ProjectData.CurrentProject.Name().Contains("RPG Maker MV"))
            //{
            //    ret = "RPGMakerMV";
            //}
            //else if (ProjectData.CurrentProject.Name().Contains("RPGMaker") || ProjectData.CurrentProject.Name().Contains("RPG Maker"))
            //{
            //    ret = "RPGMakerTransPatch";
            //}

            ret = Path.Combine(Application.StartupPath, "DB", ret.Length > 0 ? ret : "Other");
            Directory.CreateDirectory(ret);

            return ret;
        }

        internal static string GetDBFileName(bool IsSaveAs = false)
        {
            string fName = Path.GetFileName(ProjectData.SelectedDir);
            if (ProjectData.CurrentProject != null && ProjectData.CurrentProject.GetProjectDBFileName().Length > 0)
            {
                fName = ProjectData.CurrentProject.GetProjectDBFileName();
            }
            else if (ProjectData.CurrentProject.Name().Contains(new RPGMMVGame().Name()))
            {
                if (ProjectData.Main.THFilesList.GetItemsCount() == 1 && ProjectData.Main.THFilesList.GetItemName(0) != null && !string.IsNullOrWhiteSpace(ProjectData.Main.THFilesList.GetItemName(0).ToString()))
                {
                    if (fName == "data")
                    {
                        fName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(ProjectData.SelectedDir))) + "_" + Path.GetFileNameWithoutExtension(ProjectData.Main.THFilesList.GetItemName(0).ToString());
                    }
                    else
                    {
                        fName = Path.GetFileNameWithoutExtension(ProjectData.Main.THFilesList.GetItemName(0).ToString());
                    }
                }
            }
            else if (ProjectData.Main.THFilesList.GetItemsCount() == 1 && ProjectData.Main.THFilesList.GetItemName(0) != null && !string.IsNullOrWhiteSpace(ProjectData.Main.THFilesList.GetItemName(0).ToString()))
            {
                //dbfilename as name of single file in files list
                fName = Path.GetFileNameWithoutExtension(ProjectData.Main.THFilesList.GetItemName(0).ToString());
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
                    new XElement(THSettings.OriginalColumnName(), kv.Key),
                    new XElement(THSettings.TranslationColumnName(), kv.Value)
                    )
                ));

            //el = new XElement("TranslationCache");
            //foreach (var kv in db)
            //{
            //    el.Add(new XElement("Value",
            //        new XElement(THSettings.OriginalColumnName(), kv.Key),
            //        new XElement(THSettings.TranslationColumnName(), kv.Value)
            //        ));
            //}

            //el.Save("cache.xml");

            WriteXElementToXMLFile(el, xmlPath);
        }

        internal static Dictionary<string, string> ReadXMLDBToDictionary(string xmlPath)
        {
            int OriginalLength = THSettings.OriginalColumnName().Length;
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
                            if (reader.Name == THSettings.TranslationColumnName())
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

                            if (reader.Name == THSettings.OriginalColumnName())
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

        private static readonly ReaderWriterLockSlim WriteXElementToXMLFileLocker = new ReaderWriterLockSlim();

        internal static void WriteXElementToXMLFile(XElement el, string xmlPath)
        {
            WriteXElementToXMLFileLocker.EnterWriteLock();

            try
            {
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
            }
            finally
            {
                WriteXElementToXMLFileLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Set strng,string dataset to dictionary
        /// </summary>
        /// <param name="dBDataSet"></param>
        /// <param name="inputDB"></param>
        /// <param name="dontAddEmptyTranslation"></param>
        /// <param name="dontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> ToDictionary(this DataSet dBDataSet, Dictionary<string, string> inputDB = null, bool dontAddEmptyTranslation = true, bool dontAddEqualTranslation = false)
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
                    if (!table.Columns.Contains(THSettings.OriginalColumnName()) || !table.Columns.Contains(THSettings.TranslationColumnName()))
                    {
                        continue;
                    }

                    int RowsCount = table.Rows.Count;

                    for (int r = 0; r < RowsCount; r++)
                    {
                        var row = table.Rows[r];
                        if (!db.ContainsKey(row[THSettings.OriginalColumnName()] as string))
                        {
                            if ((dontAddEmptyTranslation && (row[THSettings.TranslationColumnName()] == null || string.IsNullOrEmpty(row[THSettings.TranslationColumnName()] as string))) || (dontAddEqualTranslation && row[THSettings.TranslationColumnName()] as string == row[THSettings.OriginalColumnName()] as string))
                            {
                                continue;
                            }

                            db.Add(row[THSettings.OriginalColumnName()] as string, row[THSettings.TranslationColumnName()] + string.Empty);
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

            int tablesCount = dBDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                try
                {
                    var table = dBDataSet.Tables[t];
                    if (!table.Columns.Contains(THSettings.OriginalColumnName()) || !table.Columns.Contains(THSettings.TranslationColumnName()))
                    {
                        continue;
                    }

                    int rowsCount = table.Rows.Count;

                    for (int r = 0; r < rowsCount; r++)
                    {
                        var row = table.Rows[r];
                        var O = row[THSettings.OriginalColumnName()] as string;

                        if (!db.ContainsKey(O))
                        {
                            db.Add(O, new Dictionary<string, Dictionary<int, string>>());
                        }

                        if (db[O].Values.Count == 0 || !db[O].ContainsKey(table.TableName))
                        {
                            db[O].Add(table.TableName, new Dictionary<int, string>());
                        }

                        db[O][table.TableName].Add(r, row[THSettings.TranslationColumnName()] + string.Empty);
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
        /// <param name="dictionary"></param>
        /// <returns></returns>
        internal static DataSet ToDataSet(this Dictionary<string, string> dictionary)
        {
            //using (var DS = new DataSet())
            {
                var dataSet = new DataSet();
                dataSet.Tables.Add("DB");
                dataSet.Tables["DB"].Columns.Add(THSettings.OriginalColumnName());
                dataSet.Tables["DB"].Columns.Add(THSettings.TranslationColumnName());

                foreach (var pair in dictionary)
                {
                    dataSet.Tables["DB"].Rows.Add(pair.Key, pair.Value);
                }

                return dataSet;
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

        internal static void MergeAllDBtoOne()
        {
            if (ProjectData.AllDBmerged == null)
            {
                ProjectData.AllDBmerged = new Dictionary<string, string>();
            }

            var newestFilesList = GetNewestFIlesList(THSettings.DBDirPath());

            foreach (var DBfile in newestFilesList)
            {
                try
                {
                    using (var DBDataSet = new DataSet())
                    {
                        ProjectData.Main.ProgressInfo(true, T._("Loading") + " " + Path.GetFileName(DBfile.Value.Name));

                        ReadDBFile(dataSet: DBDataSet, dbFilePath: DBfile.Value.FullName, useOriginaldbFilePath: true);
                        DBDataSet.ToDictionary(ProjectData.AllDBmerged, true, true);
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
                if ((ext != ".xml" && ext != ".cmx" && ext != ".cmz") || DBFile.Contains("THTranslationCache") || DBFile.Contains("_autosave") || Path.GetFileName(Path.GetDirectoryName(DBFile)) == THSettings.DBAutoSavesDirName())
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

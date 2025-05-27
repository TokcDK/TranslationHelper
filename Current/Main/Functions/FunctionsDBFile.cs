using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Projects.RPGMMV;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsDBFile
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string _baseNamePattern = @"^(.+)_[0-9]{2,4}\.[0-9]{2}\.[0-9]{2,4} [0-9]{2}-[0-9]{2}-[0-9]{2}$";

        public static void WriteTranslationCacheIfValid(DataSet translationCacheDataSet, string translationCachePath)
        {
            if (AppSettings.EnableTranslationCache && !AppSettings.IsTranslationHelperWasClosed && translationCacheDataSet.Tables[0].Rows.Count > 0)
            {
                WriteDBFile(translationCacheDataSet, translationCachePath);
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
        public static Task ReadDBFile(DataSet dataSet, string dbFilePath, bool useOriginaldbFilePath = false)
        {
            return ReadWriteDBFile(dataSet: dataSet, dbFilePath: dbFilePath, useOriginaldbFilePath: useOriginaldbFilePath);
        }

        /// <summary>
        /// File browser filters for Database Files
        /// </summary>
        /// <returns></returns>
        public static string GetDBFormatsFilters()
        {
            var formats = FunctionsInterfaces.GetDBSaveFormats();
            var formatsFilters = "DB file|" + string.Join("", formats.Select(f => $"*.{f.Ext};"));
            var formatsIndividualFilters = string.Join("", formats.Select(f => $"|{f.Description}|*.{f.Ext}"));
            return $"{formatsFilters}{formatsIndividualFilters}|All|*.*";
        }

        /// <summary>
        /// write xml file
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dbFilePath"></param>
        public static Task WriteDBFile(DataSet dataSet, string dbFilePath, bool useOriginaldbFilePath = false)
        {
            return ReadWriteDBFile(dataSet: dataSet, dbFilePath: dbFilePath, isRead: false, useOriginaldbFilePath: useOriginaldbFilePath);
        }

        private static readonly ReaderWriterLockSlim _writeDBWriteLocker = new ReaderWriterLockSlim();
        /// <summary>
        /// read or write db file
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dbFilePath"></param>
        /// <param name="isRead"></param>
        internal static Task ReadWriteDBFile(DataSet dataSet, string dbFilePath, bool isRead = true, bool useOriginaldbFilePath = false)
        {
            var dbFormat = FunctionsInterfaces.GetCurrentDBFormat(Path.GetExtension(dbFilePath));
            dbFilePath = useOriginaldbFilePath ? dbFilePath : Path.Combine(Path.GetDirectoryName(dbFilePath), Path.GetFileNameWithoutExtension(dbFilePath) + "." + dbFormat.Ext);
            Directory.CreateDirectory(Path.GetDirectoryName(dbFilePath));

            if (isRead)
            {
                dbFormat.Read(dbFilePath, dataSet);
            }
            else
            {
                lock (_writeDBWriteLocker)
                {
                    dbFormat.Write(dbFilePath, dataSet);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// read or write db file
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dbFilePaths">list of paths to read save</param>
        internal static async Task WriteDBFile(DataSet dataSet, string[] dbFilePaths, bool useOriginaldbFilePath = false)
        {
            foreach (var dbFilePath in dbFilePaths)
            {
                await ReadWriteDBFile(dataSet, dbFilePath, false, useOriginaldbFilePath);
            }
        }

        /// <summary>
        /// gets current selected format of database file
        /// </summary>
        /// <returns></returns>
        internal static IDataBaseFileFormat GetCurrentDBFormat(string ext = null)
        {
            IDataBaseFileFormat Format = new XML();
            foreach (var f in GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IDataBaseFileFormat>())
            {
                if (string.IsNullOrWhiteSpace(ext))
                {
                    if (f.Description == AppSettings.DBCompressionExt) return f;
                }
                else if (f.Ext == ext || "." + f.Ext == ext) return f;
            }

            return Format;
        }

        internal static string GetDBCompressionExt()
        {
            if (AppSettings.DBCompression) return "." + FunctionsInterfaces.GetCurrentDBFormat().Ext;
            return ".xml";
        }

        internal static string GetProjectDBFolder()
        {
            string projectDirName = string.Empty;
            if (AppData.CurrentProject != null) projectDirName = AppData.CurrentProject.ProjectDBFolderName;

            projectDirName = Path.Combine(THSettings.DBDirPathByLanguage, projectDirName.Length > 0 ? projectDirName : "Other");
            Directory.CreateDirectory(projectDirName);

            return projectDirName;
        }

        internal static string GetDBFileName(bool saveAs = false)
        {
            string fName = Path.GetFileName(AppData.CurrentProject.SelectedDir);
            if (AppData.CurrentProject != null && AppData.CurrentProject.ProjectDBFileName.Length > 0)
            {
                fName = AppData.CurrentProject.ProjectDBFileName;
            }
            else if (AppData.CurrentProject.Name.Contains(new RPGMMVGame().Name))
            {
                if (AppData.Main.THFilesList.GetItemsCount() == 1 && AppData.Main.THFilesList.GetItemName(0) != null && !string.IsNullOrWhiteSpace(AppData.Main.THFilesList.GetItemName(0).ToString()))
                {
                    if (fName == "data")
                    {
                        fName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(AppData.CurrentProject.SelectedDir))) + "_" + Path.GetFileNameWithoutExtension(AppData.Main.THFilesList.GetItemName(0).ToString());
                    }
                    else
                    {
                        fName = Path.GetFileNameWithoutExtension(AppData.Main.THFilesList.GetItemName(0).ToString());
                    }
                }
            }
            else if (AppData.Main.THFilesList.GetItemsCount() == 1 && AppData.Main.THFilesList.GetItemName(0) != null && !string.IsNullOrWhiteSpace(AppData.Main.THFilesList.GetItemName(0).ToString()))
            {
                //dbfilename as name of single file in files list
                fName = Path.GetFileNameWithoutExtension(AppData.Main.THFilesList.GetItemName(0).ToString());
            }
            return fName + (saveAs ? "_" + DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.InvariantCulture) : string.Empty);
        }

        public static void WriteDictToXMLDB(Dictionary<string, string> db, string xmlPath)
        {
            XElement el = new XElement("TranslationCache",
                db.Select(kv =>
                new XElement("Value",
                    new XElement(THSettings.OriginalColumnName, kv.Key),
                    new XElement(THSettings.TranslationColumnName, kv.Value)
                    )
                ));

            WriteXElementToXMLFile(el, xmlPath);
        }

        internal static Dictionary<string, string> ReadXMLDBToDictionary(string xmlPath)
        {
            int OriginalLength = THSettings.OriginalColumnName.Length;
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
                    if (reader.NodeType != XmlNodeType.Element) continue;

                    if (WaitingTranslation)
                    {
                        if (reader.Name != THSettings.TranslationColumnName) continue;
                        if (!(XNode.ReadFrom(reader) is XElement el)) continue;

                        db.Add(original, el.Value);
                        WaitingTranslation = false;
                    }
                    else
                    {
                        if (reader.Name.Length != OriginalLength) continue;
                        if (reader.Name != THSettings.OriginalColumnName) continue;

                        try
                        {
                            if (!(XNode.ReadFrom(reader) is XElement el)) continue;

                            if (!db.ContainsKey(el.Value))
                            {
                                original = el.Value;
                                WaitingTranslation = true;
                            }
                            else continue;
                        }
                        catch
                        {
                            continue;
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

        private static readonly ReaderWriterLockSlim _writeXElementToXMLFileLocker = new ReaderWriterLockSlim();

        internal static void WriteXElementToXMLFile(XElement el, string xmlPath)
        {
            _writeXElementToXMLFileLocker.EnterWriteLock();

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
                _writeXElementToXMLFileLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Set strng,string dataset to dictionary
        /// </summary>
        /// <param name="dbDataSet"></param>
        /// <param name="inputDB"></param>
        /// <param name="dontAddEmptyTranslation"></param>
        /// <param name="dontAddEqualTranslation"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> ToDictionary(this DataSet dbDataSet, Dictionary<string, string> inputDB = null, bool dontAddEmptyTranslation = true, bool dontAddEqualTranslation = false, object _dbDataSetToDictionaryAddLocker = null)
        {
            bool threaded = _dbDataSetToDictionaryAddLocker != null;
            Dictionary<string, string> db;
            if (inputDB == null)
            {
                db = new Dictionary<string, string>();
            }
            else
            {
                db = inputDB;
            }

            int tablesCount = dbDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                try
                {
                    var table = dbDataSet.Tables[t];
                    if (!table.Columns.Contains(THSettings.OriginalColumnName) || !table.Columns.Contains(THSettings.TranslationColumnName))
                    {
                        continue;
                    }

                    int rowsCount = table.Rows.Count;

                    for (int r = 0; r < rowsCount; r++)
                    {
                        var row = table.Rows[r];

                        if (threaded)
                        {
                            lock (_dbDataSetToDictionaryAddLocker)
                            {
                                AddRecordToDictionary(db, row, dontAddEmptyTranslation, dontAddEqualTranslation);
                            }
                        }
                        else
                        {
                            AddRecordToDictionary(db, row, dontAddEmptyTranslation, dontAddEqualTranslation);
                        }
                    }
                }
                catch
                {
                }
            }

            return db;
        }

        private static void AddRecordToDictionary(Dictionary<string, string> db, DataRow row, bool dontAddEmptyTranslation, bool dontAddEqualTranslation)
        {
            if (!db.ContainsKey(row.Field<string>(THSettings.OriginalColumnName)))
            {
                if ((dontAddEmptyTranslation && (row[THSettings.TranslationColumnName] == null || string.IsNullOrEmpty(row.Field<string>(THSettings.TranslationColumnName)))) || (dontAddEqualTranslation && row.Field<string>(THSettings.TranslationColumnName) == row.Field<string>(THSettings.OriginalColumnName)))
                {
                    return;
                }

                db.Add(row.Field<string>(THSettings.OriginalColumnName), row.Field<string>(THSettings.TranslationColumnName));
            }
        }

        /// <summary>
        /// Set -string,string- dataset to dictionary of coordinates
        /// </summary>
        /// <param name="dbDataSet"></param>
        /// <param name="inputDB"></param>
        /// <returns></returns>
        internal static Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> ToDictionary2(this DataSet dbDataSet, Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> inputDB = null)
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

            int tablesCount = dbDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                try
                {
                    var table = dbDataSet.Tables[t];
                    if (!table.Columns.Contains(THSettings.OriginalColumnName) || !table.Columns.Contains(THSettings.TranslationColumnName))
                    {
                        continue;
                    }

                    int rowsCount = table.Rows.Count;

                    for (int r = 0; r < rowsCount; r++)
                    {
                        var row = table.Rows[r];
                        var O = row.Field<string>(THSettings.OriginalColumnName);

                        if (!db.ContainsKey(O))
                        {
                            db.Add(O, new Dictionary<string, Dictionary<int, string>>());
                        }

                        if (db[O].Values.Count == 0 || !db[O].ContainsKey(table.TableName))
                        {
                            db[O].Add(table.TableName, new Dictionary<int, string>());
                        }

                        db[O][table.TableName].Add(r, row.Field<string>(THSettings.TranslationColumnName));
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
                dataSet.Tables["DB"].Columns.Add(THSettings.OriginalColumnName);
                dataSet.Tables["DB"].Columns.Add(THSettings.TranslationColumnName);

                foreach (var pair in dictionary) dataSet.Tables["DB"].Rows.Add(pair.Key, pair.Value);

                return dataSet;
            }
        }

        internal static Dictionary<string, string> GetTableRowsDataToDictionary(this DataSet dbDataSet)
        {
            Dictionary<string, string> db = new Dictionary<string, string>();

            int tablesCount = dbDataSet.Tables.Count;

            for (int t = 0; t < tablesCount; t++)
            {
                int rowsCount = dbDataSet.Tables[t].Rows.Count;

                for (int r = 0; r < rowsCount; r++)
                {
                    var row = dbDataSet.Tables[t].Rows[r];
                    if (db.ContainsKey(row.Field<string>(0)))
                    {
                        if (row[1] == null || string.IsNullOrEmpty(row.Field<string>(1)))
                        {
                            db[row.Field<string>(0)] = db[row.Field<string>(0)] + "|" + t + "!" + r;
                        }
                    }
                    else
                    {
                        db.Add(row.Field<string>(0), t + "!" + r);
                    }
                }
            }

            return db;
        }

        readonly static object _dbDataSetToDictionaryAddLocker = new object();

        internal static Task MergeAllDBtoOne()
        {
            lock (_dbDataSetToDictionaryAddLocker)
            {
                if (AppData.AllDBmerged != null && AppData.AllDBmerged.Count > 0) return Task.CompletedTask;

                if (AppData.AllDBmerged == null) AppData.AllDBmerged = new Dictionary<string, string>();

                var newestFilesList = GetNewestFIlesList(THSettings.DBDirPathByLanguage);

                foreach (var dbFile in newestFilesList)
                {
                    try
                    {
                        using (var dbDataSet = new DataSet())
                        {
                            Logger.Info(T._("Loading") + " " + Path.GetFileName(dbFile.Value.Name));

                            ReadDBFile(dataSet: dbDataSet, dbFilePath: dbFile.Value.FullName, useOriginaldbFilePath: true);
                            dbDataSet.ToDictionary(inputDB: AppData.AllDBmerged, dontAddEmptyTranslation: true, dontAddEqualTranslation: true, _dbDataSetToDictionaryAddLocker);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static List<KeyValuePair<string, FileInfo>> GetNewestFIlesList(string dbDir)
        {
            var info = new Dictionary<string, FileInfo>();
            foreach (var dbFile in Directory.EnumerateFiles(dbDir, "*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(dbFile);
                string dbFileName = Path.GetFileNameWithoutExtension(dbFile);
                if ((ext != ".xml" && ext != ".cmx" && ext != ".cmz")
                    || dbFile.Contains("THTranslationCache")
                    || dbFile.Contains("_autosave")
                    || Path.GetFileName(Path.GetDirectoryName(dbFile)) == THSettings.DBAutoSavesDirName)
                {
                    continue;
                }

                // skip when file name ends with '_bak#' or '_bak'
                if (Regex.IsMatch(dbFileName, @"_bak\d*$"))
                {
                    continue;
                }

                var baseName = GetBaseDBFileName(dbFile);

                if (info.ContainsKey(baseName))
                {
                    var dbFileInfo = new FileInfo(dbFile);
                    if (dbFileInfo.LastWriteTime > info[baseName].LastWriteTime)
                    {
                        info[baseName] = dbFileInfo;
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

        private static string GetBaseDBFileName(string dbFilePath)
        {
            string baseName = Path.GetFileNameWithoutExtension(dbFilePath);
            if (Regex.IsMatch(baseName, _baseNamePattern)) baseName = Regex.Replace(baseName, _baseNamePattern, "$1");

            return baseName;
        }

        /// <summary>
        /// search in <paramref name="inputDbPath"/> dir for any extension from exist DB formats
        /// </summary>
        /// <param name="inputDbPath"></param>
        internal static string SearchByAllDBFormatExtensions(string inputDbPath)
        {
            var dir = Path.GetDirectoryName(inputDbPath);
            var name = Path.GetFileNameWithoutExtension(inputDbPath);

            var foundDBFiles = new List<string>();

            var pathNextToSource = Path.Combine(AppData.CurrentProject.SelectedDir, Data.THSettings.TranslationFileSourceDirSuffix);
            foreach (var format in FunctionsInterfaces.GetDBSaveFormats()) // search in game dir
            {
                var PathForFormat = pathNextToSource + "." + format.Ext;
                if (!File.Exists(PathForFormat)) continue;

                foundDBFiles.Add(PathForFormat);
            }
            foreach (var format in FunctionsInterfaces.GetDBSaveFormats()) // search in the app db dir
            {
                var PathForFormat = Path.Combine(dir, name + "." + format.Ext);
                if (!File.Exists(PathForFormat)) continue;

                foundDBFiles.Add(PathForFormat);
            }

            if (foundDBFiles.Count == 0) return inputDbPath;
            if (foundDBFiles.Count == 1)
            {
                return foundDBFiles[0];
            }

            string newestDbPath = GetNewestDbPath(foundDBFiles[0], foundDBFiles[1]);

            if (foundDBFiles.Count == 2) return newestDbPath;

            for (int i = 2; i < foundDBFiles.Count; i++)
            {
                newestDbPath = GetNewestDbPath(newestDbPath, foundDBFiles[i]);
            }

            return newestDbPath;
        }

        public static string GetNewestDbPath(string lastautosavepath, string pathNextToSource)
        {
            var lastautosavepathTime = new FileInfo(lastautosavepath).LastWriteTime;
            var pathNextToSourceTime = new FileInfo(pathNextToSource).LastWriteTime;

            var dateTimeCompareResult = DateTime.Compare(lastautosavepathTime, pathNextToSourceTime);

            return dateTimeCompareResult <= 0 ? pathNextToSource : lastautosavepath;
        }

        public static async Task WriteDBFileLite(DataSet ds, string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                if (fileName.Length == 0 || ds == null) return;

                try
                {
                    new Thread(new ParameterizedThreadStart((obj) => AppData.Main.IndicateSaveProcess(T._("Saving") + "..."))).Start();

                    using (DataSet liteds = FunctionsTable.GetDataSetWithoutEmptyTableRows(ds))
                    {
                        await FunctionsDBFile.WriteDBFile(liteds, fileName).ConfigureAwait(false);
                    }

                    //AppData.Settings.THConfigINI.SetKey("Paths", "LastAutoSavePath", lastautosavepath);
                }
                catch (Exception ex) {
                    Logger.Warn("Error writing DB file: {0}", ex);
                }
            }
        }


        /// <summary>
        /// Load translation from selected DB
        /// </summary>
        /// <param name="forced">means load with current lines override even if they are not empty</param>
        internal static async Task LoadDBAs(bool forced = false)
        {
            //Do nothing if user will try to use Open menu before previous will be finished
            if (FunctionsUI.IsOpeningInProcess) return;

            FunctionsUI.IsOpeningInProcess = true;
            using (OpenFileDialog openBD = new OpenFileDialog())
            {
                openBD.Filter = FunctionsDBFile.GetDBFormatsFilters();

                openBD.InitialDirectory = FunctionsDBFile.GetProjectDBFolder();

                if (openBD.ShowDialog() == DialogResult.OK)
                {
                    if (openBD.FileName.Length > 0)
                    {
                        if (forced) await new ClearCells().AllT().ConfigureAwait(true);
                        await Task.Run(() => FunctionsDBFile.LoadTranslationFromDB(openBD.FileName, false, forced)).ConfigureAwait(true);
                    }
                }
            }
            FunctionsUI.IsOpeningInProcess = false;
        }

        internal static void UnLockDBLoad(bool unlock = true)
        {
            //Invoke((Action)(() =>
            //{
            //    LoadTranslationToolStripMenuItem.Enabled = unlock;
            //    LoadTrasnlationAsToolStripMenuItem.Enabled = unlock;
            //    LoadTrasnlationAsForcedToolStripMenuItem.Enabled = unlock;
            //}));
        }

        internal static async Task LoadDB(bool force = true)
        {
            if (AppData.CurrentProject.IsLoadingDB) return;

            AppData.CurrentProject.IsLoadingDB = true;

            await FunctionsLoadTranslationDB.LoadTranslationIfNeed(forceLoad: force, askIfLoadDB: false);

            AppData.CurrentProject.IsLoadingDB = false;
        }

        static bool LoadTranslationToolStripMenuItem_ClickIsBusy;
        internal static async Task LoadTranslationFromDB(string sPath = "", bool UseAllDB = false, bool forced = false)
        {
            if (LoadTranslationToolStripMenuItem_ClickIsBusy || (!UseAllDB && sPath.Length == 0))
            {
                return;
            }
            LoadTranslationToolStripMenuItem_ClickIsBusy = true;

            if (UseAllDB)
            {
                Logger.Info("Get all databases");
                await FunctionsDBFile.MergeAllDBtoOne();
                FunctionsLoadTranslationDB.THLoadDBCompareFromDictionaryParallellTables(AppData.AllDBmerged);
            }
            else
            {
                using (DataSet DBDataSet = new DataSet())
                {

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => ReadDBAndLoadDBCompare(DBDataSet, sPath, forced)).ConfigureAwait(true);
                }
            }


            _ = AppData.Main.THFileElementsDataGridView.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));


            LoadTranslationToolStripMenuItem_ClickIsBusy = false;
            FunctionsSounds.LoadDBCompleted();
            _ = AppData.Main.THFilesList.Invoke((Action)(() => AppData.Main.THFilesList.Refresh()));
        }

        public static async Task ReadDBAndLoadDBCompare(DataSet dbDataSet, string sPath, bool forceOverwriteTranslations = false)
        {
            if (sPath.Length == 0)
            {
                sPath = AppData.Settings.THConfigINI.GetKey("Paths", "LastAutoSavePath");
            }

            if (!File.Exists(sPath))
            {
                return;
            }

            Logger.Info(T._("Reading DB File") + "...");

            try
            {
                //load new data
                await FunctionsDBFile.ReadDBFile(dbDataSet, sPath).ConfigureAwait(false);

                //отключение DataSource для избежания проблем от изменений DataGridView
                //bool tableSourceWasCleaned = false;
                //if (ProjectData.Main.THFileElementsDataGridView.DataSource != null)
                //{
                //    tableSourceWasCleaned = true;
                //    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                //    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                //    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                //}

                //стандартное считывание. Самое медленное
                //new FunctionsLoadTranslationDB().THLoadDBCompare(DBDataSet);

                //считывание через словарь с предварительным чтением в dataset и конвертацией в словарь
                //Своего рода среднее решение, которое быстрее решения с сравнением из БД в DataSet
                //и не имеет проблем решения с чтением сразу в словарь, 
                //тут не нужно переписывать запись в xml, хотя запись таблицы в xml пишет все колонки и одинаковые значения, т.е. xml будет больше
                //чтение из xml в dataset может занимать по нескольку секунд для больших файлов
                //основную часть времени отнимал вывод информации о файлах!!
                //00.051
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionary(DBDataSet.ToDictionary(), forced);
                if (AppData.CurrentProject.DontLoadDuplicates)
                {
                    FunctionsLoadTranslationDB.THLoadDBCompareFromDictionaryParallellTables(dbDataSet.ToDictionary(), forceOverwriteTranslations);
                }
                else
                {
                    FunctionsLoadTranslationDB.THLoadDBCompareFromDictionaryParallellTables(dbDataSet.ToDictionary2(), forceOverwriteTranslations);
                }

                //многопоточный вариант предыдущего, но т.к. datatable is threadunsafe то возникают разные ошибки и повреждение внутреннего индекса таблицы, хоть это и быстрее, но после добавления lock разницы не видно
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionaryParallel(DBDataSet.DBDataSetToDBDictionary());


                //это медленнее первого варианта 
                //00.151
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionary2(DBDataSet.DBDataSetToDBDictionary());

                //считывание через словарь Чтение xml в словарь на текущий момент имеет проблемы
                //с невозможностью чтения закодированых в hex символов(решил как костыль через try catch) и пока не может читать сжатые xml
                //нужно постепенно доработать код, исправить проблемы и перейти полностью на этот наибыстрейший вариант
                //т.к. с ним и xml бд будет меньше размером
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionary(FunctionsDBFile.ReadXMLDBToDictionary(sPath));

            }
            catch (Exception ex)
            {
                Logger.Warn(ex, T._("Error") + " " + T._("Loading DB File") + ": " + sPath);
            }
        }

        internal async static Task SaveDB()
        {
            var fileName = FunctionsDBFile.GetDBFileName();
            var fileExtension = FunctionsDBFile.GetDBCompressionExt();
            var path = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), fileName + fileExtension);

            if (System.IO.File.Exists(path))
            {
               FunctionsBackup.ShiftToBackups(path);
            }

            var pathNextToSource = Path.Combine(AppData.CurrentProject.SelectedDir, Data.THSettings.TranslationFileSourceDirSuffix + fileExtension);

            await AppData.CurrentProject.PreSaveDB().ConfigureAwait(false);
            await FunctionsDBFile.WriteDBFileLite(AppData.CurrentProject.FilesContent, new[] { path, pathNextToSource }).ConfigureAwait(false);

            Logger.Info(T._("DB saved!"));

            FunctionsSounds.SaveDBComplete();
        }
    }
}

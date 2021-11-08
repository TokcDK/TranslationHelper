using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Projects;

namespace TranslationHelper.Data
{
    public static class ProjectData
    {
        /// <summary>
        /// init values and set references
        /// </summary>
        /// <param name="hfrmMain"></param>
        public static void Init(FormMain hfrmMain)
        {
            Main = hfrmMain;

            FilesListControl = new FilesListControlListBox(); // set using files list control

            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();
            THFilesElementsALLDataTable = new DataSet();
            OriginalsTableRowCoordinates = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>>();
            //THFilesElementsDictionary = new Dictionary<string, string>();
            //THFilesElementsDictionaryInfo = new Dictionary<string, string>();

            SelectedFilePath = string.Empty;
            FilePath = string.Empty;

            ProjectsList = ProjectBase.GetListOfProjectTypes();
        }

        /// <summary>
        /// Application's loaded config ini
        /// </summary>
        internal static INIFileMan.INIFile ConfigIni { get => Main.Settings.THConfigINI; set => Main.Settings.THConfigINI = value; }

        /// <summary>
        /// true - when file open, false - when file writing
        /// </summary>
        internal static bool OpenFileMode = true;
        /// <summary>
        /// true - when file write, false - when file open
        /// </summary>
        internal static bool SaveFileMode { get => !OpenFileMode; set => OpenFileMode = !value; }

        /// <summary>
        /// regex rules which appling to original to show what need to translate
        /// </summary>
        internal static Dictionary<string, string> TranslationRegexRules = new Dictionary<string, string>();
        internal static Dictionary<string, string> TranslationRegexRulesGroup = new Dictionary<string, string>();

        /// <summary>
        /// translation cell fix regex rules. same as search and replace with regex using
        /// </summary>
        internal static Dictionary<string, string> CellFixesRegexRules = new Dictionary<string, string>();

        /// <summary>
        /// reference to the main form
        /// </summary>
        internal static FormMain Main;

        /// <summary>
        /// CurrentProject
        /// </summary>
        internal static ProjectBase CurrentProject;

        /// <summary>
        /// List of project types
        /// </summary>
        internal static List<Type> ProjectsList;

        /// <summary>
        /// usually 'S'elected file 'Path' in file browse dialog
        /// </summary>
        internal static string SelectedFilePath { get; set; }

        /// <summary>
        /// Online Translation Cache
        /// </summary>
        internal static FunctionsOnlineCache OnlineTranslationCache;

        static string filepath;
        //current processing file for open/save
        internal static string FilePath
        {
            get { return string.IsNullOrWhiteSpace(filepath) ? SelectedFilePath : filepath; }
            set { filepath = value; }
        }

        /// <summary>
        /// main work table data
        /// </summary>
        internal static DataSet THFilesElementsDataset { get; set; }

        /// <summary>
        /// main work table infos
        /// </summary>
        internal static DataSet THFilesElementsDatasetInfo { get; set; }

        /// <summary>
        /// main work table data for all (wip)
        /// </summary>
        internal static DataSet THFilesElementsALLDataTable { get; set; }

        /// <summary>
        /// main table/row index coordinates data for same translation for identical and for write functions.
        /// Format:
        ///     original value:
        ///         list of table names:
        ///             list of row numbers:
        /// </summary>
        internal static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> OriginalsTableRowCoordinates { get; set; }

        /// <summary>
        /// Path for selected game dir which is translating
        /// </summary>
        internal static string SelectedGameDir { get => CurrentProject.SelectedGameDir; set => CurrentProject.SelectedGameDir = value; }
        /// <summary>
        /// Path for selected dir where to translate.
        /// In most causes it is game's dir.
        /// </summary>
        internal static string SelectedDir { get => CurrentProject.SelectedDir; set => CurrentProject.SelectedDir = value; }
        /// <summary>
        /// Path for selected dir where to translate.
        /// In most causes it is game's dir.
        /// </summary>
        internal static string OpenedFilesDir { get => CurrentProject.OpenedFilesDir; set => CurrentProject.OpenedFilesDir = value; }
        /// <summary>
        /// Path to dir where project's files are storing
        /// </summary>
        internal static string ProjectWorkDir { get => CurrentProject.ProjectWorkDir; set => CurrentProject.ProjectWorkDir = value; }

        /// <summary>
        /// target textbox control value
        /// </summary>
        internal static string TargetTextBoxPreValue;

        /// <summary>
        /// все баз данных в кучу здесь
        /// </summary>
        internal static Dictionary<string, string> AllDBmerged;// = new Dictionary<string, string>();

        /// <summary>
        /// Buffer temp value. String type.
        /// </summary>
        internal static string BufferValueString;

        /// <summary>
        /// true when settings is loading
        /// </summary>
        internal static bool SettingsIsLoading;

        /// <summary>
        /// The program session online translation cookies
        /// </summary>
        internal static System.Net.CookieContainer OnlineTranslatorCookies;

        internal static Dictionary<char, int> ENQuotesToJPLearnDataFoundPrev;
        internal static Dictionary<char, int> ENQuotesToJPLearnDataFoundNext;

        /// <summary>
        /// [for json open\save improve] skipped rpg maker mv json event codes
        /// </summary>
        internal static Dictionary<int, int> RpgMVSkippedCodesStat = new Dictionary<int, int>();

        /// <summary>
        /// [for json open\save improve] added rpg maker mv json event codes
        /// </summary>
        internal static Dictionary<int, int> RpgMVAddedCodesStat = new Dictionary<int, int>();

        /// <summary>
        /// Application log
        /// </summary>
        internal static FunctionsLogs AppLog = new FunctionsLogs();

        ///// <summary>
        ///// Fileslist control object
        ///// </summary>
        //internal static object FilesList;

        /// <summary>
        /// Files list using now control
        /// </summary>
        internal static FilesListControlBase FilesListControl;

        /// <summary>
        /// Files list
        /// </summary>
        internal static ListBox THFilesList { get => Main.THFilesList; }

        /// <summary>
        /// Index of Original column
        /// </summary>
        internal static int OriginalColumnIndex = 0;
        /// <summary>
        /// Index of main Translation column
        /// </summary>
        internal static int TranslationColumnIndex = 1;

        static readonly object TableDataAddLocker = new object();
        /// <summary>
        /// add new <paramref name="tableData"/> in tables list
        /// </summary>
        /// <param name="tableData"></param>
        internal static void AddTableData(DataTable tableData)
        {
            lock (TableDataAddLocker)
            {
                if (!THFilesElementsDataset.Tables.Contains(tableData.TableName))
                {
                    THFilesElementsDataset.Tables.Add(tableData);
                }
                else
                {

                }
            }
        }

        static readonly object TableInfoAddLocker = new object();
        /// <summary>
        /// add new <paramref name="tableInfo"/> in tables list
        /// </summary>
        /// <param name="tableInfo"></param>
        internal static void AddTableInfo(DataTable tableInfo)
        {
            lock (TableInfoAddLocker)
            {
                if (!THFilesElementsDatasetInfo.Tables.Contains(tableInfo.TableName))
                {
                    THFilesElementsDatasetInfo.Tables.Add(tableInfo);
                }
                else
                {

                }
            }
        }
    }
}

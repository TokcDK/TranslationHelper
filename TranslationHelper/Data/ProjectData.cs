using System;
using System.Collections.Generic;
using System.Data;
using TranslationHelper.Functions;
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

            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();
            THFilesElementsALLDataTable = new DataSet();
            OriginalsTableRowCoordinats = new Dictionary<string, Dictionary<string, List<int>>>();
            //THFilesElementsDictionary = new Dictionary<string, string>();
            //THFilesElementsDictionaryInfo = new Dictionary<string, string>();

            SelectedFilePath = string.Empty;
            FilePath = string.Empty;

            ProjectsList = ProjectBase.GetListOfProjectTypes();
        }

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
        /// main table/row index coordinats data for same translation for identical and for write functions
        /// </summary>
        internal static Dictionary<string, Dictionary<string, List<int>>> OriginalsTableRowCoordinats { get; set; }

        public static string SelectedGameDir;
        public static string SelectedDir;
        public static string ProjectWorkDir;

        /// <summary>
        /// target textbox control value
        /// </summary>
        internal static string TargetTextBoxPreValue;

        /// <summary>
        /// filtering records duplicates while adding to main work data table
        /// </summary>
        internal static HashSet<string> hashes = new HashSet<string>();

        /// <summary>
        /// для целей записи, скидывание сюда данных таблицы
        /// </summary>
        internal static Dictionary<string, string> TablesLinesDict = new Dictionary<string, string>();

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
    }
}

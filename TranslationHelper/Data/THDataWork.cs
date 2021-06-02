using System.Collections.Generic;
using System.Data;
using TranslationHelper.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Data
{
    internal class THDataWork
    {
        internal THDataWork()
        {
            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();
            THFilesElementsALLDataTable = new DataSet();
            OriginalsTableRowCoordinats = new Dictionary<string, Dictionary<string, List<int>>>();
            //THFilesElementsDictionary = new Dictionary<string, string>();
            //THFilesElementsDictionaryInfo = new Dictionary<string, string>();

            SPath = string.Empty;
            FilePath = string.Empty;

            ProjectsList = ProjectBase.GetListOfProjects(this);
        }

        /// <summary>
        /// true - when file open, false - when file writing
        /// </summary>
        internal bool OpenFileMode = true;
        /// <summary>
        /// true - when file write, false - when file open
        /// </summary>
        internal bool SaveFileMode { get => !OpenFileMode; set => OpenFileMode = !value; }

        /// <summary>
        /// regex rules which appling to original to show what need to translate
        /// </summary>
        internal Dictionary<string, string> TranslationRegexRules = new Dictionary<string, string>();
        internal Dictionary<string, string> TranslationRegexRulesGroup = new Dictionary<string, string>();

        /// <summary>
        /// translation cell fix regex rules. same as search and replace with regex using
        /// </summary>
        internal Dictionary<string, string> CellFixesRegexRules = new Dictionary<string, string>();

        /// <summary>
        /// reference to the main form
        /// </summary>
        internal THfrmMain Main;

        /// <summary>
        /// CurrentProject
        /// </summary>
        internal ProjectBase CurrentProject;

        /// <summary>
        /// internal ProjectBase Project
        /// </summary>
        internal List<ProjectBase> ProjectsList;

        /// <summary>
        /// usually 'S'elected file 'Path' in file browse dialog
        /// </summary>
        internal string SPath { get; set; }

        /// <summary>
        /// Online Translation Cache
        /// </summary>
        internal FunctionsOnlineCache OnlineTranslationCache;

        string filepath;
        //current processing file for open/save
        internal string FilePath
        {
            get { return string.IsNullOrWhiteSpace(filepath) ? SPath : filepath; }
            set { filepath = value; }
        }

        /// <summary>
        /// main work table data
        /// </summary>
        internal DataSet THFilesElementsDataset { get; set; }

        /// <summary>
        /// main work table infos
        /// </summary>
        internal DataSet THFilesElementsDatasetInfo { get; set; }

        /// <summary>
        /// main work table data for all (wip)
        /// </summary>
        internal DataSet THFilesElementsALLDataTable { get; set; }

        /// <summary>
        /// main table/row index coordinats data for same translation for identical and for write functions
        /// </summary>
        internal Dictionary<string, Dictionary<string, List<int>>> OriginalsTableRowCoordinats { get; set; }

        /// <summary>
        /// target textbox control value
        /// </summary>
        internal string TargetTextBoxPreValue;

        /// <summary>
        /// filtering records duplicates while adding to main work data table
        /// </summary>
        internal HashSet<string> hashes = new HashSet<string>();

        /// <summary>
        /// для целей записи, скидывание сюда данных таблицы
        /// </summary>
        internal Dictionary<string, string> TablesLinesDict = new Dictionary<string, string>();

        /// <summary>
        /// все баз данных в кучу здесь
        /// </summary>
        internal Dictionary<string, string> AllDBmerged;// = new Dictionary<string, string>();

        /// <summary>
        /// Buffer temp value. String type.
        /// </summary>
        internal string BufferValueString;

        /// <summary>
        /// true when settings is loading
        /// </summary>
        internal bool SettingsIsLoading;

        /// <summary>
        /// The program session online translation cookies
        /// </summary>
        internal System.Net.CookieContainer OnlineTranslatorCookies;

        internal Dictionary<char, int> ENQuotesToJPLearnDataFoundPrev;
        internal Dictionary<char, int> ENQuotesToJPLearnDataFoundNext;
    }
}

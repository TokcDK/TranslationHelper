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
            THFilesElementsDictionary = new Dictionary<string, string>();
            THFilesElementsDictionaryInfo = new Dictionary<string, string>();

            SPath = string.Empty;
            FilePath = string.Empty;

            ProjectsList = ProjectBase.GetListOfProjects(this);
        }

        internal Dictionary<string, string> TranslationRegexRules = new Dictionary<string, string>();
        internal Dictionary<string, string> TranslationRegexRulesGroup = new Dictionary<string, string>();
        internal Dictionary<string, string> CellFixesRegexRules = new Dictionary<string, string>();

        //Link to main form
        internal THMain Main;


        //CurrentProject
        internal ProjectBase CurrentProject;
        //internal ProjectBase Project;
        internal List<ProjectBase> ProjectsList;

        //usually 'S'elected file 'Path' in file browse dialog
        internal string SPath { get; set; }

        //Online Translation Cache
        internal FunctionsOnlineCache OnlineTranslationCache;

        //current processing file for open/save
        internal string FilePath { get; set; }

        internal DataSet THFilesElementsDataset { get; set; }

        internal DataSet THFilesElementsDatasetInfo { get; set; }

        internal DataSet THFilesElementsALLDataTable { get; set; }

        internal Dictionary<string, string> THFilesElementsDictionary { get; set; }
        internal Dictionary<string, string> THFilesElementsDictionaryInfo { get; set; }

        //target textbox control value
        internal string TargetTextBoxPreValue;

        //для целей удаления дубликатов записей, добавляемых в таблицу
        internal HashSet<string> hashes = new HashSet<string>();

        /// <summary>
        /// для целей записи, скидывание сюда данных таблицы
        /// </summary>
        internal Dictionary<string, string> TablesLinesDict = new Dictionary<string, string>();

        /// <summary>
        /// все баз данных в кучу здесь
        /// </summary>
        internal Dictionary<string, string> AllDBmerged;// = new Dictionary<string, string>();
    }
}

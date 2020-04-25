﻿using System.Collections.Generic;
using System.Data;
using TranslationHelper.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Data
{
    public class THDataWork
    {
        public THDataWork()
        {
            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();
            THFilesElementsALLDataTable = new DataSet();

            SPath = string.Empty;
            FilePath = string.Empty;

            ProjectsList = ProjectBase.GetListOfProjects(this);

            TranslationRegexRules = new Dictionary<string, string>();
            CellFixesRegexRules = new Dictionary<string, string>();
        }

        internal Dictionary<string, string> TranslationRegexRules;
        internal Dictionary<string, string> CellFixesRegexRules;

        //Link to main form
        internal THMain Main;

        internal ProjectBase Project;
        internal List<ProjectBase> ProjectsList;

        //usually 'S'elected file 'Path' in file browse dialog
        public string SPath { get; set; }

        //Online Translation Cache
        internal FunctionsOnlineCache OnlineTranslationCache;

        //current processing file for open/save
        public string FilePath { get; set; }

        public DataSet THFilesElementsDataset { get; set; }

        public DataSet THFilesElementsDatasetInfo { get; set; }

        public DataSet THFilesElementsALLDataTable { get; set; }

        //target textbox control value
        internal string TargetTextBoxPreValue;

        //CurrentProject
        internal ProjectBase CurrentProject;
    }
}

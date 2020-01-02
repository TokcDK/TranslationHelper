using System.Collections.Generic;
using System.Data;
using TranslationHelper.Projects;
using TranslationHelper.Projects.KiriKiri;

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

            ProjectsList = new List<ProjectBase>()
            {
                new RPGMTransPatch(this),
                new RPGMGame(this),
                new RPGMMVGame(this),
                new KiriKiriGame(this),
                new Raijin7Game(this)
            };
        }

        internal ProjectBase Project;
        internal List<ProjectBase> ProjectsList;

        //usually selected file path in file browse dialog
        public string SPath { get; set; }

        //current processing file
        public string FilePath { get; set; }

        public DataSet THFilesElementsDataset { get; set; }

        public DataSet THFilesElementsDatasetInfo { get; set; }

        public DataSet THFilesElementsALLDataTable { get; set; }
    }
}

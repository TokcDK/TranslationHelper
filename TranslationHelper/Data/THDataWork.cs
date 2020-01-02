using System.Collections.Generic;
using System.Data;
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

            ProjectsList = new List<ProjectBase>()
            {
                new RPGMTransPatch(this),
                new RPGMGame(this),
                new RPGMMVGame(this),
                new Raijin7Game(this)
            };
        }

        internal ProjectBase Project;
        internal List<ProjectBase> ProjectsList;

        public string SPath { get; set; }

        public string FilePath { get => FilePath.Length > 0 ? FilePath : SPath; set => FilePath = value; }

        public DataSet THFilesElementsDataset { get; set; }

        public DataSet THFilesElementsDatasetInfo { get; set; }

        public DataSet THFilesElementsALLDataTable { get; set; }
    }
}

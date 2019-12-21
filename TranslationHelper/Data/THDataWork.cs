using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Data
{
    public class THDataWork
    {
        public THDataWork()
        {
            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();
            THFilesElementsALLDataTable = new DataSet();
        }

        public string SPath { get; set; }

        public string TempPath { get; set; }

        public DataSet THFilesElementsDataset { get; set; }

        public DataSet THFilesElementsDatasetInfo { get; set; }

        public DataSet THFilesElementsALLDataTable { get; set; }
    }
}

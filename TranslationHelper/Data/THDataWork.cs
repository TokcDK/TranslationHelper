using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Data
{
    internal class THDataWork
    {
        public THDataWork()
        {
            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();
        }

        internal string OpenPath { get; set; }

        internal DataSet THFilesElementsDataset { get; set; }

        internal DataSet THFilesElementsDatasetInfo { get; set; }
    }
}

using System;
using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.OnlineTranslation
{
    class TranslationBigBlockMulti : TranslationBase
    {
        public TranslationBigBlockMulti(ProjectData projectData) : base(projectData)
        {
        }

        internal override void Get()
        {
            throw new NotImplementedException();
        }

        internal void TranslateByBlock()
        {
            foreach (DataTable table in projectData.THFilesElementsDataset.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row[1] + string.Empty))
                    {
                        foreach (var line in (row[0] + string.Empty).SplitToLines())
                        {

                        }
                    }
                }
            }
        }
    }
}

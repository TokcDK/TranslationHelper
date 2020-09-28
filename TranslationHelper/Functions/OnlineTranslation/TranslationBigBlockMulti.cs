﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.OnlineTranslation
{
    class TranslationBigBlockMulti : TranslationBase
    {
        public TranslationBigBlockMulti(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override void Get()
        {
            throw new NotImplementedException();
        }

        internal void TranslateByBlock()
        {
            foreach(DataTable table in thDataWork.THFilesElementsDataset.Tables)
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

﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class LoadRowDataToCustomDb : RowBase
    {
        public LoadRowDataToCustomDb()
        {
        }

        Dictionary<string, string> _dict = new Dictionary<string, string>();
        string _custom = ThSettings.CustomDbPath();
        protected override void ActionsPreRowsApply()
        {
            //load DB if need
            if (File.Exists(_custom))
            {
                using (var dbDataSet = new System.Data.DataSet())
                {
                    ProjectData.Main.ProgressInfo(true, "Load custom DB");

                    var loadcustom = new Task(() => FunctionsDbFile.ReadDbFile(dbDataSet, _custom));
                    loadcustom.ConfigureAwait(true);
                    loadcustom.Start();
                    loadcustom.Wait();

                    _dict = dbDataSet.ToDictionary();

                    ProjectData.Main.ProgressInfo(false);
                }
            }
        }

        protected override void ActionsPostRowsApply()
        {
            //save DB
            ProjectData.Main.ProgressInfo(true, "Save custom DB");

            var loadcustom = new Task(() => FunctionsDbFile.WriteDbFile(_dict.ToDataSet(), _custom));
            loadcustom.ConfigureAwait(true);
            loadcustom.Start();
            loadcustom.Wait();

            ProjectData.Main.ProgressInfo(false);
        }

        protected override bool Apply()
        {
            var orig = SelectedRow[0] + "";
            var trans = SelectedRow[1] + "";

            //add orig or replace exist
            if (_dict.ContainsKey(orig))
            {
                _dict[orig] = trans;
            }
            else
            {
                _dict.Add(orig, trans);
            }

            var trimmedorig = orig.TrimAllExceptLettersOrDigits();

            //when trimmed not equal orig add also trimmed
            if (trimmedorig != orig)
                if (_dict.ContainsKey(trimmedorig))
                {
                    _dict[trimmedorig] = trans.TrimAllExceptLettersOrDigits();
                }
                else
                {
                    _dict.Add(trimmedorig, trans.TrimAllExceptLettersOrDigits());
                }

            return true;
        }
    }
}

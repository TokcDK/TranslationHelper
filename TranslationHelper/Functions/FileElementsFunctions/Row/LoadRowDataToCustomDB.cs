using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class LoadRowDataToCustomDB : FileElementsRowFunctionsBase
    {
        public LoadRowDataToCustomDB(THDataWork thDataWork) : base(thDataWork)
        {
        }

        Dictionary<string, string> dict = new Dictionary<string, string>();
        string custom = THSettingsData.CustomDBPath();
        protected override void ActionsPreRowsApply()
        {
            //load DB if need
            if (File.Exists(custom))
            {
                using (var DBDataSet = new System.Data.DataSet())
                {
                    thDataWork.Main.ProgressInfo(true, "Load custom DB");

                    var loadcustom = new Task(() => FunctionsDBFile.ReadDBFile(DBDataSet, custom));
                    loadcustom.ConfigureAwait(true);
                    loadcustom.Start();
                    loadcustom.Wait();

                    dict = DBDataSet.ToDictionary();

                    thDataWork.Main.ProgressInfo(false);
                }
            }
        }

        protected override void ActionsPostRowsApply()
        {
            //save DB
            thDataWork.Main.ProgressInfo(true, "Save custom DB");

            var loadcustom = new Task(() => FunctionsDBFile.WriteDBFile(dict.ToDataSet(), custom));
            loadcustom.ConfigureAwait(true);
            loadcustom.Start();
            loadcustom.Wait();

            thDataWork.Main.ProgressInfo(false);
        }

        protected override bool Apply()
        {
            var orig = SelectedRow[0] + "";
            var trans = SelectedRow[1] + "";

            if (dict.ContainsKey(orig))
            {
                dict[orig] = trans;
            }
            else
            {
                dict.Add(orig, trans);
            }

            return true;
        }
    }
}

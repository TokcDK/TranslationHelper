using System.Collections.Generic;
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
        string _custom = THSettings.CustomDBPath;
        protected override void ActionsInit()
        {
            //load DB if need
            if (File.Exists(_custom))
            {
                using (var dbDataSet = new System.Data.DataSet())
                {
                    Logger.Info("Load custom DB");

                    var loadcustom = new Task(() => FunctionsDBFile.ReadDBFile(dbDataSet, _custom));
                    loadcustom.ConfigureAwait(true);
                    loadcustom.Start();
                    loadcustom.Wait();

                    _dict = dbDataSet.ToDictionary();

                    
                }
            }
        }

        protected override void ActionsFinalize()
        {
            //save DB
            Logger.Info("Save custom DB");

            var loadcustom = new Task(() => FunctionsDBFile.WriteDBFile(_dict.ToDataSet(), _custom));
            loadcustom.ConfigureAwait(true);
            loadcustom.Start();
            loadcustom.Wait();

            
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            var orig = rowData.Original;
            var trans = rowData.Translation;

            //add orig or replace exist
            if (_dict.ContainsKey(orig))
            {
                _dict[orig] = trans;
            }
            else
            {
                _dict.Add(orig, trans);
            }

            var trimmedorig = orig.TrimAllExceptLettersAndDigits();

            //when trimmed not equal orig add also trimmed
            if (trimmedorig != orig)
                if (_dict.ContainsKey(trimmedorig))
                {
                    _dict[trimmedorig] = trimmedorig;
                }
                else
                {
                    _dict.Add(trimmedorig, trimmedorig);
                }

            return true;
        }
    }
}

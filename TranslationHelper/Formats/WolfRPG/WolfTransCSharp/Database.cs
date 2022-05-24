using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTNet = WolfTrans.Net;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class Database:FormatBinaryBase
    {
        protected override void FileOpen()
        {
            var db_name = Path.GetFileNameWithoutExtension(FilePath);
            var db = new WTNet.Parsers.Database.Database();
            db.Read(FilePath);
            int type_index = 0;
            foreach (var type in db.Types)
            {
                if (string.IsNullOrEmpty(type.Name)) continue;

                var patch_filename = $"dump/db/{db_name}/{type.Name}.txt";

                int data_index = 0;
                foreach (var data in type.Data)
                {
                    AddRowData(data.Name);
                    foreach (var t in data.Fields.Where(f => f.IsString && f.Type == 0))
                    {
                        foreach(var s in t.String_args) AddRowData(s);
                    }
                    data_index++;
                }

                type_index++;
            }
        }
    }
}

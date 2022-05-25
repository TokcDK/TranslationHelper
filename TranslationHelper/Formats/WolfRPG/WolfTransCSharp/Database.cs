using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTNet = WolfTrans.Net;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Shared;
using WolfTrans.Net.Parsers.Database;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class Database:FormatBinaryBase
    {
        WTNet.Parsers.Database.Database DB=null;

        protected override void FileOpen()
        {
            if (!File.Exists(FilePath)) return;

            var db_name = Path.GetFileNameWithoutExtension(FilePath);
            DB = new WTNet.Parsers.Database.Database();
            DB.Read(FilePath);

            int type_index = 0;
            foreach (var type in DB.Types)
            {
                if (string.IsNullOrEmpty(type.Name)) continue;

                var patch_filename = $"dump/db/{db_name}/{type.Name}.txt";

                int data_index = 0;
                foreach (var data in type.Data)
                {
                    var dataName = data.Name;

                    // if (CommandUtils.IsTranslatable(data.Name)) strings.Add(data.Name);

                    foreach ((string s, DBField f) in data.GetTranslatable())
                    {
                        var value = s;
                        if(AddRowData(ref value, $"DB name: {db_name}\r\nType name: {type.Name} \r\nField index:{f.Index}") && ProjectData.SaveFileMode)
                        {
                            data.String_values[f.Index] = value;
                        }
                    }

                    data_index++;
                }

                type_index++;
            }
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                DB.Write();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

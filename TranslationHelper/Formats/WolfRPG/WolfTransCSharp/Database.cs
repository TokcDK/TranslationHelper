using System.IO;
using TranslationHelper.Data;
using WolfTrans.Net.Parsers.Database;
using WTNet = WolfTrans.Net;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class Database : FormatBinaryBase
    {
        WTNet.Parsers.Database.Database Data = null;

        protected override void FileOpen()
        {
            if (!File.Exists(FilePath)) return;

            var db_name = Path.GetFileNameWithoutExtension(FilePath);
            Data = new WTNet.Parsers.Database.Database();
            Data.Read(FilePath);

            int dataTypesCount = Data.Types.Count;
            for (int t = 0; t < dataTypesCount; t++)
            {
                var type = Data.Types[t];

                if (string.IsNullOrEmpty(type.Name)) continue;

                var patch_filename = $"dump/db/{db_name}/{type.Name}.txt";

                int dataTypeDataCount = type.Data.Count;
                for (int d=0;d< dataTypeDataCount;d++)
                {
                    var data = type.Data[d];

                    foreach ((string s, DBField f) in data.GetTranslatable())
                    {
                        var value = s;
                        if (AddRowData(ref value, $"DB name: {db_name}\r\nType name: {type.Name} \r\nField index:{f.Index}") && ProjectData.SaveFileMode)
                        {
                            data.String_values[f.Index] = value;
                        }
                    }
                }
            }
        }

        protected override bool WriteFileData(string filePath = "")
        {
            if (!ParseData.Ret) return false;

            try { Data.Write(); } catch { return false; }
            return true;
        }
    }
}

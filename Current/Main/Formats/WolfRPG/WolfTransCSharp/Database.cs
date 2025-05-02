using System.IO;
using TranslationHelper.Data;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Database;
using WTNet = WolfTrans.Net;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class Database : WolftransCSharpBase
    {
        public override string Extension => "";

        protected override void FileOpen()
        {
            if (!File.Exists(FilePath)) return;

            var db_name = Path.GetFileNameWithoutExtension(FilePath);
            WolfParserBase = new WTNet.Parsers.Database.ParserDatabase();
            WolfParserBase.Read(FilePath);

            var types = ((WTNet.Parsers.Database.ParserDatabase)WolfParserBase).Types;
            int dataTypesCount = types.Count;
            for (int t = 0; t < dataTypesCount; t++)
            {
                var type = types[t];

                if (string.IsNullOrEmpty(type.Name)) continue;

                //var patch_filename = $"dump/db/{db_name}/{type.Name}.txt";

                int dataTypeDataCount = type.Data.Count;
                for (int d = 0; d < dataTypeDataCount; d++)
                {
                    var data = type.Data[d];

                    foreach ((string s, DBField f) in data.GetTranslatable(filterValue: false))
                    {
                        var value = s;
                        if (AddRowData(ref value, $"DB name: {db_name}\r\nType name: {type.Name} \r\nField index: {f.Index}") && SaveFileMode)
                        {
                            data.String_values[f.Index] = value;
                        }
                    }
                }
            }
        }
    }
}

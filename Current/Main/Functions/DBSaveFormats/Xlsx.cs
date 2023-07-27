using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MiniExcelLibs;
using static RubyMarshal.RubyMarshal;

namespace TranslationHelper.Functions.DBSaveFormats
{
    internal class Xlsx : IDataBaseFileFormat
    {
        public string Ext => "xlsx";

        public string Description => $"({Ext}) Excel xlsx format";

        public void Read(string fileName, object data)
        {
            if (!(data is DataSet dataSet))
            {
                throw new InvalidDataException($"{nameof(data)} is not dataset!");
            }

            var sheetNames = MiniExcel.GetSheetNames(fileName);
            foreach (var sheetName in sheetNames)
            {
                var table = dataSet.Tables.Add(sheetName);

                bool isColumnsAdded = false;
                foreach (var row in MiniExcel.Query(fileName, sheetName: sheetName).Cast<IDictionary<string, object>>())
                {
                    if (!isColumnsAdded)
                    {
                        isColumnsAdded = true;

                        foreach(var o in row)
                        {
                            table.Columns.Add((string)o.Value);
                        }
                    }
                    else
                    {
                        table.Rows.Add(row.Values.ToArray());
                    }
                }
            }
        }

        public void Write(string fileName, object data)
        {
            MiniExcel.SaveAs(fileName, data);
        }
    }
}

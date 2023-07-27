using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MiniExcelLibs;

namespace TranslationHelper.Functions.DBSaveFormats
{
    internal class Xlsx : IDataBaseFileFormat
    {
        public string Ext => "xlsx";

        public string Description => $"({Ext}) Excel xlsx format";

        public void Read(string fileName, object data)
        {
            var sheetNames = MiniExcel.GetSheetNames(fileName);
            foreach (var sheetName in sheetNames)
            {
                foreach(var row in MiniExcel.Query(fileName, sheetName: sheetName))
                {
                }
            }
        }

        public void Write(string fileName, object data)
        {
            MiniExcel.SaveAs(fileName, data);
        }
    }
}

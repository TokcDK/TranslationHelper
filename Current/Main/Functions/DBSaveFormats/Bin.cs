using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Functions.DBSaveFormats
{
    internal class Bin : IDataBaseFileFormat
    {
        public string Ext => "bin";

        public string Description => $"({Ext}) Binary format";

        public void Read(string fileName, object data)
        {
            if (!(data is DataSet dataSet))
            {
                throw new InvalidDataException($"{nameof(data)} is not dataset!");
            }

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var ds = (DataSet)formatter.Deserialize(stream);
                dataSet.Clear();
                dataSet.Merge(ds);
            }
        }

        public void Write(string fileName, object data)
        {
            if (!(data is DataSet dataSet))
            {
                throw new InvalidDataException($"{nameof(data)} is not dataset!");
            }

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(stream, dataSet);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class XML : IDataBaseFileFormat
    {
        public virtual string Ext => "xml";

        public virtual string Description => "Uncompressed xml";

        public void Read(string fileName, object data)
        {
            ReadWrite(fileName, data, isRead: true);
        }

        public void Write(string fileName, object data)
        {
            ReadWrite(fileName, data, isRead: false);
        }

        private readonly ReaderWriterLockSlim _writeXmlLocker = new ReaderWriterLockSlim();
        void ReadWrite(string fileName, object data, bool isRead = true)
        {
            if (!(data is DataSet dataSet))
            {
                throw new InvalidDataException($"{nameof(data)} is not dataset!");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using (var fs = new FileStream(fileName, isRead ? FileMode.Open : FileMode.Create))
            {
                Stream s;
                //string fileExtension = Path.GetExtension(fileName);
                s = FileStreamMod(fs, isRead);

                if (isRead)
                {
                    try
                    {
                        dataSet.ReadXml(s);
                    }
                    catch (InvalidDataException) { }
                    catch (IOException) { }
                }
                else
                {
                    _writeXmlLocker.EnterWriteLock();
                    try
                    {
                        dataSet.WriteXml(s);
                    }
                    finally
                    {
                        _writeXmlLocker.ExitWriteLock();
                    }
                }

                s.Close();
            }
        }

        /// <summary>
        /// FileStream modification.
        /// May be need for formats with compression
        /// </summary>
        /// <param name="dbInputFileStream"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        protected virtual Stream FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return dbInputFileStream;
        }
    }
}

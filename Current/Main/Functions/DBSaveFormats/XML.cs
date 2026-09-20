using NLog;
using System;
using System.Data;
using System.IO;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class XML : IDataBaseFileFormat
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        void ReadWrite(string fileName, object data, bool isRead = true)
        {
            if (!(data is DataSet dataSet))
            {
                throw new InvalidDataException($"{nameof(data)} is not dataset!");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using (var fs = new FileStream(fileName, isRead ? FileMode.Open : FileMode.Create))
            //The compression wrapper has to be disposed, not merely closed, so that its trailer is
            //written. The previous code skipped the Close when ReadXml/WriteXml threw, which left a
            //truncated or unfinished file behind.
            //
            //The former ReaderWriterLockSlim here guarded nothing: it was an instance field while a new
            //XML is created per call. Writes are serialised by FunctionsDBFile's static lock instead.
            using (Stream s = FileStreamMod(fs, isRead))
            {
                if (isRead)
                {
                    try
                    {
                        dataSet.ReadXml(s);
                    }
                    catch (InvalidDataException ex)
                    {
                        //A broken file yields an empty DataSet; that is the intent, but it must be visible.
                        Logger.Warn(ex, "Failed to read {0}", fileName);
                    }
                    catch (IOException ex)
                    {
                        Logger.Warn(ex, "Failed to read {0}", fileName);
                    }
                }
                else
                {
                    dataSet.WriteXml(s);
                }
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

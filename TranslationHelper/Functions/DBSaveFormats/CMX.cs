using System;
using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class CMX : IDBSave
    {
        string IDBSave.Ext => "cmx";

        string IDBSave.Description => "GZip compressed xml";

        Stream IDBSave.FileStreamMod(FileStream DBInputFileStream, bool IsRead)
        {
            return new GZipStream(DBInputFileStream, IsRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

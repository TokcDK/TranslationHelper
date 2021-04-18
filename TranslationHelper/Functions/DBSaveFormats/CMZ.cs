using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class CMZ : IDBSave
    {
        string IDBSave.Ext => "cmz";

        string IDBSave.Description => "Deflate compressed xml";

        Stream IDBSave.FileStreamMod(FileStream DBInputFileStream, bool IsRead)
        {
            return new DeflateStream(DBInputFileStream, IsRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

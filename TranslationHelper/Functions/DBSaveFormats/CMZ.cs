using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class Cmz : IDbSave
    {
        string IDbSave.Ext => "cmz";

        string IDbSave.Description => "Deflate compressed xml";

        Stream IDbSave.FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return new DeflateStream(dbInputFileStream, isRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

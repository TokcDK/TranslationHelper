using System;
using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class Cmx : IDbSave
    {
        string IDbSave.Ext => "cmx";

        string IDbSave.Description => "GZip compressed xml";

        Stream IDbSave.FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return new GZipStream(dbInputFileStream, isRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

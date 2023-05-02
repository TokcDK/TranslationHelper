using System;
using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    /// <summary>
    /// XML compressed using GZip
    /// </summary>
    class CMX : XML, IDataBaseFileFormat
    {
        string IDataBaseFileFormat.Ext => "cmx";

        string IDataBaseFileFormat.Description => "GZip Compressed xml";

        protected override Stream FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return new GZipStream(dbInputFileStream, isRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

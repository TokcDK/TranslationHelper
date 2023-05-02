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
        public override string Ext => "cmx";

        public override string Description => $"({Ext}) GZip Compressed xml";

        protected override Stream FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return new GZipStream(dbInputFileStream, isRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

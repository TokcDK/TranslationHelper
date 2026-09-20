using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    /// <summary>
    /// XML compressed using Deflate
    /// </summary>
    class CMZ : XML
    {
        public override string Ext => "cmz";

        public override string Description => $"({Ext}) Deflate Compressed xml";

        protected override Stream FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return new DeflateStream(dbInputFileStream, isRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

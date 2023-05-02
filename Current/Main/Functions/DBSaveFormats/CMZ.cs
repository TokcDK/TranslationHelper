using System.IO;
using System.IO.Compression;

namespace TranslationHelper.Functions.DBSaveFormats
{
    /// <summary>
    /// XML compressed using Deflate
    /// </summary>
    class CMZ : XML, IDataBaseFileFormat
    {
        string IDataBaseFileFormat.Ext => "cmz";

        string IDataBaseFileFormat.Description => "Deflate Compressed xml";

        protected override Stream FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return new DeflateStream(dbInputFileStream, isRead ? CompressionMode.Decompress : CompressionMode.Compress);
        }
    }
}

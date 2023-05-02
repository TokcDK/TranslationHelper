using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.DBSaveFormats
{
    interface IDataBaseFileFormat
    {
        /// <summary>
        /// Save load extension
        /// </summary>
        string Ext { get; }

        /// <summary>
        /// Short description of save format
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Read file content data into <paramref name="data"/> from path <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        void Read(string fileName, object data);
        /// <summary>
        /// Write file content data from <paramref name="data"/> to path <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        void Write(string fileName, object data);
    }
}

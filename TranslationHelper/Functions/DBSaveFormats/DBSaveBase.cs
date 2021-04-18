using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.DBSaveFormats
{
    interface IDBSave
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
        /// FileStream modification.
        /// May be need for formats with compression
        /// </summary>
        /// <param name="DBInputFileStream"></param>
        /// <param name="IsRead"></param>
        /// <returns></returns>
        Stream FileStreamMod(FileStream DBInputFileStream, bool IsRead = true);
    }
}

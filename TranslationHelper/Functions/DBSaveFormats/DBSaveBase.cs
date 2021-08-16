using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.DBSaveFormats
{
    interface IDbSave
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
        /// <param name="dbInputFileStream"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        Stream FileStreamMod(FileStream dbInputFileStream, bool isRead = true);
    }
}

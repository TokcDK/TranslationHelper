using System.Collections.Generic;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsInterfaces
    {
        /// <summary>
        /// set all added db file formats
        /// </summary>
        /// <returns></returns>
        internal static List<IDataBaseFileFormat> GetDBSaveFormats()
        {
            return GetListOfSubClasses.Inherited.GetListOfInterfaceImplimentations<IDataBaseFileFormat>();
        }

        /// <summary>
        /// gets current selected format of database file
        /// </summary>
        /// <returns></returns>
        internal static IDataBaseFileFormat GetCurrentDBFormat(string ext = null)
        {
            return FunctionsDBFile.GetCurrentDBFormat(ext);
        }
    }
}

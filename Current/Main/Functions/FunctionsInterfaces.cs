using System.Collections.Generic;
using TranslationHelper.Functions.DBSaveFormats;
using TranslationHelper.INISettings;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsInterfaces
    {
        /// <summary>
        /// set all added db file formats
        /// </summary>
        /// <returns></returns>
        internal static List<IDBSave> GetDBSaveFormats()
        {
            return GetListOfSubClasses.Inherited.GetListOfInterfaceImplimentations<IDBSave>();
        }

        /// <summary>
        /// gets current selected format of database file
        /// </summary>
        /// <returns></returns>
        internal static IDBSave GetCurrentDBFormat()
        {
            return FunctionsDBFile.GetCurrentDBFormat();
        }
    }
}

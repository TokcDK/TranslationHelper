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
        internal static List<IDbSave> GetDbSaveFormats()
        {
            return GetListOfSubClasses.Inherited.GetListOfInterfaceImplimentations<IDbSave>();
        }

        /// <summary>
        /// gets current selected format of database file
        /// </summary>
        /// <returns></returns>
        internal static IDbSave GetCurrentDbFormat()
        {
            return FunctionsDbFile.GetCurrentDbFormat();
        }
    }
}

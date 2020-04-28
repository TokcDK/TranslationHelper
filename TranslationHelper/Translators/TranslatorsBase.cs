using System.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Translators
{
    public static class TranslatorsBase
    {
        public static string ReturnTranslatedOrCache(DataSet cacheDS, string InputLine)
        {
            string valuefromcache = FunctionsTable.TranslationCacheFind(cacheDS, InputLine);

            if (valuefromcache.Length != 0)
            {
                return valuefromcache;
            }
            else
            {
                return GoogleAPI.Translate(InputLine);
            }
        }
    }
}

using TranslationHelper.OnlineTranslators;

namespace TranslationHelper.Functions
{
    internal class FunctionsWeb
    {
        /// <summary>
        /// Current using useragent
        /// </summary>
        /// <returns></returns>
        internal static string GetUserAgent()
        {
            var ini = Data.AppData.ConfigIni;
            if (!ini.KeyExists("UserAgent", "Translation"))
            {
                ini.SetKey("Translation", "UserAgent", UserAgents.Chrome_Win10);
                return UserAgents.Chrome_Win10;
            }
            else
            {
                return ini.GetKey("Translation", "UserAgent");
            }
        }
    }
}
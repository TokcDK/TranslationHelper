namespace TranslationHelper.OnlineTranslators.XUA
{
    class Settings
    {
        public static string UserAgent;
        public static string Language = "English";
        public static string FromLanguage = "Japanese";
        internal static float Timeout = 60;
        public static bool EnableDumping;

        public static string RedirectedResourcesPath { get; internal set; }
    }
}

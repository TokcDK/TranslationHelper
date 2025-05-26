using l10n;

namespace THCore
{
    public static class SharedData
    {
        internal static string LanguageDirName { get => "l10n"; }

        public static string StartupPath { get; set; }
        public static string MenuModulesDirPath { get; set; }
        public static class Menus
        {
            public static string FileMenuName = T._("File");
        }
    }
}

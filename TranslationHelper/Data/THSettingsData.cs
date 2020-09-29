using System.IO;

namespace TranslationHelper.Data
{
    internal class THSettingsData
    {
        internal static string ApplicationStartupPath()
        {
            return Properties.Settings.Default.ApplicationStartupPath;
        }

        internal static string DBDirName()
        {
            return "DB";
        }

        internal static string DBDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), DBDirName());
        }

        internal static string ResDirName()
        {
            return "Res";
        }

        internal static string ResDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), ResDirName());
        }

        internal static string ResDirPath2()
        {
            return @".\" + ResDirName();
        }

        internal static string WorkDirName()
        {
            return "Work";
        }

        internal static string WorkDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), WorkDirName());
        }

        internal static string WorkDirPath2()
        {
            return @".\" + WorkDirName();
        }

        internal static string ArcConvDirName()
        {
            return "arc_conv";
        }

        internal static string ArcConvDirPath()
        {
            return Path.Combine(ResDirPath(), ArcConvDirName());
        }

        internal static string ArcConvExeName()
        {
            return "arc_conv.exe";
        }

        internal static string ArcConvExePath()
        {
            return Path.Combine(ArcConvDirPath(), ArcConvExeName());
        }

        internal static string NScriptDirName()
        {
            return "nscript";
        }

        internal static string NScriptDirPath()
        {
            return Path.Combine(ResDirPath(), NScriptDirName());
        }

        internal static string NSDECexeName()
        {
            return "NSDEC.exe";
        }

        internal static string NSDECexePath()
        {
            return Path.Combine(NScriptDirPath(), NSDECexeName());
        }

        internal static string Python37Path()
        {
            return Path.Combine(ResDirPath(), "python37");
        }

        internal static string Python37Path2()
        {
            return @".\" + ResDirName() + @"\python37";
        }

        internal static string Python37ExePath()
        {
            return Path.Combine(Python37Path(), "python.exe");
        }

        internal static string OriginalColumnName()
        {
            return "Original";
        }

        internal static string TranslationColumnName()
        {
            return "Translation";
        }

        internal static string Python37ExePath2()
        {
            return Python37Path2() + @"\python.exe";
        }

        internal static string SCPackerPath()
        {
            return Path.Combine(ResDirPath(), "scpacker");
        }

        internal static string SCPackerPath2()
        {
            return @".\" + ResDirName() + @"\scpacker";
        }

        internal static string SCPackerPYPath()
        {
            return Path.Combine(ResDirPath(), "scpacker", "scpacker.py");
        }

        internal static string SCPackerPYPath2()
        {
            return SCPackerPath2() + @"\scpacker.py";
        }

        internal static string DXADecodeWFolderName()
        {
            return "dxadecodew";
        }

        internal static string THTranslationCacheFileName()
        {
            return "THTranslationCache.cmx";
        }

        internal static string THTranslationCacheFilePath()
        {
            return Path.Combine(DBDirPath(), THTranslationCacheFileName());
        }

        internal static string DXADecodeWDirPath()
        {
            return Path.Combine(ResDirPath(), DXADecodeWFolderName());
        }

        internal static string DXADecodeWExePath()
        {
            return Path.Combine(DXADecodeWDirPath(), "DXADecode.exe");
        }

        internal static string DXAEncodeWExePath()
        {
            return Path.Combine(DXADecodeWDirPath(), "DXAEncode.exe");
        }

        internal static string TranslationRegexRulesFileName()
        {
            return "TranslationHelperTranslationRegexRules.txt";
        }

        internal static string TranslationRegexRulesFilePath()
        {
            return Path.Combine(ApplicationStartupPath(), TranslationRegexRulesFileName());
        }

        internal static string WolfTransPath()
        {
            return Path.Combine(ResDirPath(), "wolftrans", "bin", "wolftrans");
        }

        internal static string RubyPath()
        {
            return Path.Combine(ResDirPath(), "ruby", "bin", "ruby.exe");
        }

        internal static string THLogPath()
        {
            return Path.Combine(ApplicationStartupPath(), Properties.Settings.Default.ApplicationProductName + ".log");
        }

        internal static string CellFixesRegexRulesFileName()
        {
            return "TranslationHelperCellFixesRegexRules.txt";
        }

        internal static string CellFixesRegexRulesFilePath()
        {
            return Path.Combine(ApplicationStartupPath(), CellFixesRegexRulesFileName());
        }

        internal static string KiriKiriToolDirName()
        {
            return "kirikiriunpacker";
        }

        internal static string KiriKiriToolDirPath()
        {
            return Path.Combine(ResDirPath(), KiriKiriToolDirName());
        }

        internal static string KiriKiriToolExePath()
        {
            return Path.Combine(KiriKiriToolDirPath(), "kikiriki.exe");
        }

        internal static string KiriKiriToolDllPath()
        {
            return Path.Combine(KiriKiriToolDirPath(), "madCHook.dll");
        }

        internal static string LocaleEmulatorEXE()
        {
            return Path.Combine(ResDirPath(), "localeemulator", "LEProc.exe");
        }

        internal static string DBAutoSavesDirName()
        {
            return "Auto";
        }
    }
}

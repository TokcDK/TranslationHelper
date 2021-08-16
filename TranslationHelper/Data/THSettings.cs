using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TranslationHelper.Properties;

namespace TranslationHelper.Data
{
    internal class ThSettings
    {
        /// <summary>
        /// startup path of the application
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ApplicationStartupPath()
        {
            return Settings.Default.ApplicationStartupPath;
        }

        /// <summary>
        /// Log name of the application
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ApplicationLogName()
        {
            return Properties.Settings.Default.ApplicationProductName + ".log";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DbDirName()
        {
            return "DB";
        }

        internal static string DbDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), DbDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerTransDirName()
        {
            return "rpgmakertrans";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerTransDirPath()
        {
            return Path.Combine(ResDirPath(), RpgMakerTransDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerTransExeName()
        {
            return "rpgmt.exe";
        }

        /// <summary>
        /// path to Scripts dir of Python37
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string Python37ScriptsPath()
        {
            return Path.Combine(PythonPath(), "Scripts");
        }

        /// <summary>
        /// path for PyLiveMaker extraction tools
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerExtractionToolsPath()
        {
            return Python37ScriptsPath();
        }

        /// <summary>
        /// path for PyLiveMaker lmar extraction tool exe name
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLmarExtractionToolExeName()
        {
            return "lmar.exe";
        }

        /// <summary>
        /// path for PyLiveMaker lmar extraction tool exe for game main resources extraction
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLmarExtractionToolsPath()
        {
            return Path.Combine(PyLiveMakerExtractionToolsPath(), PyLiveMakerLmarExtractionToolExeName());
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLmlsbExtractionToolExeName()
        {
            return "lmlsb.exe";
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLmlsbExtractionToolPath()
        {
            return Path.Combine(PyLiveMakerExtractionToolsPath(), PyLiveMakerLmlsbExtractionToolExeName());
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLmpatchExtractionToolExeName()
        {
            return "lmpatch.exe";
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLmpatchExtractionToolPath()
        {
            return Path.Combine(PyLiveMakerExtractionToolsPath(), PyLiveMakerLmpatchExtractionToolExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerTransExePath()
        {
            return Path.Combine(RpgMakerTransDirPath(), RpgMakerTransExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ResDirName()
        {
            return "Res";
        }

        /// <summary>
        /// Res dir path
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ResDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), ResDirName());
        }

        /// <summary>
        /// Res dir path
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ResDirPath2()
        {
            return Path.GetFullPath(@".\" + ResDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string WorkDirName()
        {
            return "Work";
        }

        /// <summary>
        /// path to Work dir where project files placed
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string WorkDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), WorkDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string WorkDirPath2()
        {
            return @".\" + WorkDirName();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RulesDirName()
        {
            return "rules";
        }

        /// <summary>
        /// rules dir path where is placed some rules files
        /// </summary>
        /// <returns></returns>
        internal static string RulesDirPath()
        {
            return Path.Combine(ResDirPath(), RulesDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerMvSkipjsRulesFileName()
        {
            return "rpgmvskipjs.txt";
        }

        /// <summary>
        /// path to general skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerMvSkipjsRulesFilePath()
        {
            return Path.Combine(ThSettings.RulesDirPath(), RpgMakerMvSkipjsRulesFileName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerMvProjectSkipjsRulesFileName()
        {
            return "skipjs.txt";
        }

        /// <summary>
        /// path to project specific skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RpgMakerMvProjectSkipjsRulesFilePath()
        {
            return Path.Combine(ProjectData.SelectedGameDir, "www", "js", RpgMakerMvProjectSkipjsRulesFileName());
        }

        /// <summary>
        /// list of skijs rules file paths
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string[] RpgMakerMvSkipjsRulesFilesList()
        {
            return new[]
                {
                    ThSettings.RpgMakerMvSkipjsRulesFilePath(),//overall file
                    ThSettings.RpgMakerMvProjectSkipjsRulesFilePath()//game specific file
                };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ArcConvDirName()
        {
            return "arc_conv";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ArcConvDirPath()
        {
            return Path.Combine(ResDirPath(), ArcConvDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ArcConvExeName()
        {
            return "arc_conv.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ArcConvExePath()
        {
            return Path.Combine(ArcConvDirPath(), ArcConvExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string NScriptDirName()
        {
            return "nscript";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool SourceLanguageIsJapanese()
        {
            return Settings.Default.OnlineTranslationSourceLanguage.EndsWith("ja");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SourceLanguage()
        {
            return Settings.Default.OnlineTranslationSourceLanguage;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SourceLanguageName()
        {
            return SourceLanguage().Split(' ')[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SourceLanguageCode()
        {
            return SourceLanguage().Split(' ')[1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string TargetLanguage()
        {
            return Settings.Default.OnlineTranslationTargetLanguage;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string TargetLanguageName()
        {
            return TargetLanguage().Split(' ')[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string TargetLanguageCode()
        {
            return TargetLanguage().Split(' ')[1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string NScriptDirPath()
        {
            return Path.Combine(ResDirPath(), NScriptDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string NsdeCexeName()
        {
            return "NSDEC.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string NsdeCexePath()
        {
            return Path.Combine(NScriptDirPath(), NsdeCexeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PythonDirName()
        {
            return "python38";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PythonPath()
        {
            return Path.Combine(ResDirPath(), PythonDirName());
        }

        internal static string PythonPath2()
        {
            return @".\" + ResDirName() + @"\" + PythonDirName();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PythonExePath()
        {
            return Path.Combine(PythonPath(), "python.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string OriginalColumnName()
        {
            return "Original";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string TranslationColumnName()
        {
            return "Translation";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string Python37ExePath2()
        {
            return PythonPath2() + @"\python.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ScPackerPath()
        {
            return Path.Combine(ResDirPath(), "scpacker");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ScPackerPath2()
        {
            return @".\" + ResDirName() + @"\scpacker";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ScPackerPyPath()
        {
            return Path.Combine(ResDirPath(), "scpacker", "scpacker.py");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ScPackerPyPath2()
        {
            return ScPackerPath2() + @"\scpacker.py";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxaDecodeWFolderName()
        {
            return "dxadecodew";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ThTranslationCacheFileName()
        {
            return "THTranslationCache.cmx";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ThTranslationCacheFilePath()
        {
            return Path.Combine(DbDirPath(), ThTranslationCacheFileName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string WolfRpgExtractorExePath()
        {
            return WolfRpgExtractorsList()[1];
        }

        /// <summary>
        /// list of exe paths for wolf rpg wolf-files extractors
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Dictionary<int, string> WolfRpgExtractorsList()
        {
            return new Dictionary<int, string>
            {
                { 1, DxExtractExePath()},
                { 2, DxaDecodeWExePath()},
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxExtractDirName()
        {
            return "DXExtract";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxExtractDirPath()
        {
            return Path.Combine(ResDirPath(), DxExtractDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxExtractExeName()
        {
            return "DXExtract.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxExtractExePath()
        {
            return Path.Combine(DxExtractDirPath(), DxExtractExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxaDecodeWDirPath()
        {
            return Path.Combine(ResDirPath(), DxaDecodeWFolderName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxaDecodeWExePath()
        {
            return Path.Combine(DxaDecodeWDirPath(), "DXADecode.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DxaEncodeWExePath()
        {
            return Path.Combine(DxaDecodeWDirPath(), "DXAEncode.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string TranslationRegexRulesFileName()
        {
            return "TranslationHelperTranslationRegexRules.txt";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string TranslationRegexRulesFilePath()
        {
            return Path.Combine(ApplicationStartupPath(), TranslationRegexRulesFileName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string WolfTransPath()
        {
            return Path.Combine(ResDirPath(), "wolftrans", "bin", "wolftrans");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RubyPath()
        {
            return Path.Combine(ResDirPath(), "ruby", "bin", "ruby.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ThLogPath()
        {
            return Path.Combine(ApplicationStartupPath(), Settings.Default.ApplicationProductName + ".log");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string CellFixesRegexRulesFileName()
        {
            return "TranslationHelperCellFixesRegexRules.txt";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string CellFixesRegexRulesFilePath()
        {
            return Path.Combine(ApplicationStartupPath(), CellFixesRegexRulesFileName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string KiriKiriToolDirName()
        {
            return "kirikiriunpacker";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string KiriKiriToolDirPath()
        {
            return Path.Combine(ResDirPath(), KiriKiriToolDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string KiriKiriToolExePath()
        {
            return Path.Combine(KiriKiriToolDirPath(), "kikiriki.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string KiriKiriToolDllPath()
        {
            return Path.Combine(KiriKiriToolDirPath(), "madCHook.dll");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string LocaleEmulatorExe()
        {
            return Path.Combine(ResDirPath(), "localeemulator", "LEProc.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DbAutoSavesDirName()
        {
            return "Auto";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RgssDecrypterDirName()
        {
            return "rgssdecryptor";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RgssDecrypterDirPath()
        {
            return Path.Combine(ResDirPath(), RgssDecrypterDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RgssDecrypterExeName()
        {
            return "RgssDecrypter.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RgssDecrypterExePath()
        {
            return Path.Combine(RgssDecrypterDirPath(), RgssDecrypterExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AliceToolsDirName()
        {
            return "alice-tools";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AliceToolsDirPath()
        {
            return Path.Combine(ResDirPath(), AliceToolsDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AliceToolsExeName()
        {
            return "alice.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AliceToolsExePath()
        {
            return Path.Combine(AliceToolsDirPath(), AliceToolsExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string CustomDbName()
        {
            return "Custom.cmx";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string CustomDbPath()
        {
            return Path.Combine(DbDirPath(), CustomDbName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SoundsTextRegexPattern()
        {
            return "(((ぴちゃ)|(ぬっぶ)|(ぬっぷ)|(にちゅ)|(ひぇぐ)|(くにゅ)|(にゅく)|(べ[(ちゃ)]+)|(グル+)|(くちゅ)|(ぶちゅる+[ぅう]+)|(ちゅぷ)|(んほ[ぇえ]+)|(ギ+[ィイ]+)|(ふ[えぇ]+[ぁあ}+)|(ぐぢゅ[う]*)|(ぢゅる+)|(フ[ギャ]+)|(ずぶ+[ぅう]*)|(ぐぶ+)|(ぐぢゅう*)|(にゅっ?ぷ)|([クヌプ]チュ)|([ぷぴぬずぐ]ちゅ)|(ヒ+イ+)|(グヒ+)|(キヒ+)|(クホ+)|(グフ+)|(ぬるぅ)|(ぴゅ)|(くち)|(くっ)|(んく)|(ちゅ)|(イクう*)|(びゅる+)|(ずぶ[うぅ]+)|(ぬぷ+)|(ぶず+)|(フゴ[ォオ]+)|(んぎゅン)|(ドチュル+ゥ)|(ブ?(ビュ)+ゥ*)|(ウグ+)|(びゅ[ぐく])|(ん?は[ぁあ]+[ーっンッ]*)|(ん[ぅう…]*え)|(いや[ぁあ]+)|(ギィヒ+)|(ン[ぁあ]+)|(ん?…*[ふぅう]+[ぁあ]*)|(ぶ[(じゅ)]+)|(ぶ[りゅ]+)|(ふ[ぅう]+)|(う[んぁあ]+[ーっン]*)|(んゃ+[ぁあ]*[ーっン]*)|(へ[ぁあ]+)|(ん[ぁあ]*[ーっン]*)|(キ[ャアァ]+)|(あぎ)|(キ[イィ]+)|(ン?ぐ、?[ぅうぁあ]+)|([あは]+)|(く、?[ぅう]+[ーっン]*)|([ぅう]+)|(ひん+)|([ギ]+[ィ]*)|(へ[えぇ]+)|(ほ[お]+)|(ン?[オォおぉ]+)|(ふ+…+)|(や?[ぁあ]+[ーっン]*)|([いぃ]+)|(ゲ+)|(ン+)|(ッ+)|(く+)|(ひ+)|(グ+))[、…ーっッ]*)+";
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TranslationHelper.Properties;

namespace TranslationHelper.Data
{
    internal class THSettingsData
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ApplicationStartupPath()
        {
            return Settings.Default.ApplicationStartupPath;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DBDirName()
        {
            return "DB";
        }

        internal static string DBDirPath()
        {
            return Path.Combine(ApplicationStartupPath(), DBDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerTransDirName()
        {
            return "rpgmakertrans";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerTransDirPath()
        {
            return Path.Combine(ResDirPath(), RPGMakerTransDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerTransEXEName()
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
        internal static string PyLiveMakerLMARExtractionToolEXEName()
        {
            return "lmar.exe";
        }

        /// <summary>
        /// path for PyLiveMaker lmar extraction tool exe for game main resources extraction
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLMARExtractionToolsPath()
        {
            return Path.Combine(PyLiveMakerExtractionToolsPath(), PyLiveMakerLMARExtractionToolEXEName());
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLMLSBExtractionToolEXEName()
        {
            return "lmlsb.exe";
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLMLSBExtractionToolPath()
        {
            return Path.Combine(PyLiveMakerExtractionToolsPath(), PyLiveMakerLMLSBExtractionToolEXEName());
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLMPATCHExtractionToolEXEName()
        {
            return "lmpatch.exe";
        }

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string PyLiveMakerLMPATCHExtractionToolPath()
        {
            return Path.Combine(PyLiveMakerExtractionToolsPath(), PyLiveMakerLMPATCHExtractionToolEXEName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerTransEXEPath()
        {
            return Path.Combine(RPGMakerTransDirPath(), RPGMakerTransEXEName());
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
        internal static string RPGMakerMVSkipjsRulesFileName()
        {
            return "rpgmvskipjs.txt";
        }

        /// <summary>
        /// path to general skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerMVSkipjsRulesFilePath()
        {
            return Path.Combine(THSettingsData.RulesDirPath(), RPGMakerMVSkipjsRulesFileName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerMVProjectSkipjsRulesFileName()
        {
            return "skipjs.txt";
        }

        /// <summary>
        /// path to project specific skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RPGMakerMVProjectSkipjsRulesFilePath()
        {
            return Path.Combine(ProjectData.SelectedGameDir, "www", "js", RPGMakerMVProjectSkipjsRulesFileName());
        }

        /// <summary>
        /// list of skijs rules file paths
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string[] RPGMakerMVSkipjsRulesFilesList()
        {
            return new[]
                {
                    THSettingsData.RPGMakerMVSkipjsRulesFilePath(),//overall file
                    THSettingsData.RPGMakerMVProjectSkipjsRulesFilePath()//game specific file
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
        internal static string NSDECexeName()
        {
            return "NSDEC.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string NSDECexePath()
        {
            return Path.Combine(NScriptDirPath(), NSDECexeName());
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
            return @".\" + ResDirName() + @"\"+ PythonDirName();
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
        internal static string SCPackerPath()
        {
            return Path.Combine(ResDirPath(), "scpacker");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SCPackerPath2()
        {
            return @".\" + ResDirName() + @"\scpacker";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SCPackerPYPath()
        {
            return Path.Combine(ResDirPath(), "scpacker", "scpacker.py");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SCPackerPYPath2()
        {
            return SCPackerPath2() + @"\scpacker.py";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXADecodeWFolderName()
        {
            return "dxadecodew";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string THTranslationCacheFileName()
        {
            return "THTranslationCache.cmx";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string THTranslationCacheFilePath()
        {
            return Path.Combine(DBDirPath(), THTranslationCacheFileName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string WolfRPGExtractorExePath()
        {
            return WolfRPGExtractorsList()[1];
        }

        /// <summary>
        /// list of exe paths for wolf rpg wolf-files extractors
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Dictionary<int, string> WolfRPGExtractorsList()
        {
            return new Dictionary<int, string> 
            {
                { 1, DXExtractExePath()},
                { 2, DXADecodeWExePath()},
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXExtractDirName()
        {
            return "DXExtract";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXExtractDirPath()
        {
            return Path.Combine(ResDirPath(), DXExtractDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXExtractExeName()
        {
            return "DXExtract.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXExtractExePath()
        {
            return Path.Combine(DXExtractDirPath(), DXExtractExeName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXADecodeWDirPath()
        {
            return Path.Combine(ResDirPath(), DXADecodeWFolderName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXADecodeWExePath()
        {
            return Path.Combine(DXADecodeWDirPath(), "DXADecode.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DXAEncodeWExePath()
        {
            return Path.Combine(DXADecodeWDirPath(), "DXAEncode.exe");
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
        internal static string THLogPath()
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
        internal static string LocaleEmulatorEXE()
        {
            return Path.Combine(ResDirPath(), "localeemulator", "LEProc.exe");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string DBAutoSavesDirName()
        {
            return "Auto";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RGSSDecrypterDirName()
        {
            return "rgssdecryptor";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RGSSDecrypterDirPath()
        {
            return Path.Combine(ResDirPath(), RGSSDecrypterDirName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RGSSDecrypterEXEName()
        {
            return "RgssDecrypter.exe";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string RGSSDecrypterEXEPath()
        {
            return Path.Combine(RGSSDecrypterDirPath(), RGSSDecrypterEXEName());
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
        internal static string CustomDBName()
        {
            return "Custom.cmx";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string CustomDBPath()
        {
            return Path.Combine(DBDirPath(), CustomDBName());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string SoundsTextRegexPattern()
        {
            return "(((ぴちゃ)|(ぬっぶ)|(ぬっぷ)|(にちゅ)|(ひぇぐ)|(くにゅ)|(にゅく)|(べ[(ちゃ)]+)|(グル+)|(くちゅ)|(ぶちゅる+[ぅう]+)|(ちゅぷ)|(んほ[ぇえ]+)|(ギ+[ィイ]+)|(ふ[えぇ]+[ぁあ}+)|(ぐぢゅ[う]*)|(ぢゅる+)|(フ[ギャ]+)|(ずぶ+[ぅう]*)|(ぐぶ+)|(ぐぢゅう*)|(にゅっ?ぷ)|([クヌプ]チュ)|([ぷぴぬずぐ]ちゅ)|(ヒ+イ+)|(グヒ+)|(キヒ+)|(クホ+)|(グフ+)|(ぬるぅ)|(ぴゅ)|(くち)|(くっ)|(んく)|(ちゅ)|(イクう*)|(びゅる+)|(ずぶ[うぅ]+)|(ぬぷ+)|(ぶず+)|(フゴ[ォオ]+)|(んぎゅン)|(ドチュル+ゥ)|(ブ?(ビュ)+ゥ*)|(ウグ+)|(びゅ[ぐく])|(ん?は[ぁあ]+[ーっンッ]*)|(ん[ぅう…]*え)|(いや[ぁあ]+)|(ギィヒ+)|(ン[ぁあ]+)|(ん?…*[ふぅう]+[ぁあ]*)|(ぶ[(じゅ)]+)|(ぶ[りゅ]+)|(ふ[ぅう]+)|(う[んぁあ]+[ーっン]*)|(んゃ+[ぁあ]*[ーっン]*)|(へ[ぁあ]+)|(ん[ぁあ]*[ーっン]*)|(キ[ャアァ]+)|(あぎ)|(キ[イィ]+)|(ン?ぐ、?[ぅうぁあ]+)|([あは]+)|(く、?[ぅう]+[ーっン]*)|([ぅう]+)|(ひん+)|([ギ]+[ィ]*)|(へ[えぇ]+)|(ほ[お]+)|(ン?[オォおぉ]+)|(ふ+…+)|(や?[ぁあ]+[ーっン]*)|([いぃ]+)|(ゲ+)|(ン+)|(ッ+)|(く+)|(ひ+)|(グ+))[、…ーっッ]*)+";
        }
    }
}

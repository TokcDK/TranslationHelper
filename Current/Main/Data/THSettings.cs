using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TranslationHelper.Properties;

namespace TranslationHelper.Data
{
    internal class THSettings
    {
        /// <summary>
        /// startup path of the application
        /// </summary>
        /// <returns></returns>
        internal static string ApplicationStartupPath => AppSettings.ApplicationStartupPath;


        internal static string RPGMakerMVSkipCodesFilePath => AppData.CurrentProject != null ? Path.Combine(AppData.CurrentProject.SelectedGameDir, "skipcodes.txt") : "skipcodes.txt";

        /// <summary>
        /// Log name of the application
        /// </summary>
        /// <returns></returns>

        internal static string ApplicationLogName => AppSettings.ApplicationProductName + ".log";

        internal static string TranslationFileSourceDirSuffix { get => "THTranslationDB"; }

        internal static string DBDirName { get; } = "DB";

        internal static string DBDirPath => Path.Combine(Application.StartupPath, DBDirName);
        internal static string DBDirPathByLanguage => Path.Combine(DBDirPath, THSettings.SourceLanguage, THSettings.TargetLanguage);


        internal static string RPGMakerTransDirName { get; } = "rpgmakertrans";


        internal static string RPGMakerTransDirPath => Path.Combine(ToolsDirPath, RPGMakerTransDirName);


        internal static string RPGMakerTransEXEName { get; } = "rpgmt.exe";

        /// <summary>
        /// path to Scripts dir of Python37
        /// </summary>
        /// <returns></returns>

        internal static string Python37ScriptsPath => Path.Combine(PythonPath, "Scripts");

        /// <summary>
        /// path for PyLiveMaker extraction tools
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerExtractionToolsPath => Python37ScriptsPath;

        /// <summary>
        /// path for PyLiveMaker lmar extraction tool exe name
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMARExtractionToolEXEName { get; } = "lmar.exe";

        /// <summary>
        /// path for PyLiveMaker lmar extraction tool exe for game main resources extraction
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMARExtractionToolsPath => Path.Combine(PyLiveMakerExtractionToolsPath, PyLiveMakerLMARExtractionToolEXEName);

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMLSBExtractionToolEXEName { get; } = "lmlsb.exe";

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMLSBExtractionToolPath => Path.Combine(PyLiveMakerExtractionToolsPath, PyLiveMakerLMLSBExtractionToolEXEName);

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMPATCHExtractionToolEXEName { get; } = "lmpatch.exe";

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMPATCHExtractionToolPath => Path.Combine(PyLiveMakerExtractionToolsPath, PyLiveMakerLMPATCHExtractionToolEXEName);

        
        internal static string RPGMakerTransEXEPath => Path.Combine(RPGMakerTransDirPath, RPGMakerTransEXEName);


        internal static string ResDirName { get; } = "Res";

        /// <summary>
        /// Res dir path
        /// </summary>
        /// <returns></returns>

        internal static string ResDirPath => Path.Combine(ApplicationStartupPath, ResDirName);

        /// <summary>
        /// Res dir path
        /// </summary>
        /// <returns></returns>

        internal static string ResDirPathRelative => Path.GetFullPath(@".\" + ResDirName);
        internal static string ToolsDirName { get; } = "tools";
        internal static string ToolsDirPath => Path.Combine(ResDirPathRelative, ToolsDirName);


        internal static string WorkDirName { get; } = "Work";

        /// <summary>
        /// path to Work dir where project files placed
        /// </summary>
        /// <returns></returns>

        internal static string WorkDirPath => Path.Combine(ApplicationStartupPath, WorkDirName);


        internal static string WorkDirPathRelative => @".\" + WorkDirName;


        internal static string RulesDirName { get; } = "rules";

        /// <summary>
        /// rules dir path where is placed some rules files
        /// </summary>
        /// <returns></returns>
        internal static string RulesDirPath => Path.Combine(ResDirPath, RulesDirName);


        internal static string RPGMakerMVSkipjsRulesFileName { get; } = "rpgmvskipjs.txt";

        /// <summary>
        /// path to general skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>

        internal static string RPGMakerMVSkipjsRulesFilePath => Path.Combine(RulesDirPath, RPGMakerMVSkipjsRulesFileName);


        internal static string RPGMakerMVProjectSkipjsRulesFileName { get; } = "skipjs.txt";

        /// <summary>
        /// path to project specific skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>

        internal static string RPGMakerMVProjectSkipjsRulesFilePath => Path.Combine(AppData.CurrentProject.SelectedGameDir, "www", "js", RPGMakerMVProjectSkipjsRulesFileName);

        /// <summary>
        /// list of skijs rules file paths
        /// </summary>
        /// <returns></returns>

        internal static string[] RPGMakerMVSkipjsRulesFilesList => new[]
                {
                    RPGMakerMVSkipjsRulesFilePath,//overall file
                    RPGMakerMVProjectSkipjsRulesFilePath                };


        internal static string ArcConvDirName { get; } = "arc_conv";


        internal static string ArcConvDirPath => Path.Combine(ToolsDirPath, ArcConvDirName);


        internal static string ArcConvExeName { get; } = "arc_conv.exe";


        internal static string ArcConvExePath => Path.Combine(ArcConvDirPath, ArcConvExeName);


        internal static string NScriptDirName { get; } = "nscript";


        internal static bool SourceLanguageIsJapanese => AppSettings.OnlineTranslationSourceLanguage.EndsWith("ja");


        internal static string SourceLanguage => AppSettings.OnlineTranslationSourceLanguage;


        internal static string SourceLanguageName => SourceLanguage.Split(' ')[0];

        
        internal static string SourceLanguageCode => SourceLanguage.Split(' ')[1];


        internal static string TargetLanguage => AppSettings.OnlineTranslationTargetLanguage;


        internal static string TargetLanguageName => TargetLanguage.Split(' ')[0];

        
        internal static string TargetLanguageCode => TargetLanguage.Split(' ')[1];


        internal static string NScriptDirPath => Path.Combine(ToolsDirPath, NScriptDirName);


        internal static string NSDECexeName { get; } = "NSDEC.exe";


        internal static string NSDECexePath => Path.Combine(NScriptDirPath, NSDECexeName);


        internal static string PythonDirName { get; } = "python38";


        internal static string PythonPath => Path.Combine(ToolsDirPath, PythonDirName);

        internal static string PythonPathRelative => @".\" + ResDirName + @"\" + ToolsDirName + @"\" + PythonDirName;


        internal static string PythonExePath => Path.Combine(PythonPath, "python.exe");

        
        internal static string OriginalColumnName { get; } = "Original";

        
        internal static string TranslationColumnName { get; } = "Translation";

        
        internal static string Python37ExePathRelative => PythonPathRelative+ @"\python.exe";

        
        internal static string SCPackerPath => Path.Combine(ToolsDirPath, "scpacker");

        internal static string SCPackerPathRelative => @".\" + ResDirName + @".\" + ToolsDirName + @"\scpacker";


        internal static string SCPackerPYPath => Path.Combine(SCPackerPath, "scpacker.py");

        
        internal static string SCPackerPYPath2 => SCPackerPathRelative+ @"\scpacker.py";


        internal static string THTranslationCacheFileName { get; } = "THTranslationCache.cmx";


        internal static string THTranslationCacheFilePath => Path.Combine(DBDirPath, THTranslationCacheFileName);

        
        internal static string WolfRPGExtractorExePath => WolfRPGExtractorsList[1];

        /// <summary>
        /// list of exe paths for wolf rpg wolf-files extractors
        /// </summary>
        /// <returns></returns>

        internal static Dictionary<int, string> WolfRPGExtractorsList => new Dictionary<int, string>
            {
                { 1, DXExtractExePath},
                { 2, DXADecodeWExePath},
            };


        internal static string DXExtractDirName { get; } = "DXExtract";


        internal static string DXExtractDirPath => Path.Combine(ToolsDirPath, DXExtractDirName);


        internal static string DXExtractExeName { get; } = "DXExtract.exe";


        internal static string DXExtractExePath => Path.Combine(DXExtractDirPath, DXExtractExeName);


        internal static string DXADecodeWFolderName { get; } = "dxadecodew";


        internal static string WolfDecFolderName { get; } = "wolfdec";


        internal static string WolfdecDirPath => Path.Combine(ToolsDirPath, WolfDecFolderName);


        internal static string WolfdecExePath => Path.Combine(WolfdecDirPath, "wolfdec.exe");


        internal static string DXADecodeWDirPath => Path.Combine(ToolsDirPath, DXADecodeWFolderName);


        internal static string DXADecodeWExePath => Path.Combine(DXADecodeWDirPath, "DXADecode.exe");


        internal static string DXAEncodeWExePath => Path.Combine(DXADecodeWDirPath, "DXAEncode.exe");


        internal static string TranslationRegexRulesFileName { get; } = "TranslationHelperTranslationRegexRules.txt";


        internal static string TranslationRegexRulesFilePath => Path.Combine(ApplicationStartupPath, TranslationRegexRulesFileName);

        
        internal static string WolfTransPath => Path.Combine(ToolsDirPath, "wolftrans", "bin", "wolftrans");

        
        internal static string RubyPath => Path.Combine(ToolsDirPath, "ruby", "bin", "ruby.exe");

        
        internal static string THLogPath => Path.Combine(ApplicationStartupPath, AppSettings.ApplicationProductName + ".log");


        internal static string CellFixesRegexRulesFileName { get; } = "TranslationHelperCellFixesRegexRules.txt";


        internal static string CellFixesRegexRulesFilePath => Path.Combine(ApplicationStartupPath, CellFixesRegexRulesFileName);


        internal static string KiriKiriToolDirName { get; } = "kirikiriunpacker";


        internal static string KiriKiriToolDirPath => Path.Combine(ToolsDirPath, KiriKiriToolDirName);


        internal static string KiriKiriToolExePath => Path.Combine(KiriKiriToolDirPath, "kikiriki.exe");

        
        internal static string KiriKiriToolDllPath => Path.Combine(KiriKiriToolDirPath, "madCHook.dll");

        
        internal static string LocaleEmulatorEXE => Path.Combine(ToolsDirPath, "localeemulator", "LEProc.exe");

        
        internal static string DBAutoSavesDirName { get; } = "Auto";


        internal static string RGSSDecrypterDirName { get; } = "rgssdecryptor";


        internal static string RGSSDecrypterDirPath => Path.Combine(ToolsDirPath, RGSSDecrypterDirName);


        internal static string RGSSDecrypterEXEName { get; } = "RgssDecrypter.exe";


        internal static string RGSSDecrypterEXEPath => Path.Combine(RGSSDecrypterDirPath, RGSSDecrypterEXEName);


        internal static string AliceToolsDirName { get; } = "alice-tools";


        internal static string AliceToolsDirPath => Path.Combine(ToolsDirPath, AliceToolsDirName);


        internal static string AliceToolsExeName { get; } = "alice.exe";


        internal static string AliceToolsExePath => Path.Combine(AliceToolsDirPath, AliceToolsExeName);


        internal static string CustomDBName { get; } = "Custom.cmx";


        internal static string CustomDBPath => Path.Combine(DBDirPath, CustomDBName);

        
        internal static string SoundsTextRegexPattern { get; } = "(((ぴちゃ)|(ぬっぶ)|(ぬっぷ)|(にちゅ)|(ひぇぐ)|(くにゅ)|(にゅく)|(べ[(ちゃ)]+)|(グル+)|(くちゅ)|(ぶちゅる+[ぅう]+)|(ちゅぷ)|(んほ[ぇえ]+)|(ギ+[ィイ]+)|(ふ[えぇ]+[ぁあ}+)|(ぐぢゅ[う]*)|(ぢゅる+)|(フ[ギャ]+)|(ずぶ+[ぅう]*)|(ぐぶ+)|(ぐぢゅう*)|(にゅっ?ぷ)|([クヌプ]チュ)|([ぷぴぬずぐ]ちゅ)|(ヒ+イ+)|(グヒ+)|(キヒ+)|(クホ+)|(グフ+)|(ぬるぅ)|(ぴゅ)|(くち)|(くっ)|(んく)|(ちゅ)|(イクう*)|(びゅる+)|(ずぶ[うぅ]+)|(ぬぷ+)|(ぶず+)|(フゴ[ォオ]+)|(んぎゅン)|(ドチュル+ゥ)|(ブ?(ビュ)+ゥ*)|(ウグ+)|(びゅ[ぐく])|(ん?は[ぁあ]+[ーっンッ]*)|(ん[ぅう…]*え)|(いや[ぁあ]+)|(ギィヒ+)|(ン[ぁあ]+)|(ん?…*[ふぅう]+[ぁあ]*)|(ぶ[(じゅ)]+)|(ぶ[りゅ]+)|(ふ[ぅう]+)|(う[んぁあ]+[ーっン]*)|(んゃ+[ぁあ]*[ーっン]*)|(へ[ぁあ]+)|(ん[ぁあ]*[ーっン]*)|(キ[ャアァ]+)|(あぎ)|(キ[イィ]+)|(ン?ぐ、?[ぅうぁあ]+)|([あは]+)|(く、?[ぅう]+[ーっン]*)|([ぅう]+)|(ひん+)|([ギ]+[ィ]*)|(へ[えぇ]+)|(ほ[お]+)|(ン?[オォおぉ]+)|(ふ+…+)|(や?[ぁあ]+[ーっン]*)|([いぃ]+)|(ゲ+)|(ン+)|(ッ+)|(く+)|(ひ+)|(グ+))[、…ーっッ]*)+";

        internal static string SearchQueriesSectionName { get; } = "Search Queries";
        internal static string SearchReplacersSectionName { get; } = "Search Replacers";
        internal static string SearchReplacePatternsSectionName { get; } = "Search Replace patterns";
    }
}

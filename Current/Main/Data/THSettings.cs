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


        internal static string RPGMakerMVSkipCodesFilePath => Path.Combine(AppData.CurrentProject.SelectedGameDir, "skipcodes.txt");

        /// <summary>
        /// Log name of the application
        /// </summary>
        /// <returns></returns>

        internal static string ApplicationLogName => AppSettings.ApplicationProductName + ".log";


        internal static string DBDirName => "DB";
        internal static string DBDirPathByLanguage 
        { 
            get 
            {
                try
                {
                    if (!Directory.Exists(DBDirPath))
                    {
                        Directory.CreateDirectory(DBDirPath);
                    }
                    return Path.Combine(DBDirPath, THSettings.SourceLanguage, THSettings.TargetLanguage);
                }
                catch (Exception ex)
                {
                    new Functions.FunctionsLogs().LogToFile("An error occured while creating directory. error:\r\n" + ex);
                    return string.Empty;
                }
            } 
        }
        internal static string DBDirPath => Path.Combine(Application.StartupPath, DBDirName);
        internal static string DBDirPathByLanguage => Path.Combine(DBDirPath, THSettings.SourceLanguage, THSettings.TargetLanguage);


        internal static string RPGMakerTransDirName => "rpgmakertrans";


        internal static string RPGMakerTransDirPath => Path.Combine(ResDirPath, RPGMakerTransDirName);


        internal static string RPGMakerTransEXEName => "rpgmt.exe";

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

        internal static string PyLiveMakerLMARExtractionToolEXEName => "lmar.exe";

        /// <summary>
        /// path for PyLiveMaker lmar extraction tool exe for game main resources extraction
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMARExtractionToolsPath => Path.Combine(PyLiveMakerExtractionToolsPath, PyLiveMakerLMARExtractionToolEXEName);

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMLSBExtractionToolEXEName => "lmlsb.exe";

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMLSBExtractionToolPath => Path.Combine(PyLiveMakerExtractionToolsPath, PyLiveMakerLMLSBExtractionToolEXEName);

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe name
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMPATCHExtractionToolEXEName => "lmpatch.exe";

        /// <summary>
        /// path for PyLiveMaker lmlsb extraction tool exe for lsb files txt and menus text extraction
        /// </summary>
        /// <returns></returns>

        internal static string PyLiveMakerLMPATCHExtractionToolPath => Path.Combine(PyLiveMakerExtractionToolsPath, PyLiveMakerLMPATCHExtractionToolEXEName);


        internal static string RPGMakerTransEXEPath => Path.Combine(RPGMakerTransDirPath, RPGMakerTransEXEName);


        internal static string ResDirName => "Res";

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


        internal static string WorkDirName => "Work";

        /// <summary>
        /// path to Work dir where project files placed
        /// </summary>
        /// <returns></returns>

        internal static string WorkDirPath => Path.Combine(ApplicationStartupPath, WorkDirName);


        internal static string WorkDirPathRelative => @".\" + WorkDirName;


        internal static string RulesDirName => "rules";

        /// <summary>
        /// rules dir path where is placed some rules files
        /// </summary>
        /// <returns></returns>
        internal static string RulesDirPath => Path.Combine(ResDirPath, RulesDirName);


        internal static string RPGMakerMVSkipjsRulesFileName => "rpgmvskipjs.txt";

        /// <summary>
        /// path to general skipjs file list which will be skipped while rpg maker js opening
        /// </summary>
        /// <returns></returns>

        internal static string RPGMakerMVSkipjsRulesFilePath => Path.Combine(RulesDirPath, RPGMakerMVSkipjsRulesFileName);


        internal static string RPGMakerMVProjectSkipjsRulesFileName => "skipjs.txt";

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


        internal static string ArcConvDirName => "arc_conv";


        internal static string ArcConvDirPath => Path.Combine(ResDirPath, ArcConvDirName);


        internal static string ArcConvExeName => "arc_conv.exe";


        internal static string ArcConvExePath => Path.Combine(ArcConvDirPath, ArcConvExeName);


        internal static string NScriptDirName => "nscript";


        internal static bool SourceLanguageIsJapanese => AppSettings.OnlineTranslationSourceLanguage.EndsWith("ja");


        internal static string SourceLanguage => AppSettings.OnlineTranslationSourceLanguage;


        internal static string SourceLanguageName => SourceLanguage.Split(' ')[0];


        internal static string SourceLanguageCode => SourceLanguage.Split(' ')[1];


        internal static string TargetLanguage => AppSettings.OnlineTranslationTargetLanguage;


        internal static string TargetLanguageName => TargetLanguage.Split(' ')[0];


        internal static string TargetLanguageCode => TargetLanguage.Split(' ')[1];


        internal static string NScriptDirPath => Path.Combine(ResDirPath, NScriptDirName);


        internal static string NSDECexeName => "NSDEC.exe";


        internal static string NSDECexePath => Path.Combine(NScriptDirPath, NSDECexeName);


        internal static string PythonDirName => "python38";


        internal static string PythonPath => Path.Combine(ResDirPath, PythonDirName);

        internal static string PythonPathRelative => @".\" + ResDirName + @"\" + PythonDirName;


        internal static string PythonExePath => Path.Combine(PythonPath, "python.exe");


        internal static string OriginalColumnName => "Original";


        internal static string TranslationColumnName => "Translation";


        internal static string Python37ExePathRelative => PythonPathRelative + @"\python.exe";


        internal static string SCPackerPath => Path.Combine(ResDirPath, "scpacker");


        internal static string SCPackerPathRelative => @".\" + ResDirName + @"\scpacker";


        internal static string SCPackerPYPath => Path.Combine(ResDirPath, "scpacker", "scpacker.py");


        internal static string SCPackerPYPath2 => SCPackerPathRelative + @"\scpacker.py";


        internal static string THTranslationCacheFileName => "THTranslationCache.cmx";


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


        internal static string DXExtractDirName => "DXExtract";


        internal static string DXExtractDirPath => Path.Combine(ResDirPath, DXExtractDirName);


        internal static string DXExtractExeName => "DXExtract.exe";


        internal static string DXExtractExePath => Path.Combine(DXExtractDirPath, DXExtractExeName);


        internal static string DXADecodeWFolderName => "dxadecodew";


        internal static string WolfDecFolderName => "wolfdec";


        internal static string WolfdecDirPath => Path.Combine(ResDirPath, WolfDecFolderName);


        internal static string WolfdecExePath => Path.Combine(WolfdecDirPath, "wolfdec.exe");


        internal static string DXADecodeWDirPath => Path.Combine(ResDirPath, DXADecodeWFolderName);


        internal static string DXADecodeWExePath => Path.Combine(DXADecodeWDirPath, "DXADecode.exe");


        internal static string DXAEncodeWExePath => Path.Combine(DXADecodeWDirPath, "DXAEncode.exe");


        internal static string TranslationRegexRulesFileName => "TranslationHelperTranslationRegexRules.txt";


        internal static string TranslationRegexRulesFilePath => Path.Combine(ApplicationStartupPath, TranslationRegexRulesFileName);


        internal static string WolfTransPath => Path.Combine(ResDirPath, "wolftrans", "bin", "wolftrans");


        internal static string RubyPath => Path.Combine(ResDirPath, "ruby", "bin", "ruby.exe");


        internal static string THLogPath => Path.Combine(ApplicationStartupPath, AppSettings.ApplicationProductName + ".log");


        internal static string CellFixesRegexRulesFileName => "TranslationHelperCellFixesRegexRules.txt";


        internal static string CellFixesRegexRulesFilePath => Path.Combine(ApplicationStartupPath, CellFixesRegexRulesFileName);


        internal static string KiriKiriToolDirName => "kirikiriunpacker";


        internal static string KiriKiriToolDirPath => Path.Combine(ResDirPath, KiriKiriToolDirName);


        internal static string KiriKiriToolExePath => Path.Combine(KiriKiriToolDirPath, "kikiriki.exe");


        internal static string KiriKiriToolDllPath => Path.Combine(KiriKiriToolDirPath, "madCHook.dll");


        internal static string LocaleEmulatorEXE => Path.Combine(ResDirPath, "localeemulator", "LEProc.exe");


        internal static string DBAutoSavesDirName => "Auto";


        internal static string RGSSDecrypterDirName => "rgssdecryptor";


        internal static string RGSSDecrypterDirPath => Path.Combine(ResDirPath, RGSSDecrypterDirName);


        internal static string RGSSDecrypterEXEName => "RgssDecrypter.exe";


        internal static string RGSSDecrypterEXEPath => Path.Combine(RGSSDecrypterDirPath, RGSSDecrypterEXEName);


        internal static string AliceToolsDirName => "alice-tools";


        internal static string AliceToolsDirPath => Path.Combine(ResDirPath, AliceToolsDirName);


        internal static string AliceToolsExeName => "alice.exe";


        internal static string AliceToolsExePath => Path.Combine(AliceToolsDirPath, AliceToolsExeName);


        internal static string CustomDBName => "Custom.cmx";


        internal static string CustomDBPath => Path.Combine(DBDirPath, CustomDBName);


        internal static string SoundsTextRegexPattern => "(((ぴちゃ)|(ぬっぶ)|(ぬっぷ)|(にちゅ)|(ひぇぐ)|(くにゅ)|(にゅく)|(べ[(ちゃ)]+)|(グル+)|(くちゅ)|(ぶちゅる+[ぅう]+)|(ちゅぷ)|(んほ[ぇえ]+)|(ギ+[ィイ]+)|(ふ[えぇ]+[ぁあ}+)|(ぐぢゅ[う]*)|(ぢゅる+)|(フ[ギャ]+)|(ずぶ+[ぅう]*)|(ぐぶ+)|(ぐぢゅう*)|(にゅっ?ぷ)|([クヌプ]チュ)|([ぷぴぬずぐ]ちゅ)|(ヒ+イ+)|(グヒ+)|(キヒ+)|(クホ+)|(グフ+)|(ぬるぅ)|(ぴゅ)|(くち)|(くっ)|(んく)|(ちゅ)|(イクう*)|(びゅる+)|(ずぶ[うぅ]+)|(ぬぷ+)|(ぶず+)|(フゴ[ォオ]+)|(んぎゅン)|(ドチュル+ゥ)|(ブ?(ビュ)+ゥ*)|(ウグ+)|(びゅ[ぐく])|(ん?は[ぁあ]+[ーっンッ]*)|(ん[ぅう…]*え)|(いや[ぁあ]+)|(ギィヒ+)|(ン[ぁあ]+)|(ん?…*[ふぅう]+[ぁあ]*)|(ぶ[(じゅ)]+)|(ぶ[りゅ]+)|(ふ[ぅう]+)|(う[んぁあ]+[ーっン]*)|(んゃ+[ぁあ]*[ーっン]*)|(へ[ぁあ]+)|(ん[ぁあ]*[ーっン]*)|(キ[ャアァ]+)|(あぎ)|(キ[イィ]+)|(ン?ぐ、?[ぅうぁあ]+)|([あは]+)|(く、?[ぅう]+[ーっン]*)|([ぅう]+)|(ひん+)|([ギ]+[ィ]*)|(へ[えぇ]+)|(ほ[お]+)|(ン?[オォおぉ]+)|(ふ+…+)|(や?[ぁあ]+[ーっン]*)|([いぃ]+)|(ゲ+)|(ン+)|(ッ+)|(く+)|(ひ+)|(グ+))[、…ーっッ]*)+";
    }
}


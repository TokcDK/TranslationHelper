using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Formats.RPGMMV.JsonType;
using TranslationHelper.Formats.RPGMMV.Other;
using TranslationHelper.Formats.RPGMMV.PluginsCustom;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Projects.RPGMMV.Menus;

namespace TranslationHelper.Projects.RPGMMV
{
    internal class RPGMMVGame : ProjectBase
    {
        /// <summary>
        /// List of JavaScript types used for parsing specific plugin files.
        /// </summary>
        protected readonly List<Type> ListOfJS;

        /// <summary>
        /// Initializes a new instance of the <see cref="RPGMMVGame"/> class.
        /// </summary>
        public RPGMMVGame()
        {
            ListOfJS = JSBase.GetListOfJSTypes();
        }

        /// <summary>
        /// Gets a value indicating whether the project has a "www" directory.
        /// </summary>
        protected virtual bool HasWWWDir => true;

        /// <summary>
        /// Determines if the project is valid by checking the executable and system JSON file.
        /// </summary>
        /// <returns>True if valid, otherwise false.</returns>
        internal override bool IsValid()
        {
            return ProjectTools.IsExe(AppData.SelectedProjectFilePath) &&
                   File.Exists(Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "", "data", "system.json"));
        }

        /// <summary>
        /// Gets the file filter for game executable files.
        /// </summary>
        internal override string FileFilter => ProjectTools.GameExeFilter;

        /// <summary>
        /// Gets the name of the project type.
        /// </summary>
        public override string Name => "RPG Maker MV";

        /// <summary>
        /// Gets the database folder name for this project type.
        /// </summary>
        internal override string ProjectDBFolderName => "RPGMakerMV";

        /// <summary>
        /// Gets a value indicating whether test runs are enabled.
        /// </summary>
        internal override bool IsTestRunEnabled => true;

        /// <summary>
        /// The root directory path for the project's "www" folder or equivalent.
        /// </summary>
        protected static string WWWDir;

        /// <summary>
        /// Opens the project by setting the directory and parsing files.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public override bool Open()
        {
            WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");
            return ParseProjectFiles();
        }

        /// <summary>
        /// Determines if a table should be skipped for line splitting based on its name.
        /// </summary>
        internal override bool LineSplitProjectSpecificSkipForTable(DataTable table)
        {
            return table.TableName.ToUpperInvariant().EndsWith(".JS");
        }

        /// <summary>
        /// Determines if a line should be skipped for splitting based on specific conditions.
        /// </summary>
        internal override bool LineSplitProjectSpecificSkipForLine(string o, string t, int tind = -1, int rind = -1)
        {
            if (tind == -1 || rind == -1) return false;

            string cell = AppData.CurrentProject.FilesContentInfo.Tables[tind].Rows[rind][0] + string.Empty;
            return cell.Contains("Code=655") || cell.Contains("Code=355");
        }

        /// <summary>
        /// Message prefix used for progress updates during file parsing.
        /// </summary>
        protected string ParseFileMessage;

        /// <summary>
        /// Parses project files, either opening or saving them based on the mode.
        /// </summary>
        /// <returns>True if any file was processed successfully, otherwise false.</returns>
        private bool ParseProjectFiles()
        {
            if (OpenFileMode) BakRestore();

            ParseFileMessage = SaveFileMode ? T._("write file: ") : T._("opening file: ");
            bool hasAnyFileBeenProcessed = false;

            try
            {
                if (ParseFontsCS()) hasAnyFileBeenProcessed = true;

                var hardcodedJS = new HashSet<string>();
                if (ParsePlugins(hardcodedJS)) hasAnyFileBeenProcessed = true;

                var skipJSList = new HashSet<string>();
                SetSkipJSLists(skipJSList);

                if (ParseOtherQuotedJsPlugins(hardcodedJS, skipJSList)) hasAnyFileBeenProcessed = true;
                if (ParseJsons()) hasAnyFileBeenProcessed = true;
                if (ParsePluginsStrings()) hasAnyFileBeenProcessed = true;
            }
            catch (Exception ex)
            {
                // Exceptions are ignored per original functionality.
                // Consider logging in a future enhancement: Logger.Log(ex);
            }

            
            return hasAnyFileBeenProcessed;
        }

        /// <summary>
        /// Parses various plugin-related string files in specific directories.
        /// </summary>
        /// <returns>True if any file was processed, otherwise false.</returns>
        private bool ParsePluginsStrings()
        {
            bool hasAnyFileBeenProcessed = false;
            var configurations = new[]
            {
                (Dir: new DirectoryInfo(Path.Combine(WWWDir, "data")), Format: typeof(ExternMessageCSV), Mask: "ExternMessage.csv", SearchOption: SearchOption.AllDirectories),
                (Dir: new DirectoryInfo(Path.Combine(WWWDir, "data")), Format: typeof(QuestsTxt), Mask: "Quests.txt", SearchOption: SearchOption.TopDirectoryOnly),
                (Dir: new DirectoryInfo(Path.Combine(WWWDir, "data", "dobbyPlugin", "db")), Format: typeof(DobbyPluginDBCSV), Mask: "*.csv", SearchOption: SearchOption.AllDirectories),
                (Dir: new DirectoryInfo(Path.Combine(WWWDir, "data", "dobbyPlugin", "db", "tes")), Format: typeof(JsonEventCommandsList), Mask: "*.json", SearchOption: SearchOption.AllDirectories),
                (Dir: new DirectoryInfo(Path.Combine(WWWDir, "scenarios")), Format: typeof(StopCVPluginTXT), Mask: "*.txt", SearchOption: SearchOption.AllDirectories),
                (Dir: new DirectoryInfo(WWWDir), Format: typeof(ValueLocationPresetTxt), Mask: "valueLocationPreset.txt", SearchOption: SearchOption.AllDirectories)
            };

            foreach (var config in configurations)
            {
                if (ProjectToolsOpenSave.OpenSaveFilesBase(this, config.Dir, config.Format, config.Mask, searchOption: config.SearchOption))
                {
                    hasAnyFileBeenProcessed = true;
                }
            }

            return hasAnyFileBeenProcessed;
        }

        /// <summary>
        /// Parses JSON files in the data directory.
        /// </summary>
        /// <returns>True if any JSON file was processed, otherwise false.</returns>
        protected virtual bool ParseJsons()
        {
            var mvDataDir = new DirectoryInfo(Path.Combine(WWWDir, "data"));
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, mvDataDir, MVJsonFormats(), MVJsonFormatsMasks());
        }

        /// <summary>
        /// Gets the list of JSON format types.
        /// </summary>
        protected virtual List<Type> MVJsonFormats() => new List<Type> { typeof(JSON) };

        /// <summary>
        /// Gets the file masks for JSON files.
        /// </summary>
        protected virtual string[] MVJsonFormatsMasks() => new[] { "*.json" };

        /// <summary>
        /// Parses hardcoded JavaScript plugins specified in ListOfJS.
        /// </summary>
        /// <param name="hardcodedJS">Set of JS file names to exclude from other parsing.</param>
        /// <returns>True if any file was processed, otherwise false.</returns>
        private bool ParsePlugins(HashSet<string> hardcodedJS)
        {
            bool hasAnyFileBeenProcessed = false;
            foreach (var jsType in ListOfJS)
            {
                if (IsTypeExcluded(jsType)) continue;

                var js = (IUseJSLocationInfo)Activator.CreateInstance(jsType);
                var filePath = Path.Combine(WWWDir, "js", js.JSSubfolder, js.JSName);

                if (!File.Exists(filePath)) continue;

                try
                {
                    hardcodedJS.Add(js.JSName); //add js to exclude from parsing of other js
                    var format = (FormatBase)Activator.CreateInstance(jsType);
                    format.FilePath = filePath;

                    Logger.Info(ParseFileMessage + js.JSName);
                    if ((OpenFileMode && format.Open()) || (SaveFileMode && format.Save()))
                    {
                        hasAnyFileBeenProcessed = true;
                    }
                }
                catch
                {
                    // Exceptions are ignored per original functionality.
                }
            }
            return hasAnyFileBeenProcessed;
        }

        /// <summary>
        /// Determines if a JS type should be excluded from parsing.
        /// </summary>
        protected virtual bool IsTypeExcluded(Type jsType) => false;

        /// <summary>
        /// Parses other JavaScript plugins in the plugins directory, excluding hardcoded ones.
        /// </summary>
        /// <param name="hardcodedJS">Set of JS files already processed.</param>
        /// <param name="skipJSList">Set of JS files to skip.</param>
        /// <returns>True if any file was processed, otherwise false.</returns>
        private bool ParseOtherQuotedJsPlugins(HashSet<string> hardcodedJS, HashSet<string> skipJSList)
        {
            bool hasAnyFileBeenProcessed = false;
            foreach (var jsFileInfo in Directory.EnumerateFiles(Path.Combine(WWWDir, "js", "plugins"), "*.js"))
            {
                string jsName = Path.GetFileName(jsFileInfo);
                if (hardcodedJS.Contains(jsName) || skipJSList.Contains(jsName)) continue;
                if (!File.Exists(jsFileInfo)) continue;

                var format = new ZZZOtherJS { FilePath = jsFileInfo };
                Logger.Info(ParseFileMessage + jsName);

                try
                {
                    if ((OpenFileMode && format.Open()) || (SaveFileMode && format.Save()))
                    {
                        hasAnyFileBeenProcessed = true;
                    }
                }
                catch
                {
                    // Exceptions are ignored per original functionality.
                }
            }
            return hasAnyFileBeenProcessed;
        }

        /// <summary>
        /// Parses the gamefont.css file to allow font modifications.
        /// </summary>
        /// <returns>True if the file was processed, otherwise false.</returns>
        private bool ParseFontsCS()
        {
            var filePath = Path.Combine(WWWDir, "fonts", "gamefont.css");
            if (!File.Exists(filePath)) return false;

            var format = new GAMEFONTCSS { FilePath = filePath };
            Logger.Info(ParseFileMessage + "gamefont.css");

            try
            {
                return (OpenFileMode && format.Open()) || (SaveFileMode && format.Save());
            }
            catch
            {
                // Exceptions are ignored per original functionality.
                return false;
            }
        }

        /// <summary>
        /// Populates the skip list with JS file names from predefined rule files.
        /// </summary>
        /// <param name="skipJSList">Set to store JS files to skip.</param>
        private void SetSkipJSLists(HashSet<string> skipJSList)
        {
            foreach (var skipJsFilePath in THSettings.RPGMakerMVSkipjsRulesFilesList)
            {
                if (File.Exists(skipJsFilePath))
                {
                    skipJSList.UnionWith(File.ReadAllLines(skipJsFilePath)
                        .Select(line => line.Trim())
                        .Where(line => line.Length > 0 && line[0] != ';'));
                }
            }
        }

        /// <summary>
        /// Saves the project by parsing files in save mode.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public override bool Save()
        {
            if (string.IsNullOrWhiteSpace(WWWDir))
                WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");
            return ParseProjectFiles();
        }

        /// <summary>
        /// Gets or sets the list of paths to back up.
        /// </summary>
        public override List<string> BakPaths { get; set; } = new List<string>
    {
        @".\www\data",
        @".\www\fonts",
        @".\www\js"
    };

        /// <summary>
        /// Creates a backup of the specified project directories.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public override bool BakCreate()
        {
            if (string.IsNullOrWhiteSpace(WWWDir))
                WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");

            BakRestore();
            return ProjectToolsBackup.BackupRestorePaths(BakPaths);
        }

        /// <summary>
        /// Restores project directories from backup.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public override bool BakRestore()
        {
            if (string.IsNullOrWhiteSpace(WWWDir))
                WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");

            // Removed RestoreFromBakIfNeedData and RestoreFromBakIfNeedJS calls as they are redundant
            // with ProjectToolsBackup.BackupRestorePaths handling directory restoration.
            return ProjectToolsBackup.BackupRestorePaths(BakPaths, false);
        }

        private FillEmptyTablesLinesDictBase _filler;

        /// <summary>
        /// Applies hardcoded fixes specific to RPG Maker MV translations.
        /// </summary>
        internal override string HardcodedFixes(string original, string translation)
        {
            if (!translation.StartsWith(@"\n<>")) return translation;

            var name = Regex.Replace(original, @"^\\n<(.+)>[\s\S]*$", "$1");
            var translatedName = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(name);

            if (translatedName.Length > 0)
                return translation.Remove(3, name.Length).Insert(3, translatedName);

            if (_filler == null)
            {
                _filler = new FillEmptyTablesLinesDictForce();
                _filler.All();
            }

            return _filler.Translations.TryGetValue(name, out var value)
                ? translation.Remove(3, name.Length).Insert(3, value)
                : translation.Insert(3, name);
        }

        /// <summary>
        /// Cleans a string by removing specific patterns for validation.
        /// </summary>
        internal override string CleanStringForCheck(string str)
        {
            if (str == null) return str;

            if (Regex.IsMatch(str, @"<Mini Label: ([^>]+)>"))
                str = Regex.Match(str, @"<Mini Label: ([^>]+)>").Result("$1");

            return Regex.Replace(str, @"\\C\[[0-9]{1,3}\]", "");
        }

        /// <summary>
        /// Gets the list of file-related menu items.
        /// </summary>
        internal override IFileListMenuItem[] FilesListItemMenusList => new IFileListMenuItem[]
        {
            new AddToSkipJS(),
            new SkipJSFileOpen(),
            new OpenJsonSkipCodesList()
        };

        /// <summary>
        /// Checks if a string is valid for translation.
        /// </summary>
        internal override bool IsValidForTranslation(string inputString) => !inputString.StartsWith("<TE:");

        /// <summary>
        /// Checks if a data row has an issue requiring attention.
        /// </summary>
        internal override bool CheckForRowIssue(DataRow row)
        {
            var tableName = row.Table.TableName;
            if (!tableName.EndsWith(".js")) return false;

            var rowIndex = row.Table.Rows.IndexOf(row);
            var info = AppData.CurrentProject.FilesContentInfo.Tables[tableName].Rows[rowIndex].Field<string>(0);

            if (!info.Contains("Command code: 356")) return false;
            var originalText = row.Field<string>(AppData.CurrentProject.OriginalColumnIndex);

            return !(originalText.StartsWith("D_TEXT") || originalText.StartsWith("GabText"));
        }
    }
}

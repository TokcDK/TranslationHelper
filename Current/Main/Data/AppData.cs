﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Projects;

namespace TranslationHelper.Data
{
    public static class AppData
    {
        internal static THfrmSettings Settings;

        //Settings
        internal static void SetSettings()
        {
            Settings = new THfrmSettings();
            Settings.GetSettings();
        }

        /// <summary>
        /// init values and set references
        /// </summary>
        /// <param name="hfrmMain"></param>
        public static void Init(FormMain hfrmMain)
        {
            Main = hfrmMain;

            FilesListControl = new FilesListControlListBox(); // set using files list control

            //OriginalsTableRowCoordinates = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>>();
            //THFilesElementsDictionary = new Dictionary<string, string>();
            //THFilesElementsDictionaryInfo = new Dictionary<string, string>();

            SelectedProjectFilePath = string.Empty;

            ProjectsList = ProjectTools.GetListOfProjectTypes();
        }

        /// <summary>
        /// Application's loaded config ini
        /// </summary>
        internal static INIFileMan.INIFile ConfigIni { get => Settings.THConfigINI; set => Settings.THConfigINI = value; }

        /// <summary>
        /// regex rules which appling to original to show what need to translate
        /// </summary>
        internal static Dictionary<string, string> TranslationRegexRules = new Dictionary<string, string>();
        internal static Dictionary<string, string> TranslationRegexRulesGroup = new Dictionary<string, string>();

        /// <summary>
        /// translation cell fix regex rules. same as search and replace with regex using
        /// </summary>
        internal static Dictionary<string, string> CellFixesRegexRules = new Dictionary<string, string>();

        /// <summary>
        /// reference to the main form
        /// </summary>
        internal static FormMain Main;

        /// <summary>
        /// CurrentProject
        /// </summary>
        internal static ProjectBase CurrentProject;

        /// <summary>
        /// List of project types
        /// </summary>
        internal static List<Type> ProjectsList;

        /// <summary>
        /// Usually 'Selected file 'Path' in file browse dialog when open project
        /// </summary>
        internal static string SelectedProjectFilePath { get; set; }

        /// <summary>
        /// Online Translation Cache
        /// </summary>
        internal static FunctionsOnlineCache OnlineTranslationCache 
        {
            get
            {
                if(onlineTranslationCache == null)
                {
                    // autoinit and read
                    onlineTranslationCache = new FunctionsOnlineCache();
                    onlineTranslationCache.UsersCount++;
                    onlineTranslationCache.Read();
                }

                return onlineTranslationCache;
            }
            set
            {
                if (onlineTranslationCache == value) return;

                if(value == null)
                {
                    FunctionsOnlineCache.Unload();
                    return;
                }
                else if (value.cache.Count == 0)
                {
                    onlineTranslationCache.UsersCount++;
                    return;
                }

                onlineTranslationCache = value;
            }
        }
        static FunctionsOnlineCache onlineTranslationCache;

        /// <summary>
        /// target textbox control value
        /// </summary>
        internal static string TargetTextBoxPreValue;

        /// <summary>
        /// все баз данных в кучу здесь
        /// </summary>
        internal static Dictionary<string, string> AllDBmerged;// = new Dictionary<string, string>();

        /// <summary>
        /// Buffer temp value. String type.
        /// </summary>
        internal static string BufferValueString; // used in settings

        /// <summary>
        /// true when settings is loading
        /// </summary>
        internal static bool SettingsIsLoading;

        /// <summary>
        /// The program session online translation cookies
        /// </summary>
        internal static System.Net.CookieContainer OnlineTranslatorCookies;

        internal static Dictionary<char, int> ENQuotesToJPLearnDataFoundPrev;
        internal static Dictionary<char, int> ENQuotesToJPLearnDataFoundNext;

        /// <summary>
        /// [for json open\save improve] skipped rpg maker mv json event codes
        /// </summary>
        internal static Dictionary<int, int> RpgMVSkippedCodesStat = new Dictionary<int, int>();

        /// <summary>
        /// [for json open\save improve] added rpg maker mv json event codes
        /// </summary>
        internal static Dictionary<int, int> RpgMVAddedCodesStat = new Dictionary<int, int>();

        /// <summary>
        /// Application log
        /// </summary>
        internal static FunctionsLogs AppLog = new FunctionsLogs();

        ///// <summary>
        ///// Fileslist control object
        ///// </summary>
        //internal static object FilesList;

        /// <summary>
        /// Files list using now control
        /// </summary>
        internal static FilesListControlBase FilesListControl;

        /// <summary>
        /// Files list
        /// </summary>
        internal static ListBox THFilesList { get => Main.THFilesList; }
        public static string TranslationFileSourceDirSuffix { get => "THTranslationDB"; }
    }
}

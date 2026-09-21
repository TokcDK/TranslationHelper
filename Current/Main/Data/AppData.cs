using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Projects;
using TranslationHelper.Settings;

namespace TranslationHelper.Data
{
    public static class AppData
    {
        /// <summary>
        /// Reads every setting and writes any entry the INI file does not have yet.
        /// <para>
        /// This replaces the old "create the settings form and let it load its own controls" step.
        /// Settings no longer live in a form, so there is nothing to construct here: the form is
        /// created only when the user opens it, and it reads the same settings the rest of the
        /// application reads.
        /// </para>
        /// </summary>
        internal static void InitSettings()
        {
            SettingsRegistry.EnsureLoaded();
        }

        /// <summary>
        /// init values and set references
        /// </summary>
        /// <param name="hfrmMain"></param>
        public static void Init(FormMain hfrmMain)
        {
            Main = hfrmMain;

            FilesListControl = new FilesListControlListBox(); // set using files list control

            // Relates the entries of the files list to the content they present. Reads the current
            // project on every use, so one instance serves every project opened in this session.
            FilesListContent = new FilesListContent(() => CurrentProject?.FilesContent);

            SelectedProjectFilePath = string.Empty;

            ProjectsList = ProjectTools.GetListOfProjectTypes();
        }

        /// <summary>
        /// Application's loaded config ini.
        /// <para>
        /// This is the same file the settings are stored in. It is owned by
        /// <see cref="SettingsIniStore"/>, which keeps it open for the whole session, so reading it
        /// here cannot create a second handle on the file and cannot overwrite a setting that was
        /// just changed.
        /// </para>
        /// </summary>
        internal static INIFileMan.INIFile ConfigIni { get => SettingsIniStore.Ini; }

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
        /// target textbox control value
        /// </summary>
        internal static string TargetTextBoxPreValue;

        /// <summary>
        /// все баз данных в кучу здесь
        /// </summary>
        internal static Dictionary<string, string> AllDBmerged;// = new Dictionary<string, string>();

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

        ///// <summary>
        ///// Fileslist control object
        ///// </summary>
        //internal static object FilesList;

        /// <summary>
        /// Files list using now control
        /// </summary>
        internal static FilesListControlBase FilesListControl;

        /// <summary>
        /// Relates an entry index of the files list to the content that entry presents. The list holds
        /// one entry per file of the opened project, preceded by the "[ALL]" entry which presents all
        /// of them at once.
        /// </summary>
        internal static FilesListContent FilesListContent;

        /// <summary>
        /// Files list
        /// </summary>
        internal static ListBox THFilesList { get => Main.THFilesList; }
    }
}

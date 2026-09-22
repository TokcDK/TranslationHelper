using System;
using System.Collections.Generic;
using TranslationHelper.Models;
using TranslationHelper.Projects;
using TranslationHelper.Settings;
using TranslationHelper.Workspace;

namespace TranslationHelper.Data
{
    /// <summary>
    /// The application's own state: the window it runs in, the projects that are open, and the few
    /// values that belong to the session rather than to a project.
    /// <para>
    /// This used to be where the application kept <em>the</em> project and <em>the</em> files list.
    /// It keeps the projects now — a list and a selection — and everything that used to be read as
    /// "the" project is read through the selection, so a caller that was written for one project
    /// works for the one the user is looking at without being changed.
    /// </para>
    /// <para>
    /// What a project owns — its files, its files list, its grid, its text boxes — is deliberately not
    /// kept here any more. Those are reached through <see cref="ActiveWorkspace"/>, which is the
    /// selected project's controls.
    /// </para>
    /// </summary>
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

            // No project is open yet. The projects, their files and their controls are created as the
            // user opens them, so there is nothing to build here beyond the collection that will hold
            // them.
            ProjectsData = new ProjectsData();

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
        /// The projects open in this session, and which of them is being worked on. This is what the
        /// projects tab control is bound to.
        /// </summary>
        internal static ProjectsData ProjectsData;

        /// <summary>
        /// The workspace of the selected project: its files list, its opened files and their controls.
        /// <para>
        /// It is kept by the window that shows the projects, which is the one place that knows which
        /// project was selected, so it cannot point at a project that has been closed.
        /// </para>
        /// </summary>
        internal static IProjectWorkspace ActiveWorkspace;

        /// <summary>
        /// The project being opened right now, or null when no project is being opened.
        /// <para>
        /// A project is opened before it is added to <see cref="ProjectsData"/>, because a project
        /// that fails to parse must not appear as a tab. For the whole of that window it is not the
        /// selected project, yet it is the project the application is working on: the project's own
        /// <c>Init</c> and <c>Open</c> read their directories and their content through
        /// <see cref="CurrentProject"/>. Set by <see cref="Functions.FunctionsOpen"/> for the duration
        /// of an open and cleared when it ends, so it can never outlive one.
        /// </para>
        /// </summary>
        internal static ProjectBase OpeningProject;

        /// <summary>
        /// CurrentProject
        /// <para>
        /// The project being worked on. It used to be the only one there was; it is the selected one
        /// now, which is what makes every caller that asks for "the" project follow the user's choice
        /// instead of a value fixed when the first project was opened.
        /// </para>
        /// <para>
        /// While a project is being opened it is that project, because it is the one being worked on
        /// and it is not selectable yet — see <see cref="OpeningProject"/>. The order matters: a
        /// project being opened is the one being worked on even when another project is on screen.
        /// </para>
        /// </summary>
        internal static ProjectBase CurrentProject => OpeningProject ?? ProjectsData?.SelectedProject;

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
    }
}

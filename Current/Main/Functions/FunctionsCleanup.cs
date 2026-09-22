using NLog;
using System;
using System.Runtime;
using TranslationHelper.Functions;

namespace TranslationHelper.Data
{
    /// <summary>
    /// What has to be dropped before another project is opened.
    /// <para>
    /// It used to empty the application's single files list, grid and text boxes, because those were
    /// about to be filled with the new project. There is one of each per project now, so a project
    /// that is already open keeps its own: this clears the session-wide values and the shared menu
    /// strip only, and the new project's own workspace starts empty because it has just been built.
    /// </para>
    /// </summary>
    class FunctionsCleanup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static void THCleanupThings()
        {
            try
            {
                FunctionsSave.WriteRPGMakerMVStats();

                //Close other forms
                if (AppData.Main.search != null)
                {
                    if (!AppData.Main.search.IsDisposed)
                    {
                        AppData.Main.search.Close();
                        AppData.Main.search.Dispose();
                    }
                    AppData.Main.search = null;
                }

                // A project is about to be parsed, so the content that is about to be filled must not be
                // worked on yet. MarkProjectOpened is called again once the new project's content is
                // complete.
                ProjectReadiness.MarkProjectClosed();

                //Reset vars
                AppData.Main.Text = "Translation Helper";
                FunctionsUI.ControlsSwitchActivated = false;

                //Clean data
                AppData.AllDBmerged = null;
                AppData.SelectedProjectFilePath = string.Empty;

                //Reload regex rules
                FunctionRules.ReloadTranslationRegexRules();
                FunctionRules.ReloadCellFixesRegexRules();

                //memory cleaning thing.
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            catch (Exception ex)
            {
                //The cleanup has to continue whatever happens, but a failure must not be invisible.
                Logger.Error(ex, "Failed to clean up before opening a project");
            }
        }
    }
}

using NLog;
using System;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Functions
{
    internal static class FunctionAutoSave
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static Task StartAutoSave(System.Timers.Timer autoSaveTimer, Func<Task> autosaveAction, int timeout = 300)
        {
            if (timeout < 1 || autoSaveTimer == null || autosaveAction == null)
            {
                return Task.CompletedTask;
            }

            autoSaveTimer.Interval = timeout * 1000;
            autoSaveTimer.Elapsed += async (s, e) =>
            {
                //The handler is async void as far as the timer is concerned, so an escaping
                //exception would be unobservable; log it instead of losing it.
                try
                {
                    await autosaveAction();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Autosave failed");
                }
            };
            autoSaveTimer.AutoReset = true;
            autoSaveTimer.Start();

            return Task.CompletedTask;
        }

        internal static void StopAutoSave(System.Timers.Timer autoSaveTimer)
        {
            if (autoSaveTimer != null)
            {
                autoSaveTimer.Stop();
                autoSaveTimer.Dispose();
            }
        }

        internal static void RestartAutosave(System.Timers.Timer autoSaveTimer, Func<Task> autosave, int timeout = 300)
        {
            //A disposed System.Timers.Timer cannot be started again (Start() throws
            //ObjectDisposedException), so this path must only stop the timer, not dispose it.
            if (autoSaveTimer != null)
            {
                autoSaveTimer.Stop();
            }

            StartAutoSave(autoSaveTimer, autosave, timeout);
        }

        /// <summary>
        /// Save the database of the project a timer belongs to.
        /// <para>
        /// The project is a parameter rather than taken from the selection: every open project runs a
        /// timer of its own, so a save that asked for "the" project would write the database of
        /// whichever project the user happens to be looking at, and would leave the one the timer was
        /// started for unsaved.
        /// </para>
        /// </summary>
        internal static Task SaveDBByAutosave(ProjectBase project, object locker)
        {
            lock (locker)
            {
                var workspace = AppData.Main?.Workspace?.WorkspaceOf(project);
                if (workspace == null) return Task.CompletedTask;

                // The task is deliberately not awaited inside the lock: the save does not come back to
                // the UI thread, and a lock held across it would be held against whatever the UI thread
                // is doing at the same time — including the project's own save, which takes this lock.
                return Task.FromResult(FunctionsDBFile.SaveDB(workspace));
            }
        }
    }
}

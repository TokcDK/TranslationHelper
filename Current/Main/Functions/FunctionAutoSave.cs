using NLog;
using System;
using System.Threading.Tasks;
using TranslationHelper.Main.Functions;

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

        internal static Task SaveDBByAutosave(object locker)
        {
            lock (locker)
            {
                return Task.FromResult(FunctionsDBFile.SaveDB());
            }
        }
    }
}

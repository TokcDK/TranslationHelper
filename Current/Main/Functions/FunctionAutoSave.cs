using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    internal static class FunctionAutoSave
    {
        internal static Task StartAutoSave(ref System.Timers.Timer autoSaveTimer, Func<Task> autosaveAction, int timeout = 300)
        {
            if (timeout < 1 || autosaveAction == null)
            {
                return Task.CompletedTask;
            }

            autoSaveTimer = new System.Timers.Timer(timeout * 1000);
            autoSaveTimer.Elapsed += async (s, e) => await autosaveAction();
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

        internal static void RestartAutosave(ref System.Timers.Timer autoSaveTimer, Func<Task> autosave, int timeout = 300)
        {
            StopAutoSave(autoSaveTimer);
            StartAutoSave(ref autoSaveTimer, autosave, timeout);
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using INIFileMan;

namespace TranslationHelper.Settings
{
    /// <summary>
    /// Reads every setting from the application INI file and writes each change straight back.
    /// <para>
    /// The file stays plain <c>[Section]</c> and <c>Key=Value</c> text with a comment above every
    /// key, so it can be understood and edited by hand while the application is closed. Sections that
    /// are not settings — recent files, last opened paths — live in the same file and are left alone.
    /// </para>
    /// </summary>
    internal static class SettingsIniStore
    {
        /// <summary>
        /// Sections used before settings were grouped by feature. A value found there is adopted once
        /// and written into its new section; the old key is left untouched rather than deleted, so
        /// nothing a user has configured is thrown away by upgrading.
        /// </summary>
        private static readonly string[] LegacySections = { "General", "Optimizations", "Tools" };

        private static readonly object Gate = new object();

        private static INIFile _ini;
        private static bool _applying;
        private static int _suspended;

        /// <summary>The application INI file.</summary>
        internal static INIFile Ini
        {
            get
            {
                if (_ini != null)
                {
                    return _ini;
                }

                lock (Gate)
                {
                    if (_ini == null)
                    {
                        _ini = new INIFile(Application.ProductName + ".ini", forceCreate: true);
                    }
                }

                return _ini;
            }
        }

        /// <summary>
        /// Stops settings changed inside the returned scope from reaching the INI file.
        /// <para>
        /// Used when code overrides a setting for the duration of one operation: the override is an
        /// implementation detail of that operation, not a change the user asked for, so it must not
        /// survive a stop in the middle of it.
        /// </para>
        /// </summary>
        internal static IDisposable SuspendWriting()
        {
            Interlocked.Increment(ref _suspended);
            return new WriteSuspension();
        }

        internal static void Load(IReadOnlyList<Setting> settings)
        {
            _applying = true;

            try
            {
                foreach (var setting in settings)
                {
                    setting.SetValueFromString(ReadStoredValue(setting));
                }

                foreach (var setting in settings)
                {
                    setting.Changed += OnSettingChanged;
                }

                EnsureFileHasEverySetting(settings);
            }
            finally
            {
                _applying = false;
            }
        }

        /// <summary>
        /// The value to start from: this section, else one of the pre-refactor sections, else the
        /// setting's default.
        /// </summary>
        private static string ReadStoredValue(Setting setting)
        {
            if (Ini.KeyExists(setting.Key, setting.Section))
            {
                return Ini.GetKey(setting.Section, setting.Key);
            }

            foreach (var legacySection in LegacySections)
            {
                if (Ini.KeyExists(setting.Key, legacySection))
                {
                    return Ini.GetKey(legacySection, setting.Key);
                }
            }

            return setting.DefaultAsString;
        }

        /// <summary>
        /// Writes any setting the file does not have yet, together with its comment, so a first run
        /// leaves behind a complete and self-describing INI file rather than one that fills in
        /// key by key as the user happens to touch things.
        /// </summary>
        private static void EnsureFileHasEverySetting(IReadOnlyList<Setting> settings)
        {
            var added = false;

            foreach (var setting in settings)
            {
                if (Ini.KeyExists(setting.Key, setting.Section))
                {
                    continue;
                }

                Put(setting);
                added = true;
            }

            if (added)
            {
                Ini.WriteFile();
            }
        }

        private static void OnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (_applying || Volatile.Read(ref _suspended) > 0)
            {
                return;
            }

            lock (Gate)
            {
                Put(e.Setting);
                Ini.WriteFile();
            }
        }

        /// <summary>Stages one setting into the INI data without touching the disk.</summary>
        private static void Put(Setting setting)
        {
            var ini = Ini;

            // The key has to exist before the comment, because the comment is attached to the key.
            ini.SetKey(setting.Section, setting.Key, setting.ValueAsString, false);
            ini.SetComment(setting.Section, setting.Key, setting.Description, false);
        }

        private sealed class WriteSuspension : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                Interlocked.Decrement(ref _suspended);
            }
        }
    }
}

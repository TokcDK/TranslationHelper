using System;
using System.Collections.Generic;
using GetListOfSubClasses;

namespace TranslationHelper.Settings
{
    /// <summary>
    /// One section of the settings form: an ordered group of settings that belong to the same
    /// feature.
    /// </summary>
    internal sealed class SettingsSection
    {
        internal SettingsSection(string name, int order, IReadOnlyList<Setting> settings)
        {
            Name = name;
            Order = order;
            Settings = settings;
        }

        internal string Name { get; }

        internal int Order { get; }

        internal IReadOnlyList<Setting> Settings { get; }
    }

    /// <summary>
    /// Finds every setting in the application and hands out typed access to it.
    /// <para>
    /// Settings are declared by the feature they belong to — translation settings next to the
    /// translation code, search settings next to the search code — and this registry reads all of
    /// those implementations. A feature can therefore gain a setting by adding one class, without
    /// this file, the INI store or the settings form changing at all.
    /// </para>
    /// </summary>
    internal static class SettingsRegistry
    {
        private static readonly object Gate = new object();

        private static Dictionary<Type, Setting> _byType;
        private static List<SettingsSection> _sections;
        private static volatile bool _loaded;

        /// <summary>All settings, grouped by section, both in display order.</summary>
        internal static IReadOnlyList<SettingsSection> Sections
        {
            get
            {
                EnsureLoaded();
                return _sections;
            }
        }

        /// <summary>
        /// The one instance of a setting. Loading happens on first use, so a setting can be read
        /// before the application has finished starting up.
        /// </summary>
        internal static T Get<T>() where T : Setting
        {
            EnsureLoaded();
            return (T)_byType[typeof(T)];
        }

        internal static void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            lock (Gate)
            {
                if (_loaded)
                {
                    return;
                }

                Discover();
                SettingsIniStore.Load(Flatten());
                _loaded = true;
            }
        }

        private static void Discover()
        {
            var found = Inherited.GetListOfinheritedSubClasses<Setting>();

            _byType = new Dictionary<Type, Setting>(found.Count);
            foreach (var setting in found)
            {
                _byType[setting.GetType()] = setting;
            }

            found.Sort(CompareSettings);

            var sections = new List<SettingsSection>();
            var currentName = (string)null;
            var currentOrder = 0;
            List<Setting> current = null;

            foreach (var setting in found)
            {
                if (current == null || !string.Equals(currentName, setting.Section, StringComparison.Ordinal))
                {
                    currentName = setting.Section;
                    currentOrder = setting.SectionOrder;
                    current = new List<Setting>();
                    sections.Add(new SettingsSection(currentName, currentOrder, current));
                }

                current.Add(setting);
            }

            sections.Sort((left, right) => left.Order.CompareTo(right.Order));
            _sections = sections;
        }

        /// <summary>
        /// Reflection does not promise an order, so every setting carries its own position. Sorting
        /// by section, then by position, then by key keeps the form and the INI file stable between
        /// runs and between machines.
        /// </summary>
        private static int CompareSettings(Setting left, Setting right)
        {
            var bySectionOrder = left.SectionOrder.CompareTo(right.SectionOrder);
            if (bySectionOrder != 0)
            {
                return bySectionOrder;
            }

            var bySectionName = string.CompareOrdinal(left.Section, right.Section);
            if (bySectionName != 0)
            {
                return bySectionName;
            }

            var byOrder = left.Order.CompareTo(right.Order);
            if (byOrder != 0)
            {
                return byOrder;
            }

            return string.CompareOrdinal(left.Key, right.Key);
        }

        private static List<Setting> Flatten()
        {
            var all = new List<Setting>(_byType.Count);
            foreach (var section in _sections)
            {
                all.AddRange(section.Settings);
            }

            return all;
        }
    }
}

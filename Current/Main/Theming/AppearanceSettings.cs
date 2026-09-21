using System.Collections.Generic;
using TranslationHelper.Settings;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Settings about how the application looks.
    /// <para>
    /// Declared here, beside the theming code that reads them, in the same way that the search
    /// settings live beside the search code. The settings form finds this section on its own, so
    /// adding the theme to the application was adding this file and nothing in the settings window.
    /// </para>
    /// </summary>
    internal static class AppearanceSettings
    {
        internal const string SectionName = "Appearance";

        /// <summary>
        /// Placed before every other section: the theme applies to the whole application rather than
        /// to one feature, so it is the first thing the settings window shows.
        /// </summary>
        internal const int SectionPosition = 5;

        /// <summary>
        /// The theme the windows and menus are painted with.
        /// <para>
        /// A choice rather than a yes/no, because a theme is a name that selects a palette — that is
        /// what lets a third theme be added without a second setting, and what keeps the stored value
        /// readable when the INI file is opened by hand.
        /// </para>
        /// </summary>
        internal sealed class Theme : ChoiceSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "Theme";

            internal override int Order => 10;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Theme";

            internal override string Description =>
                "Colour scheme of the windows and menus. \"Dark\" paints them on dark surfaces; \"Light\" is the system colour scheme.";

            internal override IReadOnlyList<string> Choices => ThemeManager.Instance.AvailableThemes;

            internal override string Default => ThemeManager.LightName;
        }
    }
}

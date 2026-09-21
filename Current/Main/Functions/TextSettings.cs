using TranslationHelper.Settings;

namespace TranslationHelper.Functions
{
    /// <summary>
    /// Settings about the text itself: how long a line may be, and which strings are worth
    /// translating at all.
    /// <para>
    /// They are read by <c>SplitLongLines</c>, <c>CheckAnyLineLngerOfMaxLength</c> and
    /// <c>FunctionsRomajiKana</c>, so they are declared here rather than beside the settings form.
    /// </para>
    /// </summary>
    internal static class TextSettings
    {
        internal const string SectionName = "Text";

        internal const int SectionPosition = 40;

        /// <summary>Length above which the line splitting functions break a line.</summary>
        internal sealed class LineCharLimit : IntSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "LineCharLimit";

            internal override int Order => 10;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "char limit of line length (for line split functions)";

            internal override string Description =>
                "Lines longer than this are split by the line split functions.";

            internal override int Default => 60;

            internal override int Min => 1;

            internal override int Max => 9999;
        }

        /// <summary>Skip strings that are mostly romaji, because they usually need no translation.</summary>
        internal sealed class DontLoadStringIfRomajiPercent : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DontLoadStringIfRomajiPercent";

            internal override int Order => 20;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Do not load string if it has more of next romaji percent -";

            internal override string Description =>
                "String will not be loaded for translation if this string contains romaji characters in text more of specified percent.";

            internal override bool Default => true;
        }

        /// <summary>Percent of romaji above which a string is skipped.</summary>
        internal sealed class DontLoadStringIfRomajiPercentNumber : IntSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DontLoadStringIfRomajiPercentNumber";

            internal override int Order => 30;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Romaji percent:";

            internal override string Description =>
                "Percent of romaji characters above which a string is not loaded for translation.";

            internal override int Default => 90;

            internal override int Min => 0;

            internal override int Max => 100;
        }

        /// <summary>Apply the romaji filter while a project is being opened.</summary>
        internal sealed class DontLoadStringIfRomajiPercentForOpen : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DontLoadStringIfRomajiPercentForOpen";

            internal override int Order => 40;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Apply while opening";

            internal override string Description =>
                "Is true while opening. Always true for RPGMaker MV files or jsons for strings filtering purposes.";

            internal override bool Default => true;
        }

        /// <summary>Apply the romaji filter while a project is being translated.</summary>
        internal sealed class DontLoadStringIfRomajiPercentForTranslation : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DontLoadStringIfRomajiPercentForTranslation";

            internal override int Order => 50;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Apply while translating";

            internal override string Description =>
                "Is true while online translating. Will be used both with other chars like .!/? and other same";

            internal override bool Default => true;
        }
    }
}

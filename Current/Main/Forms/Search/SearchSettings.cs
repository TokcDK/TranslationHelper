using TranslationHelper.Settings;

namespace TranslationHelper.Forms.Search
{
    /// <summary>
    /// Settings of the row issue search.
    /// <para>
    /// Each one switches a single checker in
    /// <c>Functions.FileElementsFunctions.Row.SearchRowIssueCheckers</c> on or off, so they are
    /// declared here, beside the search feature, instead of beside the settings form.
    /// </para>
    /// </summary>
    internal static class SearchSettings
    {
        internal const string SectionName = "Search";

        internal const int SectionPosition = 30;

        /// <summary>Report lines that still contain characters of the source script.</summary>
        internal sealed class CheckNonRomaji : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "SearchRowIssueOptionsCheckNonRomaji";

            internal override int Order => 10;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Check non romaji exist";

            internal override string Description =>
                "Report lines whose translation still contains characters that look untranslated.";

            internal override bool Default => true;
        }

        /// <summary>Report rows where one actor name is translated in more than one way.</summary>
        internal sealed class CheckActors : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "SearchRowIssueOptionsCheckActors";

            internal override int Order => 20;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Check Actor names";

            internal override string Description =>
                "Report rows where the name of the same actor is translated in more than one way.";

            internal override bool Default => true;
        }

        /// <summary>Report lines that still have something left to translate.</summary>
        internal sealed class CheckAnyLineTranslatable : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "SearchRowIssueOptionsCheckAnyLineTranslatable";

            internal override int Order => 30;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Check if any line can be translated";

            internal override string Description =>
                "Report lines that are not translated yet and that can still be translated.";

            internal override bool Default => true;
        }

        /// <summary>Report issues that only the currently opened project type can detect.</summary>
        internal sealed class CheckProjectSpecific : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "SearchRowIssueOptionsCheckProjectSpecific";

            internal override int Order => 40;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Check project specific issues";

            internal override string Description =>
                "Report issues that are specific to the type of the project that is opened.";

            internal override bool Default => true;
        }
    }
}

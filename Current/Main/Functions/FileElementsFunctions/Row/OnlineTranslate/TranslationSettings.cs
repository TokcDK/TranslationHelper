using System.Collections.Generic;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate.OnlineTranslators;
using TranslationHelper.Settings;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    /// <summary>
    /// Settings of the online translation feature.
    /// <para>
    /// They live next to the translation code rather than next to the settings form, so the feature
    /// owns its own configuration: adding a translation option means adding one class here, and
    /// neither the registry, the INI store nor the settings form has to change.
    /// </para>
    /// </summary>
    internal static class TranslationSettings
    {
        internal const string SectionName = "Translation";

        internal const int SectionPosition = 10;

        /// <summary>Language the opened project is translated from.</summary>
        internal sealed class SourceLanguage : ChoiceSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "SourceLanguage";

            internal override int Order => 10;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Source Language:";

            internal override string Description => "Language the opened project is translated from.";

            internal override string Default => "Japanese ja";

            internal override IReadOnlyList<string> Choices => TranslatorsBase.SourceLanguages;
        }

        /// <summary>Language the opened project is translated to.</summary>
        internal sealed class TargetLanguage : ChoiceSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "TargetLanguage";

            internal override int Order => 20;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Target Language:";

            internal override string Description => "Language the opened project is translated to.";

            internal override string Default => "English en";

            internal override IReadOnlyList<string> Choices => TranslatorsBase.TargetLanguages;
        }

        /// <summary>Web service opened by F12 with the selected table cells inserted into the link.</summary>
        internal sealed class WebTranslationLink : SuggestingStringSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "WebTranslationLink";

            internal override int Order => 30;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Web service link for manual translation (F12):";

            internal override string Description =>
                "Web site which wil be opened by pressing F12 key with added selected table cells values. Can be any here Google, Yandex, DeepL or other.";

            internal override string Default =>
                "https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}";

            internal override IReadOnlyList<string> Suggestions => new[]
            {
                "https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}",
                "https://www.deepl.com/ru/translator#{from}/{to}/{text}",
                "https://translate.yandex.com/?lang={from}-{to}&text={text}",
            };
        }

        /// <summary>Reuse of earlier online translation results instead of contacting the service again.</summary>
        internal sealed class EnableTranslationCache : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "EnableTranslationCache";

            internal override int Order => 40;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Enable online translation cache.";

            internal override string Description =>
                "Will save online translation result in cache db to use it in next time for same values instead of attemp to connect to service.";

            internal override bool Default => true;
        }

        /// <summary>Translate the cells that are almost identical to an already translated one.</summary>
        internal sealed class AutotranslationForSimular : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "AutotranslationForSimular";

            internal override int Order => 50;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Autotranslation for simular";

            internal override string Description =>
                "Automatically will be translated all almost identical cells with same original.";

            internal override bool Default => true;
        }

        /// <summary>
        /// Whether a line whose translation equals its original counts as translated. Rows that are
        /// already correct are skipped by the translation functions when this is on.
        /// </summary>
        internal sealed class IgnoreOrigEqualTransLines : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "IgnoreOrigEqualTransLines";

            internal override int Order => 60;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Ignore Original=Translation lines";

            internal override string Description =>
                "A line whose translation is equal to its original is treated as already translated and is skipped by the translation functions.";

            internal override bool Default => true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.OnlineTranslators;


//infos for read 
//https://qna.habr.com/q/366420
//https://stackoverflow.com/questions/16642196/get-html-code-from-website-in-c-sharp
//https://www.c-sharpcorner.com/blogs/difference-between-syatemnetwebrequest-and-httpclient-and-webclient-in-c-sharp
//https://html-agility-pack.net/knowledge-base/33679834/is-there-anyway-to-use--browsersession--to-download-files--csharp
namespace TranslationHelper.Translators
{
    internal static class TranslatorsTools
    {
        /// <summary>
        /// Extracts the source language ID from the application settings.
        /// </summary>
        internal static string GetSourceLanguageID() =>
            AppSettings.OnlineTranslationSourceLanguage.Split(' ')[1];

        /// <summary>
        /// Extracts the target language ID from the application settings.
        /// </summary>
        internal static string GetTargetLanguageID() =>
            AppSettings.OnlineTranslationTargetLanguage.Split(' ')[1];
    }


    /// <summary>
    /// Base class for translators. Provides helper methods for language resolution,
    /// caching of translation results, and proper resource disposal.
    /// </summary>
    abstract class TranslatorsBase : IDisposable
    {
        // Web error counters.
        internal int ErrorsWebCnt = 0;
        internal int ErrorsWebCntOverall = 0;

        // Shared session cache for translations.
        internal static readonly Dictionary<string, string> sessionCache = new Dictionary<string, string>(1);

        // Web client and browser objects for translation HTTP requests and HTML parsing.
        protected WebClientEx webClient;
        protected WebBrowser webBrowser;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorsBase"/> class.
        /// Ensures that online translator cookies are set and a web client is available.
        /// </summary>
        protected TranslatorsBase()
        {
            EnsureCookiesInitialized();
            if (webClient == null)
            {
                webClient = new WebClientEx(AppData.OnlineTranslatorCookies);
            }
        }

        /// <summary>
        /// Resets the translator's session translation cache.
        /// </summary>
        internal void ResetCache() => sessionCache.Clear();

        /// <summary>
        /// Invoked when the translator is closed.
        /// </summary>
        internal void OnTranslatorClosed() { }

        /// <summary>
        /// Translates a single string using source and target languages defined in the settings.
        /// </summary>
        /// <param name="originalText">The text to translate.</param>
        /// <returns>The translated text or an empty string if an error occurs.</returns>
        public virtual string Translate(string originalText)
        {
            try
            {
                string sourceLanguage = GetLanguageFromSetting(AppSettings.OnlineTranslationSourceLanguage, "ja");
                string targetLanguage = GetLanguageFromSetting(AppSettings.OnlineTranslationTargetLanguage, "en");

                // If the original text is Japanese but mostly in romaji or another language, return it unchanged.
                if (originalText.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                {
                    return originalText;
                }
                return Translate(originalText, sourceLanguage, targetLanguage);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Translates an array of strings using source and target languages defined in the settings.
        /// </summary>
        /// <param name="originalTexts">An array of texts to translate.</param>
        /// <returns>The array of translated texts or null if an error occurs.</returns>
        public virtual string[] Translate(string[] originalTexts)
        {
            try
            {
                string sourceLanguage = GetLanguageFromSetting(AppSettings.OnlineTranslationSourceLanguage, "ja");
                string targetLanguage = GetLanguageFromSetting(AppSettings.OnlineTranslationTargetLanguage, "en");
                return Translate(originalTexts, sourceLanguage, targetLanguage);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Translates a single string from a specified source to target language.
        /// </summary>
        /// <param name="originalText">The text to translate.</param>
        /// <param name="languageFrom">The source language code.</param>
        /// <param name="languageTo">The target language code.</param>
        /// <returns>The translated text.</returns>
        public abstract string Translate(string originalText, string languageFrom = "auto", string languageTo = "en");

        /// <summary>
        /// Translates an array of strings from a specified source to target language.
        /// </summary>
        /// <param name="originalTexts">An array of texts to translate.</param>
        /// <param name="languageFrom">The source language code.</param>
        /// <param name="languageTo">The target language code.</param>
        /// <returns>An array of translated texts.</returns>
        public abstract string[] Translate(string[] originalTexts, string languageFrom = "auto", string languageTo = "en");

        /// <summary>
        /// Extracts the language code from a settings string by returning its last token.
        /// </summary>
        /// <param name="setting">The settings string containing the language.</param>
        /// <param name="defaultLanguage">The default language if none is found.</param>
        /// <returns>The extracted language code.</returns>
        private string GetLanguageFromSetting(string setting, string defaultLanguage)
        {
            if (!string.IsNullOrWhiteSpace(setting))
            {
                var tokens = setting.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    return tokens.Last();
                }
            }
            return defaultLanguage;
        }

        /// <summary>
        /// Ensures that the online translator cookies are initialized.
        /// </summary>
        private void EnsureCookiesInitialized()
        {
            if (AppData.OnlineTranslatorCookies == null)
            {
                AppData.OnlineTranslatorCookies = new System.Net.CookieContainer();
            }
        }

        /// <summary>
        /// Searches the given HTML document for the translation text.
        /// </summary>
        /// <param name="htmlDocument">The HTML document returned from the translation service.</param>
        /// <returns>The inner text of the element that contains the translation; otherwise, an empty string.</returns>
        protected static string GetTranslationHtmlElement(HtmlDocument htmlDocument)
        {
            // First, try to find a div with a known translation container class.
            foreach (HtmlElement element in htmlDocument.GetElementsByTagName("div"))
            {
                string className = element.GetAttribute("className");
                if (className == "result-container" || className == "t0")
                {
                    return element.InnerText;
                }
            }

            // Fallback: iterate through body children and return the first element whose inner HTML does not start with '<'.
            foreach (HtmlElement element in htmlDocument.Body.Children)
            {
                if (!string.IsNullOrEmpty(element.InnerText) && !element.InnerHtml.StartsWith("<"))
                {
                    return element.InnerText;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                webClient?.Dispose();
                webClient = null;
            }
            catch { }

            try
            {
                webBrowser?.Dispose();
                webBrowser = null;
            }
            catch { }
        }

        #region Available Languages

        // Source languages available for translation.
        protected static readonly List<string> _sourceLanguages = new List<string>
    {
        "Afrikaans af",
        "Albanian sq",
        "Amharic am",
        "Arabic ar",
        "Armenian hy",
        "Azeerbaijani az",
        "Basque eu",
        "Belarusian be",
        "Bengali bn",
        "Bosnian bs",
        "Bulgarian bg",
        "Catalan ca",
        "Cebuano ceb",
        "Chinese (Simplified) zh-CN",
        "Chinese (Traditional) zh-TW",
        "Corsican co",
        "Croatian hr",
        "Czech cs",
        "Danish da",
        "Dutch nl",
        "English en",
        "Esperanto eo",
        "Estonian et",
        "Finnish fi",
        "French fr",
        "Frisian fy",
        "Galician gl",
        "Georgian ka",
        "German de",
        "Greek el",
        "Gujarati gu",
        "Haitian Creole ht",
        "Hausa ha",
        "Hawaiian haw",
        "Hebrew iw",
        "Hindi hi",
        "Hmong hmn",
        "Hungarian hu",
        "Icelandic is",
        "Igbo ig",
        "Indonesian id",
        "Irish ga",
        "Italian it",
        "Japanese ja",
        "Javanese jw",
        "Kannada kn",
        "Kazakh kk",
        "Khmer km",
        "Korean ko",
        "Kurdish ku",
        "Kyrgyz ky",
        "Lao lo",
        "Latin la",
        "Latvian lv",
        "Lithuanian lt",
        "Luxembourgish lb",
        "Macedonian mk",
        "Malagasy mg",
        "Malay ms",
        "Malayalam ml",
        "Maori mi",
        "Marathi mr",
        "Mongolian mn",
        "Myanmar (Burmese) my",
        "Nepali ne",
        "Norwegian no",
        "Nyanja (Chichewa) ny",
        "Pashto ps",
        "Persian fa",
        "Polish pl",
        "Portuguese pt",
        "Punjabi ma",
        "Romanian ro",
        "Russian ru",
        "Samoan sm",
        "Scots Gaelic gd",
        "Serbian sr",
        "Sesotho st",
        "Shona sn",
        "Sindhi sd",
        "Sinhala (Sinhalese) si",
        "Slovak sk",
        "Slovenian sl",
        "Somali so",
        "Spanish es",
        "Sundanese su",
        "Swahili sw",
        "Swedish sv",
        "Tagalog (Filipino) tl",
        "Tajik tg",
        "Tamil ta",
        "Telugu te",
        "Thai th",
        "Turkish tr",
        "Ukrainian uk",
        "Urdu ur",
        "Uzbek uz",
        "Vietnamese vi",
        "Welsh cy",
        "Xhosa xh",
        "Yiddish yi",
        "Yoruba yo",
        "Zulu zu"
    };

        /// <summary>
        /// Gets the list of available source languages.
        /// </summary>
        public static List<string> SourceLanguages => _sourceLanguages;

        // Target languages available for translation.
        private static readonly List<string> _targetLanguages = new List<string>
    {
        "Afrikaans af",
        "Albanian sq",
        "Amharic am",
        "Arabic ar",
        "Armenian hy",
        "Azeerbaijani az",
        "Basque eu",
        "Belarusian be",
        "Bengali bn",
        "Bosnian bs",
        "Bulgarian bg",
        "Catalan ca",
        "Cebuano ceb",
        "Chinese (Simplified) zh-CN",
        "Chinese (Traditional) zh-TW",
        "Corsican co",
        "Croatian hr",
        "Czech cs",
        "Danish da",
        "Dutch nl",
        "English en",
        "Esperanto eo",
        "Estonian et",
        "Finnish fi",
        "French fr",
        "Frisian fy",
        "Galician gl",
        "Georgian ka",
        "German de",
        "Greek el",
        "Gujarati gu",
        "Haitian Creole ht",
        "Hausa ha",
        "Hawaiian haw",
        "Hebrew iw",
        "Hindi hi",
        "Hmong hmn",
        "Hungarian hu",
        "Icelandic is",
        "Igbo ig",
        "Indonesian id",
        "Irish ga",
        "Italian it",
        "Japanese ja",
        "Javanese jw",
        "Kannada kn",
        "Kazakh kk",
        "Khmer km",
        "Korean ko",
        "Kurdish ku",
        "Kyrgyz ky",
        "Lao lo",
        "Latin la",
        "Latvian lv",
        "Lithuanian lt",
        "Luxembourgish lb",
        "Macedonian mk",
        "Malagasy mg",
        "Malay ms",
        "Malayalam ml",
        "Maori mi",
        "Marathi mr",
        "Mongolian mn",
        "Myanmar (Burmese) my",
        "Nepali ne",
        "Norwegian no",
        "Nyanja (Chichewa) ny",
        "Pashto ps",
        "Persian fa",
        "Polish pl",
        "Portuguese pt",
        "Punjabi ma",
        "Romanian ro",
        "Russian ru",
        "Samoan sm",
        "Scots Gaelic gd",
        "Serbian sr",
        "Sesotho st",
        "Shona sn",
        "Sindhi sd",
        "Sinhala (Sinhalese) si",
        "Slovak sk",
        "Slovenian sl",
        "Somali so",
        "Spanish es",
        "Sundanese su",
        "Swahili sw",
        "Swedish sv",
        "Tagalog (Filipino) tl",
        "Tajik tg",
        "Tamil ta",
        "Telugu te",
        "Thai th",
        "Turkish tr",
        "Ukrainian uk",
        "Urdu ur",
        "Uzbek uz",
        "Vietnamese vi",
        "Welsh cy",
        "Xhosa xh",
        "Yiddish yi",
        "Yoruba yo",
        "Zulu zu"
    };

        /// <summary>
        /// Gets the list of available target languages.
        /// </summary>
        public static List<string> TargetLanguages => _targetLanguages;

        #endregion
    }

    /// <summary>
    /// Provides extension methods for fixing common HTML formatting issues in translated text.
    /// </summary>
    static class TranslatorsExtensions
    {
        /// <summary>
        /// Fixes common HTML tag formatting issues.
        /// </summary>
        /// <param name="input">The input string containing HTML.</param>
        /// <returns>The string with fixed HTML tags.</returns>
        internal static string FixHTMLTags(this string input) =>
            input.Replace("</ p> </ font>", " </p></font>")
                 .Replace("</ p>", "</p>")
                 .Replace("</ font>", "</font>")
                 .Replace("<p align = ", "<p align=")
                 .Replace("<img src = ", "<img src=")
                 .Replace("<font size = ", "<font size=")
                 .Replace("<font face = ", "<font face=");

        /// <summary>
        /// Applies HTML tag fixes and replaces a specific splitter with a newline.
        /// </summary>
        /// <param name="input">The input string to fix.</param>
        /// <returns>The fixed string.</returns>
        internal static string FixFormat(this string input) =>
            input.FixHTMLTags().Replace(" </br> ", Environment.NewLine);

        /// <summary>
        /// Applies only the HTML tag fixes (used for multi-line translations).
        /// </summary>
        /// <param name="input">The input string to fix.</param>
        /// <returns>The fixed string.</returns>
        internal static string FixFormatMulti(this string input) =>
            input.FixHTMLTags();
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// Assumes the setting is space-separated, returning the last token.
        /// </summary>
        internal static string GetSourceLanguageID() =>
            AppSettings.OnlineTranslationSourceLanguage.Split(' ').Last();

        /// <summary>
        /// Extracts the target language ID from the application settings.
        /// Assumes the setting is space-separated, returning the last token.
        /// </summary>
        internal static string GetTargetLanguageID() =>
            AppSettings.OnlineTranslationTargetLanguage.Split(' ').Last();
    }

    /// <summary>
    /// Base class for translation implementations. Handles language resolution,
    /// session caching, and resource management.
    /// </summary>
    abstract class TranslatorsBase : IDisposable
    {
        // Web error counters to track failures.
        internal int ErrorsWebCnt = 0;
        internal int ErrorsWebCntOverall = 0;

        // Shared cache for storing translations within the session.
        internal static readonly Dictionary<string, string> sessionCache = new Dictionary<string, string>(1);

        // Web client and browser for HTTP requests and HTML parsing.
        protected WebClientEx webClient;
        protected WebBrowser webBrowser;

        /// <summary>
        /// Initializes a new instance with cookie and web client setup.
        /// </summary>
        protected TranslatorsBase()
        {
            if (AppData.OnlineTranslatorCookies == null)
            {
                AppData.OnlineTranslatorCookies = new CookieContainer();
            }
            webClient = new WebClientEx(AppData.OnlineTranslatorCookies);
        }

        /// <summary>
        /// Clears the session translation cache.
        /// </summary>
        internal void ResetCache() => sessionCache.Clear();

        /// <summary>
        /// Placeholder method invoked when the translator is closed.
        /// </summary>
        internal void OnTranslatorClosed() { }

        /// <summary>
        /// Translates a single string using languages from app settings.
        /// </summary>
        /// <param name="originalText">Text to translate.</param>
        /// <returns>Translated text or empty string on error.</returns>
        public virtual string Translate(string originalText)
        {
            try
            {
                string sourceLanguage = GetLanguageFromSetting(AppSettings.OnlineTranslationSourceLanguage, "ja");
                string targetLanguage = GetLanguageFromSetting(AppSettings.OnlineTranslationTargetLanguage, "en");

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
        /// Translates an array of strings using languages from app settings.
        /// </summary>
        /// <param name="originalTexts">Texts to translate.</param>
        /// <returns>Translated texts or null on error.</returns>
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
        /// Extracts the language code from a setting string, taking the last token.
        /// </summary>
        /// <param name="setting">Setting string (e.g., "Japanese ja").</param>
        /// <param name="defaultLanguage">Fallback language if setting is invalid.</param>
        protected string GetLanguageFromSetting(string setting, string defaultLanguage)
        {
            if (!string.IsNullOrWhiteSpace(setting))
            {
                var tokens = setting.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return tokens.Length > 0 ? tokens.Last() : defaultLanguage;
            }
            return defaultLanguage;
        }

        /// <summary>
        /// Extracts the translation text from an HTML document.
        /// </summary>
        /// <param name="htmlDocument">HTML document from the translation service.</param>
        protected static string GetTranslationHtmlElement(HtmlDocument htmlDocument)
        {
            var possibleClasses = new[] { "result-container", "t0" };
            foreach (var className in possibleClasses)
            {
                var elements = htmlDocument.GetElementsByTagName("div")
                    .Cast<HtmlElement>()
                    .Where(e => e.GetAttribute("className") == className);
                if (elements.Any())
                {
                    return elements.First().InnerText;
                }
            }

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
        /// Disposes of web client and browser resources.
        /// </summary>
        public void Dispose()
        {
            try { webClient?.Dispose(); webClient = null; } catch { }
            try { webBrowser?.Dispose(); webBrowser = null; } catch { }
        }

        #region Language Lists

        private static readonly List<string> _languages = new List<string>
    {
        "Afrikaans af", "Albanian sq", "Amharic am", "Arabic ar", "Armenian hy", "Azeerbaijani az",
        "Basque eu", "Belarusian be", "Bengali bn", "Bosnian bs", "Bulgarian bg", "Catalan ca",
        "Cebuano ceb", "Chinese (Simplified) zh-CN", "Chinese (Traditional) zh-TW", "Corsican co",
        "Croatian hr", "Czech cs", "Danish da", "Dutch nl", "English en", "Esperanto eo",
        "Estonian et", "Finnish fi", "French fr", "Frisian fy", "Galician gl", "Georgian ka",
        "German de", "Greek el", "Gujarati gu", "Haitian Creole ht", "Hausa ha", "Hawaiian haw",
        "Hebrew iw", "Hindi hi", "Hmong hmn", "Hungarian hu", "Icelandic is", "Igbo ig",
        "Indonesian id", "Irish ga", "Italian it", "Japanese ja", "Javanese jw", "Kannada kn",
        "Kazakh kk", "Khmer km", "Korean ko", "Kurdish ku", "Kyrgyz ky", "Lao lo", "Latin la",
        "Latvian lv", "Lithuanian lt", "Luxembourgish lb", "Macedonian mk", "Malagasy mg",
        "Malay ms", "Malayalam ml", "Maori mi", "Marathi mr", "Mongolian mn", "Myanmar (Burmese) my",
        "Nepali ne", "Norwegian no", "Nyanja (Chichewa) ny", "Pashto ps", "Persian fa", "Polish pl",
        "Portuguese pt", "Punjabi ma", "Romanian ro", "Russian ru", "Samoan sm", "Scots Gaelic gd",
        "Serbian sr", "Sesotho st", "Shona sn", "Sindhi sd", "Sinhala (Sinhalese) si", "Slovak sk",
        "Slovenian sl", "Somali so", "Spanish es", "Sundanese su", "Swahili sw", "Swedish sv",
        "Tagalog (Filipino) tl", "Tajik tg", "Tamil ta", "Telugu te", "Thai th", "Turkish tr",
        "Ukrainian uk", "Urdu ur", "Uzbek uz", "Vietnamese vi", "Welsh cy", "Xhosa xh",
        "Yiddish yi", "Yoruba yo", "Zulu zu"
    };

        public static List<string> SourceLanguages => _languages;
        public static List<string> TargetLanguages => _languages;

        #endregion
    }

    /// <summary>
    /// Extension methods for fixing HTML formatting in translated text.
    /// </summary>
    static class TranslatorsExtensions
    {
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

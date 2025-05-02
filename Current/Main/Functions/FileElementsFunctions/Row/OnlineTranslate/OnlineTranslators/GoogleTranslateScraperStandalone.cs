using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate.OnlineTranslators
{
    public abstract class TranslatorBase
    {
        /// <summary>
        /// Name of the translator.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Translates a single string from source to target language.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sourceLang"></param>
        /// <param name="targetLang"></param>
        /// <returns></returns>
        public abstract Task<string> TranslateAsync(string text, string sourceLang = "auto", string targetLang = "en");

        /// <summary>
        /// Translates an array of strings, optimizing by batching requests.
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="sourceLang"></param>
        /// <param name="targetLang"></param>
        /// <returns></returns>
        public abstract Task<string[]> TranslateBatchAsync(string[] texts, string sourceLang = "auto", string targetLang = "en");

        /// <summary>
        /// List of supported source languages.
        /// </summary>
        public abstract List<string> SourceLanguages { get; }

        /// <summary>
        /// List of supported target languages.
        /// </summary>
        public abstract List<string> TargetLanguages { get; }
    }


    public class GoogleTranslateScraperStandalone : TranslatorBase
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, string> _cache;
        private const string TranslateUrl = "https://translate.google.com/translate_a/single";
        private const string ClientValue = "gtx";
        private readonly string TargetLanguageId;

        public override string Name => "Google translate";

        public GoogleTranslateScraperStandalone()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };
            _httpClient = new HttpClient(handler);
            _cache = new Dictionary<string, string>();
            TargetLanguageId = TranslatorsTools.GetTargetLanguageID();
        }

        /// <summary>
        /// Translates a single string from source to target language using Google Translate scraping.
        /// </summary>
        /// <param name="text">The text to translate. Cannot be null or empty.</param>
        /// <param name="sourceLang">Source language code. Use "auto" for auto-detect. Default is "auto".</param>
        /// <param name="targetLang">Target language code. Default is "en".</param>
        /// <returns>Translated text.</returns>
        public override async Task<string> TranslateAsync(string text, string sourceLang = "auto", string targetLang = "en")
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));

            if (!string.IsNullOrWhiteSpace(TargetLanguageId))
            {
                targetLang = TargetLanguageId;
            }

            string cacheKey = CreateCacheKey(text, sourceLang, targetLang);
            if (_cache.TryGetValue(cacheKey, out string value))
                return value;

            string translatedText = await TranslateWithoutCacheAsync(text, sourceLang, targetLang);
            _cache[cacheKey] = translatedText;
            return translatedText;
        }

        /// <summary>
        /// Translates an array of strings, optimizing by batching requests.
        /// </summary>
        /// <param name="texts">Array of texts to translate.</param>
        /// <param name="sourceLang">Source language code.</param>
        /// <param name="targetLang">Target language code.</param>
        /// <returns>Array of translated texts.</returns>
        public override async Task<string[]> TranslateBatchAsync(string[] texts, string sourceLang = "auto", string targetLang = "en")
        {
            if (texts == null || texts.Length == 0)
                throw new ArgumentException("Texts array cannot be null or empty.", nameof(texts));

            List<Task<string>> translationTasks = new List<Task<string>>();
            foreach (string text in texts)
            {
                translationTasks.Add(TranslateAsync(text, sourceLang, targetLang));
            }

            string[] results = await Task.WhenAll(translationTasks);
            return results;
        }

        public async Task<string> TranslateWithoutCacheAsync(string text, string sourceLang, string targetLang)
        {
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client", ClientValue),
                new KeyValuePair<string, string>("sl", sourceLang),
                new KeyValuePair<string, string>("tl", targetLang),
                new KeyValuePair<string, string>("dt", "t"),
                new KeyValuePair<string, string>("q", text)
                });

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(TranslateUrl, formData);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return ParseTranslation(responseBody);
            }
            catch (HttpException ex)
            {
                throw new Exception("Failed to translate text. Check network connection.", ex);
            }
        }

        private static string CreateCacheKey(string text, string sourceLang, string targetLang)
        {
            return string.Join("_", text, sourceLang, targetLang);
        }

        /// <summary>
        /// Parses the JSON response from Google Translate to extract the translated text.
        /// </summary>
        /// <param name="responseBody">The raw JSON response.</param>
        /// <returns>Extracted translated text.</returns>
        private static string ParseTranslation(string responseBody)
        {
            try
            {
                // The response is a nested JSON array, we need the first string from the first array
                JArray jsonArray = JArray.Parse(responseBody);
                if (jsonArray.Count > 0 && jsonArray[0] is JArray firstArray)
                {
                    StringBuilder result = new StringBuilder();
                    foreach (JArray segment in firstArray)
                    {
                        if (segment.Count <= 0 || !(segment[0] is JValue value))
                        {
                            continue;
                        }
                        result.Append(value.Value<string>());
                    }
                    return result.ToString();
                }
                throw new Exception("Unable to parse translation response.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse translation response.", ex);
            }
        }

        #region Available Languages

        /// <summary>
        /// Gets the list of available source languages.
        /// </summary>
        public override List<string> SourceLanguages => _sourceLanguages;

        /// <summary>
        /// Gets the list of available target languages.
        /// </summary>
        public override List<string> TargetLanguages => _targetLanguages;
        // Source languages available for translation.
        private static readonly List<string> _sourceLanguages = new List<string>
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

        #endregion
    }
}

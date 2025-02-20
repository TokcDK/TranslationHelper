using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.OnlineTranslators
{
    public class GoogleTranslateScraperStandalone
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, string> _cache;
        private const string TranslateUrl = "https://translate.google.com/translate_a/single";
        private const string ClientValue = "gtx";

        public GoogleTranslateScraperStandalone()
        {
            _httpClient = new HttpClient();
            _cache = new Dictionary<string, string>();
        }

        /// <summary>
        /// Translates a single string from source to target language using Google Translate scraping.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="sourceLang">Source language code (e.g., "auto" for auto-detect).</param>
        /// <param name="targetLang">Target language code (e.g., "en" for English).</param>
        /// <returns>Translated text.</returns>
        public async Task<string> TranslateAsync(string text, string sourceLang = "auto", string targetLang = "en")
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));

            // Check cache first
            string cacheKey = $"{text}_{sourceLang}_{targetLang}";
            if (_cache.TryGetValue(cacheKey, out string cachedTranslation))
                return cachedTranslation;

            // Prepare the form data for the POST request
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
                // Send the request
                HttpResponseMessage response = await _httpClient.PostAsync(TranslateUrl, formData);
                response.EnsureSuccessStatusCode();

                // Read and parse the response
                string responseBody = await response.Content.ReadAsStringAsync();
                string translatedText = ParseTranslation(responseBody);

                // Cache the result
                _cache[cacheKey] = translatedText;
                return translatedText;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to translate text. Check network connection.", ex);
            }
        }

        /// <summary>
        /// Parses the JSON response from Google Translate to extract the translated text.
        /// </summary>
        /// <param name="responseBody">The raw JSON response.</param>
        /// <returns>Extracted translated text.</returns>
        private string ParseTranslation(string responseBody)
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
                        if (segment.Count > 0 && segment[0] is JValue value)
                        {
                            result.Append(value.Value<string>());
                        }
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

        /// <summary>
        /// Translates an array of strings, optimizing by batching requests.
        /// </summary>
        /// <param name="texts">Array of texts to translate.</param>
        /// <param name="sourceLang">Source language code.</param>
        /// <param name="targetLang">Target language code.</param>
        /// <returns>Array of translated texts.</returns>
        public async Task<string[]> TranslateBatchAsync(string[] texts, string sourceLang = "auto", string targetLang = "en")
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
    }
}

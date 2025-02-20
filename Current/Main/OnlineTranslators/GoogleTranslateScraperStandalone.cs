﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };
            _httpClient = new HttpClient(handler);
            _cache = new Dictionary<string, string>();
        }

        private async Task<string> TranslateWithoutCacheAsync(string text, string sourceLang, string targetLang)
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
        /// Translates a single string from source to target language using Google Translate scraping.
        /// </summary>
        /// <param name="text">The text to translate. Cannot be null or empty.</param>
        /// <param name="sourceLang">Source language code. Use "auto" for auto-detect. Default is "auto".</param>
        /// <param name="targetLang">Target language code. Default is "en".</param>
        /// <returns>Translated text.</returns>
        public async Task<string> TranslateAsync(string text, string sourceLang = "auto", string targetLang = "en")
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));

            string cacheKey = CreateCacheKey(text, sourceLang, targetLang);
            if (_cache.TryGetValue(cacheKey, out string value))
                return value;

            string translatedText = await TranslateWithoutCacheAsync(text, sourceLang, targetLang);
            _cache[cacheKey] = translatedText;
            return translatedText;
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

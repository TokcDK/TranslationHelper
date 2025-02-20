using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.OnlineTranslators;
using TranslationHelper.Translators;

namespace TranslationHelper
{
    /// <summary>
    /// A legacy Google Translate API integration that translates text using a web service.
    /// Refactored for improved readability and maintainability without altering its functionality.
    /// </summary>
    class GoogleAPIOLD : TranslatorsBase
    {
        // Constants for special symbols used in formatting.
        private const string DNTT = "DNTT";
        private static readonly string splitterString = "</br>";
        private static readonly string[] splitter = new string[] { splitterString };

        // Translation Aggregator constants (credits to Sinflower).
        private readonly long m = 427761;
        private readonly long s = 1179739010;

        public GoogleAPIOLD()
        {
        }

        /// <summary>
        /// Translates a single string from one language to another.
        /// </summary>
        /// <param name="originalText">The text to translate.</param>
        /// <param name="languageFrom">Source language code (default "auto").</param>
        /// <param name="languageTo">Target language code (default "en").</param>
        /// <returns>The translated text, or an empty string on failure, or null if error count is exceeded.</returns>
        public override string Translate(string originalText, string languageFrom = "auto", string languageTo = "en")
        {
            // Abort if overall error count is too high.
            if (ErrorsWebCntOverall > 5)
                return null;

            // Return early for empty input.
            if (string.IsNullOrEmpty(originalText))
                return string.Empty;

            // Use cache if translation was previously obtained.
            if (sessionCache.TryGetValue(originalText, out string cachedTranslation))
                return cachedTranslation;

            // Normalize newlines in the input text.
            string normalizedText = Regex.Replace(originalText, "\\r\\n|\\r|\\n", $" {splitterString} ", RegexOptions.None);
            string encodedText = HttpUtility.UrlEncode(normalizedText, Encoding.UTF8);
            string address = GetUrlAddress(languageFrom, languageTo, encodedText);

            // Initialize web client if not already done.
            InitializeWebClient();

            try
            {
                string responseText = webClient.DownloadString(new Uri(address));
                HtmlDocument htmlDoc = CreateHtmlDocument();
                htmlDoc.Write(responseText);

                // Process the HTML to extract and fix the translated text.
                string translation = GetTranslationHtmlElement(htmlDoc).FixFormat();

                // Cache the result and return.
                sessionCache[originalText] = translation;
                return translation;
            }
            catch (WebException ex)
            {
                new Functions.FunctionsLogs().LogToFile($"google translation web error:{Environment.NewLine}{ex}");
                AppData.OnlineTranslatorCookies = null;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile($"google translation error:{Environment.NewLine}{ex}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Translates an array of strings.
        /// </summary>
        /// <param name="originalTexts">Array of texts to translate.</param>
        /// <param name="languageFrom">Source language code (default "auto").</param>
        /// <param name="languageTo">Target language code (default "en").</param>
        /// <returns>An array of translated strings or null if too many errors occur.</returns>
        public override string[] Translate(string[] originalTexts, string languageFrom = "auto", string languageTo = "en")
        {
            if (ErrorsWebCntOverall > 9 || originalTexts == null)
                return null;

            // Introduce a random delay (up to 2000ms) to help avoid throttling.
            Thread.Sleep(new Random().Next(0, 2000));

            int count = originalTexts.Length;
            var translations = new string[count];
            var textBuilder = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(originalTexts[i]))
                {
                    translations[i] = string.Empty;
                }
                else
                {
                    // Use cached translation if available.
                    if (sessionCache.TryGetValue(originalTexts[i], out string cached))
                    {
                        translations[i] = cached;
                    }

                    // Process text: replace newline sequences and <br> tags.
                    bool isLast = i == count - 1;
                    string processedText = Regex.Replace(originalTexts[i], "\\r\\n|\\r|\\n", DNTT, RegexOptions.None);
                    processedText = Regex.Replace(processedText, @"<br>", "QBRQ", RegexOptions.None);

                    textBuilder.Append(processedText);
                    if (!isLast)
                    {
                        textBuilder.Append(Environment.NewLine)
                                   .Append(splitterString)
                                   .Append(Environment.NewLine);
                    }
                }
            }

            string encodedText = HttpUtility.UrlEncode(textBuilder.ToString(), Encoding.UTF8);
            string address = GetUrlAddress(languageFrom, languageTo, encodedText);
            InitializeWebClient();

            Uri uri = new Uri(address);
            string responseText = string.Empty;

            try
            {
                responseText = webClient.DownloadString(uri);
            }
            catch (WebException)
            {
                // Reset cookies and attempt to retry translation.
                AppData.OnlineTranslatorCookies = new CookieContainer();
                while (ErrorsWebCnt > 0 && ErrorsWebCntOverall < 10)
                {
                    Thread.Sleep(ErrorsWebCntOverall * 10000);
                    try
                    {
                        responseText = webClient.DownloadString(uri);
                        ErrorsWebCntOverall = 0;
                        ErrorsWebCnt = 0;
                        break;
                    }
                    catch
                    {
                        ErrorsWebCntOverall++;
                        if (ErrorsWebCntOverall > 9)
                            return null;
                    }
                }
            }

            try
            {
                HtmlDocument htmlDoc = CreateHtmlDocument();
                htmlDoc.Write(responseText);
                string translatedText = GetTranslationHtmlElement(htmlDoc).FixFormatMulti();

                return string.IsNullOrEmpty(translatedText)
                    ? RetWithNullToEmpty(translations)
                    : SplitTextToLinesAndRestoreSomeSpecsymbols(translatedText);
            }
            catch (WebException ex)
            {
                new Functions.FunctionsLogs().LogToFile($"google array translation web error:{Environment.NewLine}{ex}{Environment.NewLine}uri={uri}");
                AppData.OnlineTranslatorCookies = null;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile($"google array translation error:{Environment.NewLine}{ex}{Environment.NewLine}uri={uri}");
            }

            return null;
        }

        /// <summary>
        /// Constructs the URL for the Google Translate request.
        /// </summary>
        private string GetUrlAddress(string languageFrom, string languageTo, string arg) =>
            // Uses invariant culture to format URL parameters correctly.
            string.Format(CultureInfo.InvariantCulture,
                "https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&tk={3}&q={2}",
                languageFrom, languageTo, arg, Tk(arg));

        /// <summary>
        /// Replaces null entries in the translated array with empty strings.
        /// </summary>
        private static string[] RetWithNullToEmpty(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    array[i] = string.Empty;
            }
            return array;
        }

        /// <summary>
        /// Creates a new HTML document instance.
        /// </summary>
        private HtmlDocument CreateHtmlDocument()
        {
            if (WB == null)
            {
                WB = new WebBrowser { ScriptErrorsSuppressed = true, DocumentText = string.Empty };
            }
            return WB.Document.OpenNew(true);
        }

        /// <summary>
        /// Splits the translated text into lines and restores certain special symbols.
        /// </summary>
        private static string[] SplitTextToLinesAndRestoreSomeSpecsymbols(string text)
        {
            // Restore newlines and split based on the defined splitter.
            string[] parts = text
                .Replace($" {splitterString} ", splitterString)
                .Replace("DNTT", Environment.NewLine)
                .Split(splitter, StringSplitOptions.None);

            // Revert any temporary replacements.
            return parts.Select(x => x.Replace("NBRN", splitterString)).ToArray();
        }

        /// <summary>
        /// Applies a sequence of bitwise operations on the given value using the provided operation string.
        /// </summary>
        private static long Vi(long r, string opStr)
        {
            for (int t = 0; t < opStr.Length; t += 3)
            {
                long a = opStr[t + 2];
                a = a >= 'a' ? a - 87 : a - '0';
                a = opStr[t + 1] == '+' ? r >> (int)a : r << (int)a;
                r = opStr[t] == '+' ? (r + a) & 4294967295 : r ^ a;
            }
            return r;
        }

        /// <summary>
        /// Generates a token based on the input text, used as part of the URL query.
        /// </summary>
        private string Tk(string text)
        {
            var byteList = new List<long>();
            for (int i = 0; i < text.Length; i++)
            {
                long code = text[i];
                if (code < 128)
                {
                    byteList.Add(code);
                }
                else
                {
                    if (code < 2048)
                    {
                        byteList.Add((code >> 6) | 192);
                    }
                    else if ((code & 64512) == 55296 && i + 1 < text.Length && ((long)text[i + 1] & 64512) == 56320)
                    {
                        // Handle surrogate pair.
                        code = 65536 + ((code & 1023) << 10) + (text[++i] & 1023);
                        byteList.Add((code >> 18) | 240);
                        byteList.Add(((code >> 12) & 63) | 128);
                    }
                    else
                    {
                        byteList.Add((code >> 12) | 224);
                        byteList.Add(((code >> 6) & 63) | 128);
                    }
                    byteList.Add((code & 63) | 128);
                }
            }

            // Apply two rounds of bitwise operations with fixed keys.
            const string key1 = "+-a^+6";
            const string key2 = "+-3^+b+-f";
            long token = m;

            foreach (var b in byteList)
            {
                token += b;
                token = Vi(token, key1);
            }
            token = Vi(token, key2);
            token ^= s;
            if (token < 0)
                token = (2147483647 & token) + 2147483648;

            token %= 1000000;
            return $"{token}.{token ^ m}";
        }

        /// <summary>
        /// Ensures that the web client is initialized with the proper settings.
        /// </summary>
        private void InitializeWebClient()
        {
            if (webClient == null)
            {
                webClient = new WebClientEx(AppData.OnlineTranslatorCookies ?? new CookieContainer());
            }
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.UserAgent, FunctionsWeb.GetUserAgent());
        }
    }

}

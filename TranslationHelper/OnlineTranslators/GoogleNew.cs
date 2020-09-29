﻿using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.OnlineTranslators.XUA;
using TranslationHelper.Translators;

namespace TranslationHelper.OnlineTranslators
{
    class GoogleNew : TranslatorsBase
    {
        private static readonly HashSet<string> SupportedLanguages = new HashSet<string>
        {
            "auto","romaji","af","sq","am","ar","hy","az","eu","be","bn","bs","bg","ca","ceb","zh-CN","zh-TW","co","hr","cs","da","nl","en","eo","et","fi","fr","fy","gl","ka","de","el","gu","ht","ha","haw","he","hi","hmn","hu","is","ig","id","ga","it","ja","jw","kn","kk","km","ko","ku","ky","lo","la","lv","lt","lb","mk","mg","ms","ml","mt","mi","mr","mn","my","ne","no","ny","ps","fa","pl","pt","pa","ro","ru","sm","gd","sr","st","sn","sd","si","sk","sl","so","es","su","sw","sv","tl","tg","ta","te","th","tr","uk","ur","uz","vi","cy","xh","yi","yo","zu"
        };

        //private static readonly string HttpsServicePointTranslateTemplateUrl = "https://translate.googleapis.com/translate_a/single?client=webapp&sl={0}&tl={1}&dt=t&dt=at&tk={2}&q={3}";
        private static readonly string HttpsServicePointTranslateTemplateUrl = "https://translate.googleapis.com/translate_a/single?client=webapp&sl={0}&tl={1}&dt=t&tk={2}&q={3}";
        private static readonly string HttpsServicePointRomanizeTemplateUrl = "https://translate.googleapis.com/translate_a/single?client=webapp&sl={0}&tl=en&dt=rm&tk={1}&q={2}";
        private static readonly string HttpsTranslateUserSite = "https://translate.google.com";
        private static readonly Random RandomNumbers = new Random();

        private static readonly string[] Accepts = new string[] { null, "*/*", "application/json" };
        private static readonly string[] AcceptLanguages = new string[] { null, "en-US,en;q=0.9", "en-US", "en" };
        private static readonly string[] Referers = new string[] { null, "https://translate.google.com/" };
        private static readonly string[] AcceptCharsets = new string[] { null, Encoding.UTF8.WebName };

        private static readonly string Accept = Accepts[RandomNumbers.Next(Accepts.Length)];
        private static readonly string AcceptLanguage = AcceptLanguages[RandomNumbers.Next(AcceptLanguages.Length)];
        private static readonly string Referer = Referers[RandomNumbers.Next(Referers.Length)];
        private static readonly string AcceptCharset = AcceptCharsets[RandomNumbers.Next(AcceptCharsets.Length)];

        private CookieContainer _cookieContainer;
        private bool _hasSetup;
        private long m = 427761;
        private long s = 1179739010;
        private int _translationsPerRequest = 10;
        private int _translationCount;
        private int _resetAfter = RandomNumbers.Next(75, 125);

        public GoogleNew(THDataWork thDataWork) : base(thDataWork)
        {
            Initialize();
        }

        public void Initialize(/*IInitializationContext context*/)
        {
            //context.DisableCertificateChecksFor("translate.google.com", "translate.googleapis.com");

            //if (context.DestinationLanguage == "romaji")
            //{
            //    _translationsPerRequest = 1;
            //}
            _translationsPerRequest = 1;

            //if (!SupportedLanguages.Contains(FixLanguage(context.SourceLanguage))) throw new EndpointInitializationException($"The source language '{context.SourceLanguage}' is not supported.");
            //if (!SupportedLanguages.Contains(FixLanguage(context.DestinationLanguage))) throw new EndpointInitializationException($"The destination language '{context.DestinationLanguage}' is not supported.");
        }

        public void OnInspectResponse(IHttpResponseInspectionContext context)
        {
            InspectResponse(context.Response);
        }

        public static void OnExtractTranslation(IHttpTranslationExtractionContext context)
        {
            var isRomaji = true;// context.DestinationLanguage == "romaji";
            var dataIndex = isRomaji ? 3 : 0;

            var data = context.Response.Data;
            var arr = SJSON.Parse(data);
            var lineBuilder = new StringBuilder(data.Length);

            arr = arr.AsArray[0];
            if (arr.IsNull && isRomaji)
            {
                //context.Complete(context.UntranslatedText);
                return;
            }

            foreach (JSONNode entry in arr.AsArray)
            {
                var token = entry.AsArray[dataIndex].ToString();
                token = JsonHelper.Unescape(token.Substring(1, token.Length - 2));

                if (!lineBuilder.EndsWithWhitespaceOrNewline()) lineBuilder.Append('\n');

                lineBuilder.Append(token);
            }

            var allTranslation = lineBuilder.ToString();
            if (false /*context.UntranslatedTexts.Length == 1*/)
            {
                //context.Complete(allTranslation);
            }
            else
            {
                var translatedLines = allTranslation.Split('\n');
                var translatedTexts = new List<string>();

                int current = 0;
                string[] UntranslatedTexts = new string[1];
                foreach (var untranslatedText in UntranslatedTexts/*context.UntranslatedTexts*/)
                {
                    var untranslatedLines = untranslatedText.Split('\n');
                    var untranslatedLinesCount = untranslatedLines.Length;
                    var translatedText = string.Empty;

                    for (int i = 0; i < untranslatedLinesCount; i++)
                    {
                        //if (current >= translatedLines.Length) context.Fail("Batch operation received incorrect number of translations.");

                        var translatedLine = translatedLines[current++];
                        translatedText += translatedLine;

                        if (i != untranslatedLinesCount - 1) translatedText += '\n';
                    }

                    translatedTexts.Add(translatedText);
                }

                //if (current != translatedLines.Length) context.Fail("Batch operation received incorrect number of translations.");

                //context.Complete(translatedTexts.ToArray());
            }
        }

        public IEnumerator OnBeforeTranslate(IHttpTranslationContext context)
        {
            if (!_hasSetup || _translationCount % _resetAfter == 0)
            {
                _resetAfter = RandomNumbers.Next(75, 125);
                _translationCount = 0;

                _hasSetup = true;

                // Setup TKK and cookies
                var enumerator = SetupTKK();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }

        /// <summary>
        /// Attempt to translated the provided untranslated text. Will be used in a "coroutine",
        /// so it can be implemented in an asynchronous fashion.
        /// </summary>
        public IEnumerator Translate(ITranslationContext context)
        {
            var httpContext = new HttpTranslationContext(context);

            // allow implementer of HttpEndpoint to do anything before starting translation
            var setup = OnBeforeTranslate(httpContext);
            if (setup != null)
            {
                while (setup.MoveNext()) yield return setup.Current;
            }

            // prepare request
            OnCreateRequest(httpContext);
            if (httpContext.Request == null) httpContext.Fail("No request object was provided by the translator.");

            // execute request
            var client = new XUnityWebClient();
            var response = client.Send(httpContext.Request);

            // wait for completion
            var iterator = response.GetSupportedEnumerator();
            while (iterator.MoveNext()) yield return iterator.Current;

            if (response.IsTimedOut) httpContext.Fail("Error occurred while retrieving translation. Timeout.");

            httpContext.Response = response;
            OnInspectResponse(httpContext);

            // failure
            if (response.Error != null) httpContext.Fail("Error occurred while retrieving translation.", response.Error);

            // failure
            if (response.Data == null) httpContext.Fail("Error occurred while retrieving translation. Nothing was returned.");

            // extract text
            OnExtractTranslation(httpContext);
        }

        public override string Translate(string OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            throw new NotImplementedException();
        }

        public override string[] Translate(string[] OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            throw new NotImplementedException();
        }

        private static void AddHeaders(XUnityWebRequest request, bool isTranslationRequest)
        {
            request.Headers[HttpRequestHeader.UserAgent] = string.IsNullOrEmpty(AutoTranslatorSettings.UserAgent) ? UserAgents.Chrome_Win10_Latest : AutoTranslatorSettings.UserAgent;
            if (AcceptLanguage != null)
            {
                request.Headers[HttpRequestHeader.AcceptLanguage] = AcceptLanguage;
            }
            if (Accept != null)
            {
                request.Headers[HttpRequestHeader.Accept] = Accept;
            }
            if (Referer != null && isTranslationRequest)
            {
                request.Headers[HttpRequestHeader.Referer] = Referer;
            }
            if (AcceptCharset != null)
            {
                request.Headers[HttpRequestHeader.AcceptCharset] = AcceptCharset;
            }
        }

        public void OnCreateRequest(IHttpRequestCreationContext context)
        {
            _translationCount++;

            var text = "消費税率変更についての商品価格の変更につきまして。";
            //var allUntranslatedText = string.Join("\n", context.UntranslatedTexts);
            var allUntranslatedText = text;

            XUnityWebRequest request;
            if (true /*context.DestinationLanguage == "romaji"*/)
            {
                request = new XUnityWebRequest(
                   string.Format(
                      HttpsServicePointRomanizeTemplateUrl,
                      "auto",
                      Tk(allUntranslatedText),
                      Uri.EscapeDataString(allUntranslatedText)));
            }
            //else
            //{
            //    request = new XUnityWebRequest(
            //       string.Format(
            //          HttpsServicePointTranslateTemplateUrl,
            //          FixLanguage(context.SourceLanguage),
            //          FixLanguage(context.DestinationLanguage),
            //          Tk(allUntranslatedText),
            //          Uri.EscapeDataString(allUntranslatedText)));
            //}

            request.Cookies = _cookieContainer;
            AddHeaders(request, true);

            //context.Complete(request);
        }

        private void InspectResponse(XUnityWebResponse response)
        {
            CookieCollection cookies = response.NewCookies;
            foreach (Cookie cookie in cookies)
            {
                // redirect cookie to correct domain
                cookie.Domain = ".googleapis.com";
            }

            // FIXME: Is this needed? Should already be added
            _cookieContainer.Add(cookies);
        }

        private XUnityWebRequest CreateWebSiteRequest()
        {
            var request = new XUnityWebRequest(HttpsTranslateUserSite);

            request.Cookies = _cookieContainer;
            AddHeaders(request, false);

            return request;
        }

        public IEnumerator SetupTKK()
        {
            XUnityWebResponse response = null;

            _cookieContainer = new CookieContainer();

            try
            {
                var client = new XUnityWebClient();
                var request = CreateWebSiteRequest();
                response = client.Send(request);
            }
            catch (Exception e)
            {
                //XuaLogger.AutoTranslator.Warn(e, "An error occurred while setting up GoogleTranslate TKK. Using fallback TKK values instead.");
                yield break;
            }

            // wait for response completion
            var iterator = response.GetSupportedEnumerator();
            while (iterator.MoveNext()) yield return iterator.Current;

            if (response.IsTimedOut)
            {
                //XuaLogger.AutoTranslator.Warn("A timeout error occurred while setting up GoogleTranslate TKK. Using fallback TKK values instead.");
                yield break;
            }

            // failure
            if (response.Error != null)
            {
                //XuaLogger.AutoTranslator.Warn(response.Error, "An error occurred while setting up GoogleTranslate TKK. Using fallback TKK values instead.");
                yield break;
            }

            // failure
            if (response.Data == null)
            {
                //XuaLogger.AutoTranslator.Warn(null, "An error occurred while setting up GoogleTranslate TKK. Using fallback TKK values instead.");
                yield break;
            }

            InspectResponse(response);

            try
            {
                var html = response.Data;

                bool found = false;
                string[] lookups = new[] { "tkk:'", "TKK='" };
                foreach (var lookup in lookups)
                {
                    var index = html.IndexOf(lookup);
                    if (index > -1) // simple string approach
                    {
                        var startIndex = index + lookup.Length;
                        var endIndex = html.IndexOf("'", startIndex);
                        var result = html.Substring(startIndex, endIndex - startIndex);

                        var parts = result.Split('.');
                        if (parts.Length == 2)
                        {
                            m = long.Parse(parts[0]);
                            s = long.Parse(parts[1]);
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    //XuaLogger.AutoTranslator.Warn("An error occurred while setting up GoogleTranslate TKK. Could not locate TKK value. Using fallback TKK values instead.");
                }
            }
            catch (Exception e)
            {
                //XuaLogger.AutoTranslator.Warn(e, "An error occurred while setting up GoogleTranslate TKK. Using fallback TKK values instead.");
            }
        }

        // TKK Approach stolen from Translation Aggregator r190, all credits to Sinflower

        private static long Vi(long r, string o)
        {
            for (var t = 0; t < o.Length; t += 3)
            {
                long a = o[t + 2];
                a = a >= 'a' ? a - 87 : a - '0';
                a = '+' == o[t + 1] ? r >> (int)a : r << (int)a;
                r = '+' == o[t] ? r + a & 4294967295 : r ^ a;
            }

            return r;
        }

        private string Tk(string r)
        {
            List<long> S = new List<long>();

            for (var v = 0; v < r.Length; v++)
            {
                long A = r[v];
                if (128 > A)
                    S.Add(A);
                else
                {
                    if (2048 > A)
                        S.Add(A >> 6 | 192);
                    else if (55296 == (64512 & A) && v + 1 < r.Length && 56320 == (64512 & r[v + 1]))
                    {
                        A = 65536 + ((1023 & A) << 10) + (1023 & r[++v]);
                        S.Add(A >> 18 | 240);
                        S.Add(A >> 12 & 63 | 128);
                    }
                    else
                    {
                        S.Add(A >> 12 | 224);
                        S.Add(A >> 6 & 63 | 128);
                    }

                    S.Add(63 & A | 128);
                }
            }

            const string F = "+-a^+6";
            const string D = "+-3^+b+-f";
            long p = m;

            for (var b = 0; b < S.Count; b++)
            {
                p += S[b];
                p = Vi(p, F);
            }

            p = Vi(p, D);
            p ^= s;
            if (0 > p)
                p = (2147483647 & p) + 2147483648;

            p %= (long)1e6;

            return p.ToString(CultureInfo.InvariantCulture) + "." + (p ^ m).ToString(CultureInfo.InvariantCulture);
        }
    }
}

using NLog;
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

namespace TranslationHelper.OnlineTranslators
{
    internal class GoogleTranslateMin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal int ErrorsWebCnt = 0;
        internal int ErrorsWebCntOverall = 0;

        protected WebClient webClient;
        protected WebBrowser WB;
        public readonly Dictionary<string, string> sessionCache = new Dictionary<string, string>(1);
        public string Translate(string originalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            if (ErrorsWebCntOverall > 9 || originalText == null)
            {
                return null;
            }

            var arg = HttpUtility.UrlEncode(originalText, Encoding.UTF8);
            var address = string.Format(CultureInfo.InvariantCulture, "https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&tk={3}&q={2}", LanguageFrom, LanguageTo, arg, Tk(arg));

            if (webClient == null)
            {
                webClient = new WebClientEx(AppData.OnlineTranslatorCookies ?? new CookieContainer());
            }

            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.UserAgent, FunctionsWeb.GetUserAgent());
            var uri = new Uri(address);
            try
            {
                var translationHtmlPageText = GetTranslationHtmlPage(uri);

                var htmlDocument = WBhtmlDocument();
                htmlDocument.Write(translationHtmlPageText);

                var translationText = GetTranslationHtmlElement(htmlDocument)
                .Replace("</ p> </ font>", " </p></font>")
                .Replace("</ p>", "</p>")
                .Replace("</ font>", "</font>")
                .Replace("<p align = ", "<p align=")
                .Replace("<img src = ", "<img src=")
                .Replace("<font size = ", "<font size=")
                .Replace("<font face = ", "<font face=");

                return translationText;

            }
            catch (WebException ex)
            {
                Logger.Error("google array translation web error:" + Environment.NewLine + ex + Environment.NewLine + "uri=" + uri);
                AppData.OnlineTranslatorCookies = null;
                ErrorsWebCnt++;
            }
            catch (Exception ex)
            {
                Logger.Error("google array translation error:" + Environment.NewLine + ex + Environment.NewLine + "uri=" + uri);
            }

            return null;
        }

        private string GetTranslationHtmlPage(Uri uri)
        {
            string htmlText = "";
            try
            {
                htmlText = webClient.DownloadString(uri);
            }
            catch (WebException)
            {
                AppData.OnlineTranslatorCookies = null;
                AppData.OnlineTranslatorCookies = new CookieContainer();

                while (ErrorsWebCnt > 0 && ErrorsWebCntOverall < 10)
                {
                    Thread.Sleep(ErrorsWebCntOverall * 10000);

                    try
                    {
                        //another try
                        htmlText = webClient.DownloadString(uri);
                        ErrorsWebCntOverall = 0;
                        ErrorsWebCnt = 0;
                    }
                    catch
                    {
                        ErrorsWebCntOverall++;
                        if (ErrorsWebCntOverall > 9)
                        {
                            return null;
                        }
                    }
                }
            }

            return htmlText;
        }

        protected static string GetTranslationHtmlElement(HtmlDocument htmlDocument)
        {
            foreach (HtmlElement htmlElement in htmlDocument.GetElementsByTagName("div"))
            {
                if (htmlElement.GetAttribute("className") == "result-container" || htmlElement.GetAttribute("className") == "t0")
                {
                    return htmlElement.InnerText;
                }
            }

            foreach (HtmlElement htmlElement in htmlDocument.Body.Children)
            {
                if (htmlElement.InnerText == null || htmlElement.InnerHtml.StartsWith("<"))
                {
                    continue;
                }

                return htmlElement.InnerText;
            }
            return string.Empty;
        }
        private HtmlDocument WBhtmlDocument()
        {
            if (WB == null)
            {
                WB = new WebBrowser() { ScriptErrorsSuppressed = true, DocumentText = string.Empty };
            }
            return WB.Document.OpenNew(true);
        }

        // Translation Aggregator r190. Credits to Sinflower.
        private readonly long m = 427761;
        private readonly long s = 1179739010;
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

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
    class GoogleAPIOLD : TranslatorsBase
    {
        public GoogleAPIOLD()
        {
        }

        public override string Translate(string OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            if (ErrorsWebCntOverall > 5)
            {
                return null;
            }

            string ResultOfTranslation;
            if (OriginalText.Length == 0)
            {
                ResultOfTranslation = string.Empty;
            }
            else
            {
                if (sessionCache.TryGetValue(OriginalText, out string value))
                {
                    ResultOfTranslation = value;
                }
                else
                {
                    string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", $" {splitterString} ", RegexOptions.None);

                    string arg = HttpUtility.UrlEncode(str, Encoding.UTF8);

                    string address = GetUrlAddress(LanguageFrom, LanguageTo, arg);

                    if (webClient == null)
                        webClient = new WebClientEx(AppData.OnlineTranslatorCookies ?? new CookieContainer());

                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, FunctionsWeb.GetUserAgent());

                    try
                    {
                        //Материалы
                        //http://www.cyberforum.ru/ado-net/thread903701.html
                        //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                        //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread

                        string text = webClient.DownloadString(new Uri(address));

                        HtmlDocument htmlDocument = WBhtmlDocument();
                        htmlDocument.Write(text);
                        try
                        {
                            string text2 = GetTranslationHtmlElement(htmlDocument).FixFormat();

                            sessionCache[OriginalText] = text2;

                            return text2;
                        }
                        finally
                        {
                        }
                    }
                    catch (WebException ex)
                    {
                        new Functions.FunctionsLogs().LogToFile("google translation web error:" + Environment.NewLine + ex);
                        AppData.OnlineTranslatorCookies = null;
                    }
                    catch (Exception ex)
                    {
                        new Functions.FunctionsLogs().LogToFile("google translation error:" + Environment.NewLine + ex);
                    }

                    ResultOfTranslation = string.Empty;
                }
            }
            return ResultOfTranslation;
        }

        public override string[] Translate(string[] originalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            if (ErrorsWebCntOverall > 9 || originalText == null)
            {
                return null;
            }

            Thread.Sleep((int)(new Random().NextDouble() * 2000));

            var originalTextArrayLength = originalText.Length;
            var array = new string[originalTextArrayLength];
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < originalTextArrayLength; i++)
            {
                if (originalText[i].Length == 0)
                {
                    array[i] = string.Empty;
                }
                else
                {
                    if (sessionCache.ContainsKey(originalText[i]))
                    {
                        array[i] = sessionCache[originalText[i]];
                    }

                    bool oneOrLast = i == originalTextArrayLength - 1;

                    //https://stackoverflow.com/questions/44444910/unable-to-preserve-line-breaks-in-google-translate-response
                    //https://stackoverflow.com/questions/47709517/google-translate-api-how-to-add-newline-without-changing-the-phrase-meaning
                    _ = stringBuilder.Append(string.Concat(new string[]
                    {
                                Regex.Replace(Regex.Replace(originalText[i], "\\r\\n|\\r|\\n", DNTT, RegexOptions.None), @"<br>", "QBRQ", RegexOptions.None),

                                oneOrLast ? "" : (
                                Environment.NewLine
                                //<br> заменил на </br>, последний также воспринимается как новая строка, влияя на перевод и не клонировался в середину там, где была проблема с <br> <== upd: <br> Гугл один раз раздвоил, сунув копию в середину, из-за чего была ошибка при раборе строк перевода <== <br> вроде как Гугл воспринимает как конец строки и даже не коверкает переводом 
                                + splitterString
                                + Environment.NewLine
                                )
                    }));
                }
            }

            var arg = HttpUtility.UrlEncode(stringBuilder.ToString(), Encoding.UTF8);
            var address = GetUrlAddress(LanguageFrom, LanguageTo, arg);

            if (webClient == null)
            {
                webClient = new WebClientEx(AppData.OnlineTranslatorCookies ?? new CookieContainer());
            }

            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.UserAgent, FunctionsWeb.GetUserAgent());
            var uri = new Uri(address);
            try
            {
                //Материалы
                //http://www.cyberforum.ru/ado-net/thread903701.html
                //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread

                var text = string.Empty;

                try
                {
                    text = webClient.DownloadString(uri);
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
                            text = webClient.DownloadString(uri);
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

                var htmlDocument = WBhtmlDocument();
                htmlDocument.Write(text);

                var text2 = GetTranslationHtmlElement(htmlDocument).FixFormatMulti();

                return text2.Length == 0 ? RetWithNullToEmpty(array) : SplitTextToLinesAndRestoreSomeSpecsymbols(text2);

            }
            catch (WebException ex)
            {
                new Functions.FunctionsLogs().LogToFile("google array translation web error:" + Environment.NewLine + ex + Environment.NewLine + "uri=" + uri);
                AppData.OnlineTranslatorCookies = null;
                ErrorsWebCnt++;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("google array translation error:" + Environment.NewLine + ex + Environment.NewLine + "uri=" + uri);
            }

            return null;
        }

        private string GetUrlAddress(string LanguageFrom, string LanguageTo, string arg)
        {
            return string.Format(CultureInfo.InvariantCulture, "https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&tk={3}&q={2}", LanguageFrom, LanguageTo, arg, Tk(arg));

            //string address = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}tl={1}&dt=t&q={2}", LanguageFrom, LanguageTo, arg);
            //string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, str);
            //string address = string.Format(CultureInfo.InvariantCulture, "https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
            //address = string.Format(CultureInfo.InvariantCulture, "https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&tk={3}&q={2}", LanguageFrom, LanguageTo, arg, Tk(arg));

            //address = string.Format(
            //      HttpsServicePointRomanizeTemplateUrl,
            //      LanguageFrom,
            //      Tk(arg),
            //      Uri.EscapeDataString(arg));
            //string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, stringBuilder.ToString());
        }

        private static string[] RetWithNullToEmpty(string[] array)
        {
            int arrayLength = array.Length;
            for (int j = 0; j < arrayLength; j++)
            {
                if (array[j] == null)
                {
                    array[j] = string.Empty;
                }
            }
            return array;
        }

        private HtmlDocument WBhtmlDocument()
        {
            if (WB == null)
            {
                WB = new WebBrowser() { ScriptErrorsSuppressed = true, DocumentText = string.Empty };
            }
            return WB.Document.OpenNew(true);
        }

        private static string[] SplitTextToLinesAndRestoreSomeSpecsymbols(string text2)
        {
            // возвращение знака новой строки и разделение на подстроки в массив
            string[] array = text2
                         //.Replace("<br> ", "<br>").Replace("<br>", string.Empty)
                         .Replace(" </br> ", splitterString)
                         .Replace("DNTT", Environment.NewLine)
                         .Split(splitter, StringSplitOptions.None);

            //возвращение <br>, если такие были изначально
            array = array.Select(x => x.Replace("NBRN", splitterString)).ToArray();

            //take берет все элементы кроме последнего пустого элемента массива, чтобы в основном коде не уменьшать его счет на один;
            return array;//.Take(array.Length - 1).ToArray(); // commented skip last element because there will not be empty line anymore
        }

        private const string DNTT = "DNTT";

        private static readonly string splitterString = "</br></br>";
        private static readonly string[] splitter = new string[] { splitterString };

        // Translation Aggregator r190. Credits to Sinflower.
        private long m = 427761;
        private long s = 1179739010;
        //private int _translationsPerRequest = 1;
        //private int _translationCount = 0;
        //private static readonly string HttpsServicePointTranslateTemplateUrl = "https://translate.googleapis.com/translate_a/single?client=webapp&sl={0}&tl={1}&dt=t&dt=at&tk={2}&q={3}";
        private static readonly string HttpsServicePointTranslateTemplateUrl = "https://translate.googleapis.com/translate_a/single?client=webapp&sl={0}&tl={1}&dt=t&tk={2}&q={3}";
        private static readonly string HttpsServicePointRomanizeTemplateUrl = "https://translate.googleapis.com/translate_a/single?client=webapp&sl={0}&tl=en&dt=rm&tk={1}&q={2}";
        private static readonly string HttpsTranslateUserSite = "https://translate.google.com";
        private static readonly Random RandomNumbers = new Random();
        private int _resetAfter = RandomNumbers.Next(75, 125);

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

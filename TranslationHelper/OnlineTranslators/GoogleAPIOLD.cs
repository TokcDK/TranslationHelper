﻿using System;
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
using TranslationHelper.OnlineTranslators;
using TranslationHelper.Translators;

namespace TranslationHelper
{
    class GoogleAPIOLD : TranslatorsBase
    {
        public GoogleAPIOLD(THDataWork thDataWork) : base(thDataWork)
        {
        }

        // Token: 0x06000EEC RID: 3820 RVA: 0x0006AD20 File Offset: 0x00068F20
        //public static string Translate2(string text, string fromCulture, string toCulture)
        //{
        //    fromCulture = fromCulture.ToLower();
        //    toCulture = toCulture.ToLower();
        //    string[] array = fromCulture.Split(new char[]
        //    {
        //        '-'
        //    });
        //    bool flag = array.Length > 1;
        //    if (flag)
        //    {
        //        fromCulture = array[0];
        //    }
        //    array = toCulture.Split(new char[]
        //    {
        //        '-'
        //    });
        //    bool flag2 = array.Length > 1;
        //    if (flag2)
        //    {
        //        toCulture = array[0];
        //    }
        //    string address = string.Format("http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}", JsonConvert.ToString(text, '"', StringEscapeHandling.Default), fromCulture, toCulture);
        //    string input;// = null;
        //    try
        //    {
        //        using (WebClient webClient = new WebClient
        //        {
        //            Headers =
        //            {
        //                {
        //                    HttpRequestHeader.UserAgent,
        //                    "Mozilla/5.0"
        //                },
        //                {
        //                    HttpRequestHeader.AcceptCharset,
        //                    "UTF-8"
        //                }
        //            },
        //            Encoding = Encoding.UTF8
        //        })
        //        {
        //            input = webClient.DownloadString(address);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    string value = Regex.Match(input, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value;
        //    bool flag3 = string.IsNullOrEmpty(value);
        //    string result;
        //    if (flag3)
        //    {
        //        result = null;
        //    }
        //    else
        //    {
        //        result = null;
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// кодирует некоторые символы в строке, как видно из строки адресу переводчика Гугл
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string UrlEncodeForTranslation(string value)
        //{
        //    string[,] encode = { { "%", "%25" }, { Environment.NewLine, "%0A" }, { " ", "%20" }, { ":", "%3A" }, { ",", "%2C" }, { "$", "%24" }, { "&", "%26" }, { "#", "%23" }, { "@", "%40" }, { "`", "%60" }, { "+", "%2B" }, { "^", "%5E" }, { "/", "%2F" } };

        //    for (int i = 0; i < encode.Length / 2; i++)
        //    {
        //        value = value.Replace(encode[i, 0], encode[i, 1]);
        //    }

        //    return value;
        //}

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
                if (sessionCache.ContainsKey(OriginalText))
                {
                    ResultOfTranslation = sessionCache[OriginalText];
                }
                else
                {
                    //FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nOriginalText:\r\n" + OriginalText);
                    //https://www.codementor.io/000581/use-the-google-translate-api-for-free-rmxch1s67
                    //link = 'https://translate.googleapis.com/translate_a/single'.'?client=gtx&sl=auto&tl=ru&dt=t&q='.urlencode(text_part);
                    //result = go_curl(result = go​curl(link);

                    //string str = WebUtility.UrlEncode(OriginalText);
                    //string str = UrlEncodeForTranslation(OriginalText);
                    //string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "DNTT", RegexOptions.None);
                    //https://stackoverflow.com/questions/44444910/unable-to-preserve-line-breaks-in-google-translate-response
                    //string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "<code>0</code>", RegexOptions.None);
                    string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", " </br> ", RegexOptions.None);
                    //FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nSTR:\r\n" + str);
                    //string str = OriginalText.Replace(Environment.NewLine, "BBC");
                    //string str = OriginalText.Replace(Environment.NewLine, "%0A");
                    //string str = OriginalText;
                    //string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "%0A", RegexOptions.None).Replace("\"", "\\\"");//.Replace("\r\n", "%0A").Replace("\"", "\\\"")

                    string arg = HttpUtility.UrlEncode(str, Encoding.UTF8);
                    //string arg = UrlEncodeForTranslation(str);
                    //string arg = str;

                    string address = GetUrlAddress(LanguageFrom, LanguageTo, arg);

                    if (webClient == null)
                        webClient = new WebClientEx(thDataWork.OnlineTranslatorCookies ?? new CookieContainer());
                    //webClient = new ScrapySharp.Network.ScrapingBrowser();

                    //using (WebClient webClient = new WebClient())
                    //{
                    //}
                    webClient.Encoding = Encoding.UTF8;
                    //webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgents.OperaMini);
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgents.Chrome_Iron_Win7);
                    //webClient.UserAgent = ScrapySharp.Network.FakeUserAgents.Chrome;
                    try
                    {
                        //Материалы
                        //http://www.cyberforum.ru/ado-net/thread903701.html
                        //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                        //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread

                        //string downloadString = webClient.DownloadString(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}tl={1}&dt=t&q={2}", LanguageFrom, LanguageTo, "打撃/必殺技"));

                        string text = webClient.DownloadString(new Uri(address));

                        HtmlDocument htmlDocument = WBhtmlDocument();
                        htmlDocument.Write(text);
                        try
                        {
                            string text2 = GetTranslationHtmlElement(htmlDocument).FixFormat();

                            sessionCache[OriginalText] = text2;

                            return text2;

                            {
                                //foreach (object obj in htmlDocument.Body.Children)
                                //{
                                //    HtmlElement htmlElement = (HtmlElement)obj;
                                //    if (htmlElement.InnerText == null)
                                //    {
                                //    }
                                //    else
                                //    {
                                //        //FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nhtmlElement.InnerHtml:\r\n" + htmlElement.InnerHtml);
                                //        if (htmlElement.InnerHtml.StartsWith("<"))
                                //        {
                                //        }
                                //        else
                                //        {
                                //            //string text2 = htmlElement.InnerText.Replace(" BBC ", "BBC").Replace(" BBC", "BBC").Replace("BBC ", "BBC").Replace("BBC", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                //            //string text2 = htmlElement.InnerText.Replace(" DNTT ", "DNTT").Replace(" DNTT", "DNTT").Replace("DNTT ", "DNTT").Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                //            string text2 = FixFormat(htmlElement.InnerText);
                                //            //string text2 = htmlElement.InnerText.Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                //            myCache[OriginalText] = text2;
                                //            //FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\ntext2:\r\n" + text2);
                                //            return text2;
                                //        }
                                //    }
                                //}
                            }
                        }
                        finally
                        {
                        }
                    }
                    catch (WebException ex)
                    {
                        new Functions.FunctionsLogs().LogToFile("google translation web error:" + Environment.NewLine + ex);
                        thDataWork.OnlineTranslatorCookies = null;
                    }
                    catch (Exception ex)
                    {
                        new Functions.FunctionsLogs().LogToFile("google translation error:" + Environment.NewLine + ex);
                    }
                    //finally
                    //{
                    //    webClient.Dispose();
                    //}
                    ResultOfTranslation = string.Empty;
                }
            }
            return ResultOfTranslation;
        }

        public override string[] Translate(string[] OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            if (ErrorsWebCntOverall > 9)
            {
                return null;
            }

            Thread.Sleep((int)(new Random().NextDouble() * 2000));

            int num = OriginalText.Length;
            string[] array = new string[num];
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                if (OriginalText[i].Length == 0)
                {
                    array[i] = string.Empty;
                }
                else
                {
                    bool flag2 = false;// TradPonctuation.isAllPonctuation(OriginalText[i]);
                    if (flag2)
                    {
                        array[i] = OriginalText[i];
                    }
                    else
                    {
                        if (sessionCache.ContainsKey(OriginalText[i]))
                        {
                            array[i] = sessionCache[OriginalText[i]];
                        }
                        //https://stackoverflow.com/questions/44444910/unable-to-preserve-line-breaks-in-google-translate-response
                        //https://stackoverflow.com/questions/47709517/google-translate-api-how-to-add-newline-without-changing-the-phrase-meaning
                        _ = stringBuilder.Append(string.Concat(new string[]
                        {
                                //"##",
                                //Conversions.ToString(i),
                                //"#># ",
                                //"<br>",
                                //Environment.NewLine,
                                Regex.Replace(Regex.Replace(OriginalText[i], "\\r\\n|\\r|\\n", DNTT, RegexOptions.None), @"<br>", "QBRQ", RegexOptions.None),
                                //" #<#",
                                //Conversions.ToString(i),
                                //"##\r\n"
                                Environment.NewLine,
                                //<br> заменил на </br>, последний также воспринимается как новая строка, влияя на перевод и не клонировался в середину там, где была проблема с <br> <== upd: <br> Гугл один раз раздвоил, сунув копию в середину, из-за чего была ошибка при раборе строк перевода <== <br> вроде как Гугл воспринимает как конец строки и даже не коверкает переводом 
                                "</br>",
                                Environment.NewLine
                        }));
                    }
                }
            }
            //FileWriter.WriteData("c:\\THLog.log", "\r\nstringBuilder.ToString():\r\n" + stringBuilder.ToString());
            string arg = HttpUtility.UrlEncode(stringBuilder.ToString(), Encoding.UTF8);
            string address = GetUrlAddress(LanguageFrom, LanguageTo, arg);

            if (webClient == null)
            {
                webClient = new WebClientEx(thDataWork.OnlineTranslatorCookies ?? new CookieContainer());
                //webClient = new ScrapySharp.Network.ScrapingBrowser();
            }
            //using (WebClient webClient = new WebClient())
            //{
            //}
            webClient.Encoding = Encoding.UTF8;
            //webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgents.OperaMini);
            webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgents.Chrome_Iron_Win7);
            //webClient.UserAgent= ScrapySharp.Network.FakeUserAgents.Chrome;
            Uri uri = new Uri(address);
            try
            {
                //Материалы, что помогли
                //http://www.cyberforum.ru/ado-net/thread903701.html
                //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread


                //скачать страницу
                string text = string.Empty;

                try
                {
                    text = webClient.DownloadString(uri);
                }
                catch (WebException)
                {
                    thDataWork.OnlineTranslatorCookies = null;
                    thDataWork.OnlineTranslatorCookies = new CookieContainer();

                    while (ErrorsWebCnt>0 && ErrorsWebCntOverall < 10)
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



                //FileWriter.WriteData("c:\\THLog.log", Environment.NewLine+"TEXT:"+Environment.NewLine + text);

                HtmlDocument htmlDocument = WBhtmlDocument();
                htmlDocument.Write(text);

                {
                    //string text2 = string.Empty;
                    //try
                    //{
                    //    text2 = FixFormatMulti(GetTranslationHtmlElement(htmlDocument));
                    //}
                    //finally
                    //{
                    //}
                }

                string text2 = GetTranslationHtmlElement(htmlDocument).FixFormatMulti();

                {
                    //FileWriter.WriteData("c:\\THLog.log", Environment.NewLine+"text2:"+Environment.NewLine + text2);
                    //if (text2.Length == 0)
                    //{
                    //    for (int j = 0; j < array.Count(); j++)
                    //    {
                    //        if (array[j] == null)
                    //        {
                    //            array[j] = string.Empty;
                    //        }
                    //    }
                    //    return array;
                    //}

                    //array = NormalizeResponse(text2);
                    //MatchCollection matchCollection = myReg.Matches(text2);
                    ////FileWriter.WriteData("c:\\THLog.log", "\r\nmatchCollection cnt:" + matchCollection.Count+ ", array.Count()"+ array.Count());
                    //int matchnum = 0;
                    //for (int k = 0; k < array.Count(); k++)
                    //{
                    //    //FileWriter.WriteData("c:\\THLog.log", "\r\narray[k]=" + array[k]);
                    //    if (array[k] == null)
                    //    {
                    //        if (matchCollection.Count == matchnum)
                    //        {
                    //            array[k] = string.Empty;
                    //        }
                    //        else
                    //        {
                    //            //FileWriter.WriteData("c:\\THLog.log", "\r\nSet matchCollection["+matchnum+"].Value" + matchCollection[matchnum].Value);
                    //            array[k] = matchCollection[matchnum].Value.Replace("DNTT", Environment.NewLine);
                    //            matchnum++;
                    //        }
                    //    }
                    //}
                }

                return text2.Length == 0 ? RetWithNullToEmpty(array) : SplitTextToLinesAndRestoreSomeSpecsymbols(text2);
                //return array;
            }
            catch (WebException ex)
            {
                new Functions.FunctionsLogs().LogToFile("google array translation web error:" + Environment.NewLine + ex + Environment.NewLine + "uri=" + uri);
                thDataWork.OnlineTranslatorCookies = null;
                ErrorsWebCnt++;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("google array translation error:" + Environment.NewLine + ex + Environment.NewLine + "uri=" + uri);
            }
            //finally
            //{
            //    webClient.Dispose();
            //}
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

            //using (WebBrowser WB = new WebBrowser() { ScriptErrorsSuppressed = true, DocumentText = string.Empty })//перенос WB сюда - это исправление ошибки "COM object that has been separated from its underlying RCW cannot be used.", когда этот переводчик вызывается в другом потоке STA
            //{
            //    return WB.Document.OpenNew(true);
            //}
        }

        private static string[] SplitTextToLinesAndRestoreSomeSpecsymbols(string text2)
        {
            // возвращение знака новой строки и разделение на подстроки в массив
            string[] array = text2
                         //.Replace("<br> ", "<br>").Replace("<br>", string.Empty)
                         .Replace(" </br> ", "</br>")
                         .Replace("DNTT", Environment.NewLine)
                         .Split(splitter, StringSplitOptions.None);

            //возвращение <br>, если такие были изначально
            array = array.Select(x => x.Replace("NBRN", "</br>")).ToArray();

            //take берет все элементы кроме последнего пустого элемента массива, чтобы в основном коде не уменьшать его счет на один;
            return array.Take(array.Length - 1).ToArray();
        }

        private const string DNTT = "DNTT";
        //private const string DNTT = "<code>0</code>";

        //private const string SEPARATOR = "\r\n#DONOTTRANSLATE#\r\n";

        //private static WebBrowser WB = new WebBrowser();

        //private static readonly Regex myReg = new Regex("(?<=##\\d## ).*?(?=##\\d##)", RegexOptions.Compiled);
        //заменено на <br>
        //private static readonly Regex myReg = new Regex(@"(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\<\# \1 \#\# )|(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\#\#\#\# )", RegexOptions.Compiled);

        //## 27 #># Same sex with this woman _ <# 27 ## 
        //\#\# \d{1,3} \#\# ?(.*) ?\#\# \d{1,3} \#\# ?
        private static readonly string[] splitter = new string[] { "</br>" };

        // TKK Approach stolen from Translation Aggregator r190, all credits to Sinflower
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

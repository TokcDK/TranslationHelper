//using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Web;
//using System.Windows.Forms;
//using ESP_Translation.My;
using TranslationHelper.Main.Functions;
//using System.Threading;
//using Newtonsoft.Json;

namespace TranslationHelper
{
    class GoogleAPI
    {
        public static void ResetCache()
        {
            myCache.Clear();
        }

        public static List<string> Languages
        {
            get
            {
                if (_Languages.Count == 0)
                {
                    _Languages.Add("Afrikaans af");
                    _Languages.Add("Albanian sq");
                    _Languages.Add("Amharic am");
                    _Languages.Add("Arabic ar");
                    _Languages.Add("Armenian hy");
                    _Languages.Add("Azeerbaijani az");
                    _Languages.Add("Basque eu");
                    _Languages.Add("Belarusian be");
                    _Languages.Add("Bengali bn");
                    _Languages.Add("Bosnian bs");
                    _Languages.Add("Bulgarian bg");
                    _Languages.Add("Catalan ca");
                    _Languages.Add("Cebuano ceb");
                    _Languages.Add("Chinese (Simplified) zh-CN");
                    _Languages.Add("Chinese (Traditional) zh-TW");
                    _Languages.Add("Corsican co");
                    _Languages.Add("Croatian hr");
                    _Languages.Add("Czech cs");
                    _Languages.Add("Danish da");
                    _Languages.Add("Dutch nl");
                    _Languages.Add("English en");
                    _Languages.Add("Esperanto eo");
                    _Languages.Add("Estonian et");
                    _Languages.Add("Finnish fi");
                    _Languages.Add("French fr");
                    _Languages.Add("Frisian fy");
                    _Languages.Add("Galician gl");
                    _Languages.Add("Georgian ka");
                    _Languages.Add("German de");
                    _Languages.Add("Greek el");
                    _Languages.Add("Gujarati gu");
                    _Languages.Add("Haitian Creole ht");
                    _Languages.Add("Hausa ha");
                    _Languages.Add("Hawaiian haw");
                    _Languages.Add("Hebrew iw");
                    _Languages.Add("Hindi hi");
                    _Languages.Add("Hmong hmn");
                    _Languages.Add("Hungarian hu");
                    _Languages.Add("Icelandic is");
                    _Languages.Add("Igbo ig");
                    _Languages.Add("Indonesian id");
                    _Languages.Add("Irish ga");
                    _Languages.Add("Italian it");
                    _Languages.Add("Japanese ja");
                    _Languages.Add("Javanese jw");
                    _Languages.Add("Kannada kn");
                    _Languages.Add("Kazakh kk");
                    _Languages.Add("Khmer km");
                    _Languages.Add("Korean ko");
                    _Languages.Add("Kurdish ku");
                    _Languages.Add("Kyrgyz ky");
                    _Languages.Add("Lao lo");
                    _Languages.Add("Latin la");
                    _Languages.Add("Latvian lv");
                    _Languages.Add("Lithuanian lt");
                    _Languages.Add("Luxembourgish lb");
                    _Languages.Add("Macedonian mk");
                    _Languages.Add("Malagasy mg");
                    _Languages.Add("Malay ms");
                    _Languages.Add("Malayalam ml");
                    _Languages.Add("Maori mi");
                    _Languages.Add("Marathi mr");
                    _Languages.Add("Mongolian mn");
                    _Languages.Add("Myanmar (Burmese) my");
                    _Languages.Add("Nepali ne");
                    _Languages.Add("Norwegian no");
                    _Languages.Add("Nyanja (Chichewa) ny");
                    _Languages.Add("Pashto ps");
                    _Languages.Add("Persian fa");
                    _Languages.Add("Polish pl");
                    _Languages.Add("Portuguese pt");
                    _Languages.Add("Punjabi ma");
                    _Languages.Add("Romanian ro");
                    _Languages.Add("Russian ru");
                    _Languages.Add("Samoan sm");
                    _Languages.Add("Scots Gaelic gd");
                    _Languages.Add("Serbian sr");
                    _Languages.Add("Sesotho st");
                    _Languages.Add("Shona sn");
                    _Languages.Add("Sindhi sd");
                    _Languages.Add("Sinhala (Sinhalese) si");
                    _Languages.Add("Slovak sk");
                    _Languages.Add("Slovenian sl");
                    _Languages.Add("Somali so");
                    _Languages.Add("Spanish es");
                    _Languages.Add("Sundanese su");
                    _Languages.Add("Swahili sw");
                    _Languages.Add("Swedish sv");
                    _Languages.Add("Tagalog (Filipino) tl");
                    _Languages.Add("Tajik tg");
                    _Languages.Add("Tamil ta");
                    _Languages.Add("Telugu te");
                    _Languages.Add("Thai th");
                    _Languages.Add("Turkish tr");
                    _Languages.Add("Ukrainian uk");
                    _Languages.Add("Urdu ur");
                    _Languages.Add("Uzbek uz");
                    _Languages.Add("Vietnamese vi");
                    _Languages.Add("Welsh cy");
                    _Languages.Add("Xhosa xh");
                    _Languages.Add("Yiddish yi");
                    _Languages.Add("Yoruba yo");
                    _Languages.Add("Zulu zu");
                }
                return _Languages;
            }
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

        public static string Translate(string OriginalText)
        {
            string ResultOfTranslation;
            try
            {
                string languageFrom = "jp";
                string languageTo = "en";
                if (Properties.Settings.Default.OnlineTranslationSourceLanguage.Length > 0)
                {
                    languageFrom = Properties.Settings.Default.OnlineTranslationSourceLanguage.Split(new char[]
                    {
                        ' '
                    }).Last();
                }
                if (Properties.Settings.Default.OnlineTranslationTargetLanguage.Length > 0)
                {
                    languageTo = Properties.Settings.Default.OnlineTranslationTargetLanguage.Split(new char[]
                    {
                        ' '
                    }).Last();
                }

                if (languageFrom == "jp" && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(OriginalText))
                {
                    return OriginalText;
                }
                else
                {
                    ResultOfTranslation = GetTranslation(OriginalText, languageFrom, languageTo);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return ResultOfTranslation;
        }

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

        public static string GetTranslation(string OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            string ResultOfTranslation;
            if (OriginalText.Length == 0)
            {
                ResultOfTranslation = string.Empty;
            }
            else
            {
                if (myCache.ContainsKey(OriginalText))
                {
                    ResultOfTranslation = myCache[OriginalText];
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

                    //string address = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}tl={1}&dt=t&q={2}", LanguageFrom, LanguageTo, arg);
                    string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
                    //string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, str);

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        webClient.Headers.Add(HttpRequestHeader.UserAgent, BrowserUserAgent);
                        try
                        {
                            //Материалы, что помогли
                            //http://www.cyberforum.ru/ado-net/thread903701.html
                            //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                            //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread

                            //string downloadString = webClient.DownloadString(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}tl={1}&dt=t&q={2}", LanguageFrom, LanguageTo, "打撃/必殺技"));
                            //FileWriter.WriteData("c:\\THLog1.log", "\r\ndownloadString:\r\n" + downloadString);

                            string text = webClient.DownloadString(address);
                            //FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nTEXT:\r\n" + text);

                            HtmlDocument htmlDocument = WBhtmlDocument();
                            htmlDocument.Write(text);
                            try
                            {
                                string text2 = FixFormat(GetTranslationHtmlElement(htmlDocument));

                                myCache[OriginalText] = text2;

                                return text2;

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
                            finally
                            {
                            }
                        }
                        catch// (Exception)
                        {
                            //Debog.Writeline(ex);
                        }
                    }
                    ResultOfTranslation = string.Empty;
                }
            }
            return ResultOfTranslation;
        }

        private static string FixFormat(string input)
        {
            return input
                .Replace("</ p> </ font>", " </p></font>")
                .Replace("</ p>", "</p>")
                .Replace("</ font>", "</font>")
                .Replace("<p align = ", "<p align=")
                .Replace("<img src = ", "<img src=")
                .Replace("<font size = ", "<font size=")
                .Replace("<font face = ", "<font face=")
                .Replace(" </br> ", Environment.NewLine)
                ;
        }

        private static string FixFormatMulti(string input)
        {
            //Заменил разделитель на <br>
            //input = Regex.Replace(input, @"## (\d{1,5}) # > #", "## $1 #>#");
            //input = Regex.Replace(input, @"## (\d{1,5}) #> #", "## $1 #>#");
            //input = Regex.Replace(input, @"## (\d{1,5}) # >#", "## $1 #>#");
            //input = Regex.Replace(input, @"# < # (\d{1,5}) ##", "#<# $1 ##");
            //input = Regex.Replace(input, @"# <# (\d{1,5}) ##", "#<# $1 ##");
            //input = Regex.Replace(input, @"#< # (\d{1,5}) ##", "#<# $1 ##");
            //input = Regex.Replace(input, @" <# (\d{1,5}) ##", " #<# $1 ##");
            //input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) ######", "## $1 #># $2 #<# $1 ##");
            //input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) #####", "## $1 #># $2 #<# $1 ##");
            //input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) ####", "## $1 #># $2 #<# $1 ##");
            //input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) ###", "## $1 #># $2 #<# $1 ##");

            //\#\# (\d{1,5}) \#\>\# ([^(\#\>\# )]*) \#\#\#\#\#
            return input
                //.Replace("<code> 0 </ code>", "<code>0</code>")
                //.Replace("<code> 0 < / code>", "<code>0</code>")
                //.Replace("<code> 0 </ code >", "<code>0</code>")
                //.Replace("< code> 0 </ code>", "<code>0</code>")
                //.Replace("<Code> 0 </ code>", "<code>0</code>")
                //.Replace("<code > 0 </ code>", "<code>0</code>")
                //.Replace("<c ode> 0 </ code>", "<code>0</code>")
                //.Replace(" <code>0</code> ", "<code>0</code>")
                //.Replace(" <code>0</code>", "<code>0</code>")
                //.Replace("<code>0</code> ", "<code>0</code>")
                //.Replace("<code>0</code>", Environment.NewLine)
                //.Replace(DNTT, "\r\n")
                .Replace("</ p> </ font>", " </p></font>")
                .Replace("</ p>", "</p>")
                .Replace("</ font>", "</font>")
                .Replace("<p align = ", "<p align=")
                .Replace("<img src = ", "<img src=")
                .Replace("<font size = ", "<font size=")
                .Replace("<font face = ", "<font face=")
                ;
        }

        public static string[] TranslateMultiple(string[] OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            checked
            {
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
                            if (myCache.ContainsKey(OriginalText[i]))
                            {
                                array[i] = myCache[OriginalText[i]];
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
                string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
                //string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, stringBuilder.ToString());
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, BrowserUserAgent);
                    try
                    {
                        //Материалы, что помогли
                        //http://www.cyberforum.ru/ado-net/thread903701.html
                        //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                        //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread

                        //скачать страницу
                        string text = webClient.DownloadString(address);
                        //FileWriter.WriteData("c:\\THLog.log", Environment.NewLine+"TEXT:"+Environment.NewLine + text);

                        HtmlDocument htmlDocument = WBhtmlDocument();
                        htmlDocument.Write(text);

                        //string text2 = string.Empty;
                        //try
                        //{
                        //    text2 = FixFormatMulti(GetTranslationHtmlElement(htmlDocument));
                        //}
                        //finally
                        //{
                        //}

                        string text2 = FixFormatMulti(GetTranslationHtmlElement(htmlDocument));

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

                        return text2.Length == 0 ? RetWithNullToEmpty(array) : SplitTextToLinesAndRestoreSomeSpecsymbols(text2);
                        //return array;
                    }
                    catch// (Exception ex)
                    {
                        //FileWriter.WriteData("c:\\Exc.log", "\r\nError:" + ex);
                        //Debog.Writeline(ex);
                    }
                }
                return null;
            }
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

        private static string GetTranslationHtmlElement(HtmlDocument htmlDocument)
        {
            //FileWriter.WriteData("c:\\THLog.log", "\r\nhtmlDocument.Body.Children:\r\n" + htmlDocument.Body.Children);
            foreach (object obj in htmlDocument.Body.Children)
            {
                HtmlElement htmlElement = (HtmlElement)obj;
                if (htmlElement.InnerText == null)
                {
                }
                else
                {
                    //FileWriter.WriteData("c:\\THLog.log", "\r\nhtmlElement.InnerHtml:\r\n" + htmlElement.InnerHtml);
                    if (htmlElement.InnerHtml.StartsWith("<"))
                    {
                    }
                    else
                    {
                        //text2 = htmlElement.InnerText.Replace(DNTT, "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                        return htmlElement.InnerText;
                    }
                }
            }
            return string.Empty;
        }

        private static HtmlDocument WBhtmlDocument()
        {
            using (WebBrowser WB = new WebBrowser() { ScriptErrorsSuppressed = true, DocumentText = string.Empty })//перенос WB сюда - это исправление ошибки "COM object that has been separated from its underlying RCW cannot be used.", когда этот переводчик вызывается в другом потоке STA
            {
                return WB.Document.OpenNew(true);
            }
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

        private static readonly List<string> _Languages = new List<string>();

        private static readonly Dictionary<string, string> myCache = new Dictionary<string, string>(1);

        //private static readonly Regex myReg = new Regex("(?<=##\\d## ).*?(?=##\\d##)", RegexOptions.Compiled);
        //заменено на <br>
        //private static readonly Regex myReg = new Regex(@"(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\<\# \1 \#\# )|(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\#\#\#\# )", RegexOptions.Compiled);

        //## 27 #># Same sex with this woman _ <# 27 ## 
        //\#\# \d{1,3} \#\# ?(.*) ?\#\# \d{1,3} \#\# ?
        private static readonly string[] splitter = new string[] { "</br>" };

        //Browser UserAgent
        private const string BrowserUserAgent = "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10";
    }
}

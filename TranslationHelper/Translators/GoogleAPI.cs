//using HtmlAgilityPack;
using Newtonsoft.Json;
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
using Microsoft.VisualBasic.CompilerServices;
//using System.Threading;
//using Newtonsoft.Json;

namespace TranslationHelper
{
    class GoogleAPI
    {
        // Token: 0x06000EEA RID: 3818 RVA: 0x0006A671 File Offset: 0x00068871
        public static void ResetCache()
        {
            myCache.Clear();
        }

        // Token: 0x170005D4 RID: 1492
        // (get) Token: 0x06000EEB RID: 3819 RVA: 0x0006A680 File Offset: 0x00068880
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

        // Token: 0x06000EED RID: 3821 RVA: 0x0006AE40 File Offset: 0x00069040
        public static string Translate(string OriginalText)
        {
            string ResultOfTranslation;
            try
            {
                string languageFrom = "jp";
                string languageTo = "en";
                //bool flag = MySettingsProperty.Settings.GoogleTrad_Entree.Length > 0;
                //if (flag)
                //{
                //    languageFrom = MySettingsProperty.Settings.GoogleTrad_Entree.Split(new char[]
                //    {
                //        ' '
                //    }).Last<string>();
                //}
                //bool flag2 = MySettingsProperty.Settings.GoogleTrad_Sortie.Length > 0;
                //if (flag2)
                //{
                //    languageTo = MySettingsProperty.Settings.GoogleTrad_Sortie.Split(new char[]
                //    {
                //        ' '
                //    }).Last<string>();
                //}
                ResultOfTranslation = GetTranslation(OriginalText, languageFrom, languageTo);
            }
            catch (Exception)
            {
                ResultOfTranslation = string.Empty;
            }
            return ResultOfTranslation;
        }

        /// <summary>
        /// кодирует некоторые символы в строке, как видно из строки адресу переводчика Гугл
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncodeForTranslation(string value)
        {
            string[,] encode = { { "%", "%25" }, { "\r\n", "%0A" }, { " ", "%20" }, { ":", "%3A" }, { ",", "%2C" }, { "$", "%24" }, { "&", "%26" }, { "#", "%23" }, { "@", "%40" }, { "`", "%60" }, { "+", "%2B" }, { "^", "%5E" }, { "/", "%2F" } };

            for (int i = 0; i < encode.Length / 2; i++)
            {
                value = value.Replace(encode[i,0], encode[i, 1]);
            }

            return value;
        }

        // Token: 0x06000EEE RID: 3822 RVA: 0x0006AEFC File Offset: 0x000690FC
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
                    FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nOriginalText:\r\n" + OriginalText);
                    //https://www.codementor.io/000581/use-the-google-translate-api-for-free-rmxch1s67
                    //link = 'https://translate.googleapis.com/translate_a/single'.'?client=gtx&sl=auto&tl=ru&dt=t&q='.urlencode(text_part);
                    //result = go_curl(result = go​curl(link);

                    //string str = WebUtility.UrlEncode(OriginalText);
                    //string str = UrlEncodeForTranslation(OriginalText);
                    //string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "DNTT", RegexOptions.None);
                    //https://stackoverflow.com/questions/44444910/unable-to-preserve-line-breaks-in-google-translate-response
                    string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "<code>0</code>", RegexOptions.None);
                    FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nSTR:\r\n" + str);
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
                        webClient.Headers.Add(HttpRequestHeader.UserAgent, "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10");
                        try
                        {
                            //Материалы, что помогли
                            //http://www.cyberforum.ru/ado-net/thread903701.html
                            //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                            //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread
                            HtmlDocument htmlDocument;
                            string text;
                            using (WebBrowser WB = new WebBrowser())//перенос WB сюда - это исправление ошибки "COM object that has been separated from its underlying RCW cannot be used.", когда этот переводчик вызывается в другом потоке STA
                            {
                                //string downloadString = webClient.DownloadString(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}tl={1}&dt=t&q={2}", LanguageFrom, LanguageTo, "打撃/必殺技"));
                                //FileWriter.WriteData("c:\\THLog1.log", "\r\ndownloadString:\r\n" + downloadString);
                                text = webClient.DownloadString(address);
                                FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nTEXT:\r\n" + text);
                                WB.ScriptErrorsSuppressed = true;
                                WB.DocumentText = "";
                                htmlDocument = WB.Document.OpenNew(true);
                            }
                            htmlDocument.Write(text);
                            try
                            {
                                foreach (object obj in htmlDocument.Body.Children)
                                {
                                    HtmlElement htmlElement = (HtmlElement)obj;
                                    if (htmlElement.InnerText == null)
                                    {
                                    }
                                    else
                                    {
                                        FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\nhtmlElement.InnerHtml:\r\n" + htmlElement.InnerHtml);
                                        if (htmlElement.InnerHtml.StartsWith("<"))
                                        {
                                        }
                                        else
                                        {
                                            //string text2 = htmlElement.InnerText.Replace(" BBC ", "BBC").Replace(" BBC", "BBC").Replace("BBC ", "BBC").Replace("BBC", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                            //string text2 = htmlElement.InnerText.Replace(" DNTT ", "DNTT").Replace(" DNTT", "DNTT").Replace("DNTT ", "DNTT").Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                            string text2 = FixFormat(htmlElement.InnerText);
                                            //string text2 = htmlElement.InnerText.Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                            myCache[OriginalText] = text2;
                                            FileWriter.WriteData("c:\\THLog.log", "\r\n\r\n\r\ntext2:\r\n" + text2);
                                            return text2;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                            }
                        }
                        catch (Exception)
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
            input = Regex.Replace(input, @"## (\d{1,5}) # > #", "## $1 #>#");
            input = Regex.Replace(input, @"## (\d{1,5}) #> #", "## $1 #>#");
            input = Regex.Replace(input, @"## (\d{1,5}) # >#", "## $1 #>#");
            input = Regex.Replace(input, @"# < # (\d{1,5}) ##", "#<# $1 ##");
            input = Regex.Replace(input, @"# <# (\d{1,5}) ##", "#<# $1 ##");
            input = Regex.Replace(input, @"#< # (\d{1,5}) ##", "#<# $1 ##");
            input = Regex.Replace(input, @" <# (\d{1,5}) ##", " #<# $1 ##");
            input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) ######", "## $1 #># $2 #<# $1 ##");
            input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) #####", "## $1 #># $2 #<# $1 ##");
            input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) ####", "## $1 #># $2 #<# $1 ##");
            input = Regex.Replace(input, @"## (\d{1,5}) #># ([^(#># )]*) ###", "## $1 #># $2 #<# $1 ##");
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

        // Token: 0x06000EEF RID: 3823 RVA: 0x0006B160 File Offset: 0x00069360
        public static string[] TranslateMultiple(string[] OriginalText, string LanguageFrom = "auto", string LanguageTo = "en")
        {
            checked
            {
                string[] array = new string[OriginalText.Count()];
                StringBuilder stringBuilder = new StringBuilder();
                int num = OriginalText.Count() - 1;
                for (int i = 0; i <= num; i++)
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
                            bool flag3 = myCache.ContainsKey(OriginalText[i]);
                            if (flag3)
                            {
                                array[i] = myCache[OriginalText[i]];
                            }
                            stringBuilder.Append(string.Concat(new string[]
                            {
                                "##",
                                Conversions.ToString(i),
                                "#># ",
                                Regex.Replace(OriginalText[i], "\\r\\n|\\r|\\n", DNTT, RegexOptions.None),
                                " #<#",
                                Conversions.ToString(i),
                                "##\r\n"
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
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10");
                    try
                    {
                        //Материалы, что помогли
                        //http://www.cyberforum.ru/ado-net/thread903701.html
                        //https://stackoverflow.com/questions/12546126/threading-webbrowser-in-c-sharp
                        //https://stackoverflow.com/questions/4269800/webbrowser-control-in-a-new-thread
                        HtmlDocument htmlDocument;
                        string text;
                        using (WebBrowser WB = new WebBrowser())//перенос WB сюда - это исправление ошибки "COM object that has been separated from its underlying RCW cannot be used.", когда этот переводчик вызывается в другом потоке STA
                        {
                            text = webClient.DownloadString(address);
                            FileWriter.WriteData("c:\\THLog.log", "\r\nTEXT:\r\n" + text);
                            WB.ScriptErrorsSuppressed = true;
                            WB.DocumentText = "";
                            htmlDocument = WB.Document.OpenNew(true);
                        }
                        htmlDocument.Write(text);
                        string text2 = string.Empty;
                        try
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
                                        text2 = FixFormat(htmlElement.InnerText);
                                        break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                        }

                        //FileWriter.WriteData("c:\\THLog.log", "\r\ntext2:\r\n" + text2);
                        if (text2.Length == 0)
                        {
                            for (int j = 0; j < array.Count(); j++)
                            {
                                if (array[j] == null)
                                {
                                    array[j] = string.Empty;
                                }
                            }
                            return array;
                        }
                        MatchCollection matchCollection = myReg.Matches(text2);
                        //FileWriter.WriteData("c:\\THLog.log", "\r\nmatchCollection cnt:" + matchCollection.Count+ ", array.Count()"+ array.Count());
                        int matchnum = 0;
                        for (int k = 0; k < array.Count(); k++)
                        {
                            //FileWriter.WriteData("c:\\THLog.log", "\r\narray[k]=" + array[k]);
                            if (array[k] == null)
                            {
                                if (matchCollection.Count == matchnum)
                                {
                                    array[k] = string.Empty;
                                }
                                else
                                {
                                    //FileWriter.WriteData("c:\\THLog.log", "\r\nSet matchCollection["+matchnum+"].Value" + matchCollection[matchnum].Value);
                                    array[k] = matchCollection[matchnum].Value.Replace("DNTT",Environment.NewLine);
                                    matchnum++;
                                }
                            }
                        }
                        return array;
                    }
                    catch (Exception)
                    {
                        //Debog.Writeline(ex);
                    }
                }
                return null;
            }
        }

        // Token: 0x04000444 RID: 1092
        private const string DNTT = "DNTT";
        //private const string DNTT = "<code>0</code>";

        // Token: 0x04000445 RID: 1093
        //private const string SEPARATOR = "\r\n#DONOTTRANSLATE#\r\n";

        // Token: 0x04000446 RID: 1094
        //private static WebBrowser WB = new WebBrowser();

        // Token: 0x04000447 RID: 1095
        private static readonly List<string> _Languages = new List<string>();

        // Token: 0x04000448 RID: 1096
        private static Dictionary<string, string> myCache = new Dictionary<string, string>(1);

        // Token: 0x04000449 RID: 1097
        //private static readonly Regex myReg = new Regex("(?<=##\\d## ).*?(?=##\\d##)", RegexOptions.Compiled);
        private static readonly Regex myReg = new Regex(@"(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\<\# \1 \#\# )|(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\#\#\#\# )", RegexOptions.Compiled);
        //## 27 #># Same sex with this woman _ <# 27 ## 
        //\#\# \d{1,3} \#\# ?(.*) ?\#\# \d{1,3} \#\# ?
    }
}

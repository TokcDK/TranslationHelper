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
using System.Threading;
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
                bool flag = _Languages.Count == 0;
                if (flag)
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
        public static string Translate2(string text, string fromCulture, string toCulture)
        {
            fromCulture = fromCulture.ToLower();
            toCulture = toCulture.ToLower();
            string[] array = fromCulture.Split(new char[]
            {
                '-'
            });
            bool flag = array.Length > 1;
            if (flag)
            {
                fromCulture = array[0];
            }
            array = toCulture.Split(new char[]
            {
                '-'
            });
            bool flag2 = array.Length > 1;
            if (flag2)
            {
                toCulture = array[0];
            }
            string address = string.Format("http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}", JsonConvert.ToString(text, '"', StringEscapeHandling.Default), fromCulture, toCulture);
            string input;// = null;
            try
            {
                using (WebClient webClient = new WebClient
                {
                    Headers =
                    {
                        {
                            HttpRequestHeader.UserAgent,
                            "Mozilla/5.0"
                        },
                        {
                            HttpRequestHeader.AcceptCharset,
                            "UTF-8"
                        }
                    },
                    Encoding = Encoding.UTF8
                })
                {
                    input = webClient.DownloadString(address);
                }
            }
            catch (Exception)
            {
                return null;
            }
            string value = Regex.Match(input, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value;
            bool flag3 = string.IsNullOrEmpty(value);
            string result;
            if (flag3)
            {
                result = null;
            }
            else
            {
                result = null;
            }
            return result;
        }
        // Token: 0x06000EED RID: 3821 RVA: 0x0006AE40 File Offset: 0x00069040
        public static string Translate(string OriginalText)
        {
            string result;
            try
            {
                string languageFrom = "jp";
                string languageTo = "en";
                /*bool flag = MySettingsProperty.Settings.GoogleTrad_Entree.Length > 0;
                if (flag)
                {
                    languageFrom = MySettingsProperty.Settings.GoogleTrad_Entree.Split(new char[]
                    {
                        ' '
                    }).Last<string>();
                }
                bool flag2 = MySettingsProperty.Settings.GoogleTrad_Sortie.Length > 0;
                if (flag2)
                {
                    languageTo = MySettingsProperty.Settings.GoogleTrad_Sortie.Split(new char[]
                    {
                        ' '
                    }).Last<string>();
                }*/
                result = Translate(OriginalText, languageFrom, languageTo);
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }

        // Token: 0x06000EEE RID: 3822 RVA: 0x0006AEFC File Offset: 0x000690FC
        public static string Translate(string OriginalText, string LanguageFrom, string LanguageTo)
        {
            bool flag = OriginalText.Length == 0;
            string result;
            if (flag)
            {
                result = string.Empty;
            }
            else
            {
                bool flag2 = false;//TradPonctuation.isAllPonctuation(OriginalText);
                if (flag2)
                {
                    result = OriginalText;
                }
                else
                {
                    bool flag3 = myCache.ContainsKey(OriginalText);
                    if (flag3)
                    {
                        result = myCache[OriginalText];
                    }
                    else
                    {
                        string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", DNTT, RegexOptions.None);
                        string arg = HttpUtility.UrlEncode(str, Encoding.UTF8);
                        string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
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
                                        bool flag4 = htmlElement.InnerText == null;
                                        if (!flag4)
                                        {
                                            bool flag5 = htmlElement.InnerHtml.StartsWith("<");
                                            if (!flag5)
                                            {
                                                string text2 = htmlElement.InnerText.Replace(DNTT, "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                                myCache[OriginalText] = text2;
                                                return text2;
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    //MessageBox.Show("WB.IsOffline=="+ WB.IsOffline);
                                    //IEnumerator enumerator;
                                    //if (enumerator is IDisposable)
                                    //{
                                    //    (enumerator as IDisposable).Dispose();
                                    //}
                                }
                            }
                            catch (Exception)
                            {
                                //Debog.Writeline(ex);
                            }
                        }
                        result = string.Empty;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000EEF RID: 3823 RVA: 0x0006B160 File Offset: 0x00069360
        public static string[] TranslateMultiple(string[] OriginalText, string LanguageFrom, string LanguageTo)
        {
            checked
            {
                string[] array = new string[OriginalText.Count<string>() - 1 + 1];
                StringBuilder stringBuilder = new StringBuilder();
                int num = OriginalText.Count<string>() - 1;
                for (int i = 0; i <= num; i++)
                {
                    bool flag = OriginalText[i].Length == 0;
                    if (flag)
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
                                "## ",
                                Regex.Replace(OriginalText[i], "\\r\\n|\\r|\\n", DNTT, RegexOptions.None),
                                " ##",
                                Conversions.ToString(i),
                                "##\r\n"
                            }));
                        }
                    }
                }
                string arg = HttpUtility.UrlEncode(stringBuilder.ToString(), Encoding.UTF8);
                string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
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
                            WB.ScriptErrorsSuppressed = true;
                            WB.DocumentText = "";
                            htmlDocument = WB.Document.OpenNew(true);
                        }
                        htmlDocument.Write(text);
                        string text2 = string.Empty;
                        try
                        {
                            foreach (object obj in htmlDocument.Body.Children)
                            {
                                HtmlElement htmlElement = (HtmlElement)obj;
                                bool flag4 = htmlElement.InnerText == null;
                                if (!flag4)
                                {
                                    bool flag5 = htmlElement.InnerHtml.StartsWith("<");
                                    if (!flag5)
                                    {
                                        text2 = htmlElement.InnerText.Replace(DNTT, "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                        break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            /*IEnumerator enumerator;
                            if (enumerator is IDisposable)
                            {
                                (enumerator as IDisposable).Dispose();
                            }*/
                        }
                        bool flag6 = text2.Length == 0;
                        if (flag6)
                        {
                            int num2 = array.Count<string>() - 1;
                            for (int j = 0; j <= num2; j++)
                            {
                                bool flag7 = array[j] == null;
                                if (flag7)
                                {
                                    array[j] = string.Empty;
                                }
                            }
                            return array;
                        }
                        MatchCollection matchCollection = myReg.Matches(text2);
                        int num3 = 0;
                        int num4 = array.Count<string>() - 1;
                        for (int k = 0; k <= num4; k++)
                        {
                            bool flag8 = array[k] != null;
                            if (!flag8)
                            {
                                bool flag9 = matchCollection.Count == num3;
                                if (flag9)
                                {
                                    array[k] = string.Empty;
                                }
                                else
                                {
                                    array[k] = matchCollection[num3].Value;
                                    num3++;
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

        // Token: 0x04000445 RID: 1093
        //private const string SEPARATOR = "\r\n#DONOTTRANSLATE#\r\n";

        // Token: 0x04000446 RID: 1094
        //private static WebBrowser WB = new WebBrowser();

        // Token: 0x04000447 RID: 1095
        private static readonly List<string> _Languages = new List<string>();

        // Token: 0x04000448 RID: 1096
        private static readonly Dictionary<string, string> myCache = new Dictionary<string, string>(1);

        // Token: 0x04000449 RID: 1097
        private static readonly Regex myReg = new Regex("(?<=##\\d## ).*?(?=##\\d##)", RegexOptions.Compiled);
    }
}

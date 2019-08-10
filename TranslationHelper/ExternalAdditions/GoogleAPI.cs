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
//using Newtonsoft.Json;

namespace TranslationHelper
{
    class GoogleAPI
    {
        // Token: 0x06000EEA RID: 3818 RVA: 0x0006A671 File Offset: 0x00068871
        public static void ResetCache()
        {
            GoogleAPI.myCache.Clear();
        }

        // Token: 0x170005D4 RID: 1492
        // (get) Token: 0x06000EEB RID: 3819 RVA: 0x0006A680 File Offset: 0x00068880
        public static List<string> Languages
        {
            get
            {
                bool flag = GoogleAPI._Languages.Count == 0;
                if (flag)
                {
                    GoogleAPI._Languages.Add("Afrikaans af");
                    GoogleAPI._Languages.Add("Albanian sq");
                    GoogleAPI._Languages.Add("Amharic am");
                    GoogleAPI._Languages.Add("Arabic ar");
                    GoogleAPI._Languages.Add("Armenian hy");
                    GoogleAPI._Languages.Add("Azeerbaijani az");
                    GoogleAPI._Languages.Add("Basque eu");
                    GoogleAPI._Languages.Add("Belarusian be");
                    GoogleAPI._Languages.Add("Bengali bn");
                    GoogleAPI._Languages.Add("Bosnian bs");
                    GoogleAPI._Languages.Add("Bulgarian bg");
                    GoogleAPI._Languages.Add("Catalan ca");
                    GoogleAPI._Languages.Add("Cebuano ceb");
                    GoogleAPI._Languages.Add("Chinese (Simplified) zh-CN");
                    GoogleAPI._Languages.Add("Chinese (Traditional) zh-TW");
                    GoogleAPI._Languages.Add("Corsican co");
                    GoogleAPI._Languages.Add("Croatian hr");
                    GoogleAPI._Languages.Add("Czech cs");
                    GoogleAPI._Languages.Add("Danish da");
                    GoogleAPI._Languages.Add("Dutch nl");
                    GoogleAPI._Languages.Add("English en");
                    GoogleAPI._Languages.Add("Esperanto eo");
                    GoogleAPI._Languages.Add("Estonian et");
                    GoogleAPI._Languages.Add("Finnish fi");
                    GoogleAPI._Languages.Add("French fr");
                    GoogleAPI._Languages.Add("Frisian fy");
                    GoogleAPI._Languages.Add("Galician gl");
                    GoogleAPI._Languages.Add("Georgian ka");
                    GoogleAPI._Languages.Add("German de");
                    GoogleAPI._Languages.Add("Greek el");
                    GoogleAPI._Languages.Add("Gujarati gu");
                    GoogleAPI._Languages.Add("Haitian Creole ht");
                    GoogleAPI._Languages.Add("Hausa ha");
                    GoogleAPI._Languages.Add("Hawaiian haw");
                    GoogleAPI._Languages.Add("Hebrew iw");
                    GoogleAPI._Languages.Add("Hindi hi");
                    GoogleAPI._Languages.Add("Hmong hmn");
                    GoogleAPI._Languages.Add("Hungarian hu");
                    GoogleAPI._Languages.Add("Icelandic is");
                    GoogleAPI._Languages.Add("Igbo ig");
                    GoogleAPI._Languages.Add("Indonesian id");
                    GoogleAPI._Languages.Add("Irish ga");
                    GoogleAPI._Languages.Add("Italian it");
                    GoogleAPI._Languages.Add("Japanese ja");
                    GoogleAPI._Languages.Add("Javanese jw");
                    GoogleAPI._Languages.Add("Kannada kn");
                    GoogleAPI._Languages.Add("Kazakh kk");
                    GoogleAPI._Languages.Add("Khmer km");
                    GoogleAPI._Languages.Add("Korean ko");
                    GoogleAPI._Languages.Add("Kurdish ku");
                    GoogleAPI._Languages.Add("Kyrgyz ky");
                    GoogleAPI._Languages.Add("Lao lo");
                    GoogleAPI._Languages.Add("Latin la");
                    GoogleAPI._Languages.Add("Latvian lv");
                    GoogleAPI._Languages.Add("Lithuanian lt");
                    GoogleAPI._Languages.Add("Luxembourgish lb");
                    GoogleAPI._Languages.Add("Macedonian mk");
                    GoogleAPI._Languages.Add("Malagasy mg");
                    GoogleAPI._Languages.Add("Malay ms");
                    GoogleAPI._Languages.Add("Malayalam ml");
                    GoogleAPI._Languages.Add("Maori mi");
                    GoogleAPI._Languages.Add("Marathi mr");
                    GoogleAPI._Languages.Add("Mongolian mn");
                    GoogleAPI._Languages.Add("Myanmar (Burmese) my");
                    GoogleAPI._Languages.Add("Nepali ne");
                    GoogleAPI._Languages.Add("Norwegian no");
                    GoogleAPI._Languages.Add("Nyanja (Chichewa) ny");
                    GoogleAPI._Languages.Add("Pashto ps");
                    GoogleAPI._Languages.Add("Persian fa");
                    GoogleAPI._Languages.Add("Polish pl");
                    GoogleAPI._Languages.Add("Portuguese pt");
                    GoogleAPI._Languages.Add("Punjabi ma");
                    GoogleAPI._Languages.Add("Romanian ro");
                    GoogleAPI._Languages.Add("Russian ru");
                    GoogleAPI._Languages.Add("Samoan sm");
                    GoogleAPI._Languages.Add("Scots Gaelic gd");
                    GoogleAPI._Languages.Add("Serbian sr");
                    GoogleAPI._Languages.Add("Sesotho st");
                    GoogleAPI._Languages.Add("Shona sn");
                    GoogleAPI._Languages.Add("Sindhi sd");
                    GoogleAPI._Languages.Add("Sinhala (Sinhalese) si");
                    GoogleAPI._Languages.Add("Slovak sk");
                    GoogleAPI._Languages.Add("Slovenian sl");
                    GoogleAPI._Languages.Add("Somali so");
                    GoogleAPI._Languages.Add("Spanish es");
                    GoogleAPI._Languages.Add("Sundanese su");
                    GoogleAPI._Languages.Add("Swahili sw");
                    GoogleAPI._Languages.Add("Swedish sv");
                    GoogleAPI._Languages.Add("Tagalog (Filipino) tl");
                    GoogleAPI._Languages.Add("Tajik tg");
                    GoogleAPI._Languages.Add("Tamil ta");
                    GoogleAPI._Languages.Add("Telugu te");
                    GoogleAPI._Languages.Add("Thai th");
                    GoogleAPI._Languages.Add("Turkish tr");
                    GoogleAPI._Languages.Add("Ukrainian uk");
                    GoogleAPI._Languages.Add("Urdu ur");
                    GoogleAPI._Languages.Add("Uzbek uz");
                    GoogleAPI._Languages.Add("Vietnamese vi");
                    GoogleAPI._Languages.Add("Welsh cy");
                    GoogleAPI._Languages.Add("Xhosa xh");
                    GoogleAPI._Languages.Add("Yiddish yi");
                    GoogleAPI._Languages.Add("Yoruba yo");
                    GoogleAPI._Languages.Add("Zulu zu");
                }
                return GoogleAPI._Languages;
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
            string input;
            try
            {
                input = new WebClient
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
                }.DownloadString(address);
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
                result = GoogleAPI.Translate(OriginalText, languageFrom, languageTo);
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
                    bool flag3 = GoogleAPI.myCache.ContainsKey(OriginalText);
                    if (flag3)
                    {
                        result = GoogleAPI.myCache[OriginalText];
                    }
                    else
                    {
                        string str = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "DNTT", RegexOptions.None);
                        string arg = HttpUtility.UrlEncode(str, Encoding.UTF8);
                        string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
                        WebClient webClient = new WebClient();
                        webClient.Encoding = Encoding.UTF8;
                        webClient.Headers.Add(HttpRequestHeader.UserAgent, "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10");
                        try
                        {
                            string text = webClient.DownloadString(address);
                            GoogleAPI.WB.ScriptErrorsSuppressed = true;
                            GoogleAPI.WB.DocumentText = "";
                            HtmlDocument htmlDocument = GoogleAPI.WB.Document.OpenNew(true);
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
                                            string text2 = htmlElement.InnerText.Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
                                            GoogleAPI.myCache[OriginalText] = text2;
                                            return text2;
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
                        }
                        catch (Exception)
                        {
                            //Debog.Writeline(ex);
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
                            bool flag3 = GoogleAPI.myCache.ContainsKey(OriginalText[i]);
                            if (flag3)
                            {
                                array[i] = GoogleAPI.myCache[OriginalText[i]];
                            }
                            stringBuilder.Append(string.Concat(new string[]
                            {
                                "##",
                                Conversions.ToString(i),
                                "## ",
                                Regex.Replace(OriginalText[i], "\\r\\n|\\r|\\n", "DNTT", RegexOptions.None),
                                " ##",
                                Conversions.ToString(i),
                                "##\r\n"
                            }));
                        }
                    }
                }
                string arg = HttpUtility.UrlEncode(stringBuilder.ToString(), Encoding.UTF8);
                string address = string.Format("https://translate.google.com/m?hl={1}&sl={0}&tl={1}&ie=UTF-8&q={2}", LanguageFrom, LanguageTo, arg);
                WebClient webClient = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10");
                try
                {
                    string text = webClient.DownloadString(address);
                    GoogleAPI.WB.ScriptErrorsSuppressed = true;
                    GoogleAPI.WB.DocumentText = "";
                    System.Windows.Forms.HtmlDocument htmlDocument = GoogleAPI.WB.Document.OpenNew(true);
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
                                    text2 = htmlElement.InnerText.Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
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
                    MatchCollection matchCollection = GoogleAPI.myReg.Matches(text2);
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
                return null;
            }
        }

        // Token: 0x04000444 RID: 1092
        private const string DNTT = "DNTT";

        // Token: 0x04000445 RID: 1093
        private const string SEPARATOR = "\r\n#DONOTTRANSLATE#\r\n";

        // Token: 0x04000446 RID: 1094
        private static WebBrowser WB = new WebBrowser();

        // Token: 0x04000447 RID: 1095
        private static List<string> _Languages = new List<string>();

        // Token: 0x04000448 RID: 1096
        private static Dictionary<string, string> myCache = new Dictionary<string, string>(1);

        // Token: 0x04000449 RID: 1097
        private static Regex myReg = new Regex("(?<=##\\d## ).*?(?=##\\d##)", RegexOptions.Compiled);
    }
}

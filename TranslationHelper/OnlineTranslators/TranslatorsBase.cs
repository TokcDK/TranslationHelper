using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.OnlineTranslators;


//infos for read 
//https://qna.habr.com/q/366420
//https://stackoverflow.com/questions/16642196/get-html-code-from-website-in-c-sharp
//https://www.c-sharpcorner.com/blogs/difference-between-syatemnetwebrequest-and-httpclient-and-webclient-in-c-sharp
//https://html-agility-pack.net/knowledge-base/33679834/is-there-anyway-to-use--browsersession--to-download-files--csharp
namespace TranslationHelper.Translators
{
    internal static class TranslatorsTools
    {
        internal static string GetSourceLanguageID()
        {
            return Properties.Settings.Default.OnlineTranslationSourceLanguage.Split(' ')[1];
        }
        internal static string GetTargetLanguageID()
        {
            return Properties.Settings.Default.OnlineTranslationTargetLanguage.Split(' ')[1];
        }
    }

    abstract class TranslatorsBase : IDisposable
    {
        internal int ErrorsWebCnt = 0;
        internal int ErrorsWebCntOverall = 0;
        
        protected TranslatorsBase()
        {
            
            if (webClient == null)
            {
                if (ProjectData.OnlineTranslatorCookies == null)
                {
                    ProjectData.OnlineTranslatorCookies = new System.Net.CookieContainer();
                }
                webClient = new WebClientEx(ProjectData.OnlineTranslatorCookies);
                //webClient = new ScrapingBrowser
            }
            //webClient.UseDefaultCredentials = false;            
        }

        /// <summary>
        /// resets translator's session translation cache
        /// </summary>
        internal void ResetCache()
        {
            sessionCache.Clear();
        }

        //protected CookieContainer cookies = new CookieContainer();

        internal void OnTranslatorClosed()
        {
            //webClient.Dispose();
            //webClient = null;
        }

        //protected ScrapingBrowser webClient;
        protected WebClientEx webClient;
        protected WebBrowser WB;
        public virtual string Translate(string OriginalText)
        {
            string ResultOfTranslation;
            try
            {
                string languageFrom = "ja";
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

                if (OriginalText.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                {
                    return OriginalText;
                }
                else
                {
                    ResultOfTranslation = Translate(OriginalText, languageFrom, languageTo);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return ResultOfTranslation;
        }

        public virtual string[] Translate(string[] OriginalText)
        {
            string[] Result;
            try
            {
                string languageFrom = "ja";
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

                //if (languageFrom == "ja" && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(OriginalText))
                //{
                //    return OriginalText;
                //}
                //else
                //{
                //    ResultOfTranslation = Translate(OriginalText, languageFrom, languageTo);
                //}
                Result = Translate(OriginalText, languageFrom, languageTo);
            }
            catch (Exception)
            {
                return null;
            }
            return Result;
        }

        public abstract string Translate(string OriginalText, string LanguageFrom = "auto", string LanguageTo = "en");

        public abstract string[] Translate(string[] OriginalText, string LanguageFrom = "auto", string LanguageTo = "en");

        internal static readonly Dictionary<string, string> sessionCache = new Dictionary<string, string>(1);

        private static readonly List<string> _Languages = new List<string>();

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

        protected static string GetTranslationHtmlElement(HtmlDocument htmlDocument)
        {
            //new Functions.FunctionsLogs().LogToFile(htmlDocument.Body.InnerHtml);

            foreach (HtmlElement htmlElement in htmlDocument.GetElementsByTagName("div"))
            {
                if (htmlElement.GetAttribute("className") == "result-container" || htmlElement.GetAttribute("className") == "t0")
                {
                    return htmlElement.InnerText;
                }
            }

            ////FileWriter.WriteData("c:\\THLog.log", "\r\nhtmlDocument.Body.Children:\r\n" + htmlDocument.Body.Children);
            foreach (HtmlElement htmlElement in htmlDocument.Body.Children)
            {
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

        public void Dispose()
        {
            try
            {
                webClient.Dispose();
                webClient = null;
            }
            catch { }
            try
            {
                WB.Dispose();
                WB = null;
            }
            catch { }
        }

        //public static string ReturnTranslatedOrCache(DataSet cacheDS, string InputLine, TranslatorsBase Translator=null)
        //{
        //    string valuefromcache = FunctionsTable.TranslationCacheFind(cacheDS, InputLine);

        //    if (valuefromcache.Length != 0)
        //    {
        //        return valuefromcache;
        //    }
        //    else
        //    {
        //        if (Translator != null)
        //        {
        //            return Translator.Translate(InputLine);
        //        }
        //        return new GoogleAPI().Translate(InputLine);
        //    }
        //}
    }

    static class TranslatorsExtensions
    {
        internal static string FixHTMLTags(this string input)
        {
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

        internal static string FixFormat(this string input)
        {
            return input
                .FixHTMLTags()
                .Replace(" </br> ", Environment.NewLine)
                ;
        }

        internal static string FixFormatMulti(this string input)
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
            return input.FixHTMLTags();
        }
    }
}

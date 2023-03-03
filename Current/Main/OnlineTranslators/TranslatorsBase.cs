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
            return AppSettings.OnlineTranslationSourceLanguage.Split(' ')[1];
        }
        internal static string GetTargetLanguageID()
        {
            return AppSettings.OnlineTranslationTargetLanguage.Split(' ')[1];
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
                if (AppData.OnlineTranslatorCookies == null)
                {
                    AppData.OnlineTranslatorCookies = new System.Net.CookieContainer();
                }
                webClient = new WebClientEx(AppData.OnlineTranslatorCookies);
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
                if (AppSettings.OnlineTranslationSourceLanguage.Length > 0)
                {
                    languageFrom = AppSettings.OnlineTranslationSourceLanguage.Split(new char[]
                    {
                        ' '
                    }).Last();
                }
                if (AppSettings.OnlineTranslationTargetLanguage.Length > 0)
                {
                    languageTo = AppSettings.OnlineTranslationTargetLanguage.Split(new char[]
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
                if (AppSettings.OnlineTranslationSourceLanguage.Length > 0)
                {
                    languageFrom = AppSettings.OnlineTranslationSourceLanguage.Split(new char[]
                    {
                        ' '
                    }).Last();
                }
                if (AppSettings.OnlineTranslationTargetLanguage.Length > 0)
                {
                    languageTo = AppSettings.OnlineTranslationTargetLanguage.Split(new char[]
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

        private static readonly List<string> _sourceLanguages = new List<string>();

        public static List<string> SourceLanguages
        {
            get
            {
                if (_sourceLanguages.Count == 0)
                {
                    _sourceLanguages.Add("Afrikaans af");
                    _sourceLanguages.Add("Albanian sq");
                    _sourceLanguages.Add("Amharic am");
                    _sourceLanguages.Add("Arabic ar");
                    _sourceLanguages.Add("Armenian hy");
                    _sourceLanguages.Add("Azeerbaijani az");
                    _sourceLanguages.Add("Basque eu");
                    _sourceLanguages.Add("Belarusian be");
                    _sourceLanguages.Add("Bengali bn");
                    _sourceLanguages.Add("Bosnian bs");
                    _sourceLanguages.Add("Bulgarian bg");
                    _sourceLanguages.Add("Catalan ca");
                    _sourceLanguages.Add("Cebuano ceb");
                    _sourceLanguages.Add("Chinese (Simplified) zh-CN");
                    _sourceLanguages.Add("Chinese (Traditional) zh-TW");
                    _sourceLanguages.Add("Corsican co");
                    _sourceLanguages.Add("Croatian hr");
                    _sourceLanguages.Add("Czech cs");
                    _sourceLanguages.Add("Danish da");
                    _sourceLanguages.Add("Dutch nl");
                    _sourceLanguages.Add("English en");
                    _sourceLanguages.Add("Esperanto eo");
                    _sourceLanguages.Add("Estonian et");
                    _sourceLanguages.Add("Finnish fi");
                    _sourceLanguages.Add("French fr");
                    _sourceLanguages.Add("Frisian fy");
                    _sourceLanguages.Add("Galician gl");
                    _sourceLanguages.Add("Georgian ka");
                    _sourceLanguages.Add("German de");
                    _sourceLanguages.Add("Greek el");
                    _sourceLanguages.Add("Gujarati gu");
                    _sourceLanguages.Add("Haitian Creole ht");
                    _sourceLanguages.Add("Hausa ha");
                    _sourceLanguages.Add("Hawaiian haw");
                    _sourceLanguages.Add("Hebrew iw");
                    _sourceLanguages.Add("Hindi hi");
                    _sourceLanguages.Add("Hmong hmn");
                    _sourceLanguages.Add("Hungarian hu");
                    _sourceLanguages.Add("Icelandic is");
                    _sourceLanguages.Add("Igbo ig");
                    _sourceLanguages.Add("Indonesian id");
                    _sourceLanguages.Add("Irish ga");
                    _sourceLanguages.Add("Italian it");
                    _sourceLanguages.Add("Japanese ja");
                    _sourceLanguages.Add("Javanese jw");
                    _sourceLanguages.Add("Kannada kn");
                    _sourceLanguages.Add("Kazakh kk");
                    _sourceLanguages.Add("Khmer km");
                    _sourceLanguages.Add("Korean ko");
                    _sourceLanguages.Add("Kurdish ku");
                    _sourceLanguages.Add("Kyrgyz ky");
                    _sourceLanguages.Add("Lao lo");
                    _sourceLanguages.Add("Latin la");
                    _sourceLanguages.Add("Latvian lv");
                    _sourceLanguages.Add("Lithuanian lt");
                    _sourceLanguages.Add("Luxembourgish lb");
                    _sourceLanguages.Add("Macedonian mk");
                    _sourceLanguages.Add("Malagasy mg");
                    _sourceLanguages.Add("Malay ms");
                    _sourceLanguages.Add("Malayalam ml");
                    _sourceLanguages.Add("Maori mi");
                    _sourceLanguages.Add("Marathi mr");
                    _sourceLanguages.Add("Mongolian mn");
                    _sourceLanguages.Add("Myanmar (Burmese) my");
                    _sourceLanguages.Add("Nepali ne");
                    _sourceLanguages.Add("Norwegian no");
                    _sourceLanguages.Add("Nyanja (Chichewa) ny");
                    _sourceLanguages.Add("Pashto ps");
                    _sourceLanguages.Add("Persian fa");
                    _sourceLanguages.Add("Polish pl");
                    _sourceLanguages.Add("Portuguese pt");
                    _sourceLanguages.Add("Punjabi ma");
                    _sourceLanguages.Add("Romanian ro");
                    _sourceLanguages.Add("Russian ru");
                    _sourceLanguages.Add("Samoan sm");
                    _sourceLanguages.Add("Scots Gaelic gd");
                    _sourceLanguages.Add("Serbian sr");
                    _sourceLanguages.Add("Sesotho st");
                    _sourceLanguages.Add("Shona sn");
                    _sourceLanguages.Add("Sindhi sd");
                    _sourceLanguages.Add("Sinhala (Sinhalese) si");
                    _sourceLanguages.Add("Slovak sk");
                    _sourceLanguages.Add("Slovenian sl");
                    _sourceLanguages.Add("Somali so");
                    _sourceLanguages.Add("Spanish es");
                    _sourceLanguages.Add("Sundanese su");
                    _sourceLanguages.Add("Swahili sw");
                    _sourceLanguages.Add("Swedish sv");
                    _sourceLanguages.Add("Tagalog (Filipino) tl");
                    _sourceLanguages.Add("Tajik tg");
                    _sourceLanguages.Add("Tamil ta");
                    _sourceLanguages.Add("Telugu te");
                    _sourceLanguages.Add("Thai th");
                    _sourceLanguages.Add("Turkish tr");
                    _sourceLanguages.Add("Ukrainian uk");
                    _sourceLanguages.Add("Urdu ur");
                    _sourceLanguages.Add("Uzbek uz");
                    _sourceLanguages.Add("Vietnamese vi");
                    _sourceLanguages.Add("Welsh cy");
                    _sourceLanguages.Add("Xhosa xh");
                    _sourceLanguages.Add("Yiddish yi");
                    _sourceLanguages.Add("Yoruba yo");
                    _sourceLanguages.Add("Zulu zu");
                }
                return _sourceLanguages;
            }
        }

        private static readonly List<string> _targetLanguages = new List<string>();
        public static List<string> TargetLanguages
        {
            get
            {
                if (_targetLanguages.Count == 0)
                {
                    _targetLanguages.Add("Afrikaans af");
                    _targetLanguages.Add("Albanian sq");
                    _targetLanguages.Add("Amharic am");
                    _targetLanguages.Add("Arabic ar");
                    _targetLanguages.Add("Armenian hy");
                    _targetLanguages.Add("Azeerbaijani az");
                    _targetLanguages.Add("Basque eu");
                    _targetLanguages.Add("Belarusian be");
                    _targetLanguages.Add("Bengali bn");
                    _targetLanguages.Add("Bosnian bs");
                    _targetLanguages.Add("Bulgarian bg");
                    _targetLanguages.Add("Catalan ca");
                    _targetLanguages.Add("Cebuano ceb");
                    _targetLanguages.Add("Chinese (Simplified) zh-CN");
                    _targetLanguages.Add("Chinese (Traditional) zh-TW");
                    _targetLanguages.Add("Corsican co");
                    _targetLanguages.Add("Croatian hr");
                    _targetLanguages.Add("Czech cs");
                    _targetLanguages.Add("Danish da");
                    _targetLanguages.Add("Dutch nl");
                    _targetLanguages.Add("English en");
                    _targetLanguages.Add("Esperanto eo");
                    _targetLanguages.Add("Estonian et");
                    _targetLanguages.Add("Finnish fi");
                    _targetLanguages.Add("French fr");
                    _targetLanguages.Add("Frisian fy");
                    _targetLanguages.Add("Galician gl");
                    _targetLanguages.Add("Georgian ka");
                    _targetLanguages.Add("German de");
                    _targetLanguages.Add("Greek el");
                    _targetLanguages.Add("Gujarati gu");
                    _targetLanguages.Add("Haitian Creole ht");
                    _targetLanguages.Add("Hausa ha");
                    _targetLanguages.Add("Hawaiian haw");
                    _targetLanguages.Add("Hebrew iw");
                    _targetLanguages.Add("Hindi hi");
                    _targetLanguages.Add("Hmong hmn");
                    _targetLanguages.Add("Hungarian hu");
                    _targetLanguages.Add("Icelandic is");
                    _targetLanguages.Add("Igbo ig");
                    _targetLanguages.Add("Indonesian id");
                    _targetLanguages.Add("Irish ga");
                    _targetLanguages.Add("Italian it");
                    _targetLanguages.Add("Japanese ja");
                    _targetLanguages.Add("Javanese jw");
                    _targetLanguages.Add("Kannada kn");
                    _targetLanguages.Add("Kazakh kk");
                    _targetLanguages.Add("Khmer km");
                    _targetLanguages.Add("Korean ko");
                    _targetLanguages.Add("Kurdish ku");
                    _targetLanguages.Add("Kyrgyz ky");
                    _targetLanguages.Add("Lao lo");
                    _targetLanguages.Add("Latin la");
                    _targetLanguages.Add("Latvian lv");
                    _targetLanguages.Add("Lithuanian lt");
                    _targetLanguages.Add("Luxembourgish lb");
                    _targetLanguages.Add("Macedonian mk");
                    _targetLanguages.Add("Malagasy mg");
                    _targetLanguages.Add("Malay ms");
                    _targetLanguages.Add("Malayalam ml");
                    _targetLanguages.Add("Maori mi");
                    _targetLanguages.Add("Marathi mr");
                    _targetLanguages.Add("Mongolian mn");
                    _targetLanguages.Add("Myanmar (Burmese) my");
                    _targetLanguages.Add("Nepali ne");
                    _targetLanguages.Add("Norwegian no");
                    _targetLanguages.Add("Nyanja (Chichewa) ny");
                    _targetLanguages.Add("Pashto ps");
                    _targetLanguages.Add("Persian fa");
                    _targetLanguages.Add("Polish pl");
                    _targetLanguages.Add("Portuguese pt");
                    _targetLanguages.Add("Punjabi ma");
                    _targetLanguages.Add("Romanian ro");
                    _targetLanguages.Add("Russian ru");
                    _targetLanguages.Add("Samoan sm");
                    _targetLanguages.Add("Scots Gaelic gd");
                    _targetLanguages.Add("Serbian sr");
                    _targetLanguages.Add("Sesotho st");
                    _targetLanguages.Add("Shona sn");
                    _targetLanguages.Add("Sindhi sd");
                    _targetLanguages.Add("Sinhala (Sinhalese) si");
                    _targetLanguages.Add("Slovak sk");
                    _targetLanguages.Add("Slovenian sl");
                    _targetLanguages.Add("Somali so");
                    _targetLanguages.Add("Spanish es");
                    _targetLanguages.Add("Sundanese su");
                    _targetLanguages.Add("Swahili sw");
                    _targetLanguages.Add("Swedish sv");
                    _targetLanguages.Add("Tagalog (Filipino) tl");
                    _targetLanguages.Add("Tajik tg");
                    _targetLanguages.Add("Tamil ta");
                    _targetLanguages.Add("Telugu te");
                    _targetLanguages.Add("Thai th");
                    _targetLanguages.Add("Turkish tr");
                    _targetLanguages.Add("Ukrainian uk");
                    _targetLanguages.Add("Urdu ur");
                    _targetLanguages.Add("Uzbek uz");
                    _targetLanguages.Add("Vietnamese vi");
                    _targetLanguages.Add("Welsh cy");
                    _targetLanguages.Add("Xhosa xh");
                    _targetLanguages.Add("Yiddish yi");
                    _targetLanguages.Add("Yoruba yo");
                    _targetLanguages.Add("Zulu zu");
                }
                return _targetLanguages;
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

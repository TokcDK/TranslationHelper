//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Windows.Forms;

//namespace TranslationHelper
//{
//    // Token: 0x02000018 RID: 24
//    public static class DEEPL
//    {
//        // Token: 0x0600049D RID: 1181 RVA: 0x00010682 File Offset: 0x0000E882
//        public static void ResetCache()
//        {
//            DEEPL.myCache.Clear();
//        }

//        // Token: 0x0600049E RID: 1182 RVA: 0x00010690 File Offset: 0x0000E890
//        public static string Translate(string OriginalText)
//        {
//            string result;
//            try
//            {
//                string languageFrom = "en";
//                string languageTo = "ru";
//                result = DEEPL.Translate(OriginalText, languageFrom, languageTo);
//            }
//            catch (Exception)
//            {
//                result = string.Empty;
//            }
//            return result;
//        }

//        // Token: 0x0600049F RID: 1183 RVA: 0x000106DC File Offset: 0x0000E8DC
//        public static string Translate(string OriginalText, string LanguageFrom, string LanguageTo)
//        {
//            bool flag = OriginalText.Length == 0;
//            string result;
//            if (flag)
//            {
//                result = string.Empty;
//            }
//            else
//            {
//                bool flag2 = false;// TradPonctuation.isAllPonctuation(OriginalText);
//                if (flag2)
//                {
//                    result = OriginalText;
//                }
//                else
//                {
//                    bool flag3 = DEEPL.myCache.ContainsKey(OriginalText);
//                    if (flag3)
//                    {
//                        result = DEEPL.myCache[OriginalText];
//                    }
//                    else
//                    {
//                        string stringToEscape = Regex.Replace(OriginalText, "\\r\\n|\\r|\\n", "DNTT", RegexOptions.None);
//                        string arg = Uri.EscapeDataString(stringToEscape);
//                        string address = string.Format("https://www.deepl.com/translator#{0}/{1}/{2}", LanguageFrom, LanguageTo, arg);
//                        using (WebClient webClient = new WebClient())
//                        {
//                            webClient.Encoding = Encoding.UTF8;
//                            webClient.Headers.Add(HttpRequestHeader.UserAgent, "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10");
//                            try
//                            {
//                                string text = webClient.DownloadString(address);
//                                DEEPL.WB.ScriptErrorsSuppressed = true;
//                                DEEPL.WB.DocumentText = "";
//                                HtmlDocument htmlDocument = DEEPL.WB.Document.OpenNew(true);
//                                htmlDocument.Write(text);
//                                try
//                                {
//                                    foreach (object obj in htmlDocument.Body.Children)
//                                    {
//                                        HtmlElement htmlElement = (HtmlElement)obj;
//                                        bool flag4 = htmlElement.InnerText == null;
//                                        if (!flag4)
//                                        {
//                                            bool flag5 = htmlElement.InnerHtml.StartsWith("<");
//                                            if (!flag5)
//                                            {
//                                                string text2 = htmlElement.InnerText.Replace("DNTT", "\r\n").Replace("</ p> </ font>", " </p></font>").Replace("</ p>", "</p>").Replace("</ font>", "</font>").Replace("<p align = ", "<p align=").Replace("<img src = ", "<img src=").Replace("<font size = ", "<font size=").Replace("<font face = ", "<font face=");
//                                                DEEPL.myCache[OriginalText] = text2;
//                                                return text2;
//                                            }
//                                        }
//                                    }
//                                }
//                                finally
//                                {
//                                    //IEnumerator enumerator;
//                                    //if (enumerator is IDisposable)
//                                    //{
//                                    //    (enumerator as IDisposable).Dispose();
//                                    //}
//                                }
//                            }
//                            catch (Exception)
//                            {
//                                //Debog.Writeline(ex);
//                            }
//                        }
//                        result = string.Empty;
//                    }
//                }
//            }
//            return result;
//        }

//        // Token: 0x04000047 RID: 71
//        //private const string DNTT = "DNTT";

//        // Token: 0x04000048 RID: 72
//        //private const string SEPARATOR = "\r\n#DONOTTRANSLATE#\r\n";

//        // Token: 0x04000049 RID: 73
//        private static readonly WebBrowser WB = new WebBrowser();

//        // Token: 0x0400004A RID: 74
//        //private static readonly List<string> _Languages = new List<string>();

//        // Token: 0x0400004B RID: 75
//        private static readonly Dictionary<string, string> myCache = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
//    }
//}

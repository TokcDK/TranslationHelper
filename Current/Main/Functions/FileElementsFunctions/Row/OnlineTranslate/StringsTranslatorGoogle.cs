using GoogleTranslateFreeApi;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    public interface IStringsTranslator
    {
        string Name { get; }

        bool IsReady { get; set; }

        bool RequiresKey { get; }

        void Prepare();

        bool TryTranslate(string text, out string translated);
    }

    public abstract class StringsTranslatorBase : IStringsTranslator
    {
        public abstract string Name { get; }

        public bool IsReady { get; set; }

        public virtual bool RequiresKey { get; } = false;

        public virtual string SourceLanguageIdentifier
        {
            get
            {
                return "auto";
            }
        }

        public abstract string TargetLanguageIdentifier { get; }

        public abstract void Prepare();

        public abstract bool TryTranslate(string text, out string translated);

        public abstract bool SupportsCurrentLanguage();
    }

    public class StringsTranslatorGoogle : StringsTranslatorBase
    {
        public override string Name
        {
            get
            {
                return "Google";
            }
        }

        public override string TargetLanguageIdentifier
        {
            get
            {
                string result;
                if ((result = this._cachedTranslateLanguage) == null)
                {
                    result = (this._cachedTranslateLanguage = GetTranslateLanguage());
                }
                return result;
            }
        }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000079 RID: 121 RVA: 0x00005CB9 File Offset: 0x00003EB9
        public override bool RequiresKey
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00005CBC File Offset: 0x00003EBC
        public override void Prepare()
        {
            try
            {
                string responseUnsafe = GetResponseUnsafe("https://translate.google.com");
                bool flag = string.IsNullOrEmpty(responseUnsafe);
                if (flag)
                {
                    throw new Exception("no response");
                }
                base.IsReady = true;
            }
            catch (Exception ex)
            {
                //Log.Message("<color=#34e2eb>AutoTranslation</color>: Preparing Translator named '" + this.Name + "' was failed, reason: " + ex.Message);
            }
        }

        public override bool TryTranslate(string text, out string translated)
        {
            bool result;
            try
            {
                string url = string.Format("https://translate.google.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&ie=UTF-8&oe=UTF-8&q={2}", SourceLanguageIdentifier, TargetLanguageIdentifier, WebUtility.UrlEncode(text));
                string a;
                string text2 = ParseResult(GetResponseUnsafe(url), out a);
                translated = ((a == "en") ? text : text2);
                result = true;
            }
            catch (Exception ex)
            {
                string text3 = "<color=#34e2eb>AutoTranslation</color>: " + string.Format("{0}, translate failed. reason: {1}|{2}", this.Name, ex.GetType(), ex.Message);
                //Log.WarningOnce(text3 + ", target: " + text, text3.GetHashCode());
                translated = text;
                result = false;
            }
            return result;
        }

        // Token: 0x0600007C RID: 124 RVA: 0x00005DE0 File Offset: 0x00003FE0
        public override bool SupportsCurrentLanguage()
        {
            //LoadedLanguage activeLanguage = LanguageDatabase.activeLanguage;
            bool flag = false;
            bool result;
            if (flag)
            {
                //Log.Warning("<color=#34e2eb>AutoTranslation</color>: activeLanguage was null");
                result = false;
            }
            else
            {
                string text;
                result = TranslateLanguageGetter.TryGetValue("English", out text);
            }
            return result;
        }

        public static string GetResponseUnsafe(string url)
        {
            string result = "";

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "GET";
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                try
                {
                    bool flag = httpWebResponse.StatusCode == HttpStatusCode.OK;
                    if (flag)
                    {
                        using (Stream responseStream = httpWebResponse.GetResponseStream())
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                return result = streamReader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Request failed with status: {0}", httpWebResponse.StatusCode));
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("Request failed with the exception: {0}", ex));
                }
            }

            return result;
        }
        
        internal static string ParseResult(string text, out string detectedLang)
        {
            sb.Clear();
            bool flag = false;
            for (int i = 0; i < text.Length; i++)
            {
                bool flag2 = text[i] == '"' && i > 0 && text[i - 1] != '\\';
                if (flag2)
                {
                    bool flag3 = flag;
                    if (flag3)
                    {
                        break;
                    }
                    flag = true;
                }
                else
                {
                    bool flag4 = flag;
                    if (flag4)
                    {
                        sb.Append(text[i]);
                    }
                }
            }
            string pattern = "\\[\"([^\"]+)\"\\]\\]\\]";
            Match match = Regex.Match(text, pattern);
            detectedLang = "aaaaa";
            return sb.ToString();
        }

        // Token: 0x0600007F RID: 127 RVA: 0x00005F98 File Offset: 0x00004198
        private static string GetTranslateLanguage()
        {
            bool flag = false;
            string result;
            if (flag)
            {
                //Log.Warning("<color=#34e2eb>AutoTranslation</color>: activeLanguage was null");
                result = "ru";
            }
            else
            {
                string text;
                bool flag2 = !TranslateLanguageGetter.TryGetValue("English", out text);
                if (flag2)
                {
                    //Log.Error("<color=#34e2eb>AutoTranslation</color>: Unsupported language: " + LanguageDatabase.activeLanguage.LegacyFolderName);
                    text = "ru";
                }
                result = text;
            }
            return result;
        }

        // Token: 0x06000081 RID: 129 RVA: 0x00006014 File Offset: 0x00004214
        // Note: this type is marked as 'beforefieldinit'.
        static StringsTranslatorGoogle()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["Korean"] = "ko";
            dictionary["Catalan"] = "ca";
            dictionary["ChineseSimplified"] = "zh-CN";
            dictionary["ChineseTraditional"] = "zh-TW";
            dictionary["Czech"] = "cs";
            dictionary["Danish"] = "da";
            dictionary["Dutch"] = "nl";
            dictionary["Estonian"] = "et";
            dictionary["Finnish"] = "fi";
            dictionary["French"] = "fr";
            dictionary["German"] = "de";
            dictionary["Greek"] = "el";
            dictionary["Hungarian"] = "hu";
            dictionary["Italian"] = "it";
            dictionary["Japanese"] = "ja";
            dictionary["Norwegian"] = "no";
            dictionary["Polish"] = "pl";
            dictionary["Portuguese"] = "pt-PT";
            dictionary["PortugueseBrazilian"] = "pt";
            dictionary["Romanian"] = "ro";
            dictionary["Russian"] = "ru";
            dictionary["Slovak"] = "sk";
            dictionary["SpanishLatin"] = "es";
            dictionary["Spanish"] = "es";
            dictionary["Swedish"] = "sv";
            dictionary["Turkish"] = "tr";
            dictionary["Ukrainian"] = "uk";
            dictionary["English"] = "en";
            dictionary["Vietnamese"] = "vi";
            dictionary["Thai"] = "th";
            TranslateLanguageGetter = dictionary;
        }

        private const string testUrl = "https://translate.google.com";

        private const string urlFormat = "https://translate.google.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&ie=UTF-8&oe=UTF-8&q={2}";

        private static readonly StringBuilder sb = new StringBuilder(1024);

        private string _cachedTranslateLanguage;

        private static readonly Dictionary<string, string> TranslateLanguageGetter;
    }
}

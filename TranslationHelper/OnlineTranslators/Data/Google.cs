using System.Collections.Generic;

namespace TranslationHelper.OnlineTranslators.Data
{
    class Google : TranslatorsDataBase
    {
        /// <summary>
        /// Link for open in browser
        /// </summary>
        internal override string Weblink { get => "https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}"; }

        /// <summary>
        /// list of Google languages
        /// </summary>
        /// <param name="languages"></param>
        /// <returns></returns>
        internal override List<string> GetLanguages(List<string> languages)
        {
            if (languages.Count == 0)
            {
                languages.Add("Afrikaans af");
                languages.Add("Albanian sq");
                languages.Add("Amharic am");
                languages.Add("Arabic ar");
                languages.Add("Armenian hy");
                languages.Add("Azeerbaijani az");
                languages.Add("Basque eu");
                languages.Add("Belarusian be");
                languages.Add("Bengali bn");
                languages.Add("Bosnian bs");
                languages.Add("Bulgarian bg");
                languages.Add("Catalan ca");
                languages.Add("Cebuano ceb");
                languages.Add("Chinese (Simplified) zh-CN");
                languages.Add("Chinese (Traditional) zh-TW");
                languages.Add("Corsican co");
                languages.Add("Croatian hr");
                languages.Add("Czech cs");
                languages.Add("Danish da");
                languages.Add("Dutch nl");
                languages.Add("English en");
                languages.Add("Esperanto eo");
                languages.Add("Estonian et");
                languages.Add("Finnish fi");
                languages.Add("French fr");
                languages.Add("Frisian fy");
                languages.Add("Galician gl");
                languages.Add("Georgian ka");
                languages.Add("German de");
                languages.Add("Greek el");
                languages.Add("Gujarati gu");
                languages.Add("Haitian Creole ht");
                languages.Add("Hausa ha");
                languages.Add("Hawaiian haw");
                languages.Add("Hebrew iw");
                languages.Add("Hindi hi");
                languages.Add("Hmong hmn");
                languages.Add("Hungarian hu");
                languages.Add("Icelandic is");
                languages.Add("Igbo ig");
                languages.Add("Indonesian id");
                languages.Add("Irish ga");
                languages.Add("Italian it");
                languages.Add("Japanese ja");
                languages.Add("Javanese jw");
                languages.Add("Kannada kn");
                languages.Add("Kazakh kk");
                languages.Add("Khmer km");
                languages.Add("Korean ko");
                languages.Add("Kurdish ku");
                languages.Add("Kyrgyz ky");
                languages.Add("Lao lo");
                languages.Add("Latin la");
                languages.Add("Latvian lv");
                languages.Add("Lithuanian lt");
                languages.Add("Luxembourgish lb");
                languages.Add("Macedonian mk");
                languages.Add("Malagasy mg");
                languages.Add("Malay ms");
                languages.Add("Malayalam ml");
                languages.Add("Maori mi");
                languages.Add("Marathi mr");
                languages.Add("Mongolian mn");
                languages.Add("Myanmar (Burmese) my");
                languages.Add("Nepali ne");
                languages.Add("Norwegian no");
                languages.Add("Nyanja (Chichewa) ny");
                languages.Add("Pashto ps");
                languages.Add("Persian fa");
                languages.Add("Polish pl");
                languages.Add("Portuguese pt");
                languages.Add("Punjabi ma");
                languages.Add("Romanian ro");
                languages.Add("Russian ru");
                languages.Add("Samoan sm");
                languages.Add("Scots Gaelic gd");
                languages.Add("Serbian sr");
                languages.Add("Sesotho st");
                languages.Add("Shona sn");
                languages.Add("Sindhi sd");
                languages.Add("Sinhala (Sinhalese) si");
                languages.Add("Slovak sk");
                languages.Add("Slovenian sl");
                languages.Add("Somali so");
                languages.Add("Spanish es");
                languages.Add("Sundanese su");
                languages.Add("Swahili sw");
                languages.Add("Swedish sv");
                languages.Add("Tagalog (Filipino) tl");
                languages.Add("Tajik tg");
                languages.Add("Tamil ta");
                languages.Add("Telugu te");
                languages.Add("Thai th");
                languages.Add("Turkish tr");
                languages.Add("Ukrainian uk");
                languages.Add("Urdu ur");
                languages.Add("Uzbek uz");
                languages.Add("Vietnamese vi");
                languages.Add("Welsh cy");
                languages.Add("Xhosa xh");
                languages.Add("Yiddish yi");
                languages.Add("Yoruba yo");
                languages.Add("Zulu zu");
            }
            return languages;
        }
    }
}

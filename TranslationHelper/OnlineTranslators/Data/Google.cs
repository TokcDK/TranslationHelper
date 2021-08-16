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
        /// <param name="Languages"></param>
        /// <returns></returns>
        internal override List<string> GetLanguages(List<string> Languages)
        {
            if (Languages.Count == 0)
            {
                Languages.Add("Afrikaans af");
                Languages.Add("Albanian sq");
                Languages.Add("Amharic am");
                Languages.Add("Arabic ar");
                Languages.Add("Armenian hy");
                Languages.Add("Azeerbaijani az");
                Languages.Add("Basque eu");
                Languages.Add("Belarusian be");
                Languages.Add("Bengali bn");
                Languages.Add("Bosnian bs");
                Languages.Add("Bulgarian bg");
                Languages.Add("Catalan ca");
                Languages.Add("Cebuano ceb");
                Languages.Add("Chinese (Simplified) zh-CN");
                Languages.Add("Chinese (Traditional) zh-TW");
                Languages.Add("Corsican co");
                Languages.Add("Croatian hr");
                Languages.Add("Czech cs");
                Languages.Add("Danish da");
                Languages.Add("Dutch nl");
                Languages.Add("English en");
                Languages.Add("Esperanto eo");
                Languages.Add("Estonian et");
                Languages.Add("Finnish fi");
                Languages.Add("French fr");
                Languages.Add("Frisian fy");
                Languages.Add("Galician gl");
                Languages.Add("Georgian ka");
                Languages.Add("German de");
                Languages.Add("Greek el");
                Languages.Add("Gujarati gu");
                Languages.Add("Haitian Creole ht");
                Languages.Add("Hausa ha");
                Languages.Add("Hawaiian haw");
                Languages.Add("Hebrew iw");
                Languages.Add("Hindi hi");
                Languages.Add("Hmong hmn");
                Languages.Add("Hungarian hu");
                Languages.Add("Icelandic is");
                Languages.Add("Igbo ig");
                Languages.Add("Indonesian id");
                Languages.Add("Irish ga");
                Languages.Add("Italian it");
                Languages.Add("Japanese ja");
                Languages.Add("Javanese jw");
                Languages.Add("Kannada kn");
                Languages.Add("Kazakh kk");
                Languages.Add("Khmer km");
                Languages.Add("Korean ko");
                Languages.Add("Kurdish ku");
                Languages.Add("Kyrgyz ky");
                Languages.Add("Lao lo");
                Languages.Add("Latin la");
                Languages.Add("Latvian lv");
                Languages.Add("Lithuanian lt");
                Languages.Add("Luxembourgish lb");
                Languages.Add("Macedonian mk");
                Languages.Add("Malagasy mg");
                Languages.Add("Malay ms");
                Languages.Add("Malayalam ml");
                Languages.Add("Maori mi");
                Languages.Add("Marathi mr");
                Languages.Add("Mongolian mn");
                Languages.Add("Myanmar (Burmese) my");
                Languages.Add("Nepali ne");
                Languages.Add("Norwegian no");
                Languages.Add("Nyanja (Chichewa) ny");
                Languages.Add("Pashto ps");
                Languages.Add("Persian fa");
                Languages.Add("Polish pl");
                Languages.Add("Portuguese pt");
                Languages.Add("Punjabi ma");
                Languages.Add("Romanian ro");
                Languages.Add("Russian ru");
                Languages.Add("Samoan sm");
                Languages.Add("Scots Gaelic gd");
                Languages.Add("Serbian sr");
                Languages.Add("Sesotho st");
                Languages.Add("Shona sn");
                Languages.Add("Sindhi sd");
                Languages.Add("Sinhala (Sinhalese) si");
                Languages.Add("Slovak sk");
                Languages.Add("Slovenian sl");
                Languages.Add("Somali so");
                Languages.Add("Spanish es");
                Languages.Add("Sundanese su");
                Languages.Add("Swahili sw");
                Languages.Add("Swedish sv");
                Languages.Add("Tagalog (Filipino) tl");
                Languages.Add("Tajik tg");
                Languages.Add("Tamil ta");
                Languages.Add("Telugu te");
                Languages.Add("Thai th");
                Languages.Add("Turkish tr");
                Languages.Add("Ukrainian uk");
                Languages.Add("Urdu ur");
                Languages.Add("Uzbek uz");
                Languages.Add("Vietnamese vi");
                Languages.Add("Welsh cy");
                Languages.Add("Xhosa xh");
                Languages.Add("Yiddish yi");
                Languages.Add("Yoruba yo");
                Languages.Add("Zulu zu");
            }
            return Languages;
        }
    }
}

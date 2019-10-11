using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Main.Functions
{
    class RomajiKana
    {

        //Detect languages
        //source: https://stackoverflow.com/questions/15805859/detect-japanese-character-input-and-romajis-ascii
        public static IEnumerable<char> GetCharsInRange(string text, int min, int max)
        {
            //Usage:
            //var romaji = GetCharsInRange(searchKeyword, 0x0020, 0x007E);
            //var hiragana = GetCharsInRange(searchKeyword, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(searchKeyword, 0x30A0, 0x30FF);
            //var kanji = GetCharsInRange(searchKeyword, 0x4E00, 0x9FBF);
            return text.Where(e => e >= min && e <= max);
        }

        public static string THShowLangsOfString(string target, string langlocale)
        {
            string ret = string.Empty;
            if (langlocale == "all")
            {
                var kanji = GetCharsInRange(target, 0x4E00, 0x9FBF);
                var romaji = GetCharsInRange(target, 0x0020, 0x007E);
                var hiragana = GetCharsInRange(target, 0x3040, 0x309F);
                var katakana = GetCharsInRange(target, 0x30A0, 0x30FF);

                ret += T._("contains: ") + Environment.NewLine;
                if (romaji.Any())
                {
                    ret += ("       romaji:" + GetLocaleLangCount(target, "romaji") + Environment.NewLine);
                }
                if (kanji.Any())
                {
                    ret += ("       kanji:" + GetLocaleLangCount(target, "kanji") + Environment.NewLine);
                }
                if (hiragana.Any())
                {
                    ret += ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + Environment.NewLine);
                }
                if (katakana.Any())
                {
                    ret += ("       katakana:" + GetLocaleLangCount(target, "katakana") + Environment.NewLine);
                }
                if (GetLocaleLangCount(target, "other") > 0)
                {
                    ret += ("       other:" + GetLocaleLangCount(target, "other") + Environment.NewLine);
                }
            }
            else if (string.Compare(langlocale, "romaji", true) == 0) //asdf оптимизация, сравнение с игнором регистра: http://www.vcskicks.com/optimize_csharp_code.php
            {
                return ("       romaji:" + GetLocaleLangCount(target, "romaji") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "kanji", true) == 0)
            {
                return ("       kanji:" + GetLocaleLangCount(target, "kanji") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "hiragana", true) == 0)
            {
                return ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "katakana", true) == 0)
            {
                return ("       katakana:" + GetLocaleLangCount(target, "katakana") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "other", true) == 0)
            {
                return ("       other:" + (GetLocaleLangCount(target, "other")) + Environment.NewLine);
            }

            return ret;
        }

        public static int GetLocaleLangCount(string target, string langlocale)
        {
            //var romaji = GetCharsInRange(THSourceTextBox.Text, 0x0020, 0x007E);
            //var kanji = GetCharsInRange(THSourceTextBox.Text, 0x4E00, 0x9FBF);
            //var hiragana = GetCharsInRange(THSourceTextBox.Text, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(THSourceTextBox.Text, 0x30A0, 0x30FF);

            int romaji = (GetCharsInRange(target, 0x0020, 0x007E)).Count();
            int kanji = (GetCharsInRange(target, 0x4E00, 0x9FBF)).Count();
            int hiragana = (GetCharsInRange(target, 0x3040, 0x309F)).Count();
            int katakana = (GetCharsInRange(target, 0x30A0, 0x30FF)).Count();

            int all = (romaji + kanji + hiragana + katakana);
            if (string.Compare(langlocale, "all", true) == 0)
            {
                return all + (target.Length - all);
            }
            else if (string.Compare(langlocale, "romaji", true) == 0)
            {
                return (romaji);
            }
            else if (string.Compare(langlocale, "kanji", true) == 0)
            {
                return (kanji);
            }
            else if (string.Compare(langlocale, "hiragana", true) == 0)
            {
                return (hiragana);
            }
            else if (string.Compare(langlocale, "katakana", true) == 0)
            {
                return (katakana);
            }
            else if (string.Compare(langlocale, "other", true) == 0)
            {
                return (target.Length - all);
            }

            return all;
        }
    }
}

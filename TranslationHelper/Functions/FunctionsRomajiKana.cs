using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TranslationHelper.Main.Functions
{
    public static class FunctionsRomajiKana
    {
        /// <summary>
        /// True if language is Japanese and procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool IsTheStringMostlyRomajiOrOther(string inputString)
        {
            return (Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(inputString));
        }

        /// <summary>
        /// True if procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return true;

            return SelectedLocalePercentFromStringIsNotValid(inputString)
                || SelectedLocalePercentFromStringIsNotValid(inputString, "other")
                || (GetLocaleLangCount(inputString, "romaji") + GetLocaleLangCount(inputString, "other")) == inputString.Length
                || GetLocaleLangCount(inputString, "romaji") == inputString.Length
                || GetLocaleLangCount(inputString, "other") == inputString.Length;
        }

        /// <summary>
        /// True if procent of selected locale characters in target string is lesser of set value in Settings
        /// </summary>
        /// <param name="target"></param>
        /// <param name="langlocale"></param>
        /// <returns></returns>
        public static bool SelectedLocalePercentFromStringIsNotValid(string target, string langlocale = "romaji")
        {
            try
            {
                if (Properties.Settings.Default.DontLoadStringIfRomajiPercent && !string.IsNullOrEmpty(target))
                {
                    return ((GetLocaleLangCount(target, langlocale) * 100) / GetLocaleLangCount(target, "all")) > Properties.Settings.Default.DontLoadStringIfRomajiPercentNum;
                }
            }
            catch
            {

            }
            return false;
        }

        public static bool HasNOJPcharacters(string str)
        {
            return GetLocaleLangCount(str, "kanji") < 1 && GetLocaleLangCount(str, "katakana") < 1 && GetLocaleLangCount(str, "hiragana") < 1;
        }

        /// <summary>
        /// Замена японских(и не только) цифр на стандартные
        /// </summary>
        public static string THFixDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            else
            {
                string ret = input;
                //Перевод японских(и не только) цифр в стандартные--------------------------
                string[][] ReplaceData = new string[19][]
                {
                new string[2] { "０", "0" },
                new string[2] { "１", "1" },
                new string[2] { "２", "2" },
                new string[2] { "３", "3" },
                new string[2] { "４", "4" },
                new string[2] { "５", "5" },
                new string[2] { "６", "6" },
                new string[2] { "７", "7" },
                new string[2] { "８", "8" },
                new string[2] { "９", "9" },
                new string[2] { "①", "1" },
                new string[2] { "②", "2" },
                new string[2] { "③", "3" },
                new string[2] { "④", "4" },
                new string[2] { "⑤", "5" },
                new string[2] { "⑥", "6" },
                new string[2] { "⑦", "7" },
                new string[2] { "⑧", "8" },
                new string[2] { "⑨", "9" }
                };

                int ReplaceDataCount = ReplaceData.Length / 2;
                for (int a = 0; a < ReplaceDataCount; a++)
                {
                    ret = ret.Replace(ReplaceData[a][0], ReplaceData[a][1]);
                }

                return ret;
            }
        }

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
            else if (string.Compare(langlocale, "romaji", true, CultureInfo.InvariantCulture) == 0) //asdf оптимизация, сравнение с игнором регистра: http://www.vcskicks.com/optimize_csharp_code.php
            {
                return ("       romaji:" + GetLocaleLangCount(target, "romaji") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "kanji", true, CultureInfo.InvariantCulture) == 0)
            {
                return ("       kanji:" + GetLocaleLangCount(target, "kanji") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "hiragana", true, CultureInfo.InvariantCulture) == 0)
            {
                return ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "katakana", true, CultureInfo.InvariantCulture) == 0)
            {
                return ("       katakana:" + GetLocaleLangCount(target, "katakana") + Environment.NewLine);
            }
            else if (string.Compare(langlocale, "other", true, CultureInfo.InvariantCulture) == 0)
            {
                return ("       other:" + (GetLocaleLangCount(target, "other")) + Environment.NewLine);
            }

            return ret;
        }

        public static int GetLocaleLangCount(string target, string langlocale)
        {
            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }
            //var romaji = GetCharsInRange(THSourceTextBox.Text, 0x0020, 0x007E);
            //var kanji = GetCharsInRange(THSourceTextBox.Text, 0x4E00, 0x9FBF);
            //var hiragana = GetCharsInRange(THSourceTextBox.Text, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(THSourceTextBox.Text, 0x30A0, 0x30FF);

            int romaji = (GetCharsInRange(target, 0x0020, 0x007E)).Count();
            int kanji = (GetCharsInRange(target, 0x4E00, 0x9FBF)).Count();
            int hiragana = (GetCharsInRange(target, 0x3040, 0x309F)).Count();
            int katakana = (GetCharsInRange(target, 0x30A0, 0x30FF)).Count();

            int all = (romaji + kanji + hiragana + katakana);
            if (string.Compare(langlocale, "all", true, CultureInfo.InvariantCulture) == 0)
            {
                return all + (target.Length - all);
            }
            else if (string.Compare(langlocale, "romaji", true, CultureInfo.InvariantCulture) == 0)
            {
                return (romaji);
            }
            else if (string.Compare(langlocale, "kanji", true, CultureInfo.InvariantCulture) == 0)
            {
                return (kanji);
            }
            else if (string.Compare(langlocale, "hiragana", true, CultureInfo.InvariantCulture) == 0)
            {
                return (hiragana);
            }
            else if (string.Compare(langlocale, "katakana", true, CultureInfo.InvariantCulture) == 0)
            {
                return (katakana);
            }
            else if (string.Compare(langlocale, "other", true, CultureInfo.InvariantCulture) == 0)
            {
                return (target.Length - all);
            }

            return all;
        }
    }
}

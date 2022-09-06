using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TranslationHelper.Data;

namespace TranslationHelper.Main.Functions
{
    public static class FunctionsRomajiKana
    {
        /// <summary>
        /// True if procent of selected locale characters in target string is lesser of set value in Settings
        /// </summary>
        /// <param name="target"></param>
        /// <param name="langlocale"></param>
        /// <returns></returns>
        public static bool LocalePercentIsNotValid(string target, string langlocale = "romaji", bool load = true, int Percent = -1)
        {
            try
            {
                if (!string.IsNullOrEmpty(target))
                {
                    if (Percent < 0)
                    {
                        Percent = AppSettings.DontLoadStringIfRomajiPercentNumber;
                    }

                    if (Percent >= 100 || Percent < 0)
                    {
                        return false;
                    }

                    if (langlocale == "romaji")
                    {
                        if (load && !AppSettings.DontLoadStringIfRomajiPercent)
                        {
                            return false;
                        }

                        return (GetLocaleLangCount(target, langlocale) * 100 / GetLocaleLangCount(target, "all")) > Percent;
                    }
                    else if (langlocale == "other")
                    {
                        return (GetLocaleLangCount(target, langlocale) * 100 / GetLocaleLangCount(target, "all")) > Percent;
                    }
                    else
                    {
                        Percent = 100 - Percent;//recalculate value for correct comprasion

                        return (GetLocaleLangCount(target, langlocale) * 100 / GetLocaleLangCount(target, "all")) < Percent;
                    }
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
                return input
                .Replace("０", "0")
                .Replace("１", "1")
                .Replace("２", "2")
                .Replace("３", "3")
                .Replace("４", "4")
                .Replace("５", "5")
                .Replace("６", "6")
                .Replace("７", "7")
                .Replace("８", "8")
                .Replace("９", "9")
                .Replace("①", "1")
                .Replace("②", "2")
                .Replace("③", "3")
                .Replace("④", "4")
                .Replace("⑤", "5")
                .Replace("⑥", "6")
                .Replace("⑦", "7")
                .Replace("⑧", "8")
                .Replace("⑨", "9")
                ;
            }
        }

        //Detect languages
        //source: https://stackoverflow.com/questions/15805859/detect-japanese-character-input-and-romajis-ascii
        public static IEnumerable<char> GetCharsInRange(string searchKeyword, int min, int max)
        {
            //Usage:
            //var romaji = GetCharsInRange(searchKeyword, 0x0020, 0x007E);
            //var hiragana = GetCharsInRange(searchKeyword, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(searchKeyword, 0x30A0, 0x30FF);
            //var kanji = GetCharsInRange(searchKeyword, 0x4E00, 0x9FBF);

            return searchKeyword.Where(e => e >= min && e <= max);
        }

        public static string GetLangsOfString(string target, string langlocale)
        {
            string ret = string.Empty;
            if (langlocale == "all")
            {
                ret += T._("contains") + ": " + Environment.NewLine;
                foreach (var loc in new[] { "romaji", "kanji", "hiragana", "katakana", "other" })
                {
                    int loccnt;
                    if ((loccnt = GetLocaleLangCount(target, loc)) > 0)
                    {
                        ret += "       " + loc + ":" + loccnt + Environment.NewLine;
                    }
                }
            }
            else if (string.Compare(langlocale, "romaji", true, CultureInfo.InvariantCulture) == 0) //asdf оптимизация, сравнение с игнором регистра: http://www.vcskicks.com/optimize_csharp_code.php
            {
                return "       romaji:" + GetLocaleLangCount(target, "romaji") + Environment.NewLine;
            }
            else if (string.Compare(langlocale, "kanji", true, CultureInfo.InvariantCulture) == 0)
            {
                return "       kanji:" + GetLocaleLangCount(target, "kanji") + Environment.NewLine;
            }
            else if (string.Compare(langlocale, "hiragana", true, CultureInfo.InvariantCulture) == 0)
            {
                return "       hiragana:" + GetLocaleLangCount(target, "hiragana") + Environment.NewLine;
            }
            else if (string.Compare(langlocale, "katakana", true, CultureInfo.InvariantCulture) == 0)
            {
                return "       katakana:" + GetLocaleLangCount(target, "katakana") + Environment.NewLine;
            }
            else if (string.Compare(langlocale, "other", true, CultureInfo.InvariantCulture) == 0)
            {
                return "       other:" + GetLocaleLangCount(target, "other") + Environment.NewLine;
            }

            return ret;
        }

        public static int GetLocaleLangCount(string text, string langlocale = "romaji")
        {
            //NOTICE: "\n" will calculate all romaji and \ will be romaji

            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int romaji = GetCharsInRange(text, 0x0020, 0x007E).Count();
            int kanji = GetCharsInRange(text, 0x4E00, 0x9FBF).Count();
            int hiragana = GetCharsInRange(text, 0x3040, 0x309F).Count();
            int katakana = GetCharsInRange(text, 0x30A0, 0x30FF).Count();

            int allrkhk = romaji + kanji + hiragana + katakana;
            if (string.Compare(langlocale, "all", true, CultureInfo.InvariantCulture) == 0)
            {
                return allrkhk + (text.Length - allrkhk);
            }
            else if (string.Compare(langlocale, "romaji", true, CultureInfo.InvariantCulture) == 0)
            {
                return romaji;
            }
            else if (string.Compare(langlocale, "kanji", true, CultureInfo.InvariantCulture) == 0)
            {
                return kanji;
            }
            else if (string.Compare(langlocale, "hiragana", true, CultureInfo.InvariantCulture) == 0)
            {
                return hiragana;
            }
            else if (string.Compare(langlocale, "katakana", true, CultureInfo.InvariantCulture) == 0)
            {
                return katakana;
            }
            else if (string.Compare(langlocale, "other", true, CultureInfo.InvariantCulture) == 0)
            {
                return text.Length - allrkhk;
            }

            return allrkhk;
        }
    }
}

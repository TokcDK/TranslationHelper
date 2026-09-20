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
        /// Character counts of every locale a string can be made of. Computed in one pass so the
        /// callers below no longer rescan the same string once per locale.
        /// </summary>
        private readonly struct LocaleCharCounts
        {
            internal readonly int Romaji;
            internal readonly int Kanji;
            internal readonly int Hiragana;
            internal readonly int Katakana;

            internal LocaleCharCounts(int romaji, int kanji, int hiragana, int katakana)
            {
                Romaji = romaji;
                Kanji = kanji;
                Hiragana = hiragana;
                Katakana = katakana;
            }

            internal int AllLocaleChars => Romaji + Kanji + Hiragana + Katakana;
        }

        /// <summary>
        /// Counts every locale of <paramref name="text"/> in a single pass.
        /// </summary>
        private static LocaleCharCounts CountLocaleChars(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new LocaleCharCounts(0, 0, 0, 0);
            }

            int romaji = 0, kanji = 0, hiragana = 0, katakana = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= 0x0020 && c <= 0x007E) romaji++;
                else if (c >= 0x4E00 && c <= 0x9FBF) kanji++;
                else if (c >= 0x3040 && c <= 0x309F) hiragana++;
                else if (c >= 0x30A0 && c <= 0x30FF) katakana++;
            }

            return new LocaleCharCounts(romaji, kanji, hiragana, katakana);
        }

        /// <summary>
        /// Case insensitive locale name test. Used everywhere so the same locale name is accepted
        /// no matter which method receives it.
        /// </summary>
        private static bool IsLocale(string langlocale, string locale)
        {
            return string.Compare(langlocale, locale, true, CultureInfo.InvariantCulture) == 0;
        }

        /// <summary>
        /// Count of the characters of <paramref name="langlocale"/> in <paramref name="counts"/>.
        /// </summary>
        /// <param name="allCount">Length of the analysed text, which is what the "all" locale counts.</param>
        private static int CountOfLocale(LocaleCharCounts counts, string langlocale, int allCount)
        {
            if (IsLocale(langlocale, "all")) return allCount;
            if (IsLocale(langlocale, "romaji")) return counts.Romaji;
            if (IsLocale(langlocale, "kanji")) return counts.Kanji;
            if (IsLocale(langlocale, "hiragana")) return counts.Hiragana;
            if (IsLocale(langlocale, "katakana")) return counts.Katakana;
            if (IsLocale(langlocale, "other")) return allCount - counts.AllLocaleChars;

            return counts.AllLocaleChars;
        }

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

                    if (Percent >= 100 || Percent < 1)
                    {
                        return false;
                    }

                    var counts = CountLocaleChars(target);
                    int allCount = target.Length;//the "all" locale counts every character of the text

                    if (IsLocale(langlocale, "romaji"))
                    {
                        if (load && !AppSettings.DontLoadStringIfRomajiPercent)
                        {
                            return false;
                        }

                        return (CountOfLocale(counts, langlocale, allCount) * 100 / allCount) > Percent;
                    }
                    else if (IsLocale(langlocale, "other"))
                    {
                        return (CountOfLocale(counts, langlocale, allCount) * 100 / allCount) > Percent;
                    }
                    else
                    {
                        Percent = 100 - Percent;//recalculate value for correct comprasion

                        return (CountOfLocale(counts, langlocale, allCount) * 100 / allCount) < Percent;
                    }
                }
            }
            catch
            {
                //A malformed text must not break the caller, the row is simply kept.
            }
            return false;
        }

        public static bool HasNOJPcharacters(string str)
        {
            var counts = CountLocaleChars(str);
            return counts.Kanji < 1 && counts.Katakana < 1 && counts.Hiragana < 1;
        }

        /// <summary>
        /// Замена японских(и не только) цифр на стандартные
        /// </summary>
        public static string ReplaceDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            //Single pass equivalent of the previous chain of 19 string.Replace calls: every
            //full-width digit and every circled digit is mapped to its ASCII digit.
            char[] replaced = null;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                char digit;
                if (c >= '０' && c <= '９')
                {
                    digit = (char)('0' + (c - '０'));
                }
                else if (c >= '①' && c <= '⑨')
                {
                    digit = (char)('1' + (c - '①'));
                }
                else
                {
                    continue;
                }

                if (replaced == null)
                {
                    replaced = input.ToCharArray();
                }

                replaced[i] = digit;
            }

            //Nothing to replace returns the original instance, exactly like string.Replace does.
            return replaced == null ? input : new string(replaced);
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
            var counts = CountLocaleChars(target);
            int allCount = target == null ? 0 : target.Length;

            if (IsLocale(langlocale, "all"))
            {
                string ret = T._("contains") + ": " + Environment.NewLine;
                foreach (var loc in new[] { "romaji", "kanji", "hiragana", "katakana", "other" })
                {
                    int loccnt;
                    if ((loccnt = CountOfLocale(counts, loc, allCount)) > 0)
                    {
                        ret += "       " + loc + ":" + loccnt + Environment.NewLine;
                    }
                }

                return ret;
            }

            if (IsLocale(langlocale, "romaji")) return "       romaji:" + counts.Romaji + Environment.NewLine;
            if (IsLocale(langlocale, "kanji")) return "       kanji:" + counts.Kanji + Environment.NewLine;
            if (IsLocale(langlocale, "hiragana")) return "       hiragana:" + counts.Hiragana + Environment.NewLine;
            if (IsLocale(langlocale, "katakana")) return "       katakana:" + counts.Katakana + Environment.NewLine;
            if (IsLocale(langlocale, "other")) return "       other:" + (allCount - counts.AllLocaleChars) + Environment.NewLine;

            return string.Empty;
        }

        public static int GetLocaleLangCount(string text, string langlocale = "romaji")
        {
            //NOTICE: "\n" will calculate all romaji and \ will be romaji

            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            return CountOfLocale(CountLocaleChars(text), langlocale, text.Length);
        }
    }
}

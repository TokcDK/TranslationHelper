using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class EnQuotesToJp2 : RowBase
    {
        public EnQuotesToJp2()
        {
            if (AppData.ENQuotesToJPLearnDataFoundPrev == null)
            {
                AppData.ENQuotesToJPLearnDataFoundPrev = new System.Collections.Generic.Dictionary<char, int>();
            }
            if (AppData.ENQuotesToJPLearnDataFoundNext == null)
            {
                AppData.ENQuotesToJPLearnDataFoundNext = new System.Collections.Generic.Dictionary<char, int>();
            }
        }


        bool IsNotJPLang = false;
        bool IsLangChecked = false;
        protected override bool IsValidRow()
        {
            if (IsNotJPLang || !base.IsValidRow()) return false;

            if (!IsLangChecked)
            {
                IsLangChecked = true;

                if (!IsNotJPLang && AppData.Settings.SourceLanguageComboBox.SelectedText == "\"Japanese ja\"")
                {
                    IsNotJPLang = true;
                    return false;
                }
            }

            return true;
        }

        protected override bool Apply()
        {
            var o = Original;
            var changed = ReplaceQuotesInTranslation(Original, Translation);
            if (changed == o) return false;

            Translation = changed;

            return true;
        }

        public static string ReplaceQuotesInTranslation(string japaneseText, string translatedText)
        {
            // Создаем массив из японских кавычек
            char[] japaneseQuotes = new char[] { '「', '」' };

            // Выбираем соответствующие японские кавычки на основе наличия японского текста
            if (japaneseText.Contains("『") && japaneseText.Contains("』"))
            {
                japaneseQuotes = new char[] { '『', '』' };
            }

            // Проходимся по каждому символу в переводе и заменяем латинские кавычки на японские
            bool isInsideQuote = false;
            char currentQuote = '\0';
            for (int i = 0; i < translatedText.Length; i++)
            {
                if (translatedText[i] == '\"')
                {
                    // Если внутри кавычек уже есть другие кавычки, то просто игнорируем текущую латинскую кавычку
                    if (isInsideQuote && currentQuote != '\"')
                    {
                        continue;
                    }

                    // Определяем, какая кавычка нужна (открывающая или закрывающая) на основе правил использования японских кавычек
                    char japaneseQuote = isInsideQuote ? japaneseQuotes[1] : japaneseQuotes[0];

                    // Заменяем латинскую кавычку на японскую
                    translatedText = translatedText.Remove(i, 1).Insert(i, japaneseQuote.ToString());

                    // Инвертируем флаг, указывающий, находится ли курсор внутри японских кавычек
                    isInsideQuote = !isInsideQuote;
                    currentQuote = japaneseQuote;
                }
                else if (isInsideQuote && translatedText[i] == currentQuote)
                {
                    // Если внутри кавычек уже есть другие кавычки, то сбрасываем текущие кавычки
                    currentQuote = '\0';
                    isInsideQuote = false;
                }
            }

            return translatedText;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class EnQuotesToJp : RowBase
    {
        public EnQuotesToJp()
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

        protected override void ActionsInit()
        {
            PreLearn();
        }
        protected override void ActionsFinalize()
        {
            PostLearn();
        }

        static void PreLearn()
        {
        }
        protected void PostLearn()
        {
        }

        protected override bool Apply()
        {
            return ChangeQuotes1();
        }

        readonly HashSet<char> jpQuotes = new HashSet<char>
        {
            '「',
            '」',
            '『',
            '』',
            '.',
            ',',
            '!',
            '?'
        };

        private bool ChangeQuotes1()
        {
            try
            {
                var translation = Translation;
                if (translation.Length == 0) return false;

                if (
                    !translation.Contains("\"")
                    && !translation.Contains("''")
                    && !translation.Contains("“")
                    && !translation.Contains("”")
                    ) return false;

                string originalValue = Original;

                var frontQuote = string.Empty;
                var backQuote = string.Empty;
                var isNoQuotes = false;
                if (originalValue.Contains("「") || originalValue.Contains("」"))
                {
                    frontQuote = "「";
                    backQuote = "」";
                }
                else if (originalValue.Contains("『") || originalValue.Contains("』"))
                {
                    frontQuote = "『";
                    backQuote = "』";
                }
                else // no jp quotes in original
                {
                    if (originalValue.Contains("\"")) return false; // skip when original must contains standard quote

                    isNoQuotes = true; 
                }

                var mc = Regex.Matches(translation, "(\"|('')|“|”)");
                var maxInd = translation.Length - 1;

                var result = translation;
                var max = mc.Count - 1;
                bool changed = false;
                for (int i = max; i >= 0; i--)
                {
                    var val = mc[i];
                    int ind = val.Index;
                    int valLength = val.Length;

                    if (isNoQuotes)
                    {
                        result = result.Remove(ind, valLength).Insert(ind, string.Empty);
                        changed = true;
                        continue;
                    }

                    char nextchar;
                    char prevchar;
                    bool found = false;

                    if (ind > 0)
                    {
                        if (ind - 1 + valLength == maxInd) // when next char is last is out of translation length
                        {
                            found = true;
                        }
                        else if (
                            (nextchar = translation[ind + valLength]) == ' ' 
                            || nextchar == '”' 
                            || jpQuotes.Contains(nextchar) 
                            || (nextchar != '-' && translation[ind - 1] != frontQuote[0] && char.IsPunctuation(nextchar)) 
                            || char.IsWhiteSpace(nextchar) 
                            || char.IsControl(nextchar) 
                            || (char.IsLetterOrDigit(translation[ind - 1]) && char.IsLetterOrDigit(nextchar) && i > 0))
                        {
                            //RiseChar(nextchar, false);
                            found = true;
                        }
                    }
                    if (found)
                    {
                        result = result.Remove(ind, valLength).Insert(ind, backQuote);
                        changed = true;
                    }
                    else
                    {
                        if (ind - 1 + valLength < maxInd)
                        {
                            try
                            {
                                if (ind == 0)
                                {
                                    found = true;
                                }
                                else if (
                                    (prevchar = translation[ind - 1]) == ' ' 
                                    || prevchar == '“' 
                                    || jpQuotes.Contains(prevchar) 
                                    || char.IsPunctuation(prevchar) 
                                    || char.IsWhiteSpace(prevchar) 
                                    || char.IsControl(prevchar) 
                                    || char.IsLetterOrDigit(translation[ind + valLength]))
                                {
                                    //RiseChar(prevchar);
                                    found = true;
                                }
                            }
                            catch { }
                        }

                        if (found)
                        {
                            result = result.Remove(ind, val.Length).Insert(ind, frontQuote);
                            changed = true;
                        }
                    }
                }

                if(changed) Translation = result;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

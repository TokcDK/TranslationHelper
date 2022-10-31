using System;
using System.Collections;
using System.Collections.Generic;
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
                var origTranslation = SelectedRow[ColumnIndexTranslation] + "";
                if (origTranslation.Length == 0) return false;

                if (
                    !origTranslation.Contains("\"")
                    && !origTranslation.Contains("''")
                    && !origTranslation.Contains("“")
                    && !origTranslation.Contains("”")
                    ) return false;

                string originalValue = SelectedRow[ColumnIndexOriginal] as string;
                if (string.Equals(origTranslation, originalValue)) return false;

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
                else // no jp qotes in original
                {
                    isNoQuotes = !originalValue.Contains("\""); // original must not contain quote
                }

                var mc = Regex.Matches(origTranslation, "(\"|('')|“|”)");
                var maxInd = origTranslation.Length - 1;

                var result = origTranslation;
                var mcCount = mc.Count;
                for (int i = mcCount - 1; i >= 0; i--)
                {
                    var val = mc[i];
                    int ind = val.Index;
                    int valLength = val.Length;

                    if (isNoQuotes)
                    {
                        result = result.Remove(ind, valLength).Insert(ind, string.Empty);
                        continue;
                    }

                    char nextchar;
                    char prevchar;
                    bool found = false;
                    if (ind > 0)
                    {
                        if (ind - 1 + valLength == maxInd)
                        {
                            found = true;
                        }
                        else if ((nextchar = origTranslation[ind + valLength]) == ' ' || nextchar == '”' || jpQuotes.Contains(nextchar) || (nextchar != '-' && origTranslation[ind - 1] != frontQuote[0] && char.IsPunctuation(nextchar)) || char.IsWhiteSpace(nextchar) || char.IsControl(nextchar) || (char.IsLetterOrDigit(origTranslation[ind - 1]) && char.IsLetterOrDigit(nextchar) && i != 0))
                        {
                            //RiseChar(nextchar, false);
                            found = true;
                        }
                    }
                    if (found) result = result.Remove(ind, valLength).Insert(ind, backQuote);
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
                                else if ((prevchar = origTranslation[ind - 1]) == ' ' || prevchar == '“' || jpQuotes.Contains(prevchar) || char.IsPunctuation(prevchar) || char.IsWhiteSpace(prevchar) || char.IsControl(prevchar) || char.IsLetterOrDigit(origTranslation[ind + valLength]))
                                {
                                    //RiseChar(prevchar);
                                    found = true;
                                }
                            }
                            catch { }
                        }

                        if (found) result = result.Remove(ind, val.Value.Length).Insert(ind, frontQuote);
                    }
                }

                SelectedRow[ColumnIndexTranslation] = result;
            }
            catch
            {
                return false;
            }
            return true;
        }

        //private void RiseChar(char c, bool prev = true)
        //{
        //    if (IsAll || IsTable)
        //        if (prev)
        //        {
        //            if (AppData.ENQuotesToJPLearnDataFoundPrev.ContainsKey(c))
        //            {
        //                AppData.ENQuotesToJPLearnDataFoundPrev[c]++;
        //            }
        //            else
        //            {
        //                AppData.ENQuotesToJPLearnDataFoundPrev.Add(c, 1);
        //            }
        //        }
        //        else
        //        {
        //            if (AppData.ENQuotesToJPLearnDataFoundNext.ContainsKey(c))
        //            {
        //                AppData.ENQuotesToJPLearnDataFoundNext[c]++;
        //            }
        //            else
        //            {
        //                AppData.ENQuotesToJPLearnDataFoundNext.Add(c, 1);
        //            }
        //        }

        //}
    }
}

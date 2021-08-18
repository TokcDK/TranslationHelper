using System.Text.RegularExpressions;
using System.Windows;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class EnQuotesToJp : RowBase
    {
        public EnQuotesToJp()
        {
            if (ProjectData.ENQuotesToJPLearnDataFoundPrev == null)
            {
                ProjectData.ENQuotesToJPLearnDataFoundPrev = new System.Collections.Generic.Dictionary<char, int>();
            }
            if (ProjectData.ENQuotesToJPLearnDataFoundNext == null)
            {
                ProjectData.ENQuotesToJPLearnDataFoundNext = new System.Collections.Generic.Dictionary<char, int>();
            }
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
            MessageBox.Show(
                (ProjectData.ENQuotesToJPLearnDataFoundPrev != null && ProjectData.ENQuotesToJPLearnDataFoundPrev.Keys.Count > 0 ? "Prev:\r\n" + string.Join("\r\n", ProjectData.ENQuotesToJPLearnDataFoundPrev) : "")
                + "\r\n\r\n" +
                (ProjectData.ENQuotesToJPLearnDataFoundNext != null && ProjectData.ENQuotesToJPLearnDataFoundNext.Keys.Count > 0 ? "Next:\r\n" + string.Join("\r\n", ProjectData.ENQuotesToJPLearnDataFoundNext) : "")
                );
        }

        protected override bool Apply()
        {
            try
            {
                var origTranslation = SelectedRow[ColumnIndexTranslation] + "";
                if (origTranslation.Length == 0)
                {
                    return false;
                }

                if (!Regex.IsMatch(origTranslation, "(\"|('')|“|”)"))
                {
                    return false;
                }

                var frontQuote = string.Empty;
                var backQuote = string.Empty;
                string originalValue;
                var isNoQuotes = false;
                if ((originalValue = SelectedRow[ColumnIndexOriginal] as string).Contains("「") || originalValue.Contains("」"))
                {
                    frontQuote = "「";
                    backQuote = "」";
                }
                else if (originalValue.Contains("『") || originalValue.Contains("』"))
                {
                    frontQuote = "『";
                    backQuote = "』";
                }
                else
                {
                    isNoQuotes = true;
                }

                var mc = Regex.Matches(origTranslation, "(\"|('')|“|”)");
                var maxInd = origTranslation.Length - 1;
                System.Collections.Generic.HashSet<char> jpQuotes = new System.Collections.Generic.HashSet<char>
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

                var result = origTranslation;
                var mcCount = mc.Count;
                for (int i = mcCount - 1; i >= 0; i--)
                {
                    int ind = mc[i].Index;

                    if (isNoQuotes)
                    {
                        result = result.Remove(ind, mc[i].Value.Length).Insert(ind, string.Empty);
                        continue;
                    }

                    char nextchar;
                    char prevchar;
                    bool found = false;
                    //if (ind > 0 && (ind - 1 + ind == maxInd || (nextchar = origTranslation[ind + mc[i].Length]) == ' ' || nextchar == '”' || JPQuotes.Contains(nextchar) || char.IsPunctuation(nextchar) || char.IsWhiteSpace(nextchar) || char.IsControl(nextchar)))
                    if (ind > 0)
                    {
                        if (ind - 1 + mc[i].Length == maxInd)
                        {
                            found = true;
                        }
                        else if ((nextchar = origTranslation[ind + mc[i].Length]) == ' ' || nextchar == '”' || jpQuotes.Contains(nextchar) || (nextchar != '-' && origTranslation[ind - 1] != frontQuote[0] && char.IsPunctuation(nextchar)) || char.IsWhiteSpace(nextchar) || char.IsControl(nextchar) || (char.IsLetterOrDigit(origTranslation[ind - 1]) && char.IsLetterOrDigit(nextchar) && i != 0))
                        {
                            RiseChar(nextchar, false);
                            found = true;
                        }
                    }
                    if (found)
                    {
                        result = result.Remove(ind, mc[i].Value.Length).Insert(ind, backQuote);
                    }
                    //if (ind < maxInd && (ind == 0 || (prevchar = origTranslation[ind - 1]) == ' ' || prevchar == '“' || JPQuotes.Contains(prevchar) || char.IsPunctuation(prevchar) || char.IsWhiteSpace(prevchar) || char.IsControl(prevchar)))
                    else
                    {
                        if (ind - 1 + mc[i].Length < maxInd)
                        {
                            try
                            {
                                if (ind == 0)
                                {
                                    found = true;
                                }
                                else if ((prevchar = origTranslation[ind - 1]) == ' ' || prevchar == '“' || jpQuotes.Contains(prevchar) || char.IsPunctuation(prevchar) || char.IsWhiteSpace(prevchar) || char.IsControl(prevchar) || char.IsLetterOrDigit(origTranslation[ind + mc[i].Length]))
                                {
                                    RiseChar(prevchar);
                                    found = true;
                                }
                            }
                            catch
                            {

                            }
                        }
                        if (found)
                        {
                            result = result.Remove(ind, mc[i].Value.Length).Insert(ind, frontQuote);
                        }
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

        private void RiseChar(char c, bool prev = true)
        {
            if (IsAll || IsTable)
                if (prev)
                {
                    if (ProjectData.ENQuotesToJPLearnDataFoundPrev.ContainsKey(c))
                    {
                        ProjectData.ENQuotesToJPLearnDataFoundPrev[c]++;
                    }
                    else
                    {
                        ProjectData.ENQuotesToJPLearnDataFoundPrev.Add(c, 1);
                    }
                }
                else
                {
                    if (ProjectData.ENQuotesToJPLearnDataFoundNext.ContainsKey(c))
                    {
                        ProjectData.ENQuotesToJPLearnDataFoundNext[c]++;
                    }
                    else
                    {
                        ProjectData.ENQuotesToJPLearnDataFoundNext.Add(c, 1);
                    }
                }

        }
    }
}

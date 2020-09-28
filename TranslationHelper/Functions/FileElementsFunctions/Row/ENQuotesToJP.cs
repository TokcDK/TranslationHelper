using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class ENQuotesToJP : FileElementsRowFunctionsBase
    {
        public ENQuotesToJP(THDataWork thDataWork) : base(thDataWork)
        {
            if (thDataWork.ENQuotesToJPLearnDataFoundPrev == null)
            {
                thDataWork.ENQuotesToJPLearnDataFoundPrev = new System.Collections.Generic.Dictionary<char, int>();
            }
            if (thDataWork.ENQuotesToJPLearnDataFoundNext == null)
            {
                thDataWork.ENQuotesToJPLearnDataFoundNext = new System.Collections.Generic.Dictionary<char, int>();
            }
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

                var FrontQuote = string.Empty;
                var BackQuote = string.Empty;
                string originalValue;
                if ((originalValue=SelectedRow[ColumnIndexOriginal] as string).Contains("「") || originalValue.Contains("」"))
                {
                    FrontQuote = "「";
                    BackQuote = "」";
                }
                else if (originalValue.Contains("『") || originalValue.Contains("』"))
                {
                    FrontQuote = "『";
                    BackQuote = "』";
                }
                //else
                //{
                //    return false;
                //}

                var mc = Regex.Matches(origTranslation, "(\"|('')|“|”)");
                var maxInd = origTranslation.Length - 1;
                System.Collections.Generic.HashSet<char> JPQuotes = new System.Collections.Generic.HashSet<char>
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
                for (int i = mc.Count - 1; i >= 0; i--)
                {
                    int Ind = mc[i].Index;
                    char nextchar;
                    char prevchar;
                    bool found = false;
                    //if (ind > 0 && (ind - 1 + ind == maxInd || (nextchar = origTranslation[ind + mc[i].Length]) == ' ' || nextchar == '”' || JPQuotes.Contains(nextchar) || char.IsPunctuation(nextchar) || char.IsWhiteSpace(nextchar) || char.IsControl(nextchar)))
                    if (Ind > 0)
                    {
                        if (Ind - 1 + Ind == maxInd)
                        {
                            found = true;
                        }
                        else if ((nextchar = origTranslation[Ind + mc[i].Length]) == ' ' || nextchar == '”' || JPQuotes.Contains(nextchar) || (nextchar != '-' && origTranslation[Ind - 1]!=FrontQuote[0] && char.IsPunctuation(nextchar)) || char.IsWhiteSpace(nextchar) || char.IsControl(nextchar) || char.IsLetterOrDigit(origTranslation[Ind - 1]))
                        {
                            RiseChar(nextchar, false);
                            found = true;
                        }
                    }
                    if (found)
                    {
                        result = result.Remove(Ind, mc[i].Value.Length).Insert(Ind, BackQuote);
                    }
                    //if (ind < maxInd && (ind == 0 || (prevchar = origTranslation[ind - 1]) == ' ' || prevchar == '“' || JPQuotes.Contains(prevchar) || char.IsPunctuation(prevchar) || char.IsWhiteSpace(prevchar) || char.IsControl(prevchar)))
                    else
                    {
                        if (Ind - 1 + mc[i].Length < maxInd)
                        {
                            if (Ind == 0)
                            {
                                found = true;
                            }
                            else if ((prevchar = origTranslation[Ind - 1]) == ' ' || prevchar == '“' || JPQuotes.Contains(prevchar) || char.IsPunctuation(prevchar) || char.IsWhiteSpace(prevchar) || char.IsControl(prevchar) || char.IsLetterOrDigit(origTranslation[Ind + mc[i].Length]))
                            {
                                RiseChar(prevchar);
                                found = true;
                            }
                        }
                        if (found)
                        {
                            result = result.Remove(Ind, mc[i].Value.Length).Insert(Ind, FrontQuote);
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
                    if (thDataWork.ENQuotesToJPLearnDataFoundPrev.ContainsKey(c))
                    {
                        thDataWork.ENQuotesToJPLearnDataFoundPrev[c]++;
                    }
                    else
                    {
                        thDataWork.ENQuotesToJPLearnDataFoundPrev.Add(c, 1);
                    }
                }
                else
                {
                    if (thDataWork.ENQuotesToJPLearnDataFoundNext.ContainsKey(c))
                    {
                        thDataWork.ENQuotesToJPLearnDataFoundNext[c]++;
                    }
                    else
                    {
                        thDataWork.ENQuotesToJPLearnDataFoundNext.Add(c, 1);
                    }
                }

        }
    }
}

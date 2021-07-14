using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixENJPQuoteOnStringStart2ndLine : HardFixesBase
    {
        public FixENJPQuoteOnStringStart2ndLine() : base()
        {
        }

        //Fix 1
        /////////////////////////////////	
        //
        //「……くっ……。
        //　いったい何をしてるんだ。わたしは……。
        /////////////////////////////////	
        // 
        //“…………….
        //　What are you doing? I…….
        /////////////////////////////////	
        protected override bool Apply()
        {
            try
            {
                var OriginalValue = SelectedRow[ColumnIndexOriginal] as string;
                if (OriginalValue.IsMultiline())
                {
                    string origSecondLine = string.Empty;
                    int origSecondlineIndex = 0;
                    try
                    {
                        foreach (var line in OriginalValue.SplitToLines())
                        {
                            if (origSecondlineIndex == 0)
                            {
                                origSecondlineIndex++;
                                continue;
                            }
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                origSecondLine = line;
                                break;
                            }
                            origSecondlineIndex++;
                        }
                    }
                    catch
                    {
                        return false;
                    }

                    bool quote1 = false;
                    bool quote2 = false;
                    if ((quote1 = origSecondLine.StartsWith("「")) || (quote2 = origSecondLine.StartsWith("『")))
                    {
                        bool endsWith = false;

                        var TranslationValue = SelectedRow[ColumnIndexTranslation] + "";
                        if (TranslationValue.IsMultiline())
                        {
                            string quoteString;
                            bool StartsWithJpQuote1 = false;
                            bool StartsWithJpQuote2 = false;
                            string secondline = string.Empty;
                            int secondlineIndex = 0;
                            try
                            {
                                foreach (var line in TranslationValue.SplitToLines())
                                {
                                    if (secondlineIndex == 0)
                                    {
                                        secondlineIndex++;
                                        continue;
                                    }
                                    if (!string.IsNullOrWhiteSpace(line))
                                    {
                                        secondline = line;
                                        break;
                                    }
                                    secondlineIndex++;
                                }
                            }
                            catch
                            {
                                return false;
                            }

                            string StartQuoteStringEN = string.Empty;
                            string EndQuoteStringEN = string.Empty;

                            if (secondline.StartsWith("''"))
                            {
                                StartQuoteStringEN = "''";
                            }
                            else if (secondline.StartsWith("'"))
                            {
                                StartQuoteStringEN = "'";
                            }
                            else if (secondline.StartsWith("“"))
                            {
                                StartQuoteStringEN = "“";
                            }
                            else if (secondline.StartsWith("\""))
                            {
                                StartQuoteStringEN = "\"";
                            }
                            else if (secondline.StartsWith("「"))
                            {
                                StartsWithJpQuote1 = true;
                            }
                            else if (secondline.StartsWith("『"))
                            {
                                StartsWithJpQuote2 = true;
                            }

                            if (TranslationValue.EndsWith("''"))
                            {
                                EndQuoteStringEN = "''";
                            }
                            else if (TranslationValue.EndsWith("'"))
                            {
                                EndQuoteStringEN = "'";
                            }
                            else if (TranslationValue.EndsWith("“"))
                            {
                                EndQuoteStringEN = "“";
                            }
                            else if (TranslationValue.EndsWith("\""))
                            {
                                EndQuoteStringEN = "\"";
                            }


                            if (StartQuoteStringEN.Length > 0 || EndQuoteStringEN.Length > 0)
                            {
                                if (quote1)
                                {
                                    quoteString = "「";
                                }
                                else if (quote2)
                                {
                                    quoteString = "『";
                                }
                                else
                                {
                                    return false;
                                }

                                int EndQuoteStringENLength = EndQuoteStringEN.Length;
                                endsWith = EndQuoteStringENLength > 0;

                                string resultString = string.Empty;
                                int ind = 0;

                                if (StartQuoteStringEN.Length > 0 || (!StartsWithJpQuote1 && !StartsWithJpQuote2))
                                {
                                    foreach (string line in TranslationValue.SplitToLines())
                                    {
                                        //new line for multiline
                                        if (ind > 0)
                                        {
                                            resultString += Environment.NewLine;
                                        }

                                        if (ind != secondlineIndex)
                                        {
                                            resultString += line;
                                        }
                                        else
                                        {
                                            int lineLength = line.Length;
                                            int StartQuoteStringENLength = StartQuoteStringEN.Length;
                                            if (lineLength > 1 && StartQuoteStringENLength > 0 && line.StartsWith(StartQuoteStringEN))
                                            {
                                                resultString += quoteString + line.Remove(0, StartQuoteStringENLength);
                                            }
                                            else if (lineLength == 0 || (lineLength == 1 && StartQuoteStringENLength > 0 && line == StartQuoteStringEN))
                                            {
                                                resultString += quoteString;
                                            }
                                            else if (lineLength > 0)
                                            {
                                                resultString += quoteString + line;
                                            }
                                            else
                                            {
                                                resultString += line;
                                            }
                                        }
                                        ind++;
                                    }
                                }
                                else
                                {
                                    resultString = TranslationValue;
                                }

                                string EndQuoteString = (quote1 ? "」" : "』");
                                resultString = resultString.TrimEnd();
                                if (OriginalValue.EndsWith(EndQuoteString) && !resultString.EndsWith(EndQuoteString))
                                {
                                    resultString = (endsWith ? resultString.Remove(resultString.Length - EndQuoteStringENLength, EndQuoteStringENLength) : resultString) + EndQuoteString;
                                }

                                SelectedRow[ColumnIndexTranslation] = resultString;
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return false;
        }
    }
}

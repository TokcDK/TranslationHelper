using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixEnjpQuoteOnStringStart2NdLineChanger : StringChangerBase
    {
        public FixEnjpQuoteOnStringStart2NdLineChanger()
        {
        }

        internal override string Description => $"{nameof(FixEnjpQuoteOnStringStart2NdLineChanger)}";

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
        internal override string Change(string inputString, object extraData)
        {
            try
            {
                var originalValue = extraData as string;
                if (originalValue.IsMultiline())
                {
                    string origSecondLine = string.Empty;
                    int origSecondlineIndex = 0;
                    try
                    {
                        foreach (var line in originalValue.SplitToLines())
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
                        return inputString;
                    }

                    bool quote1 = false;
                    bool quote2 = false;
                    if ((quote1 = origSecondLine.StartsWith("「")) || (quote2 = origSecondLine.StartsWith("『")))
                    {
                        bool endsWith = false;

                        var translationValue = inputString;
                        if (translationValue.IsMultiline())
                        {
                            string quoteString;
                            bool startsWithJpQuote1 = false;
                            bool startsWithJpQuote2 = false;
                            string secondline = string.Empty;
                            int secondlineIndex = 0;
                            try
                            {
                                foreach (var line in translationValue.SplitToLines())
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
                                return inputString;
                            }

                            string startQuoteStringEn = string.Empty;
                            string endQuoteStringEn = string.Empty;

                            if (secondline.StartsWith("''"))
                            {
                                startQuoteStringEn = "''";
                            }
                            else if (secondline.StartsWith("'"))
                            {
                                startQuoteStringEn = "'";
                            }
                            else if (secondline.StartsWith("“"))
                            {
                                startQuoteStringEn = "“";
                            }
                            else if (secondline.StartsWith("\""))
                            {
                                startQuoteStringEn = "\"";
                            }
                            else if (secondline.StartsWith("「"))
                            {
                                startsWithJpQuote1 = true;
                            }
                            else if (secondline.StartsWith("『"))
                            {
                                startsWithJpQuote2 = true;
                            }

                            if (translationValue.EndsWith("''"))
                            {
                                endQuoteStringEn = "''";
                            }
                            else if (translationValue.EndsWith("'"))
                            {
                                endQuoteStringEn = "'";
                            }
                            else if (translationValue.EndsWith("“"))
                            {
                                endQuoteStringEn = "“";
                            }
                            else if (translationValue.EndsWith("\""))
                            {
                                endQuoteStringEn = "\"";
                            }


                            if (startQuoteStringEn.Length > 0 || endQuoteStringEn.Length > 0)
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
                                    return inputString;
                                }

                                int endQuoteStringEnLength = endQuoteStringEn.Length;
                                endsWith = endQuoteStringEnLength > 0;

                                string resultString = string.Empty;
                                int ind = 0;

                                if (startQuoteStringEn.Length > 0 || !startsWithJpQuote1 && !startsWithJpQuote2)
                                {
                                    foreach (string line in translationValue.SplitToLines())
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
                                            int startQuoteStringEnLength = startQuoteStringEn.Length;
                                            if (lineLength > 1 && startQuoteStringEnLength > 0 && line.StartsWith(startQuoteStringEn))
                                            {
                                                resultString += quoteString + line.Remove(0, startQuoteStringEnLength);
                                            }
                                            else if (lineLength == 0 || lineLength == 1 && startQuoteStringEnLength > 0 && line == startQuoteStringEn)
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
                                    resultString = translationValue;
                                }

                                string endQuoteString = quote1 ? "」" : "』";
                                resultString = resultString.TrimEnd();
                                if (originalValue.EndsWith(endQuoteString) && !resultString.EndsWith(endQuoteString))
                                {
                                    resultString = (endsWith ? resultString.Remove(resultString.Length - endQuoteStringEnLength, endQuoteStringEnLength) : resultString) + endQuoteString;
                                }

                                return resultString;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return inputString;
        }
    }
}

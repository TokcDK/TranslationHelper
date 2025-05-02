using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.ExtractedParser;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    abstract class StringCaseMorphBase : ExtractedByTranslationRulesParserRowBase
    {
        protected StringCaseMorphBase()
        {
        }

        static string Animations { get => "Animations"; }

        /// <summary>
        /// Special translation for Animations table in rpgmaker projects.
        /// Maybe here will be better to make project specific string case morph from Project.StringCaseMorph()
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckAnims(TableData tableData)
        {
            _isAnimations = tableData.SelectedTable.TableName == Animations;
        }

        protected override void ActionsPreRowsApply(TableData tableData)
        {
            if (!IsAll && !IsTable)
            {
                CheckAnims(tableData);
            }
        }

        protected override void ActionsPreTableApply(TableData tableData)
        {
            CheckAnims(tableData);
        }

        /// <summary>
        /// 0=lowercase,1=Uppercase,2=UPPERCASE
        /// </summary>
        protected abstract VariantCase Variant { get; }

        protected enum VariantCase
        {
            /// <summary>
            /// all chars to lower case
            /// </summary>
            lower = 0,
            /// <summary>
            /// 1st char to Upper case
            /// </summary>
            Upper = 1,
            /// <summary>
            /// all chars to upper case
            /// </summary>
            UPPER = 2,
            /// <summary>
            /// 1st char to lower
            /// </summary>
            lower1st = 3,
            /// <summary>
            /// 1st char to Upper case in all lines
            /// </summary>
            UpperAllLines = 4,
        }

        /// <summary>
        /// if table name is 'Animations'
        /// </summary>
        bool _isAnimations = false;

        protected override string ActionWithExtractedTranslation(ExtractRegexValueInfo extractedOrigValueInfo, ExtractRegexValueInfo extractedTransValueInfo)
        {
            return ChangeRegistryCaseForTheCell(extractedOrigValueInfo.Original, extractedTransValueInfo.Original, Variant);
        }

        protected override string ActionWithOriginalIfNoExtracted(string original, string translation)
        {
            return ChangeRegistryCaseForTheCell(original, translation, Variant);
        }

        // move base code
        //protected override bool Apply()
        //{
        //    var orig = SelectedRow[0] as string;
        //    var trans = SelectedRow[1] + string.Empty;

        //    if (string.IsNullOrWhiteSpace(trans)) return false;

        //    var etractDataOrig = new ExtractRegexInfo(orig);
        //    var etractDataTrans = new ExtractRegexInfo(trans);
        //    var etractDataTransExtractedValuesListCount = etractDataTrans.ExtractedValuesList.Count;

        //    bool isChanged = false;
        //    if (etractDataOrig.ExtractedValuesList.Count == etractDataTransExtractedValuesListCount)
        //    {
        //        foreach(var extractedValueInfo in etractDataTrans.ExtractedValuesList)
        //        {
        //            extractedValueInfo.Translation = ChangeRegistryCaseForTheCell(extractedValueInfo.Original, Variant);

        //            if (extractedValueInfo.Translation != extractedValueInfo.Original) isChanged = true;
        //        }

        //        if (!isChanged) return false;

        //        var newValue = trans;
        //        var replacedStartIndexes = new List<int>();
        //        foreach (var info in etractDataTrans.GetByGroupIndex(isReversed: true))
        //        {
        //            if (info.Value.Translation == info.Value.Original) continue;

        //            var index = info.Key.Index;
        //            if (replacedStartIndexes.Contains(index)) continue; // this shorter group match was inside of other group with same start index

        //            newValue = newValue.Remove(index, info.Key.Length)
        //                .Insert(index, info.Value.Translation);

        //            replacedStartIndexes.Add(index);
        //        }

        //        if (newValue == trans) return false;

        //        return true;
        //    }

        //    var changedTrans = ChangeRegistryCaseForTheCell(trans, Variant);

        //    if (changedTrans == orig) return false;

        //    return true;

        //    #region old
        //    //var indexes = new List<int>();
        //    //var extractedFromTrans = trans.ExtractMulty(outIndexes: indexes);
        //    //if (extractedFromTrans.Length == indexes.Count)
        //    //{
        //    //    var result = trans;
        //    //    for (int i = extractedFromTrans.Length - 1; i >= 0; i--)
        //    //    {
        //    //        string transName = ChangeRegistryCaseForTheCell(extractedFromTrans[i], Variant);
        //    //        if (!string.IsNullOrWhiteSpace(transName) // not empty extracted value
        //    //            && extractedFromTrans[i].Trim() != transName) // not just trimmed extracted value
        //    //        {
        //    //            result = result.Remove(indexes[i], extractedFromTrans[i].Length).Insert(indexes[i], transName);
        //    //        }
        //    //    }

        //    //    if (result != trans)
        //    //    {
        //    //        SelectedRow[1] = result;
        //    //        return true;
        //    //    }
        //    //}


        //    //if (!string.IsNullOrWhiteSpace(trans)// not empty translation
        //    //    && trans != orig//not equal to original
        //    //    && (Variant != VariantCase.Upper || !trans.StartsWith("'s "))//need for states table. not starts with "'s " to prevent change of this "John's boots" to "John'S boots"
        //    //    )
        //    //{
        //    //    if (_isAnimations && Variant == VariantCase.Upper && trans.IndexOf('/') != -1)//change 'effect1/effect2' to 'Effect1/Effect2'
        //    //    {
        //    //        string[] parts = trans.Split('/');
        //    //        for (int i = 0; i < parts.Length; i++)
        //    //        {
        //    //            parts[i] = ChangeRegistryCaseForTheCell(parts[i], Variant);
        //    //        }
        //    //        SelectedRow[1] = string.Join("/", parts);
        //    //    }
        //    //    else
        //    //    {
        //    //        try
        //    //        {
        //    //            SelectedRow[1] = ChangeRegistryCaseForTheCell(trans, Variant);
        //    //        }
        //    //        catch
        //    //        {

        //    //        }
        //    //    }
        //    //}

        //    //return false;
        //    #endregion old
        //}

        /// <summary>
        /// 0=lowercase,1=Uppercase,2=UPPERCASE
        /// </summary>
        /// <param name="translationString"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        private string ChangeRegistryCaseForTheCell(string originalString, string translationString, VariantCase variant)
        {
            switch (variant)
            {
                case VariantCase.lower:
                    //lowercase
                    return translationString.ToLowerInvariant();
                case VariantCase.UpperAllLines:
                    return ToUpperAllLines(originalString, translationString);
                case VariantCase.Upper:
                    //Uppercase
                    //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                    return StringToUpper(translationString, originalString);
                case VariantCase.UPPER:
                    //UPPERCASE
                    return translationString.ToUpperInvariant();
                case VariantCase.lower1st:
                    //UPPERCASE
                    return StringToUpper(translationString, originalString, isReverse: true);
                default:
                    return translationString;
            }
        }

        private string ToUpperAllLines(string originalString, string translationString)
        {
            if(IsExtracted) return StringToUpper(translationString, originalString);

            var newLineSymbolIndex = translationString.IndexOf("\n");
            if (newLineSymbolIndex == -1)
            {
                // standart ToUpper when single line
                return StringToUpper(translationString, originalString);
            }

            string newLineSymbol = newLineSymbolIndex > 0 && translationString[newLineSymbolIndex - 1].Equals('\r') ? "\r\n" : "\n";

            string[] linesOfOriginal = originalString.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
            string[] linesOfTranslation = translationString.Split(new[] { newLineSymbol }, StringSplitOptions.None);

            if (linesOfOriginal.Length != linesOfTranslation.Length) return StringToUpper(translationString, originalString);

            for (int i = 0; i < linesOfTranslation.Length; i++)
            {
                var line = linesOfTranslation[i];

                if (string.IsNullOrWhiteSpace(line)) continue;

                // original line 1st char is equal to translation, skip
                if (line.Equals(linesOfOriginal[i])) continue;

                linesOfTranslation[i] = StringToUpper(line, originalString);
            }

            return string.Join(newLineSymbol, linesOfTranslation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="isReverse">1st har change to lower instead of Upper</param>
        /// <returns></returns>
        internal string StringToUpper(string inputString, string original, bool isReverse = false)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return inputString;

            if (char.IsLetter(inputString[0]))
            {
                if (original[0] != inputString[0]) // skip if char in original equals char in translation with same index
                {
                    inputString = (isReverse ? char.ToLowerInvariant(inputString[0]) : char.ToUpperInvariant(inputString[0])) + inputString.Substring(1);
                }
            }
            else
            {
                int dsTransCellLength = inputString.Length;
                for (int c = 0; c < dsTransCellLength; c++)
                {
                    char @char = inputString[c];
                    if (IsCustomSymbol(@char) || char.IsWhiteSpace(@char) || char.IsPunctuation(@char)) continue;

                    string orig;
                    if ((c > 0 && (@char == 's' && inputString[c - 1] == '\'' || inputString[c - 1] == '\\')) // 's or \s
                        ||
                        (orig = original).Length > c && orig[c] == inputString[c]) // skip if char in original equals char in translation with same index
                    { }
                    else inputString = inputString.Substring(0, c) + (isReverse ? char.ToLowerInvariant(inputString[c]) : char.ToUpperInvariant(inputString[c])) + (c == dsTransCellLength - 1 ? string.Empty : inputString.Substring(c + 1));

                    break;
                }
            }

            if (inputString.StartsWith("[") && inputString.IsMultiline())
            {
                int lineCnt = 0;
                string resultLine = string.Empty;
                foreach (var line in inputString.SplitToLines())
                {
                    if (lineCnt == 0)
                    {
                        resultLine += line;
                    }
                    else
                    {
                        resultLine += Environment.NewLine;
                        if (lineCnt == 1)
                        {
                            resultLine += StringToUpper(line, original, isReverse);
                        }
                        else
                        {
                            resultLine += line;
                        }
                    }
                    lineCnt++;
                }
                inputString = resultLine;
            }

            // upper case of first letter after jp bracket
            foreach (Match m in Regex.Matches(inputString, "[「『][a-z]")) inputString = inputString.Remove(m.Index, 2).Insert(m.Index, m.Value.ToUpperInvariant());

            return inputString;
        }
        static bool IsCustomSymbol(char @char)
        {
            return @char == '「' || @char == '『' || @char == '"';
        }
    }
}

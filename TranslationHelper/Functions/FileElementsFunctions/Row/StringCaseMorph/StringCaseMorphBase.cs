using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    abstract class StringCaseMorphBase : RowBase
    {
        protected StringCaseMorphBase() : base()
        {
        }

        static string Animations { get => "Animations"; }

        /// <summary>
        /// Special translation for Animations table in rpgmaker projects.
        /// Maybe here will be better to make project specific string case morph from Project.StringCaseMorph()
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckAnims()
        {
            _isAnimations = SelectedTable.TableName == Animations;
        }

        protected override void ActionsPreRowsApply()
        {
            if (!IsAll && !IsTable)
            {
                CheckAnims();
            }
        }

        protected override void ActionsPreTableApply()
        {
            CheckAnims();
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
            /// 1st char to upper case
            /// </summary>
            Upper = 1,
            /// <summary>
            /// all chars to upper case
            /// </summary>
            UPPER = 2
        }

        /// <summary>
        /// if table name is 'Animations'
        /// </summary>
        bool _isAnimations = false;

        protected override bool Apply()
        {
            var orig = SelectedRow[0] as string;
            var trans = SelectedRow[1] + string.Empty;

            var indexes = new List<int>();
            var extractedFromTrans = trans.ExtractMulty(outIndexes: indexes);
            if (extractedFromTrans.Length == indexes.Count)
            {
                var result = trans;
                for (int i = extractedFromTrans.Length - 1; i >= 0; i--)
                {
                    string transName = ChangeRegistryCaseForTheCell(extractedFromTrans[i], Variant);
                    if (!string.IsNullOrWhiteSpace(transName) // not empty extracted value
                        && extractedFromTrans[i].Trim() != transName) // not just trimmed extracted value
                    {
                        result = result.Remove(indexes[i], extractedFromTrans[i].Length).Insert(indexes[i], transName);
                    }
                }

                if (result != trans)
                {
                    SelectedRow[1] = result;
                    return true;
                }
            }


            if (!string.IsNullOrWhiteSpace(trans)// not empty translation
                && trans != orig//not equal to original
                && (Variant != VariantCase.Upper || !trans.StartsWith("'s "))//need for states table. not starts with "'s " to prevent change of this "John's boots" to "John'S boots"
                )
            {
                if (_isAnimations && Variant == VariantCase.Upper && trans.IndexOf('/') != -1)//change 'effect1/effect2' to 'Effect1/Effect2'
                {
                    string[] parts = trans.Split('/');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        parts[i] = ChangeRegistryCaseForTheCell(parts[i], Variant);
                    }
                    SelectedRow[1] = string.Join("/", parts);
                }
                else
                {
                    SelectedRow[1] = ChangeRegistryCaseForTheCell(trans, Variant);
                }
            }

            return false;
        }

        /// <summary>
        /// 0=lowercase,1=Uppercase,2=UPPERCASE
        /// </summary>
        /// <param name="dsTransCell"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        private string ChangeRegistryCaseForTheCell(string dsTransCell, VariantCase variant)
        {
            switch (variant)
            {
                case VariantCase.lower:
                    //lowercase
#pragma warning disable CA1308 // Нормализуйте строки до прописных букв
                    return dsTransCell.ToLowerInvariant();
#pragma warning restore CA1308 // Нормализуйте строки до прописных букв
                case VariantCase.Upper:
                    //Uppercase
                    //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                    return StringToUpper(dsTransCell);
                case VariantCase.UPPER:
                    //UPPERCASE
                    return dsTransCell.ToUpperInvariant();
                default:
                    return dsTransCell;
            }
        }

        internal string StringToUpper(string inputString)
        {
            if (char.IsLetter(inputString[0]))
            {
                if ((SelectedRow[0] as string)[0] != inputString[0]) // skip if char in original equals char in translation with same index
                {
                    inputString = char.ToUpper(inputString[0], CultureInfo.InvariantCulture) + inputString.Substring(1);
                }
            }
            else
            {
                int dsTransCellLength = inputString.Length;
                for (int c = 0; c < dsTransCellLength; c++)
                {
                    char @char = inputString[c];
                    if (IsCustomSymbol(@char) || char.IsWhiteSpace(@char) || char.IsPunctuation(@char))
                    {
                    }
                    else
                    {
                        string orig;
                        if ((c > 0 && (@char == 's' && inputString[c - 1] == '\'' || inputString[c - 1] == '\\')) // 's or \s
                            ||
                            (orig = SelectedRow[0] as string).Length > c && orig[c] == inputString[c]) // skip if char in original equals char in translation with same index
                        {
                        }
                        else
                        {
                            inputString = inputString.Substring(0, c) + char.ToUpper(inputString[c], CultureInfo.InvariantCulture) + (c == dsTransCellLength - 1 ? string.Empty : inputString.Substring(c + 1));
                        }
                        break;
                    }
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
                            resultLine += StringToUpper(line);
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

            return inputString;
        }
        static bool IsCustomSymbol(char @char)
        {
            return @char == '「' || @char == '『' || @char == '"';
        }
    }
}

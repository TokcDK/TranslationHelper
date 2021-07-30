using System;
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
            IsAnimations = SelectedTable.TableName == Animations;
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
        protected abstract int variant { get; }

        /// <summary>
        /// if table name is 'Animations'
        /// </summary>
        bool IsAnimations = false;

        protected override bool Apply()
        {
            var DSOrigCell = SelectedRow[0] as string;
            var DSTransCell = SelectedRow[1] + string.Empty;
            if (!string.IsNullOrWhiteSpace(DSTransCell)// not empty translation
                && DSTransCell != DSOrigCell//not equal to original
                && (variant != 1 || !DSTransCell.StartsWith("'s "))//need for states table. not starts with "'s " to prevent change of this "John's boots" to "John'S boots"
                )
            {
                if (IsAnimations && variant == 1 && DSTransCell.IndexOf('/') != -1)//change 'effect1/effect2' to 'Effect1/Effect2'
                {
                    string[] parts = DSTransCell.Split('/');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        parts[i] = ChangeRegistryCaseForTheCell(parts[i], variant);
                    }
                    SelectedRow[1] = string.Join("/", parts);
                }
                else
                {
                    SelectedRow[1] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
                }
            }

            return false;
        }

        /// <summary>
        /// 0=lowercase,1=Uppercase,2=UPPERCASE
        /// </summary>
        /// <param name="DSTransCell"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        private string ChangeRegistryCaseForTheCell(string DSTransCell, int variant)
        {
            switch (variant)
            {
                case 0:
                    //lowercase
#pragma warning disable CA1308 // Нормализуйте строки до прописных букв
                    return DSTransCell.ToLowerInvariant();
#pragma warning restore CA1308 // Нормализуйте строки до прописных букв
                case 1:
                    //Uppercase
                    //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                    return StringToUpper(DSTransCell);
                case 2:
                    //UPPERCASE
                    return DSTransCell.ToUpperInvariant();
                default:
                    return DSTransCell;
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
                int DSTransCellLength = inputString.Length;
                for (int c = 0; c < DSTransCellLength; c++)
                {
                    char @char = inputString[c];
                    if (IsCustomSymbol(@char) || char.IsWhiteSpace(@char) || char.IsPunctuation(@char))
                    {
                    }
                    else
                    {
                        string orig;
                        if ((c > 0 && (@char == 's' && inputString[c - 1] == '\'' || inputString[c - 1] == '\\')) // comment
                            ||
                            (orig = SelectedRow[0] as string).Length > c && orig[c] == inputString[c]) // skip if char in original equals char in translation with same index
                        {
                        }
                        else
                        {
                            inputString = inputString.Substring(0, c) + char.ToUpper(inputString[c], CultureInfo.InvariantCulture) + (c == DSTransCellLength - 1 ? string.Empty : inputString.Substring(c + 1));
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

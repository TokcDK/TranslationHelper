﻿using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CompleteRomajiotherLines : FileElementsRowFunctionsBase
    {
        public CompleteRomajiotherLines(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            if ((SelectedRow[ColumnIndexTranslation] == null || string.IsNullOrEmpty(SelectedRow[ColumnIndexTranslation] as string) || !Equals(SelectedRow[ColumnIndexTranslation], SelectedRow[ColumnIndexOriginal])) && (SelectedRow[ColumnIndexOriginal] as string).HaveMostOfRomajiOtherChars())
            {
                SelectedRow[ColumnIndexTranslation] = SelectedRow[ColumnIndexOriginal];
                return true;
            }
            return false;
        }
    }
}
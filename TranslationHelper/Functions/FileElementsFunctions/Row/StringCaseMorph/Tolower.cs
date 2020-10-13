﻿using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class Tolower : StringCaseMorphBase
    {
        public Tolower(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";

            if (translation.Length == 0 || Equals(SelectedRow[ColumnIndexOriginal], SelectedRow[ColumnIndexTranslation]))
                return false;

            try
            {
                SelectedRow[ColumnIndexTranslation] = translation.ToLowerInvariant();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
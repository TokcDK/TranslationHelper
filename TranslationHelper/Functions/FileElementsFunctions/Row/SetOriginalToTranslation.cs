﻿using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SetOriginalToTranslation : FileElementsRowFunctionsBase
    {
        public SetOriginalToTranslation(THDataWork thDataWork) : base(thDataWork)
        {
        }
        protected override bool Apply()
        {
            try
            {
                SelectedRow[ColumnIndexTranslation] = SelectedRow[ColumnIndexOriginal];
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
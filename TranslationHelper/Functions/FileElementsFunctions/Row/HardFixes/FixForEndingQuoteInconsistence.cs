﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForEndingQuoteInconsistence : HardFixesBase
    {
        public FixForEndingQuoteInconsistence(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";
            var original = SelectedRow[ColumnIndexOriginal] as string;
            if (translation[translation.Length - 1] == '"' && original[original.Length - 1] != '"')
            {
                SelectedRow[ColumnIndexTranslation] = translation.Remove(translation.Length - 1, 1) + original[original.Length - 1];
                return true;
            }
            return false;
        }
    }
}

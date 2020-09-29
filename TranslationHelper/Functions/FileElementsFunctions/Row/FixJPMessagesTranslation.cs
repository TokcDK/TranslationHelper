using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class FixJPMessagesTranslation : FileElementsRowFunctionsBase
    {
        public FixJPMessagesTranslation(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            var strOriginal = SelectedRow[ColumnIndexOriginal] as string;
            var strTranslation = SelectedRow[ColumnIndexTranslation] + string.Empty;
            if (strOriginal.StartsWith("は") && !strTranslation.StartsWith(" "))
            {
                SelectedRow[ColumnIndexTranslation] = " " + strTranslation.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + strTranslation.Substring(1);
            }
            else if (strOriginal.StartsWith("の") && !strTranslation.StartsWith("'s ") && !strTranslation.StartsWith(" "))
            {
                SelectedRow[ColumnIndexTranslation] = "'s " + strTranslation.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + strTranslation.Substring(1);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    internal class OnlineTranslateNewEmpty : OnlineTranslateNew
    {
        protected override bool IsValidRow(Row.RowBaseRowData rowData)
        {
            return !AppSettings.InterruptTtanslation && string.IsNullOrEmpty(rowData.Translation) && base.IsValidRow(rowData);
        }

        protected override bool IsTranslateAll => false;
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Translators;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class OpenInWeb : RowBase
    {
        /// <summary>
        /// open selected rows in web translator
        /// </summary>
        public OpenInWeb() : base()
        {
        }
        protected override bool IsValidRow()
        {
            return true;
        }

        readonly List<string> _rowOriginalValues = new List<string>();
        protected override bool Apply()
        {
            _rowOriginalValues.Add(SelectedRow[0] as string);//add original value

            if (SelectedRowsCountRest > 0)
            {
                return true;//return while all rows originals will be added
            }

            var text = string.Join("\r\n", _rowOriginalValues);

            //string result = Settings.THSettingsWebTransLinkTextBox.Text.Replace("{languagefrom}", "auto").Replace("{languageto}", "en").Replace("{text}", value.ToString().Replace("\r\n", "%0A").Replace("\"", "\\\string.Empty));
            //string result = string.Format(CultureInfo.InvariantCulture, Properties.Settings.Default.WebTranslationLink.Replace("{from}", "{0}").Replace("{to}", "{1}").Replace("{text}", "{2}"), TranslatorsTools.GetSourceLanguageID(), TranslatorsTools.GetTargetLanguageID(), HttpUtility.UrlEncode(value + string.Empty, Encoding.UTF8));
            //string result = string.Format(CultureInfo.InvariantCulture, Properties.Settings.Default.WebTranslationLink.Replace("{from}", "{0}").Replace("{to}", "{1}").Replace("{text}", "{2}"), TranslatorsTools.GetSourceLanguageID(), TranslatorsTools.GetTargetLanguageID(), Uri.EscapeUriString(value + string.Empty));

            var result = string.Format(CultureInfo.InvariantCulture, Properties.Settings.Default.WebTranslationLink.Replace("{from}", "{0}").Replace("{to}", "{1}").Replace("{text}", "{2}"), TranslatorsTools.GetSourceLanguageID(), TranslatorsTools.GetTargetLanguageID(), text.Replace("\r\n", "%0A")/*replace newline*/);

            Process.Start(result);

            return true;
        }
    }
}

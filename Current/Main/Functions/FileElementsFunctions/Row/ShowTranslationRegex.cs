using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    internal class ShowTranslationRegex : RowBase
    {
        readonly List<ExtractRegexInfo> _found = new List<ExtractRegexInfo>();
        int addedCount = 0;
        protected override bool Apply(RowBaseRowData rowData)
        {
            if (addedCount > 10) return false;

            var extractData = new ExtractRegexInfo(rowData.Original);
            if (extractData == null)
            {
                return false;
            }

            _found.Add(extractData);
            addedCount++;

            return true;
        }

        protected override void ActionsFinalize()
        {
            if(_found.Count > 0 )
            {
                var lineString = T._("Line");
                var patternString = T._("Pattern");
                var replacerString = T._("Replacer");

                string resultMessage = string.Join("\r\n\r\n",
                    _found.Select(el =>
                    $"{lineString}:" + el.InputString +
                    $"\n{patternString}:" + el.Pattern +
                    $"\n: {replacerString}:" + el.Replacer));

                Clipboard.Clear();
                Clipboard.SetText(resultMessage);

                MessageBox.Show(resultMessage + $"\r\n\r\n({T._("Copied to clipboard")})");
            }
            else { MessageBox.Show(T._("No matching translation regex found.."));  }
        }
    }
}

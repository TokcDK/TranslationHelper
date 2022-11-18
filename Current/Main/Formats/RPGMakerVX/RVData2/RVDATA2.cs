using System.Text;
using RPGMakerVXRVData2Assistant;

namespace TranslationHelper.Formats.RPGMakerVX.RVData2
{
    internal class RVDATA2 : FormatBinaryBase
    {
        public override string Extension => ".rvdata2";

        protected override void FileOpen()
        {
            var data = new Parser();
            foreach (var stringData in data.EnumerateStrings(FilePath))
            {
                //var text = stringData.Text;
                //if (stringData.TextEncoding != System.Text.Encoding.UTF8)
                //{
                //    text = Encoding.UTF8.GetString(stringData.Raw);
                //}

                AddRowData(stringData.Text);
            }
        }
    }
}

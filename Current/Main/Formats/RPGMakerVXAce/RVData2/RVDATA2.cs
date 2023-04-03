using System.Text;
using RPGMakerVXRVData2Assistant;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMakerVX.RVData2
{
    internal class RVDATA2 : FormatBinaryBase
    {
        public override string Extension => ".rvdata2";

        Parser _parser;

        protected override void FileOpen()
        {
            _parser = new Parser();
            foreach (var stringData in _parser.EnumerateStrings(FilePath))
            {
                //var text = stringData.Text;
                //if (stringData.TextEncoding != System.Text.Encoding.UTF8)
                //{
                //    text = Encoding.UTF8.GetString(stringData.Raw);
                //}

                string s = stringData.Text;
                if(AddRowData(ref s, stringData.Info.ToString()) && SaveFileMode)
                {
                    stringData.Text = s;
                }
            }
        }
        protected override bool TrySave()
        {
            return base.TrySave();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            return SaveFileMode // save mode
                && ParseData.Ret // something translated
                //&& ParseData.NewBinaryForWrite.Count > 0 // new bynary is not empty
                && !FunctionsFileFolder.FileInUse(GetSaveFilePath())
                && DoWriteFile(filePath);
        }

        protected override bool DoWriteFile(string filePath = "")
        {
            try
            {
                _parser.Write(FilePath);
                return true;
            }
            catch { }
            return false;
        }
    }
}

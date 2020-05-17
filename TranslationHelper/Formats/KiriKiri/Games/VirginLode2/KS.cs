using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.VirginLode2
{
    class KS : KiriKiriFormatBase
    {
        public KS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return OpenKS();
        }

        private bool OpenKS()
        {
            string line;

            string tablename = Path.GetFileName(thDataWork.FilePath);

            AddTables(tablename);

            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line.EndsWith("[待]") && line.TrimStart()[0] != ';')
                    {
                        var str = line.Replace("[待]", string.Empty).Replace("[落]", string.Empty);
                        AddRowData(str, string.Empty);
                    }
                }
            }

            return CheckTablesContent(tablename);
        }

        internal override bool Save()
        {
            return SaveKS();
        }

        private bool SaveKS()
        {
            try
            {
                string line;

                string tablename = Path.GetFileName(thDataWork.FilePath);

                using (StreamReader reader = new StreamReader(thDataWork.FilePath))
                {
                    //bool messageParse = false;
                    //bool statusInfoParse = false;
                    //bool Conditionalmessage = false;
                    //int MessageLineNum = 0;
                    //int RowIndex = 0;
                    StringBuilder Message = new StringBuilder();
                    StringBuilder MessageInfo = new StringBuilder();
                    StringBuilder ResultForWrite = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (true)
                        {
                        }

                        ResultForWrite.AppendLine(line);
                    }

                    File.WriteAllText(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "_patch", tablename), ResultForWrite.ToString());
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

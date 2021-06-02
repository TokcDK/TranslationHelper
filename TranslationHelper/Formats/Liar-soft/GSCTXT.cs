using RaiLTools;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Liar_soft
{
    class GSCTXT : FormatBase
    {
        public GSCTXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        protected override int ParseStringFileLine()
        {
            if (!ParseData.line.StartsWith("#"))
            {
                SaveModeAddLine();
                return 0;
            }

            var message = new StringBuilder();
            message.Append(ParseData.line);//add 1st line
            while ((ParseData.line=ParseData.reader.ReadLine())!=">" && ParseData.line != null)
            {
                message.AppendLine();
                message.Append(ParseData.line);
            }

            var str = message.ToString().Remove(0, 1);//remove 1st # symbol
            if (IsValidString(str))
            {
                if (thDataWork.OpenFileMode)
                {
                    AddRowData(str, "", true, false);
                }
                else if(SetTranslation(ref str))
                {
                    ParseData.Ret = true;
                }
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine("#"+str);
                ParseData.ResultForWrite.AppendLine(">");
            }

            return 0;
        }
        protected override bool ParseStringFilePostOpen()
        {
            if (thDataWork.OpenFileMode)
            {
                return CheckTablesContent(ParseData.tablename);
            }
            else
            {
                var ret = WriteFileData();
                var gscPath = thDataWork.FilePath.Remove(thDataWork.FilePath.Length - 4, 4);
                if (ret)
                {
                    var gscFile = TransFile.FromFile(thDataWork.FilePath).ToGSC(gscPath);
                    gscFile.Save(gscPath);
                }
                else
                {
                    File.Delete(gscPath);
                }
                File.Delete(thDataWork.FilePath);

                return ret;
            }
        }
    }
}

using RaiLTools;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Liar_soft
{
    class GSCTXT : StringFileFormatBase
    {
        public GSCTXT()
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!ParseData.Line.StartsWith("#"))
            {
                SaveModeAddLine();
                return 0;
            }

            var message = new StringBuilder();
            message.Append(ParseData.Line);//add 1st line
            while ((ParseData.Line=ParseData.Reader.ReadLine())!=">" && ParseData.Line != null)
            {
                message.AppendLine();
                message.Append(ParseData.Line);
            }

            var str = message.ToString().Remove(0, 1);//remove 1st # symbol
            if (IsValidString(str))
            {
                if (ProjectData.OpenFileMode)
                {
                    AddRowData(str, "", CheckInput: false);
                }
                else if(SetTranslation(ref str))
                {
                    ParseData.Ret = true;
                }
            }

            if (ProjectData.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine("#"+str);
                ParseData.ResultForWrite.AppendLine(">");
            }

            return 0;
        }
        protected override bool FilePostOpen()
        {
            if (ProjectData.OpenFileMode)
            {
                return CheckTablesContent(ParseData.TableName);
            }
            else
            {
                var ret = WriteFileData();
                var gscPath = ProjectData.FilePath.Remove(ProjectData.FilePath.Length - 4, 4);
                if (ret)
                {
                    var gscFile = TransFile.FromFile(ProjectData.FilePath).ToGSC(gscPath);
                    gscFile.Save(gscPath);
                }
                else
                {
                    File.Delete(gscPath);
                }
                File.Delete(ProjectData.FilePath);

                return ret;
            }
        }
    }
}

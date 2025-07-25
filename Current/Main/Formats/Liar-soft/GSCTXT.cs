﻿using RaiLTools;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.Liar_soft
{
    class GSCTXT : FormatStringBase
    {
        public GSCTXT(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".txt";

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
                if (OpenFileMode)
                {
                    AddRowData(str, "", isCheckInput: false);
                }
                else if(SetTranslation(ref str))
                {
                    ParseData.Ret = true;
                }
            }

            if (SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine("#"+str);
                ParseData.ResultForWrite.AppendLine(">");
            }

            return 0;
        }
        protected override bool FilePostOpen()
        {
            if (OpenFileMode)
            {
                return true;
            }
            else
            {
                var ret = WriteFileData();
                var filePath = GetSaveFilePath();
                var gscPath = filePath.Remove(filePath.Length - 4, 4);
                if (ret)
                {
                    var gscFile = TransFile.FromFile(filePath).ToGSC(gscPath);
                    gscFile.Save(gscPath);
                }
                else
                {
                    File.Delete(gscPath);
                }
                File.Delete(filePath);

                return ret;
            }
        }
    }
}

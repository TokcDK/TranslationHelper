using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats
{
    abstract class BinaryFileFormatBase : FormatBase
    {
        protected BinaryFileFormatBase()
        {
        }

        /// <summary>
        /// Parse Data
        /// </summary>
        internal ParseFileData ParseData;

        /// <summary>
        /// Open file actions
        /// </summary>
        protected override void FileOpen()
        {
            using (ParseData.fs = new FileStream(ProjectData.SelectedFilePath, FileMode.Open, FileAccess.Read))
            using (ParseData.br = new BinaryReader(ParseData.fs, DefaultEncoding()))
            {
                ParseBytes();
            }
        }

        protected virtual void ParseBytes()
        {
            while (ParseData.fs.Position < ParseData.fs.Length)
            {
                ParseData.currentbyte = ParseData.br.ReadByte();

                if (ParseByte() == KeywordActionAfter.Break)
                {
                    break;
                }
            }
        }

        protected virtual KeywordActionAfter ParseByte()
        {
            return KeywordActionAfter.Break;
        }

        /// <summary>
        /// Default encoding for file streamreader
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding DefaultEncoding()
        {
            return Encoding.UTF8;
        }

        /// <summary>
        /// Pre open file actions
        /// </summary>
        protected override bool FilePreOpenActions()
        {
            //ParseData = new ParseFileData();

            return base.FilePreOpenActions();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && ProjectData.SaveFileMode && ParseData.NewEXEBytesForWrite.Count > 0 && !FunctionsFileFolder.FileInUse(ProjectData.FilePath))
                {
                    File.WriteAllBytes(ProjectData.FilePath, ParseData.NewEXEBytesForWrite.ToArray());
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        protected override void SetTranslationIsTranslatedAction()
        {
            ParseData.Ret = true;
        }

        internal class ParseFileData
        {
            public ParseFileData()
            {
                TableName = ProjectData.CurrentProject?.CurrentFormat != null && ProjectData.CurrentProject.CurrentFormat.UseTableNameWithoutExtension
                    ? Path.GetFileNameWithoutExtension(ProjectData.FilePath)
                    : Path.GetFileName(ProjectData.FilePath);

                if (ProjectData.SaveFileMode)
                {
                }
            }

            /// <summary>
            /// tablename/filename
            /// </summary>
            internal string TableName;
            /// <summary>
            /// result of parsing. Must be set to true if any value was translated.
            /// </summary>
            internal bool Ret;
            /// <summary>
            /// line value
            /// </summary>
            internal byte currentbyte;
            internal FileStream fs;
            internal BinaryReader br;
            internal List<byte> NewEXEBytesForWrite;
        }
    }
}

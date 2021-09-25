using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats
{
    abstract class FormatBinaryBase : FormatBase
    {
        protected FormatBinaryBase()
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
            using (ParseData.FStream = new FileStream(ProjectData.SelectedFilePath, FileMode.Open, FileAccess.Read))
            using (ParseData.BReader = new BinaryReader(ParseData.FStream, DefaultEncoding()))
            {
                PreParseBytes();

                ParseBytes();

                PostParseBytesRequired();

                PostParseBytes();
            }
        }

        private void PostParseBytesRequired()
        {
            if (ProjectData.SaveFileMode)
            {
                //add rest bytes of the stream
                while (ParseData.FStream.Length > ParseData.FStream.Position)
                {
                    ParseData.NewBinaryForWrite.Add(ParseData.BReader.ReadByte());
                    //translatedbytesControlPosition++;
                }
            }
        }

        protected virtual void PostParseBytes()
        {
        }

        /// <summary>
        /// Actions before bytes will be parsed
        /// </summary>
        protected virtual void PreParseBytes()
        {
        }

        protected virtual void ParseBytes()
        {
            while (ParseBytesReadCondition())
            {
                ParseData.CurrentByte = ParseData.BReader.ReadByte();

                if (ParseByte() == KeywordActionAfter.Break)
                {
                    break;
                }
            }
        }

        protected virtual bool ParseBytesReadCondition()
        {
            return ParseData.FStream.Position < ParseData.FStream.Length - 1; // when position equals last position in the stream
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
            ParseData = new ParseFileData();

            return base.FilePreOpenActions();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ProjectData.SaveFileMode // save mode
                    && ParseData.Ret // something translated
                    && ParseData.NewBinaryForWrite.Count > 0 // new bynary is not empty
                    && !FunctionsFileFolder.FileInUse(ProjectData.FilePath) // file is not locked
                    )
                {
                    File.WriteAllBytes(ProjectData.FilePath, ParseData.NewBinaryForWrite.ToArray());
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
                TableName = ProjectData.CurrentProject?.CurrentFormat != null ? ProjectData.CurrentProject.CurrentFormat.TableName() : Path.GetFileName(ProjectData.FilePath);

                if (ProjectData.SaveFileMode)
                {
                    NewBinaryForWrite = new List<byte>();
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
            /// current byte value
            /// </summary>
            internal byte CurrentByte;
            /// <summary>
            /// file's content stream
            /// </summary>
            internal FileStream FStream;
            /// <summary>
            /// file's content stream reader
            /// </summary>
            internal BinaryReader BReader;
            /// <summary>
            /// new file content
            /// </summary>
            internal List<byte> NewBinaryForWrite;
        }
    }
}

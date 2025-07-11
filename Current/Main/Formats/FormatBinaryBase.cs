﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats
{
    abstract class FormatBinaryBase : FormatBase
    {
        /// <summary>
        /// Parse Data
        /// </summary>
        internal ParseFileData ParseData;

        protected FormatBinaryBase(ProjectBase parentProject) : base(parentProject)
        {
        }

        /// <summary>
        /// Open file actions
        /// </summary>
        protected override void FileOpen()
        {
            using (ParseData.FStream = new FileStream(AppData.SelectedProjectFilePath, FileMode.Open, FileAccess.Read))
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
            if (SaveFileMode)
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
            ParseData = new ParseFileData(this);

            return base.FilePreOpenActions();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            return SaveFileMode // save mode
                && ParseData.Ret // something translated
                && ParseData.NewBinaryForWrite.Count > 0 // new bynary is not empty
                && !FunctionsFileFolder.FileInUse(GetSaveFilePath())
                && DoWriteFile(filePath);
        }

        protected override bool DoWriteFile(string filePath = "")
        {
            try
            {
                File.WriteAllBytes(GetSaveFilePath(), GetFileBytes());
                return true;
            }
            catch
            {
            }
            return false;
        }

        protected virtual byte[] GetFileBytes()
        {
            return ParseData.NewBinaryForWrite.ToArray();
        }

        protected override void SetTranslationIsTranslatedAction()
        {
            ParseData.Ret = true;
        }

        internal class ParseFileData
        {
            public ParseFileData(FormatBase format)
            {
                if (format.SaveFileMode)
                {
                    NewBinaryForWrite = new List<byte>();
                }
            }

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

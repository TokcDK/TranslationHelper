namespace FormatBase
{
    /// <summary>
    /// base string file parser opens and writes the same file
    /// </summary>
    public abstract class FormatStringBase : IFormatString
    {
        public abstract string Extension { get; }

        public abstract string Description { get; }

        public virtual List<StringData> StringsList => new List<StringData>();

        protected bool IsOpenMode = true;

        FileInfo? SFI { get; set; }
        protected FileInfo? SourceFileInfo { get => SFI; }

        FileInfo? TFI { get; set; }
        protected FileInfo? TargetFileInfo { get => TFI; }

        public bool TryOpen(FileInfo fileInfo)
        {
            IsOpenMode = true;
            SFI = fileInfo;

            try
            {
                return ParseFile() && StringsList.Count > 0;
            }
            catch { return false; }
        }

        protected virtual bool ParseFile()
        {
            if (SourceFileInfo == null) return false;

            SR = new(SourceFileInfo.FullName);
            return ParseLines(SR);
        }

        StreamReader? SR { get; set; }
        /// <summary>
        /// StreamReader of the reading source file
        /// </summary>
        protected StreamReader? StreamReader { get => SR; }
        /// <summary>
        /// current parsing line of the reading source file
        /// </summary>
        protected string? Line { get; set; }

        protected string? ReadLine()
        {
            return Line = ReadLine();
        }

        protected virtual bool ParseLines(StreamReader sr)
        {
            while (ReadLine() != null)
            {
                try
                {
                    var retType = ParseLine();
                    if (retType == ParseLineRetType.Continue)
                    {
                        continue;
                    }
                    else if (retType == ParseLineRetType.Break)
                    {
                        break;
                    }
                    else if (retType == ParseLineRetType.Error)
                    {
                        return false;
                    }
                }
                catch { return false; }
            }

            return WriteFile();
        }

        private bool WriteFile()
        {
            if (IsOpenMode) return true;

            if (TargetFileInfo != null)
            {
                try
                {
                    File.WriteAllText(TargetFileInfo.FullName, "");
                }
                catch { return false; }
            }
            else return false;

            return true;
        }

        protected enum ParseLineRetType
        {
            Continue,
            Break,
            Error
        }

        protected virtual ParseLineRetType ParseLine()
        {
            return ParseLineRetType.Continue;
        }

        public bool TrySave(FileInfo? targetFileInfo)
        {
            IsOpenMode = false;
            TFI = targetFileInfo ?? SFI; // when target file info is set use it else use source file infp

            return ParseFile();
        }
    }
}

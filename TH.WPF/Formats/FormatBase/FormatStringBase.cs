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

        /// <summary>
        /// <seealso cref="true"/> when executed <seealso cref="TryOpen(FileInfo)"/> and <seealso cref="false"/> when executing <seealso cref="TrySave(FileInfo?)"/>
        /// </summary>
        protected bool IsOpenMode = true;

        FileInfo? SFI { get; set; }
        /// <summary>
        /// Source file wrom which read content
        /// </summary>
        protected FileInfo? SourceFileInfo { get => SFI; }

        FileInfo? TFI { get; set; }
        /// <summary>
        /// Target file in to will be writen changed content of <seealso cref="SourceFileInfo"/> using <seealso cref="TrySave(FileInfo?)"/>
        /// Will be use <seealso cref="SourceFileInfo"/> as target if <seealso cref="TargetFileInfo"/> is <seealso cref="null"/>
        /// </summary>
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
        /// <summary>
        /// Read <seealso cref="SourceFileInfo"/> file.
        /// Creat <seealso cref="StreamReader"/> <seealso cref="Reader"/> here and <seealso cref="ParseLines"/>
        /// </summary>
        /// <returns></returns>
        protected virtual bool ParseFile()
        {
            if (SourceFileInfo == null) return false;

            SR = new(SourceFileInfo.FullName);
            return ParseLines();
        }

        StreamReader? SR { get; set; }
        /// <summary>
        /// StreamReader of the reading <seealso cref="SourceFileInfo"/> file
        /// </summary>
        protected StreamReader? Reader { get => SR; }
        /// <summary>
        /// current parsing line of the reading <seealso cref="SourceFileInfo"/> file
        /// </summary>
        protected string? Line { get; set; }

        /// <summary>
        /// Read one line from file's stream into <seealso cref="Line"/>
        /// </summary>
        /// <returns>Just read value of <seealso cref="Line"/></returns>
        protected string? ReadLine()
        {
            return Line = SR!.ReadLine();
        }

        /// <summary>
        /// Read lines of <seealso cref="SourceFileInfo"/> file.
        /// Try to parse all lines using <seealso cref="ParseLine"/>
        /// When not in <seealso cref="IsOpenMode"/> - write file using <seealso cref="WriteFile"/>
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        protected virtual bool ParseLines()
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

        /// <summary>
        /// Write <seealso cref="SourceFileInfo"/> file strings into <seealso cref="TargetFileInfo"/>
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// <seealso cref="ParseLine"/> return type
        /// </summary>
        protected enum ParseLineRetType
        {
            Continue,
            Break,
            Error
        }

        /// <summary>
        /// Parse <seealso cref="SourceFileInfo"/> file's line.
        /// <seealso cref="ParseLineRetType.Continue"/> - <seealso cref="continue"/> other lines
        /// <seealso cref="ParseLineRetType.Break"/> - <seealso cref="break"/> other lines parse
        /// <seealso cref="ParseLineRetType.Error"/> - Break file parse and return <seealso cref="false"/>
        /// Most likely use this method override to extract file's lines
        /// </summary>
        /// <returns><seealso cref="ParseLineRetType"/> type determining what to do after.</returns>
        protected virtual ParseLineRetType ParseLine()
        {
            return ParseLineRetType.Continue;
        }

        /// <summary>
        /// Add extracted string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="info">info about string</param>
        /// <param name="check">chack if string is valid to add</param>
        protected void Add(string value, string info = "", bool check = true) 
        {
            if(check && !IsValid(value)) return;

            StringsList.Add(new StringData(value, info));
        }

        /// <summary>
        /// chack string if calid to add
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool IsValid(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public bool TrySave(FileInfo? targetFileInfo)
        {
            IsOpenMode = false;
            TFI = targetFileInfo ?? SFI; // when target file info is set use it else use source file infp

            return ParseFile();
        }
    }
}

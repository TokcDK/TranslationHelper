namespace FormatBase
{
    // in: path to file to try parse
    // out: list of extracted strings
    public interface IFormat
    {
        /// <summary>
        /// Extension of file for the format
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Description of file for the format 
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Open actions to parse file with this parser
        /// </summary>
        /// <returns></returns>
        bool TryOpen(FileInfo sourceFileIfo);

        /// <summary>
        /// Save actions to parse file with this parser. Save also have path for case if need to save to specific path not equal to <seealso cref="TryOpen(FileInfo)"/>.
        /// </summary>
        /// <returns></returns>
        bool TrySave(FileInfo? targetFileInfo);

        // out
        /// <summary>
        /// List of strings
        /// </summary>
        public List<StringData> StringsList { get; }
    }
}
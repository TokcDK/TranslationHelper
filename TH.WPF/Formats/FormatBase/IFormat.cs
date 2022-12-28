namespace FormatBase
{
    // in: path to file to try parse
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
        bool TryOpen(FileInfo Path);

        /// <summary>
        /// Save actions to parse file with this parser. Save also have path for case if need to save to specific path.
        /// </summary>
        /// <returns></returns>
        bool TrySave(FileInfo? Path);

        /// <summary>
        /// List of strings
        /// </summary>
        List<StringData> StringsList { get; }
    }
}
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
        IEnumerable<(string original, string translaton, string info)> ExtractStrings(FileInfo sourceFileIfo);

        /// <summary>
        /// Save actions to parse file with this parser. Save also have path for case if need to save to specific path not equal to <seealso cref="ExtractStrings(FileInfo)"/>.
        /// </summary>
        /// <returns></returns>
        bool WriteStrings(FileInfo? targetFileInfo, IEnumerable<(string original, string translaton, string info)> strings);
    }
}
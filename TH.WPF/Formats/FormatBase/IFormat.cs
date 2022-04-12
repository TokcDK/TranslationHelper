namespace FormatBase
{
    public interface IFormat
    {
        /// <summary>
        /// Extension of file for the format
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Open actions to parse file with this parser
        /// </summary>
        /// <returns></returns>
        bool Open();

        /// <summary>
        /// Save actions to parse file with this parser
        /// </summary>
        /// <returns></returns>
        bool Save();
    }
}
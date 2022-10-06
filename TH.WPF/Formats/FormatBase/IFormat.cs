namespace FormatBase
{
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
        bool Open(FileInfo Path);

        /// <summary>
        /// Save actions to parse file with this parser
        /// </summary>
        /// <returns></returns>
        bool Save(FileInfo Path);

        /// <summary>
        /// List of strings
        /// </summary>
        List<StringData> StringsList { get; }
    }

    public class StringData
    {
        public StringData(string original, int index)
        {
            Original = original;
            Index = index;
        }

        public int Index { get; set; }
        public string Original { get; }
        public string Translation { get; set; } = "";
    }
}
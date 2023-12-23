using FormatBase;

namespace ProjectBase
{
    // in: Path to selected file
    // out: List of extracted strings
    public interface IProject
    {
        /// <summary>
        /// Title of the project to identify
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Check if the project can try to open the <paramref name="selectedPath"/>
        /// </summary>
        /// <param name="selectedPath"></param>
        /// <returns></returns>
        bool IsValid(string selectedPath);

        /// <summary>
        /// Try open <paramref name="selectedPath"/> using the project
        /// </summary>
        /// <param name="selectedPath">Path of file or directory</param>
        /// <returns>True if the <paramref name="selectedPath"/> was succesfully opened by the project</returns>
        bool TryOpen(string selectedPath);

        /// <summary>
        /// Try save the project into <paramref name="selectedPath"/>
        /// </summary>
        /// <param name="selectedPath">Path of file or directory. If null or empty the project can save it files into path which was remembered after succesfull <seealso cref="TryOpen(string)"/></param>
        /// <returns>True if the project was succesfully saved</returns>
        bool TrySave(string selectedPath);

        // extra check if any strtings was added here
        /// <summary>
        /// Result list of files with strings
        /// </summary>
        List<FileData>? FilesList { get; }
    }

    public class FileData
    {
        public FileData(FileInfo fileInfo, IFormat format)
        {
            File = fileInfo;
            Format = format;
            Strings = format.StringsList;
        }

        /// <summary>
        /// File info
        /// </summary>
        public FileInfo File { get; }

        public IFormat Format { get; }
        /// <summary>
        /// list of extracted strings
        /// </summary>
        public List<StringData>? Strings { get; }
    }
}
using FormatBase;

namespace ProjectBase
{
    // in: Path to selected file
    // out: List of extracted strings
    public interface IProject
    {
        /// <summary>
        /// Title of the project
        /// </summary>
        string Title { get; }

        // need to check if the project can try to open the file
        /// <summary>
        /// Conditions to determine if the project can open selected file or folder
        /// </summary>
        /// <returns></returns>
        bool IsValid(FileInfo selectedFilePath);

        /// <summary>
        /// Open actions for the project
        /// </summary>
        /// <returns></returns>
        bool TryOpen(FileInfo selectedFilePath);

        /// <summary>
        /// Save actions for the project. Have no input selected file path because admit it was saved on open if need
        /// </summary>
        /// <returns></returns>
        bool TrySave();

        // extra check if any strtings was added here
        /// <summary>
        /// Result list of files with strings
        /// </summary>
        List<FileData>? FilesList { get; }
    }

    public class FileData
    {
        public FileData(FileInfo fileInfo, DirectoryInfo baseDir)
        {
            File = fileInfo;
            BaseDir = baseDir;
            RelativePath = (IsRelative = File.FullName.StartsWith(BaseDir.FullName))
                ? File.FullName.Substring(BaseDir.FullName.Length + 1)
                : File.FullName;
        }

        /// <summary>
        /// Base dir path where is first slected file was located
        /// </summary>
        public DirectoryInfo BaseDir { get; }
        /// <summary>
        /// File info
        /// </summary>
        public FileInfo File { get; }
        /// <summary>
        /// True if file is child of base dir.
        /// </summary>
        internal bool IsRelative { get; }
        /// <summary>
        /// Relative pathe in base dir. To display in files list. When <seealso cref="IsRelative"/> is false will be equal to full path.
        /// </summary>
        public string RelativePath { get; }
        /// <summary>
        /// list of extracted strings
        /// </summary>
        List<StringData>? Strings { get; } = new List<StringData>();
    }
}
using System.Data;
using TranslationHelper.Formats;

namespace TranslationHelper.Models
{
    /// <summary>
    /// One entry of the opened files of a project: the file, the format that reads and writes it, and
    /// the table its content was parsed into.
    /// <para>
    /// This is the unit the workspace is scoped by. Every open file owns its own
    /// <see cref="Table"/>, so editing one file can no longer reach another, and the grid, the source
    /// box and the target box a view creates for it belong to it alone.
    /// </para>
    /// <para>
    /// It is a model: it holds data and announces changes, and it neither reads nor writes a file.
    /// <see cref="Format"/> is what a service uses to do that, which keeps the file system out of the
    /// data the views are bound to.
    /// </para>
    /// </summary>
    public class OpenedFileData : ObservableObject
    {
        /// <summary>
        /// Creates the entry of one file of a project.
        /// </summary>
        /// <param name="fileName">Name of the file, as shown in the files list and on its tab.</param>
        /// <param name="filePath">Path the file was opened from, or empty for a file that has none.</param>
        /// <param name="format">The format that parsed the file and can write it back.</param>
        /// <param name="table">The table the file's content was parsed into.</param>
        public OpenedFileData(string fileName, string filePath, FormatBase format, DataTable table)
        {
            FileName = fileName ?? string.Empty;
            FilePath = filePath ?? string.Empty;
            Format = format;
            Table = table;
        }

        /// <summary>
        /// Creates the "[ALL]" entry: a view over every file of the project rather than a file itself.
        /// <para>
        /// It is an <see cref="OpenedFileData"/> like any other so that the files list and the opened
        /// files tabs can present it the same way they present a file, and so that the editing a view
        /// offers it is the editing it offers a file. It differs in the one way that matters:
        /// <see cref="IsAllFilesAggregate"/> is true, and a caller about to write a file has to skip
        /// it.
        /// </para>
        /// </summary>
        /// <param name="table">
        /// The aggregate table: it holds a row per row of every file and writes each edit back to the
        /// file row it came from.
        /// </param>
        public OpenedFileData(DataTable table)
        {
            FileName = AllEntryName;
            FilePath = string.Empty;
            Format = null;
            Table = table;
            IsAllFilesAggregate = true;
        }

        /// <summary>
        /// Name of the entry that presents every file of the project at once. It is always the first
        /// entry of the files list and never a file on disk.
        /// </summary>
        public const string AllEntryName = "[ALL]";

        /// <summary>
        /// Name of the file, as shown in the files list and on its tab. For the "[ALL]" entry this is
        /// <see cref="AllEntryName"/>.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Path the file was opened from. Empty for the "[ALL]" entry and for a file opened from a
        /// stream, neither of which is written back to a path of its own.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The format that parsed the file and can write it back, or null for the "[ALL]" entry, which
        /// is a view over the files rather than a file.
        /// </summary>
        public FormatBase Format { get; }

        private DataTable _table;

        /// <summary>
        /// The table this entry presents, and the one a grid is bound to while the entry is displayed.
        /// </summary>
        public DataTable Table
        {
            get => _table;
            set => SetProperty(ref _table, value);
        }

        /// <summary>
        /// True only for the "[ALL]" entry. It is what tells a caller that this entry must not be
        /// written to a file and must not be counted as one.
        /// </summary>
        public bool IsAllFilesAggregate { get; }

        /// <summary>
        /// The file's name. It is what a bound list or tab shows for this entry.
        /// </summary>
        public override string ToString() => FileName;
    }
}

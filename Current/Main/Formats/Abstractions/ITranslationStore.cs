using System.Collections.Concurrent;
using System.Data;

namespace TranslationHelper.Formats.Abstractions
{
    /// <summary>
    /// The translation data a format reads from and writes to while it parses or writes one file.
    /// <para>
    /// A format is responsible for exactly one file: it turns that file into rows and turns rows
    /// back into the file. Where those rows are kept, and how duplicates are handled, is not a
    /// format's concern, so the store is reached through this contract instead of through the
    /// concrete project type. The project implements it.
    /// </para>
    /// </summary>
    public interface ITranslationStore
    {
        /// <summary>
        /// Gets all loaded tables: one table per parsed file, named after that file.
        /// </summary>
        DataSet FilesContent { get; }

        /// <summary>
        /// Gets the per-row info tables that belong to <see cref="FilesContent"/>.
        /// </summary>
        DataSet FilesContentInfo { get; }

        /// <summary>
        /// Gets the index of the column holding the original string.
        /// </summary>
        int OriginalColumnIndex { get; }

        /// <summary>
        /// Gets the index of the column holding the translation.
        /// </summary>
        int TranslationColumnIndex { get; }

        /// <summary>
        /// Gets a value indicating whether an original string is stored once, in
        /// <see cref="Hashes"/>, instead of once per occurrence.
        /// </summary>
        bool DontLoadDuplicates { get; }

        /// <summary>
        /// Gets or sets the original to translation dictionary used to resolve translations in
        /// save mode when duplicates are not loaded. Null when the store is not in that mode.
        /// </summary>
        ConcurrentDictionary<string, string> TablesLinesDict { get; set; }

        /// <summary>
        /// Gets or sets the original strings already stored, used to skip duplicates in open mode
        /// when duplicates are not loaded. Null when the store is not in that mode.
        /// </summary>
        ConcurrentSet<string> Hashes { get; set; }

        /// <summary>
        /// Gets, for every original string, the table/row coordinates it was found at. Used to
        /// resolve translations by position when duplicates are loaded.
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> OriginalsTableRowCoordinates { get; }

        /// <summary>
        /// Adds one parsed file's tables to the store. Ignored when the table is empty or its name
        /// is already known.
        /// </summary>
        /// <param name="dataTable">The table holding the parsed strings.</param>
        /// <param name="infoTable">The info table belonging to <paramref name="dataTable"/>.</param>
        void AddTable(DataTable dataTable, DataTable infoTable);

        /// <summary>
        /// Records which format produced <paramref name="dataTable"/>.
        /// <para>
        /// A table on its own says nothing about how to write the file it came from, so the format
        /// that parsed it is kept beside it. That is what lets an opened file be written back by the
        /// same format that read it, without a second lookup by extension.
        /// </para>
        /// </summary>
        /// <param name="dataTable">A table that was just added by <see cref="AddTable"/>.</param>
        /// <param name="format">The format that parsed the file <paramref name="dataTable"/> came from.</param>
        void RegisterFormat(DataTable dataTable, FormatBase format);
    }
}

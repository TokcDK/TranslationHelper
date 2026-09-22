using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TranslationHelper.Models
{
    /// <summary>
    /// The opened files of one project, and which of them is being worked on.
    /// <para>
    /// One instance belongs to one <see cref="Projects.ProjectBase"/> and holds that project's files
    /// only, which is what replaces the single global list the application used to keep: opening a
    /// second project no longer touches the first one's files.
    /// </para>
    /// <para>
    /// The list is a <see cref="BindingList{T}"/> because that is what a bound control can follow —
    /// it announces an added or removed file, so the files list and the opened files tabs can be kept
    /// in step without either of them polling.
    /// </para>
    /// </summary>
    public class OpenedFilesData : ObservableObject
    {
        /// <summary>
        /// The opened files, in the order they were opened.
        /// <para>
        /// The "[ALL]" entry, when the project has one, is the first item: it is an
        /// <see cref="OpenedFileData"/> whose <see cref="OpenedFileData.IsAllFilesAggregate"/> is true,
        /// which is what lets the files list and the tabs present it exactly like a file.
        /// </para>
        /// </summary>
        public BindingList<OpenedFileData> OpenedFilesList { get; } = new BindingList<OpenedFileData>();

        private OpenedFileData _selectedOpenedFileData;

        /// <summary>
        /// The file being worked on: the selected entry of the files list, and the selected tab of the
        /// opened files tabs.
        /// <para>
        /// Null means nothing is being worked on, which is the state a project opens in: the entries
        /// are in the list, but no tab is shown and no controls for one exist until the user picks an
        /// entry.
        /// </para>
        /// </summary>
        public OpenedFileData SelectedOpenedFileData
        {
            get => _selectedOpenedFileData;
            set => SetProperty(ref _selectedOpenedFileData, value);
        }

        /// <summary>
        /// The entry presenting <paramref name="fileName"/>, or null when the project has no such file.
        /// </summary>
        public OpenedFileData Find(string fileName)
        {
            return OpenedFilesList.FirstOrDefault(file => file.FileName == fileName);
        }

        /// <summary>
        /// True when <paramref name="fileName"/> is already opened. A caller uses it to select the file
        /// that is open rather than to open it a second time.
        /// </summary>
        public bool Contains(string fileName) => Find(fileName) != null;

        /// <summary>
        /// The files of the project, leaving out the "[ALL]" entry, which is a view over them rather
        /// than one of them. Anything that writes, counts or saves files uses this.
        /// </summary>
        public IEnumerable<OpenedFileData> Files =>
            OpenedFilesList.Where(file => !file.IsAllFilesAggregate);

        /// <summary>
        /// The "[ALL]" entry, or null when the project has none.
        /// </summary>
        public OpenedFileData AllFilesEntry =>
            OpenedFilesList.FirstOrDefault(file => file.IsAllFilesAggregate);

        /// <summary>
        /// Add <paramref name="file"/> to the end of the list and return it, so the caller can select
        /// it without looking it up again.
        /// </summary>
        public OpenedFileData Add(OpenedFileData file)
        {
            OpenedFilesList.Add(file);
            return file;
        }

        /// <summary>
        /// Make <paramref name="file"/> the first entry of the list. Used for the "[ALL]" entry, which
        /// is always presented above the files.
        /// </summary>
        public OpenedFileData InsertFirst(OpenedFileData file)
        {
            OpenedFilesList.Insert(0, file);
            return file;
        }

        /// <summary>
        /// Drop every entry and the selection. Used when the project is closed.
        /// </summary>
        public void Clear()
        {
            SelectedOpenedFileData = null;
            OpenedFilesList.Clear();
        }
    }
}

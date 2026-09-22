using System.Windows.Forms;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Models;
using TranslationHelper.Projects;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// One open project and the controls that present it.
    /// <para>
    /// This is the unit the rest of the application is scoped by. A function that used to read the
    /// single global files list, grid or text box takes one of these instead and reads the controls of
    /// the project it was given, so the same function works for whichever project is in front of the
    /// user and cannot reach into another one.
    /// </para>
    /// <para>
    /// The members are deliberately the project, its data and its controls and nothing else: what a
    /// caller may do to a project is not part of this contract, so the contract stays a description of
    /// *what is on screen* rather than a second, competing API for opening and saving.
    /// </para>
    /// </summary>
    internal interface IProjectWorkspace
    {
        /// <summary>
        /// The project this workspace presents.
        /// </summary>
        ProjectBase Project { get; }

        /// <summary>
        /// The files of the project, and which of them is being worked on.
        /// </summary>
        OpenedFilesData OpenedFilesData { get; }

        /// <summary>
        /// The project's files list, including the "[ALL]" entry when the project has one.
        /// </summary>
        FilesListControlBase FilesList { get; }

        /// <summary>
        /// The control presenting one tab per entry of <see cref="OpenedFilesData"/>. The selected tab
        /// is <see cref="OpenedFilesData.SelectedOpenedFileData"/>.
        /// </summary>
        TabControl OpenedFilesTabs { get; }

        /// <summary>
        /// The controls of the entry being worked on — its grid, its source box and its target box —
        /// or null while no entry is selected.
        /// </summary>
        OpenedFileWorkspace ActiveFileWorkspace { get; }

        /// <summary>
        /// The label showing how much of <see cref="Project"/> is translated. It belongs to the
        /// project because the count is the project's, not the displayed file's.
        /// </summary>
        Label CompletionLabel { get; }
    }
}

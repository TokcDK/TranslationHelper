using System;
using System.Windows.Forms;
using TranslationHelper.Models;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// The opened files of one project, one tab each.
    /// <para>
    /// The tabs are the entries of <see cref="OpenedFilesData.OpenedFilesList"/> — a tab per file, and
    /// a tab for the "[ALL]" entry — and the selected tab is
    /// <see cref="OpenedFilesData.SelectedOpenedFileData"/>. Both directions are kept in step by
    /// <see cref="TabControlBinder{T}"/>, so opening a file adds its tab without any caller having to
    /// remember to, and closing one removes it.
    /// </para>
    /// <para>
    /// The class does not know what a tab contains: the factory that builds a tab page is supplied by
    /// the workspace, which is what keeps the composition of a file's controls in one place.
    /// </para>
    /// </summary>
    internal partial class OpenedFilesTabControl : UserControl
    {
        private TabControlBinder<OpenedFileData> _binder;

        /// <summary>
        /// The tab control itself. Exposed because the row framework reads the selected tab, and
        /// because it is the control a view has to reach when it needs to select a tab.
        /// </summary>
        internal TabControl Tabs => OpenedFilesTabs;

        /// <summary>
        /// The entry the selected tab presents, or null when no tab is selected.
        /// </summary>
        internal OpenedFileData SelectedFile => _binder?.SelectedItem;

        internal OpenedFilesTabControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show one tab per entry of <paramref name="openedFilesData"/>, now and for every later change
        /// of the list.
        /// </summary>
        /// <param name="openedFilesData">The files of the project.</param>
        /// <param name="createTabPage">Builds the page for one entry.</param>
        internal void Bind(OpenedFilesData openedFilesData, Func<OpenedFileData, TabPage> createTabPage)
        {
            if (openedFilesData == null) throw new ArgumentNullException(nameof(openedFilesData));
            if (createTabPage == null) throw new ArgumentNullException(nameof(createTabPage));

            _binder?.Dispose();

            _binder = new TabControlBinder<OpenedFileData>(OpenedFilesTabs, openedFilesData.OpenedFilesList, createTabPage);
            _binder.SelectionChanged += file => openedFilesData.SelectedOpenedFileData = file;

            _binder.Rebuild();
        }

        /// <summary>
        /// Select the tab presenting <paramref name="file"/>, without reporting it back as a change the
        /// user made.
        /// </summary>
        internal void SelectFile(OpenedFileData file)
        {
            _binder?.SelectItem(file);
        }

        /// <summary>
        /// Re-point the selected tab's grid at its table. Used for the "[ALL]" entry, whose table is
        /// rebuilt from the files whenever the entry is displayed.
        /// </summary>
        internal void RefreshSelectedTab()
        {
            if (OpenedFilesTabs.SelectedTab?.Controls.Count > 0
                && OpenedFilesTabs.SelectedTab.Controls[0] is OpenedFileWorkspace workspace)
            {
                workspace.RefreshBinding();
            }
        }

        /// <summary>
        /// Stop following the list. Called when the project is closed.
        /// </summary>
        internal void Unbind()
        {
            _binder?.Dispose();
            _binder = null;

            OpenedFilesTabs.TabPages.Clear();
        }
    }
}

using TranslationHelper.Data;
using TranslationHelper.Workspace;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Builds the UI adapters a row operation runs against when the caller does not supply any.
    /// <para>
    /// This is the single place where the row framework is wired to the application: the engine and
    /// the operations only ever see <see cref="ISelectionProvider"/> and <see cref="IUiUpdater"/>, and
    /// the decision that those are the WinForms implementations reading a project's workspace is made
    /// here. Keeping the decision in one place is what lets the rest of this folder stay free of the
    /// UI toolkit.
    /// </para>
    /// <para>
    /// The workspace is a parameter rather than a lookup because the adapters have to read the
    /// controls of <em>one</em> project: with several projects open, an adapter that looked the
    /// workspace up itself would read whichever project the user happens to be looking at rather than
    /// the one the run was started for. A caller that does not name one gets the selected project's,
    /// which is what the application's own menus mean.
    /// </para>
    /// </summary>
    internal static class RowServices
    {
        internal static ISelectionProvider CreateSelectionProvider(IProjectWorkspace workspace)
        {
            return new WinFormsSelectionProvider(workspace ?? AppData.ActiveWorkspace);
        }

        internal static IUiUpdater CreateUiUpdater(IProjectWorkspace workspace)
        {
            return new WinFormsUiUpdater(workspace ?? AppData.ActiveWorkspace);
        }
    }
}

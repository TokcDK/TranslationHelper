using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Builds the UI adapters a row operation runs against when the caller does not supply any.
    /// <para>
    /// This is the single place where the row framework is wired to the application: the engine and
    /// the operations only ever see <see cref="ISelectionProvider"/> and <see cref="IUiUpdater"/>, and
    /// the decision that those are the WinForms implementations reading <see cref="AppData"/> is made
    /// here. Keeping the decision in one place is what lets the rest of this folder stay free of the
    /// UI toolkit.
    /// </para>
    /// </summary>
    internal static class RowServices
    {
        internal static ISelectionProvider CreateSelectionProvider()
        {
            return new WinFormsSelectionProvider(AppData.THFilesList, AppData.Main.THFileElementsDataGridView);
        }

        internal static IUiUpdater CreateUiUpdater()
        {
            return new WinFormsUiUpdater(AppData.Main.THFileElementsDataGridView);
        }
    }
}

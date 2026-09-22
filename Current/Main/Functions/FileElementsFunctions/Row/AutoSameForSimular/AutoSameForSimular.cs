namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    class AutoSameForSimular : AutoSameForSimularBase
    {
        /// <summary>
        /// Spreads a translation from a row to the rows similar to it.
        /// </summary>
        /// <param name="project">The project to work on, or null for the selected one.</param>
        /// <param name="workspace">The project's controls, or null for the selected project's.</param>
        internal AutoSameForSimular(Projects.ProjectBase project = null, Workspace.IProjectWorkspace workspace = null)
            : base(project, workspace)
        {
        }
    }
}

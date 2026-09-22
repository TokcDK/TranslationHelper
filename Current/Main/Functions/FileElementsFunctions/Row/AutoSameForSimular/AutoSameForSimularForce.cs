namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    class AutoSameForSimularForce : AutoSameForSimularBase
    {
        /// <summary>
        /// Spreads a translation from a row to the rows similar to it, replacing a translation that is
        /// still the original.
        /// </summary>
        /// <param name="project">The project to work on, or null for the selected one.</param>
        /// <param name="workspace">The project's controls, or null for the selected project's.</param>
        internal AutoSameForSimularForce(Projects.ProjectBase project = null, Workspace.IProjectWorkspace workspace = null)
            : base(project, workspace)
        {
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return !string.IsNullOrEmpty(rowData.Original);
        }

        protected override bool IsForce => true; // force set values
    }
}

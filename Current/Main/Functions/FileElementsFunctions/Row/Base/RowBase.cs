using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Projects;
using TranslationHelper.Workspace;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Base class for row-based operations across one or more file tables.
    /// Supports single-row, multi-row, single-table, multi-table, and all-tables modes.
    /// <para>
    /// A run has four steps, and each of them is somebody else's job:
    /// </para>
    /// <list type="number">
    /// <item><description>
    /// the entry points below turn a selection into a <see cref="TableRowPlan"/> list, through
    /// <see cref="RowPlanFactory"/>;
    /// </description></item>
    /// <item><description>
    /// <see cref="Selected"/> resolves its single target through <see cref="RowTargetResolver"/>;
    /// </description></item>
    /// <item><description>
    /// <see cref="ExecutePlanAsync"/> walks the plan and calls the hooks and <see cref="Apply"/> once
    /// per row — this, and the hooks, is all this class owns;
    /// </description></item>
    /// <item><description>
    /// <see cref="ProcessingState"/> counts the progress.
    /// </description></item>
    /// </list>
    /// <para>
    /// Selection is read through <see cref="ISelectionProvider"/> and a cell is written through
    /// <see cref="UiUpdater"/>, so nothing here knows which UI toolkit is in use.
    /// </para>
    /// </summary>
    internal abstract class RowBase
    {
        public virtual string Name { get; } = string.Empty;

        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The project the run works on. It is resolved when the operation is built, so a run covers
        /// the project it was started for even if the user selects another one while it runs.
        /// </summary>
        protected ProjectBase Project { get; }

        /// <summary>
        /// The project's controls, when the run was started from the application's own menus.
        /// <para>
        /// The engine itself does not use this — it reaches the selection and the cells through the two
        /// contracts below — but an operation that has to follow the project's window can.
        /// </para>
        /// </summary>
        protected IProjectWorkspace Workspace { get; }

        /// <summary>
        /// Where the selection a run works on comes from.
        /// </summary>
        private ISelectionProvider SelectionProvider { get; }

        /// <summary>
        /// How a cell is written and the user is told about a problem. An operation that changes a
        /// row other than the current one, or that has to report something, uses this instead of
        /// reaching for the grid.
        /// </summary>
        protected IUiUpdater UiUpdater { get; }

        /// <summary>
        /// Progress of the current run.
        /// </summary>
        private ProcessingState State { get; } = new ProcessingState();

        /// <summary>
        /// The project content the operations work on.
        /// </summary>
        private readonly DataSet AllFiles;

        private readonly RowTargetResolver TargetResolver;

        /// <summary>
        /// True once any <see cref="Apply"/> of the current run returned true.
        /// </summary>
        private bool _changed;

        #region Constructor
        /// <param name="selectionProvider">
        /// Where the selection comes from, or null to use the application's own. Supplied by callers
        /// that drive the engine themselves — a batch import, or a test.
        /// </param>
        /// <param name="uiUpdater">
        /// How cells are written, or null to use the application's own.
        /// </param>
        internal RowBase(ISelectionProvider selectionProvider = null, IUiUpdater uiUpdater = null)
            : this(null, null, selectionProvider, uiUpdater)
        {
        }

        /// <summary>
        /// Build a run over one project.
        /// </summary>
        /// <param name="project">
        /// The project to work on, or null for the selected one. A caller that drives a project other
        /// than the one on screen — a batch import, or a test — names it here, which is what makes a
        /// run independent of what the user happens to be looking at.
        /// </param>
        /// <param name="workspace">
        /// The project's controls, or null for the selected project's. Only the two adapters below use
        /// it, and a caller that supplies its own <paramref name="selectionProvider"/> and
        /// <paramref name="uiUpdater"/> does not need it at all.
        /// </param>
        /// <param name="selectionProvider">Where the selection comes from, or null to read the workspace.</param>
        /// <param name="uiUpdater">How cells are written, or null to write through the workspace.</param>
        internal RowBase(ProjectBase project, IProjectWorkspace workspace,
            ISelectionProvider selectionProvider = null, IUiUpdater uiUpdater = null)
        {
            Project = project ?? AppData.CurrentProject
                ?? throw new InvalidOperationException("A row operation needs a project, and none is open.");

            Workspace = workspace ?? AppData.ActiveWorkspace;

            AllFiles = Project.FilesContent;

            SelectionProvider = selectionProvider ?? RowServices.CreateSelectionProvider(Workspace);
            UiUpdater = uiUpdater ?? RowServices.CreateUiUpdater(Workspace);

            // The resolver reads which content a files list entry presents, which is the project's own
            // relation: it is what turns a row of the "[ALL]" entry back into the file row it came from.
            TargetResolver = new RowTargetResolver(SelectionProvider, AllFiles, Project.FilesListContent);
        }
        #endregion

        #region Mode flags
        /// <summary>
        /// The run covers every row of every table of the project.
        /// </summary>
        protected bool IsAll { get; private set; }

        /// <summary>
        /// The run covers every row of more than one table.
        /// </summary>
        protected bool IsTables { get; private set; }

        /// <summary>
        /// The run covers every row of exactly one table.
        /// </summary>
        protected bool IsTable { get; private set; }

        /// <summary>
        /// Rows are independent, so they can be processed in parallel. Off by default: most
        /// operations touch shared state or the UI.
        /// </summary>
        protected virtual bool IsParallelRows => false;
        #endregion

        #region Extension points (hooks)
        // The following hooks have been changed to return Task to support asynchronous execution.
        /// <summary>
        /// Actions initialization hook.
        /// </summary>
        protected virtual async Task ActionsInit()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? GetType().Name : Name;
            Logger.Info(T._("{0}: initializing actions..."), name);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on multiple tables.
        /// </summary>
        protected virtual async Task ActionsPreTablesApply()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on a single table.
        /// </summary>
        protected virtual async Task ActionsPreTableApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on rows within a table.
        /// </summary>
        protected virtual async Task ActionsPreRowsApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook before applying actions on a single row.
        /// </summary>
        protected virtual async Task ActionsPreRowApply(RowBaseRowData rowData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on a single row.
        /// </summary>
        protected virtual async Task ActionsPostRowApply()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on rows within a table.
        /// </summary>
        protected virtual async Task ActionsPostRowsApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on a single table.
        /// </summary>
        protected virtual async Task ActionsPostTableApply(TableData tableData)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hook after applying actions on multiple tables.
        /// </summary>
        protected virtual async Task ActionsPostTablesApply()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Finalization hook.
        /// </summary>
        protected virtual async Task ActionsFinalize()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? GetType().Name : Name;
            Logger.Info(T._("{0}: parsed {1} values"), name, State.ParsedCount);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Plays completion sound.
        /// </summary>
        protected virtual async Task CompleteSound()
        {
            System.Media.SystemSounds.Asterisk.Play();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Whether the single target of a <see cref="Selected"/> run may be processed. Vetoes the run
        /// before anything happens.
        /// </summary>
        protected virtual bool IsOkSelected(TableData tableData) => true;

        /// <summary>
        /// Whether a whole table may be processed. Only asked for whole-table scopes, so a run over
        /// explicitly selected rows is never blocked by it.
        /// </summary>
        protected virtual bool IsOkTable(TableData tableData) => true;

        /// <summary>
        /// Whether a run that starts from the files list selection may happen at all. Asked by
        /// <see cref="Table"/> and <see cref="All"/>, and deliberately not by their "T" variants.
        /// </summary>
        protected virtual bool IsOkAll() => true;

        /// <summary>
        /// Whether a row is worth looking at. The default skips rows whose translation still equals
        /// the original, when the user asked for those to be ignored.
        /// </summary>
        protected virtual bool IsValidRow(RowBaseRowData rowData) =>
            !AppSettings.IgnoreOrigEqualTransLines || !Equals(rowData.Original, rowData.Translation);

        /// <summary>
        /// The operation itself: change <paramref name="rowData"/> and return true when something was
        /// changed. Returning false means "this row needed nothing", and it is not counted.
        /// </summary>
        protected abstract bool Apply(RowBaseRowData rowData);
        #endregion

        #region Public/internal APIs
        /// <summary>
        /// Process a single DataRow, using current selection if indices not provided.
        /// </summary>
        /// <param name="row">Unused; the row is identified by the indexes.</param>
        /// <param name="tableIndex">Index of table in DataSet; -1 to use UI selection.</param>
        /// <param name="rowIndex">Row index; -1 to use UI selection.</param>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Selected(DataRow row, int tableIndex = -1, int rowIndex = -1)
        {
            if (!TargetResolver.TryResolve(ref tableIndex, ref rowIndex, out var tableData, out var realRowIdx))
                return false;

            if (!IsOkSelected(tableData))
                return false;

            return await ExecutePlanAsync(new[] { new TableRowPlan(tableData, new[] { realRowIdx }) }).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the rows selected in the work table grid.
        /// <para>
        /// An empty plan is not an error: it means nothing was selected, or the operation's
        /// <see cref="IsOkSelected"/> policy rejected the selection. Either way the run stops before
        /// <see cref="ActionsInit"/>, so no hook of a run that has nothing to do ever fires.
        /// </para>
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Rows()
        {
            var plan = RowPlanFactory.ForSelectedRows(SelectionProvider, IsOkSelected);
            if (plan.Length == 0)
                return false;

            return await ExecutePlanAsync(plan).ConfigureAwait(false);
        }

        /// <summary>
        /// Process one or more user-selected entries fully. The "[ALL]" entry stands for every file,
        /// so selecting it applies the operation to all of them.
        /// <para>
        /// Runs the <see cref="IsOkAll"/> policy first. <see cref="TableT"/> is the same run without
        /// that check, and is what callers use when the policy must not block them.
        /// </para>
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> Table()
        {
            if (!IsOkAll())
                return false;

            return await ExecutePlanAsync(GetSelectedEntriesPlan()).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the selected entries without asking <see cref="IsOkAll"/> first. See
        /// <see cref="Table"/>.
        /// </summary>
        internal async Task<bool> TableT() => await ExecutePlanAsync(GetSelectedEntriesPlan()).ConfigureAwait(false);

        /// <summary>
        /// Process every table in the DataSet.
        /// <para>
        /// Runs the <see cref="IsOkAll"/> policy first. <see cref="AllT"/> is the same run without
        /// that check, and is what callers use when the policy must not block them.
        /// </para>
        /// </summary>
        /// <returns>True if any Apply() succeeded.</returns>
        internal async Task<bool> All()
        {
            if (!IsOkAll())
                return false;

            return await ExecutePlanAsync(GetAllTablesPlan()).ConfigureAwait(false);
        }

        /// <summary>
        /// Process every table without asking <see cref="IsOkAll"/> first. See <see cref="All"/>.
        /// </summary>
        internal async Task<bool> AllT() => await ExecutePlanAsync(GetAllTablesPlan()).ConfigureAwait(false);

        /// <summary>
        /// Whole-table plan for the entries selected in the files list.
        /// </summary>
        private TableRowPlan[] GetSelectedEntriesPlan() => RowPlanFactory.ForSelectedEntries(SelectionProvider, AllFiles);

        /// <summary>
        /// Whole-table plan for every table of the DataSet.
        /// </summary>
        private TableRowPlan[] GetAllTablesPlan() => RowPlanFactory.ForAllTables(AllFiles);
        #endregion

        #region Core execution
        /// <summary>
        /// Drives processing per table and per row.
        /// </summary>
        private async Task<bool> ExecutePlanAsync(TableRowPlan[] plan)
        {
            _changed = false;
            State.Reset(plan);

            // "every row of every table", "every row of several tables" and "every row of one table"
            // are still told apart, because the hooks and the completion sound depend on that.
            bool wholeTables = plan.All(entry => entry.RowIndexes == null);
            IsAll = wholeTables && plan.Length == AllFiles.Tables.Count;
            IsTables = wholeTables && plan.Length > 1;
            IsTable = wholeTables && plan.Length == 1;

            await ActionsInit().ConfigureAwait(false);
            if (IsTables)
                await ActionsPreTablesApply().ConfigureAwait(false);

            foreach (var entry in plan)
            {
                if (entry.RowIndexes == null && !IsOkTable(entry.Table))
                    continue;

                await ActionsPreTableApply(entry.Table).ConfigureAwait(false);
                await ExecuteRowsAsync(entry.Table, entry.RowIndexes).ConfigureAwait(false);
                await ActionsPostTableApply(entry.Table).ConfigureAwait(false);
            }

            if (IsTables)
                await ActionsPostTablesApply().ConfigureAwait(false);

            await ActionsFinalize().ConfigureAwait(false);
            if (IsTables || IsAll)
                await CompleteSound().ConfigureAwait(false);

            return _changed;
        }

        /// <summary>
        /// Iterate over specified or all rows in a table.
        /// </summary>
        private async Task ExecuteRowsAsync(TableData tableData, int[] rowIndexes)
        {
            int count = rowIndexes?.Length ?? tableData.SelectedTable.Rows.Count;
            State.SelectedRowsCount = count;
            State.SelectedRowsCountRest = count;
            await ActionsPreRowsApply(tableData).ConfigureAwait(false);

            if (IsParallelRows)
            {
                var tasks = new List<Task>();
                for (int i = 0; i < count; i++)
                {
                    int index = rowIndexes == null ? i : rowIndexes[i];
                    tasks.Add(Task.Run(() => ProcessRowSafeAsync(tableData, index)));
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int index = rowIndexes == null ? i : rowIndexes[i];
                    await ProcessRowSafeAsync(tableData, index).ConfigureAwait(false);
                }
            }

            await ActionsPostRowsApply(tableData).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs hooks and Apply() for a single row safely.
        /// </summary>
        private async Task ProcessRowSafeAsync(TableData tableData, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= tableData.SelectedTable.Rows.Count)
                return;
            await ProcessRowAsync(tableData, rowIndex).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs hooks and Apply() for a single row.
        /// </summary>
        private async Task ProcessRowAsync(TableData tableData, int rowIndex)
        {
            var rowData = new RowBaseRowData(Project, tableData.SelectedTable.Rows[rowIndex], rowIndex, tableData, UiUpdater)
            {
                IsLastRow = (--State.SelectedRowsCountRest == 0)
            };

            try
            {
                if (!IsValidRow(rowData))
                    return;
            }
            catch
            {
                Logger.Warn("Error to check row valid");
                return;
            }

            await ActionsPreRowApply(rowData).ConfigureAwait(false);
            try
            {
                if (Apply(rowData))
                {
                    _changed = true;

                    State.IncrementParsed();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to parse row. Error: {0}", ex);
            }
            await ActionsPostRowApply().ConfigureAwait(false);
        }
        #endregion
    }
}

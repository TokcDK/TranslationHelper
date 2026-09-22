using NLog;
using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Forms.Search.SearchNew;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Helpers;
using TranslationHelper.Main.Functions;
using TranslationHelper.Theming;
using TranslationHelper.Workspace;

namespace TranslationHelper
{
    /// <summary>
    /// The application window: the menus, the log, and the workspace of the open projects.
    /// <para>
    /// The window owns <see cref="Workspace"/> and nothing else about a project. Everything that
    /// presents a project — its files list, its opened files, the grid and the text boxes of the file
    /// being worked on — lives in that project's own workspace, in a tab of the projects tab control.
    /// That is what lets two projects be open at once: there is no longer a single set of controls whose
    /// contents have to be swapped when the user changes project.
    /// </para>
    /// <para>
    /// The handlers that used to be here and worked on those controls are gone with them; they are
    /// methods of the control that owns the data now, and forward to the function that owns the
    /// behaviour.
    /// </para>
    /// </summary>
    public partial class FormMain : ThemableForm
    {
        internal string extractedpatchpath = string.Empty;

        internal string FVariant = string.Empty;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The open projects and their controls. Created once, and the control it presents is hosted
        /// by <c>frmMainPanel</c>.
        /// </summary>
        internal MainWorkspace Workspace { get; }

        public FormMain()
        {
            InitializeComponent();

            AppData.Init(this);

            // The workspace is built after the projects collection it is bound to, and before anything
            // can open a project. Its control is the only thing the window hosts for a project.
            Workspace = new MainWorkspace(AppData.ProjectsData);
            frmMainPanel.Controls.Add(Workspace.Control);

            FunctionsUI.Init(this);
        }

        private void THMain_Load(object sender, EventArgs e)
        {
            AppHelper.SetupLogging(this);
            FunctionsUI.THMain_Load();
        }

        private async void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsDBFile.UnLockDBLoad(false);
            await FunctionsDBFile.LoadDB();
        }

        private void LoadTrasnlationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        [Obsolete]
        internal bool IsTranslating;

        private void CellFixesSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = new FixCells().Rows();
        }

        private void SetAsDatasourceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The grid of the file being worked on, rather than the window's single grid: there is one
            // grid per open file now, and this menu acts on the one the user is looking at.
            var workspace = AppData.ActiveWorkspace;
            var grid = workspace?.ActiveFileWorkspace?.ElementsDataGridView;
            if (grid == null) return;

            grid.DataSource = workspace.Project.FilesContentAll;
        }

        private void SetColumnSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void AddToCustomDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void THInfolabel_Click(object sender, EventArgs e)
        {
        }

        private void OpenCurrentLogFileButton_Click(object sender, EventArgs e)
        {
            AppHelper.OpenCurrentFileLogFile();
        }

        private void THMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            FunctionsUI.THMain_FormClosing(sender, e);
        }

        private void THfrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            Logger.Info(T._($"Application started"));
        }

        public void IndicateSaveProcess(string infoText = "")
        {
            Logger.Info(infoText);
        }

        internal THfrmSearch search;
        internal SearchForm searchformNew;

        internal static bool DGVCellInEditMode
        {
            get => AppSettings.DGVCellInEditMode;
        }

        //Материалы
        //по оптимизации кода
        //https://cc.davelozinski.com/c-sharp/fastest-way-to-compare-strings
        //http://www.vcskicks.com/optimize_csharp_code.php
        //https://stackoverflow.com/questions/7872633/most-advisable-way-of-checking-empty-strings-in-c-sharp
        //https://social.msdn.microsoft.com/Forums/en-US/9977e45f-c8c5-4a8f-9e02-12f74c1c4579/what-is-the-difference-between-stringempty-and-quotquot-?forum=csharplanguage
        //Сортировка при виртуальном режиме DatagridView
        //http://qaru.site/questions/1486005/c-datagridview-virtual-mode-enable-sorting
        //c# - Поиск ячеек/строк по DataGridView
        //http://www.skillcoding.com/Default.aspx?id=151
        //Ошибка "Строку, связанную с положением CurrencyManager, нельзя сделать невидимой"
        //http://www.cyberforum.ru/csharp-beginners/thread757809.html
        //Виртуальный режим
        //https://stackoverflow.com/questions/31458197/how-to-sort-datagridview-data-when-virtual-mode-enable
    }
}

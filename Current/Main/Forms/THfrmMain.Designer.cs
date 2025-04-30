namespace TranslationHelper
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.MainMenus = new System.Windows.Forms.MenuStrip();
            this.mainMenusHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FilesListMenus = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectedForceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RowMenus = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.frmMainPanel = new System.Windows.Forms.Panel();
            this.tlpFrmMain = new System.Windows.Forms.TableLayoutPanel();
            this.THWorkSpaceSplitContainer = new System.Windows.Forms.SplitContainer();
            this.THFilesElementsPanel = new System.Windows.Forms.Panel();
            this.THsplitContainerFilesElements = new System.Windows.Forms.SplitContainer();
            this.THFilesListPanel = new System.Windows.Forms.Panel();
            this.THFilesList = new System.Windows.Forms.ListBox();
            this.tlpFileElements = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFileElementsFilterAndReset = new System.Windows.Forms.TableLayoutPanel();
            this.THFiltersDataGridView = new System.Windows.Forms.DataGridView();
            this.THbtnMainResetTable = new System.Windows.Forms.Button();
            this.THFileElementsDataGridView = new System.Windows.Forms.DataGridView();
            this.THTextInfoAndEditPanel = new System.Windows.Forms.Panel();
            this.THInfoEditSplitContainer = new System.Windows.Forms.SplitContainer();
            this.THInfoTextBox = new System.Windows.Forms.TextBox();
            this.THEditElementsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.THSourceRichTextBox = new System.Windows.Forms.RichTextBox();
            this.THTargetRichTextBox = new System.Windows.Forms.RichTextBox();
            this.tlpWorkInfo = new System.Windows.Forms.TableLayoutPanel();
            this.tlpTextLenPosInfo = new System.Windows.Forms.TableLayoutPanel();
            this.TargetTextBoxColumnPositionLabel = new System.Windows.Forms.Label();
            this.TargetTextBoxLinePositionLabelData = new System.Windows.Forms.Label();
            this.TranslationLongestLineLenghtLabel = new System.Windows.Forms.Label();
            this.TargetTextBoxLinePositionLabel = new System.Windows.Forms.Label();
            this.RTBInfoLengthLabel = new System.Windows.Forms.Label();
            this.TargetTextBoxColumnPositionLabelData = new System.Windows.Forms.Label();
            this.TableCompleteInfoLabel = new System.Windows.Forms.Label();
            this.mainFormRootWorkspaceLogContainer = new System.Windows.Forms.SplitContainer();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.logSplitter = new System.Windows.Forms.Splitter();
            this.MainMenus.SuspendLayout();
            this.frmMainPanel.SuspendLayout();
            this.tlpFrmMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THWorkSpaceSplitContainer)).BeginInit();
            this.THWorkSpaceSplitContainer.Panel1.SuspendLayout();
            this.THWorkSpaceSplitContainer.Panel2.SuspendLayout();
            this.THWorkSpaceSplitContainer.SuspendLayout();
            this.THFilesElementsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THsplitContainerFilesElements)).BeginInit();
            this.THsplitContainerFilesElements.Panel1.SuspendLayout();
            this.THsplitContainerFilesElements.Panel2.SuspendLayout();
            this.THsplitContainerFilesElements.SuspendLayout();
            this.THFilesListPanel.SuspendLayout();
            this.tlpFileElements.SuspendLayout();
            this.tlpFileElementsFilterAndReset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THFiltersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THFileElementsDataGridView)).BeginInit();
            this.THTextInfoAndEditPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THInfoEditSplitContainer)).BeginInit();
            this.THInfoEditSplitContainer.Panel1.SuspendLayout();
            this.THInfoEditSplitContainer.Panel2.SuspendLayout();
            this.THInfoEditSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THEditElementsSplitContainer)).BeginInit();
            this.THEditElementsSplitContainer.Panel1.SuspendLayout();
            this.THEditElementsSplitContainer.Panel2.SuspendLayout();
            this.THEditElementsSplitContainer.SuspendLayout();
            this.tlpWorkInfo.SuspendLayout();
            this.tlpTextLenPosInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainFormRootWorkspaceLogContainer)).BeginInit();
            this.mainFormRootWorkspaceLogContainer.Panel1.SuspendLayout();
            this.mainFormRootWorkspaceLogContainer.Panel2.SuspendLayout();
            this.mainFormRootWorkspaceLogContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenus
            // 
            this.MainMenus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainMenusHereToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.MainMenus.Location = new System.Drawing.Point(0, 0);
            this.MainMenus.Name = "MainMenus";
            this.MainMenus.Size = new System.Drawing.Size(790, 24);
            this.MainMenus.TabIndex = 1;
            this.MainMenus.Text = "MainMenu";
            // 
            // mainMenusHereToolStripMenuItem
            // 
            this.mainMenusHereToolStripMenuItem.Name = "mainMenusHereToolStripMenuItem";
            this.mainMenusHereToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.mainMenusHereToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // FilesListMenus
            // 
            this.FilesListMenus.Name = "contextMenuStrip1";
            this.FilesListMenus.Size = new System.Drawing.Size(61, 4);
            this.FilesListMenus.Text = "2000";
            // 
            // selectedForceToolStripMenuItem
            // 
            this.selectedForceToolStripMenuItem.Name = "selectedForceToolStripMenuItem";
            this.selectedForceToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // RowMenus
            // 
            this.RowMenus.Name = "contextMenuStrip1";
            this.RowMenus.Size = new System.Drawing.Size(61, 4);
            // 
            // frmMainPanel
            // 
            this.frmMainPanel.Controls.Add(this.tlpFrmMain);
            this.frmMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMainPanel.Location = new System.Drawing.Point(0, 0);
            this.frmMainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.frmMainPanel.Name = "frmMainPanel";
            this.frmMainPanel.Size = new System.Drawing.Size(790, 425);
            this.frmMainPanel.TabIndex = 16;
            // 
            // tlpFrmMain
            // 
            this.tlpFrmMain.ColumnCount = 1;
            this.tlpFrmMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFrmMain.Controls.Add(this.THWorkSpaceSplitContainer, 0, 0);
            this.tlpFrmMain.Controls.Add(this.tlpWorkInfo, 0, 1);
            this.tlpFrmMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFrmMain.Location = new System.Drawing.Point(0, 0);
            this.tlpFrmMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFrmMain.Name = "tlpFrmMain";
            this.tlpFrmMain.RowCount = 2;
            this.tlpFrmMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFrmMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpFrmMain.Size = new System.Drawing.Size(790, 425);
            this.tlpFrmMain.TabIndex = 0;
            // 
            // THWorkSpaceSplitContainer
            // 
            this.THWorkSpaceSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THWorkSpaceSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.THWorkSpaceSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.THWorkSpaceSplitContainer.Name = "THWorkSpaceSplitContainer";
            this.THWorkSpaceSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // THWorkSpaceSplitContainer.Panel1
            // 
            this.THWorkSpaceSplitContainer.Panel1.Controls.Add(this.THFilesElementsPanel);
            // 
            // THWorkSpaceSplitContainer.Panel2
            // 
            this.THWorkSpaceSplitContainer.Panel2.Controls.Add(this.THTextInfoAndEditPanel);
            this.THWorkSpaceSplitContainer.Size = new System.Drawing.Size(790, 405);
            this.THWorkSpaceSplitContainer.SplitterDistance = 310;
            this.THWorkSpaceSplitContainer.TabIndex = 5;
            // 
            // THFilesElementsPanel
            // 
            this.THFilesElementsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.THFilesElementsPanel.Controls.Add(this.THsplitContainerFilesElements);
            this.THFilesElementsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THFilesElementsPanel.Location = new System.Drawing.Point(0, 0);
            this.THFilesElementsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.THFilesElementsPanel.Name = "THFilesElementsPanel";
            this.THFilesElementsPanel.Size = new System.Drawing.Size(790, 310);
            this.THFilesElementsPanel.TabIndex = 4;
            // 
            // THsplitContainerFilesElements
            // 
            this.THsplitContainerFilesElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THsplitContainerFilesElements.Location = new System.Drawing.Point(0, 0);
            this.THsplitContainerFilesElements.Margin = new System.Windows.Forms.Padding(0);
            this.THsplitContainerFilesElements.Name = "THsplitContainerFilesElements";
            // 
            // THsplitContainerFilesElements.Panel1
            // 
            this.THsplitContainerFilesElements.Panel1.Controls.Add(this.THFilesListPanel);
            // 
            // THsplitContainerFilesElements.Panel2
            // 
            this.THsplitContainerFilesElements.Panel2.Controls.Add(this.tlpFileElements);
            this.THsplitContainerFilesElements.Size = new System.Drawing.Size(788, 308);
            this.THsplitContainerFilesElements.SplitterDistance = 127;
            this.THsplitContainerFilesElements.TabIndex = 3;
            // 
            // THFilesListPanel
            // 
            this.THFilesListPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THFilesListPanel.Controls.Add(this.THFilesList);
            this.THFilesListPanel.Location = new System.Drawing.Point(0, 0);
            this.THFilesListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.THFilesListPanel.Name = "THFilesListPanel";
            this.THFilesListPanel.Size = new System.Drawing.Size(128, 281);
            this.THFilesListPanel.TabIndex = 1;
            // 
            // THFilesList
            // 
            this.THFilesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THFilesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THFilesList.FormattingEnabled = true;
            this.THFilesList.Location = new System.Drawing.Point(0, 0);
            this.THFilesList.Margin = new System.Windows.Forms.Padding(0);
            this.THFilesList.Name = "THFilesList";
            this.THFilesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.THFilesList.Size = new System.Drawing.Size(128, 281);
            this.THFilesList.TabIndex = 0;
            // 
            // tlpFileElements
            // 
            this.tlpFileElements.ColumnCount = 1;
            this.tlpFileElements.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFileElements.Controls.Add(this.tlpFileElementsFilterAndReset, 0, 0);
            this.tlpFileElements.Controls.Add(this.THFileElementsDataGridView, 0, 1);
            this.tlpFileElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFileElements.Location = new System.Drawing.Point(0, 0);
            this.tlpFileElements.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFileElements.Name = "tlpFileElements";
            this.tlpFileElements.RowCount = 2;
            this.tlpFileElements.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpFileElements.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFileElements.Size = new System.Drawing.Size(657, 308);
            this.tlpFileElements.TabIndex = 5;
            // 
            // tlpFileElementsFilterAndReset
            // 
            this.tlpFileElementsFilterAndReset.ColumnCount = 2;
            this.tlpFileElementsFilterAndReset.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFileElementsFilterAndReset.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpFileElementsFilterAndReset.Controls.Add(this.THFiltersDataGridView, 0, 0);
            this.tlpFileElementsFilterAndReset.Controls.Add(this.THbtnMainResetTable, 1, 0);
            this.tlpFileElementsFilterAndReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFileElementsFilterAndReset.Location = new System.Drawing.Point(1, 1);
            this.tlpFileElementsFilterAndReset.Margin = new System.Windows.Forms.Padding(1);
            this.tlpFileElementsFilterAndReset.Name = "tlpFileElementsFilterAndReset";
            this.tlpFileElementsFilterAndReset.RowCount = 1;
            this.tlpFileElementsFilterAndReset.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFileElementsFilterAndReset.Size = new System.Drawing.Size(655, 23);
            this.tlpFileElementsFilterAndReset.TabIndex = 0;
            // 
            // THFiltersDataGridView
            // 
            this.THFiltersDataGridView.AllowUserToAddRows = false;
            this.THFiltersDataGridView.AllowUserToDeleteRows = false;
            this.THFiltersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.THFiltersDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.THFiltersDataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.THFiltersDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THFiltersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.THFiltersDataGridView.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.THFiltersDataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.THFiltersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THFiltersDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.THFiltersDataGridView.Enabled = false;
            this.THFiltersDataGridView.Location = new System.Drawing.Point(1, 1);
            this.THFiltersDataGridView.Margin = new System.Windows.Forms.Padding(1);
            this.THFiltersDataGridView.Name = "THFiltersDataGridView";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.THFiltersDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.THFiltersDataGridView.RowTemplate.Height = 23;
            this.THFiltersDataGridView.Size = new System.Drawing.Size(633, 21);
            this.THFiltersDataGridView.TabIndex = 3;
            this.THFiltersDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFiltersDataGridView_CellClick);
            this.THFiltersDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFiltersDataGridView_CellValueChanged);
            this.THFiltersDataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.THFiltersDataGridView_RowPostPaint);
            this.THFiltersDataGridView.MouseEnter += new System.EventHandler(this.THFiltersDataGridView_MouseEnter);
            this.THFiltersDataGridView.MouseLeave += new System.EventHandler(this.THFiltersDataGridView_MouseLeave);
            // 
            // THbtnMainResetTable
            // 
            this.THbtnMainResetTable.Location = new System.Drawing.Point(636, 1);
            this.THbtnMainResetTable.Margin = new System.Windows.Forms.Padding(1);
            this.THbtnMainResetTable.Name = "THbtnMainResetTable";
            this.THbtnMainResetTable.Size = new System.Drawing.Size(18, 21);
            this.THbtnMainResetTable.TabIndex = 4;
            this.THbtnMainResetTable.Text = "-";
            this.THbtnMainResetTable.UseVisualStyleBackColor = true;
            this.THbtnMainResetTable.Click += new System.EventHandler(this.THMainResetTableButton_Click);
            // 
            // THFileElementsDataGridView
            // 
            this.THFileElementsDataGridView.AllowUserToAddRows = false;
            this.THFileElementsDataGridView.AllowUserToDeleteRows = false;
            this.THFileElementsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THFileElementsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.THFileElementsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.THFileElementsDataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.THFileElementsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THFileElementsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.THFileElementsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.THFileElementsDataGridView.Location = new System.Drawing.Point(1, 26);
            this.THFileElementsDataGridView.Margin = new System.Windows.Forms.Padding(1, 1, 3, 1);
            this.THFileElementsDataGridView.Name = "THFileElementsDataGridView";
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.THFileElementsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.THFileElementsDataGridView.RowTemplate.Height = 23;
            this.THFileElementsDataGridView.Size = new System.Drawing.Size(653, 281);
            this.THFileElementsDataGridView.TabIndex = 2;
            this.THFileElementsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.THFileElementsDataGridView_CellBeginEdit);
            this.THFileElementsDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFileElementsDataGridView_CellClick);
            this.THFileElementsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFileElementsDataGridView_CellEndEdit);
            this.THFileElementsDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFileElementsDataGridView_CellEnter);
            this.THFileElementsDataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.THFileElementsDataGridView_CellMouseClick);
            this.THFileElementsDataGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.THFileElementsDataGridView_CellMouseDown);
            this.THFileElementsDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFileElementsDataGridView_CellValidated);
            this.THFileElementsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFileElementsDataGridView_CellValueChanged);
            this.THFileElementsDataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.THFileElementsDataGridView_RowPostPaint);
            this.THFileElementsDataGridView.SelectionChanged += new System.EventHandler(this.THFileElementsDataGridView_SelectionChanged);
            this.THFileElementsDataGridView.Sorted += new System.EventHandler(this.THFileElementsDataGridView_Sorted);
            // 
            // THTextInfoAndEditPanel
            // 
            this.THTextInfoAndEditPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.THTextInfoAndEditPanel.Controls.Add(this.THInfoEditSplitContainer);
            this.THTextInfoAndEditPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THTextInfoAndEditPanel.Location = new System.Drawing.Point(0, 0);
            this.THTextInfoAndEditPanel.Margin = new System.Windows.Forms.Padding(0);
            this.THTextInfoAndEditPanel.Name = "THTextInfoAndEditPanel";
            this.THTextInfoAndEditPanel.Size = new System.Drawing.Size(790, 91);
            this.THTextInfoAndEditPanel.TabIndex = 6;
            // 
            // THInfoEditSplitContainer
            // 
            this.THInfoEditSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THInfoEditSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.THInfoEditSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.THInfoEditSplitContainer.Name = "THInfoEditSplitContainer";
            // 
            // THInfoEditSplitContainer.Panel1
            // 
            this.THInfoEditSplitContainer.Panel1.Controls.Add(this.THInfoTextBox);
            // 
            // THInfoEditSplitContainer.Panel2
            // 
            this.THInfoEditSplitContainer.Panel2.Controls.Add(this.THEditElementsSplitContainer);
            this.THInfoEditSplitContainer.Size = new System.Drawing.Size(788, 89);
            this.THInfoEditSplitContainer.SplitterDistance = 126;
            this.THInfoEditSplitContainer.TabIndex = 5;
            // 
            // THInfoTextBox
            // 
            this.THInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THInfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THInfoTextBox.Location = new System.Drawing.Point(0, 0);
            this.THInfoTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.THInfoTextBox.Multiline = true;
            this.THInfoTextBox.Name = "THInfoTextBox";
            this.THInfoTextBox.ReadOnly = true;
            this.THInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.THInfoTextBox.Size = new System.Drawing.Size(126, 89);
            this.THInfoTextBox.TabIndex = 0;
            // 
            // THEditElementsSplitContainer
            // 
            this.THEditElementsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THEditElementsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.THEditElementsSplitContainer.Name = "THEditElementsSplitContainer";
            // 
            // THEditElementsSplitContainer.Panel1
            // 
            this.THEditElementsSplitContainer.Panel1.Controls.Add(this.THSourceRichTextBox);
            // 
            // THEditElementsSplitContainer.Panel2
            // 
            this.THEditElementsSplitContainer.Panel2.Controls.Add(this.THTargetRichTextBox);
            this.THEditElementsSplitContainer.Size = new System.Drawing.Size(658, 89);
            this.THEditElementsSplitContainer.SplitterDistance = 304;
            this.THEditElementsSplitContainer.TabIndex = 4;
            // 
            // THSourceRichTextBox
            // 
            this.THSourceRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THSourceRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSourceRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.THSourceRichTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.THSourceRichTextBox.Name = "THSourceRichTextBox";
            this.THSourceRichTextBox.ReadOnly = true;
            this.THSourceRichTextBox.Size = new System.Drawing.Size(304, 89);
            this.THSourceRichTextBox.TabIndex = 2;
            this.THSourceRichTextBox.Text = "";
            this.THSourceRichTextBox.SelectionChanged += new System.EventHandler(this.THSourceRichTextBox_SelectionChanged);
            this.THSourceRichTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.THSourceRichTextBox_MouseClick);
            this.THSourceRichTextBox.MouseEnter += new System.EventHandler(this.THSourceRichTextBox_MouseEnter);
            this.THSourceRichTextBox.MouseLeave += new System.EventHandler(this.THSourceRichTextBox_MouseLeave);
            // 
            // THTargetRichTextBox
            // 
            this.THTargetRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THTargetRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THTargetRichTextBox.Enabled = false;
            this.THTargetRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.THTargetRichTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.THTargetRichTextBox.Name = "THTargetRichTextBox";
            this.THTargetRichTextBox.Size = new System.Drawing.Size(350, 89);
            this.THTargetRichTextBox.TabIndex = 1;
            this.THTargetRichTextBox.Text = "";
            this.THTargetRichTextBox.SelectionChanged += new System.EventHandler(this.THTargetRichTextBox_SelectionChanged);
            this.THTargetRichTextBox.TextChanged += new System.EventHandler(this.THTargetRichTextBox_TextChanged);
            this.THTargetRichTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.THTargetTextBox_KeyDown);
            this.THTargetRichTextBox.Leave += new System.EventHandler(this.THTargetTextBox_Leave);
            this.THTargetRichTextBox.MouseEnter += new System.EventHandler(this.THTargetRichTextBox_MouseEnter);
            this.THTargetRichTextBox.MouseLeave += new System.EventHandler(this.THTargetRichTextBox_MouseLeave);
            // 
            // tlpWorkInfo
            // 
            this.tlpWorkInfo.ColumnCount = 4;
            this.tlpWorkInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tlpWorkInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.44445F));
            this.tlpWorkInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.55458F));
            this.tlpWorkInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.04949F));
            this.tlpWorkInfo.Controls.Add(this.tlpTextLenPosInfo, 2, 0);
            this.tlpWorkInfo.Controls.Add(this.TableCompleteInfoLabel, 3, 0);
            this.tlpWorkInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpWorkInfo.Location = new System.Drawing.Point(0, 405);
            this.tlpWorkInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tlpWorkInfo.Name = "tlpWorkInfo";
            this.tlpWorkInfo.RowCount = 1;
            this.tlpWorkInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWorkInfo.Size = new System.Drawing.Size(790, 20);
            this.tlpWorkInfo.TabIndex = 6;
            // 
            // tlpTextLenPosInfo
            // 
            this.tlpTextLenPosInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tlpTextLenPosInfo.ColumnCount = 6;
            this.tlpTextLenPosInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpTextLenPosInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tlpTextLenPosInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpTextLenPosInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tlpTextLenPosInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpTextLenPosInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tlpTextLenPosInfo.Controls.Add(this.TargetTextBoxColumnPositionLabel, 4, 0);
            this.tlpTextLenPosInfo.Controls.Add(this.TargetTextBoxLinePositionLabelData, 3, 0);
            this.tlpTextLenPosInfo.Controls.Add(this.TranslationLongestLineLenghtLabel, 1, 0);
            this.tlpTextLenPosInfo.Controls.Add(this.TargetTextBoxLinePositionLabel, 2, 0);
            this.tlpTextLenPosInfo.Controls.Add(this.RTBInfoLengthLabel, 0, 0);
            this.tlpTextLenPosInfo.Controls.Add(this.TargetTextBoxColumnPositionLabelData, 5, 0);
            this.tlpTextLenPosInfo.Location = new System.Drawing.Point(400, 0);
            this.tlpTextLenPosInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tlpTextLenPosInfo.Name = "tlpTextLenPosInfo";
            this.tlpTextLenPosInfo.RowCount = 1;
            this.tlpTextLenPosInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTextLenPosInfo.Size = new System.Drawing.Size(135, 20);
            this.tlpTextLenPosInfo.TabIndex = 8;
            // 
            // TargetTextBoxColumnPositionLabel
            // 
            this.TargetTextBoxColumnPositionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetTextBoxColumnPositionLabel.AutoSize = true;
            this.TargetTextBoxColumnPositionLabel.Location = new System.Drawing.Point(123, 7);
            this.TargetTextBoxColumnPositionLabel.Name = "TargetTextBoxColumnPositionLabel";
            this.TargetTextBoxColumnPositionLabel.Size = new System.Drawing.Size(16, 13);
            this.TargetTextBoxColumnPositionLabel.TabIndex = 14;
            this.TargetTextBoxColumnPositionLabel.Text = "c:";
            // 
            // TargetTextBoxLinePositionLabelData
            // 
            this.TargetTextBoxLinePositionLabelData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetTextBoxLinePositionLabelData.AutoSize = true;
            this.TargetTextBoxLinePositionLabelData.Location = new System.Drawing.Point(89, 7);
            this.TargetTextBoxLinePositionLabelData.Name = "TargetTextBoxLinePositionLabelData";
            this.TargetTextBoxLinePositionLabelData.Size = new System.Drawing.Size(25, 13);
            this.TargetTextBoxLinePositionLabelData.TabIndex = 11;
            this.TargetTextBoxLinePositionLabelData.Text = "999";
            // 
            // TranslationLongestLineLenghtLabel
            // 
            this.TranslationLongestLineLenghtLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TranslationLongestLineLenghtLabel.AutoSize = true;
            this.TranslationLongestLineLenghtLabel.Location = new System.Drawing.Point(23, 7);
            this.TranslationLongestLineLenghtLabel.Name = "TranslationLongestLineLenghtLabel";
            this.TranslationLongestLineLenghtLabel.Size = new System.Drawing.Size(37, 13);
            this.TranslationLongestLineLenghtLabel.TabIndex = 9;
            this.TranslationLongestLineLenghtLabel.Text = "99999";
            // 
            // TargetTextBoxLinePositionLabel
            // 
            this.TargetTextBoxLinePositionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetTextBoxLinePositionLabel.AutoSize = true;
            this.TargetTextBoxLinePositionLabel.Location = new System.Drawing.Point(66, 7);
            this.TargetTextBoxLinePositionLabel.Name = "TargetTextBoxLinePositionLabel";
            this.TargetTextBoxLinePositionLabel.Size = new System.Drawing.Size(13, 13);
            this.TargetTextBoxLinePositionLabel.TabIndex = 12;
            this.TargetTextBoxLinePositionLabel.Text = "r:";
            // 
            // RTBInfoLengthLabel
            // 
            this.RTBInfoLengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RTBInfoLengthLabel.AutoSize = true;
            this.RTBInfoLengthLabel.Location = new System.Drawing.Point(3, 7);
            this.RTBInfoLengthLabel.Name = "RTBInfoLengthLabel";
            this.RTBInfoLengthLabel.Size = new System.Drawing.Size(12, 13);
            this.RTBInfoLengthLabel.TabIndex = 10;
            this.RTBInfoLengthLabel.Text = "l:";
            // 
            // TargetTextBoxColumnPositionLabelData
            // 
            this.TargetTextBoxColumnPositionLabelData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetTextBoxColumnPositionLabelData.AutoSize = true;
            this.TargetTextBoxColumnPositionLabelData.Location = new System.Drawing.Point(145, 7);
            this.TargetTextBoxColumnPositionLabelData.Name = "TargetTextBoxColumnPositionLabelData";
            this.TargetTextBoxColumnPositionLabelData.Size = new System.Drawing.Size(25, 13);
            this.TargetTextBoxColumnPositionLabelData.TabIndex = 13;
            this.TargetTextBoxColumnPositionLabelData.Text = "999";
            // 
            // TableCompleteInfoLabel
            // 
            this.TableCompleteInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TableCompleteInfoLabel.AutoSize = true;
            this.TableCompleteInfoLabel.Location = new System.Drawing.Point(763, 7);
            this.TableCompleteInfoLabel.Name = "TableCompleteInfoLabel";
            this.TableCompleteInfoLabel.Size = new System.Drawing.Size(24, 13);
            this.TableCompleteInfoLabel.TabIndex = 8;
            this.TableCompleteInfoLabel.Text = "0/0";
            this.TableCompleteInfoLabel.Click += new System.EventHandler(this.TableCompleteInfoLabel_Click);
            // 
            // mainFormRootWorkspaceLogContainer
            // 
            this.mainFormRootWorkspaceLogContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainFormRootWorkspaceLogContainer.Location = new System.Drawing.Point(0, 24);
            this.mainFormRootWorkspaceLogContainer.Name = "mainFormRootWorkspaceLogContainer";
            this.mainFormRootWorkspaceLogContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainFormRootWorkspaceLogContainer.Panel1
            // 
            this.mainFormRootWorkspaceLogContainer.Panel1.Controls.Add(this.frmMainPanel);
            // 
            // mainFormRootWorkspaceLogContainer.Panel2
            // 
            this.mainFormRootWorkspaceLogContainer.Panel2.Controls.Add(this.rtbLog);
            this.mainFormRootWorkspaceLogContainer.Panel2.Controls.Add(this.logSplitter);
            this.mainFormRootWorkspaceLogContainer.Size = new System.Drawing.Size(790, 485);
            this.mainFormRootWorkspaceLogContainer.SplitterDistance = 425;
            this.mainFormRootWorkspaceLogContainer.TabIndex = 17;
            // 
            // rtbLog
            // 
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Location = new System.Drawing.Point(0, 3);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(790, 53);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            // 
            // logSplitter
            // 
            this.logSplitter.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.logSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.logSplitter.Location = new System.Drawing.Point(0, 0);
            this.logSplitter.Margin = new System.Windows.Forms.Padding(1);
            this.logSplitter.Name = "logSplitter";
            this.logSplitter.Size = new System.Drawing.Size(790, 3);
            this.logSplitter.TabIndex = 0;
            this.logSplitter.TabStop = false;
            // 
            // FormMain
            // 
            this.AccessibleDescription = "Program help with translation of some RPG games";
            this.AccessibleName = "Translation Helper";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 509);
            this.Controls.Add(this.mainFormRootWorkspaceLogContainer);
            this.Controls.Add(this.MainMenus);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.MainMenus;
            this.Name = "FormMain";
            this.Text = "Translation Helper by DenisK";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.THMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.THfrmMain_FormClosed);
            this.Load += new System.EventHandler(this.THMain_Load);
            this.MainMenus.ResumeLayout(false);
            this.MainMenus.PerformLayout();
            this.frmMainPanel.ResumeLayout(false);
            this.tlpFrmMain.ResumeLayout(false);
            this.THWorkSpaceSplitContainer.Panel1.ResumeLayout(false);
            this.THWorkSpaceSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THWorkSpaceSplitContainer)).EndInit();
            this.THWorkSpaceSplitContainer.ResumeLayout(false);
            this.THFilesElementsPanel.ResumeLayout(false);
            this.THsplitContainerFilesElements.Panel1.ResumeLayout(false);
            this.THsplitContainerFilesElements.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THsplitContainerFilesElements)).EndInit();
            this.THsplitContainerFilesElements.ResumeLayout(false);
            this.THFilesListPanel.ResumeLayout(false);
            this.tlpFileElements.ResumeLayout(false);
            this.tlpFileElementsFilterAndReset.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THFiltersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THFileElementsDataGridView)).EndInit();
            this.THTextInfoAndEditPanel.ResumeLayout(false);
            this.THInfoEditSplitContainer.Panel1.ResumeLayout(false);
            this.THInfoEditSplitContainer.Panel1.PerformLayout();
            this.THInfoEditSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THInfoEditSplitContainer)).EndInit();
            this.THInfoEditSplitContainer.ResumeLayout(false);
            this.THEditElementsSplitContainer.Panel1.ResumeLayout(false);
            this.THEditElementsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THEditElementsSplitContainer)).EndInit();
            this.THEditElementsSplitContainer.ResumeLayout(false);
            this.tlpWorkInfo.ResumeLayout(false);
            this.tlpWorkInfo.PerformLayout();
            this.tlpTextLenPosInfo.ResumeLayout(false);
            this.tlpTextLenPosInfo.PerformLayout();
            this.mainFormRootWorkspaceLogContainer.Panel1.ResumeLayout(false);
            this.mainFormRootWorkspaceLogContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainFormRootWorkspaceLogContainer)).EndInit();
            this.mainFormRootWorkspaceLogContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedForceToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip FilesListMenus;
        internal System.Windows.Forms.MenuStrip MainMenus;
        internal System.Windows.Forms.ContextMenuStrip RowMenus;
        private System.Windows.Forms.ToolStripMenuItem mainMenusHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        internal System.Windows.Forms.Panel frmMainPanel;
        private System.Windows.Forms.TableLayoutPanel tlpFrmMain;
        internal System.Windows.Forms.SplitContainer THWorkSpaceSplitContainer;
        private System.Windows.Forms.Panel THFilesElementsPanel;
        private System.Windows.Forms.SplitContainer THsplitContainerFilesElements;
        private System.Windows.Forms.Panel THFilesListPanel;
        public System.Windows.Forms.ListBox THFilesList;
        private System.Windows.Forms.TableLayoutPanel tlpFileElements;
        private System.Windows.Forms.TableLayoutPanel tlpFileElementsFilterAndReset;
        internal System.Windows.Forms.DataGridView THFiltersDataGridView;
        public System.Windows.Forms.Button THbtnMainResetTable;
        public System.Windows.Forms.DataGridView THFileElementsDataGridView;
        private System.Windows.Forms.Panel THTextInfoAndEditPanel;
        private System.Windows.Forms.SplitContainer THInfoEditSplitContainer;
        internal System.Windows.Forms.TextBox THInfoTextBox;
        private System.Windows.Forms.SplitContainer THEditElementsSplitContainer;
        public System.Windows.Forms.RichTextBox THSourceRichTextBox;
        public System.Windows.Forms.RichTextBox THTargetRichTextBox;
        private System.Windows.Forms.TableLayoutPanel tlpWorkInfo;
        internal System.Windows.Forms.TableLayoutPanel tlpTextLenPosInfo;
        internal System.Windows.Forms.Label TargetTextBoxColumnPositionLabel;
        internal System.Windows.Forms.Label TargetTextBoxLinePositionLabelData;
        internal System.Windows.Forms.Label TranslationLongestLineLenghtLabel;
        internal System.Windows.Forms.Label TargetTextBoxLinePositionLabel;
        internal System.Windows.Forms.Label RTBInfoLengthLabel;
        internal System.Windows.Forms.Label TargetTextBoxColumnPositionLabelData;
        internal System.Windows.Forms.Label TableCompleteInfoLabel;
        private System.Windows.Forms.SplitContainer mainFormRootWorkspaceLogContainer;
        private System.Windows.Forms.Splitter logSplitter;
        public System.Windows.Forms.RichTextBox rtbLog;
    }
}


namespace TranslationHelper
{
    partial class THMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeTranslationInGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTranslationFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tryToTranslateOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commitChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertToPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.THsplitContainerFilesElements = new System.Windows.Forms.SplitContainer();
            this.THFilesListBox = new System.Windows.Forms.ListBox();
            this.THFileElementsDataGridView = new System.Windows.Forms.DataGridView();
            this.THFiltersDataGridView = new System.Windows.Forms.DataGridView();
            this.THEditElementsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.THSourceTextBox = new System.Windows.Forms.TextBox();
            this.THTargetTextBox = new System.Windows.Forms.TextBox();
            this.THWorkSpaceSplitContainer = new System.Windows.Forms.SplitContainer();
            this.THInfoEditSplitContainer = new System.Windows.Forms.SplitContainer();
            this.THInfoTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.THActionProgressBar = new System.Windows.Forms.ProgressBar();
            this.THInfolabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THsplitContainerFilesElements)).BeginInit();
            this.THsplitContainerFilesElements.Panel1.SuspendLayout();
            this.THsplitContainerFilesElements.Panel2.SuspendLayout();
            this.THsplitContainerFilesElements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THFileElementsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THFiltersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THEditElementsSplitContainer)).BeginInit();
            this.THEditElementsSplitContainer.Panel1.SuspendLayout();
            this.THEditElementsSplitContainer.Panel2.SuspendLayout();
            this.THEditElementsSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THWorkSpaceSplitContainer)).BeginInit();
            this.THWorkSpaceSplitContainer.Panel1.SuspendLayout();
            this.THWorkSpaceSplitContainer.Panel2.SuspendLayout();
            this.THWorkSpaceSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THInfoEditSplitContainer)).BeginInit();
            this.THInfoEditSplitContainer.Panel1.SuspendLayout();
            this.THInfoEditSplitContainer.Panel2.SuspendLayout();
            this.THInfoEditSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(790, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.writeTranslationInGameToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.loadTranslationFromToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Visible = false;
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // writeTranslationInGameToolStripMenuItem
            // 
            this.writeTranslationInGameToolStripMenuItem.Enabled = false;
            this.writeTranslationInGameToolStripMenuItem.Name = "writeTranslationInGameToolStripMenuItem";
            this.writeTranslationInGameToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.writeTranslationInGameToolStripMenuItem.Text = "Write translation in game";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toXmlToolStripMenuItem});
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.exportToolStripMenuItem.Text = "Export translation to";
            // 
            // toXmlToolStripMenuItem
            // 
            this.toXmlToolStripMenuItem.Name = "toXmlToolStripMenuItem";
            this.toXmlToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.toXmlToolStripMenuItem.Text = "Xml";
            // 
            // loadTranslationFromToolStripMenuItem
            // 
            this.loadTranslationFromToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xmlToolStripMenuItem});
            this.loadTranslationFromToolStripMenuItem.Enabled = false;
            this.loadTranslationFromToolStripMenuItem.Name = "loadTranslationFromToolStripMenuItem";
            this.loadTranslationFromToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.loadTranslationFromToolStripMenuItem.Text = "Import translation from";
            // 
            // xmlToolStripMenuItem
            // 
            this.xmlToolStripMenuItem.Name = "xmlToolStripMenuItem";
            this.xmlToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.xmlToolStripMenuItem.Text = "Xml";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInWebToolStripMenuItem,
            this.tryToTranslateOnlineToolStripMenuItem,
            this.gitToolStripMenuItem});
            this.editToolStripMenuItem.Enabled = false;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // openInWebToolStripMenuItem
            // 
            this.openInWebToolStripMenuItem.Enabled = false;
            this.openInWebToolStripMenuItem.Name = "openInWebToolStripMenuItem";
            this.openInWebToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.openInWebToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.openInWebToolStripMenuItem.Text = "Open in web";
            // 
            // tryToTranslateOnlineToolStripMenuItem
            // 
            this.tryToTranslateOnlineToolStripMenuItem.Enabled = false;
            this.tryToTranslateOnlineToolStripMenuItem.Name = "tryToTranslateOnlineToolStripMenuItem";
            this.tryToTranslateOnlineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F12)));
            this.tryToTranslateOnlineToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.tryToTranslateOnlineToolStripMenuItem.Text = "Try to translate online";
            // 
            // gitToolStripMenuItem
            // 
            this.gitToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSelectedToolStripMenuItem,
            this.commitChangesToolStripMenuItem,
            this.revertToPreviousToolStripMenuItem});
            this.gitToolStripMenuItem.Enabled = false;
            this.gitToolStripMenuItem.Name = "gitToolStripMenuItem";
            this.gitToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.gitToolStripMenuItem.Text = "Git";
            // 
            // addSelectedToolStripMenuItem
            // 
            this.addSelectedToolStripMenuItem.Name = "addSelectedToolStripMenuItem";
            this.addSelectedToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.addSelectedToolStripMenuItem.Text = "Add current project";
            // 
            // commitChangesToolStripMenuItem
            // 
            this.commitChangesToolStripMenuItem.Enabled = false;
            this.commitChangesToolStripMenuItem.Name = "commitChangesToolStripMenuItem";
            this.commitChangesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.commitChangesToolStripMenuItem.Text = "Commit changes";
            // 
            // revertToPreviousToolStripMenuItem
            // 
            this.revertToPreviousToolStripMenuItem.Enabled = false;
            this.revertToPreviousToolStripMenuItem.Name = "revertToPreviousToolStripMenuItem";
            this.revertToPreviousToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.revertToPreviousToolStripMenuItem.Text = "Revert to previous";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Enabled = false;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // THsplitContainerFilesElements
            // 
            this.THsplitContainerFilesElements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THsplitContainerFilesElements.Location = new System.Drawing.Point(3, 3);
            this.THsplitContainerFilesElements.Name = "THsplitContainerFilesElements";
            // 
            // THsplitContainerFilesElements.Panel1
            // 
            this.THsplitContainerFilesElements.Panel1.Controls.Add(this.THFilesListBox);
            // 
            // THsplitContainerFilesElements.Panel2
            // 
            this.THsplitContainerFilesElements.Panel2.Controls.Add(this.THFileElementsDataGridView);
            this.THsplitContainerFilesElements.Panel2.Controls.Add(this.THFiltersDataGridView);
            this.THsplitContainerFilesElements.Size = new System.Drawing.Size(773, 369);
            this.THsplitContainerFilesElements.SplitterDistance = 125;
            this.THsplitContainerFilesElements.TabIndex = 3;
            // 
            // THFilesListBox
            // 
            this.THFilesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THFilesListBox.FormattingEnabled = true;
            this.THFilesListBox.Location = new System.Drawing.Point(0, 0);
            this.THFilesListBox.Name = "THFilesListBox";
            this.THFilesListBox.Size = new System.Drawing.Size(125, 381);
            this.THFilesListBox.TabIndex = 0;
            this.THFilesListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.THFilesListBox_MouseClick);
            // 
            // THFileElementsDataGridView
            // 
            this.THFileElementsDataGridView.AllowUserToAddRows = false;
            this.THFileElementsDataGridView.AllowUserToDeleteRows = false;
            this.THFileElementsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THFileElementsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.THFileElementsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.THFileElementsDataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.THFileElementsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.THFileElementsDataGridView.Location = new System.Drawing.Point(0, 17);
            this.THFileElementsDataGridView.Name = "THFileElementsDataGridView";
            this.THFileElementsDataGridView.RowTemplate.Height = 23;
            this.THFileElementsDataGridView.Size = new System.Drawing.Size(641, 352);
            this.THFileElementsDataGridView.TabIndex = 2;
            this.THFileElementsDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFileElementsDataGridView_CellEnter);
            this.THFileElementsDataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.THFileElementsDataGridView_CellValueNeeded);
            this.THFileElementsDataGridView.NewRowNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.THFileElementsDataGridView_NewRowNeeded);
            this.THFileElementsDataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.THFileElementsDataGridView_RowPostPaint);
            this.THFileElementsDataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.THFileElementsDataGridView_RowsAdded);
            this.THFileElementsDataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.THFileElementsDataGridView_Scroll);
            // 
            // THFiltersDataGridView
            // 
            this.THFiltersDataGridView.AllowUserToAddRows = false;
            this.THFiltersDataGridView.AllowUserToDeleteRows = false;
            this.THFiltersDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THFiltersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.THFiltersDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.THFiltersDataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.THFiltersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.THFiltersDataGridView.ColumnHeadersVisible = false;
            this.THFiltersDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.THFiltersDataGridView.Enabled = false;
            this.THFiltersDataGridView.Location = new System.Drawing.Point(0, 0);
            this.THFiltersDataGridView.Name = "THFiltersDataGridView";
            this.THFiltersDataGridView.RowTemplate.Height = 23;
            this.THFiltersDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.THFiltersDataGridView.Size = new System.Drawing.Size(641, 21);
            this.THFiltersDataGridView.TabIndex = 3;
            this.THFiltersDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.THFiltersDataGridView_CellValueChanged);
            // 
            // THEditElementsSplitContainer
            // 
            this.THEditElementsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THEditElementsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.THEditElementsSplitContainer.Name = "THEditElementsSplitContainer";
            // 
            // THEditElementsSplitContainer.Panel1
            // 
            this.THEditElementsSplitContainer.Panel1.Controls.Add(this.THSourceTextBox);
            // 
            // THEditElementsSplitContainer.Panel2
            // 
            this.THEditElementsSplitContainer.Panel2.Controls.Add(this.THTargetTextBox);
            this.THEditElementsSplitContainer.Size = new System.Drawing.Size(639, 97);
            this.THEditElementsSplitContainer.SplitterDistance = 291;
            this.THEditElementsSplitContainer.TabIndex = 4;
            // 
            // THSourceTextBox
            // 
            this.THSourceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSourceTextBox.Enabled = false;
            this.THSourceTextBox.Font = new System.Drawing.Font("Arial Unicode MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.THSourceTextBox.Location = new System.Drawing.Point(0, 0);
            this.THSourceTextBox.Multiline = true;
            this.THSourceTextBox.Name = "THSourceTextBox";
            this.THSourceTextBox.ReadOnly = true;
            this.THSourceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.THSourceTextBox.Size = new System.Drawing.Size(291, 97);
            this.THSourceTextBox.TabIndex = 1;
            // 
            // THTargetTextBox
            // 
            this.THTargetTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.THTargetTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.THTargetTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THTargetTextBox.Enabled = false;
            this.THTargetTextBox.Location = new System.Drawing.Point(0, 0);
            this.THTargetTextBox.Multiline = true;
            this.THTargetTextBox.Name = "THTargetTextBox";
            this.THTargetTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.THTargetTextBox.Size = new System.Drawing.Size(344, 97);
            this.THTargetTextBox.TabIndex = 0;
            this.THTargetTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.THTargetTextBox_KeyDown);
            // 
            // THWorkSpaceSplitContainer
            // 
            this.THWorkSpaceSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THWorkSpaceSplitContainer.Location = new System.Drawing.Point(12, 27);
            this.THWorkSpaceSplitContainer.Name = "THWorkSpaceSplitContainer";
            this.THWorkSpaceSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // THWorkSpaceSplitContainer.Panel1
            // 
            this.THWorkSpaceSplitContainer.Panel1.Controls.Add(this.THsplitContainerFilesElements);
            // 
            // THWorkSpaceSplitContainer.Panel2
            // 
            this.THWorkSpaceSplitContainer.Panel2.Controls.Add(this.THInfoEditSplitContainer);
            this.THWorkSpaceSplitContainer.Size = new System.Drawing.Size(773, 470);
            this.THWorkSpaceSplitContainer.SplitterDistance = 371;
            this.THWorkSpaceSplitContainer.TabIndex = 5;
            // 
            // THInfoEditSplitContainer
            // 
            this.THInfoEditSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THInfoEditSplitContainer.Location = new System.Drawing.Point(0, -2);
            this.THInfoEditSplitContainer.Name = "THInfoEditSplitContainer";
            // 
            // THInfoEditSplitContainer.Panel1
            // 
            this.THInfoEditSplitContainer.Panel1.Controls.Add(this.THInfoTextBox);
            // 
            // THInfoEditSplitContainer.Panel2
            // 
            this.THInfoEditSplitContainer.Panel2.Controls.Add(this.THEditElementsSplitContainer);
            this.THInfoEditSplitContainer.Size = new System.Drawing.Size(770, 97);
            this.THInfoEditSplitContainer.SplitterDistance = 127;
            this.THInfoEditSplitContainer.TabIndex = 5;
            // 
            // THInfoTextBox
            // 
            this.THInfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THInfoTextBox.Location = new System.Drawing.Point(0, 0);
            this.THInfoTextBox.Multiline = true;
            this.THInfoTextBox.Name = "THInfoTextBox";
            this.THInfoTextBox.ReadOnly = true;
            this.THInfoTextBox.Size = new System.Drawing.Size(127, 97);
            this.THInfoTextBox.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // THActionProgressBar
            // 
            this.THActionProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.THActionProgressBar.Location = new System.Drawing.Point(50, 498);
            this.THActionProgressBar.MarqueeAnimationSpeed = 50;
            this.THActionProgressBar.Name = "THActionProgressBar";
            this.THActionProgressBar.Size = new System.Drawing.Size(82, 10);
            this.THActionProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.THActionProgressBar.TabIndex = 6;
            this.THActionProgressBar.Visible = false;
            // 
            // THInfolabel
            // 
            this.THInfolabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.THInfolabel.AutoSize = true;
            this.THInfolabel.Location = new System.Drawing.Point(9, 495);
            this.THInfolabel.Name = "THInfolabel";
            this.THInfolabel.Size = new System.Drawing.Size(19, 13);
            this.THInfolabel.TabIndex = 7;
            this.THInfolabel.Text = "...";
            this.THInfolabel.Visible = false;
            // 
            // THMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 509);
            this.Controls.Add(this.THInfolabel);
            this.Controls.Add(this.THActionProgressBar);
            this.Controls.Add(this.THWorkSpaceSplitContainer);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "THMain";
            this.Text = "Translation Helper by DenisK";
            this.Load += new System.EventHandler(this.THMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.THsplitContainerFilesElements.Panel1.ResumeLayout(false);
            this.THsplitContainerFilesElements.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THsplitContainerFilesElements)).EndInit();
            this.THsplitContainerFilesElements.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THFileElementsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THFiltersDataGridView)).EndInit();
            this.THEditElementsSplitContainer.Panel1.ResumeLayout(false);
            this.THEditElementsSplitContainer.Panel1.PerformLayout();
            this.THEditElementsSplitContainer.Panel2.ResumeLayout(false);
            this.THEditElementsSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.THEditElementsSplitContainer)).EndInit();
            this.THEditElementsSplitContainer.ResumeLayout(false);
            this.THWorkSpaceSplitContainer.Panel1.ResumeLayout(false);
            this.THWorkSpaceSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THWorkSpaceSplitContainer)).EndInit();
            this.THWorkSpaceSplitContainer.ResumeLayout(false);
            this.THInfoEditSplitContainer.Panel1.ResumeLayout(false);
            this.THInfoEditSplitContainer.Panel1.PerformLayout();
            this.THInfoEditSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.THInfoEditSplitContainer)).EndInit();
            this.THInfoEditSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.SplitContainer THsplitContainerFilesElements;
        private System.Windows.Forms.SplitContainer THEditElementsSplitContainer;
        private System.Windows.Forms.TextBox THSourceTextBox;
        private System.Windows.Forms.TextBox THTargetTextBox;
        private System.Windows.Forms.SplitContainer THWorkSpaceSplitContainer;
        private System.Windows.Forms.SplitContainer THInfoEditSplitContainer;
        public System.Windows.Forms.DataGridView THFileElementsDataGridView;
        public System.Windows.Forms.ListBox THFilesListBox;
        private System.Windows.Forms.TextBox THInfoTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem writeTranslationInGameToolStripMenuItem;
        private System.Windows.Forms.DataGridView THFiltersDataGridView;
        private System.Windows.Forms.ToolStripMenuItem openInWebToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tryToTranslateOnlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commitChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revertToPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTranslationFromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xmlToolStripMenuItem;
        public System.Windows.Forms.ProgressBar THActionProgressBar;
        public System.Windows.Forms.Label THInfolabel;
    }
}


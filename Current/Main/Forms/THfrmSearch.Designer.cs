namespace TranslationHelper
{
    partial class THfrmSearch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.THSearchTabs = new System.Windows.Forms.TabControl();
            this.THSearch1st = new System.Windows.Forms.TabPage();
            this.SearchResultsPanel = new System.Windows.Forms.Panel();
            this.SearchResultsDatagridview = new System.Windows.Forms.DataGridView();
            this.lblSearchMsg = new System.Windows.Forms.Label();
            this.THSearchPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ConfirmReplaceAllCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchAlwaysOnTopCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkbxDoNotTouchEqualOT = new System.Windows.Forms.CheckBox();
            this.THSearchMatchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchInInfoCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchFindLinesWithPossibleIssuesCheckBox = new System.Windows.Forms.CheckBox();
            this.ClearReplaceWithTextBoxLabel = new System.Windows.Forms.Label();
            this.ClearFindWhatTextBoxLabel = new System.Windows.Forms.Label();
            this.SearchModeGroupBox = new System.Windows.Forms.GroupBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.SearchModeNormalRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchModeRegexRadioButton = new System.Windows.Forms.RadioButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.SearchRangeTableRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchRangeAllRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.SearchMethodOriginalToTranslationRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchMethodTranslationRadioButton = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.FindAllButton = new System.Windows.Forms.Button();
            this.SearchFormFindNextButton = new System.Windows.Forms.Button();
            this.SearchFormReplaceButton = new System.Windows.Forms.Button();
            this.SearchFormReplaceAllButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.SearchFormReplaceWithTextBox = new System.Windows.Forms.TextBox();
            this.SearchFormFindWhatTextBox = new System.Windows.Forms.TextBox();
            this.THSearchFindWhatLabel = new System.Windows.Forms.Label();
            this.SearchFormFindWhatComboBox = new System.Windows.Forms.ComboBox();
            this.SearchFormReplaceWithComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchRangeSelectedRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchRangeVisibleRadioButton = new System.Windows.Forms.RadioButton();
            this.THSearchTabs.SuspendLayout();
            this.THSearch1st.SuspendLayout();
            this.SearchResultsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SearchResultsDatagridview)).BeginInit();
            this.THSearchPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SearchModeGroupBox.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // THSearchTabs
            // 
            this.THSearchTabs.Controls.Add(this.THSearch1st);
            this.THSearchTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSearchTabs.Location = new System.Drawing.Point(0, 0);
            this.THSearchTabs.Name = "THSearchTabs";
            this.THSearchTabs.SelectedIndex = 0;
            this.THSearchTabs.Size = new System.Drawing.Size(580, 555);
            this.THSearchTabs.TabIndex = 2;
            // 
            // THSearch1st
            // 
            this.THSearch1st.Controls.Add(this.SearchResultsPanel);
            this.THSearch1st.Controls.Add(this.lblSearchMsg);
            this.THSearch1st.Controls.Add(this.THSearchPanel);
            this.THSearch1st.Location = new System.Drawing.Point(4, 22);
            this.THSearch1st.Name = "THSearch1st";
            this.THSearch1st.Padding = new System.Windows.Forms.Padding(3);
            this.THSearch1st.Size = new System.Drawing.Size(572, 529);
            this.THSearch1st.TabIndex = 0;
            this.THSearch1st.Text = "Find and Replace";
            this.THSearch1st.UseVisualStyleBackColor = true;
            // 
            // SearchResultsPanel
            // 
            this.SearchResultsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchResultsPanel.Controls.Add(this.SearchResultsDatagridview);
            this.SearchResultsPanel.Location = new System.Drawing.Point(3, 310);
            this.SearchResultsPanel.Name = "SearchResultsPanel";
            this.SearchResultsPanel.Size = new System.Drawing.Size(566, 218);
            this.SearchResultsPanel.TabIndex = 20;
            this.SearchResultsPanel.Visible = false;
            // 
            // SearchResultsDatagridview
            // 
            this.SearchResultsDatagridview.AllowUserToAddRows = false;
            this.SearchResultsDatagridview.AllowUserToDeleteRows = false;
            this.SearchResultsDatagridview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SearchResultsDatagridview.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.SearchResultsDatagridview.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.SearchResultsDatagridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SearchResultsDatagridview.ColumnHeadersVisible = false;
            this.SearchResultsDatagridview.Location = new System.Drawing.Point(0, 3);
            this.SearchResultsDatagridview.Name = "SearchResultsDatagridview";
            this.SearchResultsDatagridview.ReadOnly = true;
            this.SearchResultsDatagridview.RowTemplate.Height = 23;
            this.SearchResultsDatagridview.Size = new System.Drawing.Size(565, 216);
            this.SearchResultsDatagridview.TabIndex = 19;
            this.SearchResultsDatagridview.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SearchResultsDatagridview_CellClick_1);
            this.SearchResultsDatagridview.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.SearchResultsDatagridview_CellEnter);
            // 
            // lblError
            // 
            this.lblSearchMsg.AutoSize = true;
            this.lblSearchMsg.Location = new System.Drawing.Point(6, 294);
            this.lblSearchMsg.Name = "lblError";
            this.lblSearchMsg.Size = new System.Drawing.Size(25, 13);
            this.lblSearchMsg.TabIndex = 18;
            this.lblSearchMsg.Text = "info";
            this.lblSearchMsg.Visible = false;
            // 
            // THSearchPanel
            // 
            this.THSearchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THSearchPanel.Controls.Add(this.tableLayoutPanel2);
            this.THSearchPanel.Controls.Add(this.tableLayoutPanel1);
            this.THSearchPanel.Controls.Add(this.ClearReplaceWithTextBoxLabel);
            this.THSearchPanel.Controls.Add(this.ClearFindWhatTextBoxLabel);
            this.THSearchPanel.Controls.Add(this.SearchModeGroupBox);
            this.THSearchPanel.Controls.Add(this.panel4);
            this.THSearchPanel.Controls.Add(this.panel3);
            this.THSearchPanel.Location = new System.Drawing.Point(3, 3);
            this.THSearchPanel.Name = "THSearchPanel";
            this.THSearchPanel.Size = new System.Drawing.Size(565, 304);
            this.THSearchPanel.TabIndex = 7;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.ConfirmReplaceAllCheckBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.SearchAlwaysOnTopCheckBox, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(438, 132);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(108, 47);
            this.tableLayoutPanel2.TabIndex = 28;
            // 
            // ConfirmReplaceAllCheckBox
            // 
            this.ConfirmReplaceAllCheckBox.AutoSize = true;
            this.ConfirmReplaceAllCheckBox.Checked = true;
            this.ConfirmReplaceAllCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ConfirmReplaceAllCheckBox.Location = new System.Drawing.Point(3, 3);
            this.ConfirmReplaceAllCheckBox.Name = "ConfirmReplaceAllCheckBox";
            this.ConfirmReplaceAllCheckBox.Size = new System.Drawing.Size(87, 17);
            this.ConfirmReplaceAllCheckBox.TabIndex = 23;
            this.ConfirmReplaceAllCheckBox.Text = "Confirmation";
            this.ConfirmReplaceAllCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchAlwaysOnTopCheckBox
            // 
            this.SearchAlwaysOnTopCheckBox.AutoSize = true;
            this.SearchAlwaysOnTopCheckBox.Checked = true;
            this.SearchAlwaysOnTopCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SearchAlwaysOnTopCheckBox.Location = new System.Drawing.Point(3, 26);
            this.SearchAlwaysOnTopCheckBox.Name = "SearchAlwaysOnTopCheckBox";
            this.SearchAlwaysOnTopCheckBox.Size = new System.Drawing.Size(96, 17);
            this.SearchAlwaysOnTopCheckBox.TabIndex = 21;
            this.SearchAlwaysOnTopCheckBox.Text = "Always on Top";
            this.SearchAlwaysOnTopCheckBox.UseVisualStyleBackColor = true;
            this.SearchAlwaysOnTopCheckBox.CheckedChanged += new System.EventHandler(this.SearchAlwaysOnTopCheckBox_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chkbxDoNotTouchEqualOT, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.THSearchMatchCaseCheckBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SearchInInfoCheckBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SearchFindLinesWithPossibleIssuesCheckBox, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(135, 73);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(283, 102);
            this.tableLayoutPanel1.TabIndex = 27;
            // 
            // ckkbxDoNotTouchEqualOT
            // 
            this.chkbxDoNotTouchEqualOT.AutoSize = true;
            this.chkbxDoNotTouchEqualOT.Checked = true;
            this.chkbxDoNotTouchEqualOT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbxDoNotTouchEqualOT.Location = new System.Drawing.Point(3, 72);
            this.chkbxDoNotTouchEqualOT.Name = "ckkbxDoNotTouchEqualOT";
            this.chkbxDoNotTouchEqualOT.Size = new System.Drawing.Size(182, 17);
            this.chkbxDoNotTouchEqualOT.TabIndex = 27;
            this.chkbxDoNotTouchEqualOT.Text = "Ignore Original=Translation lines";
            this.chkbxDoNotTouchEqualOT.UseVisualStyleBackColor = true;
            // 
            // THSearchMatchCaseCheckBox
            // 
            this.THSearchMatchCaseCheckBox.AutoSize = true;
            this.THSearchMatchCaseCheckBox.Location = new System.Drawing.Point(3, 3);
            this.THSearchMatchCaseCheckBox.Name = "THSearchMatchCaseCheckBox";
            this.THSearchMatchCaseCheckBox.Size = new System.Drawing.Size(82, 17);
            this.THSearchMatchCaseCheckBox.TabIndex = 19;
            this.THSearchMatchCaseCheckBox.Text = "Match Case";
            this.THSearchMatchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchInInfoCheckBox
            // 
            this.SearchInInfoCheckBox.AutoSize = true;
            this.SearchInInfoCheckBox.Location = new System.Drawing.Point(3, 26);
            this.SearchInInfoCheckBox.Name = "SearchInInfoCheckBox";
            this.SearchInInfoCheckBox.Size = new System.Drawing.Size(59, 17);
            this.SearchInInfoCheckBox.TabIndex = 26;
            this.SearchInInfoCheckBox.Text = "In Info";
            this.SearchInInfoCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchFindLinesWithPossibleIssuesCheckBox
            // 
            this.SearchFindLinesWithPossibleIssuesCheckBox.AutoSize = true;
            this.SearchFindLinesWithPossibleIssuesCheckBox.Location = new System.Drawing.Point(3, 49);
            this.SearchFindLinesWithPossibleIssuesCheckBox.Name = "SearchFindLinesWithPossibleIssuesCheckBox";
            this.SearchFindLinesWithPossibleIssuesCheckBox.Size = new System.Drawing.Size(166, 17);
            this.SearchFindLinesWithPossibleIssuesCheckBox.TabIndex = 22;
            this.SearchFindLinesWithPossibleIssuesCheckBox.Text = "Find lines with possible issues";
            this.SearchFindLinesWithPossibleIssuesCheckBox.UseVisualStyleBackColor = true;
            this.SearchFindLinesWithPossibleIssuesCheckBox.CheckedChanged += new System.EventHandler(this.SearchFindLinesWithPossibleIssuesCheckBox_CheckedChanged);
            // 
            // ClearReplaceWithTextBoxLabel
            // 
            this.ClearReplaceWithTextBoxLabel.AutoSize = true;
            this.ClearReplaceWithTextBoxLabel.Location = new System.Drawing.Point(419, 45);
            this.ClearReplaceWithTextBoxLabel.Name = "ClearReplaceWithTextBoxLabel";
            this.ClearReplaceWithTextBoxLabel.Size = new System.Drawing.Size(13, 13);
            this.ClearReplaceWithTextBoxLabel.TabIndex = 25;
            this.ClearReplaceWithTextBoxLabel.Text = "x";
            this.ClearReplaceWithTextBoxLabel.Click += new System.EventHandler(this.ClearReplaceWithTextBoxLabel_Click);
            // 
            // ClearFindWhatTextBoxLabel
            // 
            this.ClearFindWhatTextBoxLabel.AutoSize = true;
            this.ClearFindWhatTextBoxLabel.Location = new System.Drawing.Point(419, 18);
            this.ClearFindWhatTextBoxLabel.Name = "ClearFindWhatTextBoxLabel";
            this.ClearFindWhatTextBoxLabel.Size = new System.Drawing.Size(13, 13);
            this.ClearFindWhatTextBoxLabel.TabIndex = 24;
            this.ClearFindWhatTextBoxLabel.Text = "x";
            this.ClearFindWhatTextBoxLabel.Click += new System.EventHandler(this.ClearFindWhatTextBoxLabel_Click);
            // 
            // SearchModeGroupBox
            // 
            this.SearchModeGroupBox.Controls.Add(this.panel6);
            this.SearchModeGroupBox.Location = new System.Drawing.Point(0, 208);
            this.SearchModeGroupBox.Name = "SearchModeGroupBox";
            this.SearchModeGroupBox.Size = new System.Drawing.Size(565, 80);
            this.SearchModeGroupBox.TabIndex = 20;
            this.SearchModeGroupBox.TabStop = false;
            this.SearchModeGroupBox.Text = "Search Mode";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel2);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Controls.Add(this.panel1);
            this.panel6.Location = new System.Drawing.Point(3, 19);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(543, 61);
            this.panel6.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.SearchModeNormalRadioButton);
            this.panel2.Controls.Add(this.SearchModeRegexRadioButton);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(81, 61);
            this.panel2.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Input";
            // 
            // SearchModeNormalRadioButton
            // 
            this.SearchModeNormalRadioButton.AutoSize = true;
            this.SearchModeNormalRadioButton.Checked = true;
            this.SearchModeNormalRadioButton.Location = new System.Drawing.Point(3, 16);
            this.SearchModeNormalRadioButton.Name = "SearchModeNormalRadioButton";
            this.SearchModeNormalRadioButton.Size = new System.Drawing.Size(58, 17);
            this.SearchModeNormalRadioButton.TabIndex = 7;
            this.SearchModeNormalRadioButton.TabStop = true;
            this.SearchModeNormalRadioButton.Text = "Normal";
            this.SearchModeNormalRadioButton.UseVisualStyleBackColor = true;
            this.SearchModeNormalRadioButton.Click += new System.EventHandler(this.SearchModeNormalRadioButton_Click);
            // 
            // SearchModeRegexRadioButton
            // 
            this.SearchModeRegexRadioButton.AutoSize = true;
            this.SearchModeRegexRadioButton.Location = new System.Drawing.Point(3, 39);
            this.SearchModeRegexRadioButton.Name = "SearchModeRegexRadioButton";
            this.SearchModeRegexRadioButton.Size = new System.Drawing.Size(56, 17);
            this.SearchModeRegexRadioButton.TabIndex = 8;
            this.SearchModeRegexRadioButton.Text = "Regex";
            this.SearchModeRegexRadioButton.UseVisualStyleBackColor = true;
            this.SearchModeRegexRadioButton.Click += new System.EventHandler(this.SearchModeRegexRadioButton_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.SearchRangeVisibleRadioButton);
            this.panel5.Controls.Add(this.SearchRangeSelectedRadioButton);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.SearchRangeTableRadioButton);
            this.panel5.Controls.Add(this.SearchRangeAllRadioButton);
            this.panel5.Location = new System.Drawing.Point(376, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(149, 61);
            this.panel5.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Range";
            // 
            // SearchRangeTableRadioButton
            // 
            this.SearchRangeTableRadioButton.AutoSize = true;
            this.SearchRangeTableRadioButton.Checked = true;
            this.SearchRangeTableRadioButton.Location = new System.Drawing.Point(3, 16);
            this.SearchRangeTableRadioButton.Name = "SearchRangeTableRadioButton";
            this.SearchRangeTableRadioButton.Size = new System.Drawing.Size(51, 17);
            this.SearchRangeTableRadioButton.TabIndex = 7;
            this.SearchRangeTableRadioButton.Text = "Table";
            this.SearchRangeTableRadioButton.UseVisualStyleBackColor = true;
            this.SearchRangeTableRadioButton.Click += new System.EventHandler(this.SearchRangeTableRadioButton_Click);
            // 
            // SearchRangeAllRadioButton
            // 
            this.SearchRangeAllRadioButton.AutoSize = true;
            this.SearchRangeAllRadioButton.Location = new System.Drawing.Point(3, 39);
            this.SearchRangeAllRadioButton.Name = "SearchRangeAllRadioButton";
            this.SearchRangeAllRadioButton.Size = new System.Drawing.Size(36, 17);
            this.SearchRangeAllRadioButton.TabIndex = 8;
            this.SearchRangeAllRadioButton.Text = "All";
            this.SearchRangeAllRadioButton.UseVisualStyleBackColor = true;
            this.SearchRangeAllRadioButton.Click += new System.EventHandler(this.SearchRangeAllRadioButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.SearchMethodOriginalToTranslationRadioButton);
            this.panel1.Controls.Add(this.SearchMethodTranslationRadioButton);
            this.panel1.Location = new System.Drawing.Point(114, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 61);
            this.panel1.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, -1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Method";
            // 
            // SearchMethodOriginalToTranslationRadioButton
            // 
            this.SearchMethodOriginalToTranslationRadioButton.AutoSize = true;
            this.SearchMethodOriginalToTranslationRadioButton.Location = new System.Drawing.Point(3, 39);
            this.SearchMethodOriginalToTranslationRadioButton.Name = "SearchMethodOriginalToTranslationRadioButton";
            this.SearchMethodOriginalToTranslationRadioButton.Size = new System.Drawing.Size(215, 17);
            this.SearchMethodOriginalToTranslationRadioButton.TabIndex = 9;
            this.SearchMethodOriginalToTranslationRadioButton.Text = "Find in Original and Paste to Translation";
            this.SearchMethodOriginalToTranslationRadioButton.UseVisualStyleBackColor = true;
            this.SearchMethodOriginalToTranslationRadioButton.Click += new System.EventHandler(this.SearchMethodTranslationRadioButton_Click);
            // 
            // SearchMethodTranslationRadioButton
            // 
            this.SearchMethodTranslationRadioButton.AutoSize = true;
            this.SearchMethodTranslationRadioButton.Checked = true;
            this.SearchMethodTranslationRadioButton.Location = new System.Drawing.Point(3, 16);
            this.SearchMethodTranslationRadioButton.Name = "SearchMethodTranslationRadioButton";
            this.SearchMethodTranslationRadioButton.Size = new System.Drawing.Size(174, 17);
            this.SearchMethodTranslationRadioButton.TabIndex = 10;
            this.SearchMethodTranslationRadioButton.TabStop = true;
            this.SearchMethodTranslationRadioButton.Text = "Find and Replace in Translation";
            this.SearchMethodTranslationRadioButton.UseVisualStyleBackColor = true;
            this.SearchMethodTranslationRadioButton.Click += new System.EventHandler(this.SearchMethodOriginalTranslationRadioButton_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.FindAllButton);
            this.panel4.Controls.Add(this.SearchFormFindNextButton);
            this.panel4.Controls.Add(this.SearchFormReplaceButton);
            this.panel4.Controls.Add(this.SearchFormReplaceAllButton);
            this.panel4.Location = new System.Drawing.Point(438, 13);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(108, 113);
            this.panel4.TabIndex = 16;
            // 
            // FindAllButton
            // 
            this.FindAllButton.Location = new System.Drawing.Point(0, 58);
            this.FindAllButton.Name = "FindAllButton";
            this.FindAllButton.Size = new System.Drawing.Size(108, 23);
            this.FindAllButton.TabIndex = 7;
            this.FindAllButton.Text = "Find All";
            this.FindAllButton.UseVisualStyleBackColor = true;
            this.FindAllButton.Click += new System.EventHandler(this.SearchAllButton_Click);
            // 
            // SearchFormFindNextButton
            // 
            this.SearchFormFindNextButton.Location = new System.Drawing.Point(0, 0);
            this.SearchFormFindNextButton.Name = "SearchFormFindNextButton";
            this.SearchFormFindNextButton.Size = new System.Drawing.Size(108, 23);
            this.SearchFormFindNextButton.TabIndex = 4;
            this.SearchFormFindNextButton.Text = "Find Next";
            this.SearchFormFindNextButton.UseVisualStyleBackColor = true;
            this.SearchFormFindNextButton.Click += new System.EventHandler(this.SearchFormFindNextButton_Click);
            // 
            // SearchFormReplaceButton
            // 
            this.SearchFormReplaceButton.Location = new System.Drawing.Point(0, 29);
            this.SearchFormReplaceButton.Name = "SearchFormReplaceButton";
            this.SearchFormReplaceButton.Size = new System.Drawing.Size(108, 23);
            this.SearchFormReplaceButton.TabIndex = 5;
            this.SearchFormReplaceButton.Text = "Replace";
            this.SearchFormReplaceButton.UseVisualStyleBackColor = true;
            this.SearchFormReplaceButton.Click += new System.EventHandler(this.SearchFormReplaceButton_Click);
            // 
            // SearchFormReplaceAllButton
            // 
            this.SearchFormReplaceAllButton.Location = new System.Drawing.Point(0, 87);
            this.SearchFormReplaceAllButton.Name = "SearchFormReplaceAllButton";
            this.SearchFormReplaceAllButton.Size = new System.Drawing.Size(108, 26);
            this.SearchFormReplaceAllButton.TabIndex = 6;
            this.SearchFormReplaceAllButton.Text = "Replace All";
            this.SearchFormReplaceAllButton.UseVisualStyleBackColor = true;
            this.SearchFormReplaceAllButton.Click += new System.EventHandler(this.SearchFormReplaceAllButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.SearchFormReplaceWithTextBox);
            this.panel3.Controls.Add(this.SearchFormFindWhatTextBox);
            this.panel3.Controls.Add(this.THSearchFindWhatLabel);
            this.panel3.Controls.Add(this.SearchFormFindWhatComboBox);
            this.panel3.Controls.Add(this.SearchFormReplaceWithComboBox);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(54, 13);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(364, 54);
            this.panel3.TabIndex = 15;
            // 
            // SearchFormReplaceWithTextBox
            // 
            this.SearchFormReplaceWithTextBox.Location = new System.Drawing.Point(81, 29);
            this.SearchFormReplaceWithTextBox.Name = "SearchFormReplaceWithTextBox";
            this.SearchFormReplaceWithTextBox.Size = new System.Drawing.Size(260, 21);
            this.SearchFormReplaceWithTextBox.TabIndex = 5;
            // 
            // SearchFormFindWhatTextBox
            // 
            this.SearchFormFindWhatTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.SearchFormFindWhatTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.SearchFormFindWhatTextBox.Location = new System.Drawing.Point(81, 2);
            this.SearchFormFindWhatTextBox.Name = "SearchFormFindWhatTextBox";
            this.SearchFormFindWhatTextBox.Size = new System.Drawing.Size(260, 21);
            this.SearchFormFindWhatTextBox.TabIndex = 4;
            // 
            // THSearchFindWhatLabel
            // 
            this.THSearchFindWhatLabel.AutoSize = true;
            this.THSearchFindWhatLabel.Location = new System.Drawing.Point(17, 5);
            this.THSearchFindWhatLabel.Name = "THSearchFindWhatLabel";
            this.THSearchFindWhatLabel.Size = new System.Drawing.Size(58, 13);
            this.THSearchFindWhatLabel.TabIndex = 1;
            this.THSearchFindWhatLabel.Text = "Find what:";
            // 
            // SearchFormFindWhatComboBox
            // 
            this.SearchFormFindWhatComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.SearchFormFindWhatComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.SearchFormFindWhatComboBox.Location = new System.Drawing.Point(81, 2);
            this.SearchFormFindWhatComboBox.Name = "SearchFormFindWhatComboBox";
            this.SearchFormFindWhatComboBox.Size = new System.Drawing.Size(278, 21);
            this.SearchFormFindWhatComboBox.TabIndex = 0;
            this.SearchFormFindWhatComboBox.SelectedValueChanged += new System.EventHandler(this.SearchFormFindWhatComboBox_SelectedValueChanged);
            // 
            // SearchFormReplaceWithComboBox
            // 
            this.SearchFormReplaceWithComboBox.FormattingEnabled = true;
            this.SearchFormReplaceWithComboBox.Location = new System.Drawing.Point(81, 29);
            this.SearchFormReplaceWithComboBox.Name = "SearchFormReplaceWithComboBox";
            this.SearchFormReplaceWithComboBox.Size = new System.Drawing.Size(278, 21);
            this.SearchFormReplaceWithComboBox.TabIndex = 2;
            this.SearchFormReplaceWithComboBox.SelectedIndexChanged += new System.EventHandler(this.SearchFormReplaceWithComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Replace with:";
            // 
            // SearchRangeSelectedRadioButton
            // 
            this.SearchRangeSelectedRadioButton.AutoSize = true;
            this.SearchRangeSelectedRadioButton.Location = new System.Drawing.Point(71, 16);
            this.SearchRangeSelectedRadioButton.Name = "SearchRangeSelectedRadioButton";
            this.SearchRangeSelectedRadioButton.Size = new System.Drawing.Size(66, 17);
            this.SearchRangeSelectedRadioButton.TabIndex = 13;
            this.SearchRangeSelectedRadioButton.Text = "Selected";
            this.SearchRangeSelectedRadioButton.UseVisualStyleBackColor = true;
            this.SearchRangeSelectedRadioButton.Click += new System.EventHandler(this.SearchRangeSelectedRadioButton_Click);
            // 
            // SearchRangeVisibleRadioButton
            // 
            this.SearchRangeVisibleRadioButton.AutoSize = true;
            this.SearchRangeVisibleRadioButton.Location = new System.Drawing.Point(71, 38);
            this.SearchRangeVisibleRadioButton.Name = "SearchRangeVisibleRadioButton";
            this.SearchRangeVisibleRadioButton.Size = new System.Drawing.Size(54, 17);
            this.SearchRangeVisibleRadioButton.TabIndex = 14;
            this.SearchRangeVisibleRadioButton.Text = "Visible";
            this.SearchRangeVisibleRadioButton.UseVisualStyleBackColor = true;
            this.SearchRangeVisibleRadioButton.Click += new System.EventHandler(this.SearchRangeVisibleRadioButton_Click);
            // 
            // THfrmSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(580, 555);
            this.Controls.Add(this.THSearchTabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(596, 368);
            this.Name = "THfrmSearch";
            this.Text = "Find and Replace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.THSearch_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.THSearch_FormClosed);
            this.Load += new System.EventHandler(this.THSearch_Load);
            this.THSearchTabs.ResumeLayout(false);
            this.THSearch1st.ResumeLayout(false);
            this.THSearch1st.PerformLayout();
            this.SearchResultsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SearchResultsDatagridview)).EndInit();
            this.THSearchPanel.ResumeLayout(false);
            this.THSearchPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.SearchModeGroupBox.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl THSearchTabs;
        private System.Windows.Forms.TabPage THSearch1st;
        private System.Windows.Forms.Panel THSearchPanel;
        private System.Windows.Forms.RadioButton SearchModeRegexRadioButton;
        private System.Windows.Forms.RadioButton SearchModeNormalRadioButton;
        private System.Windows.Forms.Button SearchFormReplaceAllButton;
        private System.Windows.Forms.Button SearchFormReplaceButton;
        private System.Windows.Forms.Button SearchFormFindNextButton;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox SearchFormReplaceWithComboBox;
        private System.Windows.Forms.Label THSearchFindWhatLabel;
        public System.Windows.Forms.ComboBox SearchFormFindWhatComboBox;
        private System.Windows.Forms.RadioButton SearchMethodTranslationRadioButton;
        private System.Windows.Forms.RadioButton SearchMethodOriginalToTranslationRadioButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton SearchRangeTableRadioButton;
        private System.Windows.Forms.RadioButton SearchRangeAllRadioButton;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lblSearchMsg;
        private System.Windows.Forms.Button FindAllButton;
        private System.Windows.Forms.CheckBox THSearchMatchCaseCheckBox;
        private System.Windows.Forms.GroupBox SearchModeGroupBox;
        private System.Windows.Forms.DataGridView SearchResultsDatagridview;
        private System.Windows.Forms.Panel SearchResultsPanel;
        private System.Windows.Forms.TextBox SearchFormReplaceWithTextBox;
        private System.Windows.Forms.TextBox SearchFormFindWhatTextBox;
        private System.Windows.Forms.CheckBox SearchAlwaysOnTopCheckBox;
        private System.Windows.Forms.CheckBox SearchFindLinesWithPossibleIssuesCheckBox;
        private System.Windows.Forms.CheckBox ConfirmReplaceAllCheckBox;
        private System.Windows.Forms.CheckBox SearchInInfoCheckBox;
        private System.Windows.Forms.Label ClearReplaceWithTextBoxLabel;
        private System.Windows.Forms.Label ClearFindWhatTextBoxLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chkbxDoNotTouchEqualOT;
        private System.Windows.Forms.RadioButton SearchRangeSelectedRadioButton;
        private System.Windows.Forms.RadioButton SearchRangeVisibleRadioButton;
    }
}
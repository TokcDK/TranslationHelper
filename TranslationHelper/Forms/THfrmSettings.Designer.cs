namespace TranslationHelper
{
    partial class THfrmSettings
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
            this.THSettingsTabControl = new System.Windows.Forms.TabControl();
            this.THSettingsMainTabPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LineCharLimitTextBox = new System.Windows.Forms.TextBox();
            this.THOptionLineCharLimitLabel = new System.Windows.Forms.Label();
            this.THOptionAutotranslationForSimularCheckBox = new System.Windows.Forms.CheckBox();
            this.THOptionFullComprasionDBloadCheckBox = new System.Windows.Forms.CheckBox();
            this.THSettingsOptimizationsTabPage = new System.Windows.Forms.TabPage();
            this.THProgramSettingsReadOptionsPanel = new System.Windows.Forms.Panel();
            this.THOptionDontLoadDuplicates = new System.Windows.Forms.CheckBox();
            this.THOptionIgnoreOrigEqualTransLinesCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox = new System.Windows.Forms.CheckBox();
            this.THOptionDBCompressionExtComboBox = new System.Windows.Forms.ComboBox();
            this.THOptionDBCompressionCheckBox = new System.Windows.Forms.CheckBox();
            this.DontLoadStringIfRomajiPercentNumberTextBox = new System.Windows.Forms.TextBox();
            this.THOptionDontLoadStringIfRomajiPercentCheckBox = new System.Windows.Forms.CheckBox();
            this.THSettingsToolsTabPage = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbSearchRowIssueOptionsCheckProjectSpecific = new System.Windows.Forms.CheckBox();
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable = new System.Windows.Forms.CheckBox();
            this.cbSearchRowIssueOptionsCheckNonRomaji = new System.Windows.Forms.CheckBox();
            this.cbSearchRowIssueOptionsCheckActors = new System.Windows.Forms.CheckBox();
            this.SearchRowIssueOptionsLabel = new System.Windows.Forms.Label();
            this.flpQuickTranslatorSelection = new System.Windows.Forms.FlowLayoutPanel();
            this.SettingsAutosaveTimeoutValueTextBox = new System.Windows.Forms.TextBox();
            this.SettingsAutosaveEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.THOptionEnableTranslationCacheCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.THSettingsWebTranslationLinkTextBox = new System.Windows.Forms.TextBox();
            this.cbxWebTranslatorsSelector = new System.Windows.Forms.ComboBox();
            this.THSettingsTabControl.SuspendLayout();
            this.THSettingsMainTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.THSettingsOptimizationsTabPage.SuspendLayout();
            this.THProgramSettingsReadOptionsPanel.SuspendLayout();
            this.THSettingsToolsTabPage.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // THSettingsTabControl
            // 
            this.THSettingsTabControl.Controls.Add(this.THSettingsMainTabPage);
            this.THSettingsTabControl.Controls.Add(this.THSettingsOptimizationsTabPage);
            this.THSettingsTabControl.Controls.Add(this.THSettingsToolsTabPage);
            this.THSettingsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSettingsTabControl.Location = new System.Drawing.Point(0, 0);
            this.THSettingsTabControl.Name = "THSettingsTabControl";
            this.THSettingsTabControl.SelectedIndex = 0;
            this.THSettingsTabControl.Size = new System.Drawing.Size(532, 405);
            this.THSettingsTabControl.TabIndex = 1;
            // 
            // THSettingsMainTabPage
            // 
            this.THSettingsMainTabPage.Controls.Add(this.panel1);
            this.THSettingsMainTabPage.Location = new System.Drawing.Point(4, 22);
            this.THSettingsMainTabPage.Name = "THSettingsMainTabPage";
            this.THSettingsMainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.THSettingsMainTabPage.Size = new System.Drawing.Size(524, 379);
            this.THSettingsMainTabPage.TabIndex = 0;
            this.THSettingsMainTabPage.Text = "General";
            this.THSettingsMainTabPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LineCharLimitTextBox);
            this.panel1.Controls.Add(this.THOptionLineCharLimitLabel);
            this.panel1.Controls.Add(this.THOptionAutotranslationForSimularCheckBox);
            this.panel1.Controls.Add(this.THOptionFullComprasionDBloadCheckBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 373);
            this.panel1.TabIndex = 7;
            // 
            // LineCharLimitTextBox
            // 
            this.LineCharLimitTextBox.Location = new System.Drawing.Point(5, 49);
            this.LineCharLimitTextBox.Name = "LineCharLimitTextBox";
            this.LineCharLimitTextBox.Size = new System.Drawing.Size(35, 21);
            this.LineCharLimitTextBox.TabIndex = 16;
            this.LineCharLimitTextBox.Text = "60";
            this.LineCharLimitTextBox.TextChanged += new System.EventHandler(this.LineCharLimitTextBox_TextChanged);
            // 
            // THOptionLineCharLimitLabel
            // 
            this.THOptionLineCharLimitLabel.AutoSize = true;
            this.THOptionLineCharLimitLabel.Location = new System.Drawing.Point(46, 52);
            this.THOptionLineCharLimitLabel.Name = "THOptionLineCharLimitLabel";
            this.THOptionLineCharLimitLabel.Size = new System.Drawing.Size(227, 13);
            this.THOptionLineCharLimitLabel.TabIndex = 15;
            this.THOptionLineCharLimitLabel.Text = "char limit of line length (for line split functions)";
            // 
            // THOptionAutotranslationForSimularCheckBox
            // 
            this.THOptionAutotranslationForSimularCheckBox.AutoSize = true;
            this.THOptionAutotranslationForSimularCheckBox.Checked = true;
            this.THOptionAutotranslationForSimularCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionAutotranslationForSimularCheckBox.Location = new System.Drawing.Point(5, 3);
            this.THOptionAutotranslationForSimularCheckBox.Name = "THOptionAutotranslationForSimularCheckBox";
            this.THOptionAutotranslationForSimularCheckBox.Size = new System.Drawing.Size(153, 17);
            this.THOptionAutotranslationForSimularCheckBox.TabIndex = 14;
            this.THOptionAutotranslationForSimularCheckBox.Text = "Autotranslation for simular";
            this.THOptionAutotranslationForSimularCheckBox.UseVisualStyleBackColor = true;
            this.THOptionAutotranslationForSimularCheckBox.CheckedChanged += new System.EventHandler(this.THOptionAutotranslationForIdenticalCheckBox_CheckedChanged);
            // 
            // THOptionFullComprasionDBloadCheckBox
            // 
            this.THOptionFullComprasionDBloadCheckBox.AutoSize = true;
            this.THOptionFullComprasionDBloadCheckBox.Location = new System.Drawing.Point(5, 26);
            this.THOptionFullComprasionDBloadCheckBox.Name = "THOptionFullComprasionDBloadCheckBox";
            this.THOptionFullComprasionDBloadCheckBox.Size = new System.Drawing.Size(290, 17);
            this.THOptionFullComprasionDBloadCheckBox.TabIndex = 0;
            this.THOptionFullComprasionDBloadCheckBox.Text = "Full recursive scan while translation DB loading (slower)";
            this.THOptionFullComprasionDBloadCheckBox.UseVisualStyleBackColor = true;
            this.THOptionFullComprasionDBloadCheckBox.CheckedChanged += new System.EventHandler(this.FullComparasionDBload_CheckedChanged);
            // 
            // THSettingsOptimizationsTabPage
            // 
            this.THSettingsOptimizationsTabPage.Controls.Add(this.THProgramSettingsReadOptionsPanel);
            this.THSettingsOptimizationsTabPage.Location = new System.Drawing.Point(4, 22);
            this.THSettingsOptimizationsTabPage.Name = "THSettingsOptimizationsTabPage";
            this.THSettingsOptimizationsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.THSettingsOptimizationsTabPage.Size = new System.Drawing.Size(524, 379);
            this.THSettingsOptimizationsTabPage.TabIndex = 1;
            this.THSettingsOptimizationsTabPage.Text = "Optimizations";
            this.THSettingsOptimizationsTabPage.UseVisualStyleBackColor = true;
            // 
            // THProgramSettingsReadOptionsPanel
            // 
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadDuplicates);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionIgnoreOrigEqualTransLinesCheckBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.label5);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDBCompressionExtComboBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDBCompressionCheckBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.DontLoadStringIfRomajiPercentNumberTextBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentCheckBox);
            this.THProgramSettingsReadOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THProgramSettingsReadOptionsPanel.Location = new System.Drawing.Point(3, 3);
            this.THProgramSettingsReadOptionsPanel.Name = "THProgramSettingsReadOptionsPanel";
            this.THProgramSettingsReadOptionsPanel.Size = new System.Drawing.Size(518, 373);
            this.THProgramSettingsReadOptionsPanel.TabIndex = 1;
            // 
            // THOptionDontLoadDuplicates
            // 
            this.THOptionDontLoadDuplicates.AutoSize = true;
            this.THOptionDontLoadDuplicates.Checked = true;
            this.THOptionDontLoadDuplicates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDontLoadDuplicates.Location = new System.Drawing.Point(3, 85);
            this.THOptionDontLoadDuplicates.Name = "THOptionDontLoadDuplicates";
            this.THOptionDontLoadDuplicates.Size = new System.Drawing.Size(123, 17);
            this.THOptionDontLoadDuplicates.TabIndex = 10;
            this.THOptionDontLoadDuplicates.Text = "Dont load duplicates";
            this.THOptionDontLoadDuplicates.UseVisualStyleBackColor = true;
            this.THOptionDontLoadDuplicates.CheckedChanged += new System.EventHandler(this.THOptionDontLoadDuplicates_CheckedChanged);
            // 
            // THOptionIgnoreOrigEqualTransLinesCheckBox
            // 
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.AutoSize = true;
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.Checked = true;
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.Location = new System.Drawing.Point(3, 62);
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.Name = "THOptionIgnoreOrigEqualTransLinesCheckBox";
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.Size = new System.Drawing.Size(182, 17);
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.TabIndex = 9;
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.Text = "Ignore Original=Translation lines";
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.UseVisualStyleBackColor = true;
            this.THOptionIgnoreOrigEqualTransLinesCheckBox.CheckedChanged += new System.EventHandler(this.THOptionIgnoreOrigEqualTransLinesCheckBox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(321, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "%";
            // 
            // THOptionDontLoadStringIfRomajiPercentForOpenCheckBox
            // 
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.AutoSize = true;
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.Checked = true;
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.Location = new System.Drawing.Point(345, 7);
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.Name = "THOptionDontLoadStringIfRomajiPercentForOpenCheckBox";
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.Size = new System.Drawing.Size(50, 17);
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.TabIndex = 7;
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.Text = "open";
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.UseVisualStyleBackColor = true;
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.Visible = false;
            this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox.CheckedChanged += new System.EventHandler(this.THOptionDontLoadStringIfRomajiPercentForOpenCheckBox_CheckedChanged);
            // 
            // THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox
            // 
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.AutoSize = true;
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.Checked = true;
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.Location = new System.Drawing.Point(401, 7);
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.Name = "THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox";
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.Size = new System.Drawing.Size(77, 17);
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.TabIndex = 6;
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.Text = "translation";
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.UseVisualStyleBackColor = true;
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.Visible = false;
            this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox.CheckedChanged += new System.EventHandler(this.THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox_CheckedChanged);
            // 
            // THOptionDBCompressionExtComboBox
            // 
            this.THOptionDBCompressionExtComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.THOptionDBCompressionExtComboBox.FormattingEnabled = true;
            this.THOptionDBCompressionExtComboBox.Location = new System.Drawing.Point(155, 30);
            this.THOptionDBCompressionExtComboBox.MaxDropDownItems = 3;
            this.THOptionDBCompressionExtComboBox.Name = "THOptionDBCompressionExtComboBox";
            this.THOptionDBCompressionExtComboBox.Size = new System.Drawing.Size(121, 21);
            this.THOptionDBCompressionExtComboBox.TabIndex = 5;
            this.THOptionDBCompressionExtComboBox.SelectionChangeCommitted += new System.EventHandler(this.THOptionDBCompressionComboBox_SelectionChangeCommitted);
            // 
            // THOptionDBCompressionCheckBox
            // 
            this.THOptionDBCompressionCheckBox.AutoSize = true;
            this.THOptionDBCompressionCheckBox.Checked = true;
            this.THOptionDBCompressionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDBCompressionCheckBox.Location = new System.Drawing.Point(3, 30);
            this.THOptionDBCompressionCheckBox.Name = "THOptionDBCompressionCheckBox";
            this.THOptionDBCompressionCheckBox.Size = new System.Drawing.Size(146, 17);
            this.THOptionDBCompressionCheckBox.TabIndex = 4;
            this.THOptionDBCompressionCheckBox.Text = "Compression for DB files:";
            this.THOptionDBCompressionCheckBox.UseVisualStyleBackColor = true;
            this.THOptionDBCompressionCheckBox.CheckedChanged += new System.EventHandler(this.THOptionDBCompressionCheckBox_CheckedChanged);
            // 
            // DontLoadStringIfRomajiPercentNumberTextBox
            // 
            this.DontLoadStringIfRomajiPercentNumberTextBox.Location = new System.Drawing.Point(291, 3);
            this.DontLoadStringIfRomajiPercentNumberTextBox.MaxLength = 3;
            this.DontLoadStringIfRomajiPercentNumberTextBox.Name = "DontLoadStringIfRomajiPercentNumberTextBox";
            this.DontLoadStringIfRomajiPercentNumberTextBox.ShortcutsEnabled = false;
            this.DontLoadStringIfRomajiPercentNumberTextBox.Size = new System.Drawing.Size(24, 21);
            this.DontLoadStringIfRomajiPercentNumberTextBox.TabIndex = 2;
            this.DontLoadStringIfRomajiPercentNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DontLoadStringIfRomajiPercentNumberTextBox.WordWrap = false;
            this.DontLoadStringIfRomajiPercentNumberTextBox.TextChanged += new System.EventHandler(this.THOptionDontLoadStringIfRomajiPercentTextBox_TextChanged);
            // 
            // THOptionDontLoadStringIfRomajiPercentCheckBox
            // 
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.AutoSize = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Checked = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Location = new System.Drawing.Point(3, 7);
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Name = "THOptionDontLoadStringIfRomajiPercentCheckBox";
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Size = new System.Drawing.Size(296, 17);
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.TabIndex = 1;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Text = "Do not load string if it has more of next romaji percent - ";
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.UseVisualStyleBackColor = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.CheckedChanged += new System.EventHandler(this.THOptionDontLoadStringIfRomajiPercentCheckBox_CheckedChanged);
            // 
            // THSettingsToolsTabPage
            // 
            this.THSettingsToolsTabPage.Controls.Add(this.panel2);
            this.THSettingsToolsTabPage.Location = new System.Drawing.Point(4, 22);
            this.THSettingsToolsTabPage.Name = "THSettingsToolsTabPage";
            this.THSettingsToolsTabPage.Size = new System.Drawing.Size(524, 379);
            this.THSettingsToolsTabPage.TabIndex = 2;
            this.THSettingsToolsTabPage.Text = "Tools";
            this.THSettingsToolsTabPage.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbSearchRowIssueOptionsCheckProjectSpecific);
            this.panel2.Controls.Add(this.cbSearchRowIssueOptionsCheckAnyLineTranslatable);
            this.panel2.Controls.Add(this.cbSearchRowIssueOptionsCheckNonRomaji);
            this.panel2.Controls.Add(this.cbSearchRowIssueOptionsCheckActors);
            this.panel2.Controls.Add(this.SearchRowIssueOptionsLabel);
            this.panel2.Controls.Add(this.flpQuickTranslatorSelection);
            this.panel2.Controls.Add(this.SettingsAutosaveTimeoutValueTextBox);
            this.panel2.Controls.Add(this.SettingsAutosaveEnabledCheckBox);
            this.panel2.Controls.Add(this.THOptionEnableTranslationCacheCheckBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.THSettingsWebTranslationLinkTextBox);
            this.panel2.Controls.Add(this.cbxWebTranslatorsSelector);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(524, 379);
            this.panel2.TabIndex = 8;
            // 
            // cbSearchRowIssueOptionsCheckProjectSpecific
            // 
            this.cbSearchRowIssueOptionsCheckProjectSpecific.AutoSize = true;
            this.cbSearchRowIssueOptionsCheckProjectSpecific.Checked = true;
            this.cbSearchRowIssueOptionsCheckProjectSpecific.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSearchRowIssueOptionsCheckProjectSpecific.Location = new System.Drawing.Point(28, 182);
            this.cbSearchRowIssueOptionsCheckProjectSpecific.Name = "cbSearchRowIssueOptionsCheckProjectSpecific";
            this.cbSearchRowIssueOptionsCheckProjectSpecific.Size = new System.Drawing.Size(162, 17);
            this.cbSearchRowIssueOptionsCheckProjectSpecific.TabIndex = 21;
            this.cbSearchRowIssueOptionsCheckProjectSpecific.Text = "Check project specific issues";
            this.cbSearchRowIssueOptionsCheckProjectSpecific.UseVisualStyleBackColor = true;
            this.cbSearchRowIssueOptionsCheckProjectSpecific.CheckedChanged += new System.EventHandler(this.cbSearchRowIssueOptionsCheckProjectSpecific_CheckedChanged);
            // 
            // cbSearchRowIssueOptionsCheckAnyLineTranslatable
            // 
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.AutoSize = true;
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.Checked = true;
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.Location = new System.Drawing.Point(28, 159);
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.Name = "cbSearchRowIssueOptionsCheckAnyLineTranslatable";
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.Size = new System.Drawing.Size(191, 17);
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.TabIndex = 20;
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.Text = "Check if any line can be translated";
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.UseVisualStyleBackColor = true;
            this.cbSearchRowIssueOptionsCheckAnyLineTranslatable.CheckedChanged += new System.EventHandler(this.cbSearchRowIssueOptionsCheckAnyLineTranslatable_CheckedChanged);
            // 
            // cbSearchRowIssueOptionsCheckNonRomaji
            // 
            this.cbSearchRowIssueOptionsCheckNonRomaji.AutoSize = true;
            this.cbSearchRowIssueOptionsCheckNonRomaji.Checked = true;
            this.cbSearchRowIssueOptionsCheckNonRomaji.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSearchRowIssueOptionsCheckNonRomaji.Location = new System.Drawing.Point(28, 113);
            this.cbSearchRowIssueOptionsCheckNonRomaji.Name = "cbSearchRowIssueOptionsCheckNonRomaji";
            this.cbSearchRowIssueOptionsCheckNonRomaji.Size = new System.Drawing.Size(134, 17);
            this.cbSearchRowIssueOptionsCheckNonRomaji.TabIndex = 19;
            this.cbSearchRowIssueOptionsCheckNonRomaji.Text = "Check non romaji exist";
            this.cbSearchRowIssueOptionsCheckNonRomaji.UseVisualStyleBackColor = true;
            this.cbSearchRowIssueOptionsCheckNonRomaji.CheckedChanged += new System.EventHandler(this.cbSearchRowIssueOptionsCheckNonRomaji_CheckedChanged);
            // 
            // cbSearchRowIssueOptionsCheckActors
            // 
            this.cbSearchRowIssueOptionsCheckActors.AutoSize = true;
            this.cbSearchRowIssueOptionsCheckActors.Checked = true;
            this.cbSearchRowIssueOptionsCheckActors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSearchRowIssueOptionsCheckActors.Location = new System.Drawing.Point(28, 136);
            this.cbSearchRowIssueOptionsCheckActors.Name = "cbSearchRowIssueOptionsCheckActors";
            this.cbSearchRowIssueOptionsCheckActors.Size = new System.Drawing.Size(118, 17);
            this.cbSearchRowIssueOptionsCheckActors.TabIndex = 18;
            this.cbSearchRowIssueOptionsCheckActors.Text = "Check Actor names";
            this.cbSearchRowIssueOptionsCheckActors.UseVisualStyleBackColor = true;
            this.cbSearchRowIssueOptionsCheckActors.CheckedChanged += new System.EventHandler(this.cbSearchRowIssueOptionsCheckActors_CheckedChanged);
            // 
            // SearchRowIssueOptionsLabel
            // 
            this.SearchRowIssueOptionsLabel.AutoSize = true;
            this.SearchRowIssueOptionsLabel.Location = new System.Drawing.Point(8, 97);
            this.SearchRowIssueOptionsLabel.Name = "SearchRowIssueOptionsLabel";
            this.SearchRowIssueOptionsLabel.Size = new System.Drawing.Size(154, 13);
            this.SearchRowIssueOptionsLabel.TabIndex = 17;
            this.SearchRowIssueOptionsLabel.Text = "Search Row issue find options:";
            // 
            // flpQuickTranslatorSelection
            // 
            this.flpQuickTranslatorSelection.Location = new System.Drawing.Point(238, 23);
            this.flpQuickTranslatorSelection.Margin = new System.Windows.Forms.Padding(0);
            this.flpQuickTranslatorSelection.Name = "flpQuickTranslatorSelection";
            this.flpQuickTranslatorSelection.Size = new System.Drawing.Size(278, 13);
            this.flpQuickTranslatorSelection.TabIndex = 16;
            // 
            // SettingsAutosaveTimeoutValueTextBox
            // 
            this.SettingsAutosaveTimeoutValueTextBox.Location = new System.Drawing.Point(172, 64);
            this.SettingsAutosaveTimeoutValueTextBox.MaxLength = 3;
            this.SettingsAutosaveTimeoutValueTextBox.Name = "SettingsAutosaveTimeoutValueTextBox";
            this.SettingsAutosaveTimeoutValueTextBox.ShortcutsEnabled = false;
            this.SettingsAutosaveTimeoutValueTextBox.Size = new System.Drawing.Size(33, 21);
            this.SettingsAutosaveTimeoutValueTextBox.TabIndex = 14;
            this.SettingsAutosaveTimeoutValueTextBox.Text = "300";
            this.SettingsAutosaveTimeoutValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SettingsAutosaveTimeoutValueTextBox.WordWrap = false;
            this.SettingsAutosaveTimeoutValueTextBox.TextChanged += new System.EventHandler(this.SettingsAutosaveTimeoutValueTextBox_TextChanged);
            // 
            // SettingsAutosaveEnabledCheckBox
            // 
            this.SettingsAutosaveEnabledCheckBox.AutoSize = true;
            this.SettingsAutosaveEnabledCheckBox.Checked = true;
            this.SettingsAutosaveEnabledCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SettingsAutosaveEnabledCheckBox.Location = new System.Drawing.Point(8, 66);
            this.SettingsAutosaveEnabledCheckBox.Name = "SettingsAutosaveEnabledCheckBox";
            this.SettingsAutosaveEnabledCheckBox.Size = new System.Drawing.Size(158, 17);
            this.SettingsAutosaveEnabledCheckBox.TabIndex = 13;
            this.SettingsAutosaveEnabledCheckBox.Text = "Enable autosave. Timeout -";
            this.SettingsAutosaveEnabledCheckBox.UseVisualStyleBackColor = true;
            this.SettingsAutosaveEnabledCheckBox.CheckedChanged += new System.EventHandler(this.SettingsAutosaveEnabledCheckBox_CheckedChanged);
            // 
            // THOptionEnableTranslationCacheCheckBox
            // 
            this.THOptionEnableTranslationCacheCheckBox.AutoSize = true;
            this.THOptionEnableTranslationCacheCheckBox.Checked = true;
            this.THOptionEnableTranslationCacheCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionEnableTranslationCacheCheckBox.Location = new System.Drawing.Point(8, 3);
            this.THOptionEnableTranslationCacheCheckBox.Name = "THOptionEnableTranslationCacheCheckBox";
            this.THOptionEnableTranslationCacheCheckBox.Size = new System.Drawing.Size(178, 17);
            this.THOptionEnableTranslationCacheCheckBox.TabIndex = 12;
            this.THOptionEnableTranslationCacheCheckBox.Text = "Enable online translation cache.";
            this.THOptionEnableTranslationCacheCheckBox.UseVisualStyleBackColor = true;
            this.THOptionEnableTranslationCacheCheckBox.CheckedChanged += new System.EventHandler(this.THOptionEnableTranslationCacheCheckBox_CheckedChanged_1);
            this.THOptionEnableTranslationCacheCheckBox.Click += new System.EventHandler(this.THOptionEnableTranslationCacheCheckBox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Web service link for manual translation (F12):";
            // 
            // THSettingsWebTranslationLinkTextBox
            // 
            this.THSettingsWebTranslationLinkTextBox.Location = new System.Drawing.Point(8, 39);
            this.THSettingsWebTranslationLinkTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.THSettingsWebTranslationLinkTextBox.Name = "THSettingsWebTranslationLinkTextBox";
            this.THSettingsWebTranslationLinkTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.THSettingsWebTranslationLinkTextBox.Size = new System.Drawing.Size(491, 21);
            this.THSettingsWebTranslationLinkTextBox.TabIndex = 7;
            this.THSettingsWebTranslationLinkTextBox.Text = "https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}" +
    "";
            this.THSettingsWebTranslationLinkTextBox.WordWrap = false;
            this.THSettingsWebTranslationLinkTextBox.TextChanged += new System.EventHandler(this.THSettingsWebTranslationLinkTextBox_TextChanged);
            this.THSettingsWebTranslationLinkTextBox.Validated += new System.EventHandler(this.THSettingsWebTransLinkTextBox_Validated);
            // 
            // cbxWebTranslatorsSelector
            // 
            this.cbxWebTranslatorsSelector.FormattingEnabled = true;
            this.cbxWebTranslatorsSelector.Items.AddRange(new object[] {
            "https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}" +
                "",
            "https://www.deepl.com/ru/translator#{from}/{to}/{text}",
            "https://translate.yandex.com/?lang={from}-{to}&text={text}"});
            this.cbxWebTranslatorsSelector.Location = new System.Drawing.Point(8, 39);
            this.cbxWebTranslatorsSelector.Margin = new System.Windows.Forms.Padding(1);
            this.cbxWebTranslatorsSelector.Name = "cbxWebTranslatorsSelector";
            this.cbxWebTranslatorsSelector.Size = new System.Drawing.Size(508, 21);
            this.cbxWebTranslatorsSelector.TabIndex = 15;
            this.cbxWebTranslatorsSelector.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // THfrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 405);
            this.Controls.Add(this.THSettingsTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "THfrmSettings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.THSettings_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.THSettings_FormClosed);
            this.Load += new System.EventHandler(this.THProgramSettingsForm_Load);
            this.THSettingsTabControl.ResumeLayout(false);
            this.THSettingsMainTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.THSettingsOptimizationsTabPage.ResumeLayout(false);
            this.THProgramSettingsReadOptionsPanel.ResumeLayout(false);
            this.THProgramSettingsReadOptionsPanel.PerformLayout();
            this.THSettingsToolsTabPage.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl THSettingsTabControl;
        private System.Windows.Forms.TabPage THSettingsMainTabPage;
        public System.Windows.Forms.TabPage THSettingsOptimizationsTabPage;
        private System.Windows.Forms.Panel THProgramSettingsReadOptionsPanel;
        public System.Windows.Forms.TextBox DontLoadStringIfRomajiPercentNumberTextBox;
        private System.Windows.Forms.TabPage THSettingsToolsTabPage;
        public System.Windows.Forms.CheckBox THOptionDBCompressionCheckBox;
        public System.Windows.Forms.ComboBox THOptionDBCompressionExtComboBox;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentForOpenCheckBox;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentForTranslationCheckBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox THOptionEnableTranslationCacheCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label THOptionLineCharLimitLabel;
        internal System.Windows.Forms.CheckBox THOptionFullComprasionDBloadCheckBox;
        internal System.Windows.Forms.CheckBox THOptionAutotranslationForSimularCheckBox;
        internal System.Windows.Forms.TextBox LineCharLimitTextBox;
        internal System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentCheckBox;
        public System.Windows.Forms.TextBox SettingsAutosaveTimeoutValueTextBox;
        internal System.Windows.Forms.CheckBox SettingsAutosaveEnabledCheckBox;
        internal System.Windows.Forms.ComboBox cbxWebTranslatorsSelector;
        internal System.Windows.Forms.FlowLayoutPanel flpQuickTranslatorSelection;
        internal System.Windows.Forms.TextBox THSettingsWebTranslationLinkTextBox;
        public System.Windows.Forms.CheckBox THOptionIgnoreOrigEqualTransLinesCheckBox;
        internal System.Windows.Forms.CheckBox THOptionDontLoadDuplicates;
        internal System.Windows.Forms.CheckBox cbSearchRowIssueOptionsCheckNonRomaji;
        internal System.Windows.Forms.CheckBox cbSearchRowIssueOptionsCheckActors;
        private System.Windows.Forms.Label SearchRowIssueOptionsLabel;
        internal System.Windows.Forms.CheckBox cbSearchRowIssueOptionsCheckAnyLineTranslatable;
        internal System.Windows.Forms.CheckBox cbSearchRowIssueOptionsCheckProjectSpecific;
    }
}
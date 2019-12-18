namespace TranslationHelper
{
    partial class THSettings
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
            this.THOptionAutotranslationForIdenticalCheckBox = new System.Windows.Forms.CheckBox();
            this.THOptionFullComprasionDBload = new System.Windows.Forms.CheckBox();
            this.THSettingsOptimizationsTabPage = new System.Windows.Forms.TabPage();
            this.THProgramSettingsReadOptionsPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen = new System.Windows.Forms.CheckBox();
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation = new System.Windows.Forms.CheckBox();
            this.THOptionDBCompressionComboBox = new System.Windows.Forms.ComboBox();
            this.THOptionDBCompressionCheckBox = new System.Windows.Forms.CheckBox();
            this.THOptionDontLoadStringIfRomajiPercentTextBox = new System.Windows.Forms.TextBox();
            this.THOptionDontLoadStringIfRomajiPercentCheckBox = new System.Windows.Forms.CheckBox();
            this.THSettingsToolsTabPage = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.THOptionEnableTranslationCacheCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.THSettingsWebTransLinkTextBox = new System.Windows.Forms.TextBox();
            this.THOptionLineCharLimitLabel = new System.Windows.Forms.Label();
            this.LineCharLimitTextBox = new System.Windows.Forms.TextBox();
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
            this.panel1.Controls.Add(this.THOptionAutotranslationForIdenticalCheckBox);
            this.panel1.Controls.Add(this.THOptionFullComprasionDBload);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 373);
            this.panel1.TabIndex = 7;
            // 
            // THOptionAutotranslationForIdenticalCheckBox
            // 
            this.THOptionAutotranslationForIdenticalCheckBox.AutoSize = true;
            this.THOptionAutotranslationForIdenticalCheckBox.Checked = true;
            this.THOptionAutotranslationForIdenticalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionAutotranslationForIdenticalCheckBox.Location = new System.Drawing.Point(5, 3);
            this.THOptionAutotranslationForIdenticalCheckBox.Name = "THOptionAutotranslationForIdenticalCheckBox";
            this.THOptionAutotranslationForIdenticalCheckBox.Size = new System.Drawing.Size(159, 17);
            this.THOptionAutotranslationForIdenticalCheckBox.TabIndex = 14;
            this.THOptionAutotranslationForIdenticalCheckBox.Text = "Autotranslation for identical";
            this.THOptionAutotranslationForIdenticalCheckBox.UseVisualStyleBackColor = true;
            this.THOptionAutotranslationForIdenticalCheckBox.CheckedChanged += new System.EventHandler(this.THOptionAutotranslationForIdenticalCheckBox_CheckedChanged);
            // 
            // THOptionFullComprasionDBload
            // 
            this.THOptionFullComprasionDBload.AutoSize = true;
            this.THOptionFullComprasionDBload.Location = new System.Drawing.Point(5, 26);
            this.THOptionFullComprasionDBload.Name = "THOptionFullComprasionDBload";
            this.THOptionFullComprasionDBload.Size = new System.Drawing.Size(290, 17);
            this.THOptionFullComprasionDBload.TabIndex = 0;
            this.THOptionFullComprasionDBload.Text = "Full recursive scan while translation DB loading (slower)";
            this.THOptionFullComprasionDBload.UseVisualStyleBackColor = true;
            this.THOptionFullComprasionDBload.CheckedChanged += new System.EventHandler(this.FullComparasionDBload_CheckedChanged);
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
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.label5);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDBCompressionComboBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDBCompressionCheckBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentTextBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentCheckBox);
            this.THProgramSettingsReadOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THProgramSettingsReadOptionsPanel.Location = new System.Drawing.Point(3, 3);
            this.THProgramSettingsReadOptionsPanel.Name = "THProgramSettingsReadOptionsPanel";
            this.THProgramSettingsReadOptionsPanel.Size = new System.Drawing.Size(518, 373);
            this.THProgramSettingsReadOptionsPanel.TabIndex = 1;
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
            // THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen
            // 
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.AutoSize = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.Checked = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.Location = new System.Drawing.Point(345, 7);
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.Name = "THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen";
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.Size = new System.Drawing.Size(50, 17);
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.TabIndex = 7;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.Text = "open";
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.UseVisualStyleBackColor = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen.Visible = false;
            // 
            // THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation
            // 
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.AutoSize = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.Checked = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.Location = new System.Drawing.Point(401, 7);
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.Name = "THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation";
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.Size = new System.Drawing.Size(77, 17);
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.TabIndex = 6;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.Text = "translation";
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.UseVisualStyleBackColor = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation.Visible = false;
            // 
            // THOptionDBCompressionComboBox
            // 
            this.THOptionDBCompressionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.THOptionDBCompressionComboBox.FormattingEnabled = true;
            this.THOptionDBCompressionComboBox.Items.AddRange(new object[] {
            "XML (none)",
            "Gzip (cmx)",
            "Deflate (cmz)"});
            this.THOptionDBCompressionComboBox.Location = new System.Drawing.Point(155, 30);
            this.THOptionDBCompressionComboBox.MaxDropDownItems = 3;
            this.THOptionDBCompressionComboBox.Name = "THOptionDBCompressionComboBox";
            this.THOptionDBCompressionComboBox.Size = new System.Drawing.Size(121, 21);
            this.THOptionDBCompressionComboBox.TabIndex = 5;
            this.THOptionDBCompressionComboBox.SelectionChangeCommitted += new System.EventHandler(this.THOptionDBCompressionComboBox_SelectionChangeCommitted);
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
            // THOptionDontLoadStringIfRomajiPercentTextBox
            // 
            this.THOptionDontLoadStringIfRomajiPercentTextBox.Location = new System.Drawing.Point(291, 3);
            this.THOptionDontLoadStringIfRomajiPercentTextBox.MaxLength = 3;
            this.THOptionDontLoadStringIfRomajiPercentTextBox.Name = "THOptionDontLoadStringIfRomajiPercentTextBox";
            this.THOptionDontLoadStringIfRomajiPercentTextBox.ShortcutsEnabled = false;
            this.THOptionDontLoadStringIfRomajiPercentTextBox.Size = new System.Drawing.Size(24, 21);
            this.THOptionDontLoadStringIfRomajiPercentTextBox.TabIndex = 2;
            this.THOptionDontLoadStringIfRomajiPercentTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.THOptionDontLoadStringIfRomajiPercentTextBox.WordWrap = false;
            this.THOptionDontLoadStringIfRomajiPercentTextBox.TextChanged += new System.EventHandler(this.THOptionDontLoadStringIfRomajiPercentTextBox_TextChanged);
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
            this.panel2.Controls.Add(this.THOptionEnableTranslationCacheCheckBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.THSettingsWebTransLinkTextBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(524, 379);
            this.panel2.TabIndex = 8;
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
            // THSettingsWebTransLinkTextBox
            // 
            this.THSettingsWebTransLinkTextBox.Location = new System.Drawing.Point(8, 39);
            this.THSettingsWebTransLinkTextBox.Name = "THSettingsWebTransLinkTextBox";
            this.THSettingsWebTransLinkTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.THSettingsWebTransLinkTextBox.Size = new System.Drawing.Size(508, 21);
            this.THSettingsWebTransLinkTextBox.TabIndex = 7;
            this.THSettingsWebTransLinkTextBox.Text = "https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}";
            this.THSettingsWebTransLinkTextBox.Validated += new System.EventHandler(this.THSettingsWebTransLinkTextBox_Validated);
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
            // LineCharLimitTextBox
            // 
            this.LineCharLimitTextBox.Location = new System.Drawing.Point(5, 49);
            this.LineCharLimitTextBox.Name = "LineCharLimitTextBox";
            this.LineCharLimitTextBox.Size = new System.Drawing.Size(35, 21);
            this.LineCharLimitTextBox.TabIndex = 16;
            this.LineCharLimitTextBox.Text = "60";
            this.LineCharLimitTextBox.TextChanged += new System.EventHandler(this.LineCharLimitTextBox_TextChanged);
            // 
            // THSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 405);
            this.Controls.Add(this.THSettingsTabControl);
            this.Name = "THSettings";
            this.Text = "Settings";
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
        public System.Windows.Forms.TextBox THOptionDontLoadStringIfRomajiPercentTextBox;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentCheckBox;
        private System.Windows.Forms.TabPage THSettingsToolsTabPage;
        public System.Windows.Forms.CheckBox THOptionDBCompressionCheckBox;
        public System.Windows.Forms.ComboBox THOptionDBCompressionComboBox;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentCheckBoxForOpen;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentCheckBoxForTranslation;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox THSettingsWebTransLinkTextBox;
        public System.Windows.Forms.CheckBox THOptionEnableTranslationCacheCheckBox;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox THOptionAutotranslationForIdenticalCheckBox;
        private System.Windows.Forms.CheckBox THOptionFullComprasionDBload;
        private System.Windows.Forms.TextBox LineCharLimitTextBox;
        private System.Windows.Forms.Label THOptionLineCharLimitLabel;
    }
}
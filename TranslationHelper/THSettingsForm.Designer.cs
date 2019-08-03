namespace TranslationHelper
{
    partial class THSettingsForm
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
            this.THSettingsOptimizationsTabPage = new System.Windows.Forms.TabPage();
            this.THProgramSettingsReadOptionsPanel = new System.Windows.Forms.Panel();
            this.THOptionDBCompressionComboBox = new System.Windows.Forms.ComboBox();
            this.THOptionDBCompressionCheckBox = new System.Windows.Forms.CheckBox();
            this.THSettingsOptimizationsGroupLabel = new System.Windows.Forms.Label();
            this.THOptionDontLoadStringIfRomajiPercentTextBox = new System.Windows.Forms.TextBox();
            this.THOptionDontLoadStringIfRomajiPercentCheckBox = new System.Windows.Forms.CheckBox();
            this.THSettingsToolsTabPage = new System.Windows.Forms.TabPage();
            this.THSettingsTabControl.SuspendLayout();
            this.THSettingsOptimizationsTabPage.SuspendLayout();
            this.THProgramSettingsReadOptionsPanel.SuspendLayout();
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
            this.THSettingsMainTabPage.Location = new System.Drawing.Point(4, 22);
            this.THSettingsMainTabPage.Name = "THSettingsMainTabPage";
            this.THSettingsMainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.THSettingsMainTabPage.Size = new System.Drawing.Size(524, 379);
            this.THSettingsMainTabPage.TabIndex = 0;
            this.THSettingsMainTabPage.Text = "Main";
            this.THSettingsMainTabPage.UseVisualStyleBackColor = true;
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
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDBCompressionComboBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDBCompressionCheckBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THSettingsOptimizationsGroupLabel);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentTextBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentCheckBox);
            this.THProgramSettingsReadOptionsPanel.Location = new System.Drawing.Point(8, 6);
            this.THProgramSettingsReadOptionsPanel.Name = "THProgramSettingsReadOptionsPanel";
            this.THProgramSettingsReadOptionsPanel.Size = new System.Drawing.Size(508, 365);
            this.THProgramSettingsReadOptionsPanel.TabIndex = 1;
            // 
            // THOptionDBCompressionComboBox
            // 
            this.THOptionDBCompressionComboBox.FormattingEnabled = true;
            this.THOptionDBCompressionComboBox.Items.AddRange(new object[] {
            "XML (none)",
            "Gzip (cmx)",
            "Deflate (cmz)"});
            this.THOptionDBCompressionComboBox.Location = new System.Drawing.Point(149, 39);
            this.THOptionDBCompressionComboBox.MaxDropDownItems = 3;
            this.THOptionDBCompressionComboBox.Name = "THOptionDBCompressionComboBox";
            this.THOptionDBCompressionComboBox.Size = new System.Drawing.Size(121, 21);
            this.THOptionDBCompressionComboBox.TabIndex = 5;
            this.THOptionDBCompressionComboBox.Text = "XML (None)";
            this.THOptionDBCompressionComboBox.SelectionChangeCommitted += new System.EventHandler(this.THOptionDBCompressionComboBox_SelectionChangeCommitted);
            // 
            // THOptionDBCompressionCheckBox
            // 
            this.THOptionDBCompressionCheckBox.AutoSize = true;
            this.THOptionDBCompressionCheckBox.Checked = true;
            this.THOptionDBCompressionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.THOptionDBCompressionCheckBox.Location = new System.Drawing.Point(3, 43);
            this.THOptionDBCompressionCheckBox.Name = "THOptionDBCompressionCheckBox";
            this.THOptionDBCompressionCheckBox.Size = new System.Drawing.Size(146, 17);
            this.THOptionDBCompressionCheckBox.TabIndex = 4;
            this.THOptionDBCompressionCheckBox.Text = "Compression for DB files:";
            this.THOptionDBCompressionCheckBox.UseVisualStyleBackColor = true;
            this.THOptionDBCompressionCheckBox.CheckedChanged += new System.EventHandler(this.THOptionDBCompressionCheckBox_CheckedChanged);
            // 
            // THSettingsOptimizationsGroupLabel
            // 
            this.THSettingsOptimizationsGroupLabel.AutoSize = true;
            this.THSettingsOptimizationsGroupLabel.Location = new System.Drawing.Point(3, 4);
            this.THSettingsOptimizationsGroupLabel.Name = "THSettingsOptimizationsGroupLabel";
            this.THSettingsOptimizationsGroupLabel.Size = new System.Drawing.Size(71, 13);
            this.THSettingsOptimizationsGroupLabel.TabIndex = 3;
            this.THSettingsOptimizationsGroupLabel.Text = "Optimizations";
            // 
            // THOptionDontLoadStringIfRomajiPercentTextBox
            // 
            this.THOptionDontLoadStringIfRomajiPercentTextBox.Location = new System.Drawing.Point(292, 16);
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
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Location = new System.Drawing.Point(3, 20);
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Name = "THOptionDontLoadStringIfRomajiPercentCheckBox";
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Size = new System.Drawing.Size(296, 17);
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.TabIndex = 1;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.Text = "Do not load string if it has more of next romaji percent - ";
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.UseVisualStyleBackColor = true;
            this.THOptionDontLoadStringIfRomajiPercentCheckBox.CheckedChanged += new System.EventHandler(this.THOptionDontLoadStringIfRomajiPercentCheckBox_CheckedChanged);
            // 
            // THSettingsToolsTabPage
            // 
            this.THSettingsToolsTabPage.Location = new System.Drawing.Point(4, 22);
            this.THSettingsToolsTabPage.Name = "THSettingsToolsTabPage";
            this.THSettingsToolsTabPage.Size = new System.Drawing.Size(524, 379);
            this.THSettingsToolsTabPage.TabIndex = 2;
            this.THSettingsToolsTabPage.Text = "Tools";
            this.THSettingsToolsTabPage.UseVisualStyleBackColor = true;
            // 
            // THSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 405);
            this.Controls.Add(this.THSettingsTabControl);
            this.Name = "THSettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.THProgramSettingsForm_Load);
            this.THSettingsTabControl.ResumeLayout(false);
            this.THSettingsOptimizationsTabPage.ResumeLayout(false);
            this.THProgramSettingsReadOptionsPanel.ResumeLayout(false);
            this.THProgramSettingsReadOptionsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl THSettingsTabControl;
        private System.Windows.Forms.TabPage THSettingsMainTabPage;
        public System.Windows.Forms.TabPage THSettingsOptimizationsTabPage;
        private System.Windows.Forms.Panel THProgramSettingsReadOptionsPanel;
        private System.Windows.Forms.Label THSettingsOptimizationsGroupLabel;
        public System.Windows.Forms.TextBox THOptionDontLoadStringIfRomajiPercentTextBox;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentCheckBox;
        private System.Windows.Forms.TabPage THSettingsToolsTabPage;
        public System.Windows.Forms.CheckBox THOptionDBCompressionCheckBox;
        public System.Windows.Forms.ComboBox THOptionDBCompressionComboBox;
    }
}
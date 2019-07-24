namespace TranslationHelper
{
    partial class THProgramSettingsForm
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
            this.THProgramSettingsReadOptionsPanel = new System.Windows.Forms.Panel();
            this.THSettingsOptimizationsGroupLabel = new System.Windows.Forms.Label();
            this.THOptionDontLoadStringIfRomajiPercentTextBox = new System.Windows.Forms.TextBox();
            this.THOptionDontLoadStringIfRomajiPercentCheckBox = new System.Windows.Forms.CheckBox();
            this.THProgramSettingsReadOptionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // THProgramSettingsReadOptionsPanel
            // 
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THSettingsOptimizationsGroupLabel);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentTextBox);
            this.THProgramSettingsReadOptionsPanel.Controls.Add(this.THOptionDontLoadStringIfRomajiPercentCheckBox);
            this.THProgramSettingsReadOptionsPanel.Location = new System.Drawing.Point(13, 13);
            this.THProgramSettingsReadOptionsPanel.Name = "THProgramSettingsReadOptionsPanel";
            this.THProgramSettingsReadOptionsPanel.Size = new System.Drawing.Size(473, 292);
            this.THProgramSettingsReadOptionsPanel.TabIndex = 0;
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
            // THProgramSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 405);
            this.Controls.Add(this.THProgramSettingsReadOptionsPanel);
            this.Name = "THProgramSettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.THProgramSettingsForm_Load);
            this.THProgramSettingsReadOptionsPanel.ResumeLayout(false);
            this.THProgramSettingsReadOptionsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel THProgramSettingsReadOptionsPanel;
        public System.Windows.Forms.TextBox THOptionDontLoadStringIfRomajiPercentTextBox;
        public System.Windows.Forms.CheckBox THOptionDontLoadStringIfRomajiPercentCheckBox;
        private System.Windows.Forms.Label THSettingsOptimizationsGroupLabel;
    }
}
namespace TranslationHelper.Forms.Search
{
    partial class ReplaceWhatWithUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ReplaceWithComboBox = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ReplaceWhatComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ReplaceWhatWithTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.ReplaceWhatWithTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReplaceWithComboBox
            // 
            this.ReplaceWithComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReplaceWithComboBox.FormattingEnabled = true;
            this.ReplaceWithComboBox.Location = new System.Drawing.Point(114, 34);
            this.ReplaceWithComboBox.Name = "ReplaceWithComboBox";
            this.ReplaceWithComboBox.Size = new System.Drawing.Size(549, 21);
            this.ReplaceWithComboBox.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ReplaceWhatComboBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(114, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(549, 25);
            this.panel1.TabIndex = 4;
            // 
            // ReplaceWhatComboBox
            // 
            this.ReplaceWhatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReplaceWhatComboBox.FormattingEnabled = true;
            this.ReplaceWhatComboBox.Location = new System.Drawing.Point(0, 0);
            this.ReplaceWhatComboBox.Name = "ReplaceWhatComboBox";
            this.ReplaceWhatComboBox.Size = new System.Drawing.Size(549, 21);
            this.ReplaceWhatComboBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Replace what:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 31);
            this.label3.TabIndex = 3;
            this.label3.Text = "Replace with:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ReplaceWhatWithTableLayoutPanel
            // 
            this.ReplaceWhatWithTableLayoutPanel.ColumnCount = 2;
            this.ReplaceWhatWithTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.ReplaceWhatWithTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.33334F));
            this.ReplaceWhatWithTableLayoutPanel.Controls.Add(this.label3, 0, 1);
            this.ReplaceWhatWithTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.ReplaceWhatWithTableLayoutPanel.Controls.Add(this.panel1, 1, 0);
            this.ReplaceWhatWithTableLayoutPanel.Controls.Add(this.ReplaceWithComboBox, 1, 1);
            this.ReplaceWhatWithTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReplaceWhatWithTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ReplaceWhatWithTableLayoutPanel.Name = "ReplaceWhatWithTableLayoutPanel";
            this.ReplaceWhatWithTableLayoutPanel.RowCount = 2;
            this.ReplaceWhatWithTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.6129F));
            this.ReplaceWhatWithTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.3871F));
            this.ReplaceWhatWithTableLayoutPanel.Size = new System.Drawing.Size(666, 62);
            this.ReplaceWhatWithTableLayoutPanel.TabIndex = 0;
            // 
            // ReplaceWhatWithUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ReplaceWhatWithTableLayoutPanel);
            this.Name = "ReplaceWhatWithUserControl";
            this.Size = new System.Drawing.Size(666, 62);
            this.Load += new System.EventHandler(this.ReplaceWhatWithUserControl_Load);
            this.panel1.ResumeLayout(false);
            this.ReplaceWhatWithTableLayoutPanel.ResumeLayout(false);
            this.ReplaceWhatWithTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel ReplaceWhatWithTableLayoutPanel;
        internal System.Windows.Forms.ComboBox ReplaceWithComboBox;
        internal System.Windows.Forms.ComboBox ReplaceWhatComboBox;
    }
}

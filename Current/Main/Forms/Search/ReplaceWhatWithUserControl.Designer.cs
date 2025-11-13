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
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ReplaceWhatWithTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.ReplaceWhatWithTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox2
            // 
            this.comboBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(114, 35);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(549, 21);
            this.comboBox2.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(114, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(549, 26);
            this.panel1.TabIndex = 4;
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(0, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(549, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Replace what:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 30);
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
            this.ReplaceWhatWithTableLayoutPanel.Controls.Add(this.comboBox2, 1, 1);
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
            this.panel1.ResumeLayout(false);
            this.ReplaceWhatWithTableLayoutPanel.ResumeLayout(false);
            this.ReplaceWhatWithTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel ReplaceWhatWithTableLayoutPanel;
    }
}

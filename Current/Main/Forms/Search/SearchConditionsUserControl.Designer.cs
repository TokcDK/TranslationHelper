namespace TranslationHelper.Forms.Search
{
    partial class SearchConditionsUserControl
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
            this.components = new System.ComponentModel.Container();
            this.SearchConditionsRootTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SearchTextFieldsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ConditionsTabControl = new System.Windows.Forms.TabControl();
            this.SearchConditionsRootTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchConditionsRootTableLayoutPanel
            // 
            this.SearchConditionsRootTableLayoutPanel.ColumnCount = 1;
            this.SearchConditionsRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchConditionsRootTableLayoutPanel.Controls.Add(this.SearchTextFieldsTableLayoutPanel, 0, 0);
            this.SearchConditionsRootTableLayoutPanel.Controls.Add(this.ConditionsTabControl, 0, 1);
            this.SearchConditionsRootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionsRootTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchConditionsRootTableLayoutPanel.Name = "SearchConditionsRootTableLayoutPanel";
            this.SearchConditionsRootTableLayoutPanel.RowCount = 2;
            this.SearchConditionsRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.09348F));
            this.SearchConditionsRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.90652F));
            this.SearchConditionsRootTableLayoutPanel.Size = new System.Drawing.Size(616, 353);
            this.SearchConditionsRootTableLayoutPanel.TabIndex = 0;
            // 
            // SearchTextFieldsTableLayoutPanel
            // 
            this.SearchTextFieldsTableLayoutPanel.ColumnCount = 1;
            this.SearchTextFieldsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchTextFieldsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchTextFieldsTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.SearchTextFieldsTableLayoutPanel.Name = "SearchTextFieldsTableLayoutPanel";
            this.SearchTextFieldsTableLayoutPanel.RowCount = 2;
            this.SearchTextFieldsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchTextFieldsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchTextFieldsTableLayoutPanel.Size = new System.Drawing.Size(610, 131);
            this.SearchTextFieldsTableLayoutPanel.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // ConditionsTabControl
            // 
            this.ConditionsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConditionsTabControl.Location = new System.Drawing.Point(3, 140);
            this.ConditionsTabControl.Name = "ConditionsTabControl";
            this.ConditionsTabControl.SelectedIndex = 0;
            this.ConditionsTabControl.Size = new System.Drawing.Size(610, 210);
            this.ConditionsTabControl.TabIndex = 1;
            // 
            // SearchConditionsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SearchConditionsRootTableLayoutPanel);
            this.Name = "SearchConditionsUserControl";
            this.Size = new System.Drawing.Size(616, 353);
            this.SearchConditionsRootTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel SearchConditionsRootTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel SearchTextFieldsTableLayoutPanel;
        private System.Windows.Forms.TabControl ConditionsTabControl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}

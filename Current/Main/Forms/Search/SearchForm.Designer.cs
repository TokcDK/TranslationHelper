namespace TranslationHelper.Forms.Search
{
    partial class SearchForm
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
            this.SearchRootPanel = new System.Windows.Forms.Panel();
            this.SearchRootTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SearchButtonsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SearchAllButton = new System.Windows.Forms.Button();
            this.ReplaceAllButton = new System.Windows.Forms.Button();
            this.SearchConditionsTabControl = new System.Windows.Forms.TabControl();
            this.SearchRootPanel.SuspendLayout();
            this.SearchRootTableLayoutPanel.SuspendLayout();
            this.SearchButtonsTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchRootPanel
            // 
            this.SearchRootPanel.Controls.Add(this.SearchRootTableLayoutPanel);
            this.SearchRootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchRootPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchRootPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchRootPanel.Name = "SearchRootPanel";
            this.SearchRootPanel.Size = new System.Drawing.Size(765, 418);
            this.SearchRootPanel.TabIndex = 0;
            // 
            // SearchRootTableLayoutPanel
            // 
            this.SearchRootTableLayoutPanel.ColumnCount = 1;
            this.SearchRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.5F));
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchConditionsTabControl, 0, 0);
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchButtonsTableLayoutPanel, 0, 1);
            this.SearchRootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchRootTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchRootTableLayoutPanel.Margin = new System.Windows.Forms.Padding(1);
            this.SearchRootTableLayoutPanel.Name = "SearchRootTableLayoutPanel";
            this.SearchRootTableLayoutPanel.RowCount = 3;
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.88889F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.11111F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.SearchRootTableLayoutPanel.Size = new System.Drawing.Size(765, 418);
            this.SearchRootTableLayoutPanel.TabIndex = 0;
            // 
            // SearchButtonsTableLayoutPanel
            // 
            this.SearchButtonsTableLayoutPanel.ColumnCount = 2;
            this.SearchButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 388F));
            this.SearchButtonsTableLayoutPanel.Controls.Add(this.SearchAllButton, 0, 0);
            this.SearchButtonsTableLayoutPanel.Controls.Add(this.ReplaceAllButton, 1, 0);
            this.SearchButtonsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchButtonsTableLayoutPanel.Location = new System.Drawing.Point(10, 192);
            this.SearchButtonsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(10);
            this.SearchButtonsTableLayoutPanel.Name = "SearchButtonsTableLayoutPanel";
            this.SearchButtonsTableLayoutPanel.RowCount = 1;
            this.SearchButtonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchButtonsTableLayoutPanel.Size = new System.Drawing.Size(745, 62);
            this.SearchButtonsTableLayoutPanel.TabIndex = 0;
            // 
            // SearchAllButton
            // 
            this.SearchAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchAllButton.Location = new System.Drawing.Point(3, 3);
            this.SearchAllButton.Name = "SearchAllButton";
            this.SearchAllButton.Size = new System.Drawing.Size(351, 56);
            this.SearchAllButton.TabIndex = 1;
            this.SearchAllButton.Text = "Search all";
            this.SearchAllButton.UseVisualStyleBackColor = true;
            this.SearchAllButton.Click += new System.EventHandler(this.SearchAllButton_Click);
            // 
            // ReplaceAllButton
            // 
            this.ReplaceAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReplaceAllButton.Location = new System.Drawing.Point(360, 3);
            this.ReplaceAllButton.Name = "ReplaceAllButton";
            this.ReplaceAllButton.Size = new System.Drawing.Size(382, 56);
            this.ReplaceAllButton.TabIndex = 3;
            this.ReplaceAllButton.Text = "Replace all";
            this.ReplaceAllButton.UseVisualStyleBackColor = true;
            this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
            // 
            // SearchConditionsTabControl
            // 
            this.SearchConditionsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionsTabControl.Location = new System.Drawing.Point(3, 3);
            this.SearchConditionsTabControl.Name = "SearchConditionsTabControl";
            this.SearchConditionsTabControl.SelectedIndex = 0;
            this.SearchConditionsTabControl.Size = new System.Drawing.Size(759, 176);
            this.SearchConditionsTabControl.TabIndex = 1;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 418);
            this.Controls.Add(this.SearchRootPanel);
            this.Name = "SearchForm";
            this.Text = "Search";
            this.SearchRootPanel.ResumeLayout(false);
            this.SearchRootTableLayoutPanel.ResumeLayout(false);
            this.SearchButtonsTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SearchRootPanel;
        private System.Windows.Forms.TableLayoutPanel SearchRootTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel SearchButtonsTableLayoutPanel;
        private System.Windows.Forms.Button ReplaceAllButton;
        private System.Windows.Forms.Button SearchAllButton;
        internal System.Windows.Forms.TabControl SearchConditionsTabControl;
    }
}
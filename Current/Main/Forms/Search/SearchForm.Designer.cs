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
            this.SearchConditionsPanel = new System.Windows.Forms.Panel();
            this.SearchResultsPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FoundRowsPanel = new System.Windows.Forms.Panel();
            this.SearchResultInfoLabel = new System.Windows.Forms.Label();
            this.SearchRootPanel.SuspendLayout();
            this.SearchRootTableLayoutPanel.SuspendLayout();
            this.SearchButtonsTableLayoutPanel.SuspendLayout();
            this.SearchResultsPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
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
            this.SearchRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchButtonsTableLayoutPanel, 0, 1);
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchConditionsPanel, 0, 0);
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchResultsPanel, 0, 2);
            this.SearchRootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchRootTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchRootTableLayoutPanel.Margin = new System.Windows.Forms.Padding(1);
            this.SearchRootTableLayoutPanel.Name = "SearchRootTableLayoutPanel";
            this.SearchRootTableLayoutPanel.RowCount = 3;
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
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
            this.SearchButtonsTableLayoutPanel.Location = new System.Drawing.Point(10, 211);
            this.SearchButtonsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(10);
            this.SearchButtonsTableLayoutPanel.Name = "SearchButtonsTableLayoutPanel";
            this.SearchButtonsTableLayoutPanel.RowCount = 1;
            this.SearchButtonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchButtonsTableLayoutPanel.Size = new System.Drawing.Size(745, 47);
            this.SearchButtonsTableLayoutPanel.TabIndex = 0;
            // 
            // SearchAllButton
            // 
            this.SearchAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchAllButton.Location = new System.Drawing.Point(3, 3);
            this.SearchAllButton.Name = "SearchAllButton";
            this.SearchAllButton.Size = new System.Drawing.Size(351, 41);
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
            this.ReplaceAllButton.Size = new System.Drawing.Size(382, 41);
            this.ReplaceAllButton.TabIndex = 3;
            this.ReplaceAllButton.Text = "Replace all";
            this.ReplaceAllButton.UseVisualStyleBackColor = true;
            this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
            // 
            // SearchConditionsPanel
            // 
            this.SearchConditionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionsPanel.Location = new System.Drawing.Point(3, 3);
            this.SearchConditionsPanel.Name = "SearchConditionsPanel";
            this.SearchConditionsPanel.Size = new System.Drawing.Size(759, 195);
            this.SearchConditionsPanel.TabIndex = 3;
            // 
            // SearchResultsPanel
            // 
            this.SearchResultsPanel.Controls.Add(this.tableLayoutPanel1);
            this.SearchResultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchResultsPanel.Location = new System.Drawing.Point(3, 271);
            this.SearchResultsPanel.Name = "SearchResultsPanel";
            this.SearchResultsPanel.Size = new System.Drawing.Size(759, 144);
            this.SearchResultsPanel.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.FoundRowsPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SearchResultInfoLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(759, 144);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // FoundRowsPanel
            // 
            this.FoundRowsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FoundRowsPanel.Location = new System.Drawing.Point(3, 25);
            this.FoundRowsPanel.Name = "FoundRowsPanel";
            this.FoundRowsPanel.Size = new System.Drawing.Size(753, 116);
            this.FoundRowsPanel.TabIndex = 0;
            // 
            // SearchResultInfoLabel
            // 
            this.SearchResultInfoLabel.AutoSize = true;
            this.SearchResultInfoLabel.Location = new System.Drawing.Point(3, 0);
            this.SearchResultInfoLabel.Name = "SearchResultInfoLabel";
            this.SearchResultInfoLabel.Size = new System.Drawing.Size(25, 13);
            this.SearchResultInfoLabel.TabIndex = 1;
            this.SearchResultInfoLabel.Text = "___";
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
            this.SearchResultsPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SearchRootPanel;
        private System.Windows.Forms.TableLayoutPanel SearchRootTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel SearchButtonsTableLayoutPanel;
        private System.Windows.Forms.Button ReplaceAllButton;
        private System.Windows.Forms.Button SearchAllButton;
        private System.Windows.Forms.Panel SearchConditionsPanel;
        private System.Windows.Forms.Panel SearchResultsPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel FoundRowsPanel;
        private System.Windows.Forms.Label SearchResultInfoLabel;
    }
}
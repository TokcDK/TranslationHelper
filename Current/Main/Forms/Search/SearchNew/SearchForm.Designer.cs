namespace TranslationHelper.Forms.Search.SearchNew
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.AddNewSearchConditionTabButton = new System.Windows.Forms.Button();
            this.SearchConditionTabsPanel = new System.Windows.Forms.Panel();
            this.SearchResultsPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FoundRowsPanel = new System.Windows.Forms.Panel();
            this.SearchResultInfoLabel = new System.Windows.Forms.Label();
            this.SearchRootPanel.SuspendLayout();
            this.SearchRootTableLayoutPanel.SuspendLayout();
            this.SearchButtonsTableLayoutPanel.SuspendLayout();
            this.SearchConditionsPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.SearchRootPanel.Size = new System.Drawing.Size(584, 421);
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
            this.SearchRootTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchRootTableLayoutPanel.Name = "SearchRootTableLayoutPanel";
            this.SearchRootTableLayoutPanel.RowCount = 3;
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.SearchRootTableLayoutPanel.Size = new System.Drawing.Size(584, 421);
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
            this.SearchButtonsTableLayoutPanel.Location = new System.Drawing.Point(0, 234);
            this.SearchButtonsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchButtonsTableLayoutPanel.Name = "SearchButtonsTableLayoutPanel";
            this.SearchButtonsTableLayoutPanel.RowCount = 1;
            this.SearchButtonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchButtonsTableLayoutPanel.Size = new System.Drawing.Size(584, 30);
            this.SearchButtonsTableLayoutPanel.TabIndex = 0;
            // 
            // SearchAllButton
            // 
            this.SearchAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchAllButton.Location = new System.Drawing.Point(3, 3);
            this.SearchAllButton.Name = "SearchAllButton";
            this.SearchAllButton.Size = new System.Drawing.Size(190, 24);
            this.SearchAllButton.TabIndex = 1;
            this.SearchAllButton.Text = "Search all";
            this.SearchAllButton.UseVisualStyleBackColor = true;
            this.SearchAllButton.Click += new System.EventHandler(this.SearchAllButton_Click);
            // 
            // ReplaceAllButton
            // 
            this.ReplaceAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReplaceAllButton.Location = new System.Drawing.Point(199, 3);
            this.ReplaceAllButton.Name = "ReplaceAllButton";
            this.ReplaceAllButton.Size = new System.Drawing.Size(382, 24);
            this.ReplaceAllButton.TabIndex = 3;
            this.ReplaceAllButton.Text = "Replace all";
            this.ReplaceAllButton.UseVisualStyleBackColor = true;
            this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
            // 
            // SearchConditionsPanel
            // 
            this.SearchConditionsPanel.Controls.Add(this.tableLayoutPanel2);
            this.SearchConditionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionsPanel.Location = new System.Drawing.Point(3, 3);
            this.SearchConditionsPanel.Name = "SearchConditionsPanel";
            this.SearchConditionsPanel.Size = new System.Drawing.Size(578, 228);
            this.SearchConditionsPanel.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.AddNewSearchConditionTabButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.SearchConditionTabsPanel, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(578, 228);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // AddNewSearchConditionTabButton
            // 
            this.AddNewSearchConditionTabButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddNewSearchConditionTabButton.Location = new System.Drawing.Point(561, 3);
            this.AddNewSearchConditionTabButton.Name = "AddNewSearchConditionTabButton";
            this.AddNewSearchConditionTabButton.Size = new System.Drawing.Size(14, 222);
            this.AddNewSearchConditionTabButton.TabIndex = 0;
            this.AddNewSearchConditionTabButton.Text = "+";
            this.AddNewSearchConditionTabButton.UseVisualStyleBackColor = true;
            // 
            // SearchConditionTabsPanel
            // 
            this.SearchConditionTabsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionTabsPanel.Location = new System.Drawing.Point(3, 3);
            this.SearchConditionTabsPanel.Name = "SearchConditionTabsPanel";
            this.SearchConditionTabsPanel.Size = new System.Drawing.Size(552, 222);
            this.SearchConditionTabsPanel.TabIndex = 1;
            // 
            // SearchResultsPanel
            // 
            this.SearchResultsPanel.Controls.Add(this.tableLayoutPanel1);
            this.SearchResultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchResultsPanel.Location = new System.Drawing.Point(3, 267);
            this.SearchResultsPanel.Name = "SearchResultsPanel";
            this.SearchResultsPanel.Size = new System.Drawing.Size(578, 151);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(578, 151);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // FoundRowsPanel
            // 
            this.FoundRowsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FoundRowsPanel.Location = new System.Drawing.Point(3, 25);
            this.FoundRowsPanel.Name = "FoundRowsPanel";
            this.FoundRowsPanel.Size = new System.Drawing.Size(572, 123);
            this.FoundRowsPanel.TabIndex = 0;
            // 
            // SearchResultInfoLabel
            // 
            this.SearchResultInfoLabel.AutoSize = true;
            this.SearchResultInfoLabel.Location = new System.Drawing.Point(3, 0);
            this.SearchResultInfoLabel.Name = "SearchResultInfoLabel";
            this.SearchResultInfoLabel.Size = new System.Drawing.Size(22, 13);
            this.SearchResultInfoLabel.TabIndex = 1;
            this.SearchResultInfoLabel.Text = "     ";
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 421);
            this.Controls.Add(this.SearchRootPanel);
            this.Name = "SearchForm";
            this.Text = "Search";
            this.SearchRootPanel.ResumeLayout(false);
            this.SearchRootTableLayoutPanel.ResumeLayout(false);
            this.SearchButtonsTableLayoutPanel.ResumeLayout(false);
            this.SearchConditionsPanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button AddNewSearchConditionTabButton;
        private System.Windows.Forms.Panel SearchConditionTabsPanel;
    }
}
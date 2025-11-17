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
            this.SearchRootPanel.BackColor = System.Drawing.Color.White;
            this.SearchRootPanel.Controls.Add(this.SearchRootTableLayoutPanel);
            this.SearchRootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchRootPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchRootPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchRootPanel.Name = "SearchRootPanel";
            this.SearchRootPanel.Size = new System.Drawing.Size(800, 600);
            this.SearchRootPanel.TabIndex = 0;
            // 
            // SearchRootTableLayoutPanel
            // 
            this.SearchRootTableLayoutPanel.BackColor = System.Drawing.Color.White;
            this.SearchRootTableLayoutPanel.ColumnCount = 1;
            this.SearchRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchButtonsTableLayoutPanel, 0, 1);
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchConditionsPanel, 0, 0);
            this.SearchRootTableLayoutPanel.Controls.Add(this.SearchResultsPanel, 0, 2);
            this.SearchRootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchRootTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchRootTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchRootTableLayoutPanel.Name = "SearchRootTableLayoutPanel";
            this.SearchRootTableLayoutPanel.Padding = new System.Windows.Forms.Padding(12);
            this.SearchRootTableLayoutPanel.RowCount = 3;
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.SearchRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.SearchRootTableLayoutPanel.Size = new System.Drawing.Size(800, 600);
            this.SearchRootTableLayoutPanel.TabIndex = 0;
            // 
            // SearchButtonsTableLayoutPanel
            // 
            this.SearchButtonsTableLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.SearchButtonsTableLayoutPanel.ColumnCount = 3;
            this.SearchButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.SearchButtonsTableLayoutPanel.Controls.Add(this.SearchAllButton, 0, 0);
            this.SearchButtonsTableLayoutPanel.Controls.Add(this.ReplaceAllButton, 1, 0);
            this.SearchButtonsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchButtonsTableLayoutPanel.Location = new System.Drawing.Point(12, 302);
            this.SearchButtonsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchButtonsTableLayoutPanel.Name = "SearchButtonsTableLayoutPanel";
            this.SearchButtonsTableLayoutPanel.RowCount = 1;
            this.SearchButtonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchButtonsTableLayoutPanel.Size = new System.Drawing.Size(776, 48);
            this.SearchButtonsTableLayoutPanel.TabIndex = 0;
            // 
            // SearchAllButton
            // 
            this.SearchAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.SearchAllButton.FlatAppearance.BorderSize = 0;
            this.SearchAllButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.SearchAllButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(235)))));
            this.SearchAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchAllButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchAllButton.ForeColor = System.Drawing.Color.White;
            this.SearchAllButton.Location = new System.Drawing.Point(3, 12);
            this.SearchAllButton.Margin = new System.Windows.Forms.Padding(3, 12, 4, 12);
            this.SearchAllButton.Name = "SearchAllButton";
            this.SearchAllButton.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.SearchAllButton.Size = new System.Drawing.Size(377, 24);
            this.SearchAllButton.TabIndex = 1;
            this.SearchAllButton.Text = "Search All";
            this.SearchAllButton.UseVisualStyleBackColor = false;
            this.SearchAllButton.Click += new System.EventHandler(this.SearchAllButton_Click);
            // 
            // ReplaceAllButton
            // 
            this.ReplaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.ReplaceAllButton.FlatAppearance.BorderSize = 0;
            this.ReplaceAllButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(140)))), ((int)(((byte)(60)))));
            this.ReplaceAllButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(180)))), ((int)(((byte)(80)))));
            this.ReplaceAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReplaceAllButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceAllButton.ForeColor = System.Drawing.Color.White;
            this.ReplaceAllButton.Location = new System.Drawing.Point(388, 12);
            this.ReplaceAllButton.Margin = new System.Windows.Forms.Padding(4, 12, 3, 12);
            this.ReplaceAllButton.Name = "ReplaceAllButton";
            this.ReplaceAllButton.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.ReplaceAllButton.Size = new System.Drawing.Size(377, 24);
            this.ReplaceAllButton.TabIndex = 3;
            this.ReplaceAllButton.Text = "Replace All";
            this.ReplaceAllButton.UseVisualStyleBackColor = false;
            this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
            // 
            // SearchConditionsPanel
            // 
            this.SearchConditionsPanel.BackColor = System.Drawing.Color.White;
            this.SearchConditionsPanel.Controls.Add(this.tableLayoutPanel2);
            this.SearchConditionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionsPanel.Location = new System.Drawing.Point(12, 12);
            this.SearchConditionsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SearchConditionsPanel.Name = "SearchConditionsPanel";
            this.SearchConditionsPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.SearchConditionsPanel.Size = new System.Drawing.Size(776, 290);
            this.SearchConditionsPanel.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.Controls.Add(this.AddNewSearchConditionTabButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.SearchConditionTabsPanel, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(776, 282);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // AddNewSearchConditionTabButton
            // 
            this.AddNewSearchConditionTabButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.AddNewSearchConditionTabButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(58)))), ((int)(((byte)(64)))));
            this.AddNewSearchConditionTabButton.FlatAppearance.BorderSize = 0;
            this.AddNewSearchConditionTabButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.AddNewSearchConditionTabButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.AddNewSearchConditionTabButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddNewSearchConditionTabButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddNewSearchConditionTabButton.ForeColor = System.Drawing.Color.White;
            this.AddNewSearchConditionTabButton.Location = new System.Drawing.Point(739, 3);
            this.AddNewSearchConditionTabButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.AddNewSearchConditionTabButton.Name = "AddNewSearchConditionTabButton";
            this.AddNewSearchConditionTabButton.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.AddNewSearchConditionTabButton.Size = new System.Drawing.Size(34, 279);
            this.AddNewSearchConditionTabButton.TabIndex = 0;
            this.AddNewSearchConditionTabButton.Text = "+";
            this.AddNewSearchConditionTabButton.UseVisualStyleBackColor = false;
            // 
            // SearchConditionTabsPanel
            // 
            this.SearchConditionTabsPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SearchConditionTabsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionTabsPanel.Location = new System.Drawing.Point(3, 3);
            this.SearchConditionTabsPanel.Name = "SearchConditionTabsPanel";
            this.SearchConditionTabsPanel.Size = new System.Drawing.Size(730, 276);
            this.SearchConditionTabsPanel.TabIndex = 1;
            // 
            // SearchResultsPanel
            // 
            this.SearchResultsPanel.BackColor = System.Drawing.Color.White;
            this.SearchResultsPanel.Controls.Add(this.tableLayoutPanel1);
            this.SearchResultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchResultsPanel.Location = new System.Drawing.Point(15, 353);
            this.SearchResultsPanel.Name = "SearchResultsPanel";
            this.SearchResultsPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.SearchResultsPanel.Size = new System.Drawing.Size(770, 232);
            this.SearchResultsPanel.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.FoundRowsPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SearchResultInfoLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(770, 224);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // FoundRowsPanel
            // 
            this.FoundRowsPanel.BackColor = System.Drawing.Color.White;
            this.FoundRowsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FoundRowsPanel.Location = new System.Drawing.Point(3, 27);
            this.FoundRowsPanel.Name = "FoundRowsPanel";
            this.FoundRowsPanel.Size = new System.Drawing.Size(764, 194);
            this.FoundRowsPanel.TabIndex = 0;
            // 
            // SearchResultInfoLabel
            // 
            this.SearchResultInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchResultInfoLabel.BackColor = System.Drawing.Color.Transparent;
            this.SearchResultInfoLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchResultInfoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.SearchResultInfoLabel.Location = new System.Drawing.Point(3, 0);
            this.SearchResultInfoLabel.Name = "SearchResultInfoLabel";
            this.SearchResultInfoLabel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.SearchResultInfoLabel.Size = new System.Drawing.Size(764, 24);
            this.SearchResultInfoLabel.TabIndex = 1;
            this.SearchResultInfoLabel.Text = "No results found. Refine your search criteria.";
            this.SearchResultInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.SearchRootPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Search";
            this.SearchRootPanel.ResumeLayout(false);
            this.SearchRootTableLayoutPanel.ResumeLayout(false);
            this.SearchButtonsTableLayoutPanel.ResumeLayout(false);
            this.SearchConditionsPanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.SearchResultsPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
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
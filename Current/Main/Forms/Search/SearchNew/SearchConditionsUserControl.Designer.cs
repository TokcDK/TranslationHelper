using Vip.ComboBox;

namespace TranslationHelper.Forms.Search
{
    partial class SearchConditionUserControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.FindWhatComboBox = new Vip.ComboBox.ComboBoxVip();
            this.SearchOptionsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SearchOptionSelectedColumnComboBox = new System.Windows.Forms.ComboBox();
            this.SearchOptionCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchOptionRegexCheckBox = new System.Windows.Forms.CheckBox();
            this.ReplaceWhatWithPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SearchConditionsRootTableLayoutPanel.SuspendLayout();
            this.SearchTextFieldsTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SearchOptionsFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchConditionsRootTableLayoutPanel
            // 
            this.SearchConditionsRootTableLayoutPanel.ColumnCount = 1;
            this.SearchConditionsRootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchConditionsRootTableLayoutPanel.Controls.Add(this.SearchTextFieldsTableLayoutPanel, 0, 0);
            this.SearchConditionsRootTableLayoutPanel.Controls.Add(this.ReplaceWhatWithPanel, 0, 1);
            this.SearchConditionsRootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchConditionsRootTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchConditionsRootTableLayoutPanel.Name = "SearchConditionsRootTableLayoutPanel";
            this.SearchConditionsRootTableLayoutPanel.RowCount = 2;
            this.SearchConditionsRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchConditionsRootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SearchConditionsRootTableLayoutPanel.Size = new System.Drawing.Size(616, 212);
            this.SearchConditionsRootTableLayoutPanel.TabIndex = 0;
            // 
            // SearchTextFieldsTableLayoutPanel
            // 
            this.SearchTextFieldsTableLayoutPanel.ColumnCount = 1;
            this.SearchTextFieldsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SearchTextFieldsTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.SearchTextFieldsTableLayoutPanel.Controls.Add(this.SearchOptionsFlowLayoutPanel, 0, 1);
            this.SearchTextFieldsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchTextFieldsTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.SearchTextFieldsTableLayoutPanel.Name = "SearchTextFieldsTableLayoutPanel";
            this.SearchTextFieldsTableLayoutPanel.RowCount = 2;
            this.SearchTextFieldsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SearchTextFieldsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SearchTextFieldsTableLayoutPanel.Size = new System.Drawing.Size(610, 100);
            this.SearchTextFieldsTableLayoutPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.57616F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.42384F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FindWhatComboBox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 24);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FindWhatComboBox
            // 
            this.FindWhatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FindWhatComboBox.FormattingEnabled = true;
            this.FindWhatComboBox.Location = new System.Drawing.Point(85, 3);
            this.FindWhatComboBox.Name = "FindWhatComboBox";
            this.FindWhatComboBox.Size = new System.Drawing.Size(516, 21);
            this.FindWhatComboBox.TabIndex = 1;
            // 
            // SearchOptionsFlowLayoutPanel
            // 
            this.SearchOptionsFlowLayoutPanel.Controls.Add(this.SearchOptionSelectedColumnComboBox);
            this.SearchOptionsFlowLayoutPanel.Controls.Add(this.SearchOptionCaseSensitiveCheckBox);
            this.SearchOptionsFlowLayoutPanel.Controls.Add(this.SearchOptionRegexCheckBox);
            this.SearchOptionsFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchOptionsFlowLayoutPanel.Location = new System.Drawing.Point(3, 33);
            this.SearchOptionsFlowLayoutPanel.Name = "SearchOptionsFlowLayoutPanel";
            this.SearchOptionsFlowLayoutPanel.Size = new System.Drawing.Size(604, 64);
            this.SearchOptionsFlowLayoutPanel.TabIndex = 1;
            // 
            // SearchOptionSelectedColumnComboBox
            // 
            this.SearchOptionSelectedColumnComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SearchOptionSelectedColumnComboBox.FormattingEnabled = true;
            this.SearchOptionSelectedColumnComboBox.Location = new System.Drawing.Point(3, 3);
            this.SearchOptionSelectedColumnComboBox.Name = "SearchOptionSelectedColumnComboBox";
            this.SearchOptionSelectedColumnComboBox.Size = new System.Drawing.Size(153, 21);
            this.SearchOptionSelectedColumnComboBox.TabIndex = 2;
            // 
            // SearchOptionCaseSensitiveCheckBox
            // 
            this.SearchOptionCaseSensitiveCheckBox.AutoSize = true;
            this.SearchOptionCaseSensitiveCheckBox.Location = new System.Drawing.Point(162, 3);
            this.SearchOptionCaseSensitiveCheckBox.Name = "SearchOptionCaseSensitiveCheckBox";
            this.SearchOptionCaseSensitiveCheckBox.Size = new System.Drawing.Size(94, 17);
            this.SearchOptionCaseSensitiveCheckBox.TabIndex = 0;
            this.SearchOptionCaseSensitiveCheckBox.Text = "Case sensitive";
            this.SearchOptionCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchOptionRegexCheckBox
            // 
            this.SearchOptionRegexCheckBox.AutoSize = true;
            this.SearchOptionRegexCheckBox.Location = new System.Drawing.Point(262, 3);
            this.SearchOptionRegexCheckBox.Name = "SearchOptionRegexCheckBox";
            this.SearchOptionRegexCheckBox.Size = new System.Drawing.Size(57, 17);
            this.SearchOptionRegexCheckBox.TabIndex = 1;
            this.SearchOptionRegexCheckBox.Text = "Regex";
            this.SearchOptionRegexCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReplaceWhatWithPanel
            // 
            this.ReplaceWhatWithPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReplaceWhatWithPanel.Location = new System.Drawing.Point(3, 109);
            this.ReplaceWhatWithPanel.Name = "ReplaceWhatWithPanel";
            this.ReplaceWhatWithPanel.Size = new System.Drawing.Size(610, 100);
            this.ReplaceWhatWithPanel.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // SearchConditionUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SearchConditionsRootTableLayoutPanel);
            this.Name = "SearchConditionUserControl";
            this.Size = new System.Drawing.Size(616, 212);
            this.Load += new System.EventHandler(this.SearchConditionUserControl_Load);
            this.SearchConditionsRootTableLayoutPanel.ResumeLayout(false);
            this.SearchTextFieldsTableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.SearchOptionsFlowLayoutPanel.ResumeLayout(false);
            this.SearchOptionsFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel SearchConditionsRootTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel SearchTextFieldsTableLayoutPanel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.ComboBox SearchOptionSelectedColumnComboBox;
        internal System.Windows.Forms.CheckBox SearchOptionCaseSensitiveCheckBox;
        internal System.Windows.Forms.CheckBox SearchOptionRegexCheckBox;
        private System.Windows.Forms.Panel ReplaceWhatWithPanel;
        internal ComboBoxVip FindWhatComboBox;
        internal System.Windows.Forms.FlowLayoutPanel SearchOptionsFlowLayoutPanel;
    }
}

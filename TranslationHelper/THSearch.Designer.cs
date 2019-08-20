namespace TranslationHelper
{
    partial class THSearch
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
            this.THSearchTabs = new System.Windows.Forms.TabControl();
            this.THSearch1st = new System.Windows.Forms.TabPage();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.THSearchPanel = new System.Windows.Forms.Panel();
            this.SearchModeGroupBox = new System.Windows.Forms.GroupBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.SearchModeNormalRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchModeRegexRadioButton = new System.Windows.Forms.RadioButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.SearchRangeTableRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchRangeAllRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.SearchMethodTranslationRadioButton = new System.Windows.Forms.RadioButton();
            this.SearchMethodOriginalTranslationRadioButton = new System.Windows.Forms.RadioButton();
            this.THSearchMatchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.FindAllButton = new System.Windows.Forms.Button();
            this.SearchFormFindNextButton = new System.Windows.Forms.Button();
            this.SearchFormReplaceButton = new System.Windows.Forms.Button();
            this.SearchFormReplaceAllButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.THSearchFindWhatLabel = new System.Windows.Forms.Label();
            this.SearchFormFindWhatComboBox = new System.Windows.Forms.ComboBox();
            this.SearchFormReplaceWithComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.THSearchTabs.SuspendLayout();
            this.THSearch1st.SuspendLayout();
            this.THSearchPanel.SuspendLayout();
            this.SearchModeGroupBox.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // THSearchTabs
            // 
            this.THSearchTabs.Controls.Add(this.THSearch1st);
            this.THSearchTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSearchTabs.Location = new System.Drawing.Point(0, 0);
            this.THSearchTabs.Name = "THSearchTabs";
            this.THSearchTabs.SelectedIndex = 0;
            this.THSearchTabs.Size = new System.Drawing.Size(580, 334);
            this.THSearchTabs.TabIndex = 2;
            // 
            // THSearch1st
            // 
            this.THSearch1st.Controls.Add(this.linkLabel2);
            this.THSearch1st.Controls.Add(this.label3);
            this.THSearch1st.Controls.Add(this.lblError);
            this.THSearch1st.Controls.Add(this.THSearchPanel);
            this.THSearch1st.Location = new System.Drawing.Point(4, 22);
            this.THSearch1st.Name = "THSearch1st";
            this.THSearch1st.Padding = new System.Windows.Forms.Padding(3);
            this.THSearch1st.Size = new System.Drawing.Size(572, 308);
            this.THSearch1st.TabIndex = 0;
            this.THSearch1st.Text = "Find and Replace";
            this.THSearch1st.UseVisualStyleBackColor = true;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel2.Location = new System.Drawing.Point(375, 294);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(193, 13);
            this.linkLabel2.TabIndex = 13;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://patreon.com/TranslationHelper";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 294);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Translation  Helper support:";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(6, 294);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(25, 13);
            this.lblError.TabIndex = 18;
            this.lblError.Text = "info";
            this.lblError.Visible = false;
            // 
            // THSearchPanel
            // 
            this.THSearchPanel.Controls.Add(this.SearchModeGroupBox);
            this.THSearchPanel.Controls.Add(this.THSearchMatchCaseCheckBox);
            this.THSearchPanel.Controls.Add(this.panel4);
            this.THSearchPanel.Controls.Add(this.panel3);
            this.THSearchPanel.Location = new System.Drawing.Point(3, 3);
            this.THSearchPanel.Name = "THSearchPanel";
            this.THSearchPanel.Size = new System.Drawing.Size(565, 293);
            this.THSearchPanel.TabIndex = 7;
            // 
            // SearchModeGroupBox
            // 
            this.SearchModeGroupBox.Controls.Add(this.panel6);
            this.SearchModeGroupBox.Location = new System.Drawing.Point(0, 208);
            this.SearchModeGroupBox.Name = "SearchModeGroupBox";
            this.SearchModeGroupBox.Size = new System.Drawing.Size(565, 85);
            this.SearchModeGroupBox.TabIndex = 20;
            this.SearchModeGroupBox.TabStop = false;
            this.SearchModeGroupBox.Text = "Search Mode";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel2);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Controls.Add(this.panel1);
            this.panel6.Location = new System.Drawing.Point(3, 19);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(471, 61);
            this.panel6.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.SearchModeNormalRadioButton);
            this.panel2.Controls.Add(this.SearchModeRegexRadioButton);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(81, 61);
            this.panel2.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Input";
            // 
            // SearchModeNormalRadioButton
            // 
            this.SearchModeNormalRadioButton.AutoSize = true;
            this.SearchModeNormalRadioButton.Checked = true;
            this.SearchModeNormalRadioButton.Location = new System.Drawing.Point(3, 16);
            this.SearchModeNormalRadioButton.Name = "SearchModeNormalRadioButton";
            this.SearchModeNormalRadioButton.Size = new System.Drawing.Size(58, 17);
            this.SearchModeNormalRadioButton.TabIndex = 7;
            this.SearchModeNormalRadioButton.TabStop = true;
            this.SearchModeNormalRadioButton.Text = "Normal";
            this.SearchModeNormalRadioButton.UseVisualStyleBackColor = true;
            this.SearchModeNormalRadioButton.Click += new System.EventHandler(this.SearchModeNormalRadioButton_Click);
            // 
            // SearchModeRegexRadioButton
            // 
            this.SearchModeRegexRadioButton.AutoSize = true;
            this.SearchModeRegexRadioButton.Location = new System.Drawing.Point(3, 39);
            this.SearchModeRegexRadioButton.Name = "SearchModeRegexRadioButton";
            this.SearchModeRegexRadioButton.Size = new System.Drawing.Size(56, 17);
            this.SearchModeRegexRadioButton.TabIndex = 8;
            this.SearchModeRegexRadioButton.Text = "Regex";
            this.SearchModeRegexRadioButton.UseVisualStyleBackColor = true;
            this.SearchModeRegexRadioButton.Click += new System.EventHandler(this.SearchModeRegexRadioButton_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.SearchRangeTableRadioButton);
            this.panel5.Controls.Add(this.SearchRangeAllRadioButton);
            this.panel5.Location = new System.Drawing.Point(376, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(81, 61);
            this.panel5.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Range";
            // 
            // SearchRangeTableRadioButton
            // 
            this.SearchRangeTableRadioButton.AutoSize = true;
            this.SearchRangeTableRadioButton.Checked = true;
            this.SearchRangeTableRadioButton.Location = new System.Drawing.Point(3, 16);
            this.SearchRangeTableRadioButton.Name = "SearchRangeTableRadioButton";
            this.SearchRangeTableRadioButton.Size = new System.Drawing.Size(51, 17);
            this.SearchRangeTableRadioButton.TabIndex = 7;
            this.SearchRangeTableRadioButton.TabStop = true;
            this.SearchRangeTableRadioButton.Text = "Table";
            this.SearchRangeTableRadioButton.UseVisualStyleBackColor = true;
            this.SearchRangeTableRadioButton.Click += new System.EventHandler(this.SearchRangeTableRadioButton_Click);
            // 
            // SearchRangeAllRadioButton
            // 
            this.SearchRangeAllRadioButton.AutoSize = true;
            this.SearchRangeAllRadioButton.Location = new System.Drawing.Point(3, 39);
            this.SearchRangeAllRadioButton.Name = "SearchRangeAllRadioButton";
            this.SearchRangeAllRadioButton.Size = new System.Drawing.Size(36, 17);
            this.SearchRangeAllRadioButton.TabIndex = 8;
            this.SearchRangeAllRadioButton.Text = "All";
            this.SearchRangeAllRadioButton.UseVisualStyleBackColor = true;
            this.SearchRangeAllRadioButton.Click += new System.EventHandler(this.SearchRangeAllRadioButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.SearchMethodTranslationRadioButton);
            this.panel1.Controls.Add(this.SearchMethodOriginalTranslationRadioButton);
            this.panel1.Location = new System.Drawing.Point(114, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 61);
            this.panel1.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, -1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Method";
            // 
            // SearchMethodTranslationRadioButton
            // 
            this.SearchMethodTranslationRadioButton.AutoSize = true;
            this.SearchMethodTranslationRadioButton.Location = new System.Drawing.Point(3, 39);
            this.SearchMethodTranslationRadioButton.Name = "SearchMethodTranslationRadioButton";
            this.SearchMethodTranslationRadioButton.Size = new System.Drawing.Size(215, 17);
            this.SearchMethodTranslationRadioButton.TabIndex = 9;
            this.SearchMethodTranslationRadioButton.Text = "Find in Original and Paste to Translation";
            this.SearchMethodTranslationRadioButton.UseVisualStyleBackColor = true;
            this.SearchMethodTranslationRadioButton.Click += new System.EventHandler(this.SearchMethodTranslationRadioButton_Click);
            // 
            // SearchMethodOriginalTranslationRadioButton
            // 
            this.SearchMethodOriginalTranslationRadioButton.AutoSize = true;
            this.SearchMethodOriginalTranslationRadioButton.Checked = true;
            this.SearchMethodOriginalTranslationRadioButton.Location = new System.Drawing.Point(3, 16);
            this.SearchMethodOriginalTranslationRadioButton.Name = "SearchMethodOriginalTranslationRadioButton";
            this.SearchMethodOriginalTranslationRadioButton.Size = new System.Drawing.Size(174, 17);
            this.SearchMethodOriginalTranslationRadioButton.TabIndex = 10;
            this.SearchMethodOriginalTranslationRadioButton.TabStop = true;
            this.SearchMethodOriginalTranslationRadioButton.Text = "Find and Replace in Translation";
            this.SearchMethodOriginalTranslationRadioButton.UseVisualStyleBackColor = true;
            this.SearchMethodOriginalTranslationRadioButton.Click += new System.EventHandler(this.SearchMethodOriginalTranslationRadioButton_Click);
            // 
            // THSearchMatchCaseCheckBox
            // 
            this.THSearchMatchCaseCheckBox.AutoSize = true;
            this.THSearchMatchCaseCheckBox.Location = new System.Drawing.Point(135, 69);
            this.THSearchMatchCaseCheckBox.Name = "THSearchMatchCaseCheckBox";
            this.THSearchMatchCaseCheckBox.Size = new System.Drawing.Size(82, 17);
            this.THSearchMatchCaseCheckBox.TabIndex = 19;
            this.THSearchMatchCaseCheckBox.Text = "Match Case";
            this.THSearchMatchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.FindAllButton);
            this.panel4.Controls.Add(this.SearchFormFindNextButton);
            this.panel4.Controls.Add(this.SearchFormReplaceButton);
            this.panel4.Controls.Add(this.SearchFormReplaceAllButton);
            this.panel4.Location = new System.Drawing.Point(438, 13);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(108, 113);
            this.panel4.TabIndex = 16;
            // 
            // FindAllButton
            // 
            this.FindAllButton.Location = new System.Drawing.Point(0, 58);
            this.FindAllButton.Name = "FindAllButton";
            this.FindAllButton.Size = new System.Drawing.Size(108, 23);
            this.FindAllButton.TabIndex = 7;
            this.FindAllButton.Text = "Find All";
            this.FindAllButton.UseVisualStyleBackColor = true;
            this.FindAllButton.Click += new System.EventHandler(this.FindAllButton_Click);
            // 
            // SearchFormFindNextButton
            // 
            this.SearchFormFindNextButton.Location = new System.Drawing.Point(0, 0);
            this.SearchFormFindNextButton.Name = "SearchFormFindNextButton";
            this.SearchFormFindNextButton.Size = new System.Drawing.Size(108, 23);
            this.SearchFormFindNextButton.TabIndex = 4;
            this.SearchFormFindNextButton.Text = "Find Next";
            this.SearchFormFindNextButton.UseVisualStyleBackColor = true;
            this.SearchFormFindNextButton.Click += new System.EventHandler(this.SearchFormFindNextButton_Click);
            // 
            // SearchFormReplaceButton
            // 
            this.SearchFormReplaceButton.Location = new System.Drawing.Point(0, 29);
            this.SearchFormReplaceButton.Name = "SearchFormReplaceButton";
            this.SearchFormReplaceButton.Size = new System.Drawing.Size(108, 23);
            this.SearchFormReplaceButton.TabIndex = 5;
            this.SearchFormReplaceButton.Text = "Replace";
            this.SearchFormReplaceButton.UseVisualStyleBackColor = true;
            // 
            // SearchFormReplaceAllButton
            // 
            this.SearchFormReplaceAllButton.Location = new System.Drawing.Point(0, 87);
            this.SearchFormReplaceAllButton.Name = "SearchFormReplaceAllButton";
            this.SearchFormReplaceAllButton.Size = new System.Drawing.Size(108, 26);
            this.SearchFormReplaceAllButton.TabIndex = 6;
            this.SearchFormReplaceAllButton.Text = "Replace All";
            this.SearchFormReplaceAllButton.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.THSearchFindWhatLabel);
            this.panel3.Controls.Add(this.SearchFormFindWhatComboBox);
            this.panel3.Controls.Add(this.SearchFormReplaceWithComboBox);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(54, 13);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(364, 54);
            this.panel3.TabIndex = 15;
            // 
            // THSearchFindWhatLabel
            // 
            this.THSearchFindWhatLabel.AutoSize = true;
            this.THSearchFindWhatLabel.Location = new System.Drawing.Point(17, 5);
            this.THSearchFindWhatLabel.Name = "THSearchFindWhatLabel";
            this.THSearchFindWhatLabel.Size = new System.Drawing.Size(58, 13);
            this.THSearchFindWhatLabel.TabIndex = 1;
            this.THSearchFindWhatLabel.Text = "Find what:";
            // 
            // SearchFormFindWhatComboBox
            // 
            this.SearchFormFindWhatComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.SearchFormFindWhatComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.SearchFormFindWhatComboBox.FormattingEnabled = true;
            this.SearchFormFindWhatComboBox.Location = new System.Drawing.Point(81, 2);
            this.SearchFormFindWhatComboBox.Name = "SearchFormFindWhatComboBox";
            this.SearchFormFindWhatComboBox.Size = new System.Drawing.Size(278, 21);
            this.SearchFormFindWhatComboBox.TabIndex = 0;
            // 
            // SearchFormReplaceWithComboBox
            // 
            this.SearchFormReplaceWithComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.SearchFormReplaceWithComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.SearchFormReplaceWithComboBox.FormattingEnabled = true;
            this.SearchFormReplaceWithComboBox.Location = new System.Drawing.Point(81, 29);
            this.SearchFormReplaceWithComboBox.Name = "SearchFormReplaceWithComboBox";
            this.SearchFormReplaceWithComboBox.Size = new System.Drawing.Size(278, 21);
            this.SearchFormReplaceWithComboBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Replace with:";
            // 
            // THSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 334);
            this.Controls.Add(this.THSearchTabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "THSearch";
            this.Text = "Find and Replace";
            this.Load += new System.EventHandler(this.THSearch_Load);
            this.THSearchTabs.ResumeLayout(false);
            this.THSearch1st.ResumeLayout(false);
            this.THSearch1st.PerformLayout();
            this.THSearchPanel.ResumeLayout(false);
            this.THSearchPanel.PerformLayout();
            this.SearchModeGroupBox.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl THSearchTabs;
        private System.Windows.Forms.TabPage THSearch1st;
        internal System.Windows.Forms.LinkLabel linkLabel2;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel THSearchPanel;
        private System.Windows.Forms.RadioButton SearchModeRegexRadioButton;
        private System.Windows.Forms.RadioButton SearchModeNormalRadioButton;
        private System.Windows.Forms.Button SearchFormReplaceAllButton;
        private System.Windows.Forms.Button SearchFormReplaceButton;
        private System.Windows.Forms.Button SearchFormFindNextButton;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox SearchFormReplaceWithComboBox;
        private System.Windows.Forms.Label THSearchFindWhatLabel;
        public System.Windows.Forms.ComboBox SearchFormFindWhatComboBox;
        private System.Windows.Forms.RadioButton SearchMethodOriginalTranslationRadioButton;
        private System.Windows.Forms.RadioButton SearchMethodTranslationRadioButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton SearchRangeTableRadioButton;
        private System.Windows.Forms.RadioButton SearchRangeAllRadioButton;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button FindAllButton;
        private System.Windows.Forms.CheckBox THSearchMatchCaseCheckBox;
        private System.Windows.Forms.GroupBox SearchModeGroupBox;
    }
}
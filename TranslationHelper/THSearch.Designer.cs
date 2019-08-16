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
            this.THSearchPanel = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.THSearchFindWhatLabel = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.THSearchTabs.SuspendLayout();
            this.THSearch1st.SuspendLayout();
            this.THSearchPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // THSearchTabs
            // 
            this.THSearchTabs.Controls.Add(this.THSearch1st);
            this.THSearchTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSearchTabs.Location = new System.Drawing.Point(0, 0);
            this.THSearchTabs.Name = "THSearchTabs";
            this.THSearchTabs.SelectedIndex = 0;
            this.THSearchTabs.Size = new System.Drawing.Size(579, 325);
            this.THSearchTabs.TabIndex = 2;
            // 
            // THSearch1st
            // 
            this.THSearch1st.Controls.Add(this.linkLabel2);
            this.THSearch1st.Controls.Add(this.label3);
            this.THSearch1st.Controls.Add(this.THSearchPanel);
            this.THSearch1st.Location = new System.Drawing.Point(4, 22);
            this.THSearch1st.Name = "THSearch1st";
            this.THSearch1st.Padding = new System.Windows.Forms.Padding(3);
            this.THSearch1st.Size = new System.Drawing.Size(571, 299);
            this.THSearch1st.TabIndex = 0;
            this.THSearch1st.Text = "Find and Replace";
            this.THSearch1st.UseVisualStyleBackColor = true;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel2.Location = new System.Drawing.Point(331, 366);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(193, 13);
            this.linkLabel2.TabIndex = 13;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://patreon.com/TranslationHelper";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(194, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Translation  Helper support:";
            // 
            // THSearchPanel
            // 
            this.THSearchPanel.Controls.Add(this.radioButton2);
            this.THSearchPanel.Controls.Add(this.radioButton1);
            this.THSearchPanel.Controls.Add(this.button3);
            this.THSearchPanel.Controls.Add(this.button2);
            this.THSearchPanel.Controls.Add(this.button1);
            this.THSearchPanel.Controls.Add(this.label1);
            this.THSearchPanel.Controls.Add(this.comboBox2);
            this.THSearchPanel.Controls.Add(this.THSearchFindWhatLabel);
            this.THSearchPanel.Controls.Add(this.comboBox1);
            this.THSearchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSearchPanel.Location = new System.Drawing.Point(3, 3);
            this.THSearchPanel.Name = "THSearchPanel";
            this.THSearchPanel.Size = new System.Drawing.Size(565, 293);
            this.THSearchPanel.TabIndex = 7;
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(109, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(278, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // THSearchFindWhatLabel
            // 
            this.THSearchFindWhatLabel.AutoSize = true;
            this.THSearchFindWhatLabel.Location = new System.Drawing.Point(45, 18);
            this.THSearchFindWhatLabel.Name = "THSearchFindWhatLabel";
            this.THSearchFindWhatLabel.Size = new System.Drawing.Size(58, 13);
            this.THSearchFindWhatLabel.TabIndex = 1;
            this.THSearchFindWhatLabel.Text = "Find what:";
            // 
            // comboBox2
            // 
            this.comboBox2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(109, 42);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(278, 21);
            this.comboBox2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Replace with:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(424, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Find Next";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(424, 42);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Replace";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(424, 71);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Replace All";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(34, 248);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(58, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.Text = "Normal";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Click += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(34, 271);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(56, 17);
            this.radioButton2.TabIndex = 8;
            this.radioButton2.Text = "Regex";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Click += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // THSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 325);
            this.Controls.Add(this.THSearchTabs);
            this.Name = "THSearch";
            this.Text = "Find and Replace";
            this.THSearchTabs.ResumeLayout(false);
            this.THSearch1st.ResumeLayout(false);
            this.THSearch1st.PerformLayout();
            this.THSearchPanel.ResumeLayout(false);
            this.THSearchPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl THSearchTabs;
        private System.Windows.Forms.TabPage THSearch1st;
        internal System.Windows.Forms.LinkLabel linkLabel2;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel THSearchPanel;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label THSearchFindWhatLabel;
        public System.Windows.Forms.ComboBox comboBox1;
    }
}
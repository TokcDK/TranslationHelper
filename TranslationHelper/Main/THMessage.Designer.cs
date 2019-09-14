namespace TranslationHelper
{
    partial class THMsg
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
            this.button1 = new System.Windows.Forms.Button();
            this.THMessageFormInfoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.THMessageFormInfoLabel = new System.Windows.Forms.Label();
            this.THMessageFormMessageLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Location = new System.Drawing.Point(64, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // THMessageFormInfoLinkLabel
            // 
            this.THMessageFormInfoLinkLabel.AutoSize = true;
            this.THMessageFormInfoLinkLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.THMessageFormInfoLinkLabel.Location = new System.Drawing.Point(29, 122);
            this.THMessageFormInfoLinkLabel.Name = "THMessageFormInfoLinkLabel";
            this.THMessageFormInfoLinkLabel.Size = new System.Drawing.Size(193, 13);
            this.THMessageFormInfoLinkLabel.TabIndex = 1;
            this.THMessageFormInfoLinkLabel.TabStop = true;
            this.THMessageFormInfoLinkLabel.Text = "https://patreon.com/TranslationHelper";
            this.THMessageFormInfoLinkLabel.Visible = false;
            this.THMessageFormInfoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.THMessageFormInfoLinkLabel_LinkClicked);
            // 
            // THMessageFormInfoLabel
            // 
            this.THMessageFormInfoLabel.AutoSize = true;
            this.THMessageFormInfoLabel.Location = new System.Drawing.Point(61, 109);
            this.THMessageFormInfoLabel.Name = "THMessageFormInfoLabel";
            this.THMessageFormInfoLabel.Size = new System.Drawing.Size(128, 13);
            this.THMessageFormInfoLabel.TabIndex = 2;
            this.THMessageFormInfoLabel.Text = "Translation  Helper 2019:";
            this.THMessageFormInfoLabel.Visible = false;
            // 
            // THMessageFormMessageLabel
            // 
            this.THMessageFormMessageLabel.AutoEllipsis = true;
            this.THMessageFormMessageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THMessageFormMessageLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.THMessageFormMessageLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.THMessageFormMessageLabel.Location = new System.Drawing.Point(0, 0);
            this.THMessageFormMessageLabel.Name = "THMessageFormMessageLabel";
            this.THMessageFormMessageLabel.Size = new System.Drawing.Size(248, 70);
            this.THMessageFormMessageLabel.TabIndex = 3;
            this.THMessageFormMessageLabel.Text = "Message!";
            this.THMessageFormMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.THMessageFormMessageLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 70);
            this.panel1.TabIndex = 4;
            // 
            // THMsg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 144);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.THMessageFormInfoLabel);
            this.Controls.Add(this.THMessageFormInfoLinkLabel);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "THMsg";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Translation Helper";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.LinkLabel THMessageFormInfoLinkLabel;
        internal System.Windows.Forms.Label THMessageFormInfoLabel;
        public System.Windows.Forms.Label THMessageFormMessageLabel;
    }
}
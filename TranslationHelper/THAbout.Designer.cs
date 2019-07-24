namespace TranslationHelper
{
    partial class THAboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(THAboutForm));
            this.THAboutTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // THAboutTextBox
            // 
            this.THAboutTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.THAboutTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.THAboutTextBox.Location = new System.Drawing.Point(13, 13);
            this.THAboutTextBox.Multiline = true;
            this.THAboutTextBox.Name = "THAboutTextBox";
            this.THAboutTextBox.ReadOnly = true;
            this.THAboutTextBox.Size = new System.Drawing.Size(298, 223);
            this.THAboutTextBox.TabIndex = 0;
            this.THAboutTextBox.Text = resources.GetString("THAboutTextBox.Text");
            // 
            // THAboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 246);
            this.Controls.Add(this.THAboutTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "THAboutForm";
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox THAboutTextBox;
    }
}
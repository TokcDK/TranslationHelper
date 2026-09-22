namespace TranslationHelper.Workspace
{
    partial class OpenedFilesTabControl
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
            this.OpenedFilesTabs = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // OpenedFilesTabs
            // 
            this.OpenedFilesTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenedFilesTabs.Location = new System.Drawing.Point(0, 0);
            this.OpenedFilesTabs.Margin = new System.Windows.Forms.Padding(0);
            this.OpenedFilesTabs.Name = "OpenedFilesTabs";
            this.OpenedFilesTabs.SelectedIndex = 0;
            this.OpenedFilesTabs.Size = new System.Drawing.Size(650, 400);
            this.OpenedFilesTabs.TabIndex = 0;
            // 
            // OpenedFilesTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OpenedFilesTabs);
            this.Name = "OpenedFilesTabControl";
            this.Size = new System.Drawing.Size(650, 400);
            this.ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.TabControl OpenedFilesTabs;
    }
}

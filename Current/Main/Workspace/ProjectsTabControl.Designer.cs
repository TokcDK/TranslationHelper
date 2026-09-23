namespace TranslationHelper.Workspace
{
    partial class ProjectsTabControl
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
            this.ProjectsTabs = new ClosableTabControl();
            this.SuspendLayout();
            // 
            // ProjectsTabs
            // 
            this.ProjectsTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectsTabs.Location = new System.Drawing.Point(0, 0);
            this.ProjectsTabs.Margin = new System.Windows.Forms.Padding(0);
            this.ProjectsTabs.Name = "ProjectsTabs";
            this.ProjectsTabs.SelectedIndex = 0;
            this.ProjectsTabs.Size = new System.Drawing.Size(790, 425);
            this.ProjectsTabs.TabIndex = 0;
            // 
            // ProjectsTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProjectsTabs);
            this.Name = "ProjectsTabControl";
            this.Size = new System.Drawing.Size(790, 425);
            this.ResumeLayout(false);
        }

        #endregion

        internal ClosableTabControl ProjectsTabs;
    }
}

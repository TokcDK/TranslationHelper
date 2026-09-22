namespace TranslationHelper.Workspace
{
    partial class ProjectFilesWorkspacePanel
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
            this.FilesAndOpenedFilesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.FilesListPanel = new System.Windows.Forms.Panel();
            this.FilesList = new System.Windows.Forms.ListBox();
            this.OpenedFileWorkspace = new TranslationHelper.Workspace.OpenedFileWorkspace();
            this.tlpWorkInfo = new System.Windows.Forms.TableLayoutPanel();
            this.TableCompleteInfoLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FilesAndOpenedFilesSplitContainer)).BeginInit();
            this.FilesAndOpenedFilesSplitContainer.Panel1.SuspendLayout();
            this.FilesAndOpenedFilesSplitContainer.Panel2.SuspendLayout();
            this.FilesAndOpenedFilesSplitContainer.SuspendLayout();
            this.FilesListPanel.SuspendLayout();
            this.tlpWorkInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilesAndOpenedFilesSplitContainer
            // 
            this.FilesAndOpenedFilesSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FilesAndOpenedFilesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilesAndOpenedFilesSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.FilesAndOpenedFilesSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.FilesAndOpenedFilesSplitContainer.Name = "FilesAndOpenedFilesSplitContainer";
            // 
            // FilesAndOpenedFilesSplitContainer.Panel1
            // 
            this.FilesAndOpenedFilesSplitContainer.Panel1.Controls.Add(this.FilesListPanel);
            // 
            // FilesAndOpenedFilesSplitContainer.Panel2
            // 
            this.FilesAndOpenedFilesSplitContainer.Panel2.Controls.Add(this.OpenedFileWorkspace);
            this.FilesAndOpenedFilesSplitContainer.Size = new System.Drawing.Size(790, 405);
            this.FilesAndOpenedFilesSplitContainer.SplitterDistance = 127;
            this.FilesAndOpenedFilesSplitContainer.TabIndex = 3;
            // 
            // FilesListPanel
            // 
            this.FilesListPanel.Controls.Add(this.FilesList);
            this.FilesListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilesListPanel.Location = new System.Drawing.Point(0, 0);
            this.FilesListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FilesListPanel.Name = "FilesListPanel";
            this.FilesListPanel.Size = new System.Drawing.Size(125, 403);
            this.FilesListPanel.TabIndex = 1;
            // 
            // FilesList
            // 
            this.FilesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FilesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilesList.FormattingEnabled = true;
            this.FilesList.Location = new System.Drawing.Point(0, 0);
            this.FilesList.Margin = new System.Windows.Forms.Padding(0);
            this.FilesList.Name = "FilesList";
            this.FilesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.FilesList.Size = new System.Drawing.Size(125, 403);
            this.FilesList.TabIndex = 0;
            // 
            // OpenedFileWorkspace
            // 
            this.OpenedFileWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenedFileWorkspace.Location = new System.Drawing.Point(0, 0);
            this.OpenedFileWorkspace.Margin = new System.Windows.Forms.Padding(0);
            this.OpenedFileWorkspace.Name = "OpenedFileWorkspace";
            this.OpenedFileWorkspace.Size = new System.Drawing.Size(657, 403);
            this.OpenedFileWorkspace.TabIndex = 4;
            // 
            // tlpWorkInfo
            // 
            this.tlpWorkInfo.ColumnCount = 1;
            this.tlpWorkInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWorkInfo.Controls.Add(this.TableCompleteInfoLabel, 0, 0);
            this.tlpWorkInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tlpWorkInfo.Location = new System.Drawing.Point(0, 405);
            this.tlpWorkInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tlpWorkInfo.Name = "tlpWorkInfo";
            this.tlpWorkInfo.RowCount = 1;
            this.tlpWorkInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWorkInfo.Size = new System.Drawing.Size(790, 20);
            this.tlpWorkInfo.TabIndex = 6;
            // 
            // TableCompleteInfoLabel
            // 
            this.TableCompleteInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TableCompleteInfoLabel.AutoSize = true;
            this.TableCompleteInfoLabel.Location = new System.Drawing.Point(763, 7);
            this.TableCompleteInfoLabel.Name = "TableCompleteInfoLabel";
            this.TableCompleteInfoLabel.Size = new System.Drawing.Size(24, 13);
            this.TableCompleteInfoLabel.TabIndex = 8;
            this.TableCompleteInfoLabel.Text = "0/0";
            this.TableCompleteInfoLabel.Click += new System.EventHandler(this.TableCompleteInfoLabel_Click);
            // 
            // ProjectFilesWorkspacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FilesAndOpenedFilesSplitContainer);
            this.Controls.Add(this.tlpWorkInfo);
            this.Name = "ProjectFilesWorkspacePanel";
            this.Size = new System.Drawing.Size(790, 425);
            this.FilesAndOpenedFilesSplitContainer.Panel1.ResumeLayout(false);
            this.FilesAndOpenedFilesSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FilesAndOpenedFilesSplitContainer)).EndInit();
            this.FilesAndOpenedFilesSplitContainer.ResumeLayout(false);
            this.FilesListPanel.ResumeLayout(false);
            this.tlpWorkInfo.ResumeLayout(false);
            this.tlpWorkInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer FilesAndOpenedFilesSplitContainer;
        private System.Windows.Forms.Panel FilesListPanel;
        internal System.Windows.Forms.ListBox FilesList;
        internal OpenedFileWorkspace OpenedFileWorkspace;
        private System.Windows.Forms.TableLayoutPanel tlpWorkInfo;
        internal System.Windows.Forms.Label TableCompleteInfoLabel;
    }
}

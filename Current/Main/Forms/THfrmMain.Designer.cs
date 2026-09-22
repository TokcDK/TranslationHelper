
namespace TranslationHelper
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainMenus = new System.Windows.Forms.MenuStrip();
            this.mainMenusHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FilesListMenus = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectedForceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RowMenus = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.frmMainPanel = new System.Windows.Forms.Panel();
            this.mainFormRootWorkspaceLogContainer = new System.Windows.Forms.SplitContainer();
            this.LogTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.OpenCurrentLogFileButton = new System.Windows.Forms.Button();
            this.MainMenus.SuspendLayout();
            this.frmMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainFormRootWorkspaceLogContainer)).BeginInit();
            this.mainFormRootWorkspaceLogContainer.Panel1.SuspendLayout();
            this.mainFormRootWorkspaceLogContainer.Panel2.SuspendLayout();
            this.mainFormRootWorkspaceLogContainer.SuspendLayout();
            this.LogTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenus
            // 
            this.MainMenus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainMenusHereToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.MainMenus.Location = new System.Drawing.Point(0, 0);
            this.MainMenus.Name = "MainMenus";
            this.MainMenus.Size = new System.Drawing.Size(790, 24);
            this.MainMenus.TabIndex = 1;
            this.MainMenus.Text = "MainMenu";
            // 
            // mainMenusHereToolStripMenuItem
            // 
            this.mainMenusHereToolStripMenuItem.Name = "mainMenusHereToolStripMenuItem";
            this.mainMenusHereToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.mainMenusHereToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // FilesListMenus
            // 
            this.FilesListMenus.Name = "contextMenuStrip1";
            this.FilesListMenus.Size = new System.Drawing.Size(61, 4);
            this.FilesListMenus.Text = "2000";
            // 
            // selectedForceToolStripMenuItem
            // 
            this.selectedForceToolStripMenuItem.Name = "selectedForceToolStripMenuItem";
            this.selectedForceToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // RowMenus
            // 
            this.RowMenus.Name = "contextMenuStrip1";
            this.RowMenus.Size = new System.Drawing.Size(61, 4);
            // 
            // frmMainPanel
            // 
            // The workspace of the selected project is added here by the form's constructor: the
            // projects tab control is created by MainWorkspace, which is what binds it to the open
            // projects. An empty panel rather than a designer-hosted control, because the control does
            // not exist until the projects collection it is bound to does.
            this.frmMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMainPanel.Location = new System.Drawing.Point(0, 0);
            this.frmMainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.frmMainPanel.Name = "frmMainPanel";
            this.frmMainPanel.Size = new System.Drawing.Size(790, 425);
            this.frmMainPanel.TabIndex = 16;
            // 
            // mainFormRootWorkspaceLogContainer
            // 
            this.mainFormRootWorkspaceLogContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainFormRootWorkspaceLogContainer.Location = new System.Drawing.Point(0, 24);
            this.mainFormRootWorkspaceLogContainer.Name = "mainFormRootWorkspaceLogContainer";
            this.mainFormRootWorkspaceLogContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainFormRootWorkspaceLogContainer.Panel1
            // 
            this.mainFormRootWorkspaceLogContainer.Panel1.Controls.Add(this.frmMainPanel);
            // 
            // mainFormRootWorkspaceLogContainer.Panel2
            // 
            this.mainFormRootWorkspaceLogContainer.Panel2.Controls.Add(this.LogTableLayoutPanel);
            this.mainFormRootWorkspaceLogContainer.Size = new System.Drawing.Size(790, 485);
            this.mainFormRootWorkspaceLogContainer.SplitterDistance = 425;
            this.mainFormRootWorkspaceLogContainer.TabIndex = 17;
            // 
            // LogTableLayoutPanel
            // 
            this.LogTableLayoutPanel.ColumnCount = 2;
            this.LogTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LogTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.LogTableLayoutPanel.Controls.Add(this.rtbLog, 0, 0);
            this.LogTableLayoutPanel.Controls.Add(this.OpenCurrentLogFileButton, 1, 0);
            this.LogTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.LogTableLayoutPanel.Name = "LogTableLayoutPanel";
            this.LogTableLayoutPanel.RowCount = 1;
            this.LogTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LogTableLayoutPanel.Size = new System.Drawing.Size(790, 56);
            this.LogTableLayoutPanel.TabIndex = 2;
            // 
            // rtbLog
            // 
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Location = new System.Drawing.Point(0, 0);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(0);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(775, 56);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            // 
            // OpenCurrentLogFileButton
            // 
            this.OpenCurrentLogFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenCurrentLogFileButton.Location = new System.Drawing.Point(775, 0);
            this.OpenCurrentLogFileButton.Margin = new System.Windows.Forms.Padding(0);
            this.OpenCurrentLogFileButton.Name = "OpenCurrentLogFileButton";
            this.OpenCurrentLogFileButton.Size = new System.Drawing.Size(15, 56);
            this.OpenCurrentLogFileButton.TabIndex = 2;
            this.OpenCurrentLogFileButton.Text = ">";
            this.OpenCurrentLogFileButton.UseVisualStyleBackColor = true;
            this.OpenCurrentLogFileButton.Click += new System.EventHandler(this.OpenCurrentLogFileButton_Click);
            // 
            // FormMain
            // 
            this.AccessibleDescription = "Program help with translation of some RPG games";
            this.AccessibleName = "Translation Helper";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 509);
            this.Controls.Add(this.mainFormRootWorkspaceLogContainer);
            this.Controls.Add(this.MainMenus);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.MainMenus;
            this.Name = "FormMain";
            this.Text = "Translation Helper by DenisK";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.THMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.THfrmMain_FormClosed);
            this.Load += new System.EventHandler(this.THMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.MainMenus.ResumeLayout(false);
            this.MainMenus.PerformLayout();
            this.frmMainPanel.ResumeLayout(false);
            this.mainFormRootWorkspaceLogContainer.Panel1.ResumeLayout(false);
            this.mainFormRootWorkspaceLogContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainFormRootWorkspaceLogContainer)).EndInit();
            this.mainFormRootWorkspaceLogContainer.ResumeLayout(false);
            this.LogTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedForceToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip FilesListMenus;
        internal System.Windows.Forms.MenuStrip MainMenus;
        internal System.Windows.Forms.ContextMenuStrip RowMenus;
        private System.Windows.Forms.ToolStripMenuItem mainMenusHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        internal System.Windows.Forms.Panel frmMainPanel;
        private System.Windows.Forms.SplitContainer mainFormRootWorkspaceLogContainer;
        public System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.TableLayoutPanel LogTableLayoutPanel;
        private System.Windows.Forms.Button OpenCurrentLogFileButton;
    }
}

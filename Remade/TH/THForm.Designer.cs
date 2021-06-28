
namespace TH
{
    partial class UICoreMainForm
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
            this.UICoreMainPanel = new System.Windows.Forms.Panel();
            this.UICoreMainMenuWorkspaceTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.MenusPanel = new System.Windows.Forms.Panel();
            this.MenusCoreMenuStrip = new System.Windows.Forms.MenuStrip();
            this.WorkspaceCorePanel = new System.Windows.Forms.Panel();
            this.UICoreMainPanel.SuspendLayout();
            this.UICoreMainMenuWorkspaceTableLayoutPanel.SuspendLayout();
            this.MenusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // UICoreMainPanel
            // 
            this.UICoreMainPanel.Controls.Add(this.UICoreMainMenuWorkspaceTableLayoutPanel);
            this.UICoreMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UICoreMainPanel.Location = new System.Drawing.Point(0, 0);
            this.UICoreMainPanel.Name = "UICoreMainPanel";
            this.UICoreMainPanel.Size = new System.Drawing.Size(790, 509);
            this.UICoreMainPanel.TabIndex = 0;
            // 
            // UICoreMainMenuWorkspaceTableLayoutPanel
            // 
            this.UICoreMainMenuWorkspaceTableLayoutPanel.ColumnCount = 1;
            this.UICoreMainMenuWorkspaceTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.UICoreMainMenuWorkspaceTableLayoutPanel.Controls.Add(this.MenusPanel, 0, 0);
            this.UICoreMainMenuWorkspaceTableLayoutPanel.Controls.Add(this.WorkspaceCorePanel, 0, 1);
            this.UICoreMainMenuWorkspaceTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UICoreMainMenuWorkspaceTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.UICoreMainMenuWorkspaceTableLayoutPanel.Name = "UICoreMainMenuWorkspaceTableLayoutPanel";
            this.UICoreMainMenuWorkspaceTableLayoutPanel.RowCount = 2;
            this.UICoreMainMenuWorkspaceTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.UICoreMainMenuWorkspaceTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.UICoreMainMenuWorkspaceTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.UICoreMainMenuWorkspaceTableLayoutPanel.Size = new System.Drawing.Size(790, 509);
            this.UICoreMainMenuWorkspaceTableLayoutPanel.TabIndex = 0;
            // 
            // MenusPanel
            // 
            this.MenusPanel.Controls.Add(this.MenusCoreMenuStrip);
            this.MenusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenusPanel.Location = new System.Drawing.Point(1, 1);
            this.MenusPanel.Margin = new System.Windows.Forms.Padding(1);
            this.MenusPanel.Name = "MenusPanel";
            this.MenusPanel.Size = new System.Drawing.Size(788, 23);
            this.MenusPanel.TabIndex = 1;
            // 
            // MenusCoreMenuStrip
            // 
            this.MenusCoreMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenusCoreMenuStrip.Name = "MenusCoreMenuStrip";
            this.MenusCoreMenuStrip.Size = new System.Drawing.Size(788, 24);
            this.MenusCoreMenuStrip.TabIndex = 0;
            this.MenusCoreMenuStrip.Text = "MenusCore";
            // 
            // WorkspaceCorePanel
            // 
            this.WorkspaceCorePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorkspaceCorePanel.Location = new System.Drawing.Point(1, 25);
            this.WorkspaceCorePanel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.WorkspaceCorePanel.Name = "WorkspaceCorePanel";
            this.WorkspaceCorePanel.Size = new System.Drawing.Size(788, 483);
            this.WorkspaceCorePanel.TabIndex = 2;
            // 
            // UICoreMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 509);
            this.Controls.Add(this.UICoreMainPanel);
            this.MainMenuStrip = this.MenusCoreMenuStrip;
            this.Name = "UICoreMainForm";
            this.Text = "Translation Helper Remade";
            this.UICoreMainPanel.ResumeLayout(false);
            this.UICoreMainMenuWorkspaceTableLayoutPanel.ResumeLayout(false);
            this.MenusPanel.ResumeLayout(false);
            this.MenusPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel UICoreMainPanel;
        private System.Windows.Forms.TableLayoutPanel UICoreMainMenuWorkspaceTableLayoutPanel;
        private System.Windows.Forms.Panel MenusPanel;
        private System.Windows.Forms.Panel WorkspaceCorePanel;
        private System.Windows.Forms.MenuStrip MenusCoreMenuStrip;
    }
}


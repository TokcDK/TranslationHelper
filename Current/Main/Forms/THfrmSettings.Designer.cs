namespace TranslationHelper
{
    partial class THfrmSettings
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
            this.components = new System.ComponentModel.Container();
            this.THSettingsTabControl = new System.Windows.Forms.TabControl();
            this.THSettingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // THSettingsTabControl
            // 
            // Only the tab strip is designed. The pages and their rows are built from
            // SettingsRegistry when the window is constructed, so a setting is declared in exactly
            // one place and the designer cannot drift out of step with it.
            // 
            this.THSettingsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.THSettingsTabControl.Location = new System.Drawing.Point(0, 0);
            this.THSettingsTabControl.Name = "THSettingsTabControl";
            this.THSettingsTabControl.SelectedIndex = 0;
            this.THSettingsTabControl.Size = new System.Drawing.Size(744, 441);
            this.THSettingsTabControl.TabIndex = 0;
            // 
            // THSettingsToolTip
            // 
            this.THSettingsToolTip.AutoPopDelay = 32000;
            this.THSettingsToolTip.InitialDelay = 1000;
            this.THSettingsToolTip.ReshowDelay = 500;
            this.THSettingsToolTip.ShowAlways = true;
            this.THSettingsToolTip.UseAnimation = true;
            this.THSettingsToolTip.UseFading = true;
            // 
            // THfrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 441);
            this.Controls.Add(this.THSettingsTabControl);
            this.MinimumSize = new System.Drawing.Size(560, 360);
            this.Name = "THfrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl THSettingsTabControl;
        private System.Windows.Forms.ToolTip THSettingsToolTip;
    }
}

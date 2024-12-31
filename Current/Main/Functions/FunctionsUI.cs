using System;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    internal class FunctionsUI
    {
        internal static void ShowNonEmptyRowsCount(System.Windows.Forms.Label tableCompleteInfoLabel)
        {
            int RowsCount = FunctionsTable.GetDatasetRowsCount(AppData.CurrentProject.FilesContent);
            if (RowsCount == 0)
            {
                tableCompleteInfoLabel.Visible = false;
            }
            else
            {
                tableCompleteInfoLabel.Visible = true;
                tableCompleteInfoLabel.Text = FunctionsTable.GetDatasetNonEmptyRowsCount(AppData.CurrentProject.FilesContent) + "/" + RowsCount;
            }
        }

        internal static bool ControlsSwitchIsOn = true;
        internal static bool ControlsSwitchActivated;
        internal static void ControlsSwitch(bool switchon = false)
        {
            if (ControlsSwitchActivated)
            {
                if (switchon && !ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                    //System.Media.SystemSounds.Asterisk.Play();
                    //CutToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.X;
                    //CopyCellValuesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
                    //PasteCellValuesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
                }
                else if (ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                    //System.Media.SystemSounds.Hand.Play();
                    //CutToolStripMenuItem1.ShortcutKeys = Keys.None;
                    //CopyCellValuesToolStripMenuItem.ShortcutKeys = Keys.None;
                    //PasteCellValuesToolStripMenuItem.ShortcutKeys = Keys.None;
                }
            }
        }
    }
}
using System;
using System.Runtime;
using TranslationHelper.Functions;

namespace TranslationHelper.Data
{
    class CleanupData
    {
        internal static void THCleanupThings()
        {
            try
            {
                FunctionsSave.WriteRPGMakerMVStats();

                //Close other forms
                if (ProjectData.Main.search != null)
                {
                    if (!ProjectData.Main.search.IsDisposed)
                    {
                        ProjectData.Main.search.Close();
                        ProjectData.Main.search.Dispose();
                    }
                    ProjectData.Main.search = null;
                }

                //ProjectData.FilesListControl.Dispose(); // dispose for control elements

                //if (ProjectData.Main.Settings != null)
                //{
                //    if (!ProjectData.Main.Settings.IsDisposed)
                //    {
                //        ProjectData.Main.Settings.Close();
                //        ProjectData.Main.Settings.Dispose();
                //    }
                //    ProjectData.Main.Settings = null;
                //}

                //to prevent some autooperations while project will not be opened
                Properties.Settings.Default.ProjectIsOpened = false;

                //Reset vars
                ProjectData.Main.Text = "Translation Helper by Dev";
                ProjectData.Main.THInfoTextBox.Text = string.Empty;
                ProjectData.Main.THSourceRichTextBox.Text = string.Empty;
                ProjectData.Main.THTargetRichTextBox.Text = string.Empty;
                ProjectData.Main.TableCompleteInfoLabel.Text = string.Empty;
                ProjectData.Main.TranslationLongestLineLenghtLabel.Text = string.Empty;
                ProjectData.Main.ControlsSwitchActivated = false;

                //Clean data
                ProjectData.Main.THFilesList.Items.Clear();
                ProjectData.THFilesElementsDataset.Reset();
                ProjectData.THFilesElementsDatasetInfo.Reset();
                ProjectData.THFilesElementsALLDataTable.Reset();
                ProjectData.Main.THFileElementsDataGridView.Columns.Clear();
                ProjectData.hashes.Clear();
                ProjectData.AllDBmerged = null;
                //THFileElementsDataGridView.Rows.Clear();
                ProjectData.FilePath = string.Empty;
                ProjectData.SelectedFilePath = string.Empty;

                //Reload regex rules
                FormMain.ReloadTranslationRegexRules();
                FormMain.ReloadCellFixesRegexRules();

                //Dispose objects
                //ProjectData.THFilesElementsDataset.Dispose();
                //ProjectData.THFilesElementsDatasetInfo.Dispose();
                //ProjectData.THFilesElementsALLDataTable.Dispose();

                //Hide some items 
                ProjectData.Main.tlpTextLenPosInfo.Visible = false;
                ProjectData.Main.TableCompleteInfoLabel.Visible = false;
                ProjectData.Main.THWorkSpaceSplitContainer.Visible = false;

                //Disable items
                ProjectData.Main.saveToolStripMenuItem.Enabled = false;
                ProjectData.Main.saveAsToolStripMenuItem.Enabled = false;
                ProjectData.Main.editToolStripMenuItem.Enabled = false;
                ProjectData.Main.viewToolStripMenuItem.Enabled = false;
                ProjectData.Main.saveTranslationToolStripMenuItem.Enabled = false;
                ProjectData.Main.writeTranslationInGameToolStripMenuItem.Enabled = false;
                ProjectData.Main.loadTranslationToolStripMenuItem.Enabled = false;
                ProjectData.Main.loadTrasnlationAsToolStripMenuItem.Enabled = false;
                ProjectData.Main.loadTrasnlationAsForcedToolStripMenuItem.Enabled = false;
                ProjectData.Main.saveTranslationAsToolStripMenuItem.Enabled = false;
                ProjectData.Main.savemenusNOTenabled = true;
                ProjectData.Main.THSourceRichTextBox.Enabled = false;
                ProjectData.Main.THTargetRichTextBox.Enabled = false;
                ProjectData.Main.openInWebToolStripMenuItem.Enabled = false;
                ProjectData.Main.selectedToolStripMenuItem1.Enabled = false;
                ProjectData.Main.tableToolStripMenuItem1.Enabled = false;
                ProjectData.Main.fixCellsSelectedToolStripMenuItem.Enabled = false;
                ProjectData.Main.fixCellsTableToolStripMenuItem.Enabled = false;
                ProjectData.Main.setOriginalValueToTranslationToolStripMenuItem.Enabled = false;
                ProjectData.Main.completeRomajiotherLinesToolStripMenuItem.Enabled = false;
                ProjectData.Main.completeRomajiotherLinesToolStripMenuItem1.Enabled = false;
                ProjectData.Main.forceSameForSimularToolStripMenuItem.Enabled = false;
                ProjectData.Main.forceSameForSimularToolStripMenuItem1.Enabled = false;
                ProjectData.Main.cutToolStripMenuItem1.Enabled = false;
                ProjectData.Main.copyCellValuesToolStripMenuItem.Enabled = false;
                ProjectData.Main.pasteCellValuesToolStripMenuItem.Enabled = false;
                ProjectData.Main.ClearSelectedCellsToolStripMenuItem.Enabled = false;
                ProjectData.Main.toUPPERCASEToolStripMenuItem.Enabled = false;
                ProjectData.Main.firstCharacterToUppercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.toLOWERCASEToolStripMenuItem.Enabled = false;
                ProjectData.Main.setColumnSortingToolStripMenuItem.Enabled = false;
                ProjectData.Main.OpenInWebContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.TranslateSelectedContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.TranslateTableContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.fixSymbolsContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.fixSymbolsTableContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.OriginalToTransalationContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.CutToolStripMenuItem.Enabled = false;
                ProjectData.Main.CopyToolStripMenuItem.Enabled = false;
                ProjectData.Main.pasteToolStripMenuItem.Enabled = false;
                ProjectData.Main.CleanSelectedCellsToolStripMenuItem1.Enabled = false;
                ProjectData.Main.toolStripMenuItem14.Enabled = false;
                ProjectData.Main.uppercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.lowercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.exportToolStripMenuItem1.Enabled = false;
                ProjectData.Main.openProjectsDirToolStripMenuItem.Enabled = false;
                ProjectData.Main.openTranslationRulesFileToolStripMenuItem.Enabled = false;
                ProjectData.Main.openCellFixesFileToolStripMenuItem.Enabled = false;
                ProjectData.Main.reloadRulesToolStripMenuItem.Enabled = false;

                ProjectData.Main.runTestGameToolStripMenuItem.Enabled = false;

                ProjectData.Main.TargetTextBoxLinePositionLabelData.Text = string.Empty;
                ProjectData.Main.TargetTextBoxColumnPositionLabelData.Text = string.Empty;

                //remove project category
                var CategoryName = T._("Project");
                foreach(var menu in ProjectData.Main.CMSFilesList.Items)
                {
                    if(menu is System.Windows.Forms.ToolStripMenuItem t && t.Text== CategoryName)
                    {
                        ProjectData.Main.CMSFilesList.Items.Remove(t);
                        break;
                    }
                }

                //reset vars

                //memory cleaning thing.
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            catch
            {

            }
        }
    }
}

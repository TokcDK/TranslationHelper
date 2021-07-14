using System;
using System.Runtime;

namespace TranslationHelper.Data
{
    class CleanupData
    {
        readonly ProjectData projectData;
        public CleanupData(ProjectData projectData)
        {
            this.projectData = projectData;
        }

        internal void THCleanupThings()
        {
            try
            {
                //Close other forms
                if (projectData.Main.search != null)
                {
                    if (!projectData.Main.search.IsDisposed)
                    {
                        projectData.Main.search.Close();
                        projectData.Main.search.Dispose();
                    }
                    projectData.Main.search = null;
                }
                //if (projectData.Main.Settings != null)
                //{
                //    if (!projectData.Main.Settings.IsDisposed)
                //    {
                //        projectData.Main.Settings.Close();
                //        projectData.Main.Settings.Dispose();
                //    }
                //    projectData.Main.Settings = null;
                //}

                //to prevent some autooperations while project will not be opened
                Properties.Settings.Default.ProjectIsOpened = false;

                //Reset vars
                projectData.Main.Text = "Translation Helper by Dev";
                projectData.Main.THInfoTextBox.Text = string.Empty;
                projectData.Main.THSourceRichTextBox.Text = string.Empty;
                projectData.Main.THTargetRichTextBox.Text = string.Empty;
                projectData.Main.TableCompleteInfoLabel.Text = string.Empty;
                projectData.Main.TranslationLongestLineLenghtLabel.Text = string.Empty;
                projectData.Main.ControlsSwitchActivated = false;

                //Clean data
                projectData.Main.THFilesList.Items.Clear();
                projectData.THFilesElementsDataset.Reset();
                projectData.THFilesElementsDatasetInfo.Reset();
                projectData.THFilesElementsALLDataTable.Reset();
                projectData.Main.THFileElementsDataGridView.Columns.Clear();
                projectData.hashes.Clear();
                projectData.AllDBmerged = null;
                //THFileElementsDataGridView.Rows.Clear();
                projectData.FilePath = string.Empty;
                projectData.SPath = string.Empty;

                //Reload regex rules
                projectData.Main.ReloadTranslationRegexRules();
                projectData.Main.ReloadCellFixesRegexRules();

                //Dispose objects
                //projectData.THFilesElementsDataset.Dispose();
                //projectData.THFilesElementsDatasetInfo.Dispose();
                //projectData.THFilesElementsALLDataTable.Dispose();

                //Hide some items 
                projectData.Main.tlpTextLenPosInfo.Visible = false;
                projectData.Main.TableCompleteInfoLabel.Visible = false;
                projectData.Main.THWorkSpaceSplitContainer.Visible = false;

                //Disable items
                projectData.Main.saveToolStripMenuItem.Enabled = false;
                projectData.Main.saveAsToolStripMenuItem.Enabled = false;
                projectData.Main.editToolStripMenuItem.Enabled = false;
                projectData.Main.viewToolStripMenuItem.Enabled = false;
                projectData.Main.saveTranslationToolStripMenuItem.Enabled = false;
                projectData.Main.writeTranslationInGameToolStripMenuItem.Enabled = false;
                projectData.Main.loadTranslationToolStripMenuItem.Enabled = false;
                projectData.Main.loadTrasnlationAsToolStripMenuItem.Enabled = false;
                projectData.Main.loadTrasnlationAsForcedToolStripMenuItem.Enabled = false;
                projectData.Main.saveTranslationAsToolStripMenuItem.Enabled = false;
                projectData.Main.savemenusNOTenabled = true;
                projectData.Main.THSourceRichTextBox.Enabled = false;
                projectData.Main.THTargetRichTextBox.Enabled = false;
                projectData.Main.openInWebToolStripMenuItem.Enabled = false;
                projectData.Main.selectedToolStripMenuItem1.Enabled = false;
                projectData.Main.tableToolStripMenuItem1.Enabled = false;
                projectData.Main.fixCellsSelectedToolStripMenuItem.Enabled = false;
                projectData.Main.fixCellsTableToolStripMenuItem.Enabled = false;
                projectData.Main.setOriginalValueToTranslationToolStripMenuItem.Enabled = false;
                projectData.Main.completeRomajiotherLinesToolStripMenuItem.Enabled = false;
                projectData.Main.completeRomajiotherLinesToolStripMenuItem1.Enabled = false;
                projectData.Main.forceSameForSimularToolStripMenuItem.Enabled = false;
                projectData.Main.forceSameForSimularToolStripMenuItem1.Enabled = false;
                projectData.Main.cutToolStripMenuItem1.Enabled = false;
                projectData.Main.copyCellValuesToolStripMenuItem.Enabled = false;
                projectData.Main.pasteCellValuesToolStripMenuItem.Enabled = false;
                projectData.Main.ClearSelectedCellsToolStripMenuItem.Enabled = false;
                projectData.Main.toUPPERCASEToolStripMenuItem.Enabled = false;
                projectData.Main.firstCharacterToUppercaseToolStripMenuItem.Enabled = false;
                projectData.Main.toLOWERCASEToolStripMenuItem.Enabled = false;
                projectData.Main.setColumnSortingToolStripMenuItem.Enabled = false;
                projectData.Main.OpenInWebContextToolStripMenuItem.Enabled = false;
                projectData.Main.TranslateSelectedContextToolStripMenuItem.Enabled = false;
                projectData.Main.TranslateTableContextToolStripMenuItem.Enabled = false;
                projectData.Main.fixSymbolsContextToolStripMenuItem.Enabled = false;
                projectData.Main.fixSymbolsTableContextToolStripMenuItem.Enabled = false;
                projectData.Main.OriginalToTransalationContextToolStripMenuItem.Enabled = false;
                projectData.Main.CutToolStripMenuItem.Enabled = false;
                projectData.Main.CopyToolStripMenuItem.Enabled = false;
                projectData.Main.pasteToolStripMenuItem.Enabled = false;
                projectData.Main.CleanSelectedCellsToolStripMenuItem1.Enabled = false;
                projectData.Main.toolStripMenuItem14.Enabled = false;
                projectData.Main.uppercaseToolStripMenuItem.Enabled = false;
                projectData.Main.lowercaseToolStripMenuItem.Enabled = false;
                projectData.Main.exportToolStripMenuItem1.Enabled = false;
                projectData.Main.openProjectsDirToolStripMenuItem.Enabled = false;
                projectData.Main.openTranslationRulesFileToolStripMenuItem.Enabled = false;
                projectData.Main.openCellFixesFileToolStripMenuItem.Enabled = false;
                projectData.Main.reloadRulesToolStripMenuItem.Enabled = false;

                projectData.Main.runTestGameToolStripMenuItem.Enabled = false;

                projectData.Main.TargetTextBoxLinePositionLabelData.Text = string.Empty;
                projectData.Main.TargetTextBoxColumnPositionLabelData.Text = string.Empty;

                //remove project category
                var CategoryName = T._("Project");
                foreach(var menu in projectData.Main.CMSFilesList.Items)
                {
                    if(menu is System.Windows.Forms.ToolStripMenuItem t && t.Text== CategoryName)
                    {
                        projectData.Main.CMSFilesList.Items.Remove(t);
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

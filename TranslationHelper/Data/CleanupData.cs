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
                ProjectData.Main.SaveToolStripMenuItem.Enabled = false;
                ProjectData.Main.SaveAsToolStripMenuItem.Enabled = false;
                ProjectData.Main.EditToolStripMenuItem.Enabled = false;
                ProjectData.Main.ViewToolStripMenuItem.Enabled = false;
                ProjectData.Main.SaveTranslationToolStripMenuItem.Enabled = false;
                ProjectData.Main.WriteTranslationInGameToolStripMenuItem.Enabled = false;
                ProjectData.Main.LoadTranslationToolStripMenuItem.Enabled = false;
                ProjectData.Main.LoadTrasnlationAsToolStripMenuItem.Enabled = false;
                ProjectData.Main.LoadTrasnlationAsForcedToolStripMenuItem.Enabled = false;
                ProjectData.Main.SaveTranslationAsToolStripMenuItem.Enabled = false;
                ProjectData.Main.SavemenusNOTenabled = true;
                ProjectData.Main.THSourceRichTextBox.Enabled = false;
                ProjectData.Main.THTargetRichTextBox.Enabled = false;
                ProjectData.Main.OpenInWebToolStripMenuItem.Enabled = false;
                ProjectData.Main.SelectedToolStripMenuItem1.Enabled = false;
                ProjectData.Main.TableToolStripMenuItem1.Enabled = false;
                ProjectData.Main.FixCellsSelectedToolStripMenuItem.Enabled = false;
                ProjectData.Main.FixCellsTableToolStripMenuItem.Enabled = false;
                ProjectData.Main.SetOriginalValueToTranslationToolStripMenuItem.Enabled = false;
                ProjectData.Main.CompleteRomajiotherLinesToolStripMenuItem.Enabled = false;
                ProjectData.Main.CompleteRomajiotherLinesToolStripMenuItem1.Enabled = false;
                ProjectData.Main.ForceSameForSimularToolStripMenuItem.Enabled = false;
                ProjectData.Main.ForceSameForSimularToolStripMenuItem1.Enabled = false;
                ProjectData.Main.CutToolStripMenuItem1.Enabled = false;
                ProjectData.Main.CopyCellValuesToolStripMenuItem.Enabled = false;
                ProjectData.Main.PasteCellValuesToolStripMenuItem.Enabled = false;
                ProjectData.Main.ClearSelectedCellsToolStripMenuItem.Enabled = false;
                ProjectData.Main.ToUPPERCASEToolStripMenuItem.Enabled = false;
                ProjectData.Main.FirstCharacterToUppercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.ToLowercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.SetColumnSortingToolStripMenuItem.Enabled = false;
                ProjectData.Main.OpenInWebContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.TranslateSelectedContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.TranslateTableContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.FixSymbolsContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.FixSymbolsTableContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.OriginalToTransalationContextToolStripMenuItem.Enabled = false;
                ProjectData.Main.CutToolStripMenuItem.Enabled = false;
                ProjectData.Main.CopyToolStripMenuItem.Enabled = false;
                ProjectData.Main.PasteToolStripMenuItem.Enabled = false;
                ProjectData.Main.CleanSelectedCellsToolStripMenuItem1.Enabled = false;
                ProjectData.Main.ToolStripMenuItem14.Enabled = false;
                ProjectData.Main.UppercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.LowercaseToolStripMenuItem.Enabled = false;
                ProjectData.Main.ExportToolStripMenuItem1.Enabled = false;
                ProjectData.Main.OpenProjectsDirToolStripMenuItem.Enabled = false;
                ProjectData.Main.OpenTranslationRulesFileToolStripMenuItem.Enabled = false;
                ProjectData.Main.OpenCellFixesFileToolStripMenuItem.Enabled = false;
                ProjectData.Main.ReloadRulesToolStripMenuItem.Enabled = false;

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

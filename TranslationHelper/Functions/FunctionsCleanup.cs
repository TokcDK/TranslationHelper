using System;
using System.Runtime;
using TranslationHelper.Functions;

namespace TranslationHelper.Data
{
    class FunctionsCleanup
    {
        internal static void THCleanupThings()
        {
            try
            {
                FunctionsSave.WriteRPGMakerMVStats();

                //Close other forms
                if (AppData.Main.search != null)
                {
                    if (!AppData.Main.search.IsDisposed)
                    {
                        AppData.Main.search.Close();
                        AppData.Main.search.Dispose();
                    }
                    AppData.Main.search = null;
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
                AppSettings.ProjectIsOpened = false;

                //Reset vars
                AppData.Main.Text = "Translation Helper";
                AppData.Main.THInfoTextBox.Text = string.Empty;
                AppData.Main.THSourceRichTextBox.Text = string.Empty;
                AppData.Main.THTargetRichTextBox.Text = string.Empty;
                AppData.Main.TableCompleteInfoLabel.Text = string.Empty;
                AppData.Main.TranslationLongestLineLenghtLabel.Text = string.Empty;
                AppData.Main.ControlsSwitchActivated = false;

                //Clean data
                AppData.Main.THFilesList.Items.Clear();
                //ProjectData.CurrentProject.FilesContent.Reset();
                //ProjectData.CurrentProject.FilesContentInfo.Reset();
                //ProjectData.CurrentProject.FilesContentAll.Reset();
                AppData.Main.THFileElementsDataGridView.Columns.Clear();
                AppData.AllDBmerged = null;
                //THFileElementsDataGridView.Rows.Clear();
                AppData.SelectedFilePath = string.Empty;

                //Reload regex rules
                FormMain.ReloadTranslationRegexRules();
                FormMain.ReloadCellFixesRegexRules();

                //Dispose objects
                //ProjectData.THFilesElementsDataset.Dispose();
                //ProjectData.THFilesElementsDatasetInfo.Dispose();
                //ProjectData.THFilesElementsALLDataTable.Dispose();

                //Hide some items 
                AppData.Main.tlpTextLenPosInfo.Visible = false;
                AppData.Main.TableCompleteInfoLabel.Visible = false;
                AppData.Main.THWorkSpaceSplitContainer.Visible = false;

                //Disable items
                AppData.Main.EditToolStripMenuItem.Enabled = false;
                AppData.Main.ViewToolStripMenuItem.Enabled = false;
                AppData.Main.SaveTranslationToolStripMenuItem.Enabled = false;
                AppData.Main.WriteTranslationInGameToolStripMenuItem.Enabled = false;
                AppData.Main.LoadTranslationToolStripMenuItem.Enabled = false;
                AppData.Main.LoadTrasnlationAsToolStripMenuItem.Enabled = false;
                AppData.Main.LoadTrasnlationAsForcedToolStripMenuItem.Enabled = false;
                AppData.Main.SaveTranslationAsToolStripMenuItem.Enabled = false;
                AppData.Main.SavemenusNOTenabled = true;
                AppData.Main.THSourceRichTextBox.Enabled = false;
                AppData.Main.THTargetRichTextBox.Enabled = false;
                AppData.Main.OpenInWebToolStripMenuItem.Enabled = false;
                AppData.Main.SelectedToolStripMenuItem1.Enabled = false;
                AppData.Main.TableToolStripMenuItem1.Enabled = false;
                AppData.Main.FixCellsSelectedToolStripMenuItem.Enabled = false;
                AppData.Main.FixCellsTableToolStripMenuItem.Enabled = false;
                AppData.Main.SetOriginalValueToTranslationToolStripMenuItem.Enabled = false;
                AppData.Main.CompleteRomajiotherLinesToolStripMenuItem.Enabled = false;
                AppData.Main.CompleteRomajiotherLinesToolStripMenuItem1.Enabled = false;
                AppData.Main.ForceSameForSimularToolStripMenuItem.Enabled = false;
                AppData.Main.ForceSameForSimularToolStripMenuItem1.Enabled = false;
                AppData.Main.CutToolStripMenuItem1.Enabled = false;
                AppData.Main.CopyCellValuesToolStripMenuItem.Enabled = false;
                AppData.Main.PasteCellValuesToolStripMenuItem.Enabled = false;
                AppData.Main.ClearSelectedCellsToolStripMenuItem.Enabled = false;
                AppData.Main.ToUPPERCASEToolStripMenuItem.Enabled = false;
                AppData.Main.FirstCharacterToUppercaseToolStripMenuItem.Enabled = false;
                AppData.Main.ToLowercaseToolStripMenuItem.Enabled = false;
                AppData.Main.SetColumnSortingToolStripMenuItem.Enabled = false;
                AppData.Main.OpenInWebContextToolStripMenuItem.Enabled = false;
                AppData.Main.TranslateSelectedContextToolStripMenuItem.Enabled = false;
                AppData.Main.TranslateTableContextToolStripMenuItem.Enabled = false;
                AppData.Main.FixSymbolsContextToolStripMenuItem.Enabled = false;
                AppData.Main.FixSymbolsTableContextToolStripMenuItem.Enabled = false;
                AppData.Main.OriginalToTransalationContextToolStripMenuItem.Enabled = false;
                AppData.Main.CutToolStripMenuItem.Enabled = false;
                AppData.Main.CopyCMStripMenuItem.Enabled = false;
                AppData.Main.PasteToolStripMenuItem.Enabled = false;
                AppData.Main.CleanSelectedCellsToolStripMenuItem1.Enabled = false;
                AppData.Main.ToolStripMenuItem14.Enabled = false;
                AppData.Main.UppercaseToolStripMenuItem.Enabled = false;
                AppData.Main.LowercaseToolStripMenuItem.Enabled = false;
                AppData.Main.ExportToolStripMenuItem1.Enabled = false;
                AppData.Main.OpenProjectsDirToolStripMenuItem.Enabled = false;
                AppData.Main.OpenTranslationRulesFileToolStripMenuItem.Enabled = false;
                AppData.Main.OpenCellFixesFileToolStripMenuItem.Enabled = false;
                AppData.Main.ReloadRulesToolStripMenuItem.Enabled = false;

                AppData.Main.runTestGameToolStripMenuItem.Enabled = false;

                AppData.Main.TargetTextBoxLinePositionLabelData.Text = string.Empty;
                AppData.Main.TargetTextBoxColumnPositionLabelData.Text = string.Empty;

                //remove project category
                var CategoryName = T._("Project");
                foreach(var menu in AppData.Main.CMSFilesList.Items)
                {
                    if(menu is System.Windows.Forms.ToolStripMenuItem t && t.Text== CategoryName)
                    {
                        AppData.Main.CMSFilesList.Items.Remove(t);
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

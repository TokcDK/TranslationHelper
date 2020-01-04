﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Data
{
    class CleanupData
    {
        THDataWork thDataWork;
        public CleanupData(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal void THCleanupThings()
        {
            try
            {
                //Reset vars
                thDataWork.Main.Text = "Translation Helper by Dev";
                thDataWork.Main.THInfoTextBox.Text = string.Empty;
                thDataWork.Main.THSourceRichTextBox.Text = string.Empty;
                thDataWork.Main.THTargetRichTextBox.Text = string.Empty;
                thDataWork.Main.TableCompleteInfoLabel.Text = string.Empty;
                thDataWork.Main.TranslationLongestLineLenghtLabel.Text = string.Empty;
                thDataWork.Main.ControlsSwitchActivated = false;

                //Clean data
                thDataWork.Main.THFilesList.Items.Clear();
                //thDataWork.THFilesElementsDataset.Reset();
                //thDataWork.THFilesElementsDatasetInfo.Reset();
                //thDataWork.THFilesElementsALLDataTable.Reset();
                thDataWork.Main.THFileElementsDataGridView.Columns.Clear();
                //THFileElementsDataGridView.Rows.Clear();

                //Dispose objects
                //thDataWork.THFilesElementsDataset.Dispose();
                //thDataWork.THFilesElementsDatasetInfo.Dispose();
                //thDataWork.THFilesElementsALLDataTable.Dispose();

                //Disable items
                thDataWork.Main.saveToolStripMenuItem.Enabled = false;
                thDataWork.Main.saveAsToolStripMenuItem.Enabled = false;
                thDataWork.Main.editToolStripMenuItem.Enabled = false;
                thDataWork.Main.viewToolStripMenuItem.Enabled = false;
                thDataWork.Main.saveTranslationToolStripMenuItem.Enabled = false;
                thDataWork.Main.writeTranslationInGameToolStripMenuItem.Enabled = false;
                thDataWork.Main.loadTranslationToolStripMenuItem.Enabled = false;
                thDataWork.Main.loadTrasnlationAsToolStripMenuItem.Enabled = false;
                thDataWork.Main.saveTranslationAsToolStripMenuItem.Enabled = false;
                thDataWork.Main.savemenusNOTenabled = true;
                thDataWork.Main.THSourceRichTextBox.Enabled = false;
                thDataWork.Main.THTargetRichTextBox.Enabled = false;
                thDataWork.Main.openInWebToolStripMenuItem.Enabled = false;
                thDataWork.Main.selectedToolStripMenuItem1.Enabled = false;
                thDataWork.Main.tableToolStripMenuItem1.Enabled = false;
                thDataWork.Main.fixCellsSelectedToolStripMenuItem.Enabled = false;
                thDataWork.Main.fixCellsTableToolStripMenuItem.Enabled = false;
                thDataWork.Main.setOriginalValueToTranslationToolStripMenuItem.Enabled = false;
                thDataWork.Main.completeRomajiotherLinesToolStripMenuItem.Enabled = false;
                thDataWork.Main.completeRomajiotherLinesToolStripMenuItem1.Enabled = false;
                thDataWork.Main.forceSameForSimularToolStripMenuItem.Enabled = false;
                thDataWork.Main.forceSameForSimularToolStripMenuItem1.Enabled = false;
                thDataWork.Main.cutToolStripMenuItem1.Enabled = false;
                thDataWork.Main.copyCellValuesToolStripMenuItem.Enabled = false;
                thDataWork.Main.pasteCellValuesToolStripMenuItem.Enabled = false;
                thDataWork.Main.clearSelectedCellsToolStripMenuItem.Enabled = false;
                thDataWork.Main.toUPPERCASEToolStripMenuItem.Enabled = false;
                thDataWork.Main.firstCharacterToUppercaseToolStripMenuItem.Enabled = false;
                thDataWork.Main.toLOWERCASEToolStripMenuItem.Enabled = false;
                thDataWork.Main.setColumnSortingToolStripMenuItem.Enabled = false;
                thDataWork.Main.OpenInWebContextToolStripMenuItem.Enabled = false;
                thDataWork.Main.TranslateSelectedContextToolStripMenuItem.Enabled = false;
                thDataWork.Main.TranslateTableContextToolStripMenuItem.Enabled = false;
                thDataWork.Main.fixSymbolsContextToolStripMenuItem.Enabled = false;
                thDataWork.Main.fixSymbolsTableContextToolStripMenuItem.Enabled = false;
                thDataWork.Main.OriginalToTransalationContextToolStripMenuItem.Enabled = false;
                thDataWork.Main.CutToolStripMenuItem.Enabled = false;
                thDataWork.Main.CopyToolStripMenuItem.Enabled = false;
                thDataWork.Main.pasteToolStripMenuItem.Enabled = false;
                thDataWork.Main.CleanSelectedCellsToolStripMenuItem1.Enabled = false;
                thDataWork.Main.toolStripMenuItem14.Enabled = false;
                thDataWork.Main.uppercaseToolStripMenuItem.Enabled = false;
                thDataWork.Main.lowercaseToolStripMenuItem.Enabled = false;

                thDataWork.Main.runTestGameToolStripMenuItem.Enabled = false;

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

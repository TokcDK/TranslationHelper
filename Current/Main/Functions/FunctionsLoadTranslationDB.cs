using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Main.Functions;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

namespace TranslationHelper.Functions
{
    class FunctionsLoadTranslationDB
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public FunctionsLoadTranslationDB()
        {

        }

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal static void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string, string> db, bool forced = false)
        {
            var project = AppData.CurrentProject;
            int translationColumnIndex = project.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;

            if (translationColumnIndex <= 0)
            {
                return;
            }

            //The controls of the project being loaded into, taken once: the load runs in parallel and
            //must not pick up another project's grid or list if the user switches tabs while it runs.
            var workTableDatagridview = AppData.ActiveWorkspace?.ActiveFileWorkspace?.ElementsDataGridView;
            var filesList = AppData.ActiveWorkspace?.FilesList;

            Logger.Info(T._("Load DB"));

            Parallel.ForEach(project.FilesContent.Tables.Cast<DataTable>(), (table, state, tableIndex) =>
            {
                if (!forced && FunctionsTable.IsTableColumnCellsAll(table))
                {
                    return;
                }

                bool resetDGV = ResetDGVDataSource(project.FilesListContent.GetListIndex((int)tableIndex), filesList, workTableDatagridview);

                //string tableProgressInfo = string.Format("{0} {1}: {2}>{3}/{4}", T._("Load"), T._(THSettings.TranslationColumnName), table.TableName, tableIndex, AppData.CurrentProject.FilesContent.Tables.Count);
                //Logger.Info(tableProgressInfo);

                bool dbTryToCheckLinesOfEachMultilineValue = AppSettings.DBTryToCheckLinesOfEachMultilineValue;

                foreach (DataRow row in table.Rows)
                {
                    var translation = row.Field<string>(translationColumnIndex);
                    if (!forced && !string.IsNullOrEmpty(translation))
                    {
                        continue;
                    }

                    var originalCellValue = row.Field<string>(0);
                    var isRN = originalCellValue.IndexOf("\r\n") != -1;
                    string translatedValue = null;

                    if (db.TryGetValue(originalCellValue, out translatedValue) || db.TryGetValue(originalCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n"), out translatedValue))
                    {
                        row.SetValue(translationColumnIndex, translatedValue);
                    }
                    else if (dbTryToCheckLinesOfEachMultilineValue && originalCellValue.IsMultiline())
                    {
                        var mergedLines = new List<string>();
                        bool isAllLinesTranslated = true;

                        foreach (var line in originalCellValue.SplitToLines())
                        {
                            if (line.HaveMostOfRomajiOtherChars())
                            {
                                mergedLines.Add(line);
                            }
                            else if (db.TryGetValue(line, out translatedValue))
                            {
                                if (string.IsNullOrEmpty(translatedValue))
                                {
                                    isAllLinesTranslated = false;
                                    break;
                                }

                                mergedLines.Add(translatedValue);
                            }
                            else
                            {
                                isAllLinesTranslated = false;
                                break;
                            }
                        }

                        if (isAllLinesTranslated && mergedLines.Count > 0)
                        {
                            row.SetValue(translationColumnIndex, string.Join(Environment.NewLine, mergedLines));
                        }
                    }
                }

                if (resetDGV)
                {
                    ResetDGVDataSource(null, filesList, workTableDatagridview, false, table);
                }

            });

            Logger.Info(T._("DB loaded!"));

            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forceOverwriteTranslations"></param>
        internal static void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db, bool forceOverwriteTranslations = false)
        {
            var tables = AppData.CurrentProject.FilesContent.Tables;
            var translationColIndex = tables[0].Columns[THSettings.TranslationColumnName].Ordinal;

            if (translationColIndex < 1) return;

            //var progressMessage = $"{T._("Load")}:";
            Logger.Info(T._("Load DB"));

            //The project being loaded into and its controls, taken once: the load runs in parallel and
            //must not pick up another project's grid or list if the user switches tabs while it runs.
            var workspace = AppData.ActiveWorkspace;
            var workTableDatagridview = workspace?.ActiveFileWorkspace?.ElementsDataGridView;
            var filesList = workspace?.FilesList;
            var filesListContent = AppData.CurrentProject.FilesListContent;

            _ = Parallel.ForEach(tables.Cast<DataTable>(), (table, _, tableIndex) =>
            {
                if (!forceOverwriteTranslations && FunctionsTable.IsTableColumnCellsAll(table))
                {
                    return;
                }

                //Only reset the grid for tables which are really processed: the early return above
                //would otherwise leave the grid unbound, because its DataSource stays null.
                var isTableReset = ResetDGVDataSource(filesListContent.GetListIndex((int)tableIndex), filesList, workTableDatagridview);

                //var tableProgressMessage = $"{progressMessage} {table.TableName}>{tableIndex + 1}/{tables.Count}";
                //Logger.Info(tableProgressMessage);

                var rows = table.Rows;
                var rowCount = rows.Count;

                for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    var row = rows[rowIndex];
                    var translationValue = row.Field<string>(translationColIndex);

                    if (!forceOverwriteTranslations && !string.IsNullOrEmpty(translationValue))
                    {
                        continue;
                    }

                    var origCellValue = row.Field<string>(0);
                    var dbFound = db.TryGetValue(origCellValue, out var dbFilesListByOriginal);
                    if (!dbFound)
                    {
                        var isRN = origCellValue.IndexOf("\r\n") != -1;
                        var altOrigCellValue = origCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n");
                        dbFound = db.TryGetValue(altOrigCellValue, out dbFilesListByOriginal);
                    }

                    if (dbFound)
                    {
                        foreach (var fileLinesListByRowIndex in dbFilesListByOriginal.Values)
                        {
                            if (fileLinesListByRowIndex.TryGetValue(rowIndex, out var dbTranslation))
                            {
                                row.SetValue(translationColIndex, dbTranslation);
                                break;
                            }
                            else if (fileLinesListByRowIndex.Values.FirstOrDefault() is string firstTranslation)
                            {
                                row.SetValue(translationColIndex, firstTranslation);
                                break;
                            }
                        }
                    }
                    else if (origCellValue.IsMultiline() && AppSettings.DBTryToCheckLinesOfEachMultilineValue)
                    {
                        var mergedLines = new List<string>();
                        var allLinesTranslated = true;

                        foreach (var line in origCellValue.SplitToLines())
                        {
                            if (line.HaveMostOfRomajiOtherChars())
                            {
                                mergedLines.Add(line);
                            }
                            else if (db.TryGetValue(line, out var tablesList))
                            {
                                if (tablesList.Values.FirstOrDefault()?.Values.FirstOrDefault() is string translation)
                                {
                                    mergedLines.Add(translation);
                                }
                                else
                                {
                                    allLinesTranslated = false;
                                    break;
                                }
                            }
                            else
                            {
                                allLinesTranslated = false;
                                break;
                            }
                        }

                        if (allLinesTranslated && mergedLines.Count > 0)
                        {
                            row.SetValue(translationColIndex, string.Join(Environment.NewLine, mergedLines));
                        }
                    }
                }

                if (isTableReset)
                {
                    ResetDGVDataSource(null, filesList, workTableDatagridview, false, table);
                }

                //Logger.Info(tableProgressMessage);
            });

            Logger.Info(T._("Load DB finished!"));

            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Unbind or rebind <paramref name="dgv"/> when it is presenting the entry at
        /// <paramref name="listIndex"/>.
        /// </summary>
        /// <param name="listIndex">
        /// Index of the entry in the files list, or null to rebind without consulting the selection.
        /// It is an entry index, not a table index: the list starts with the "[ALL]" entry, so the two
        /// differ by one whenever a project has one, and a caller holding a table index resolves it
        /// with <see cref="FilesListControl.FilesListContent.GetListIndex"/>.
        /// </param>
        /// <param name="filesList">The files list of the project being loaded into.</param>
        /// <param name="dgv">The work grid of the entry being shown.</param>
        /// <param name="isReset">True to unbind the grid, false to bind <paramref name="table"/> to it.</param>
        /// <param name="table">The table to bind when <paramref name="isReset"/> is false.</param>
        private static bool ResetDGVDataSource(int? listIndex, FilesListControlBase filesList, DataGridView dgv, bool isReset = true, DataTable table = null)
        {
            bool b = false;

            if (dgv == null) return false;

            if (dgv.InvokeRequired)
            {
                dgv.Invoke(new Action(() =>
                {
                    b = ResetDGVDataSource(listIndex, filesList, dgv, isReset, table);
                }));
            }
            else
            {
                if ((isReset && dgv.DataSource != null || !isReset && dgv.DataSource == null)
                    && (listIndex == null || (filesList != null && filesList.GetSelectedIndex() == listIndex)))
                {
                    dgv.DataSource = isReset ? null : table;
                    dgv.Update();
                    dgv.Refresh();
                    b = true;
                }
            }

            return b;
        }

        internal async static Task LoadTranslationIfNeed(bool forceLoad = false, bool askIfLoadDB = true, bool askIfLoadAllDB = true)
        {
            // The whole step is reported as a load, not just the read: a forced load clears the
            // translations first, and the window in which the project holds cleared translations is
            // exactly the window an automatic operation must stay out of.
            using (ProjectReadiness.BeginDatabaseLoad())
            {
                var dbPath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName() + FunctionsDBFile.GetDBCompressionExt());
                dbPath = FunctionsDBFile.SearchByAllDBFormatExtensions(dbPath);

                if (File.Exists(dbPath) && (!askIfLoadDB || (askIfLoadDB && MessageBox.Show(T._("Found translation DB. Load it?"), T._("Load translation DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)))
                {
                    if (forceLoad) await new ClearCells().AllT().ConfigureAwait(true);

                    await Task.Run(() => FunctionsDBFile.LoadTranslationFromDB(sPath: dbPath, UseAllDB: false, forced: true)).ConfigureAwait(false);
                }
                else if (askIfLoadAllDB)
                {
                    var loadTranslationsFromAllDBQuestion = MessageBox.Show(T._("Try to find translations in all avalaible DB? (Can take some time)"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (loadTranslationsFromAllDBQuestion != DialogResult.Yes) return;

                    await Task.Run(() => FunctionsDBFile.LoadTranslationFromDB(sPath: string.Empty, UseAllDB: true)).ConfigureAwait(false);
                }
            }
        }
    }
}

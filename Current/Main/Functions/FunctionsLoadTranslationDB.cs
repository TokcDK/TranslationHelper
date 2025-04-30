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
using TranslationHelper.Main.Functions;

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
            int translationColumnIndex = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;

            if (translationColumnIndex <= 0)
            {
                return;
            }

            var workTableDatagridview = AppData.Main.THFileElementsDataGridView;
            var filesList = AppData.THFilesList;

            Logger.Info(T._("Load DB"));

            Parallel.ForEach(AppData.CurrentProject.FilesContent.Tables.Cast<DataTable>(), (table, state, tableIndex) =>
            {
                if (!forced && FunctionsTable.IsTableColumnCellsAll(table))
                {
                    return;
                }

                bool resetDGV = ResetDGVDataSource(tableIndex, filesList, workTableDatagridview);

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
                    ResetDGVDataSource(-1, filesList, workTableDatagridview, false, table);
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

            var workTableDatagridview = AppData.Main.THFileElementsDataGridView;
            var filesList = AppData.THFilesList;

            _ = Parallel.ForEach(tables.Cast<DataTable>(), (table, _, tableIndex) =>
            {
                var isTableReset = ResetDGVDataSource(tableIndex, filesList, workTableDatagridview);

                if (!forceOverwriteTranslations && FunctionsTable.IsTableColumnCellsAll(table))
                {
                    return;
                }

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
                    ResetDGVDataSource(-1, filesList, workTableDatagridview, false, table);
                }

                //Logger.Info(tableProgressMessage);
            });

            Logger.Info(T._("Load DB finished!"));

            System.Media.SystemSounds.Beep.Play();
        }

        private static bool ResetDGVDataSource(long tableIndex, ListBox filesList, DataGridView dgv, bool isReset = true, DataTable table = null)
        {
            bool b = false;

            if (dgv.InvokeRequired)
            {
                dgv.Invoke(new Action(() =>
                {
                    b = ResetDGVDataSource(tableIndex, filesList, dgv, isReset, table);
                }));
            }
            else
            {
                if ((isReset && dgv.DataSource != null || !isReset && dgv.DataSource == null) && filesList.SelectedIndex == tableIndex)
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

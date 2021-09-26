using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class RPGMKTRANSPATCH : RPGMTransPatchBase
    {
        public RPGMKTRANSPATCH()
        {
        }

        internal override bool Open()
        {
            FormatBase format = new TranslationHelper.Formats.RPGMTrans.TXT
            {
                FilePath = FilePath
            };
            return format.Open();
        }

        internal override bool Save()
        {
            return WritePatchV3();
        }

        private bool WritePatchV3()
        {
            var ret = false;
            List<string> LinesToWrite = new List<string>
            {
                "> RPGMAKER TRANS PATCH FILE VERSION 3.2"//v3
            };

            var TablesCount = ProjectData.THFilesElementsDataset.Tables.Count;
            for (int t = 0; t < TablesCount; t++)
            {
                try
                {
                    var table = ProjectData.THFilesElementsDataset.Tables[t];
                    var tableRowsCount = table.Rows.Count;
                    for (int r = 0; r < tableRowsCount; r++)
                    {
                        var row = table.Rows[r];

                        var original = row[0] as string;
                        var translation = row[1] as string;

                        List<string> context = new List<string>();
                        var infoRow = ProjectData.THFilesElementsDatasetInfo.Tables[t].Rows[r] + string.Empty;
                        foreach (var line in infoRow.SplitToLines())
                        {
                            if (line.StartsWith("> CONTEXT"))
                            {
                                context.Add(line);
                            }
                        }

                        //String add block
                        LinesToWrite.Add("> BEGIN STRING");
                        LinesToWrite.Add(original);
                        LinesToWrite.Add(string.Join(Environment.NewLine, context));
                        LinesToWrite.Add(translation);
                        LinesToWrite.Add("> END STRING");
                    }

                    var path = Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ProjectData.ProjectWorkDir) + "_patch", "patch", table.TableName);
                    File.WriteAllLines(path, LinesToWrite);
                    ret = true;
                }
                catch
                {
                }
            }

            return ret;
        }
    }
}

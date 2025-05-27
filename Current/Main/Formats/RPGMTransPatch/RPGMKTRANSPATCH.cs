using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMTrans;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class RPGMKTRANSPATCH : RPGMTransPatchBase
    {
        public RPGMKTRANSPATCH(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override string PatchFileID() // using for write rpgmaker trans patch v3
        {
            return "> RPGMAKER TRANS PATCH FILE VERSION 3.2";
        }

        protected override bool TryOpen()
        {
            var format = new TXTv3(ParentProject);
            bool result = format.Open(this.FilePath);
            if (!result)
            {
                return false;
            }

            this.Data = format.Data;
            this.Info = format.Info;

            return true;
        }

        protected override bool TrySave()
        {
            return WritePatchV3();
        }

        private static bool WritePatchV3()
        {
            var ret = false;
            List<string> LinesToWrite = new List<string>
            {
                "> RPGMAKER TRANS PATCH FILE VERSION 3.2"//v3
            };

            var TablesCount = AppData.CurrentProject.FilesContent.Tables.Count;
            for (int t = 0; t < TablesCount; t++)
            {
                try
                {
                    var table = AppData.CurrentProject.FilesContent.Tables[t];
                    var tableRowsCount = table.Rows.Count;
                    for (int r = 0; r < tableRowsCount; r++)
                    {
                        var row = table.Rows[r];

                        var original = row.Field<string>(AppData.CurrentProject.OriginalColumnIndex);
                        var translation = row.Field<string>(AppData.CurrentProject.TranslationColumnIndex);

                        List<string> context = new List<string>();
                        var infoRow = AppData.CurrentProject.FilesContentInfo.Tables[t].Rows[r] + string.Empty;
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

                    var path = Path.Combine(AppData.CurrentProject.ProjectWorkDir, Path.GetFileName(AppData.CurrentProject.ProjectWorkDir) + "_patch", "patch", table.TableName);
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

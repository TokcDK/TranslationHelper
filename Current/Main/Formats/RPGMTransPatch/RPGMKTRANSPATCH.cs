using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.Abstractions;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class RPGMKTRANSPATCH : RPGMTransPatchBase
    {
        public RPGMKTRANSPATCH(IFormatHost host) : base(host)
        {
        }

        protected override string PatchFileID() // using for write rpgmaker trans patch v3
        {
            return "> RPGMAKER TRANS PATCH FILE VERSION 3.2";
        }

        protected override bool TryOpen()
        {
            var format = new TXTv3(Host);
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

        private bool WritePatchV3()
        {
            var ret = false;
            List<string> LinesToWrite = new List<string>
            {
                "> RPGMAKER TRANS PATCH FILE VERSION 3.2"//v3
            };

            var TablesCount = Host.FilesContent.Tables.Count;
            for (int t = 0; t < TablesCount; t++)
            {
                try
                {
                    var table = Host.FilesContent.Tables[t];
                    var tableRowsCount = table.Rows.Count;
                    for (int r = 0; r < tableRowsCount; r++)
                    {
                        var row = table.Rows[r];

                        var original = row.Field<string>(Host.OriginalColumnIndex);
                        var translation = row.Field<string>(Host.TranslationColumnIndex);

                        List<string> context = new List<string>();
                        var infoRow = Host.FilesContentInfo.Tables[t].Rows[r] + string.Empty;
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

                    var path = Path.Combine(Host.ProjectWorkDir, Path.GetFileName(Host.ProjectWorkDir) + "_patch", "patch", table.TableName);
                    File.WriteAllLines(path, LinesToWrite);
                    ret = true;
                }
                catch (Exception ex)
                {
                    Logger.Warn($"{GetType().Name}: failed to write patch file: {ex.Message}");
                }
            }

            return ret;
        }
    }
}

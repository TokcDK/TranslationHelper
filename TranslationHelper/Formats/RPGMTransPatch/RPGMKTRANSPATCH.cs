using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class Rpgmktranspatch : RpgmTransPatchBase
    {
        public Rpgmktranspatch()
        {
        }

        internal override bool Open() 
        {
            return new TranslationHelper.Formats.RPGMTrans.Txt().Open(); 
        }

        internal override bool Save() 
        {
            return WritePatchV3();
        }

        private bool WritePatchV3()
        {
            var ret = false;
            List<string> linesToWrite = new List<string>
            {
                "> RPGMAKER TRANS PATCH FILE VERSION 3.2"//v3
            };

            var tablesCount = ProjectData.ThFilesElementsDataset.Tables.Count;
            for (int t=0; t < tablesCount; t++)
            {
                try
                {
                    var table = ProjectData.ThFilesElementsDataset.Tables[t];
                    var tableRowsCount = table.Rows.Count;
                    for (int r = 0; r < tableRowsCount; r++)
                    {
                        var row = table.Rows[r];

                        var original = row[0] as string;
                        var translation = row[1] as string;

                        List<string> context = new List<string>();
                        var infoRow = ProjectData.ThFilesElementsDatasetInfo.Tables[t].Rows[r] + string.Empty;
                        foreach (var line in infoRow.SplitToLines())
                        {
                            if (line.StartsWith("> CONTEXT"))
                            {
                                context.Add(line);
                            }
                        }

                        //String add block
                        linesToWrite.Add("> BEGIN STRING");
                        linesToWrite.Add(original);
                        linesToWrite.Add(string.Join(Environment.NewLine, context));
                        linesToWrite.Add(translation);
                        linesToWrite.Add("> END STRING");
                    }

                    var path = Path.Combine(ProjectData.ProjectWorkDir, Path.GetFileName(ProjectData.ProjectWorkDir) + "_patch", "patch", table.TableName);
                    File.WriteAllLines(path, linesToWrite);
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

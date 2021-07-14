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
    class RPGMKTRANSPATCH : RPGMTransPatchBase
    {
        public RPGMKTRANSPATCH(ProjectData projectData) : base(projectData)
        {
        }

        internal override bool Open() 
        {
            return new TranslationHelper.Formats.RPGMTrans.TXT(projectData).Open(); 
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

            var TablesCount = projectData.THFilesElementsDataset.Tables.Count;
            for (int t=0; t < TablesCount; t++)
            {
                try
                {
                    var table = projectData.THFilesElementsDataset.Tables[t];
                    var tableRowsCount = table.Rows.Count;
                    for (int r = 0; r < tableRowsCount; r++)
                    {
                        var row = table.Rows[r];

                        var original = row[0] as string;
                        var translation = row[1] as string;

                        List<string> context = new List<string>();
                        var infoRow = projectData.THFilesElementsDatasetInfo.Tables[t].Rows[r] + string.Empty;
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

                    var path = Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(Properties.Settings.Default.THProjectWorkDir) + "_patch", "patch", table.TableName);
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

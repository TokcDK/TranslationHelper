using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;

namespace TranslationHelper.Main.Functions
{
    class FunctionsDBFile
    {
        public static void WriteTranslationCacheIfValid(DataSet THTranslationCache, string tHTranslationCachePath)
        {
            if (Properties.Settings.Default.IsTranslationCacheEnabled && !Properties.Settings.Default.IsTranslationHelperWasClosed && THTranslationCache.Tables[0].Rows.Count > 0)
            {
                FunctionsDBFile.WriteDBFile(THTranslationCache, tHTranslationCachePath);
                //THTranslationCache.Reset();
            }
        }

        //https://stackoverflow.com/questions/223738/net-stream-dataset-of-xml-data-to-zip-file
        //http://madprops.org/blog/saving-datasets-locally-with-compression/
        public static void ReadDBFile(DataSet DS, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Stream s;
                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Decompress);
                }
                else if (fileExtension == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Decompress);
                }
                else
                {
                    s = fs;
                }
                DS.ReadXml(s);
                s.Close();
            }
        }

        public static void WriteDBFile(DataSet DS, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                Stream s;
                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Compress);
                }
                else if (fileExtension == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Compress);
                }
                else
                {
                    s = fs;
                }
                DS.WriteXml(s);
                s.Close();
            }
        }

        internal static string GetDBCompressionExt(THDataWork thDataWork)
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked") == "True")
            {
                //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression"));
                if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "XML (none)")
                {
                    //MessageBox.Show(".xml");
                    return ".xml";
                }
                else if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Gzip (cmx)")
                {
                    //MessageBox.Show(".cmx");
                    return ".cmx";
                }
                else if (thDataWork.Main.Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Deflate (cmz)")
                {
                    //MessageBox.Show(".cmz");
                    return ".cmz";
                }

            }
            //MessageBox.Show("Default .xml");
            return ".xml";
        }

        internal static string GetProjectDBFolder()
        {
            string ret = string.Empty;
            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker MV"))
            {
                ret = "RPGMakerMV";
            }
            else if (RPGMFunctions.THSelectedSourceType.Contains("RPGMaker") || RPGMFunctions.THSelectedSourceType.Contains("RPG Maker"))
            {
                ret = "RPGMakerTransPatch";
            }
            return Path.Combine(Application.StartupPath, "DB", ret);
        }

        internal static string GetDBFileName(THDataWork thDataWork, bool IsSaveAs = false)
        {
            string fName = Path.GetFileName(Properties.Settings.Default.THSelectedDir);
            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker MV"))
            {
                if (thDataWork.Main.THFilesList.Items.Count == 1 && thDataWork.Main.THFilesList.Items[0] != null && !string.IsNullOrWhiteSpace(thDataWork.Main.THFilesList.Items[0].ToString()))
                {
                    if (fName == "data")
                    {
                        fName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(Properties.Settings.Default.THSelectedDir))) + "_" + Path.GetFileNameWithoutExtension(thDataWork.Main.THFilesList.Items[0].ToString());
                    }
                    else
                    {
                        fName = Path.GetFileNameWithoutExtension(thDataWork.Main.THFilesList.Items[0].ToString());
                    }
                }
            }
            //else if (THSelectedSourceType.Contains("RPGMaker") || THSelectedSourceType.Contains("RPG Maker"))
            //{

            //}
            return fName + (IsSaveAs ? "_" + DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.GetCultureInfo("en-US")) : string.Empty);
        }
    }
}

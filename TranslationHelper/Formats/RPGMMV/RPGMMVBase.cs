using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JS;

namespace TranslationHelper.Formats.RPGMMV
{
    abstract class RPGMMVBase : FormatBase
    {
        public RPGMMVBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal void MakeFilesBuckup()
        {
            RestoreFromBakIfNeedData();
            try
            {
                string dataPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data");
                CopyFolder.Copy(dataPath, dataPath + "_bak");
            }
            catch
            {
            }
            foreach (JSBase JS in new JSSharedData(thDataWork).ListOfJS)
            {
                try
                {
                    string jsPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);
                    RestoreFromBakIfNeedJS(JS);
                    File.Copy(jsPath, jsPath + ".bak");
                }
                catch
                {
                }
            }
        }

        internal void RestoreFromBakIfNeed()
        {
            RestoreFromBakIfNeedData();
            foreach (JSBase JS in new JSSharedData(thDataWork).ListOfJS)
            {
                RestoreFromBakIfNeedJS(JS);
            }
        }

        internal static void RestoreFromBakIfNeedData()
        {
            string dataPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data");
            if (Directory.Exists(dataPath + "_bak"))
            {
                try
                {
                    if (Directory.Exists(dataPath))
                    {
                        Directory.Delete(dataPath, true);
                    }

                    Directory.Move(dataPath + "_bak", dataPath);
                }
                catch
                {
                }
            }
        }

        internal static void RestoreFromBakIfNeedJS(JSBase JS)
        {
            string jsPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);
            if (File.Exists(jsPath + ".bak"))
            {
                try
                {
                    if (File.Exists(jsPath))
                    {
                        File.Delete(jsPath);
                    }

                    File.Move(jsPath + ".bak", jsPath);
                }
                catch
                {
                }
            }
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TranslationHelper.Functions
{
    class FunctionsLogs
    {
        //readonly bool THdebug = true;
        StringBuilder THsbLog;// = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TextToLog">TextToLog</param>
        /// <param name="WriteNow">If True then text will be now writed to log else only added to be writed later<br/></param>
        public void LogToFile(string TextToLog, bool WriteNow = true)
        {
            if (THsbLog == null)
            {
                THsbLog = new StringBuilder();
            }
            if (Properties.Settings.Default.THdebug)
            {
                if (WriteNow)
                {
                    if (THsbLog.Length == 0)
                    {
                        FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName + ".log"), DateTime.Now + " >>" + TextToLog + Environment.NewLine, Properties.Settings.Default.DebugMode);
                    }
                    else
                    {
                        FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName + ".log"), DateTime.Now + " >>" + THsbLog + Environment.NewLine + TextToLog + Environment.NewLine, Properties.Settings.Default.DebugMode);
                        //File.Move(Application.StartupPath + "\\TranslationHelper.log", Application.StartupPath + "\\TranslationHelper" + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss") + ".log");
                        THsbLog.Clear();
                    }
                }
                else
                {
                    THsbLog.Append(DateTime.Now + " >>" + TextToLog + Environment.NewLine);
                }
            }
        }
    }
}

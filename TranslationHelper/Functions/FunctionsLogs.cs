using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TranslationHelper.Functions
{
    class FunctionsLogs
    {
        //readonly bool THdebug = true;
        StringBuilder THsbLog;// = new StringBuilder();

        public FunctionsLogs()
        {
            DebugData = new List<string>();
        }

        /// <summary>
        /// some debug data can be added here
        /// </summary>
        public List<string> DebugData { get; internal set; }

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
                        FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName + ".log")
                            , Environment.NewLine
                            + DateTime.Now + " >>" + TextToLog
                            + Environment.NewLine
                            + (DebugData.Count > 0 ? "Debug data:" + Environment.NewLine + string.Join(Environment.NewLine, DebugData) : "")
                            , Properties.Settings.Default.DebugMode);
                    }
                    else
                    {
                        FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName + ".log")
                            , Environment.NewLine
                            + DateTime.Now + " >>" + THsbLog
                            + Environment.NewLine
                            + TextToLog
                            + Environment.NewLine
                            + (DebugData.Count > 0 ? "Debug data:" + Environment.NewLine + string.Join(Environment.NewLine, DebugData) : "")
                            , Properties.Settings.Default.DebugMode);
                        //File.Move(Application.StartupPath + "\\TranslationHelper.log", Application.StartupPath + "\\TranslationHelper" + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss") + ".log");
                        THsbLog.Clear();
                    }

                    DebugData.Clear();//clear debug data after they was logged
                }
                else
                {
                    THsbLog.Append(DateTime.Now + " >>" + TextToLog + Environment.NewLine);
                }
            }
        }
    }
}

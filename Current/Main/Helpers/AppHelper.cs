using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.IO;

namespace TranslationHelper.Helpers
{
    internal class AppHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static void OpenCurrentFileLogFile()
        {
            var logFilePath = Logger.Factory.Configuration.FindTargetByName<FileTarget>("file")?.FileName.Render(new LogEventInfo());
            if (logFilePath != null && File.Exists(logFilePath))
            {
                System.Diagnostics.Process.Start("explorer.exe", logFilePath);
            }
            else
            {
                Logger.Info("Logger file is not found", logFilePath);
            }
        }

        internal static void SetupLogging(FormMain mainForm)
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            string generalLayout = "${longdate} (${level:uppercase=true}): ${message}";
            var rtbTarget = new RichTextBoxTarget()
            {
                Name = "ui",
                ControlName = mainForm.rtbLog.Name,
                FormName = mainForm.Name,
                MaxLines = 500,
                AutoScroll = true,
                UseDefaultRowColoringRules = true,
                Layout = generalLayout
            };

            var fileTarget = new FileTarget("file")
            {
                FileName = "Logs\\${date:yyyy-MM-dd}.txt",
                MaxArchiveDays = 10,
                Layout = generalLayout
            };

            config.AddTarget("ui", rtbTarget);
            config.AddTarget("file", fileTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, "ui");
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, "file");

            LogManager.Configuration = config;
        }
    }
}

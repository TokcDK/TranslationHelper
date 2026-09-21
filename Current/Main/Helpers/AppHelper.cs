using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using NLog;
using System;
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
                // the path has to be a message-template placeholder, otherwise NLog drops it
                Logger.Info("Logger file is not found: {0}", logFilePath);
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

                // The target is deliberately left to write every line in the colour the box already
                // has, rather than colouring each line itself. Its own rules name one colour per level
                // and no theme can reach them — it draws info in black, which is invisible on the dark
                // background and is what made the first line unreadable. Without rules the text takes
                // the box's ForeColor, which the theme sets, so the log is legible in both themes and
                // follows the theme when it changes: assigning ForeColor to a rich text box recolours
                // what is already in it, so the lines written before the theme was applied are put
                // right by the same change.
                UseDefaultRowColoringRules = false,
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

using System;
using System.Windows.Forms;

namespace TranslationHelper
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //try
            //{
            Application.Run(new FormMain());
            //}
            //catch (Exception ex)
            //{
            //    new Functions.FunctionsLogs().LogToFile(Environment.NewLine + "Main appplication error occured. Error text:" + Environment.NewLine + ex + Environment.NewLine);
            //}
        }
    }
}
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
#pragma warning disable CA2000 // Ликвидировать объекты перед потерей области
            var main = (new THMain());
            try
            {
                Application.Run(main);
            }
            catch (Exception ex)
            {
                main.LogToFile(Environment.NewLine + "Main appplication error occured. Error text:" + Environment.NewLine + ex + Environment.NewLine);
            }
#pragma warning restore CA2000 // Ликвидировать объекты перед потерей области
        }
    }
}
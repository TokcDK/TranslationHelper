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
            Application.Run(new THMain());
#pragma warning restore CA2000 // Ликвидировать объекты перед потерей области
        }
    }
}
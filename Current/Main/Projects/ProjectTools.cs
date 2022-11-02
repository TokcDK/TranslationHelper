using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Projects
{
    internal static class ProjectTools
    {
        /// <summary>
        /// exe files *.exe
        /// </summary>
        internal static string GameExeFilter { get => "Game execute|\"*.exe\""; }
        /// <summary>
        /// return if selected file of project is exe
        /// </summary>
        /// <returns></returns>
        internal static bool IsExe(string path)
        {
            return string.Equals(Path.GetExtension(path), ".exe", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

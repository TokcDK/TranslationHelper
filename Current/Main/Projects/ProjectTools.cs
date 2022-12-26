using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Functions;

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
        /// <summary>
        /// Get all types of inherited classes of ProjectBase class
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetListOfProjectTypes()
        {
            return GetListOfSubClasses.Inherited.GetListOfInheritedTypes(typeof(ProjectBase));
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        public static List<ProjectBase> GetListOfProjects()
        {
            return GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<ProjectBase>();
        }
    }
}

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

        /// <summary>
        /// Check all parent dirst is exists in list of paths
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listOfPaths"></param>
        /// <returns></returns>
        public static bool IsAnyParentPathInTheList(this string path, List<string> listOfPaths)
        {
            while ((path = Path.GetDirectoryName(path)) != null)
            {
                if (listOfPaths.Contains(path)) return true;
            }

            return false;
        }
        public static List<FileInfo> GetNewestFilesList(DirectoryInfo dir, string mask = "*.*")
        {
            var newestfiles = new Dictionary<string, FileInfo>();

            foreach (var file in dir.EnumerateFiles(mask, SearchOption.AllDirectories))
            {
                var name = file.Name;
                bool isAlreadyContains = newestfiles.ContainsKey(name);
                if (isAlreadyContains)
                {
                    if (file.LastWriteTime <= newestfiles[name].LastWriteTime) continue;

                    newestfiles[name] = file;
                }
                else
                {
                    newestfiles.Add(name, file);
                }
            }

            return newestfiles.Values.ToList();
        }
    }
}

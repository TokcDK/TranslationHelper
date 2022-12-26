using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Projects
{
    public static class ProjectUtils
    {
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
            while((path = Path.GetDirectoryName(path)) != null)
            {
                if (listOfPaths.Contains(path)) return true;
            }

            return false;
        }
    }
}

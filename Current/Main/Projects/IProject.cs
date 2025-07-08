using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Projects
{
    interface IProject
    {
        /// <summary>
        /// Project title
        /// </summary>
        /// <returns></returns>
        string Name { get; }
        /// <summary>
        /// Open project files
        /// </summary>        
        /// <returns></returns>
        bool Open();

        /// <summary>
        /// Save project files
        /// </summary>        
        /// <returns></returns>
        bool Save(HashSet<int> fileIndexToSave = null);
    }
}

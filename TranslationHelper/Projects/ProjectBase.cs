using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Projects
{
    internal abstract class ProjectBase
    {
        protected THDataWork thDataWork;

        protected ProjectBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        /// <summary>
        /// Conditions to detect on open
        /// </summary>
        /// <returns></returns>
        internal abstract bool OpenDetect();

        /// <summary>
        /// Project title
        /// </summary>
        /// <returns></returns>
        internal abstract string ProjectTitle();

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjecFolderName()
        {
            return "Other";
        }

        /// <summary>
        /// Open project files
        /// </summary>
        /// <param name="thData"></param>
        /// <returns></returns>
        internal abstract bool Open();

        /// <summary>
        /// Save project files
        /// </summary>
        /// <param name="thData"></param>
        /// <returns></returns>
        internal abstract bool Save();
    }
}

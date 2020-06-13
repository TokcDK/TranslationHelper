using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

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
        /// Returns project's DB file name for save/load
        /// </summary>
        /// <returns></returns>
        internal virtual string GetProjectDBFileName()
        {
            return string.Empty;
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

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string NewlineSymbol => Environment.NewLine;

        internal virtual bool IsTestRunEnabled => false;

        /// <summary>
        /// Must make buckup of project translating original files<br/>if any code exit here else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BuckupCreate()
        {
            return false;
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BuckupRestore()
        {
            return false;
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<ProjectBase> GetListOfProjects(THDataWork thDataWork)
        {
            //ProjectsList = new List<ProjectBase>()
            //{
            //    new RPGMTransPatch(this)
            //    ,
            //    new RPGMGame(this)
            //    ,
            //    new RPGMMVGame(this)
            //    ,
            //    new KiriKiriGame(this)
            //    ,
            //    new Raijin7Game(this)
            //    ,
            //    new HowToMakeTrueSlavesRiseofaDarkEmpire(this)
            //};

            //https://stackoverflow.com/a/5411981
            //Get all inherited classes of an abstract class
            IEnumerable<ProjectBase> SubclassesOfProjectBase = typeof(ProjectBase)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ProjectBase)) && !t.IsAbstract)
            .Select(t => (ProjectBase)Activator.CreateInstance(t, thDataWork));

            return (from ProjectBase SubClass in SubclassesOfProjectBase
                    select SubClass).ToList();
        }

        internal void FillTHFilesElementsDictionary()
        {
            if (thDataWork.TablesLinesDict == null)
            {
                foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string orig;
                        if (!string.IsNullOrEmpty(orig = row[0] + string.Empty) && !thDataWork.THFilesElementsDictionary.ContainsKey(orig))
                        {
                            thDataWork.THFilesElementsDictionary.Add(orig, row[1] + string.Empty);
                        }
                    }
                }
            }
        }

        internal void FillTablesLinesDict()
        {
            bool notnull;
            if ((notnull = thDataWork.TablesLinesDict != null) && thDataWork.TablesLinesDict.Count > 0)
            {
                return;
            }
            else if (!notnull)
            {
                thDataWork.TablesLinesDict = new Dictionary<string, string>();
            }

            foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    string orig;
                    string trans;
                    if (!string.IsNullOrWhiteSpace(orig = row[0] + string.Empty) && !string.IsNullOrEmpty(trans = row[1] + string.Empty))
                    {
                        thDataWork.TablesLinesDict.AddTry(orig, trans);

                        if (!trans.StartsWith(@"\n<>") && Regex.IsMatch(orig, @"\\n<.+>[\s\S]*$"))
                        {
                            orig = Regex.Replace(orig, @"\\n<(.+)>[\s\S]*$", "$1");
                            trans = Regex.Replace(trans, @"\\n<(.+)>[\s\S]*$", "$1");
                            if (orig != trans)
                            {
                                thDataWork.TablesLinesDict.AddTry(
                                orig
                                ,
                                trans
                                );
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hardcoded fixes for cells for specific projects
        /// </summary>
        /// <returns></returns>
        internal virtual string HardcodedFixes(string original, string translation)
        {
            return translation;
        }

    }
}

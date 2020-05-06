﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string NewlineSymbol => Environment.NewLine;

        internal virtual bool IsTestRunEnabled => false;

        /// <summary>
        /// here is project can place code buckup of translating original files for test run
        /// </summary>
        internal virtual void MakeFilesBuckup() { }

        /// <summary>
        /// here is project can place code restore buckup of translating original files after test run
        /// </summary>
        internal virtual void RestoreFromBakIfNeed() { }

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
            foreach(DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                foreach(DataRow row in table.Rows)
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
}

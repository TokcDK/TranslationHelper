using System;
using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    internal static class SettingsBaseTools
    {
        internal static Dictionary<string, SettingsBase> GetSettingsList(THDataWork thDataWork)
        {
            var listOfSubclasses = GetListOfSubClasses<SettingsBase>(thDataWork);
            return GetSettingsList(listOfSubclasses);
        }

        /// <summary>
        /// Get list of settings classes with 'settingname, class' pair  
        /// </summary>
        /// <param name="listOfSubclasses"></param>
        /// <returns></returns>
        internal static Dictionary<string, SettingsBase> GetSettingsList(List<SettingsBase> listOfSubclasses)
        {
            var SettingsList = new Dictionary<string, SettingsBase>();
            foreach (var Subclass in listOfSubclasses)
            {
                SettingsList.Add(Subclass.ID(), Subclass);
            }
            return SettingsList;
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq version of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        internal static List<T> GetListOfSubClasses<T>(THDataWork parameter)
        {
            var ListOfSubClasses = new List<T>();
            foreach (var ClassType in typeof(T).Assembly.GetTypes())
            {
                if (ClassType.IsSubclassOf(typeof(T)) && !ClassType.IsAbstract)
                {
                    ListOfSubClasses.Add((T)Activator.CreateInstance(ClassType, parameter));
                }
            }

            return ListOfSubClasses;
        }
    }

    abstract class SettingsBase/* : ApplicationSettingsBase*/
    {
        //Dictionary<string, SettingsBase> SettingsList;
        protected THDataWork thDataWork;
        protected SettingsBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal virtual string Section { get => string.Empty; }

        internal abstract string Key { get; }

        internal abstract string Default { get; }
        internal virtual bool DefaultBool { get => false; }
        internal virtual int DefaultInt { get => 0; }

        internal abstract void Set(bool SetObject = false);
        internal abstract string Get();

        internal abstract string ID();

        //protected INIFile THConfigINI = new INIFile(Application.ProductName+".ini");
    }
}

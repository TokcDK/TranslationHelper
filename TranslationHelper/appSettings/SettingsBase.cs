using System;
using System.Collections.Generic;
using System.Linq;
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
        /// non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        internal static List<T> GetListOfSubClasses<T>(params object[] args)
        {
            var ListOfSubClasses = new List<T>();
            var type = typeof(T);
            foreach (var ClassType in type.Assembly.GetTypes())
            {
                if (ClassType.IsSubclassOf(type) && !ClassType.IsAbstract)
                {
                    ListOfSubClasses.Add((T)Activator.CreateInstance(ClassType, args));
                }
            }

            return ListOfSubClasses;
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        internal static List<T> GetListOfSubClasses<T>()
        {
            var ListOfSubClasses = new List<T>();
            var type = typeof(T);
            foreach (var ClassType in type.Assembly.GetTypes())
            {
                if (ClassType.IsSubclassOf(type) && !ClassType.IsAbstract)
                {
                    ListOfSubClasses.Add((T)Activator.CreateInstance(ClassType));
                }
            }

            return ListOfSubClasses;
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq version of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        internal static List<T> GetListOfInterfaceImplimentations<T>()
        {
            var ListOfSubClasses = new List<T>();
            var type = typeof(T);
            foreach (var ClassType in type.Assembly.GetTypes())
            {
                if (ClassType.Name.Contains("CMX"))
                {

                }
                var b = type.IsAssignableFrom(ClassType);
                if (type.IsAssignableFrom(ClassType) && ClassType.IsClass && !ClassType.IsInterface && !ClassType.IsAbstract)
                {
                    ListOfSubClasses.Add((T)Activator.CreateInstance(ClassType));
                }
            }

            //var l = AppDomain.CurrentDomain.GetAssemblies()
            //.SelectMany(s => s.GetTypes())
            //.Where(p => type.IsAssignableFrom(p))
            //.ToList();

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
